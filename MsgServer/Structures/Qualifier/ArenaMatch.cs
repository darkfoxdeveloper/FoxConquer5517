// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - Arena Match.cs
// Last Edit: 2017/02/20 09:17
// Created: 2017/02/04 14:38

using System;
using System.Drawing;
using Core.Common.Enums;
using MsgServer.Structures.Entities;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Qualifier
{
    public sealed class ArenaMatch
    {
        private Character m_pPlayer1, m_pPlayer2;
        private TimeOut m_tInvite = new TimeOut(60);
        private TimeOut m_tStartup = new TimeOut(10);
        private TimeOut m_tMatchTime = new TimeOut(300);
        private Map m_pMap;
        private ArenaStatus m_pStatus = ArenaStatus.WAITING_APPROVE;
        private bool m_bP1Accept, m_bP2Accept;
        private uint m_dwMapId;

        private PkModeType m_pPkMode1;
        private PkModeType m_pPkMode2;

        public ArenaMatch(Character pUser1, Character pUser2, Map pMap)
        {
            m_pPlayer1 = pUser1;
            m_pPlayer2 = pUser2;
            m_pMap = pMap;
            m_dwMapId = pMap.Identity;
            ServerKernel.Maps.TryAdd(pMap.Identity, m_pMap);
        }

        public bool Notify()
        {
            if (m_pMap == null || ReadyToStart()) return false;

            m_pPlayer1.ArenaQualifier.Status = ArenaWaitStatus.WAITING_INACTIVE;
            m_pPlayer2.ArenaQualifier.Status = ArenaWaitStatus.WAITING_INACTIVE;

            m_tInvite.Startup(60);

            MsgQualifyingInteractive pMsg = new MsgQualifyingInteractive
            {
                Type = ArenaType.START_COUNT_DOWN,
                Identity = (uint) ArenaWaitStatus.WAITING_INACTIVE
            };
            m_pPlayer1.Send(pMsg);
            m_pPlayer2.Send(pMsg);
            return true;
        }

        public bool Accept(Character pSender)
        {
            if (pSender == m_pPlayer1)
                m_bP1Accept = true;
            else if (pSender == m_pPlayer2)
                m_bP2Accept = true;
            else return false;
            return m_tStartup.IsActive() && m_tStartup.IsTimeOut();
        }

        public bool GiveUp(Character pSender)
        {
            if (pSender == m_pPlayer1)
                Finish(m_pPlayer2, m_pPlayer1, true);
            else
                Finish(m_pPlayer1, m_pPlayer2, true);
            return true;
        }

        public bool ReadyToStart()
        {
            return m_bP1Accept && m_bP2Accept;
        }

        public bool IsRunning()
        {
            return m_pStatus >= ArenaStatus.NOT_STARTED && m_pStatus < ArenaStatus.FINISHED;
        }

        public bool HasStarted()
        {
            return m_pStatus > ArenaStatus.NOT_STARTED && m_pStatus < ArenaStatus.FINISHED;
        }

        public bool ReadyToDispose()
        {
            return m_pStatus == ArenaStatus.DISPOSED;
        }

        public Map Map
        {
            get { return m_pMap; }
        }

        public uint MapIdentity
        {
            get { return m_dwMapId; }
        }

        public Character User1
        {
            get { return m_pPlayer1; }
        }

        public Character User2
        {
            get { return m_pPlayer2; }
        }

        public uint Identity1
        {
            get { return m_pPlayer1.Identity; }
        }

        public uint Identity2
        {
            get { return m_pPlayer2.Identity; }
        }

        public bool Win1;
        public bool Win2;

        public uint Points1 = 0u;
        public uint Points2 = 0u;

        public uint Watchers => (uint) Math.Max(0, m_pMap.Players.Count - 2);

        public uint Waving1 = 0u;
        public uint Waving2 = 0u;

        public bool IsAttackEnable
        {
            get
            {
                return m_pStatus == ArenaStatus.RUNNING || m_pStatus == ArenaStatus.FINISHED ||
                       (m_tMatchTime.IsActive() && !m_tMatchTime.IsTimeOut());
            }
        }

        public void Start()
        {
            m_tStartup.Startup(10);
            m_pStatus = ArenaStatus.NOT_STARTED;

            m_pPlayer1.DetachBadlyStatus(m_pPlayer1);
            m_pPlayer2.DetachBadlyStatus(m_pPlayer2);

            //Console.WriteLine("Players {0} vs {1} has been sent to map {2}", m_pPlayer1.Name, m_pPlayer2.Name, m_pMap.Identity);

            if (m_pPlayer1.Map.IsRecordDisable())
            {
                uint dwMapId = m_pPlayer1.RecordMapIdentity;
                Point pos = new Point(m_pPlayer1.RecordMapX, m_pPlayer1.RecordMapY);

                m_pPlayer1.Map.GetRebornMap(ref dwMapId, ref pos);
                m_pPlayer1.SetRecordPos(dwMapId, (ushort) pos.X, (ushort) pos.Y);
            }
            else
            {
                m_pPlayer1.SetRecordPos(m_pPlayer1.MapIdentity, m_pPlayer1.MapX, m_pPlayer1.MapY);
            }

            if (m_pPlayer2.Map.IsRecordDisable())
            {
                uint dwMapId = m_pPlayer2.RecordMapIdentity;
                Point pos = new Point(m_pPlayer2.RecordMapX, m_pPlayer2.RecordMapY);

                m_pPlayer2.Map.GetRebornMap(ref dwMapId, ref pos);
                m_pPlayer2.SetRecordPos(dwMapId, (ushort) pos.X, (ushort) pos.Y);
            }
            else
            {
                m_pPlayer2.SetRecordPos(m_pPlayer2.MapIdentity, m_pPlayer2.MapX, m_pPlayer2.MapY);
            }

            if (!m_pPlayer1.IsAlive)
                m_pPlayer1.Reborn(false, true);
            if (!m_pPlayer2.IsAlive)
                m_pPlayer2.Reborn(false, true);

            try
            {
                m_pPlayer1.ChangeMap((ushort) ThreadSafeRandom.RandGet(35, 70),
                    (ushort) ThreadSafeRandom.RandGet(35, 70), m_pMap.Identity);
            }
            catch
            {
                Finish(m_pPlayer2, m_pPlayer1, true);
                return;
            }
            try
            {
                m_pPlayer2.ChangeMap((ushort) ThreadSafeRandom.RandGet(35, 70),
                    (ushort) ThreadSafeRandom.RandGet(35, 70), m_pMap.Identity);
            }
            catch
            {
                Finish(m_pPlayer1, m_pPlayer2, true);
                return;
            }

            SendToMap();

            m_pPkMode1 = m_pPlayer1.PkMode;
            m_pPkMode2 = m_pPlayer2.PkMode;

            m_pPlayer1.PkMode = PkModeType.PK_MODE;
            m_pPlayer2.PkMode = PkModeType.PK_MODE;

            m_pPlayer1.SendArenaInformation(m_pPlayer2);
            m_pPlayer2.SendArenaInformation(m_pPlayer1);

            MsgQualifyingInteractive pMsg = new MsgQualifyingInteractive();
            pMsg.Type = ArenaType.MATCH;
            pMsg.Option = 5; // Match On
            m_pPlayer1.Send(pMsg);
            m_pPlayer2.Send(pMsg);

            m_pPlayer1.DetachStatus(FlagInt.RIDING);
            m_pPlayer2.DetachStatus(FlagInt.RIDING);

            m_pPlayer1.FillLife();
            m_pPlayer1.FillMana();
            m_pPlayer2.FillLife();
            m_pPlayer2.FillMana();
        }

        public void OnTimer()
        {
            if (m_pStatus == ArenaStatus.WAITING_APPROVE && m_tInvite.IsTimeOut(60))
            {
                if (m_bP1Accept && !m_bP2Accept)
                    Finish(m_pPlayer1, m_pPlayer2, true);
                else if (m_bP2Accept && !m_bP1Accept)
                    Finish(m_pPlayer2, m_pPlayer1, true);
                else
                {
                    if (Calculations.ChanceCalc(50f))
                        Finish(m_pPlayer1, m_pPlayer2, true);
                    else
                        Finish(m_pPlayer2, m_pPlayer1, true);
                }
            }
            if (m_pStatus == ArenaStatus.NOT_STARTED && m_tStartup.IsActive() && m_tStartup.IsTimeOut(10))
            {
                m_tMatchTime.Startup(300);
                m_pStatus = ArenaStatus.RUNNING;
            }
            else if (m_pStatus == ArenaStatus.RUNNING && m_tMatchTime.IsTimeOut(300))
            {
                Finish();
                m_tStartup.Startup(3);
                m_pStatus = ArenaStatus.FINISHED;
            }
            else if (m_pStatus == ArenaStatus.FINISHED && m_tStartup.IsTimeOut(3))
            {
                Dispose();
                m_pStatus = ArenaStatus.DISPOSED;
            }
        }

        public void Finish()
        {
            if (Points1 > Points2)
            {
                Finish(m_pPlayer1, m_pPlayer2, false);
            }
            else
            {
                Finish(m_pPlayer2, m_pPlayer1, false);
            }
        }

        public void Finish(Character loser)
        {
            if (loser == m_pPlayer1)
                Finish(m_pPlayer2, m_pPlayer1, false);
            else if (loser == m_pPlayer2)
                Finish(m_pPlayer1, m_pPlayer2, false);
        }

        public void Finish(Character pWinner, Character pLoser, bool bDispose)
        {
            if (pWinner == m_pPlayer1)
                Win1 = true;
            else
                Win2 = true;

            pWinner.ArenaQualifier.Points = (uint) (pWinner.ArenaQualifier.Points*1.03f);
            pLoser.ArenaQualifier.Points = (uint) (pLoser.ArenaQualifier.Points*.97f);

            pWinner.ArenaQualifier.TodayWins += 1;
            pWinner.ArenaQualifier.TotalWins += 1;
            pLoser.ArenaQualifier.TodayLoses += 1;
            pLoser.ArenaQualifier.TotalLoses += 1;

            int todayMatch0 = (int) (pWinner.ArenaQualifier.TodayWins + pWinner.ArenaQualifier.TodayLoses);
            int todayMatch1 = (int) (pLoser.ArenaQualifier.TodayWins + pLoser.ArenaQualifier.TodayLoses);
            if (pWinner.ArenaQualifier.TodayWins == 9)
            {
                pWinner.ArenaQualifier.TotalHonorPoints += 5000;
                pWinner.ArenaQualifier.HonorPoints += 5000;
            }
            if (todayMatch0 == 20)
            {
                pWinner.ArenaQualifier.TotalHonorPoints += 5000;
                pWinner.ArenaQualifier.HonorPoints += 5000;
            }
            if (todayMatch1 == 20)
            {
                pLoser.ArenaQualifier.TotalHonorPoints += 5000;
                pLoser.ArenaQualifier.HonorPoints += 5000;
            }

            MsgQualifyingInteractive pMsg = new MsgQualifyingInteractive
            {
                Type = ArenaType.DIALOG,
                Option = 3 // lose
            };
            try
            {
                pLoser.Send(pMsg);
            }
            catch
            {
            }
            pMsg.Option = 1; // win
            try
            {
                pWinner.Send(pMsg);
            }
            catch
            {
            }

            pWinner.ArenaQualifier.Save();
            pLoser.ArenaQualifier.Save();

            if (pWinner.Syndicate != null && pLoser.Syndicate != null)
            {
                ServerKernel.SendMessageToAll(string.Format(ServerString.STR_ARENIC_MATCH_END3,
                    pWinner.SyndicateName, pWinner.Name, pLoser.SyndicateName, pLoser.Name,
                    pWinner.ArenaQualifier.Ranking), ChatTone.QUALIFIER);
            }
            else if (pWinner.Syndicate != null && pLoser.Syndicate == null)
            {
                ServerKernel.SendMessageToAll(string.Format(ServerString.STR_ARENIC_MATCH_END1,
                    pWinner.SyndicateName, pWinner.Name, pLoser.Name,
                    pWinner.ArenaQualifier.Ranking), ChatTone.QUALIFIER);
            }
            else if (pWinner.Syndicate == null && pLoser.Syndicate != null)
            {
                ServerKernel.SendMessageToAll(string.Format(ServerString.STR_ARENIC_MATCH_END2,
                    pWinner.Name, pLoser.SyndicateName, pLoser.Name,
                    pWinner.ArenaQualifier.Ranking), ChatTone.QUALIFIER);
            }
            else
            {
                ServerKernel.SendMessageToAll(string.Format(ServerString.STR_ARENIC_MATCH_END0,
                    pWinner.Name,
                    pLoser.Name,
                    pWinner.ArenaQualifier.Ranking), ChatTone.QUALIFIER);
            }

            if (bDispose)
            {
                m_pStatus = ArenaStatus.DISPOSED;
                ShowFinalDialog(pWinner, pLoser);
            }
            else
            {
                m_tStartup.Startup(3);
                m_pStatus = ArenaStatus.FINISHED;
            }
        }

        public void ShowFinalDialog(Character winner, Character loser)
        {
            MsgQualifyingInteractive pMsg = new MsgQualifyingInteractive
            {
                Type = ArenaType.END_DIALOG,
                Option = 1 // win
            };

            try
            {
                winner.Send(pMsg);
            }
            catch
            {
            }

            pMsg = new MsgQualifyingInteractive();
            pMsg.Type = ArenaType.END_DIALOG;
            try
            {
                loser.Send(pMsg);
            }
            catch
            {
            }
            winner.SendArenaStatus();
            loser.SendArenaStatus();
        }

        public void Dispose()
        {
            try
            {
                m_pPlayer1.ChangeMap(m_pPlayer1.RecordMapX, m_pPlayer1.RecordMapY, m_pPlayer1.RecordMapIdentity);
                if (!m_pPlayer1.IsAlive)
                {
                    m_pPlayer1.Reborn(false, true);
                }
                m_pPlayer1.PkMode = m_pPkMode1;
            }
            catch
            {
            }
            try
            {
                m_pPlayer2.ChangeMap(m_pPlayer2.RecordMapX, m_pPlayer2.RecordMapY, m_pPlayer2.RecordMapIdentity);
                if (!m_pPlayer2.IsAlive)
                {
                    m_pPlayer2.Reborn(false, true);
                }
                m_pPlayer2.PkMode = m_pPkMode2;
            }
            catch
            {
            }

            if (Win1)
                ShowFinalDialog(m_pPlayer1, m_pPlayer2);
            else
                ShowFinalDialog(m_pPlayer2, m_pPlayer1);

            TeleportBack();
            m_pStatus = ArenaStatus.DISPOSED;
        }

        public void SendToMap()
        {
            if (m_pMap.Identity < 900000)
                return;
            MsgArenicScore pScore = new MsgArenicScore
            {
                Damage1 = Points1,
                Damage2 = Points2,
                Name1 = m_pPlayer1.Name,
                Name2 = m_pPlayer2.Name,
                EntityIdentity1 = Identity1,
                EntityIdentity2 = Identity2
            };
            MsgArenicWitness pWitness = new MsgArenicWitness
            {
                Action = MsgArenicWitness.RequestView,
                Cheers1 = Waving1,
                Cheers2 = Waving2
            };
            try
            {
                foreach (var plr in m_pMap.Players.Values)
                {
                    plr.Send(pScore);
                    if (plr.IsWatcher)
                    {
                        plr.Send(pWitness);
                        if (plr.Identity != Identity1 && plr.Identity != Identity2)
                        {
                            MsgArenicWitness pWatchers = new MsgArenicWitness
                            {
                                Action = MsgArenicWitness.Watchers,
                                Cheers2 = Waving2,
                                Cheers1 = Waving1
                            };
                            try
                            {
                                foreach (var plr0 in m_pMap.Players.Values)
                                    //.Where(x => x.Identity != plr.Identity && x.Identity != Identity1 && x.Identity != Identity2))
                                {
                                    if (plr0.Identity == Identity1 || plr0.Identity == Identity2)
                                        continue;
                                    pWatchers.AppendName(plr0.Name, plr0.Lookface, plr0.Identity, plr0.Level,
                                        plr0.Profession,
                                        plr0.ArenaQualifier.Ranking);
                                }
                            }
                            catch
                            {

                            }
                            plr.Send(pWatchers);
                        }
                    }
                }
            }
            catch
            {
                
            }
        }

        public void TeleportBack()
        {
            try
            {
                MsgArenicScore pMsg = new MsgArenicScore();
                MsgArenicWitness pWatchers = new MsgArenicWitness
                {
                    Action = MsgArenicWitness.Leave
                };
                foreach (var plr in m_pMap.Players.Values)
                {
                    plr.Send(pMsg);
                    plr.Send(pWatchers);
                    plr.ChangeMap(plr.RecordMapX, plr.RecordMapY, plr.RecordMapIdentity);
                }
            }
            catch
            {
                
            }
        }
    }

    public enum ArenaStatus
    {
        WAITING_APPROVE,
        NOT_STARTED,
        RUNNING,
        FINISHED,
        DISPOSED
    }
}