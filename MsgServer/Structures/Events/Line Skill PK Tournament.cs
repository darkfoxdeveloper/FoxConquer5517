// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - Line Skill PK Tournament.cs
// Last Edit: 2017/02/20 09:07
// Created: 2017/02/20 08:20

using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;

namespace MsgServer.Structures.Events
{
    public class LineSkillPkTournament
    {
        public DynamicRankRecordRepository Repository = new DynamicRankRecordRepository();

        public const uint MAP_ID_U = 7505;
        private const string _ERROR_FILE = "lineskillpkt_error";
        private const uint _RANK_TYPE_U = 10;

        private EventState m_pState = EventState.ERROR;
        private Map m_pMap;
        private ConcurrentDictionary<uint, LineSkillPkStatistic> m_pStatistic = new ConcurrentDictionary<uint, LineSkillPkStatistic>();
        private TimeOut m_pTableTimeOut = new TimeOut(10);

        public bool Create()
        {
            if (!ServerKernel.Maps.TryGetValue(MAP_ID_U, out m_pMap))
            {
                ServerKernel.Log.SaveLog("ERROR MAP NOT FOUND LINE SKILL PK", true, LogType.ERROR);
                return false;
            }

            var allRank = Repository.FetchByType(_RANK_TYPE_U);
            if (allRank != null)
            {
                foreach (var rank in allRank.Where(x => x.PlayerIdentity > 0))
                {
                    LineSkillPkStatistic obj = new LineSkillPkStatistic(rank);
                    m_pStatistic.TryAdd(rank.PlayerIdentity, obj);
                }
            }
            m_pState = EventState.IDLE;
            return true;
        }

        public bool IsReady
        {
            get { return m_pState > EventState.ERROR; }
        }

        public bool IsRunning
        {
            get { return m_pState == EventState.RUNNING; }
        }

        public void OnTimer()
        {
            if (m_pState == EventState.ERROR)
                return;

            DateTime now = DateTime.Now;
            int nNow = int.Parse(now.ToString("HHmmss"));

            switch (m_pState)
            {
                case EventState.IDLE:
                {
                    if (nNow%10000 < 1000 || nNow%10000 >= 2000)
                        return;
                    // todo check what's needed before event start
                    m_pState = EventState.STARTING;
                    break;
                }
                case EventState.STARTING:
                {
                    foreach (var plr in m_pStatistic.Values)
                    {
                        plr.HitsDealtNow = 0;
                        plr.HitsTakenNow = 0;
                    }
                    ServerKernel.SendMessageToAll("Line Skill PK Tournament has started.", ChatTone.TALK);
                    m_pState = EventState.RUNNING;
                    break;
                }
                case EventState.RUNNING:
                {
                    if (nNow%10000 >= 1000
                        && nNow%10000 < 2000)
                    {
                        if (m_pTableTimeOut.ToNextTime())
                            SendTable();
                    }
                    else
                    {
                        m_pState = EventState.FINISHING;
                    }
                    break;
                }
                case EventState.FINISHING:
                {
                    ServerKernel.SendMessageToAll("The Line Skill PK Tournament has ended.", ChatTone.TOP_LEFT);
                    foreach (var plr in m_pStatistic.Values.OrderByDescending(x => x.KDA)
                        .ThenByDescending(x => x.HitsDealtNow)
                        .ThenBy(x => x.HitsTakenNow)
                        .Where(x => x.HitsDealtNow > 0))
                    {
                        uint gold = 0;
                        uint emoney = 0;
                        try
                        {
                            switch (GetRank(plr.Identity))
                            {
                                case 1:
                                    gold = ServerKernel.LineSkillGoldReward[0];
                                    emoney = ServerKernel.LineSkillEmoneyReward[0];
                                    ServerKernel.SendMessageToAll(
                                        string.Format("Congratulations! {0} has won the Line Skill PK Tournament.",
                                            plr.Name),
                                        ChatTone.TALK);
                                    break;
                                case 2:
                                    gold = ServerKernel.LineSkillGoldReward[1];
                                    emoney = ServerKernel.LineSkillEmoneyReward[1];
                                    ServerKernel.SendMessageToAll(
                                        string.Format("{0} has taken the 2nd place in the Line Skill PK Tournament.",
                                            plr.Name),
                                        ChatTone.TALK);
                                    break;
                                case 3:
                                    gold = ServerKernel.LineSkillGoldReward[2];
                                    emoney = ServerKernel.LineSkillEmoneyReward[2];
                                    ServerKernel.SendMessageToAll(
                                        string.Format("{0} has taken the 3rd place in the Line Skill PK Tournament.",
                                            plr.Name),
                                        ChatTone.TALK);
                                    break;
                                default:
                                    gold = ServerKernel.LineSkillGoldReward[3];
                                    emoney = ServerKernel.LineSkillEmoneyReward[3];
                                    break;
                            }
                        }
                        catch
                        {
                            ServerKernel.Log.SaveLog("ERROR MESSAGE GET PRIZE LINE SKILL", true, LogType.ERROR);
                        }

                        Client player;
                        if (ServerKernel.Players.TryGetValue(plr.Identity, out player))
                        {
                            if (player.Character == null)
                                continue; //?

                            player.Character.AwardMoney(gold);
                            player.Character.AwardEmoney(emoney);
                            player.Character.Send(string.Format("You received {0} silvers and {1} CPs for playing on the Line Skill PK Tournament.",
                                    gold, emoney));
                        }
                        else
                        {
                            DbUser user = Database.Characters.SearchByIdentity(plr.Identity);
                            if (user == null)
                                continue;
                            if (user.Money + gold > int.MaxValue)
                                user.Money = int.MaxValue;
                            else
                                user.Money += gold;
                            if (user.Emoney + emoney > int.MaxValue)
                                user.Emoney = int.MaxValue;
                            else
                                user.Emoney += emoney;
                            Database.Characters.SaveOrUpdate(user);
                        }
                    }

                    m_pState = EventState.ENDED;
                    break;
                }
                case EventState.ENDED:
                {
                    foreach (var plr in m_pMap.Players.Values)
                    {
                        plr.ChangeMap(plr.RecordMapX, plr.RecordMapY, plr.RecordMapIdentity);
                    }

                    ServerKernel.SendMessageToAll("Line Skill PK Tournament has ended.", ChatTone.TOP_LEFT);
                    m_pState = EventState.IDLE;
                    break;
                }
            }
        }

