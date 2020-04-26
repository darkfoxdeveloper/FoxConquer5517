// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Generator.cs
// Last Edit: 2016/12/06 14:14
// Created: 2016/12/06 14:14

using System;
using System.Collections.Concurrent;
using System.Drawing;
using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using ServerCore.Common;
using ServerCore.Common.Enums;

namespace MsgServer.Structures.World
{
    public class Generator : IOnTimer
    {
        private const int _MAX_PER_GEN = 500;
        private const int _MIN_TIME_BETWEEN_GEN = 5;

        private DbGenerator m_dbGen;
        private DbMonstertype m_dbMonster;
        private Point m_pCenter;
        private Map m_pMap;
        private TimeOut m_pTimer;
        private Random m_pRandom = new Random();
        private Monster m_pDemo;

        private bool m_bIsDynamic = false;

        public ConcurrentDictionary<uint, Monster> Collection;

        public Generator(DbGenerator dbGen)
        {
            m_dbGen = dbGen;
        }

        public Generator(uint idMap, uint idMonster, ushort usX, ushort usY, ushort usCx, ushort usCy)
        {
            m_dbGen = new DbGenerator
            {
                Mapid = idMap,
                BoundX = usX,
                BoundY = usY,
                BoundCx = usCx,
                BoundCy = usCy,
                Npctype = idMonster,
                MaxNpc = 0,
                MaxPerGen = 0
            };

            if (!ServerKernel.Maps.TryGetValue(m_dbGen.Mapid, out m_pMap))
            {
                ServerKernel.Log.SaveLog(string.Format("Could not load map ({0}) for generator ({1})", m_dbGen.Mapid,
                    m_dbGen.Id));
                return;
            }

            if (!ServerKernel.Monsters.TryGetValue(m_dbGen.Npctype, out m_dbMonster))
            {
                ServerKernel.Log.SaveLog(string.Format("Could not load monster ({0}) for generator ({1})",
                    m_dbGen.Npctype, m_dbGen.Id));
                return;
            }

            m_pCenter = new Point(m_dbGen.BoundX + (m_dbGen.BoundCx / 2), m_dbGen.BoundY + (m_dbGen.BoundCy / 2));
            m_bIsDynamic = true;
            Collection = new ConcurrentDictionary<uint, Monster>();
            FirstGeneration();
        }

        public uint Identity
        {
            get { return m_dbGen.Id; }
        }

        public uint RoleType
        {
            get { return m_dbGen.Npctype; }
        }

        public int RestSeconds
        {
            get { return m_dbGen.RestSecs; }
        }

        public uint MapIdentity
        {
            get { return m_dbGen.Mapid; }
        }

        public string MonsterName
        {
            get { return m_dbMonster.Name; }
        }

        public bool Create()
        {
            if (!ServerKernel.Maps.TryGetValue(m_dbGen.Mapid, out m_pMap))
            {
                ServerKernel.Log.SaveLog(string.Format("Could not load map ({0}) for generator ({1})", m_dbGen.Mapid,
                    m_dbGen.Id));
                return false;
            }

            if (!ServerKernel.Monsters.TryGetValue(m_dbGen.Npctype, out m_dbMonster))
            {
                ServerKernel.Log.SaveLog(string.Format("Could not load monster ({0}) for generator ({1})",
                    m_dbGen.Npctype, m_dbGen.Id));
                return false;
            }

            if (!m_pMap.Loaded)
                m_pMap.Load();

            m_dbGen.MaxNpc = m_dbGen.MaxNpc <= 0 ? 1 : m_dbGen.MaxNpc;
            m_dbGen.MaxPerGen = m_dbGen.MaxPerGen;
            m_dbGen.RestSecs += _MIN_TIME_BETWEEN_GEN;
            m_dbGen.MaxPerGen = (m_dbGen.MaxPerGen > _MAX_PER_GEN ? _MAX_PER_GEN : m_dbGen.MaxPerGen);
            m_pCenter = new Point(m_dbGen.BoundX + (m_dbGen.BoundCx / 2), m_dbGen.BoundY + (m_dbGen.BoundCy / 2));

            m_pTimer = new TimeOut(m_dbGen.RestSecs);
            Collection = new ConcurrentDictionary<uint, Monster>();
            return true;
        }

        private bool NewPoint(out Point pPos)
        {
            pPos = default(Point);
            try
            {
                ushort x =
                    (ushort)Math.Max(m_dbGen.BoundX, m_pRandom.Next(m_dbGen.BoundX, m_dbGen.BoundX + m_dbGen.BoundCx));
                ushort y =
                    (ushort)Math.Max(m_dbGen.BoundY, m_pRandom.Next(m_dbGen.BoundY, m_dbGen.BoundY + m_dbGen.BoundCy));

                if (!m_pMap.IsStandEnable(x, y))
                    return false;

                pPos = new Point(x, y);
                return true;
            }
            catch
            {
                pPos = default(Point);
                return false;
            }
        }

