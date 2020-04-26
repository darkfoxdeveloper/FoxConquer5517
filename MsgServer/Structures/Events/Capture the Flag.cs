// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Capture the Flag.cs
// Last Edit: 2017/01/18 18:28
// Created: 2016/12/29 21:32

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Enums;
using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Society;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Events
{
    public sealed class CaptureTheFlag
    {
        private DynamicRankRecordRepository m_pRepo;

        private Dictionary<uint, DbDynamicRankRecord> m_pRanking = new Dictionary<uint, DbDynamicRankRecord>();
        private Dictionary<uint, DbDynamicRankRecord> m_pUserRank = new Dictionary<uint, DbDynamicRankRecord>();
        private List<Point> m_pValidTiles = new List<Point>();

        private const uint _MAP_ID_U = 2057;
        private const uint _FLAG_MESH = 513;
        private const int _FLAG_MAX_AMOUNT = 30;
        private const int _STARTUP_TIME = 6200000;
        private const int _END_TIME = 6210000;
        private const string _ERROR_FILE = "ctferror";
        private const uint _RANK_TYPE_U = 6;

        private TimeOutMS m_tFlagGen = new TimeOutMS(2000);
        private TimeOut m_tRankSend = new TimeOut(10);

        private Map m_pMap;
        private EventState m_pState = EventState.IDLE;

        public CaptureTheFlag()
        {
            m_pRepo = new DynamicRankRecordRepository();
            m_tFlagGen.Clear();
        }

        public bool Create()
        {
            if (!ServerKernel.Maps.TryGetValue(_MAP_ID_U, out m_pMap))
            {
                ServerKernel.Log.GmLog(_ERROR_FILE, "Could not load map on startup");
                m_pState = EventState.ENDED;
                return false;
            }

            if (!ParseMap())
                return false;

            var allRank = m_pRepo.FetchByType(_RANK_TYPE_U);
            if (allRank != null)
            {
                foreach (var rank in allRank.Where(x => x.PlayerIdentity == 0))
                {
                    m_pRanking.Add(rank.ObjectIdentity, rank);
                }
                foreach (var rank in allRank.Where(x => x.PlayerIdentity > 0))
                {
                    m_pUserRank.Add(rank.PlayerIdentity, rank);
                }
            }

            int weekday = (int)(DateTime.Now.DayOfWeek) % 7 == 0 ? 7 : (int)(DateTime.Now.DayOfWeek);
            int now = int.Parse(DateTime.Now.ToString("hhmmss")) + weekday * 1000000;
            //if (m_pState == EventState.IDLE && now >= _STARTUP_TIME && now < _END_TIME)
            //{
            //    m_bHasStarted = true;
            //}
            //else 
            //{
            //    foreach (var rank in m_pRanking.Values)
            //    {
            //        rank.Value1 = 0;
            //        rank.Value3 = 0;
            //        Repository.SaveOrUpdate(rank);
            //    }
            //    foreach (var rank in m_pUserRank.Values)
            //    {
            //        rank.Value1 = 0;
            //        rank.Value3 = 0;
            //        Repository.SaveOrUpdate(rank);
            //    }
            //}

            return true;
        }

        public bool ParseMap()
        {
            try
            {
                if (!m_pMap.Loaded)
                    m_pMap.Load();

                for (int x = 0; x < m_pMap.Width; x++)
                {
                    for (int y = 0; y < m_pMap.Height; y++)
                    {
                        if (m_pMap[x,y].Access == TileType.AVAILABLE)
                            m_pValidTiles.Add(new Point(x, y));
                    }
                }

                m_pMap.SetStatus(1, true);
                return true;
            }
            catch
            {
                ServerKernel.Log.SaveLog("CTF: could not parse map coordinates", true, LogType.ERROR);
                return false;
            }
        }

        public bool IsActive
        {
            get { return m_pState != EventState.ENDED; }
        }

        public bool IsRunning
        {
            get { return m_pState == EventState.RUNNING; }
        }

        public void OnTimer()
        {
            int weekday = (int) (DateTime.Now.DayOfWeek)%7 == 0 ? 7 : (int) (DateTime.Now.DayOfWeek);
            int now = int.Parse(DateTime.Now.ToString("HHmmss")) + weekday*1000000;
            if (m_pState == EventState.IDLE && now >= _STARTUP_TIME && now < _END_TIME)
            {
                GenerateFlags();

                foreach (var rank in m_pRanking.Values)
                {
                    rank.Value1 = 0;
                    rank.Value3 = 0;
                    m_pRepo.SaveOrUpdate(rank);
                }
                foreach (var rank in m_pUserRank.Values)
                {
                    rank.Value1 = 0;
                    rank.Value3 = 0;
                    m_pRepo.SaveOrUpdate(rank);
                }

                m_pState = EventState.RUNNING;
            } 
            else if (m_pState == EventState.RUNNING)
            {
                if (m_tFlagGen.ToNextTime())
                    GenerateFlags();

                if (m_tRankSend.ToNextTime())
                {
                    // todo send ctf rank
                    List<string> pRank = new List<string>(9);
                    pRank.Add("Capture The Flag");
                    int count = 0;
                    foreach (var rnk in m_pRanking.Values.OrderByDescending(x => x.Value1))
                    {
                        if (rnk.Value1 > 0)
                        {
                            if (count++ < 8)
                                pRank.Add(string.Format("Nº{0}. {1} - {2}", count, rnk.ObjectName.PadRight(16),
                                    rnk.Value1));
                            else
                                break;
                        }
                    }
                    foreach (var player in m_pMap.Players.Values.ToList())
                    {
                        player.Send(pRank[0], ChatTone.EVENT_RANKING);
                        for (int i = 1; i < pRank.Count; i++)
                        {
                            player.Send(pRank[i], ChatTone.EVENT_RANKING_NEXT);
                        }
                    }
                }

                if (now < _STARTUP_TIME || now >= _END_TIME)
                {
                    Finish();
                    DeliveRewards();
                    m_pState = EventState.IDLE;
                }
            }
            // if not, nothing will be done, event not loaded
        }

        public void AddPoints(Character pUser, int nAmount, bool bFlag = false)
        {
            if (pUser.Syndicate == null)
                return;

            int nCount = m_pMap.Players.Values.Count(x => x.SyndicateIdentity == pUser.SyndicateIdentity);

            DbDynamicRankRecord pRec, pUserRec;
            if (!m_pRanking.TryGetValue(pUser.SyndicateIdentity, out pRec))
            {
                pRec = new DbDynamicRankRecord
                {
                    ObjectIdentity = pUser.SyndicateIdentity,
                    ObjectName = pUser.SyndicateName,
                    RankType = _RANK_TYPE_U,
                    PlayerIdentity = 0,
                    PlayerName = ""
                };
                m_pRepo.SaveOrUpdate(pRec);
                m_pRanking.Add(pRec.ObjectIdentity, pRec);
            }
            pRec.Value1 = nCount; // now player num
            if (nCount > pRec.Value2)
                pRec.Value2 = nCount; // total player num
            pRec.Value3 += nAmount; // points
            pRec.Value4 += nAmount; // lifetime points
            m_pRepo.SaveOrUpdate(pRec);

            if (!m_pUserRank.TryGetValue(pUser.Identity, out pUserRec))
            {
                pUserRec = new DbDynamicRankRecord
                {
                    ObjectIdentity = pUser.SyndicateIdentity,
                    ObjectName = pUser.SyndicateName,
                    PlayerIdentity = pUser.Identity,
                    PlayerName = pUser.Name,
                    RankType = _RANK_TYPE_U
                };
                m_pRepo.SaveOrUpdate(pUserRec);
                m_pUserRank.Add(pUserRec.PlayerIdentity, pUserRec);
            }
            else if (pUserRec.ObjectIdentity != pUser.SyndicateIdentity)
            {
                pUserRec.ObjectIdentity = pUser.SyndicateIdentity;
                pUserRec.ObjectName = pUser.Syndicate.Name;
                pUserRec.Value3 = 0;
                pUserRec.Value4 = 0;
            }

            if (bFlag)
            {
                pUserRec.Value1 += 1; // now flag num
                pUserRec.Value2 += 1; // total flag num
            }
            pUserRec.Value3 += nAmount; // now ctf points
            pUserRec.Value4 += nAmount; // total ctf points
            m_pRepo.SaveOrUpdate(pUserRec);
        }

        public bool IsInBase(Character pUser)
        {
            foreach (var obj in pUser.Screen.GetAroundRoles)
            {
                if (obj is DynamicNpc)
                {
                    DynamicNpc pNpc = obj as DynamicNpc;
                    if (!pNpc.IsCtfFlag())
                        continue;
                    if (pNpc.GetDistance(pUser) > 10)
                        continue;
                    if (pNpc.OwnerType == 2 && pNpc.OwnerIdentity == pUser.SyndicateIdentity)
                        return true;
                }
            }
            return false;
        }

        public void DeliverFlag(Character pUser)
        {
            if (ServerKernel.CaptureTheFlag.IsInBase(pUser) && pUser.QueryStatus(FlagInt.CTF_FLAG) != null)
            {
                ServerKernel.CaptureTheFlag.AddPoints(pUser, 15, true);
                pUser.DetachStatus(FlagInt.CTF_FLAG);
                MsgWarFlag pMsg = new MsgWarFlag();
                pMsg.Type = WarFlagType.GRAB_FLAG_EFFECT;
                pMsg.Identity = pUser.Identity;
                pUser.Send(pMsg);
            }
        }

        public void SendRanking()
        {
            
        }

        public void SendInterfaceRanking(Character pUser, MsgSelfSynMemAwardRank pMsg)
        {
            if (m_pState != EventState.ENDED && m_pRanking.Count > 0)
            {
                if (m_pState == EventState.IDLE)
                {
                    int nCount = 0;
                    foreach (var syn in m_pRanking.Values.OrderByDescending(x => x.Value3).ThenByDescending(x => x.Value1))
                    {
                        if (nCount ++ >= 8)
                            break;
                        pMsg.AddToRanking(syn.ObjectName, (uint) syn.Value3, (uint) syn.Value2, ServerKernel.CTF_MONEY_REWARD[nCount-1], ServerKernel.CTF_EMONEY_REWARD[nCount-1]);
                    }
                    for (; nCount < 8; nCount++)
                    {
                        pMsg.AddToRanking("None", 0, 0, ServerKernel.CTF_MONEY_REWARD[nCount], ServerKernel.CTF_EMONEY_REWARD[nCount]);
                    }
                } 
                else if (m_pState == EventState.RUNNING)
                {
                    pMsg.Unknown8 = 1;
                    int nCount = 0;
                    foreach (var syn in m_pRanking.Values.OrderByDescending(x => x.Value3).ThenByDescending(x => x.Value1))
                    {
                        if (nCount++ >= 8)
                            break;
                        pMsg.AddToRanking(syn.ObjectName, (uint)syn.Value3, (uint)syn.Value1, syn.Value5, (uint) syn.Value6);
                    }
                    DbDynamicRankRecord pUserObj;
                    if (m_pUserRank.TryGetValue(pUser.Identity, out pUserObj))
                    {
                        pMsg.Exploits = (uint) pUserObj.Value3;
                    }
                    DbDynamicRankRecord pSynObj;
                    if (m_pRanking.TryGetValue(pUser.SyndicateIdentity, out pSynObj))
                    {
                        pMsg.SetMoney = (uint) pSynObj.Value5;
                        pMsg.SetEmoney = (uint) pSynObj.Value6;
                    }
                }
            }
            else
            {
                pMsg.AddToRanking("None", 0, 0, ServerKernel.CTF_MONEY_REWARD[0], ServerKernel.CTF_EMONEY_REWARD[0]);
                pMsg.AddToRanking("None", 0, 0, ServerKernel.CTF_MONEY_REWARD[1], ServerKernel.CTF_EMONEY_REWARD[1]);
                pMsg.AddToRanking("None", 0, 0, ServerKernel.CTF_MONEY_REWARD[2], ServerKernel.CTF_EMONEY_REWARD[2]);
                pMsg.AddToRanking("None", 0, 0, ServerKernel.CTF_MONEY_REWARD[3], ServerKernel.CTF_EMONEY_REWARD[3]);
                pMsg.AddToRanking("None", 0, 0, ServerKernel.CTF_MONEY_REWARD[4], ServerKernel.CTF_EMONEY_REWARD[4]);
                pMsg.AddToRanking("None", 0, 0, ServerKernel.CTF_MONEY_REWARD[5], ServerKernel.CTF_EMONEY_REWARD[5]);
                pMsg.AddToRanking("None", 0, 0, ServerKernel.CTF_MONEY_REWARD[6], ServerKernel.CTF_EMONEY_REWARD[6]);
                pMsg.AddToRanking("None", 0, 0, ServerKernel.CTF_MONEY_REWARD[7], ServerKernel.CTF_EMONEY_REWARD[7]);
            }
            pUser.Send(pMsg);
        }

        public void DeliveRewards()
        {
            //var allRecords = new List<DbDynamicRankRecord>();
            // value 1 -> syndicate players inside or user flag num (delivered)
            // value 2 -> syndicate total players or user lifetime flag num (delivered)
            // value 3 -> syndicate points or user points
            // value 4 -> syndicate lifetime points or user lifetime points
            // value 5 -> syndicate silver reward or user awarded silver
            // value 6 -> syndicate emoney reward or user awarded emoney
            foreach (var syn in m_pRanking.Values)
            {
                Syndicate pSyn;
                if (ServerKernel.Syndicates.TryGetValue(syn.ObjectIdentity, out pSyn))
                {
                    syn.Value5 = pSyn.MoneyPrize;
                    syn.Value6 = pSyn.EmoneyPrize;
                    pSyn.MoneyPrize = 0;
                    pSyn.EmoneyPrize = 0;
                }
            }

            foreach (var syn in m_pRanking.Values.Where(x => x.Value3 > 0 && (x.Value5 > 0 || x.Value6 > 0)))
            {
                //var pDelivered = new Dictionary<uint, Point>();
                uint synId = syn.ObjectIdentity;
                int deliverMoney = 0;
                int deliverEmoney = 0;

                foreach (var plr in m_pUserRank.Values.Where(x => x.ObjectIdentity == synId))
                {
                    float nPercent = (plr.Value3/(float) syn.Value3);
                    int money = (int) (syn.Value5*nPercent);
                    int emoney = (int) (syn.Value6*nPercent);
                    deliverMoney += money;
                    deliverEmoney += emoney;
                    Client pClient;
                    if (ServerKernel.Players.TryGetValue(plr.PlayerIdentity, out pClient))
                    {
                        pClient.Character.AwardMoney(money);
                        pClient.Character.AwardEmoney(emoney);
                        //pDelivered.Add(plr.PlayerIdentity, new Point(money, emoney));
                        pClient.SendMessage(string.Format(ServerString.STR_CTF_AWARDED_PRIZE,
                            plr.Value3, money, emoney), ChatTone.GUILD);
                    }
                    else
                    {
                        DbUser pUser = Database.Characters.SearchByIdentity(plr.PlayerIdentity);
                        if (pUser != null)
                        {
                            pUser.Money = (uint) Math.Min(int.MaxValue, pUser.Money + money);
                            pUser.Emoney = (uint) Math.Min(int.MaxValue, pUser.Emoney + emoney);
                            Database.Characters.SaveOrUpdate(pUser);
                        }
                    }
                    plr.Value5 = money;
                    plr.Value6 = emoney;
                    ServerKernel.Log.GmLog("ctfreward", 
                        string.Format("{0}({1}),awarded(money:{2},emoney:{3}),for({4})points", plr.PlayerIdentity, plr.PlayerName,
                            money, emoney, plr.Value3));
                }
                syn.Value5 = 0;
                syn.Value6 = 0;

                Syndicate pSyn;
                if (ServerKernel.Syndicates.TryGetValue(syn.ObjectIdentity, out pSyn))
                {
                    //if (pDelivered.Count <= 0)
                    //    continue;
                    DbDynamicRankRecord pDyna =
                        m_pUserRank.Values.OrderByDescending(x => x.Value3)
                            .FirstOrDefault(x => x.ObjectIdentity == syn.Identity && x.PlayerIdentity > 0);

                    if (pDyna != null)
                    {
                        pSyn.Send(string.Format(ServerString.STR_CTF_HIGHEST_REWARD,
                            pDyna.PlayerName, pDyna.Value5, pDyna.Value6));
                    }
                }

                m_pRepo.SaveOrUpdate(syn);
                ServerKernel.Log.GmLog("ctfsynreward", 
                    string.Format("syn(id:{0},name{1}),paid({2},{3}),totalprize({4},{5})",
                    syn.ObjectIdentity, syn.ObjectName, deliverMoney, deliverEmoney, syn.Value5, syn.Value6));
            }

            int nCount = 0;
            foreach (var syn in m_pRanking.Values.OrderByDescending(x => x.Value3).ThenByDescending(x => x.Value1))
            {
                if (nCount >= 8)
                    break;

                Syndicate updateSyn;
                if (!ServerKernel.Syndicates.TryGetValue(syn.ObjectIdentity, out updateSyn))
                    continue;

                updateSyn.ChangeFunds(ServerKernel.CTF_MONEY_REWARD[nCount]);
                updateSyn.ChangeEmoneyFunds((int) ServerKernel.CTF_EMONEY_REWARD[nCount]);
                nCount++;
            }
        }

        public void Finish()
        {
            foreach (var flag in m_pMap.GameObjects.Values)
                if (flag.Identity > IdentityRange.TRAPID_FIRST && flag.Identity < IdentityRange.TRAPID_LAST)
                    m_pMap.RemoveNpc(flag);
            foreach (var flag in m_pMap.Players.Values)
                if (flag.QueryStatus(FlagInt.CTF_FLAG) != null)
                    flag.DetachStatus(FlagInt.CTF_FLAG);

            foreach (var rank in m_pRanking.Values)
                m_pRepo.SaveOrUpdate(rank);
            foreach (var rank in m_pUserRank.Values)
                m_pRepo.SaveOrUpdate(rank);
        }

        public void RemoveMember(uint idUser)
        {
            if (!m_pUserRank.ContainsKey(idUser))
                return;
            m_pRepo.Delete(m_pUserRank[idUser]);
            m_pUserRank.Remove(idUser);
        }

        public List<DbDynamicRankRecord> GetSyndicatesByDonation()
        {
            return m_pRanking.Values.Where(x => x.Value5 > 0 || x.Value6 > 0)
                .OrderByDescending(x => x.Value5)
                .ThenByDescending(x => x.Value6)
                .ToList();
        }

        public List<DbDynamicRankRecord> GetSyndicateMembers(uint idSyn)
        {
            return m_pUserRank.Values.Where(x => x.ObjectIdentity == idSyn).ToList();
        }

        public void GenerateFlags()
        {
            int flags = FlagCount();
            if (flags < _FLAG_MAX_AMOUNT)
            {
                for (int i = flags; i < _FLAG_MAX_AMOUNT; i++)
                {
                    Point pos = default(Point);
                    if (!GetRandomMapPosition(ref pos))
                        continue;
                    EventFlag pRole = new EventFlag(FlagPacket((ushort) pos.X, (ushort) pos.Y), m_pMap);
                    m_pMap.AddNpc(pRole);
                }
            }
        }

        public int FlagCount()
        {
            int nCount = 0;
            foreach (var flag in m_pMap.GameObjects.Values)
                if (flag.Identity > IdentityRange.TRAPID_FIRST && flag.Identity < IdentityRange.TRAPID_LAST)
                    nCount += 1;
            foreach (var flag in m_pMap.Players.Values)
                if (flag.QueryStatus(FlagInt.CTF_FLAG) != null)
                    nCount += 1;
            return nCount;
        }

        public bool GetRandomMapPosition(ref Point pRet)
        {
            try
            {
                pRet = m_pValidTiles[ThreadSafeRandom.RandGet(m_pValidTiles.Count)%m_pValidTiles.Count];
                if (m_pMap[pRet.X, pRet.Y].Access != TileType.AVAILABLE)
                    return false;
                if (m_pMap.QueryRegion(7, (ushort)pRet.X, (ushort)pRet.Y))
                    return false;
                if (m_pMap.QueryRegion(8, (ushort)pRet.X, (ushort)pRet.Y))
                    return false;
                if (m_pMap.QueryRole((ushort)pRet.X, (ushort)pRet.Y) != null)
                    return false;
                //Console.WriteLine("Random Flag Pos Generated x({0}),y({1})", pRet.X, pRet.Y);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public MsgPlayer FlagPacket(ushort x, ushort y)
        {
            return new MsgPlayer((uint) (900000+x*100+y))
            {
                Mesh = _FLAG_MESH,
                MapX = x,
                MapY = y,
                Level = 1,
                MonsterLevel = 1,
                StringCount = 1,
                Name = "Flag"
            };
        }

        public bool IsInside(uint idRole)
        {
            return m_pMap.Players.ContainsKey(idRole);
        }
    }
}