        public int GetRank(uint idMember)
        {
            int pos = 1;
            foreach (var pls in m_pStatistic.Values.OrderByDescending(x => x.KDA)
                .ThenByDescending(x => x.HitsDealtNow)
                .ThenBy(x => x.HitsTakenNow))
            {
                if (pls.Name.Contains("[GM]") || pls.Name.Contains("[PM]"))
                    continue;
                if (pls.Identity == idMember)
                    return pos;
                pos++;
            }
            return 999;
        }

        private void AwardPoint(Character pRole, int point)
        {
            LineSkillPkStatistic stt;
            if (!m_pStatistic.TryGetValue(pRole.Identity, out stt))
            {
                stt = new LineSkillPkStatistic(pRole);
                m_pStatistic.TryAdd(pRole.Identity, stt);
            }

            if (point > 0)
            {
                stt.HitsDealt += 1;
                stt.HitsDealtNow += 1;
            }
            else if (point < 0)
            {
                stt.HitsTaken += 1;
                stt.HitsTakenNow += 1;
            }
        }

        public void Hit(Character pAtker, Character pTarget)
        {
            if (pAtker == null || pTarget == null || pTarget == pAtker)
                return;
            
            AwardPoint(pAtker, 1);
            AwardPoint(pTarget, -1);

            LineSkillPkStatistic target = m_pStatistic.Values.FirstOrDefault(x => x.Identity == pTarget.Identity);

            if (target?.HitsTakenNow >= 10)
            {
                pTarget.ChangeMap(430, 378, 1002);
                pTarget.Send("You've been hit 10 times and has been disqualified.");
            }
        }

        public bool IsEnterEnable(Character pTarget)
        {
            LineSkillPkStatistic target = m_pStatistic.Values.FirstOrDefault(x => x.Identity == pTarget.Identity);
            if (target == null) return true; // ??
            return target.HitsTakenNow < 10;
        }

        public void SendTable()
        {
            m_pMap.SendMessageToMap("Line Skill Tournament - DT(Dealt/Taken)", ChatTone.EVENT_RANKING);
            int nRank = 0;
            foreach (var plr in m_pStatistic.Values.OrderByDescending(x => x.KDA))
            {
                if (nRank++ >= 8)
                    break;
                m_pMap.SendMessageToMap(string.Format("Nº {0} - {1,16} - {2:0.00}({3}/{4})", nRank, plr.Name, plr.KDA,
                    plr.HitsDealtNow, plr.HitsTakenNow), ChatTone.EVENT_RANKING_NEXT);
            }
            foreach (var plr in m_pStatistic.Values)
            {
                Client pClient;
                if (ServerKernel.Players.TryGetValue(plr.Identity, out pClient) && pClient.Character.MapIdentity == MAP_ID_U)
                {
                    pClient.SendMessage(string.Format("You: {0:0.00}({1},{2})", plr.KDA, plr.HitsDealtNow,
                        plr.HitsTakenNow), ChatTone.EVENT_RANKING_NEXT);
                    pClient.SendMessage(string.Format("Lifetime Statistic: {0:0.00}({1},{2})", plr.LifetimeKDA, plr.HitsDealt,
                        plr.HitsTaken), ChatTone.EVENT_RANKING_NEXT);
                }
            }
        }

        public void GetRebornPos(out Point pos)
        {
            pos = new Point();
            pos.X = (ushort) ThreadSafeRandom.RandGet(35, 70);
            pos.Y = (ushort) ThreadSafeRandom.RandGet(35, 70);
        }
    }
}