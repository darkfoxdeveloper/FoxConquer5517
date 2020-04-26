// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Nobility.cs
// Last Edit: 2016/11/24 10:28
// Created: 2016/11/24 10:27

using System.Linq;
using DB.Entities;
using DB.Repositories;
using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public sealed class Nobility
    {
        private readonly Character m_pOwner;
        private readonly DbDynaRankRec m_dbRank;
        private NobilityLevel m_pLevel;
        private int m_nRanking;

        public Nobility(Character pOwner)
        {
            m_pOwner = pOwner;

            if (!ServerKernel.Nobility.TryGetValue(pOwner.Identity, out m_dbRank))
            {
                m_dbRank = new DbDynaRankRec
                {
                    ObjectId = 0,
                    ObjectName = "",
                    RankType = 3000003,
                    UserIdentity = pOwner.Identity,
                    Username = pOwner.Name,
                    Value = pOwner.NobilityDonation
                };

                m_pLevel = NobilityLevel.SERF;
                m_nRanking = 0;
            }
            else
            {
                m_nRanking = GetRanking;
                m_pLevel = GetNobility;
            }
            SendNobilityIcon();
        }

        public long Donate(long nAmount)
        {
            if (nAmount < 0) return 0;
            if (nAmount > long.MaxValue) nAmount = long.MaxValue;

            if (nAmount + Donation > long.MaxValue)
            {
                m_pOwner.Send(ServerString.STR_PEERAGE_DONATE_ABOVE_AMOUNT);
                return 0;
            }

            m_dbRank.Value = (m_pOwner.NobilityDonation += nAmount);
            Save();
            return m_dbRank.Value;
        }

        public long Donation { get { return m_pOwner.NobilityDonation; } }

        public NobilityLevel Level { get { return m_pLevel; } }

        public int UpdateRanking()
        {
            return m_nRanking = GetRanking;
        }

        public NobilityLevel UpdateLevel()
        {
            return m_pOwner.NobilityRank = m_pLevel = GetNobility;
        }

        public DbDynaRankRec Database
        {
            get { return m_dbRank; }
        }

        public int GetRanking
        {
            get
            {
                int nRanking = 0;
                if (ServerKernel.Nobility.Values.OrderByDescending(x => x.Value)
                        .TakeWhile(obj => nRanking++ < 60)
                        .Any(obj => obj.UserIdentity == m_pOwner.Identity))
                    return nRanking;
                return -1;
            }
        }

        public NobilityLevel GetNobility
        {
            get
            {
                if (m_nRanking >= 0)
                {
                    if (m_nRanking <= 3)
                        return NobilityLevel.KING;
                    if (m_nRanking <= 15)
                        return NobilityLevel.PRINCE;
                    if (m_nRanking <= 50)
                        return NobilityLevel.DUKE;
                }
                if (Donation >= 200000000)
                    return NobilityLevel.EARL;
                if (Donation >= 100000000)
                    return NobilityLevel.BARON;
                if (Donation >= 30000000)
                    return NobilityLevel.KNIGHT;
                return NobilityLevel.SERF;
            }
        }

        public static NobilityLevel GetNobilityLevel(long position)
        {
            if (position < 0) return NobilityLevel.SERF;

            if (position <= 2)
                return NobilityLevel.KING;
            if (position < 15)
                return NobilityLevel.PRINCE;
            if (position < 50)
                return NobilityLevel.DUKE;
            return NobilityLevel.SERF;
        }

        public void SendNobilityIcon()
        {
            m_nRanking = GetRanking;
            m_pLevel = m_pOwner.NobilityRank = GetNobility;
            string nobilityInfoString = string.Format("{0} {1} {2:d} {3}", m_dbRank.UserIdentity, m_dbRank.Value, (byte)Level, m_nRanking - 1);
            var nPacket = new MsgPeerage(35 + nobilityInfoString.Length);
            nPacket.WriteByte(1, 32);
            nPacket.WriteStringWithLength(nobilityInfoString, 33);
            nPacket.Action = NobilityAction.INFO;
            nPacket.DataLow = m_dbRank.UserIdentity;
            m_pOwner.Send(nPacket);
        }

        public bool Save()
        {
            return Donation > 2999999 && new NobilityRepository().SaveOrUpdate(m_dbRank);
        }
    }
}
