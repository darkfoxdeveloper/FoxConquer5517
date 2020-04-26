// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Syndicate Score War.cs
// Last Edit: 2017/01/24 00:24
// Created: 2017/01/16 23:02

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    public sealed class SyndicateScoreWar
    {
        private const uint _MAP_ID_U = 7600;
        private const uint _MAP_JAIL_ID_U = 7601;
        private const int _STARTUP_TIME = 220000;
        private const int _END_TIME = 230000;
        private const uint _RANK_TYPE_U = 9;

        private TimeOut m_tRank = new TimeOut(10);

        private DynamicRankRecordRepository m_pRepo;
        private SyndicateWarState m_pState = SyndicateWarState.NOT_BUILT;

        private ConcurrentDictionary<uint, DbDynamicRankRecord> m_pSynPoints;
        private ConcurrentDictionary<uint, DbDynamicRankRecord> m_pUserPoints;

        private Map m_pMainMap;
        private Map m_pJailMap;

        public SyndicateScoreWar()
        {
            m_pRepo = new DynamicRankRecordRepository();
            m_pSynPoints = new ConcurrentDictionary<uint, DbDynamicRankRecord>();
            m_pUserPoints = new ConcurrentDictionary<uint, DbDynamicRankRecord>();
        }

        public bool Create()
        {
            try
            {
                if (!ServerKernel.Maps.TryGetValue(_MAP_ID_U, out m_pMainMap) ||
                    !ServerKernel.Maps.TryGetValue(_MAP_JAIL_ID_U, out m_pJailMap))
                    throw new Exception("SCW_INVALID_MAP_ID");

                if (!m_pMainMap.Load() || !m_pMainMap.Load())
                    throw new Exception("SCW_COULT_NOT_LOAD_MAP");

                var allRank = m_pRepo.FetchByType(_RANK_TYPE_U);
                if (allRank != null)
                {
                    foreach (var rank in allRank.Where(x => x.PlayerIdentity == 0))
                    {
                        m_pSynPoints.TryAdd(rank.ObjectIdentity, rank);
                    }
                    foreach (var rank in allRank.Where(x => x.PlayerIdentity > 0))
                    {
                        m_pUserPoints.TryAdd(rank.PlayerIdentity, rank);
                    }
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not start syndicate score war", true, LogType.ERROR);
                return false;
            }
            m_pState = SyndicateWarState.NOT_RUNNING;
            return true;
        }

        public bool IsAvaiable
        {
            get { return m_pState > SyndicateWarState.NOT_BUILT; }
        }

        public bool IsRunning
        {
            get { return m_pState == SyndicateWarState.RUNNING; }
        }

        /*
         * Value1 = Syn Actual Points or Player Actual Points
         */
        public void AwardPoints(Character pSender, uint dwPoints)
        {
            if (pSender.Syndicate == null || dwPoints == 0)
                return;

            DbDynamicRankRecord pDyna;
            if (!m_pSynPoints.TryGetValue(pSender.SyndicateIdentity, out pDyna))
            {
                pDyna = new DbDynamicRankRecord
                {
                    ObjectIdentity = pSender.SyndicateIdentity,
                    ObjectName = pSender.SyndicateName,
                    PlayerIdentity = 0,
                    PlayerName = "",
                    RankType = _RANK_TYPE_U
                };
                m_pRepo.SaveOrUpdate(pDyna);
                m_pSynPoints.TryAdd(pDyna.ObjectIdentity, pDyna);
            }
            pDyna.Value1 += dwPoints;

            if (!m_pUserPoints.TryGetValue(pSender.Identity, out pDyna))
            {
                pDyna = new DbDynamicRankRecord
                {
                    ObjectIdentity = pSender.SyndicateIdentity,
                    ObjectName = pSender.SyndicateName,
                    PlayerIdentity = pSender.Identity,
                    PlayerName = pSender.Name,
                    RankType = _RANK_TYPE_U
                };
                m_pRepo.SaveOrUpdate(pDyna);
                m_pUserPoints.TryAdd(pDyna.PlayerIdentity, pDyna);
            }
            else if (pDyna.ObjectIdentity != pSender.SyndicateIdentity)
            {
                pDyna.ObjectIdentity = pSender.SyndicateIdentity;
                pDyna.ObjectName = pSender.SyndicateName;
                pDyna.Value1 = 0;
            }
            pDyna.Value1 += dwPoints;
        }

        public void OnTimer()
        {
            if (m_pState == SyndicateWarState.NOT_BUILT)
                return;

            int now = int.Parse(DateTime.Now.ToString("HHmmss"));
            if (m_pState == SyndicateWarState.NOT_RUNNING 
                && (now >= _STARTUP_TIME && now < _END_TIME))
            {
                // start
                foreach (var syn in m_pSynPoints.Values)
                {
                    syn.Value1 = 0;
                    m_pRepo.SaveOrUpdate(syn);
                }

                foreach (var user in m_pUserPoints.Values)
                {
                    user.Value1 = 0;
                    m_pRepo.SaveOrUpdate(user);
                }

                IScreenObject pScrObj;
                if (m_pMainMap.GameObjects.TryGetValue(920, out pScrObj) && pScrObj is DynamicNpc)
                {
                    DynamicNpc pNpc = pScrObj as DynamicNpc;
                    pNpc.Life = pNpc.MaxLife;
                    pNpc.SendToRange();
                }

                m_tRank.Startup(10);
                m_pState = SyndicateWarState.RUNNING;
            } 
            else if (m_pState == SyndicateWarState.RUNNING
                       && (now >= _STARTUP_TIME && now < _END_TIME))
            {
                // running
                if (m_tRank.IsActive() && m_tRank.ToNextTime())
                {
                    List<string> pRank = new List<string>(9);
                    pRank.Add("Syndicate Score War");
                    int count = 0;
                    foreach (var rnk in m_pSynPoints.Values.OrderByDescending(x => x.Value1))
                    {
                        if (rnk.Value1 > 0)
                        {
                            if (count++ < 8)
                                pRank.Add(string.Format("Nº{0}. {1,16} - {2}", count, rnk.ObjectName.PadRight(16),
                                    rnk.Value1));
                            else
                                break;
                        }
                    }
                    foreach (var player in m_pMainMap.Players.Values.ToList())
                    {
                        player.Send(pRank[0], ChatTone.EVENT_RANKING);
                        for (int i = 1; i < pRank.Count; i++)
                        {
                            player.Send(pRank[i], ChatTone.EVENT_RANKING_NEXT);
                        }
                    }
                }
            }
            else if (m_pState == SyndicateWarState.RUNNING
                     && (now < _STARTUP_TIME || now > _END_TIME))
            {
                // prepare to finish
                ServerKernel.SendMessageToAll(string.Format("The time is up! The Guild Score War is over."), ChatTone.TOP_LEFT);
                m_pState = SyndicateWarState.ENDING;
            }
            else if (m_pState == SyndicateWarState.ENDING)
            {
                // end time

                int rank = 0;
                foreach (var syn in m_pSynPoints.Values.OrderByDescending(x => x.Value1))
                {
                    if (rank++ >= 4)
                        break;

                    uint synId = syn.ObjectIdentity;
                    int deliverMoney = 0;
                    int deliverEmoney = 0;
                    uint totalMoney = ServerKernel.SYN_SCORE_MONEY_REWARD[rank - 1];
                    uint totalEmoney = ServerKernel.SYN_SCORE_EMONEY_REWARD[rank - 1];

                    foreach (var user in
                            m_pUserPoints.Values.Where(x => x.ObjectIdentity == synId && x.PlayerIdentity > 0)
                            .OrderByDescending(x => x.Value1))
                    {
                        float nPercent = (user.Value1 / (float)syn.Value1);
                        int money = (int)(totalMoney * nPercent);
                        int emoney = (int)(totalEmoney * nPercent);
                        deliverMoney += money;
                        deliverEmoney += emoney;

                        if (money <= 0 && emoney <= 0)
                            continue;

                        Client pClient;
                        if (ServerKernel.Players.TryGetValue(user.PlayerIdentity, out pClient))
                        {
                            pClient.Character.AwardMoney(money);
                            pClient.Character.AwardEmoney(emoney);
                            pClient.SendMessage(string.Format(ServerString.STR_SSPK_AWARDED_PRIZE,
                                user.Value1, money, emoney), ChatTone.GUILD);
                        }
                        else
                        {
                            DbUser pUser = Database.Characters.SearchByIdentity(user.PlayerIdentity);
                            if (pUser != null)
                            {
                                pUser.Money = (uint)Math.Min(int.MaxValue, pUser.Money + money);
                                pUser.Emoney = (uint)Math.Min(int.MaxValue, pUser.Emoney + emoney);
                                Database.Characters.SaveOrUpdate(pUser);
                            }
                        }
                    }
                }

                ServerKernel.SendMessageToAll(string.Format("The Guild Score War is over."), ChatTone.TALK);
                m_pState = SyndicateWarState.NOT_RUNNING;
            }
        }
    }

    public enum SyndicateWarState
    {
        NOT_BUILT, NOT_RUNNING, STARTING, RUNNING, ENDING
    }
}