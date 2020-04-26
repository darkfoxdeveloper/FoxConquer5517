using System;
using System.Drawing;
using System.Linq;
using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Items
{
    public class MapItem : IScreenObject, IOnTimer
    {
        private uint m_dwMap;
        private ushort m_usMapX;
        private ushort m_usMapY;
        private Map m_pMap;
        private uint m_dwIdentity;
        private short m_sElevation;

        private const uint _ITEM_SILVER_MIN = 1;
        private const uint _ITEM_SILVER_MAX = 9;
        private const uint _ITEM_SYCEE_MIN = 10;
        private const uint _ITEM_SYCEE_MAX = 99;
        private const uint _ITEM_GOLD_MIN = 100;
        private const uint _ITEM_GOLD_MAX = 999;
        private const uint _ITEM_GOLDBULLION_MIN = 1000;
        private const uint _ITEM_GOLDBULLION_MAX = 1999;
        private const uint _ITEM_GOLDBAR_MIN = 2000;
        private const uint _ITEM_GOLDBAR_MAX = 4999;
        private const uint _ITEM_GOLDBARS_MIN = 5000;
        private const uint _ITEM_GOLDBARS_MAX = 10000000;

        private const int _PICKUP_TIME = 30;
        private const int _DISAPPEAR_TIME = 60;
        private const int _MAPITEM_ONTIMER_SECS = 5;
        private const int _MAPITEM_MONSTER_ALIVESECS = 60;
        private const int _MAPITEM_USERMAX_ALIVESECS = 90;
        private const int _MAPITEM_USERMIN_ALIVESECS = 60;
        private const int _MAPITEM_ALIVESECS_PERPRICE = 1000 / (_MAPITEM_USERMAX_ALIVESECS - _MAPITEM_USERMIN_ALIVESECS);		// Íæ¼ÒÈÓµÄµØÃæÎïÆ·µÄÉú´æÊ±¼ä(Ã¿¶àÉÙÇ®¼Ó1Ãë)
        private const int _MAPITEM_PRIV_SECS = 30;
        private const int _PICKMAPITEMDIST_LIMIT = 0;

        private Item m_pItem;
        private DbItemtype m_pItemtype;
        private MsgMapItem m_pPacket;
        private TimeOut m_tPriv;
        private TimeOut m_tAlive;
        private byte m_nPlus;
        private byte m_nDmg;
        private short m_nDura;
        private uint m_idOwner;
        private uint m_dwAmount;

        // double check to avoid duplicate
        private object m_pSyncRoot = new object();
        private bool m_bTaken = false;

        public MapItem()
        {
            m_pPacket = new MsgMapItem();
        }

        #region IScreen Object
        public uint Identity
        {
            get { return m_dwIdentity; }
        }

        public string Name
        {
            get { return m_pItemtype.Name; }
            set { }
        }

        public uint MapIdentity
        {
            get { return m_dwMap; }
            set { m_dwMap = value; }
        }

        public ushort MapX
        {
            get { return m_usMapX; }
            set
            {
                m_usMapX = value;
                m_pPacket.MapX = value;
            }
        }

        public ushort MapY
        {
            get { return m_usMapY; }
            set
            {
                m_usMapY = value;
                m_pPacket.MapY = value;
            }
        }

        public Map Map
        {
            get { return m_pMap; }
            set { m_pMap = value; }
        }

        public short Elevation
        {
            get { return m_sElevation; }
        }

        public IScreenObject FindAroundRole(uint idRole)
        {
            Character temp;
            if (Map.Players.TryGetValue(idRole, out temp))
                return temp;
            return Map.GameObjects.Values.FirstOrDefault(x => x.Identity == idRole);
        }

        public void SendSpawnTo(Character pObj)
        {
            pObj.Send(m_pPacket);
        }

        public void SendRemoveFromScreen(Character pObj)
        {
            m_pPacket.DropType = 2;
            pObj.Send(m_pPacket);
        }
        #endregion

        #region Creation

        public bool Create(uint idNewMapItem, Map map, Point pos, uint idType, uint idOwner, byte nPlus, byte nDmg,
            short nDura)
        {
            if (ServerKernel.Itemtype.TryGetValue(idType, out m_pItemtype))
            {
                return Create(idNewMapItem, map, pos, m_pItemtype, idOwner, nPlus, nDmg, nDura);
            }
            return false;
        }

        public bool Create(uint idNewMapItem, Map map, Point pos, DbItemtype idType, uint idOwner, byte nPlus, byte nDmg,
            short nDura)
        {
            if (map == null || idType == null) return false;

            m_tAlive = new TimeOut(_DISAPPEAR_TIME);
            m_tAlive.Startup(_DISAPPEAR_TIME);

            Map = map;
            MapIdentity = map.Identity;
            MapX = (ushort)pos.X;
            MapY = (ushort)pos.Y;

            m_nPlus = nPlus;
            m_nDmg = nDmg;
            m_nDura = nDura;

            m_pItemtype = idType;
            Type = m_pItemtype.Type;
            m_dwIdentity = idNewMapItem;
            m_pPacket.Identity = idNewMapItem;

            if (idOwner != 0)
            {
                m_idOwner = idOwner;
                m_tPriv = new TimeOut(_MAPITEM_PRIV_SECS);
                m_tPriv.Startup(_MAPITEM_PRIV_SECS);
                m_tPriv.Update();
            }

            m_pPacket.DropType = 1;
            m_pPacket.ItemColor = 3;
            Map.AddItem(this);

            return true;
        }

        public bool Create(uint idNewMapItem, Map map, Point pos, Item pInfo, uint idOwner)
        {
            if (map == null || pInfo == null) return false;

            int nAliveSecs = _MAPITEM_USERMAX_ALIVESECS;
            if (pInfo.Itemtype != null)
                nAliveSecs = (int)(pInfo.Itemtype.Price / _MAPITEM_ALIVESECS_PERPRICE + _MAPITEM_USERMIN_ALIVESECS);
            if (nAliveSecs > _MAPITEM_USERMAX_ALIVESECS)
                nAliveSecs = _MAPITEM_USERMAX_ALIVESECS;

            m_tAlive = new TimeOut(nAliveSecs);
            m_tAlive.Update();

            Map = map;
            MapIdentity = map.Identity;
            MapX = (ushort)pos.X;
            MapY = (ushort)pos.Y;

            m_pItem = pInfo;
            m_dwIdentity = idNewMapItem;
            m_pPacket.Identity = idNewMapItem;
            m_pItemtype = pInfo.Itemtype;
            m_pItem.OwnerIdentity = 0;
            m_pItem.PlayerIdentity = idOwner;

            m_pItem.Position = (ItemPosition)254;

            m_pPacket.ItemColor = 3;
            m_pPacket.DropType = 1;
            m_pPacket.Itemtype = Type = pInfo.Type;
            Map.AddItem(this);

            return true;
        }

        public bool CreateMoney(uint idNewMapItem, Map map, Point pos, uint dwMoney, uint idOwner)
        {
            if (map == null || idNewMapItem == 0) return false;

            int nAliveSecs = _MAPITEM_MONSTER_ALIVESECS;
            if (idOwner == 0)
            {
                nAliveSecs = (int)(dwMoney / _MAPITEM_ALIVESECS_PERPRICE + _MAPITEM_USERMIN_ALIVESECS);
                if (nAliveSecs > _MAPITEM_USERMAX_ALIVESECS)
                    nAliveSecs = _MAPITEM_USERMAX_ALIVESECS;
            }

            m_tAlive = new TimeOut(nAliveSecs);
            m_tAlive.Update();

            Map = map;
            MapIdentity = map.Identity;
            MapX = (ushort)pos.X;
            MapY = (ushort)pos.Y;

            uint idType;
            if (dwMoney < _ITEM_SILVER_MAX)
                idType = 1090000;
            else if (dwMoney < _ITEM_SYCEE_MAX)
                idType = 1090010;
            else if (dwMoney < _ITEM_GOLD_MAX)
                idType = 1090020;
            else if (dwMoney < _ITEM_GOLDBULLION_MAX)
                idType = 1091000;
            else if (dwMoney < _ITEM_GOLDBAR_MAX)
                idType = 1091010;
            else
                idType = 1091020;

            m_dwIdentity = idNewMapItem;
            m_pPacket.Identity = idNewMapItem;
            m_dwAmount = dwMoney;

            Type = idType;

            if (idOwner != 0)
            {
                m_idOwner = idOwner;
                m_tPriv = new TimeOut(_MAPITEM_PRIV_SECS);
                m_tPriv.Startup(_MAPITEM_PRIV_SECS);
                m_tPriv.Update();
            }

            m_pPacket.DropType = 1;
            try
            {
                Map.AddItem(this);
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString());
            }

            return true;
        }

        public Item GeneratePickUp(Character pPicker)
        {
            lock (m_pSyncRoot)
            {
                if (m_bTaken)
                    return null;

                m_bTaken = true;

                m_pItem = new Item(pPicker);
                m_pItem.Type = m_pItemtype.Type;
                m_pItem.OwnerIdentity = m_idOwner;
                m_pItem.PlayerIdentity = pPicker.Identity;
                m_pItem.Durability = (ushort) (m_pItemtype.Amount/60);
                m_pItem.MaximumDurability = (ushort) (m_pItemtype.AmountLimit + m_nDura);
                m_pItem.Color = (ItemColor) ThreadSafeRandom.RandGet(2, 9);

                if (m_pItem.Durability > 1)
                {
                    const int nRate = 50;
                    if (Calculations.ChanceCalc(nRate))
                        m_pItem.Durability = (ushort) (m_pItem.Durability*(ThreadSafeRandom.RandGet(15) + 20)/100);
                    else
                        m_pItem.Durability = (ushort) (m_pItem.Durability*(ThreadSafeRandom.RandGet(15) + 35)/100);

                    m_pItem.Durability = Calculations.CutTrail((ushort) 1, m_pItem.Durability);
                }

                m_pItem.Position = ItemPosition.FLOOR;

                m_pItem.SocketOne = (SocketGem) m_pItemtype.Gem1;
                m_pItem.SocketTwo = (SocketGem) m_pItemtype.Gem2;
                m_pItem.Effect = (ItemEffect) m_pItemtype.Magic1;

                if (m_nPlus > 0)
                    m_pItem.Plus = m_nPlus;
                else
                    m_pItem.Plus = m_pItemtype.Magic3;

                if (m_nDmg > 0 && (m_pItem.IsEquipment() || m_pItem.IsWeapon()))
                    m_pItem.ReduceDamage = m_nDmg;
                return m_pItem;
            }
        }

        public uint Type
        {
            get { return m_pPacket.Itemtype; }
            set { m_pPacket.Itemtype = value; }
        }

        public void SelfDelete()
        {
            if (m_pItem != null && m_pItem.Identity != 0)
                m_pItem.Delete();
        }

        public bool IsMoney()
        {
            return Type >= 1090000 && Type <= 1091020;
        }

        public uint GetPlayerId()
        {
            return m_pItem != null ? m_pItem.PlayerIdentity : 0;
        }

        public bool IsPriv()
        {
            if (m_tPriv == null) return false;
            return !m_tPriv.IsTimeOut();
        }

        public bool IsDisappear()
        {
            return m_tAlive.IsTimeOut();
        }

        public uint GetAmount()
        {
            return m_dwAmount;
        }

        public Item GetInfo(Character pPicker)
        {
            if (m_pItem == null)
                GeneratePickUp(pPicker);

            if (m_pItem == null)
                return null;

            m_pItem.Type = Type;
            m_pItem.ChangeOwner(pPicker);
            if (m_pItem.StackAmount <= 0)
                m_pItem.StackAmount = 1;
            return m_pItem;
        }

        #endregion

        public void OnTimer()
        {

        }
    }
}
