// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - Arena Engine.cs
// Last Edit: 2017/02/20 09:17
// Created: 2017/02/04 14:38

using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using DB.Entities;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Qualifier
{
    public class ArenaEngine
    {
        private TimeOut m_tMatchCheckDelay = new TimeOut(1);
        private Map m_pBaseMap;
        private readonly ConcurrentDictionary<uint, Character> m_pPlayers;
        private readonly ConcurrentDictionary<uint, ArenaMatch> m_pMatches;
        private ArenaQualifierStatus m_pStatus;

        private uint[] m_dwStartPoints =
        {
            1500, // over 70
            2200, // over 90
            2700, // over 100
            3200, // over 110
            4000 // over 120
        };

        private uint m_dwStartPointsPrice = 6000u;

        public ArenaEngine()
        {
            m_pPlayers = new ConcurrentDictionary<uint, Character>();
            m_pMatches = new ConcurrentDictionary<uint, ArenaMatch>();
        }

        public bool Create()
        {
            try
            {
                m_pBaseMap = ServerKernel.Maps[900000];
            }
            catch
            {
                ServerKernel.Log.SaveLog("COULD NOT START ARENA QUALIFIER MAP EXCEPTION", true, LogType.ERROR);
                return false;
            }

            m_pStatus = ArenaQualifierStatus.ENABLED;
            return true;
        }

        public uint GetStartupPoints(byte pLevel)
        {
            uint startPoints = m_dwStartPoints[0];
            if (pLevel < 130)
                startPoints = m_dwStartPoints[3];
            else if (pLevel < 120)
                startPoints = m_dwStartPoints[2];
            else if (pLevel < 110)
                startPoints = m_dwStartPoints[1];
            else if (pLevel < 90)
                startPoints = m_dwStartPoints[0];
            else
                startPoints = m_dwStartPoints[4];
            return startPoints;
        }

        public bool GenerateFirstData(Character pUser)
        {
            QualifierRankObj trash;
            if (ServerKernel.ArenaRecord.TryGetValue(pUser.Identity, out trash))
            {
                if (pUser.ArenaQualifier == null)
                {
                    pUser.ArenaQualifier = trash;
                    return true;
                }
                return false;
            }

            uint startPoints = GetStartupPoints(pUser.Level);

            DbArena newObj = new DbArena
            {
                PlayerIdentity = pUser.Identity,
                Name = pUser.Name,
                Points = startPoints,
                Lookface = pUser.Lookface,
                Level = pUser.Level,
                Profession = pUser.Profession
            };
            Database.ArenaRepository.SaveOrUpdate(newObj);

            pUser.ArenaQualifier = new QualifierRankObj(newObj);
            return ServerKernel.ArenaRecord.TryAdd(pUser.Identity, pUser.ArenaQualifier);
        }

        public void OnTimer()
        {
            if (m_tMatchCheckDelay.ToNextTime(1))
            {
                foreach (var plr in m_pPlayers.Values.Where(x => x.ArenaStatus == ArenaWaitStatus.WAITING_FOR_OPPONENT))
                {
                    try
                    {
                        FindMatch(plr);
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog("Could not find match");
                    }
                }

                foreach (var match in m_pMatches.Values)
                {
                    if (!match.ReadyToDispose())
                    {
                        try
                        {
                            match.OnTimer();
                        }
                        catch (Exception ex)
                        {
                            ServerKernel.Log.SaveLog("Error match.OnTimer()\r\n" + ex, true, LogType.EXCEPTION);
                        }
                    }
                    else
                    {
                        try
                        {
                            ArenaMatch trash;
                            m_pMatches.TryRemove(match.MapIdentity, out trash);

                            Client p1, p2;
                            if (ServerKernel.Players.TryGetValue(trash.Identity1, out p1))
                                if (p1 != null && p1.Character != null)
                                    m_pPlayers.TryAdd(p1.Identity, p1.Character);

                            if (ServerKernel.Players.TryGetValue(trash.Identity2, out p2))
                                if (p2 != null && p2.Character != null)
                                    m_pPlayers.TryAdd(p2.Identity, p2.Character);

                            Map trash0;
                            ServerKernel.Maps.TryRemove(trash.MapIdentity, out trash0);
                        }
                        catch (Exception ex)
                        {
                            ServerKernel.Log.SaveLog("Error try remove from arena\r\n" + ex, true, LogType.EXCEPTION);
                        }
                    }
                }
            }
        }

        public bool Inscribe(Character pUser)
        {
            if (pUser.ArenaQualifier == null)
            {
                if (!GenerateFirstData(pUser))
                    return false;
            }

            if (pUser.ArenaQualifier.IsLocked)
            {
                pUser.ArenaQualifier.Status = ArenaWaitStatus.NOT_SIGNED_UP;
                pUser.Send(ServerString.STR_ARENIC_BANNED);
                return false;
            }

            if (pUser.Level < 70)
            {
                pUser.ArenaQualifier.Status = ArenaWaitStatus.NOT_SIGNED_UP;
                pUser.Send(ServerString.STR_ARENIC_LOW_LEVEL);
                return false;
            }

            if (IsInsideMatch(pUser.Identity))
            {
                pUser.ArenaQualifier.Status = ArenaWaitStatus.WAITING_INACTIVE;
                pUser.Send(ServerString.STR_ARENIC_ALREADY_JOINED);
                return false;
            }

            if (m_pPlayers.ContainsKey(pUser.Identity))
            {
                pUser.ArenaQualifier.Status = ArenaWaitStatus.WAITING_FOR_OPPONENT;
                pUser.Send(ServerString.STR_ARENIC_ALREADY_JOINED);
                return false;
            }

            pUser.ArenaQualifier.Status = ArenaWaitStatus.WAITING_FOR_OPPONENT;
            return m_pPlayers.TryAdd(pUser.Identity, pUser);
        }

        public bool Uninscribe(Character pUser)
        {
            if (!m_pPlayers.ContainsKey(pUser.Identity))
            {
                pUser.ArenaQualifier.Status = ArenaWaitStatus.NOT_SIGNED_UP;
                return false;
            }

            if (IsInsideMatch(pUser.Identity))
                return false;

            pUser.ArenaQualifier.Status = ArenaWaitStatus.NOT_SIGNED_UP;
            Character trash;
            return m_pPlayers.TryRemove(pUser.Identity, out trash);
        }

        public void FindMatch(Character user)
        {
            Character pTarget = FindClosestPoints(user.ArenaQualifier.Points, user.Identity);
            if (pTarget == null || pTarget.ArenaStatus == ArenaWaitStatus.WAITING_INACTIVE)
                return;

            if (user.Map.IsPrisionMap())
            {
                Uninscribe(user);
                return;
            }

            if (pTarget.Map.IsPrisionMap())
            {
                Uninscribe(user);
                return;
            }

            ArenaMatch pMatch = new ArenaMatch(user, pTarget, PrepareMap());
            if (!pMatch.Notify())
                return;

            Character trash;
            m_pPlayers.TryRemove(user.Identity, out trash);
            m_pPlayers.TryRemove(pTarget.Identity, out trash);

            user.ArenaQualifier.Status = ArenaWaitStatus.WAITING_INACTIVE;
            pTarget.ArenaQualifier.Status = ArenaWaitStatus.WAITING_INACTIVE;
            if (!m_pMatches.TryAdd(pMatch.MapIdentity, pMatch))
            {
                user.ArenaQualifier.Status = ArenaWaitStatus.NOT_SIGNED_UP;
                pTarget.ArenaQualifier.Status = ArenaWaitStatus.NOT_SIGNED_UP;
                user.Send(ServerString.STR_ARENIC_ERROR);
                pTarget.Send(ServerString.STR_ARENIC_ERROR);
            }
        }

        private Map PrepareMap()
        {
            //Map pMap = m_pBaseMap.CreateSample();
            //pMap.Identity = (uint) ServerKernel.NextQualifierMap();
            DbDynamicMap dbMap = new DbDynamicMap
            {
                Identity = (uint)ServerKernel.NextQualifierMap(),
                MapDoc = 900000,
                Name = "ArenaQualifier",
                Type = 7,
                FileName = "newarena.DMap"
            };
            Map pMap = new Map(dbMap);
            pMap.Load();
            return pMap;
        }

        public Character FindClosestPoints(uint dwPoints, uint idSender)
        {
            if (m_pPlayers.Count <= 1) return null;
            return m_pPlayers.Values.Where(x => x.Identity != idSender 
                && (x.ArenaQualifier.Points > dwPoints*.8f && x.ArenaQualifier.Points < dwPoints * 1.25f))
                .Aggregate((x, y) => Math.Abs(x.ArenaQualifier.Points - dwPoints) < Math.Abs(y.ArenaQualifier.Points - dwPoints) ? x : y);
        }

        public bool IsWaitingMatch(uint idUser)
        {
            return m_pPlayers.ContainsKey(idUser);
        }

        public bool IsInsideMatch(uint idUser)
        {
            return m_pMatches.Values.FirstOrDefault(x => x.Identity1 == idUser || x.Identity2 == idUser) != null;
        }

        public ArenaMatch FindUser(uint idUser)
        {
            return m_pMatches.Values.FirstOrDefault(x => x.Identity1 == idUser || x.Identity2 == idUser);
        }

        public ReadOnlyCollection<ArenaMatch> ArenaMatches
        {
            get { return m_pMatches.Values.Where(x => x.ReadyToStart()).ToList().AsReadOnly(); }
        }

        public void WatchMatch(Character watcher, uint idRole)
        {
            ArenaMatch match = FindUser(idRole);
            if (match == null) // match doesn't exist
                return;

            if (FindUser(watcher.Identity) != null) // can't watch, already fighting
                return;

            if (match.Identity1 == watcher.Identity
                || match.Identity2 == watcher.Identity) // user is one of the players (might not happen)
                return;

            if (!match.IsRunning()) // fight has ended
                return;

            if (watcher.Map.IsRecordDisable())
            {
                uint dwMapId = watcher.RecordMapIdentity;
                Point pos = new Point(watcher.RecordMapX, watcher.RecordMapY);

                watcher.Map.GetRebornMap(ref dwMapId, ref pos);
                watcher.SetRecordPos(dwMapId, (ushort)pos.X, (ushort)pos.Y);
            }
            else
            {
                watcher.SetRecordPos(watcher.MapIdentity, watcher.MapX, watcher.MapY);
            }
            //match.Map.AddClient(watcher);
            watcher.ChangeMap((ushort)ThreadSafeRandom.RandGet(35, 70),
                    (ushort)ThreadSafeRandom.RandGet(35, 70), match.Map.Identity);

            match.SendToMap();
        }

        public void QuitWatch(Character pUser)
        {
            if (pUser.MapIdentity < 900000)
                return;

            MsgArenicWitness pMsg = new MsgArenicWitness
            {
                Action = MsgArenicWitness.Leave
            };
            pUser.Send(pMsg);
            pUser.ChangeMap(pUser.RecordMapX, pUser.RecordMapY, pUser.RecordMapIdentity);
        }

        public uint Matches
        {
            get { return (uint) m_pMatches.Values.Count(x => x.ReadyToStart()); }
        }

        public uint Participants
        {
            get { return (uint) m_pPlayers.Count; }
        }

        public ArenaQualifierStatus Status
        {
            get { return m_pStatus; }
        }
    }

    public enum ArenaQualifierStatus
    {
        DISABLED, ENABLED, SAVING_TABLE
    }
}