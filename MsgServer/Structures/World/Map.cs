// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Map.cs
// Last Edit: 2016/11/24 08:31
// Created: 2016/11/23 10:26

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using DB.Entities;
using MsgServer.Network;
using MsgServer.Network.GameServer.Handlers;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using MsgServer.Structures.Qualifier;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.World
{
    public sealed class Map : Floor
    {
        public const uint DEFAULT_LIGHT_RGB = 0xFFFFFF;

        private DbMap m_dbMap;
        private DbDynamicMap m_dbDynamicMap;
        private ulong m_ulFlag;
        private KeyValuePair<int, Point> m_kRebirthPoint;
        private int m_nNextMonsterId = IdentityRange.MONSTERID_FIRST;
        private int m_nPetId = IdentityRange.CALLPETID_FIRST;

        public ConcurrentDictionary<uint, Character> Players = new ConcurrentDictionary<uint, Character>();
        public ConcurrentDictionary<uint, IScreenObject> GameObjects = new ConcurrentDictionary<uint, IScreenObject>(); // npcs
        public ConcurrentDictionary<uint, DbRegion> Regions = new ConcurrentDictionary<uint, DbRegion>();
        public ConcurrentDictionary<uint, IPassway> Portals = new ConcurrentDictionary<uint, IPassway>();

        public uint RebornMapIdentity { get { return (uint)m_kRebirthPoint.Key; } }
        public KeyValuePair<int, Point> RevivePoint { get { return m_kRebirthPoint; } }
        private uint m_dwBaseIdentity;

        private int m_floorItem = IdentityRange.MAPITEM_FIRST;
        public int FloorItem
        {
            get
            {
                if (m_floorItem > IdentityRange.MAPITEM_LAST)
                    m_floorItem = IdentityRange.MAPITEM_FIRST;
                return Interlocked.Increment(ref m_floorItem);
            }
        }

        public Map(DbMap dbMap)
            : base(dbMap.Path)
        {
            m_dbMap = dbMap;

            m_dwBaseIdentity = dbMap.MapDoc;
            m_ulFlag = dbMap.Type;

            m_kRebirthPoint =
                new KeyValuePair<int, Point>(int.Parse(m_dbMap.RebornMap.ToString()),
                new Point(int.Parse(m_dbMap.PortalX.ToString()), int.Parse(m_dbMap.PortalY.ToString())));
        }

        public Map(DbDynamicMap dbMap)
            : base(dbMap.FileName)
        {
            m_dbDynamicMap = dbMap;

            m_dwBaseIdentity = dbMap.MapDoc;
            m_ulFlag = dbMap.Type;

            m_kRebirthPoint =
                new KeyValuePair<int, Point>(int.Parse(m_dbDynamicMap.RebornMapid.ToString()),
                new Point(int.Parse(m_dbDynamicMap.Portal0X.ToString()), int.Parse(m_dbDynamicMap.Portal0Y.ToString())));
        }

        public int Width { get { return Boundaries.Width; } }
        public int Height { get { return Boundaries.Height; } }

        public uint Identity
        {
            get { return m_dbMap == null ? m_dbDynamicMap.Identity : m_dbMap.Identity; }
            set
            {
                if (IsDynamicMap()) m_dbDynamicMap.Identity = value;
                else m_dbMap.Identity = value;
            }
        }

        public uint MapDoc
        {
            get { return m_dbMap == null ? m_dbDynamicMap.MapDoc : m_dbMap.MapDoc; }
            set { if (IsDynamicMap()) m_dbDynamicMap.MapDoc = value; }
        }

        public uint Type
        {
            get { return m_dbMap == null ? m_dbDynamicMap.Type : m_dbMap.Type; }
            set
            {
                if (m_dbMap != null) m_dbMap.Type = value;
                else m_dbDynamicMap.Type = value;
            }
        }

        public string Name
        {
            get { return m_dbMap == null ? m_dbDynamicMap.Name : m_dbMap.Name; }
            set
            {
                if (m_dbMap != null) m_dbMap.Name = value;
                else m_dbDynamicMap.Name = value;
            }
        }

        public ulong WarFlag { get { return m_ulFlag; } }

        public uint RebornMapId { get; set; }

        public Point MapRebornPoint { get; set; }

        public uint OwnerIdentity
        {
            get
            {
                if (IsDynamicMap())
                    return m_dbDynamicMap.OwnerId;
                return m_dbMap.OwnerId;
            }
            set
            {
                if (IsDynamicMap())
                    m_dbDynamicMap.OwnerId = value;
                m_dbMap.OwnerId = value;
            }
        }

        public Map CreateSample()
        {
            return new Map(m_dbMap);
        }

        public uint BaseIdentity
        {
            get { return m_dwBaseIdentity; }
            set { m_dwBaseIdentity = value; }
        }

        public override bool Load()
        {
            if (base.Load())
            {
                Loaded = true;
                return true;
            }
            return false;
        }

        public uint NextMonsterIdentity
        {
            get
            {
                if (m_nNextMonsterId >= IdentityRange.NPCSERVERID_LAST)
                    m_nNextMonsterId = IdentityRange.MONSTERID_FIRST;
                return (uint)Interlocked.Increment(ref m_nNextMonsterId);
            }
        }

        public uint NextPetIdentity
        {
            get
            {
                if (m_nPetId >= IdentityRange.CALLPETID_LAST)
                    m_nPetId = IdentityRange.CALLPETID_FIRST;
                return (uint)Interlocked.Increment(ref m_nPetId);
            }
        }

        /// <summary>
        /// Checks if the map is a pk field. Wont add pk points.
        /// </summary>
        public bool IsPkField() { return (Type & (uint)MapTypeFlags.PK_FIELD) != 0; }
        /// <summary>
        /// Disable teleporting by skills or scrolls.
        /// </summary>
        public bool IsChgMapDisable() { return (Type & (uint)MapTypeFlags.CHANGE_MAP_DISABLE) != 0; }
        /// <summary>
        /// Disable recording the map position into the database.
        /// </summary>
        public bool IsRecordDisable() { return (Type & (uint)MapTypeFlags.RECORD_DISABLE) != 0; }
        /// <summary>
        /// Disable team creation into the map.
        /// </summary>
        public bool IsTeamDisable() { return (Type & (uint)MapTypeFlags.TEAM_DISABLE) != 0; }
        /// <summary>
        /// Disable use of pk on the map.
        /// </summary>
        public bool IsPkDisable() { return (Type & (uint)MapTypeFlags.PK_DISABLE) != 0; }
        /// <summary>
        /// Disable teleporting by actions.
        /// </summary>
        public bool IsTeleportDisable() { return (Type & (uint)MapTypeFlags.TELEPORT_DISABLE) != 0; }
        /// <summary>
        /// Checks if the map is a syndicate map
        /// </summary>
        /// <returns></returns>
        public bool IsSynMap() { return (Type & (uint)MapTypeFlags.GUILD_MAP) != 0; }
        /// <summary>
        /// Checks if the map is a prision
        /// </summary>
        public bool IsPrisionMap() { return (Type & (uint)MapTypeFlags.PRISON_MAP) != 0; }
        /// <summary>
        /// If the map enable the fly skill.
        /// </summary>
        public bool IsWingDisable() { return (Type & (uint)MapTypeFlags.WING_DISABLE) != 0; }
        /// <summary>
        /// Check if the map is in war.
        /// </summary>
        public bool IsWarTime() { return (WarFlag & 1) != 0; }
        /// <summary>
        /// Check if the map is the training ground. [1039]
        /// </summary>
        public bool IsTrainingMap() { return Identity == 1039; }
        /// <summary>
        /// Check if its the family (clan) map.
        /// </summary>
        public bool IsFamilyMap() { return (Type & (uint)MapTypeFlags.FAMILY) != 0; }
        /// <summary>
        /// If the map enables booth to be built.
        /// </summary>
        public bool IsBoothEnable() { return (Type & (uint)MapTypeFlags.BOOTH_ENABLE) != 0; }
        public bool IsDeadIsland() { return (Type & (uint)MapTypeFlags.DEAD_ISLAND) != 0; }
        public bool IsPkGameMap() { return (Type & (uint)MapTypeFlags.PK_GAME) != 0; }
        public bool IsMineField() { return (Type & (uint)MapTypeFlags.MINE_FIELD) != 0; }
        public bool IsSkillMap() { return (Type & (uint)MapTypeFlags.SKILL_MAP) != 0; }
        public bool IsLineSkillMap()  { return (Type & (ulong) MapTypeFlags.LINE_SKILL_ONLY) != 0; }

        public bool IsDynamicMap()
        {
            return Identity > 999999;
        }

        public void SendToRange(byte[] pMsg, ushort x, ushort y)
        {
            foreach (Character pUser in Players.Values)
            {
                if (Calculations.InScreen(x, y, pUser.MapX, pUser.MapY))
                {
                    pUser.Send(pMsg);
                }
            }
        }

        public void Pos2Lt(ref Point pTarget, Point pos, int nRange)
        {
            pTarget.X = pos.X - nRange;
            pTarget.Y = pos.Y - nRange;
        }

        public void PosAdd(ref Point pTarget, int nOffsetX, int nOffsetY)
        {
            pTarget.X += nOffsetX;
            pTarget.Y += nOffsetY;
        }

        public void PosSub(ref Point pTarget, int nOffsetX, int nOffsetY)
        {
            pTarget.X -= nOffsetX;
            pTarget.Y -= nOffsetY;
        }

        public List<Point> For9Blocks(int nOffsetX, int nOffsetY)
        {
            var temp = new List<Point>(19 * 19 + 1);
            for (int x = Math.Max(nOffsetX - 1, 0); x <= nOffsetX + 1; x++)
                for (int y = Math.Max(nOffsetY - 1, 0); y <= nOffsetY + 1; y++)
                    temp.Add(new Point(x, y));
            return temp;
        }

        public void SendMsgToMap(string szMsg, ChatTone pChat = ChatTone.TOP_LEFT)
        {
            foreach (var pUser in Players.Values)
                pUser.Send(szMsg, pChat);
        }

        public bool QueryRegion(int regionType, ushort x, ushort y)
        {
            return Regions.Values.Where(re => (x > re.BoundX && x < re.BoundX + re.BoundCX) && (y > re.BoundY && y < re.BoundY + re.BoundCY)).Any(region => region.Type == regionType);
        }

        public double GetDistance(IScreenObject obj1, IScreenObject obj2)
        {
            return Calculations.GetDistance(obj1.MapX, obj1.MapY, obj2.MapX, obj2.MapY);
        }

        public double GetDistance(ushort x, ushort y, ushort nx, ushort ny)
        {
            return Calculations.GetDistance(x, y, nx, ny);
        }

        public bool IsInScreen(IScreenObject obj1, IScreenObject obj2)
        {
            return Calculations.InScreen(obj1.MapX, obj1.MapY, obj2.MapX, obj2.MapY);
        }

        public bool IsInScreen(Point p, IScreenObject obj2)
        {
            return Calculations.InScreen((ushort)p.X, (ushort)p.Y, obj2.MapX, obj2.MapY);
        }

        public IScreenObject QueryRole(uint idRole)
        {
            if (idRole >= 1000000)
                return Players.Values.FirstOrDefault(x => x.Identity == idRole);
            return GameObjects.Values.FirstOrDefault(x => x.Identity == idRole);
        }

        public IScreenObject QueryRole(ushort x, ushort y)
        {
            return ((Players.Values.FirstOrDefault(p => p.MapX == x && p.MapY == y) ??
                                GameObjects.Values.FirstOrDefault(p => p.MapX == x && p.MapY == y)));
        }

        public IList<IRole> QueryRoleInRange(IScreenObject pSender, int nRange)
        {
            List<IRole> pRoles = Players.Values.Where(x => x.GetDistance(pSender.MapX, pSender.MapY) <= nRange).Cast<IRole>().ToList();
            pRoles.AddRange(GameObjects.Values.Where(x => Calculations.GetDistance(x.MapX, x.MapY, pSender.MapX, pSender.MapY) <= nRange).Cast<IRole>());
            return pRoles;
        }

        public int Pos2Index(int x, int y, int cx, int cy) { return (x + y * cx); }
        public int Index2X(int idx, int cx, int cy) { return (idx % cy); }
        public int Index2Y(int idx, int cx, int cy) { return (idx / cy); }

        //============================= Item part======================================================================
        public bool FindDropItemCell(int nRange, ref Point pPos)
        {
            int nSize = nRange * 2 + 1; // set the size, remember, if value is 2, it's 2 for each side, that's why we multiply
            int nBufSize = nSize ^ 2;

            if (IsLayItemEnable(pPos.X, pPos.Y))
            {
                return true;
            }

            for (int i = 0; i < 8; i++)
            {
                int newX = pPos.X + Handlers.WALK_X_COORDS[i];
                int newY = pPos.Y + Handlers.WALK_Y_COORDS[i];
                if (IsLayItemEnable(newX, newY))
                {
                    pPos.X = newX;
                    pPos.Y = newY;
                    return true;
                }
            }

            for (int i = 0; i < 24; i++)
            {
                int newX = pPos.X + Handlers.DELTA_WALK_X_COORDS[i];
                int newY = pPos.Y + Handlers.DELTA_WALK_Y_COORDS[i];
                if (IsLayItemEnable(newX, newY))
                {
                    pPos.X = newX;
                    pPos.Y = newY;
                    return true;
                }
            }

            int nIndex = Calculations.Random.Next(nBufSize);
            var posTest = new Point();
            int nLeft = pPos.X - nRange;
            int nTop = pPos.Y - nRange;
            posTest.X = nLeft + Index2X(nIndex, nSize, nSize);
            posTest.Y = nTop + Index2Y(nIndex, nSize, nSize);
            if (IsLayItemEnable(posTest.X, posTest.Y))
            {
                pPos = posTest;
                return true;
            }

            if (nRange < 2)
                return false;

            var setItem = CollectMapItem(ref pPos, nRange);

            int nMinRange = nRange + 1;
            bool ret = false;
            var posFree = new Point();
            for (int i = Math.Max(pPos.X - nRange, 0); i <= pPos.X + nRange && i < Width; i++)
            {
                for (int j = Math.Max(pPos.Y - nRange, 0); j <= pPos.Y + nRange && j < Height; j++)
                {
                    int idx = Pos2Index(i - (pPos.X - nRange), j - (pPos.Y - nRange), nSize, nSize);

                    if (idx >= 0 && idx < nBufSize)
                        if (setItem.FirstOrDefault(x => Pos2Index(x.MapX - i + nRange, x.MapY - j + nRange, nRange, nRange) == idx) != null)
                            continue;

                    if (IsLayItemEnable(pPos.X, pPos.Y))
                    {
                        double nDistance = Calculations.GetDistance((ushort)i, (ushort)j, (ushort)pPos.X, (ushort)pPos.Y);
                        if (nDistance < nMinRange)
                        {
                            nMinRange = (int)nDistance;
                            posFree.X = i;
                            posFree.Y = j;
                            ret = true;
                        }
                    }
                }
            }

            if (ret)
            {
                pPos = posFree;
                return true;
            }

            return true;
        }

        public List<IScreenObject> CollectMapThing(ref Point pPos, int nRange)
        {
            var temp = new List<IScreenObject>();
            foreach (var obj in GameObjects.Values)
                if (IsInScreen(pPos, obj))
                {
                    int idx = Pos2Index(obj.MapX - pPos.X + nRange, obj.MapY - pPos.Y + nRange, nRange, nRange);
                    temp.Add(obj);
                }
            return temp.Count <= 0 ? null : temp;
        }

        public List<IRole> CollectMapThing(int nRange, ref Point pPos)
        {
            var list = new List<IRole>();
            foreach (var pUser in Players.Values)
                if (Calculations.GetDistance((ushort)pPos.X, (ushort)pPos.Y, pUser.MapX, pUser.MapY) <= nRange)
                    list.Add(pUser);
            foreach (var pRole in GameObjects.Values.Where(x => x is IRole))
                if (Calculations.GetDistance((ushort)pPos.X, (ushort)pPos.Y, pRole.MapX, pRole.MapY) <= nRange)
                    list.Add(pRole as IRole);
            return list;
        }

        public List<IRole> CollectMapThing(int nRange, Point pPos)
        {
            var pTemp = GameObjects.Values.OfType<IRole>().Where(obj => Calculations.GetDistance((ushort)pPos.X, (ushort)pPos.Y, obj.MapX, obj.MapY) < nRange).ToList();
            pTemp.AddRange(from obja in Players.Values select obja into obj where Calculations.GetDistance((ushort)pPos.X, (ushort)pPos.Y, obj.MapX, obj.MapY) < nRange select obj as IRole);
            return pTemp;
        }

        public List<MapItem> CollectMapItem(ref Point pPos, int nRange)
        {
            var list = new List<MapItem>();
            foreach (var item in GameObjects.Values.Where(x => x is MapItem).Cast<MapItem>())
                if (Calculations.GetDistance((ushort)pPos.X, (ushort)pPos.Y, item.MapX, item.MapY) <= nRange)
                {
                    // int idx = Pos2Index(item.MapX - pPos.X + nRange, item.MapY - pPos.Y + nRange, nRange, nRange);
                    list.Add(item);
                }
            return list;
        }

        public bool IsLayItemEnable(int x, int y)
        {
            return this[x, y].Access > TileType.TERRAIN && GameObjects.Values.FirstOrDefault(a => a.MapX == x && a.MapY == y) == null;
        }

        public bool IsAltOver(Point point, int nAlt)
        {
            if (!IsValidPoint(point))
                return false;

            if (this[point.X, point.Y].Elevation > nAlt)
                return true;

            return false;
        }

        public bool IsValidPoint(Point pos)
        {
            return IsValidPoint(pos.X, pos.Y);
        }

        public bool IsValidPoint(int x, int y)
        {
            return (x >= 0 && x < Width && y >= 0 && y < Height);
        }

        public bool GetRebornMap(ref uint mapId, ref Point pposTarget)
        {
            Map targetMap = ServerKernel.Maps.Values.FirstOrDefault(x => x.Identity == RebornMapId);
            if (targetMap == null)
            {
                ServerKernel.Log.SaveLog(string.Format("ERROR: Could not find reborn map [{0}] to map [{1}]", RebornMapId, Identity), false, LogType.WARNING);
                return false;
            }

            var posNew = new Point(targetMap.MapRebornPoint.X, targetMap.MapRebornPoint.Y);
            if (posNew.X == 0 || posNew.Y == 0)
            {
                posNew.X = 430;
                posNew.Y = 378;
                mapId = 1002;
                return false;
            }

            mapId = RebornMapId != 0 ? RebornMapId : Identity;
            pposTarget = posNew;
            return true;
        }

        public bool IsStandEnable(ushort nPosX, ushort nPosY)
        {
            if (nPosX > 0 && nPosX < Width && nPosY > 0 && nPosY < Height)
                return this[nPosX, nPosY].Access > TileType.TERRAIN;
            return false;
        }

        public bool IsMoveEnable(int x, int y, int nDir, int nSizeAdd, int nClimbCap)
        {
            if (nSizeAdd > 4)
                nSizeAdd = 4;

            ushort newX = (ushort)(x + Handlers.WALK_X_COORDS[nDir]);
            ushort newY = (ushort)(y + Handlers.WALK_Y_COORDS[nDir]);

            if (!IsValidPoint(newX, newY))
                return false;

            int nElevation = 0;
            int nOldElevation = this[x, y].Elevation;
            int nNewElevation = this[newX, newY].Elevation;
            if (nOldElevation >= nNewElevation)
                nElevation = nOldElevation - nNewElevation;
            else
                nElevation = nNewElevation - nOldElevation;

            if (nClimbCap > 0 && nElevation > nClimbCap)
                return false;

            if (nSizeAdd > 0 && nSizeAdd <= 2)
            {
                int nMoreDir = (nDir % 2) > 0 ? 1 : 2;
                for (int i = -1 * nMoreDir; i <= nMoreDir; i++)
                {
                    int nDir2 = (nDir + i + 8) % 8;
                    int nNewX2 = newX + Handlers.WALK_X_COORDS[nDir2];
                    int nNewY2 = newY + Handlers.WALK_Y_COORDS[nDir2];
                    if (!IsValidPoint(nNewX2, nNewY2))
                        return false;
                }
            }
            else if (nSizeAdd > 2)
            {
                int nRange = (nSizeAdd + 1) / 2;
                for (ushort i = (ushort)(newX - nRange); i <= newX + nRange; i++)
                {
                    for (ushort j = (ushort)(newY - nRange); j <= newY + nRange; j++)
                    {
                        if (GetDistance(i, j, (ushort)x, (ushort)y) > nRange)
                        {
                            if (!IsValidPoint(i, j))
                                return false;
                        }
                    }
                }
            }

            if (this[newX, newY].Access < TileType.MONSTER)
                return false;

            return true;
        }

        public void SetStatus(byte value, bool flag)
        {
            ulong oldFlag = WarFlag;
            if (flag)
                m_ulFlag |= value;
            else
                m_ulFlag &= (byte)~value;

            if (WarFlag != oldFlag)
                SendToAll(new MsgMapInfo(Identity, MapDoc, WarFlag));
        }

        public void SendToAll(MsgMapInfo pMsg)
        {
            foreach (var plr in Players.Values)
                plr.Send(pMsg);
        }

        public bool IsSuperposition(IRole pRole)
        {
            return Players.Values.Count(x => x.MapX == pRole.MapX && x.MapY == pRole.MapY) > 0;
        }

        public void DelNpcByType(uint nType)
        {
            foreach (var obj in GameObjects.Values.Where(x => x is DynamicNpc))
            {
                var pNpc = obj as DynamicNpc;
                if (pNpc.Kind == nType)
                    pNpc.DelNpc();
            }
        }

        #region Screen Objects
        public void AddClient(Character pRole)
        {
            if (!Loaded) Load();

            if (pRole.Map != null)
                pRole.Map.RemoveClient(pRole.Identity);

            Character trash;
            Players.TryRemove(pRole.Identity, out trash);

            if (Players.TryAdd(pRole.Identity, pRole))
            {
                pRole.Map = this;
                pRole.Owner.Tile = base[pRole.MapX, pRole.MapY];
                pRole.Elevation = pRole.Owner.Tile.Elevation;
            }
        }

        public void RemoveClient(uint idUser)
        {
            try
            {
                Character pTrash;

                if (!Players.TryRemove(idUser, out pTrash) || pTrash.Screen == null) return;

                //if (ServerKernel.ArenaQualifier.IsInsideMatch(idUser))
                //{
                //    ArenaMatch pMatch = ServerKernel.ArenaQualifier.FindUser(idUser);
                //    if (pMatch != null && pMatch.ReadyToStart())
                //    {
                //        pMatch.GiveUp(pTrash);
                //    }
                //}

                pTrash.Screen.RemoveFromObservers();
                pTrash.Screen.Clear();
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
            }
        }

        public bool AddNpc(IScreenObject pNpc)
        {
            if (GameObjects.TryAdd(pNpc.Identity, pNpc))
            {
                pNpc.MapIdentity = Identity;
                foreach (var player in Players.Values.Where(x => IsInScreen(x, pNpc)))
                {
                    pNpc.SendSpawnTo(player);
                }
                return true;
            }
            return false;
        }

        public bool RemoveNpc(IScreenObject pNpc)
        {
            if (GameObjects.TryRemove(pNpc.Identity, out pNpc))
            {
                foreach (var player in Players.Values)
                    player.Screen.Delete(pNpc.Identity);
                return true;
            }
            return GameObjects.TryRemove(pNpc.Identity, out pNpc);
        }

        public void AddDynaNpc(DynamicNpc npc)
        {
            if (GameObjects.TryAdd(npc.Identity, npc))
            {
                npc.Map = this;
                foreach (Character observer in Players.Values)
                    if (Calculations.InScreen(observer.MapX, observer.MapY,
                        npc.MapX, npc.MapY))
                    {
                        observer.Screen.Add(npc);
                        npc.SendSpawnTo(observer);
                    }
            }
        }

        public void AddItem(MapItem mapItem)
        {
            if (!Loaded) Load();

            if (GameObjects.TryAdd(mapItem.Identity, mapItem))
            {
                mapItem.MapIdentity = Identity;
                mapItem.Map = this;
                foreach (var client in Players.Values)
                    if (Calculations.GetDistance(mapItem.MapX, mapItem.MapY, client.MapX, client.MapY) <= Calculations.SCREEN_DISTANCE)
                    {
                        client.Screen.Add(mapItem);
                        mapItem.SendSpawnTo(client);
                    }
            }
        }

        public void RemoveItem(MapItem pItem, bool bDel = true)
        {
            IScreenObject pTrash;
            if (GameObjects.TryRemove(pItem.Identity, out pTrash))
            {
                foreach (var pRole in Players.Values)
                {
                    if (Calculations.GetDistance(pRole.MapX, pRole.MapY, pItem.MapX, pItem.MapY) <= Calculations.SCREEN_DISTANCE)
                    {
                        pRole.Screen.Delete(pItem.Identity);
                        pItem.SendRemoveFromScreen(pRole);
                        // pRole.Send(pTrash.GetPacket());
                        // if (bDel) pTrash.Delete();
                    }
                }
            }
        }

        public void AddMonster(Monster pRole)
        {
            if (!Loaded) Load();

            if (pRole.Map != null) pRole.Map.RemoveMonster(pRole);

            if (GameObjects.TryAdd(pRole.Identity, pRole))
            {
                pRole.MapIdentity = Identity;
                pRole.Map = this;
                foreach (var pUser in Players.Values)
                {
                    if (Calculations.InScreen(pRole.MapX, pRole.MapY, pUser.MapX, pUser.MapY))
                    {
                        pUser.Screen.Add(pRole);
                        pRole.SendSpawnTo(pUser);
                        pUser.Send(new MsgAction(pRole.Identity, 0, pRole.MapX, pRole.MapY,
                            GeneralActionType.SPAWN_EFFECT)
                        {
                            Direction = FacingDirection.WEST
                        });
                    }
                }
            }
        }

        public void RemoveMonster(Monster pRole)
        {
            IScreenObject pTrash;
            if (GameObjects.TryRemove(pRole.Identity, out pTrash))
            {
                foreach (var pUser in Players.Values/*.Where(pUser => Calculations.InScreen(pRole.MapX, pRole.MapY, pUser.MapX, pUser.MapY))*/)
                {
                    pUser.Screen.Delete(pRole.Identity);
                }
            }
        }

        public IScreenObject FindAroundRole(IScreenObject pOwner, uint idRole)
        {
            IScreenObject ret;
            if (idRole >= 1000000)
                ret = Players.Values.FirstOrDefault(x => x.Identity == idRole);
            else
                ret = GameObjects.Values.FirstOrDefault(x => x.Identity == idRole);
            if (ret == null) return null;
            return Calculations.InScreen(pOwner.MapX, pOwner.MapY, ret.MapX, ret.MapY) ? ret : null;
        }

        public bool ItemAccessible(ushort x, ushort y)
        {
            return GameObjects.Values.All(item => item.MapX != x || item.MapY != y) && GameObjects.Values.All(item => item.MapX != x || item.MapY != y);
        }
        #endregion

        /// <summary>
        /// This method samples the map for elevation problems. If a player is jumping, this method will sample
        /// the map for key elevation changes and check that the player is not wall jumping. It checks all tiles
        /// in between the player and the jumping destination. 
        /// </summary>
        /// <param name="distance">The distance between the two points.</param>
        /// <param name="startX">The starting x-coordinate.</param>
        /// <param name="startY">The starting y-coordinate.</param>
        /// <param name="deltaX">The difference between the starting x-coordinate and final x-coordinate.</param>
        /// <param name="deltaY">The difference between the starting y-coordinate and final y-coordinate.</param>
        /// <param name="elevation">The initial elevation of the player.</param>
        public bool SampleElevation(int distance, ushort startX, ushort startY,
            int deltaX, int deltaY, short elevation)
        {
            // Initialize variables and sample the area between the start and final position.
            int violations = 0;
            for (int index = 1; index <= distance; index++)
            {
                int x = startX + ((int)(((double)(index * deltaX)) / distance));
                int y = startY + ((int)(((double)(index * deltaY)) / distance));
                if (!Calculations.WithinElevation(this[x, y].Elevation, elevation))
                    if (++violations > 1) return false;
            }
            return true;
        }

        public void SendMessageToMap(string szMsg, ChatTone chatTone)
        {
            foreach (var plr in Players.Values)
                plr.Send(szMsg, chatTone);
        }

        public bool Save()
        {
            return IsDynamicMap() ? Database.DynamicMapRepository.SaveOrUpdate(m_dbDynamicMap) : Database.Maps.SaveOrUpdate(m_dbMap);
        }

        public void Send(byte[] pBuf)
        {
            foreach (var plr in Players.Values)
                plr.Send(pBuf);
        }
    }
}