        private bool Monster(out Monster mob)
        {
            mob = default(Monster);
            Point pos = new Point(-1, -1);
            try
            {
                if (!NewPoint(out pos))
                    return false;

                if (pos == default(Point) || pos.X == -1 || pos.Y == -1 || pos.X == 0 || pos.Y == 0)
                    return false;

                ushort x = (ushort)pos.X, y = (ushort)pos.Y;

                mob = new Monster(m_dbMonster, m_pMap.NextMonsterIdentity, this)
                {
                    Action = EntityAction.STAND,
                    AttackHitRate = (ushort)m_dbMonster.AttackSpeed,
                    AttackRange = m_dbMonster.AttackRange,
                    Direction = (FacingDirection)m_pRandom.Next(0, 7),
                    Life = (uint)m_dbMonster.Life,
                    Mana = (ushort)m_dbMonster.Mana,
                    ViewRange = m_dbMonster.ViewRange,
                    Level = (byte)m_dbMonster.Level,
                    MapIdentity = m_pMap.Identity,
                    MapX = x,
                    MapY = y,
                    Lookface = m_dbMonster.Lookface
                };
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void FirstGeneration()
        {
            m_pTimer = new TimeOut(m_dbGen.RestSecs);
            m_pTimer.Startup(m_dbGen.RestSecs);
            m_pDemo = new Monster(m_dbMonster, 0, this)
            {
                Action = EntityAction.STAND,
                AttackHitRate = (ushort)m_dbMonster.AttackSpeed,
                AttackRange = m_dbMonster.AttackRange,
                Direction = (FacingDirection)m_pRandom.Next(0, 7),
                Life = (uint)m_dbMonster.Life,
                Mana = (ushort)m_dbMonster.Mana,
                ViewRange = m_dbMonster.ViewRange,
                Level = (byte)m_dbMonster.Level,
                MapIdentity = m_pMap.Identity,
                MapX = 0,
                MapY = 0,
                Lookface = m_dbMonster.Lookface
            };

            for (int i = 0; i < m_dbGen.MaxPerGen; i++)
            {
                Monster pRole;
                if (Monster(out pRole))
                {
                    if (Collection.Count >= m_dbGen.MaxPerGen)
                        return;
                    Collection.TryAdd(pRole.Identity, pRole);
                    m_pMap.AddMonster(pRole);
                }
            }
        }

        public bool SpawnMob(Monster pRole)
        {
            try
            {
                Collection.TryAdd(pRole.Identity, pRole);
                pRole.Map.AddMonster(pRole);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Deactivate()
        {
            foreach (var mob in Collection.Values)
            {
                m_pMap.RemoveMonster(mob);
            }
            Collection.Clear();
        }

        public void OnTimer()
        {
            try
            {
                if (m_pTimer.ToNextTime(m_dbGen.RestSecs))
                {
                    if (Collection.Count < m_dbGen.MaxNpc) // spawn new mobs
                    {
                        int gen = m_dbGen.MaxNpc - Collection.Count;
                        if (gen > m_dbGen.MaxPerGen)
                            gen = m_dbGen.MaxPerGen;
                        if (gen > _MAX_PER_GEN)
                            gen = _MAX_PER_GEN;
                        for (int i = 0; i < gen; i++)
                        {
                            Monster pRole;
                            if (!Monster(out pRole))
                                continue;

                            Collection.TryAdd(pRole.Identity, pRole);
                            m_pMap.AddMonster(pRole);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        public Point GetCenter()
        {
            return m_pCenter;
        }

        public bool IsTooFar(ushort x, ushort y, int nRange)
        {
            return !(x >= m_dbGen.BoundX - nRange
                     && x < m_dbGen.BoundX + m_dbGen.BoundCx + nRange
                     && y >= m_dbGen.BoundY - nRange
                     && y < m_dbGen.BoundY + m_dbGen.BoundCy + nRange);
        }

        public bool IsInRegion(int x, int y)
        {
            return x >= m_dbGen.BoundX && x < m_dbGen.BoundX + m_dbGen.BoundCx
                   && y >= m_dbGen.BoundY && y < m_dbGen.BoundY + m_dbGen.BoundCy;
        }

        public int GetWidth()
        {
            return m_dbGen.BoundCx;
        }

        public int GetHeight()
        {
            return m_dbGen.BoundCy;
        }

        public int GetPosX()
        {
            return m_dbGen.BoundX;
        }

        public int GetPosY()
        {
            return m_dbGen.BoundY;
        }
    }
}
