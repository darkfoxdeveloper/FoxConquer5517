// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Syndicate Member.cs
// Last Edit: 2016/11/29 17:04
// Created: 2016/11/24 22:01

using System.Drawing;
using System.Linq;
using DB.Entities;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Society
{
    public sealed class SyndicateMember
    {
        private DbCqSynattr m_dbObj;
        private Syndicate m_pSyn;

        public SyndicateMember(Syndicate pSyn)
        {
            m_pSyn = pSyn;
        }

        public bool Create(Character pUser)
        {
            if (m_pSyn == null)
                return false;

            m_dbObj = new DbCqSynattr
            {
                JoinDate = (uint) UnixTimestamp.Timestamp(),
                Id = pUser.Identity,
                SynId = m_pSyn.Identity,
                Rank = (ushort) (SyndicateRank.MEMBER)
            };
            Name = pUser.Name;
            Mate = pUser.Mate;
            Lookface = pUser.Lookface;
            Level = pUser.Level;
            Profession = pUser.Profession;
            return Save();
        }

        public bool Create(DbCqSynattr dbSynAttr, DbUser pUser)
        {
            if (m_pSyn == null)
                return false;

            m_dbObj = dbSynAttr;
            Name = pUser.Name;
            Mate = pUser.Mate;
            Lookface = pUser.Lookface;
            Level = pUser.Level;
            Profession = pUser.Profession;
            return true;
        }

        public uint Identity
        {
            get { return m_dbObj.Id; }
        }

        public ushort SyndicateIdentity
        {
            get { return (ushort) m_dbObj.SynId; }
        }

        public uint Lookface { get; set; }
        public string Name { get; set; }
        public string Mate { get; set; }
        public byte Level { get; set; }
        public ushort Profession { get; set; }
        public uint Nobility
        {
            get
            {
                if (!IsOnline)
                {
                    //DbDynaRankRec dbRank = ServerKernel.Nobility.Values.FirstOrDefault(x => x.UserIdentity == Identity);
                    return 0;
                }
                return (uint) Owner.Character.NobilityRank;
            }
        }

        public SyndicateRank Position
        {
            get { return (SyndicateRank) m_dbObj.Rank; }
            set
            {
                m_dbObj.Rank = (ushort) value;
                if (IsOnline)
                {
                    Owner.Character.SyndicateRank = value;
                }
                Save();
            }
        }

        public uint SilverDonation
        {
            get { return (uint) (m_dbObj.Proffer/10000); }
        }

        public uint TotalSilverDonation
        {
            get { return (uint) m_dbObj.ProfferTotal/10000; }
        }

        public uint EmoneyDonation
        {
            get { return m_dbObj.Emoney*20; }
        }

        public uint TotalEmoneyDonation
        {
            get { return m_dbObj.EmoneyTotal*20; }
        }

        public int PkDonation
        {
            get { return m_dbObj.Pk; }
        }

        public uint TotalPkDonation
        {
            get { return m_dbObj.PkTotal; }
        }

        public uint GuideDonation
        {
            get { return m_dbObj.Guide; }
        }

        public uint GuideTotal
        {
            get { return m_dbObj.GuideTotal; }
        }

        public uint ArsenalDonation
        {
            get { return m_dbObj.Arsenal; }
            set
            {
                m_dbObj.Arsenal = value;
                Save();
            }
        }

        public uint TotalDonation
        {
            get { return (uint) (SilverDonation + EmoneyDonation + PkDonation + GuideDonation + ArsenalDonation); }
        }

        public uint RedRoseDonation { get; set; }
        public uint WhiteRoseDonation { get; set; }
        public uint OrchidDonation { get; set; }
        public uint TulipDonation { get; set; }

        public uint Exploit
        {
            get { return m_dbObj.Exploit; }
        }

        public uint PositionExpire
        {
            get { return m_dbObj.Expiration; }
            set { m_dbObj.Expiration = value; }
        }

        public bool PositionExpired
        {
            get { return UnixTimestamp.Timestamp() > m_dbObj.Expiration; }
        }

        public uint JoinDate
        {
            get { return m_dbObj.JoinDate; }
        }

        public Client Owner
        {
            get { return ServerKernel.Players.Values.FirstOrDefault(x => x.Identity == m_dbObj.Id); }
        }

        public bool IsOnline
        {
            get { return ServerKernel.Players.ContainsKey(m_dbObj.Id); }
        }

        #region Socket

        public void Send(byte[] pBuffer)
        {
            Client pTarget;
            if (ServerKernel.Players.TryGetValue(Identity, out pTarget))
                pTarget.Send(pBuffer);
        }

        public void SendSyndicate()
        {
            Client pTarget;
            if (ServerKernel.Players.TryGetValue(Identity, out pTarget))
            {
                pTarget.Send(new MsgTalk(m_pSyn.Announcement, ChatTone.GUILD_ANNOUNCEMENT, Color.White)
                {
                    Identity = m_pSyn.Identity
                });

                MsgSyndicateAttributeInfo pInfo = new MsgSyndicateAttributeInfo
                {
                    EMoneyFund = m_pSyn.EmoneyDonation,
                    EnrollmentDate = uint.Parse(UnixTimestamp.ToDateTime(JoinDate).ToString("yyyyMMdd")),
                    LeaderName = m_pSyn.LeaderName,
                    MemberAmount = m_pSyn.MemberCount,
                    MoneyFund = m_pSyn.SilverDonation,
                    Position = Position,
                    RequiredLevel = m_pSyn.LevelRequirement,
                    RequiredMetempsychosis = m_pSyn.MetempsychosisRequirement,
                    RequiredProfession = m_pSyn.ProfessionRequirement,
                    SyndicateIdentity = m_pSyn.Identity,
                    SyndicateLevel = m_pSyn.Level,
                };
                uint expire = 0;
                uint time = (uint)UnixTimestamp.Timestamp();
                if (time < PositionExpire)
                    expire = uint.Parse(UnixTimestamp.ToDateTime(PositionExpire).ToString("yyyyMMdd"));
                pInfo.PositionExpire = expire;
                pTarget.Send(pInfo);
            }
        }

        public void SendPromotionList()
        {
            if (m_pSyn == null || !IsOnline)
                return;

            byte synLevel = m_pSyn.Level;

            var msg = new MsgSyndicate();
            msg.Action = SyndicateRequest.SYN_PROMOTE;

            if (Position == SyndicateRank.GUILD_LEADER)
            {
                int maxDl = (synLevel < 4 ? 2 : synLevel < 7 ? 3 : 4);
                int maxHdl = (synLevel < 6 ? 1 : 2);
                int maxHMan = (synLevel < 5 ? 1 : synLevel < 7 ? 2 : synLevel < 9 ? 4 : 6);
                int maxHSuper = (synLevel < 5 ? 1 : synLevel < 7 ? 2 : synLevel < 9 ? 4 : 6);
                int maxHStew = (synLevel < 3 ? 1 : synLevel < 5 ? 2 : synLevel < 7 ? 4 : synLevel < 9 ? 6 : 8);
                int maxAide = (synLevel < 4 ? 2 : synLevel < 6 ? 4 : 6);

                msg.Positions.Add((ushort)SyndicateRank.GUILD_LEADER + " 1 1 " + m_pSyn.Arsenal.SharedBattlePower(SyndicateRank.GUILD_LEADER) + " " + 0);
                msg.Positions.Add((ushort)SyndicateRank.DEPUTY_LEADER + " " + m_pSyn.DeputyLeaderCount + " " + maxDl + " " + m_pSyn.Arsenal.SharedBattlePower(SyndicateRank.DEPUTY_LEADER) + " " + 0);
                msg.Positions.Add((ushort)SyndicateRank.HONORARY_DEPUTY_LEADER + " " + m_pSyn.HonoraryDeputyLeaderCount + " " + maxHdl + " " + m_pSyn.Arsenal.SharedBattlePower(SyndicateRank.HONORARY_DEPUTY_LEADER) + " " + Syndicate.HONORARY_DEPUTY_LEADER_PRICE*-1);
                msg.Positions.Add((ushort)SyndicateRank.HONORARY_MANAGER + " " + m_pSyn.HonoraryManagerCount + " " + maxHMan + " " + m_pSyn.Arsenal.SharedBattlePower(SyndicateRank.HONORARY_MANAGER) + " " + Syndicate.HONORARY_MANAGER_PRICE*-1);
                msg.Positions.Add((ushort)SyndicateRank.HONORARY_SUPERVISOR + " " + m_pSyn.HonorarySupervisorCount + " " + maxHSuper + " " + m_pSyn.Arsenal.SharedBattlePower(SyndicateRank.HONORARY_SUPERVISOR) + " " + Syndicate.HONORARY_SUPERVISOR_PRICE*-1);
                msg.Positions.Add((ushort)SyndicateRank.HONORARY_STEWARD + " " + m_pSyn.HonoraryStewardCount + " " + maxHStew + " " + m_pSyn.Arsenal.SharedBattlePower(SyndicateRank.HONORARY_STEWARD) + " " + Syndicate.HONORARY_STEWARD_PRICE*-1);
                msg.Positions.Add((ushort)SyndicateRank.AIDE + " " + m_pSyn.AideCount + " " + maxAide + " " + m_pSyn.Arsenal.SharedBattlePower(SyndicateRank.AIDE) + " " + 0);
            }
            else if (Position == SyndicateRank.LEADER_SPOUSE)
            {
                msg.Positions.Add((ushort)SyndicateRank.LEADER_SPOUSE_AIDE + " " + m_pSyn.LeaderSpouseAideCount + " 1 " + m_pSyn.Arsenal.SharedBattlePower(SyndicateRank.LEADER_SPOUSE_AIDE) + " " + 0);
            }
            else if (Position == SyndicateRank.DEPUTY_LEADER || Position == SyndicateRank.HONORARY_DEPUTY_LEADER)
            {
                msg.Positions.Add((ushort)SyndicateRank.DEPUTY_LEADER_AIDE + " " + m_pSyn.DeputyLeaderAideCount + " 1 " + m_pSyn.Arsenal.SharedBattlePower(SyndicateRank.DEPUTY_LEADER_AIDE) + " " + 0);
            }
            else if (Position == SyndicateRank.MANAGER || Position == SyndicateRank.HONORARY_MANAGER)
            {
                msg.Positions.Add((ushort)SyndicateRank.MANAGER_AIDE + " " + m_pSyn.ManagerAideCount + " 1 " + m_pSyn.Arsenal.SharedBattlePower(SyndicateRank.MANAGER_AIDE) + " " + 0);
            }
            else if (Position == SyndicateRank.SUPERVISOR
                     || Position == SyndicateRank.ARSENAL_SUPERVISOR
                     || Position == SyndicateRank.CP_SUPERVISOR
                     || Position == SyndicateRank.GUIDE_SUPERVISOR
                     || Position == SyndicateRank.HONORARY_SUPERVISOR
                     || Position == SyndicateRank.LILY_SUPERVISOR
                     || Position == SyndicateRank.ORCHID_SUPERVISOR
                     || Position == SyndicateRank.PK_SUPERVISOR
                     || Position == SyndicateRank.ROSE_SUPERVISOR
                     || Position == SyndicateRank.SILVER_SUPERVISOR
                     || Position == SyndicateRank.TULIP_SUPERVISOR)
            {
                msg.Positions.Add((ushort) SyndicateRank.SUPERVISOR_AIDE + " " + m_pSyn.SupervisorAideCount + " 1 " +
                                  m_pSyn.Arsenal.SharedBattlePower(SyndicateRank.SUPERVISOR_AIDE) + " " + 0);
            }
            else
            {
                return;
            }
            msg.SetList();
            Send(msg);
        }

        public void SendCharacterInformation()
        {
            Client pTarget;
            if (ServerKernel.Players.TryGetValue(Identity, out pTarget))
            {
                var sPacket = new MsgSynpOffer
                {
                    Flag = Identity,
                    MoneyDonation = (int) SilverDonation,
                    EmoneyDonation = EmoneyDonation,
                    GuideDonation = GuideDonation,
                    PkDonation = PkDonation,
                    Exploits = Exploit,
                    TotemDonation = ArsenalDonation,
                    RedRoseDonation = pTarget.Character.RedRoses,
                    WhiteRoseDonation = pTarget.Character.WhiteRoses,
                    OrchidDonation = pTarget.Character.Orchids,
                    TulipDonation = pTarget.Character.Tulips
                };
                pTarget.Send(sPacket);
            }
        }

        #endregion

        #region Promotion

        public bool IsUserSetPosition()
        {
            return IsUserSetPosition(Position);
        }

        public static bool IsUserSetPosition(SyndicateRank pos)
        {
            switch (pos)
            {
                case SyndicateRank.GUILD_LEADER:
                case SyndicateRank.DEPUTY_LEADER:
                case SyndicateRank.DEPUTY_LEADER_AIDE:
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                case SyndicateRank.HONORARY_MANAGER:
                case SyndicateRank.HONORARY_STEWARD:
                case SyndicateRank.SUPERVISOR_AIDE:
                case SyndicateRank.HONORARY_SUPERVISOR:
                case SyndicateRank.MANAGER_AIDE:
                case SyndicateRank.AIDE:
                case SyndicateRank.LEADER_SPOUSE_AIDE:
                    return true;
                default:
                    return false;
            }
        }

        public string GetRankName()
        {
            switch (Position)
            {
                case SyndicateRank.GUILD_LEADER:
                    return "Guild Leader";
                case SyndicateRank.LEADER_SPOUSE:
                    return "Leader Spouse";
                case SyndicateRank.LEADER_SPOUSE_AIDE:
                    return "Leader Spouse Aide";
                case SyndicateRank.DEPUTY_LEADER:
                    return "Deputy Leader";
                case SyndicateRank.DEPUTY_LEADER_AIDE:
                    return "Deputy Leader Aide";
                case SyndicateRank.DEPUTY_LEADER_SPOUSE:
                    return "Deputy Leader Spouse";
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                    return "Honorary Deputy Leader";
                case SyndicateRank.MANAGER:
                    return "Manager";
                case SyndicateRank.MANAGER_AIDE:
                    return "Manager Aide";
                case SyndicateRank.MANAGER_SPOUSE:
                    return "Manager Spouse";
                case SyndicateRank.HONORARY_MANAGER:
                    return "Honorary Manager";
                case SyndicateRank.SUPERVISOR:
                    return "Supervisor";
                case SyndicateRank.SUPERVISOR_AIDE:
                    return "Supervisor Aide";
                case SyndicateRank.SUPERVISOR_SPOUSE:
                    return "Supervisor Spouse";
                case SyndicateRank.TULIP_SUPERVISOR:
                    return "Tulip Supervisor";
                case SyndicateRank.ARSENAL_SUPERVISOR:
                    return "Arsenal Supervisor";
                case SyndicateRank.CP_SUPERVISOR:
                    return "CP Supervisor";
                case SyndicateRank.GUIDE_SUPERVISOR:
                    return "Guide Supervisor";
                case SyndicateRank.LILY_SUPERVISOR:
                    return "Lily Supervisor";
                case SyndicateRank.ORCHID_SUPERVISOR:
                    return "Orchid Supervisor";
                case SyndicateRank.SILVER_SUPERVISOR:
                    return "Silver Supervisor";
                case SyndicateRank.ROSE_SUPERVISOR:
                    return "Rose Supervisor";
                case SyndicateRank.PK_SUPERVISOR:
                    return "PK Supervisor";
                case SyndicateRank.HONORARY_SUPERVISOR:
                    return "Honorary Supervisor";
                case SyndicateRank.STEWARD:
                    return "Steward";
                case SyndicateRank.STEWARD_SPOUSE:
                    return "Steward Spouse";
                case SyndicateRank.DEPUTY_STEWARD:
                    return "Deputy Steward";
                case SyndicateRank.HONORARY_STEWARD:
                    return "Honorary Steward";
                case SyndicateRank.AIDE:
                    return "Aide";
                case SyndicateRank.TULIP_AGENT:
                    return "Tulip Agent";
                case SyndicateRank.ORCHID_AGENT:
                    return "Orchid Agent";
                case SyndicateRank.CP_AGENT:
                    return "CP Agent";
                case SyndicateRank.ARSENAL_AGENT:
                    return "Arsenal Agent";
                case SyndicateRank.SILVER_AGENT:
                    return "Silver Agent";
                case SyndicateRank.GUIDE_AGENT:
                    return "Guide Agent";
                case SyndicateRank.PK_AGENT:
                    return "PK Agent";
                case SyndicateRank.ROSE_AGENT:
                    return "Rose Agent";
                case SyndicateRank.LILY_AGENT:
                    return "Lily Agent";
                case SyndicateRank.AGENT:
                    return "Agent Follower";
                case SyndicateRank.TULIP_FOLLOWER:
                    return "Tulip Follower";
                case SyndicateRank.ORCHID_FOLLOWER:
                    return "Orchid Follower";
                case SyndicateRank.CP_FOLLOWER:
                    return "CP Follower";
                case SyndicateRank.ARSENAL_FOLLOWER:
                    return "Arsenal Follower";
                case SyndicateRank.SILVER_FOLLOWER:
                    return "Silver Follower";
                case SyndicateRank.GUIDE_FOLLOWER:
                    return "Guide Follower";
                case SyndicateRank.PK_FOLLOWER:
                    return "PK Follower";
                case SyndicateRank.ROSE_FOLLOWER:
                    return "Rose Follower";
                case SyndicateRank.LILY_FOLLOWER:
                    return "Lily Follower";
                case SyndicateRank.FOLLOWER:
                    return "Follower";
                case SyndicateRank.SENIOR_MEMBER:
                    return "Member";
                case SyndicateRank.MEMBER:
                    return "Member";
                default:
                    return "ERROR";
            }
        }

        #endregion

        #region Donation

        public bool IncreaseMoney(uint dwAmount)
        {
            if (m_dbObj.Proffer >= int.MaxValue)
                return true;

            if (m_dbObj.Proffer + dwAmount >= int.MaxValue)
                m_dbObj.Proffer = int.MaxValue;
            else if (m_dbObj.Proffer + dwAmount < int.MaxValue)
                m_dbObj.Proffer += dwAmount;

            if (m_dbObj.ProfferTotal < int.MaxValue)
            {
                if (m_dbObj.ProfferTotal + dwAmount >= int.MaxValue)
                    m_dbObj.ProfferTotal = int.MaxValue;
                else if (m_dbObj.ProfferTotal + dwAmount < int.MaxValue)
                    m_dbObj.ProfferTotal += dwAmount;
            }
            return Save();
        }

        /// <summary>
        /// This method will decrease the user donation. It doesn't check if the user has enough Money Donation.
        /// </summary>
        public bool SpendMoney(int dwAmount)
        {
            if (dwAmount > 0)
            {
                if (m_dbObj.Proffer + dwAmount >= int.MaxValue)
                    m_dbObj.Proffer = int.MaxValue;
                else
                    m_dbObj.Proffer += dwAmount;
            }
            else
            {
                if (m_dbObj.Proffer + dwAmount <= int.MinValue)
                    m_dbObj.Proffer = int.MinValue;
                else
                    m_dbObj.Proffer += dwAmount;
            }
            return Save();
        }

        public bool IncreaseEmoney(uint dwAmount)
        {
            if (m_dbObj.Emoney >= uint.MaxValue)
                return true;

            if (m_dbObj.Emoney + dwAmount >= uint.MaxValue)
                m_dbObj.Emoney = uint.MaxValue;
            else if (m_dbObj.Emoney + dwAmount < uint.MaxValue)
                m_dbObj.Emoney += dwAmount;

            if (m_dbObj.EmoneyTotal < uint.MaxValue)
            {
                if (m_dbObj.EmoneyTotal + dwAmount >= uint.MaxValue)
                    m_dbObj.EmoneyTotal = uint.MaxValue;
                else if (m_dbObj.EmoneyTotal + dwAmount < uint.MaxValue)
                    m_dbObj.EmoneyTotal += dwAmount;
            }
            return Save();
        }

        /// <summary>
        /// This method does check if the user has enough cps to expend.
        /// </summary>
        public bool DecreaseEmoney(uint dwAmount)
        {
            if (dwAmount > m_dbObj.Emoney)
                return false;

            if (m_dbObj.Emoney + (dwAmount * -1) < 0)
                m_dbObj.Emoney = 0;
            else
                m_dbObj.Emoney -= dwAmount;

            return Save();
        }

        public bool IncreasePkDonation(uint dwAmount)
        {
            if (m_dbObj.Pk >= int.MaxValue)
                return true;

            if (m_dbObj.Pk + dwAmount >= int.MaxValue)
                m_dbObj.Pk = int.MaxValue;
            else if (m_dbObj.Pk + dwAmount < int.MaxValue)
                m_dbObj.Pk += (int)dwAmount;

            if (m_dbObj.PkTotal < uint.MaxValue)
            {
                if (m_dbObj.PkTotal + dwAmount >= uint.MaxValue)
                    m_dbObj.PkTotal = uint.MaxValue;
                else if (m_dbObj.PkTotal + dwAmount < uint.MaxValue)
                    m_dbObj.PkTotal += dwAmount;
            }
            return Save();
        }

        public bool DecreasePkDonation(int dwAmount)
        {
            if (dwAmount > 0)
            {
                if (m_dbObj.Pk + dwAmount >= int.MaxValue)
                    m_dbObj.Pk = int.MaxValue;
                else
                    m_dbObj.Pk += dwAmount;
            }
            else
            {
                if (m_dbObj.Pk + dwAmount <= int.MinValue)
                    m_dbObj.Pk = int.MinValue;
                else
                    m_dbObj.Pk += dwAmount;
            }
            return Save();
        }

        public bool IncreaseGuideDonation(uint dwAmount)
        {
            if (m_dbObj.Guide >= int.MaxValue)
                return true;

            if (m_dbObj.Guide + dwAmount >= int.MaxValue)
                m_dbObj.Guide = int.MaxValue;
            else if (m_dbObj.Guide + dwAmount < int.MaxValue)
                m_dbObj.Guide += dwAmount;

            if (m_dbObj.GuideTotal < uint.MaxValue)
            {
                if (m_dbObj.GuideTotal + dwAmount >= uint.MaxValue)
                    m_dbObj.GuideTotal = uint.MaxValue;
                else if (m_dbObj.GuideTotal + dwAmount < uint.MaxValue)
                    m_dbObj.GuideTotal += dwAmount;
            }
            return Save();
        }

        public bool DecreaseGuideDonation(int dwAmount)
        {
            if (dwAmount > 0)
            {
                if (m_dbObj.Guide + dwAmount >= int.MaxValue)
                    m_dbObj.Guide = int.MaxValue;
                else
                    m_dbObj.Guide += (uint) dwAmount;
            }
            else
            {
                if (m_dbObj.Guide + dwAmount <= int.MinValue)
                    m_dbObj.Guide = uint.MinValue;
                else
                    m_dbObj.Guide -= (uint) (dwAmount * -1);
            }
            return Save();
        }

        #endregion

        #region Database
        public bool Save()
        {
            return m_dbObj != null && Database.SyndicateMembersRepository.SaveOrUpdate(m_dbObj);
        }

        public bool Delete()
        {
            return m_dbObj != null && Database.SyndicateMembersRepository.Delete(m_dbObj);
        }
        #endregion
    }
}