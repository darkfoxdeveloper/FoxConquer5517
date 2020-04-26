// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Family Member.cs
// Last Edit: 2016/12/05 06:59
// Created: 2016/12/05 05:55

using DB.Entities;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Common.Enums;

namespace MsgServer.Structures.Society
{
    public sealed class FamilyMember
    {
        private DbFamilyMember m_dbObj;
        private Family m_pFamily;

        public FamilyMember(Family pFamily)
        {
            m_pFamily = pFamily;
        }

        public bool Create(Character pUser)
        {
            m_dbObj = new DbFamilyMember
            {
                FamilyIdentity = m_pFamily.Identity,
                Identity = pUser.Identity,
                JoinDate = (uint) UnixTimestamp.Timestamp(),
                Position = (byte) FamilyRank.MEMBER
            };
            Save();

            Name = pUser.Name;
            Level = pUser.Level;

            return true;
        }

        public bool Create(DbUser dbUser, DbFamilyMember obj)
        {
            m_dbObj = obj;
            Name = dbUser.Name;
            Level = dbUser.Level;
            return true;
        }

        public uint Identity { get { return m_dbObj.Identity; } }

        public string Name { get; set; }

        public byte Level { get; set; }

        public uint Donation
        {
            get { return m_dbObj.Money; }
            set
            {
                m_dbObj.Money = value;
                Save();
            }
        }

        public FamilyRank Position
        {
            get { return (FamilyRank) m_dbObj.Position; }
            set
            {
                m_dbObj.Position = (byte) value;
                Save();
            }
        }

        public bool IsOnline { get { return ServerKernel.Players.ContainsKey(Identity); } }

        public Character Owner
        {
            get
            {
                Client pClient;
                return ServerKernel.Players.TryGetValue(Identity, out pClient) ? pClient.Character : null;
            }
        }

        public uint JoinDate { get { return m_dbObj.JoinDate; } }

        public bool Save()
        {
            return m_dbObj != null && Database.FamilyMemberRepository.SaveOrUpdate(m_dbObj);
        }

        public bool Delete()
        {
            return m_dbObj != null && Database.FamilyMemberRepository.Delete(m_dbObj);
        }
    }
}