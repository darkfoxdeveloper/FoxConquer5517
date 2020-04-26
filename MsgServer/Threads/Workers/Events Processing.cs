// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - Events Processing.cs
// Last Edit: 2017/02/20 09:18
// Created: 2017/02/04 14:38

using System;
using System.Linq;
using System.Threading;
using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Qualifier;
using ServerCore.Common;

namespace MsgServer.Threads
{
    public static partial class ThreadHandler
    {
        private static TimeOutMS m_pQuizShow = new TimeOutMS(750);
        private static TimeOut m_pPigeon = new TimeOut(1);
        private static TimeOut m_pMapEvent = new TimeOut(10);
        private static TimeOut m_pGuildWar = new TimeOut(1);
        private static TimeOut m_pScorePK = new TimeOut(1);
        private static TimeOut m_pSynRecruit = new TimeOut(60);
        private static TimeOut m_pUpdateLock = new TimeOut(10);

        public static void EventTasks()
        {
            m_pScorePK.Startup(1);
            m_pQuizShow.Startup(750);
            m_pPigeon.Startup(1);
            m_pMapEvent.Startup(10);
            m_pGuildWar.Startup(1);
            while (true)
            {
                try
                {
                    if (m_pGuildWar.ToNextTime())
                    {
                        foreach (var itr in ServerKernel.Maps.Values.Where(x => x.IsSynMap()))
                            foreach (var pNpc in itr.GameObjects.Values.OfType<DynamicNpc>())
                            {
                                pNpc.CheckFightTime();
                            }
                    }

                    if (m_pQuizShow.ToNextTime())
                        ServerKernel.QuizShow.OnTimer();
                    if (m_pPigeon.ToNextTime())
                        ServerKernel.Broadcast.OnTimer();
                    if (ServerKernel.ScorePkEvent.IsReady && m_pScorePK.ToNextTime())
                        ServerKernel.ScorePkEvent.OnTimer();
                    if (ServerKernel.CaptureTheFlag != null && ServerKernel.CaptureTheFlag.IsActive)
                        ServerKernel.CaptureTheFlag.OnTimer();
                    if (ServerKernel.ArenaQualifier != null &&
                        ServerKernel.ArenaQualifier.Status == ArenaQualifierStatus.ENABLED)
                        ServerKernel.ArenaQualifier.OnTimer();
                    if (ServerKernel.SyndicateScoreWar != null && ServerKernel.SyndicateScoreWar.IsAvaiable)
                        ServerKernel.SyndicateScoreWar.OnTimer();
                    if (ServerKernel.LineSkillPk != null && ServerKernel.LineSkillPk.IsReady)
                        ServerKernel.LineSkillPk.OnTimer();

                    if (m_pSynRecruit.ToNextTime())
                    {
                        ServerKernel.SyndicateRecruitment.CheckSyndicates();
                    }

                    DateTime now = DateTime.Now;
                    if (now.Hour == 0 && now.Minute == 0 && now.Second == 0
                        && m_pUpdateLock.ToNextTime())
                    {
                        foreach (var plr in ServerKernel.Players.Values.ToList())
                        {
                            plr.Character.DailyReset();
                        }

                        foreach (var arena in ServerKernel.ArenaRecord.Values)
                        {
                            DbUser dbUsr = Database.Characters.SearchByIdentity(arena.PlayerIdentity);
                            if (dbUsr == null)
                            {
                                arena.Delete();
                                continue;
                            }

                            arena.LastRanking = arena.Ranking;
                            arena.LastSeasonPoints = arena.Points;
                            arena.LastSeasonWins = arena.TodayWins;
                            arena.LastSeasonsLoses = arena.TodayLoses;

                            arena.Lookface = dbUsr.Lookface;
                            arena.Level = dbUsr.Level;
                            arena.Profession = dbUsr.Profession;

                            if (arena.Ranking > 0)
                            {
                                IHonorReward reward =
                                    ServerKernel.HonorRewards.Values.FirstOrDefault(x => x.Ranking == arena.Ranking);
                                if (reward != null)
                                {
                                    arena.HonorPoints += reward.DailyHonor;
                                    arena.TotalHonorPoints += reward.DailyHonor;
                                }
                            }
                            arena.Level = dbUsr.Level;
                            arena.Lookface = dbUsr.Lookface;
                            arena.Profession = dbUsr.Profession;
                            arena.PlayerName = dbUsr.Name;
                            arena.Save();
                        }
                        foreach (var arena in ServerKernel.ArenaRecord.Values)
                        {
                            arena.Points = ServerKernel.ArenaQualifier.GetStartupPoints(arena.Level);
                            arena.TodayWins = 0;
                            arena.TodayLoses = 0;
                            arena.Save();
                        }
                    }

                    if (m_pMapEvent.ToNextTime())
                    {
                        foreach (
                            var npc in
                                ServerKernel.Maps.Values.Where(x => x.IsPkField() || x.IsPkGameMap() || x.IsSynMap())
                                    .SelectMany(map => map.GameObjects.Values)
                                    .OfType<DynamicNpc>())
                        {
                            if (npc.MapIdentity == 7600 || npc.MapIdentity == 2057)
                                continue;

                            npc.SendOwnerRanking();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
                }
                finally
                {
                    Thread.Sleep(750);
                }
            }
            Console.WriteLine("Game Event Processing Thread exited");
        }
    }
}