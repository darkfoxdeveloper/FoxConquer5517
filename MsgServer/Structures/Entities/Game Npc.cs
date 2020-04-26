// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Game Npc.cs
// Last Edit: 2016/12/05 10:42
// Created: 2016/12/05 10:42

using DB.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.World;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Entities
{
    public sealed class GameNpc : IScreenObject, INpc
    {
        private DbNpc m_dbNpc;
        private MsgNpcInfo m_pPacket;
        private Map m_pMap;

        private uint m_dwMapId = 5000;
        private short m_sElevation;

        public GameNpc(DbNpc pNpc)
        {
            m_dbNpc = pNpc;
            m_pPacket = new MsgNpcInfo
            {
                Identity = pNpc.Id,
                MapX = pNpc.Cellx,
                MapY = pNpc.Celly,
                Kind = pNpc.Type,
                Sort = pNpc.Sort,
                Lookface = pNpc.Lookface
            };
        }

        public uint Identity
        {
            get { return m_dbNpc.Id; }
        }

        public uint MapIdentity
        {
            get { return m_dwMapId; }
            set { m_dwMapId = value; }
        }

        public string Name
        {
            get { return m_pPacket.Name; }
            set
            {
                m_pPacket.Name = value.Substring(0, value.Length > 16 ? 16 : value.Length);
            }
        }

        public ushort MapX
        {
            get { return m_dbNpc.Cellx; }
            set
            {
                m_dbNpc.Cellx = value;
                m_pPacket.MapX = value;
            }
        }

        public ushort MapY
        {
            get { return m_dbNpc.Celly; }
            set
            {
                m_dbNpc.Celly = value;
                m_pPacket.MapY = value;
            }
        }

        public short Elevation
        {
            get { return m_sElevation; }
        }

        public Map Map
        {
            get { return m_pMap; }
            set { m_pMap = value; }
        }

        public IScreenObject FindAroundRole(uint idRole)
        {
            return Map.FindAroundRole(this, idRole);
        }

        public void SendSpawnTo(Character pUser)
        {
            pUser.Send(m_pPacket);
        }

        #region Npc Task and Data

        public uint OwnerIdentity
        {
            get { return m_dbNpc.Ownerid; }
            set { m_dbNpc.Ownerid = value; }
        }

        public bool Vending { get; set; }
        public uint Task0 { get { return m_dbNpc.Task0; } }
        public uint Task1 { get { return m_dbNpc.Task1; } }
        public uint Task2 { get { return m_dbNpc.Task2; } }
        public uint Task3 { get { return m_dbNpc.Task3; } }
        public uint Task4 { get { return m_dbNpc.Task4; } }
        public uint Task5 { get { return m_dbNpc.Task5; } }
        public uint Task6 { get { return m_dbNpc.Task6; } }
        public uint Task7 { get { return m_dbNpc.Task7; } }
        public int Data0 { get { return m_dbNpc.Data0; } set { m_dbNpc.Data0 = value; Save(); } }
        public int Data1 { get { return m_dbNpc.Data1; } set { m_dbNpc.Data1 = value; Save(); } }
        public int Data2 { get { return m_dbNpc.Data2; } set { m_dbNpc.Data2 = value; Save(); } }
        public int Data3 { get { return m_dbNpc.Data3; } set { m_dbNpc.Data3 = value; Save(); } }

        #endregion

        #region Database
        public bool Save()
        {
            return Database.NpcRepository.SaveOrUpdate(m_dbNpc);
        }

        public bool Delete()
        {
            return Database.NpcRepository.Delete(m_dbNpc);
        }
        #endregion
    }
}