// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Trade Partner.cs
// Last Edit: 2016/12/06 15:23
// Created: 2016/12/06 15:23

using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public class TradePartner
    {
        private Character m_pOwner;
        private DbBusiness m_dbObj;
        private MsgTradeBuddy m_pPacket;
        private uint m_dwIdentity;
        private string m_szName;
        private uint m_dwAddDate;

        public TradePartner(Character pOwner)
        {
            m_pOwner = pOwner;
            m_pPacket = new MsgTradeBuddy();
        }

        public TradePartner(Character pOwner, DbBusiness dbBusiness)
        {
            m_pOwner = pOwner;
            m_dbObj = dbBusiness;
            m_dwIdentity = dbBusiness.Business;
            m_szName = dbBusiness.Name;
            m_dwAddDate = dbBusiness.Date;
            m_pPacket = new MsgTradeBuddy
            {
                Identity = m_dwIdentity,
                Name = m_szName,
                HoursLeft = HoursLeft,
                Online = TargetOnline
            };
        }

        public bool Create(uint idTarget)
        {
            Client target;
            if (!ServerKernel.Players.TryGetValue(idTarget, out target))
            {
                //m_pOwner.Send("The target is not online.");
                return false;
            }

            if (target.Character == null)
            {
                return false;
            }

            m_dwIdentity = target.Character.Identity;
            m_szName = target.Character.Name;
            m_dwAddDate = (uint)UnixTimestamp.Timestamp();

            new BusinessRepository().SaveOrUpdate(new DbBusiness
            {
                Userid = m_pOwner.Identity,
                Business = m_dwIdentity,
                Date = m_dwAddDate,
                Name = m_szName // target name
            });

            m_pOwner.TradePartners.TryAdd(idTarget, this);

            m_pPacket = new MsgTradeBuddy
            {
                Identity = m_dwIdentity,
                Name = m_szName,
                HoursLeft = (int)(((UnixTimestamp.Timestamp() + UnixTimestamp.TIME_SECONDS_DAY * 3) - m_dwAddDate) / 60 / 60),
                Type = TradePartnerType.ADD_PARTNER,
                Online = TargetOnline
            };
            m_pOwner.Send(m_pPacket);
            return true;
        }

        public bool Delete()
        {
            return new BusinessRepository().Delete(m_dbObj);
        }

        public uint OwnerIdentity
        {
            get { return m_pOwner.Identity; }
        }

        public uint TargetIdentity
        {
            get { return m_dwIdentity; }
        }

        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        public int HoursLeft
        {
            get { return !IsActive ? (int)(((m_dwAddDate + UnixTimestamp.TIME_SECONDS_DAY * 3) - UnixTimestamp.Timestamp()) / 60 / 60) : 0; }
        }

        public Character Owner
        {
            get { return m_pOwner; }
        }

        public bool TargetOnline
        {
            get { return ServerKernel.Players.ContainsKey(m_dwIdentity); }
        }

        public bool IsActive
        {
            get { return UnixTimestamp.Timestamp() > (m_dwAddDate + (UnixTimestamp.TIME_SECONDS_DAY * 3)); }
        }

        public MsgTradeBuddy ToArray(TradePartnerType type)
        {
            m_pPacket.Type = type;
            return m_pPacket;
        }
    }
}
