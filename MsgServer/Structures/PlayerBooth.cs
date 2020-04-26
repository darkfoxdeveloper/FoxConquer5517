// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Player Booth.cs
// Last Edit: 2016/12/06 21:39
// Created: 2016/12/06 21:37

using System.Collections.Concurrent;
using System.Linq;
using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using MsgServer.Structures.World;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public class PlayerBooth : IScreenObject
    {
        private uint m_dwMapIdentity;
        private ushort m_usMapX;
        private ushort m_usMapY;
        private Map m_pMap;
        private uint m_dwIdentity;
        private short m_sElevation;
        private string m_szWords;

        private GameNpc m_pNpc;
        private DynamicNpc m_pShop;
        private Character m_pOwner;

        public ConcurrentDictionary<uint, BoothItem> Items;
        private bool m_vending = false;

        public PlayerBooth(Character owner)
        {
            m_pOwner = owner;
            m_pMap = owner.Map;
        }

        public bool Create()
        {
            m_pMap = m_pOwner.Map;
            Items = new ConcurrentDictionary<uint, BoothItem>();
            m_pNpc = m_pMap.GameObjects.Values.FirstOrDefault(x => x.MapX == m_pOwner.MapX - 2 && x.MapY == m_pOwner.MapY) as GameNpc;
            if (m_pNpc == null) return false;
            if (m_pNpc.Vending) return false;

            Vending = true;

            m_szWords = "";

            m_pOwner.Direction = FacingDirection.SOUTH_EAST;
            m_pOwner.Action = EntityAction.SIT;

            var tempDb = new DbDynamicNPC
            {
                Id = ((m_pOwner.Identity % 1000000) + ((m_pOwner.Identity / 1000000) * 100000)),
                Name = m_pOwner.Name,
                Cellx = (ushort)(m_pNpc.MapX + 3),
                Celly = m_pNpc.MapY,
                Type = NpcTypes.BOOTH_NPC,
                Lookface = 406,
                Mapid = m_pOwner.MapIdentity,
                Datastr = "PlayerShop"
            };
            m_pShop = new DynamicNpc(tempDb) { Name = m_pOwner.Name };
            m_dwIdentity = m_pShop.Identity;
            m_pMap.AddDynaNpc(m_pShop);
            return true;
        }

        public bool Destroy()
        {
            m_szWords = "";

            if (m_pShop != null && Vending)
            {
                m_vending = false;
                m_pNpc.Vending = false;
                m_pNpc.Name = "";
                m_pMap.RemoveNpc(m_pShop);
                m_pOwner.Action = EntityAction.STAND;
                m_pNpc = null;
                m_pShop = null;
            }
            Items.Clear();
            return true;
        }

        public string Name
        {
            get { return m_pOwner.Name; }
            set { }
        }

        public bool Vending
        {
            get { return m_vending; }
            set
            {
                m_vending = value;
                m_pNpc.Vending = value;
            }
        }

        #region IScreen Object Properties
        public uint MapIdentity
        {
            get { return m_dwMapIdentity; }
            set { m_dwMapIdentity = value; }
        }

        public ushort MapX
        {
            get { return m_usMapX; }
            set
            {
                m_usMapX = value;
                m_pNpc.MapX = value;
            }
        }

        public ushort MapY
        {
            get { return m_usMapY; }
            set
            {
                m_usMapY = value;
                m_pNpc.MapY = value;
            }
        }

        public Map Map
        {
            get { return m_pMap; }
            set { m_pMap = value; }
        }

        public uint Identity
        {
            get { return m_dwIdentity; }
        }

        public short Elevation
        {
            get { return m_sElevation; }
        }

        public string HawkMessage
        {
            get { return m_szWords; }
            set { m_szWords = value; }
        }

        public IScreenObject FindAroundRole(uint idRole)
        {
            return Map.FindAroundRole(this, idRole);
        }

        public void SendSpawnTo(Character pObj)
        {
            m_pNpc.SendSpawnTo(pObj);
        }
        #endregion

        public void SendHawkMessage(Character pTarget)
        {
            pTarget.Send(new MsgTalk(m_szWords, ChatTone.VENDOR_HAWK)
            {
                Identity = m_pOwner.Identity,
                Sender = m_pOwner.Name,
                SenderMesh = m_pOwner.Lookface
            });
        }
    }
}