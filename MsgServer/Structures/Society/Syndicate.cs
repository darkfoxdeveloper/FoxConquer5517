// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Syndicate.cs
// Last Edit: 2016/12/06 22:02
// Created: 2016/11/24 22:01

using System;
using System.Collections.Concurrent;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Society
{
    public sealed class Syndicate
    {
        #region Private Getters
        private int MaxDeputyLeader { get { return Level < 4 ? 2 : Level < 7 ? 3 : 4; } }
        private int MaxHonoraryDeputyLeader { get { return Level < 6 ? 1 : 2; } }
        private int MaxHonoraryManager { get { return Level < 5 ? 1 : Level < 7 ? 2 : Level < 9 ? 4 : 6; } }
        private int MaxHonorarySupervisor { get { return Level < 5 ? 1 : Level < 7 ? 2 : Level < 9 ? 4 : 6; } }
        private int MaxHonorarySteward { get { return Level < 3 ? 1 : Level < 5 ? 2 : Level < 7 ? 4 : Level < 9 ? 6 : 8; } }
        private int MaxAide { get { return Level < 4 ? 2 : Level < 6 ? 4 : 6; } }
        private int MaxManager
        {
            get {
                switch (Level)
                {
                    case 1:
                    case 2: return 1;
                    case 3:
                    case 4: return 2;
                    case 5:
                    case 6: return 4;
                    case 7:
                    case 8: return 6;
                    case 9: return 8;
                    default: return 0;
                }
            }
        }
        private int MaxSupervisor { get { return Level < 4 ? 0 : Level < 8 ? 1 : 2; } }
        private int MaxSteward
        {
            get
            {
                switch (Level)
                {
                    case 0:
                    case 1: return 0;
                    case 2: return 1;
                    case 3: return 2;
                    case 4: return 3;
                    case 5: return 4;
                    case 6: return 5;
                    case 7: return 6;
                    case 8:
                    case 9: return 8;
                    default: return 0;
                }
            }
        }

        #endregion

        public const int HONORARY_DEPUTY_LEADER_PRICE = -65000,
            HONORARY_MANAGER_PRICE = -32000,
            HONORARY_SUPERVISOR_PRICE = -25000,
            HONORARY_STEWARD_PRICE = -10000;

        private DbSyndicate m_dbSyn;
        private ushort m_usMembers;

        public ConcurrentDictionary<uint, SyndicateMember> Members;
        public ConcurrentDictionary<uint, Syndicate> Allies;
        public ConcurrentDictionary<uint, Syndicate> Enemies;
        public ConcurrentDictionary<uint, Character> WaitQueue = new ConcurrentDictionary<uint, Character>();

        public Arsenal Arsenal;

        #region Create

        public bool Create(string name, Character owner)
        {
            Members = new ConcurrentDictionary<uint, SyndicateMember>(10, 800);
            m_dbSyn = new DbSyndicate
            {
                Amount = 1,
                Announce = "This is a new guild.",
                DelFlag = 0,
                LeaderIdentity = owner.Identity,
                LeaderName = owner.Name,
                Money = 500000,
                Name = name,
                ReqLevel = 1,
                AnnounceDate = uint.Parse(DateTime.Now.ToString("yyyyMMdd"))
            };
            Save();
            Level = 1;

            Arsenal = new Arsenal(this);
            Allies = new ConcurrentDictionary<uint, Syndicate>();
            Enemies = new ConcurrentDictionary<uint, Syndicate>();
            return true;
        }

        public bool Create(DbSyndicate syndicate)
        {
            m_dbSyn = syndicate;
            LeaderIdentity = syndicate.LeaderIdentity;
            Members = new ConcurrentDictionary<uint, SyndicateMember>(10, 800);

            Arsenal = new Arsenal(this);
            Allies = new ConcurrentDictionary<uint, Syndicate>();
            Enemies = new ConcurrentDictionary<uint, Syndicate>();

            CheckLeaderSpouse();
            return true;
        }

        #endregion
        
        public uint Identity
        {
            get { return m_dbSyn.Identity; }
        }

        public string Name
        {
            get { return m_dbSyn.Name; }
            set
            {
                m_dbSyn.Name = value;
                Save();
            }
        }

        #region Leader
        public uint LeaderIdentity
        {
            get { return m_dbSyn.LeaderIdentity; }
            set
            {
                m_dbSyn.LeaderIdentity = value;
                Save();
            }
        }

        public string LeaderName
        {
            get { return m_dbSyn.LeaderName; }
            set
            {
                m_dbSyn.LeaderName = value;
                Save();
            }
        }

        public bool LeaderIsOnline
        {
            get { return ServerKernel.Players.ContainsKey(m_dbSyn.LeaderIdentity); }
        }

        public Character LeaderRole
        {
            get
            {
                Client pRet;
                return ServerKernel.Players.TryGetValue(LeaderIdentity, out pRet) ? pRet.Character : null;
            }
        }
        #endregion
        
        public byte LevelRequirement
        {
            get { return m_dbSyn.ReqLevel; }
            set
            {
                m_dbSyn.ReqLevel = value;
                Save();
            }
        }

        public byte MetempsychosisRequirement
        {
            get { return m_dbSyn.ReqMetempsychosis; }
            set
            {
                m_dbSyn.ReqMetempsychosis = value;
                Save();
            }
        }

        public byte ProfessionRequirement
        {
            get { return m_dbSyn.ReqClass; }
            set
            {
                m_dbSyn.ReqClass = value;
                Save();
            }
        }

        public bool TrojanEnabled
        {
            get { return (m_dbSyn.ReqClass & SyndicateProfessionRequirement.CLASS_TROJAN) == 0; }
        }

        public bool WarriorEnabled
        {
            get { return (m_dbSyn.ReqClass & SyndicateProfessionRequirement.CLASS_WARRIOR) == 0; }
        }

        public bool ArcherEnabled
        {
            get { return (m_dbSyn.ReqClass & SyndicateProfessionRequirement.CLASS_ARCHER) == 0; }
        }

        public bool NinjaEnabled
        {
            get { return (m_dbSyn.ReqClass & SyndicateProfessionRequirement.CLASS_NINJA) == 0; }
        }

        public bool MonkEnabled
        {
            get { return (m_dbSyn.ReqClass & SyndicateProfessionRequirement.CLASS_MONK) == 0; }
        }

        public bool TaoistEnabled
        {
            get { return (m_dbSyn.ReqClass & SyndicateProfessionRequirement.CLASS_TAOIST) == 0; }
        }

        public byte Level { get; set; }

        public ushort MemberCount
        {
            get { return m_usMembers; }
            set
            {
                m_usMembers = value;
                m_dbSyn.Amount = value;
                Save();
            }
        }

        public ulong SilverDonation
        {
            get { return m_dbSyn.Money; }
            set
            {
                m_dbSyn.Money = value;
                Save();
            }
        }

        public uint EmoneyDonation
        {
            get { return m_dbSyn.EMoney; }
            set
            {
                m_dbSyn.EMoney = value;
                Save();
            }
        }

        public string Announcement
        {
            get { return m_dbSyn.Announce; }
            set
            {
                m_dbSyn.Announce = value;
                m_dbSyn.AnnounceDate = uint.Parse(DateTime.Now.ToString("yyyyMMdd"));
                Save();
            }
        }

        public uint AnnouncementDate
        {
            get { return m_dbSyn.AnnounceDate; }
        }

        public bool Deleted
        {
            get { return m_dbSyn.DelFlag != 0; }
        }

        public long MoneyPrize
        {
            get { return m_dbSyn.MoneyPrize; }
            set
            {
                m_dbSyn.MoneyPrize = value;
                Save();
            }
        }

        public uint EmoneyPrize
        {
            get { return m_dbSyn.EmoneyPrize; }
            set
            {
                m_dbSyn.EmoneyPrize = value;
                Save();
            }
        }

        public void SetRequirements(byte pJob, byte pReb, byte pLev)
        {
            LevelRequirement = pLev;
            MetempsychosisRequirement = pReb;
            ProfessionRequirement = pJob;
        }

        #region Members Management

        public bool AppendMember(Character pTarget)
        {
            if (pTarget.Syndicate != null)
                return false;

            switch ((int)Math.Floor((pTarget.Profession / 10f)))
            {
                case 1:
                    if (!TrojanEnabled)
                        return false;
                    break;
                case 2:
                    if (!WarriorEnabled)
                        return false;
                    break;
                case 4:
                    if (!ArcherEnabled)
                        return false;
                    break;
                case 5:
                    if (!NinjaEnabled)
                        return false;
                    break;
                case 6:
                    if (!MonkEnabled)
                        return false;
                    break;
                case 10:
                case 13:
                case 14:
                    if (!TaoistEnabled)
                        return false;
                    break;
            }

            if (pTarget.Level < LevelRequirement)
                return false;

            if (pTarget.Metempsychosis < MetempsychosisRequirement)
                return false;

            var newMember = new SyndicateMember(this);
            if (!newMember.Create(pTarget))
            {
                newMember.Delete();
                return false;
            }
            
            if (!Members.TryAdd(newMember.Identity, newMember))
            {
                newMember.Delete();
                return false;
            }

            MemberCount = (ushort)Members.Count;

            pTarget.SyndicateIdentity = Identity;
            pTarget.SyndicateRank = newMember.Position;

            pTarget.Syndicate = this;
            pTarget.SyndicateMember = newMember;

            newMember.SendSyndicate();
            pTarget.Screen.RefreshSpawnForObservers();
            SendName(pTarget, true);

            Send(string.Format(ServerString.STR_SYNRECRUIT_JOINED, pTarget.Name));
            return true;
        }

        public bool AppendMember(Character pSender, uint dwTarget, bool bInvite)
        {
            Client pClient;
            if (!ServerKernel.Players.TryGetValue(dwTarget, out pClient))
            {
                pSender.Send("The target is not online.");
                return false;
            }

            var pTarget = pClient.Character;
            if (pTarget.SyndicateMember != null)
            {
                pSender.Send("The target already does have a guild.");
                return false;
            }

            var pInvite = pSender.SyndicateMember;
            switch (pInvite.Position)
            {
                case SyndicateRank.GUILD_LEADER:
                case SyndicateRank.DEPUTY_LEADER:
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                case SyndicateRank.LEADER_SPOUSE:
                case SyndicateRank.MANAGER:
                case SyndicateRank.HONORARY_MANAGER:
                    break;
                default:
                    return false;
            }

            switch ((int) Math.Floor((pTarget.Profession / 10f)))
            {
                case 1:
                    if (!TrojanEnabled)
                        return false;
                    break;
                case 2:
                    if (!WarriorEnabled)
                        return false;
                    break;
                case 4:
                    if (!ArcherEnabled)
                        return false;
                    break;
                case 5:
                    if (!NinjaEnabled)
                        return false;
                    break;
                case 6:
                    if (!MonkEnabled)
                        return false;
                    break;
                case 10:
                case 13:
                case 14:
                    if (!TaoistEnabled)
                        return false;
                    break;
            }

            if (pTarget.Level < LevelRequirement)
                return false;

            if (pTarget.Metempsychosis < MetempsychosisRequirement)
                return false;

            var newMember = new SyndicateMember(this);
            if (!newMember.Create(pTarget))
            {
                pSender.Send("An error ocurred. Please, try again. Syn::0001");
                newMember.Delete();
                return false;
            }

            Send(!bInvite
                ? string.Format(ServerString.STR_JOIN_A_GUILD, pInvite.GetRankName(), pInvite.Name, pTarget.Name)
                : string.Format(ServerString.STR_INVITE_GUILD, pInvite.GetRankName(), pInvite.Name, pTarget.Name));

            if (!Members.TryAdd(newMember.Identity, newMember))
            {
                pSender.Send("An error ocurred. Please, try again. Syn::0002");
                newMember.Delete();
                return false;
            }

            MemberCount = (ushort) Members.Count;

            pTarget.SyndicateIdentity = Identity;
            pTarget.SyndicateRank = newMember.Position;

            pTarget.Syndicate = this;
            pTarget.SyndicateMember = newMember;
            
            newMember.SendSyndicate();
            pTarget.Screen.RefreshSpawnForObservers();
            SendName(pTarget, true);
            return true;
        }

        public void QuitSyndicate(Character pSender)
        {
            SyndicateMember pUser;
            if (!Members.TryGetValue(pSender.Identity, out pUser))
                return;

            if (pUser.Position == SyndicateRank.GUILD_LEADER)
                return;

            if (pUser.SilverDonation < 2)
                return;

            SyndicateRank oldRank = pUser.Position;

            switch (pUser.Position)
            {
                case SyndicateRank.DEPUTY_LEADER:
                    DeputyLeaderCount -= 1;
                    break;
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                    HonoraryDeputyLeaderCount -= 1;
                    break;
                case SyndicateRank.HONORARY_MANAGER:
                    HonoraryManagerCount -= 1;
                    break;
                case SyndicateRank.HONORARY_SUPERVISOR:
                    HonorarySupervisorCount -= 1;
                    break;
                case SyndicateRank.HONORARY_STEWARD:
                    HonoraryStewardCount -= 1;
                    break;
                case SyndicateRank.AIDE:
                    AideCount -= 1;
                    break;
                case SyndicateRank.LEADER_SPOUSE_AIDE:
                    LeaderSpouseAideCount -= 1;
                    break;
                case SyndicateRank.DEPUTY_LEADER_AIDE:
                    DeputyLeaderAideCount -= 1;
                    break;
                case SyndicateRank.MANAGER_AIDE:
                    ManagerAideCount -= 1;
                    break;
                case SyndicateRank.SUPERVISOR_AIDE:
                    SupervisorAideCount -= 1;
                    break;
            }

            Arsenal.RemoveAllFromUser(pUser.Identity);

            ServerKernel.CaptureTheFlag.RemoveMember(pUser.Identity);

            Members.TryRemove(pUser.Identity, out pUser);
            pUser.Owner.Character.SyndicateIdentity = 0;
            pUser.Owner.Character.SyndicateRank = SyndicateRank.NONE;
            pUser.Delete();
            pUser.Send(new MsgSyndicate { Action = SyndicateRequest.SYN_DISBAND, Param = Identity });
            pUser.Owner.Screen.RefreshSpawnForObservers();
            MemberCount = (ushort) Members.Values.Count;
            Save();
            ServerKernel.Log.GmLog("Syndicate",
                string.Format("{0} has left the syndicate {1} [{2}]", pSender.Name, Name, Identity));
            pUser.Owner.Character.Syndicate = null;
            pUser.Owner.Character.SyndicateMember = null;
        }

        public void ExpelMember(Character pSender, string szName, bool bKick)
        {
            var pTarget = Members.Values.FirstOrDefault(x => x.Name == szName);
            if (pTarget == null)
            {
                pSender.Send(ServerString.STR_TARGET_NOT_IN_RANGE);
                return;
            }

            if (pSender.SyndicateRank < SyndicateRank.LEADER_SPOUSE)
                return;

            SyndicateRank oldRank = pTarget.Position;

            if (pSender.SyndicateRank == SyndicateRank.DEPUTY_LEADER
                || pSender.SyndicateRank == SyndicateRank.LEADER_SPOUSE
                || pSender.SyndicateRank == SyndicateRank.HONORARY_DEPUTY_LEADER)
                if (pTarget.Position >= SyndicateRank.SUPERVISOR)
                    return; // if DL or Spouse cannot kick position higher than superv

            switch (pTarget.Position)
            {
                case SyndicateRank.GUILD_LEADER:
                    return; // the target is leader?
                case SyndicateRank.DEPUTY_LEADER:
                    DeputyLeaderCount -= 1;
                    break;
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                    HonoraryDeputyLeaderCount -= 1;
                    break;
                case SyndicateRank.HONORARY_MANAGER:
                    HonoraryManagerCount -= 1;
                    break;
                case SyndicateRank.HONORARY_SUPERVISOR:
                    HonorarySupervisorCount -= 1;
                    break;
                case SyndicateRank.HONORARY_STEWARD:
                    HonoraryStewardCount -= 1;
                    break;
                case SyndicateRank.AIDE:
                    AideCount -= 1;
                    break;
                case SyndicateRank.LEADER_SPOUSE_AIDE:
                    LeaderSpouseAideCount -= 1;
                    break;
                case SyndicateRank.DEPUTY_LEADER_AIDE:
                    DeputyLeaderAideCount -= 1;
                    break;
                case SyndicateRank.MANAGER_AIDE:
                    ManagerAideCount -= 1;
                    break;
                case SyndicateRank.SUPERVISOR_AIDE:
                    SupervisorAideCount -= 1;
                    break;
            }

            if (bKick)
                Send(string.Format(ServerString.STR_KICK_OUT_GUILD, pSender.SyndicateMember.GetRankName(), pSender.Name, pTarget.Name));

            Arsenal.RemoveAllFromUser(pTarget.Identity);

            bool bKernel = Members.TryRemove(pTarget.Identity, out pTarget);
            if (pTarget.IsOnline)
            {
                pTarget.Owner.Character.SyndicateIdentity = 0;
                pTarget.Owner.Character.SyndicateRank = SyndicateRank.NONE;

                pTarget.Owner.Character.Syndicate = null;
                pTarget.Delete();
                pTarget.Owner.Character.SyndicateMember = null;

                pTarget.Send(new MsgSyndicate() { Action = SyndicateRequest.SYN_DISBAND, Param = Identity });
                pTarget.Owner.Screen.RefreshSpawnForObservers();
            }

            ServerKernel.CaptureTheFlag.RemoveMember(pTarget.Identity);
            SendMembers(pSender, 0);
            MemberCount = (ushort) Members.Values.Count;
            Save();
            ServerKernel.Log.GmLog("Syndicate", string.Format("{0} has kicked {1} from the syndicate {2} [{3}]", pSender.Name, pTarget.Name, Name, Identity));
        }

        public void DisbandSyndicate(Character pSender)
        {
            if (pSender == null ||
                pSender.Syndicate == null ||
                pSender.SyndicateMember == null)
                return; // how the fuck is this possible? asshole

            if (pSender.SyndicateRank != SyndicateRank.GUILD_LEADER)
            {
                pSender.Send(ServerString.STR_NO_DISBAND_LEADER);
                return;
            }

            if (Members.Count > 1)
            {
                pSender.Send(ServerString.STR_NO_DISBAND);
                return;
            }

            pSender.SyndicateIdentity = 0;
            pSender.SyndicateRank = SyndicateRank.NONE;
            pSender.Syndicate = null;
            pSender.SyndicateMember.Delete();
            pSender.SyndicateMember = null;
            pSender.Screen.RefreshSpawnForObservers();

            pSender.Send(new MsgSyndicate() { Action = SyndicateRequest.SYN_DISBAND, Param = Identity});

            Delete();
            MemberCount = 0;

            SyndicateMember trash0;
            Members.TryRemove(pSender.Identity, out trash0);
            Save();

            // clear allies
            foreach (var ally in Allies.Values)
            {
                Syndicate trash;
                ally.Allies.TryRemove(Identity, out trash);
                ally.Send(new MsgSyndicate
                {
                    Action = SyndicateRequest.SYN_NEUTRAL1,
                    Param = Identity
                });
            }
            // clear enemies
            foreach (var enemy in Enemies.Values)
            {
                Syndicate trash;
                enemy.Enemies.TryRemove(Identity, out trash);
                enemy.Send(new MsgSyndicate
                {
                    Action = SyndicateRequest.SYN_NEUTRAL2,
                    Param = Identity
                });
            }

            new SyndicateAlliesRepository().ClearAlliesAndEnemies((ushort) Identity);
        }

        public void ProcessPaidPromotion(MsgSyndicate msg, Character sender)
        {
            if (sender == null || sender.SyndicateRank != SyndicateRank.GUILD_LEADER)
                return;

            var target = Members.Values.FirstOrDefault(x => x.Name == msg.Name);

            if (target == null
                || target.Owner == null
                || target.Owner.Character == null)
                return; // ?

            switch ((SyndicateRank) msg.Param)
            {
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                    if (HonoraryDeputyLeaderCount >= MaxHonoraryDeputyLeader)
                        return;
                    if (!ChangeEmoneyFunds(HONORARY_DEPUTY_LEADER_PRICE))
                    {
                        sender.Send("Not enough CPs fund.");
                        return;
                    }
                    break;
                case SyndicateRank.HONORARY_MANAGER:
                    if (HonoraryManagerCount >= MaxHonoraryManager)
                        return;
                    if (!ChangeEmoneyFunds(HONORARY_MANAGER_PRICE))
                    {
                        sender.Send("Not enough CPs fund.");
                        return;
                    }
                    break;
                case SyndicateRank.HONORARY_SUPERVISOR:
                    if (HonorarySupervisorCount >= MaxHonorarySupervisor)
                        return;
                    if (!ChangeEmoneyFunds(HONORARY_SUPERVISOR_PRICE))
                    {
                        sender.Send("Not enough CPs fund.");
                        return;
                    }
                    break;
                case SyndicateRank.HONORARY_STEWARD:
                    if (HonoraryStewardCount >= MaxHonorarySteward)
                        return;
                    if (!ChangeEmoneyFunds(HONORARY_STEWARD_PRICE))
                    {
                        sender.Send("Not enough CPs fund.");
                        return;
                    }
                    break;
                default:
                    return;
            }

            PromoteMember(sender, target, (SyndicateRank)msg.Param);
        }

        public bool PromoteMember(Character sender, SyndicateMember target, SyndicateRank pos)
        {
            if (target.Position == SyndicateRank.GUILD_LEADER)
            {
                return false;
            }

            // for guild leader or discharge use Abdicate or Discharge
            switch (pos)
            {
                case SyndicateRank.GUILD_LEADER:
                {
                    return AbdicateSyndicate(sender.SyndicateMember, target);
                }
                case SyndicateRank.DEPUTY_LEADER:
                    if (DeputyLeaderCount >= MaxDeputyLeader)
                        return false; // limit exceed
                    if (sender.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return false;
                    break;
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                    if (HonoraryDeputyLeaderCount >= MaxHonoraryDeputyLeader)
                        return false; // limit exceed
                    if (sender.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return false;
                    break;
                case SyndicateRank.DEPUTY_LEADER_AIDE:
                    if (DeputyLeaderAideCount >= 1)
                        return false; // limit exceed
                    if (sender.SyndicateRank != SyndicateRank.DEPUTY_LEADER)
                        return false;
                    break;
                case SyndicateRank.HONORARY_MANAGER:
                    if (HonoraryManagerCount >= MaxHonoraryManager)
                        return false; // limit exceed
                    if (sender.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return false;
                    break;
                case SyndicateRank.MANAGER_AIDE:
                    if (ManagerAideCount >= 1)
                        return false; // limit exceed
                    if (sender.SyndicateRank != SyndicateRank.MANAGER)
                        return false;
                    break;
                case SyndicateRank.HONORARY_SUPERVISOR:
                    if (HonorarySupervisorCount >= MaxHonorarySupervisor)
                        return false; // limit exceed
                    if (sender.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return false;
                    break;
                case SyndicateRank.SUPERVISOR_AIDE:
                    if (SupervisorAideCount >= 1)
                        return false; // limit exceed
                    if (sender.SyndicateRank != SyndicateRank.SUPERVISOR
                        || sender.SyndicateRank != SyndicateRank.ARSENAL_SUPERVISOR
                        || sender.SyndicateRank != SyndicateRank.CP_SUPERVISOR
                        || sender.SyndicateRank != SyndicateRank.SILVER_SUPERVISOR
                        || sender.SyndicateRank != SyndicateRank.GUIDE_SUPERVISOR
                        || sender.SyndicateRank != SyndicateRank.HONORARY_SUPERVISOR
                        || sender.SyndicateRank != SyndicateRank.LILY_SUPERVISOR
                        || sender.SyndicateRank != SyndicateRank.ORCHID_SUPERVISOR
                        || sender.SyndicateRank != SyndicateRank.ROSE_SUPERVISOR
                        || sender.SyndicateRank != SyndicateRank.TULIP_SUPERVISOR
                        || sender.SyndicateRank != SyndicateRank.PK_SUPERVISOR)
                        return false;
                    break;
                case SyndicateRank.HONORARY_STEWARD:
                    if (HonoraryStewardCount >= MaxHonorarySteward)
                        return false; // limit exceed
                    if (sender.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return false;
                    break;
                case SyndicateRank.AIDE:
                    if (AideCount >= MaxAide)
                        return false; // limit exceed
                    if (sender.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return false;
                    break;
                default:
                    return false; // change not enabled
            }
            // gotta change the amount
            switch (target.Position)
            {
                case SyndicateRank.GUILD_LEADER:
                    return false; // you cannot promote the guild leader -.-
                case SyndicateRank.DEPUTY_LEADER:
                    DeputyLeaderCount -= 1;
                    break;
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                    HonoraryDeputyLeaderCount -= 1;
                    break;
                case SyndicateRank.HONORARY_MANAGER:
                    HonoraryManagerCount -= 1;
                    break;
                case SyndicateRank.HONORARY_SUPERVISOR:
                    HonorarySupervisorCount -= 1;
                    break;
                case SyndicateRank.HONORARY_STEWARD:
                    HonoraryStewardCount -= 1;
                    break;
                case SyndicateRank.AIDE:
                    AideCount -= 1;
                    break;
                case SyndicateRank.LEADER_SPOUSE:
                    LeaderSpouseAideCount -= 1;
                    break;
                case SyndicateRank.DEPUTY_LEADER_AIDE:
                    DeputyLeaderAideCount -= 1;
                    break;
                case SyndicateRank.MANAGER_AIDE:
                    ManagerAideCount -= 1;
                    break;
                case SyndicateRank.SUPERVISOR_AIDE:
                    SupervisorAideCount -= 1;
                    break;
            }

            SyndicateRank oldRank = target.Position;

            target.Position = pos;
            target.Owner.Screen.RefreshSpawnForObservers();
            target.SendSyndicate();

            if (pos == SyndicateRank.HONORARY_DEPUTY_LEADER || pos == SyndicateRank.HONORARY_MANAGER ||
                pos == SyndicateRank.HONORARY_STEWARD || pos == SyndicateRank.HONORARY_SUPERVISOR)
                target.PositionExpire = (uint) UnixTimestamp.Timestamp() + UnixTimestamp.TIME_SECONDS_DAY*30;
            else
                target.PositionExpire = 0;

            Send(string.Format(ServerString.STR_SYNDICATE_PROMOTE, sender.SyndicateMember.GetRankName(), sender.Name, target.Name, target.GetRankName()));

            target.Save();

            switch (target.Position)
            {
                case SyndicateRank.DEPUTY_LEADER:
                    DeputyLeaderCount += 1;
                    break;
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                    HonoraryDeputyLeaderCount += 1;
                    break;
                case SyndicateRank.HONORARY_MANAGER:
                    HonoraryManagerCount += 1;
                    break;
                case SyndicateRank.HONORARY_SUPERVISOR:
                    HonorarySupervisorCount += 1;
                    break;
                case SyndicateRank.HONORARY_STEWARD:
                    HonoraryStewardCount += 1;
                    break;
                case SyndicateRank.AIDE:
                    AideCount += 1;
                    break;
                case SyndicateRank.LEADER_SPOUSE:
                    LeaderSpouseAideCount += 1;
                    break;
                case SyndicateRank.DEPUTY_LEADER_AIDE:
                    DeputyLeaderAideCount += 1;
                    break;
                case SyndicateRank.MANAGER_AIDE:
                    ManagerAideCount += 1;
                    break;
                case SyndicateRank.SUPERVISOR_AIDE:
                    SupervisorAideCount += 1;
                    break;
            }
            
            Arsenal.UpdatePoles();
            target.Owner.Character.UpdateClient(ClientUpdateType.GUILD_BATTLEPOWER, Arsenal.SharedBattlePower(target.Position), false);
            return true;
        }

        public bool DischargeMember(Client sender, string strName)
        {
            var target = Members.Values.FirstOrDefault(x => x.Name == strName);

            if (sender == null
                || target == null
                || sender.Character == null)
                return false; // ?

            return DischargeMember(sender.Character, target);
        }

        public bool DischargeMember(Character sender, SyndicateMember target)
        {
            // gotta change the amount
            switch (target.Position)
            {
                case SyndicateRank.GUILD_LEADER:
                    return false; // you cannot demote the guild leader -.-
                case SyndicateRank.DEPUTY_LEADER:
                    DeputyLeaderCount -= 1;
                    break;
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                    HonoraryDeputyLeaderCount -= 1;
                    break;
                case SyndicateRank.HONORARY_MANAGER:
                    HonoraryManagerCount -= 1;
                    break;
                case SyndicateRank.HONORARY_SUPERVISOR:
                    HonorarySupervisorCount -= 1;
                    break;
                case SyndicateRank.HONORARY_STEWARD:
                    HonoraryStewardCount -= 1;
                    break;
                case SyndicateRank.AIDE:
                    AideCount -= 1;
                    break;
                case SyndicateRank.LEADER_SPOUSE:
                    LeaderSpouseAideCount -= 1;
                    break;
                case SyndicateRank.DEPUTY_LEADER_AIDE:
                    DeputyLeaderAideCount -= 1;
                    break;
                case SyndicateRank.MANAGER_AIDE:
                    ManagerAideCount -= 1;
                    break;
                case SyndicateRank.SUPERVISOR_AIDE:
                    SupervisorAideCount -= 1;
                    break;
            }

            var oldPos = target.GetRankName();
            var oldRank = target.Position;
            target.Position = SyndicateRank.MEMBER;
            target.PositionExpire = 0;

            if (target.IsOnline)
            {
                target.Owner.Screen.RefreshSpawnForObservers();
                target.SendSyndicate();
            }

            Send(string.Format(ServerString.STR_SYNDICATE_DISCHARGE, sender.SyndicateMember.GetRankName(), sender.Name, target.Name, oldPos));
            SendMembers(sender, 0);
            return true;
        }

        public void CheckLeaderSpouse()
        {
            DbUser pLeader = Database.Characters.SearchByName(LeaderName);
            if (!Deleted && pLeader != null && pLeader.Mate != "None")
            {
                DbUser pSpouse = Database.Characters.SearchByName(pLeader.Mate);
                if (pSpouse != null)
                {
                    DbCqSynattr spAttr = Database.SyndicateMembersRepository.FetchByUser(pSpouse.Identity);
                    if (spAttr != null && spAttr.SynId == Identity
                        && spAttr.Rank != (int)SyndicateRank.LEADER_SPOUSE
                        && !SyndicateMember.IsUserSetPosition((SyndicateRank)spAttr.Rank))
                    {
                        spAttr.Rank = (ushort)SyndicateRank.LEADER_SPOUSE;
                        Database.SyndicateMembersRepository.SaveOrUpdate(spAttr);
                    }
                }
            }
        }

        public bool IsSpousePosition(SyndicateRank pos)
        {
            switch (pos)
            {
                case SyndicateRank.LEADER_SPOUSE:
                case SyndicateRank.DEPUTY_LEADER_SPOUSE:
                case SyndicateRank.MANAGER_SPOUSE:
                case SyndicateRank.SUPERVISOR_SPOUSE:
                    return true;
            }
            return false;
        }

        public bool HasSpousePosition(SyndicateRank pos)
        {
            switch (pos)
            {
                case SyndicateRank.GUILD_LEADER:
                case SyndicateRank.DEPUTY_LEADER:
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                case SyndicateRank.MANAGER:
                case SyndicateRank.HONORARY_MANAGER:
                case SyndicateRank.SUPERVISOR:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Used to compare when switching positions automatically.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns>Returns true if p1 is higher than p2.</returns>
        private bool IsHigherPosition(SyndicateRank p1, SyndicateRank p2)
        {
            if (SyndicateMember.IsUserSetPosition(p2))
                return false;
            return p1 > p2;
        }

        public bool AbdicateSyndicate(SyndicateMember sender, string strName)
        {
            var target = Members.Values.FirstOrDefault(x => x.Name == strName);

            if (sender == null
                || target == null
                || target.Owner == null
                || target.Owner.Character == null
                || sender.Position != SyndicateRank.GUILD_LEADER)
                return false; // ?

            return AbdicateSyndicate(sender, target);
        }

        public bool AbdicateSyndicate(SyndicateMember sender, SyndicateMember target)
        {
            try
            {
                if (sender.Position != SyndicateRank.GUILD_LEADER || sender == target)
                    return false;

                // gotta change the amount
                switch (target.Position)
                {
                    case SyndicateRank.GUILD_LEADER:
                        return false; // the target is already leader?
                    case SyndicateRank.DEPUTY_LEADER:
                        DeputyLeaderCount -= 1;
                        break;
                    case SyndicateRank.HONORARY_DEPUTY_LEADER:
                        HonoraryDeputyLeaderCount -= 1;
                        break;
                    case SyndicateRank.HONORARY_MANAGER:
                        HonoraryManagerCount -= 1;
                        break;
                    case SyndicateRank.HONORARY_SUPERVISOR:
                        HonorarySupervisorCount -= 1;
                        break;
                    case SyndicateRank.HONORARY_STEWARD:
                        HonoraryStewardCount -= 1;
                        break;
                    case SyndicateRank.AIDE:
                        AideCount -= 1;
                        break;
                    case SyndicateRank.LEADER_SPOUSE_AIDE:
                        LeaderSpouseAideCount -= 1;
                        break;
                    case SyndicateRank.DEPUTY_LEADER_AIDE:
                        DeputyLeaderAideCount -= 1;
                        break;
                    case SyndicateRank.MANAGER_AIDE:
                        ManagerAideCount -= 1;
                        break;
                    case SyndicateRank.SUPERVISOR_AIDE:
                        SupervisorAideCount -= 1;
                        break;
                }

                LeaderIdentity = target.Identity;
                LeaderName = target.Name;

                target.Position = SyndicateRank.GUILD_LEADER;
                target.PositionExpire = 0;
                target.Owner.Screen.RefreshSpawnForObservers();
                target.SendSyndicate();

                sender.Position = SyndicateRank.MEMBER;
                sender.Owner.Screen.RefreshSpawnForObservers();
                sender.SendSyndicate();

                CheckLeaderSpouse();

                Send(string.Format("{0} has abdicated the guild to {1}.", sender.Name, target.Name));
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Alliance / Antagonize
        public bool IsHostile(ushort identity) { return Enemies.ContainsKey(identity); }

        public bool AntagonizeSyndicate(string szName)
        {
            Syndicate temp = Enemies.Values.FirstOrDefault(x => x.Name == szName);
            if (temp != null)
                return false;
            var syn = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Name == szName && !Deleted);
            if (syn == null) return false;

            return AntagonizeSyndicate(syn);
        }

        public bool AntagonizeSyndicate(ushort identity)
        {
            Syndicate temp;
            if (Enemies.ContainsKey(identity) || !ServerKernel.Syndicates.TryGetValue(identity, out temp))
                return false;

            return AntagonizeSyndicate(temp);
        }

        public bool AntagonizeSyndicate(Syndicate syn)
        {
            if (Enemies.ContainsKey((ushort)syn.Identity) || Allies.ContainsKey((ushort)syn.Identity)) return false;

            if (Enemies.Count >= 5)
                return false;

            var enemy = new DbCqSynEnemy
            {
                Enemyid = syn.Identity,
                Enemyname = syn.Name,
                Synid = (ushort)Identity,
                Synname = Name
            };

            if (Enemies.TryAdd((ushort)syn.Identity, syn) && new SyndicateEnemiesRepository().SaveOrUpdate(enemy))
            {
                var msg = new MsgSyndicate { Action = SyndicateRequest.SYN_ENEMIED, Param = syn.Identity };
                // msg.Append(Name);
                Send(msg);

                var pMsg = new MsgName
                {
                    Action = StringAction.SET_ENEMY,
                    Identity = syn.Identity
                };
                pMsg.Append(string.Format("{0} {1} {2} {3}", syn.Name, syn.LeaderName, syn.Level, syn.MemberCount));
                Send(pMsg);

                Database.SyndicateEnemies.SaveOrUpdate(enemy);

                Send(string.Format("Guild Leader {0} has added Guild {1} to the enemies list.", LeaderName, syn.Name));
                syn.Send(string.Format("The Guild Leader {0} of the Guild {1} has added us to the enemies list.", LeaderName, Name));
                return true;
            }
            return false;
        }

        public bool RemoveEnemy(uint identity)
        {
            Syndicate temp;
            if (!Enemies.TryRemove(identity, out temp)) return false;
            new SyndicateEnemiesRepository().DeleteAntagonize((ushort) identity, (ushort)Identity);

            var msg = new MsgSyndicate
            {
                Action = SyndicateRequest.SYN_NEUTRAL2,
                Param = identity
            };
            Send(msg);

            Send(string.Format("Guild Leader {0} has removed the Guild {1} from the enemy list.", LeaderName, temp.Name));
            temp.Send(string.Format("The Guild Leader {0} of {1} has removed our guild from the enemy list.", LeaderName, Name));

            return true;
        }

        public bool AllySyndicate(string szName)
        {
            Syndicate temp = Allies.Values.FirstOrDefault(x => x.Name == szName);
            if (temp != null)
                return false;
            var syn = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Name == szName && !Deleted);
            if (syn == null) return false;

            return AllySyndicate(syn);
        }

        public bool AllySyndicate(ushort identity)
        {
            Syndicate temp;
            if (Allies.ContainsKey(identity) || !ServerKernel.Syndicates.TryGetValue(identity, out temp))
                return false;

            return AllySyndicate(temp);
        }

        public bool AllySyndicate(Syndicate syn)
        {
            if (Allies.ContainsKey((ushort)syn.Identity) || Enemies.ContainsKey((ushort)syn.Identity)) return false;

            if (Allies.Count >= 5)
                return false;

            if (Allies.TryAdd((ushort)syn.Identity, syn) && syn.Allies.TryAdd((ushort)Identity, this))
            {
                Database.SyndicateAllies.SaveOrUpdate(new DbSynAlly
                {
                    Allyid = (ushort)syn.Identity,
                    Allyname = syn.Name,
                    Synid = (ushort)Identity,
                    Synname = Name
                });
                Database.SyndicateAllies.SaveOrUpdate(new DbSynAlly
                {
                    Allyid = Identity,
                    Allyname = Name,
                    Synid = syn.Identity,
                    Synname = syn.Name
                });

                var msg = new MsgSyndicate { Action = SyndicateRequest.SYN_ALLIED, Param = syn.Identity };
                // msg.Append(Name);
                Send(msg);

                var pMsg = new MsgName
                {
                    Action = StringAction.SET_ALLY,
                    Identity = syn.Identity
                };
                pMsg.Append(string.Format("{0} {1} {2} {3}", syn.Name, syn.LeaderName, syn.Level, syn.MemberCount));
                Send(pMsg);

                msg.Param = Identity;
                syn.Send(msg);
                pMsg = new MsgName
                {
                    Action = StringAction.SET_ALLY,
                    Identity = Identity
                };
                pMsg.Append(string.Format("{0} {1} {2} {3}", Name, LeaderName, Level, MemberCount));
                syn.Send(pMsg);

                //Send(string.Format("Guild Leader {0} has added Guild {1} to the allies list.", LeaderName, syn.Name));
                //syn.Send(string.Format("Guild Leader {0} of guild {1} added us allies list.", LeaderName, Name));
                return true;
            }
            return false;
        }

        public bool RemoveAlliance(uint identity)
        {
            Syndicate temp;
            if (!Allies.TryRemove(identity, out temp)) return false;
            new SyndicateAlliesRepository().DeleteRelationship((ushort) identity, (ushort)Identity);

            var msg = new MsgSyndicate
            {
                Action = SyndicateRequest.SYN_NEUTRAL1,
                Param = identity
            };
            Send(msg);
            Send(string.Format("Guild Leader {0} has removed the Guild {1} from the allies list.", LeaderName, temp.Name));

            msg.Param = Identity;
            temp.Send(msg);
            temp.Send(string.Format("The Guild Leader {0} of {1} has removed our guild from the allies list.", LeaderName, Name));

            return true;
        }

        public bool IsFriendly(ushort identity) { return Allies.ContainsKey(identity); }
        #endregion

        #region Funds Management

        public bool ChangeFunds(int nAmount)
        {
            if (nAmount < 0)
            {
                if (nAmount + (long) SilverDonation < 0)
                {
                    SilverDonation = (ulong) Math.Max(nAmount + (long) SilverDonation, 0);
                    return false;
                }
                nAmount *= -1;
                SilverDonation -= (ulong)nAmount;
                return Save();
            }

            if (nAmount > 0)
            {
                if ((ulong)nAmount + SilverDonation > ulong.MaxValue)
                {
                    SilverDonation = ulong.MaxValue;
                    return Save();
                }
                SilverDonation += (ulong)nAmount;
                return Save();
            }
            return false;
        }

        public bool ChangeEmoneyFunds(int nAmount)
        {
            if (nAmount < 0)
            {
                if (nAmount + EmoneyDonation < 0)
                    return false;
                nAmount *= -1;
                EmoneyDonation -= (uint) nAmount;
                return Save();
            }

            if (nAmount > 0)
            {
                if ((uint)nAmount + EmoneyDonation > uint.MaxValue)
                {
                    EmoneyDonation = uint.MaxValue;
                    return Save();
                }
                EmoneyDonation += (uint)nAmount;
                return Save();
            }
            return false;
        }

        #endregion

        #region Syndicate War

        public void AddSynWarScore(DynamicNpc pNpc, uint nScore)
        {
            if (!pNpc.Scores.ContainsKey(Identity))
            {
                var sScore = new SynScore(this);
                sScore.Score += nScore;
                pNpc.Scores.TryAdd(Identity, sScore);
                return;
            }
            pNpc.Scores[Identity].Score += nScore;
        }

        #endregion

        #region Socket

        public MsgName NamePacket
        {
            get
            {
                MsgName pMsg = new MsgName
                {
                    Action = StringAction.GUILD,
                    Identity = Identity
                };
                pMsg.Append(string.Format("{0} {1} {2} {3}", Name.Replace(' ', '~'), LeaderName.Replace(' ', '~'), Level, MemberCount));
                return pMsg;
            }
        }

        public void SendName()
        {
            foreach (var plr in ServerKernel.Players.Values)
                plr.Send(NamePacket);
        }

        public void SendName(Character pTarget, bool bTransmit = false)
        {
            if (bTransmit)
            {
                pTarget.Screen.Send(NamePacket, true);
                return;
            }
            pTarget.Send(NamePacket);
        }

        public void SendRelation(Character pTarget)
        {
            foreach (var syn in Allies.Values)
            {
                var allies = new MsgName
                {
                    Identity = syn.Identity,
                    Action = StringAction.SET_ALLY
                };
                string test = string.Format("{0} {1} {2} {3}", syn.Name, syn.LeaderName, syn.Level, syn.MemberCount);
                allies.Append(test);
                pTarget.Send(allies);
            }

            foreach (var syn in Enemies.Values)
            {
                var enemies = new MsgName
                {
                    Identity = syn.Identity,
                    Action = StringAction.SET_ENEMY
                };
                enemies.Append(string.Format("{0} {1} {2} {3}", syn.Name, syn.LeaderName, syn.Level, syn.MemberCount));
                pTarget.Send(enemies);
            }
        }

        public void SendMembers(Character pTarget, uint idx)
        {
            var msg = new MsgSynMemberList
            {
                StartIndex = idx,
            };

            uint maxmem = idx + 12;
            uint minmem = idx;
            uint count = 0;

            foreach (var member in Members.Values.OrderByDescending(x => x.IsOnline ? 1 : 0).ThenByDescending(x => x.Position))
            {
                if (count < minmem || count >= maxmem)
                {
                    count++;
                    continue;
                }

                uint expire = 0;
                uint time = (uint)UnixTimestamp.Timestamp();
                if (time < member.PositionExpire)
                    expire = uint.Parse(UnixTimestamp.ToDateTime(member.PositionExpire).ToString("yyyyMMdd"));

                msg.Append(member.Name, member.Lookface, member.Nobility, member.Level, member.Position, expire /*time*/,
                    member.TotalDonation, member.IsOnline, member.Profession, 1);

                count++;
            }

            pTarget.Send(msg);
        }

        #endregion

        #region Message

        public void SetAnnouncement(string szMessage)
        {
            if (szMessage.Length > 127)
                szMessage = szMessage.Substring(0, 127);
            
            Announcement = szMessage;

            Send(new MsgTalk(Announcement, ChatTone.GUILD_ANNOUNCEMENT));
        }

        public void Send(byte[] pBuffer)
        {
            foreach (var plr in Members.Values.Where(x => x.IsOnline))
            {
                plr.Send(pBuffer);
            }
        }

        public void Send(byte[] pBuffer, uint idSender)
        {
            foreach (var plr in Members.Values.Where(x => x.IsOnline && x.Identity != idSender))
            {
                plr.Send(pBuffer);
            }
        }

        public void Send(string szMessage)
        {
            foreach (var plr in Members.Values.Where(x => x.IsOnline))
            {
                plr.Send(new MsgTalk(szMessage, ChatTone.GUILD));
            }
        }

        public void Send(string szMessage, uint idSender)
        {
            foreach (var plr in Members.Values.Where(x => x.IsOnline && x.Identity != idSender))
            {
                plr.Send(new MsgTalk(szMessage, ChatTone.GUILD));
            }
        }

        #endregion

        #region Position Count

        public int DeputyLeaderCount = 0,
            HonoraryDeputyLeaderCount = 0,
            DeputyLeaderAideCount = 0,
            HonoraryManagerCount = 0,
            HonorarySupervisorCount = 0,
            AideCount = 0,
            ManagerAideCount = 0,
            SupervisorAideCount = 0,
            LeaderSpouseAideCount = 0,
            HonoraryStewardCount = 0;

        public ushort MaxAllies()
        {
            return MaxAllies(Level);
        }

        public ushort MaxAllies(ushort level)
        {
            switch (level)
            {
                case 1: return 5;
                case 2: return 7;
                case 3: return 9;
                case 4: return 12;
                default: return 15;
            }
        }

        public ushort MaxEnemies()
        {
            return MaxEnemies(Level);
        }

        public ushort MaxEnemies(ushort level)
        {
            switch (level)
            {
                case 1: return 5;
                case 2: return 7;
                case 3: return 9;
                case 4: return 12;
                default: return 15;
            }
        }

        public uint MaxPositionAmount(SyndicateRank pos)
        {
            switch (Level)
            {
                #region Level 1
                case 1:
                    {
                        switch (pos)
                        {
                            case SyndicateRank.MANAGER:
                            case SyndicateRank.MANAGER_AIDE:
                                return 1;
                            case SyndicateRank.SUPERVISOR:
                            case SyndicateRank.ARSENAL_SUPERVISOR:
                            case SyndicateRank.CP_SUPERVISOR:
                            case SyndicateRank.GUIDE_SUPERVISOR:
                            case SyndicateRank.LILY_SUPERVISOR:
                            case SyndicateRank.ORCHID_SUPERVISOR:
                            case SyndicateRank.PK_SUPERVISOR:
                            case SyndicateRank.SILVER_SUPERVISOR:
                            case SyndicateRank.TULIP_SUPERVISOR:
                            case SyndicateRank.STEWARD:
                                return 0;
                            case SyndicateRank.AGENT:
                            case SyndicateRank.ARSENAL_AGENT:
                            case SyndicateRank.CP_AGENT:
                            case SyndicateRank.GUIDE_AGENT:
                            case SyndicateRank.LILY_AGENT:
                            case SyndicateRank.ORCHID_AGENT:
                            case SyndicateRank.PK_AGENT:
                            case SyndicateRank.SILVER_AGENT:
                            case SyndicateRank.TULIP_AGENT:
                            case SyndicateRank.FOLLOWER:
                            case SyndicateRank.ARSENAL_FOLLOWER:
                            case SyndicateRank.CP_FOLLOWER:
                            case SyndicateRank.GUIDE_FOLLOWER:
                            case SyndicateRank.LILY_FOLLOWER:
                            case SyndicateRank.ORCHID_FOLLOWER:
                            case SyndicateRank.PK_FOLLOWER:
                            case SyndicateRank.SILVER_FOLLOWER:
                            case SyndicateRank.TULIP_FOLLOWER:
                                return 1;
                            default:
                                return 0;
                        }
                    }
                #endregion
                #region Level 2
                case 2:
                    {
                        switch (pos)
                        {
                            case SyndicateRank.MANAGER:
                            case SyndicateRank.MANAGER_AIDE:
                                return 1;
                            case SyndicateRank.SUPERVISOR:
                            case SyndicateRank.ARSENAL_SUPERVISOR:
                            case SyndicateRank.CP_SUPERVISOR:
                            case SyndicateRank.GUIDE_SUPERVISOR:
                            case SyndicateRank.LILY_SUPERVISOR:
                            case SyndicateRank.ORCHID_SUPERVISOR:
                            case SyndicateRank.PK_SUPERVISOR:
                            case SyndicateRank.SILVER_SUPERVISOR:
                            case SyndicateRank.TULIP_SUPERVISOR:
                                return 0;
                            case SyndicateRank.STEWARD:
                                return 1;
                            case SyndicateRank.AGENT:
                            case SyndicateRank.ARSENAL_AGENT:
                            case SyndicateRank.CP_AGENT:
                            case SyndicateRank.GUIDE_AGENT:
                            case SyndicateRank.LILY_AGENT:
                            case SyndicateRank.ORCHID_AGENT:
                            case SyndicateRank.PK_AGENT:
                            case SyndicateRank.SILVER_AGENT:
                            case SyndicateRank.TULIP_AGENT:
                            case SyndicateRank.FOLLOWER:
                            case SyndicateRank.ARSENAL_FOLLOWER:
                            case SyndicateRank.CP_FOLLOWER:
                            case SyndicateRank.GUIDE_FOLLOWER:
                            case SyndicateRank.LILY_FOLLOWER:
                            case SyndicateRank.ORCHID_FOLLOWER:
                            case SyndicateRank.PK_FOLLOWER:
                            case SyndicateRank.SILVER_FOLLOWER:
                            case SyndicateRank.TULIP_FOLLOWER:
                                return 1;
                            default:
                                return 0;
                        }
                    }
                #endregion
                #region Level 3
                case 3:
                    {
                        switch (pos)
                        {
                            case SyndicateRank.MANAGER:
                            case SyndicateRank.MANAGER_AIDE:
                                return 2;
                            case SyndicateRank.SUPERVISOR:
                            case SyndicateRank.ARSENAL_SUPERVISOR:
                            case SyndicateRank.CP_SUPERVISOR:
                            case SyndicateRank.GUIDE_SUPERVISOR:
                            case SyndicateRank.LILY_SUPERVISOR:
                            case SyndicateRank.ORCHID_SUPERVISOR:
                            case SyndicateRank.PK_SUPERVISOR:
                            case SyndicateRank.SILVER_SUPERVISOR:
                            case SyndicateRank.TULIP_SUPERVISOR:
                                return 0;
                            case SyndicateRank.STEWARD:
                                return 2;
                            case SyndicateRank.AGENT:
                            case SyndicateRank.ARSENAL_AGENT:
                            case SyndicateRank.CP_AGENT:
                            case SyndicateRank.GUIDE_AGENT:
                            case SyndicateRank.LILY_AGENT:
                            case SyndicateRank.ORCHID_AGENT:
                            case SyndicateRank.PK_AGENT:
                            case SyndicateRank.SILVER_AGENT:
                            case SyndicateRank.TULIP_AGENT:
                            case SyndicateRank.FOLLOWER:
                            case SyndicateRank.ARSENAL_FOLLOWER:
                            case SyndicateRank.CP_FOLLOWER:
                            case SyndicateRank.GUIDE_FOLLOWER:
                            case SyndicateRank.LILY_FOLLOWER:
                            case SyndicateRank.ORCHID_FOLLOWER:
                            case SyndicateRank.PK_FOLLOWER:
                            case SyndicateRank.SILVER_FOLLOWER:
                            case SyndicateRank.TULIP_FOLLOWER:
                                return 1;
                            default:
                                return 0;
                        }
                    }
                #endregion
                #region Level 4
                case 4:
                    {
                        switch (pos)
                        {
                            case SyndicateRank.MANAGER:
                            case SyndicateRank.MANAGER_AIDE:
                                return 2;
                            case SyndicateRank.SUPERVISOR:
                            case SyndicateRank.ARSENAL_SUPERVISOR:
                            case SyndicateRank.CP_SUPERVISOR:
                            case SyndicateRank.GUIDE_SUPERVISOR:
                            case SyndicateRank.LILY_SUPERVISOR:
                            case SyndicateRank.ORCHID_SUPERVISOR:
                            case SyndicateRank.PK_SUPERVISOR:
                            case SyndicateRank.SILVER_SUPERVISOR:
                            case SyndicateRank.TULIP_SUPERVISOR:
                                return 1;
                            case SyndicateRank.STEWARD:
                                return 3;
                            case SyndicateRank.AGENT:
                            case SyndicateRank.ARSENAL_AGENT:
                            case SyndicateRank.CP_AGENT:
                            case SyndicateRank.GUIDE_AGENT:
                            case SyndicateRank.LILY_AGENT:
                            case SyndicateRank.ORCHID_AGENT:
                            case SyndicateRank.PK_AGENT:
                            case SyndicateRank.SILVER_AGENT:
                            case SyndicateRank.TULIP_AGENT:
                            case SyndicateRank.FOLLOWER:
                            case SyndicateRank.ARSENAL_FOLLOWER:
                            case SyndicateRank.CP_FOLLOWER:
                            case SyndicateRank.GUIDE_FOLLOWER:
                            case SyndicateRank.LILY_FOLLOWER:
                            case SyndicateRank.ORCHID_FOLLOWER:
                            case SyndicateRank.PK_FOLLOWER:
                            case SyndicateRank.SILVER_FOLLOWER:
                            case SyndicateRank.TULIP_FOLLOWER:
                                return 1;
                            default:
                                return 0;
                        }
                    }
                #endregion
                #region Level 5
                case 5:
                    {
                        switch (pos)
                        {
                            case SyndicateRank.MANAGER:
                            case SyndicateRank.MANAGER_AIDE:
                                return 4;
                            case SyndicateRank.SUPERVISOR:
                            case SyndicateRank.ARSENAL_SUPERVISOR:
                            case SyndicateRank.CP_SUPERVISOR:
                            case SyndicateRank.GUIDE_SUPERVISOR:
                            case SyndicateRank.LILY_SUPERVISOR:
                            case SyndicateRank.ORCHID_SUPERVISOR:
                            case SyndicateRank.PK_SUPERVISOR:
                            case SyndicateRank.SILVER_SUPERVISOR:
                            case SyndicateRank.TULIP_SUPERVISOR:
                                return 1;
                            case SyndicateRank.STEWARD:
                                return 4;
                            case SyndicateRank.AGENT:
                            case SyndicateRank.ARSENAL_AGENT:
                            case SyndicateRank.CP_AGENT:
                            case SyndicateRank.GUIDE_AGENT:
                            case SyndicateRank.LILY_AGENT:
                            case SyndicateRank.ORCHID_AGENT:
                            case SyndicateRank.PK_AGENT:
                            case SyndicateRank.SILVER_AGENT:
                            case SyndicateRank.TULIP_AGENT:
                            case SyndicateRank.FOLLOWER:
                            case SyndicateRank.ARSENAL_FOLLOWER:
                            case SyndicateRank.CP_FOLLOWER:
                            case SyndicateRank.GUIDE_FOLLOWER:
                            case SyndicateRank.LILY_FOLLOWER:
                            case SyndicateRank.ORCHID_FOLLOWER:
                            case SyndicateRank.PK_FOLLOWER:
                            case SyndicateRank.SILVER_FOLLOWER:
                            case SyndicateRank.TULIP_FOLLOWER:
                                return 1;
                            default:
                                return 0;
                        }
                    }
                #endregion
                #region Level 6
                case 6:
                    {
                        switch (pos)
                        {
                            case SyndicateRank.MANAGER:
                            case SyndicateRank.MANAGER_AIDE:
                                return 4;
                            case SyndicateRank.SUPERVISOR:
                            case SyndicateRank.ARSENAL_SUPERVISOR:
                            case SyndicateRank.CP_SUPERVISOR:
                            case SyndicateRank.GUIDE_SUPERVISOR:
                            case SyndicateRank.LILY_SUPERVISOR:
                            case SyndicateRank.ORCHID_SUPERVISOR:
                            case SyndicateRank.PK_SUPERVISOR:
                            case SyndicateRank.SILVER_SUPERVISOR:
                            case SyndicateRank.TULIP_SUPERVISOR:
                                return 1;
                            case SyndicateRank.STEWARD:
                                return 5;
                            case SyndicateRank.AGENT:
                            case SyndicateRank.ARSENAL_AGENT:
                            case SyndicateRank.CP_AGENT:
                            case SyndicateRank.GUIDE_AGENT:
                            case SyndicateRank.LILY_AGENT:
                            case SyndicateRank.ORCHID_AGENT:
                            case SyndicateRank.PK_AGENT:
                            case SyndicateRank.SILVER_AGENT:
                            case SyndicateRank.TULIP_AGENT:
                            case SyndicateRank.FOLLOWER:
                            case SyndicateRank.ARSENAL_FOLLOWER:
                            case SyndicateRank.CP_FOLLOWER:
                            case SyndicateRank.GUIDE_FOLLOWER:
                            case SyndicateRank.LILY_FOLLOWER:
                            case SyndicateRank.ORCHID_FOLLOWER:
                            case SyndicateRank.PK_FOLLOWER:
                            case SyndicateRank.SILVER_FOLLOWER:
                            case SyndicateRank.TULIP_FOLLOWER:
                                return 1;
                            default:
                                return 0;
                        }
                    }
                #endregion
                #region Level 7
                case 7:
                    {
                        switch (pos)
                        {
                            case SyndicateRank.MANAGER:
                            case SyndicateRank.MANAGER_AIDE:
                                return 6;
                            case SyndicateRank.SUPERVISOR:
                            case SyndicateRank.ARSENAL_SUPERVISOR:
                            case SyndicateRank.CP_SUPERVISOR:
                            case SyndicateRank.GUIDE_SUPERVISOR:
                            case SyndicateRank.LILY_SUPERVISOR:
                            case SyndicateRank.ORCHID_SUPERVISOR:
                            case SyndicateRank.PK_SUPERVISOR:
                            case SyndicateRank.SILVER_SUPERVISOR:
                            case SyndicateRank.TULIP_SUPERVISOR:
                                return 1;
                            case SyndicateRank.STEWARD:
                                return 6;
                            case SyndicateRank.AGENT:
                            case SyndicateRank.ARSENAL_AGENT:
                            case SyndicateRank.CP_AGENT:
                            case SyndicateRank.GUIDE_AGENT:
                            case SyndicateRank.LILY_AGENT:
                            case SyndicateRank.ORCHID_AGENT:
                            case SyndicateRank.PK_AGENT:
                            case SyndicateRank.SILVER_AGENT:
                            case SyndicateRank.TULIP_AGENT:
                            case SyndicateRank.FOLLOWER:
                            case SyndicateRank.ARSENAL_FOLLOWER:
                            case SyndicateRank.CP_FOLLOWER:
                            case SyndicateRank.GUIDE_FOLLOWER:
                            case SyndicateRank.LILY_FOLLOWER:
                            case SyndicateRank.ORCHID_FOLLOWER:
                            case SyndicateRank.PK_FOLLOWER:
                            case SyndicateRank.SILVER_FOLLOWER:
                            case SyndicateRank.TULIP_FOLLOWER:
                                return 1;
                            default:
                                return 0;
                        }
                    }
                #endregion
                #region Level 8
                case 8:
                    {
                        switch (pos)
                        {
                            case SyndicateRank.MANAGER:
                            case SyndicateRank.MANAGER_AIDE:
                                return 6;
                            case SyndicateRank.SUPERVISOR:
                            case SyndicateRank.ARSENAL_SUPERVISOR:
                            case SyndicateRank.CP_SUPERVISOR:
                            case SyndicateRank.GUIDE_SUPERVISOR:
                            case SyndicateRank.LILY_SUPERVISOR:
                            case SyndicateRank.ORCHID_SUPERVISOR:
                            case SyndicateRank.PK_SUPERVISOR:
                            case SyndicateRank.SILVER_SUPERVISOR:
                            case SyndicateRank.TULIP_SUPERVISOR:
                                return 2;
                            case SyndicateRank.STEWARD:
                                return 7;
                            case SyndicateRank.AGENT:
                            case SyndicateRank.ARSENAL_AGENT:
                            case SyndicateRank.CP_AGENT:
                            case SyndicateRank.GUIDE_AGENT:
                            case SyndicateRank.LILY_AGENT:
                            case SyndicateRank.ORCHID_AGENT:
                            case SyndicateRank.PK_AGENT:
                            case SyndicateRank.SILVER_AGENT:
                            case SyndicateRank.TULIP_AGENT:
                            case SyndicateRank.FOLLOWER:
                            case SyndicateRank.ARSENAL_FOLLOWER:
                            case SyndicateRank.CP_FOLLOWER:
                            case SyndicateRank.GUIDE_FOLLOWER:
                            case SyndicateRank.LILY_FOLLOWER:
                            case SyndicateRank.ORCHID_FOLLOWER:
                            case SyndicateRank.PK_FOLLOWER:
                            case SyndicateRank.SILVER_FOLLOWER:
                            case SyndicateRank.TULIP_FOLLOWER:
                                return 1;
                            default:
                                return 0;
                        }
                    }
                #endregion
                #region Level 9
                case 9:
                    {
                        switch (pos)
                        {
                            case SyndicateRank.MANAGER:
                            case SyndicateRank.MANAGER_AIDE:
                                return 8;
                            case SyndicateRank.SUPERVISOR:
                            case SyndicateRank.ARSENAL_SUPERVISOR:
                            case SyndicateRank.CP_SUPERVISOR:
                            case SyndicateRank.GUIDE_SUPERVISOR:
                            case SyndicateRank.LILY_SUPERVISOR:
                            case SyndicateRank.ORCHID_SUPERVISOR:
                            case SyndicateRank.PK_SUPERVISOR:
                            case SyndicateRank.SILVER_SUPERVISOR:
                            case SyndicateRank.TULIP_SUPERVISOR:
                                return 2;
                            case SyndicateRank.STEWARD:
                                return 8;
                            case SyndicateRank.AGENT:
                            case SyndicateRank.ARSENAL_AGENT:
                            case SyndicateRank.CP_AGENT:
                            case SyndicateRank.GUIDE_AGENT:
                            case SyndicateRank.LILY_AGENT:
                            case SyndicateRank.ORCHID_AGENT:
                            case SyndicateRank.PK_AGENT:
                            case SyndicateRank.SILVER_AGENT:
                            case SyndicateRank.TULIP_AGENT:
                            case SyndicateRank.FOLLOWER:
                            case SyndicateRank.ARSENAL_FOLLOWER:
                            case SyndicateRank.CP_FOLLOWER:
                            case SyndicateRank.GUIDE_FOLLOWER:
                            case SyndicateRank.LILY_FOLLOWER:
                            case SyndicateRank.ORCHID_FOLLOWER:
                            case SyndicateRank.PK_FOLLOWER:
                            case SyndicateRank.SILVER_FOLLOWER:
                            case SyndicateRank.TULIP_FOLLOWER:
                                return 1;
                            default:
                                return 0;
                        }
                    }
                #endregion

                default:
                    return 0;
            }
        }

        #endregion

        #region Contribution Rank

        public uint MinimumDonation(SyndicateRank pos)
        {
            try
            {
                PropertyInfo pInfo = null;
                int nMaxCount = 0;
                switch (pos)
                {
                    case SyndicateRank.MANAGER:
                    {
                        pInfo = typeof (SyndicateMember).GetProperty("TotalDonation");
                        nMaxCount = MaxManager;
                        break;
                    }
                    case SyndicateRank.SUPERVISOR:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("TotalDonation");
                        nMaxCount = MaxSupervisor;
                        break;
                    }
                    case SyndicateRank.ARSENAL_SUPERVISOR:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("ArsenalDonation");
                        nMaxCount = MaxSupervisor;
                        break;
                    }
                    case SyndicateRank.CP_SUPERVISOR:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("EmoneyDonation");
                        nMaxCount = MaxSupervisor;
                        break;
                    }
                    case SyndicateRank.SILVER_SUPERVISOR:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("SilverDonation");
                        nMaxCount = MaxSupervisor;
                        break;
                    }
                    case SyndicateRank.GUIDE_SUPERVISOR:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("GuideDonation");
                        nMaxCount = MaxSupervisor;
                        break;
                    }
                    case SyndicateRank.PK_SUPERVISOR:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("PkDonation");
                        nMaxCount = MaxSupervisor;
                        break;
                    }
                    case SyndicateRank.ROSE_SUPERVISOR:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("RedRoseDonation");
                        nMaxCount = MaxSupervisor;
                        break;
                    }
                    case SyndicateRank.LILY_SUPERVISOR:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("WhiteRoseDonation");
                        nMaxCount = MaxSupervisor;
                        break;
                    }
                    case SyndicateRank.ORCHID_SUPERVISOR:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("OrchidDonation");
                        nMaxCount = MaxSupervisor;
                        break;
                    }
                    case SyndicateRank.TULIP_SUPERVISOR:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("TulipDonation");
                        nMaxCount = MaxSupervisor;
                        break;
                    }
                    case SyndicateRank.STEWARD:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("TotalDonation");
                        nMaxCount = MaxSteward;
                        break;
                    }
                    case SyndicateRank.AGENT:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("TotalDonation");
                        break;
                    }
                    case SyndicateRank.ARSENAL_AGENT:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("ArsenalDonation");
                        break;
                    }
                    case SyndicateRank.CP_AGENT:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("EmoneyDonation");
                        break;
                    }
                    case SyndicateRank.SILVER_AGENT:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("SilverDonation");
                        break;
                    }
                    case SyndicateRank.GUIDE_AGENT:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("GuideDonation");
                        break;
                    }
                    case SyndicateRank.PK_AGENT:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("PkDonation");
                        break;
                    }
                    case SyndicateRank.ROSE_AGENT:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("RedRoseDonation");
                        nMaxCount = 1;
                        break;
                    }
                    case SyndicateRank.LILY_AGENT:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("WhiteRoseDonation");
                        nMaxCount = 1;
                        break;
                    }
                    case SyndicateRank.ORCHID_AGENT:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("OrchidDonation");
                        nMaxCount = 1;
                        break;
                    }
                    case SyndicateRank.TULIP_AGENT:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("TulipDonation");
                        nMaxCount = 1;
                        break;
                    }
                    case SyndicateRank.FOLLOWER:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("TotalDonation");
                        break;
                    }
                    case SyndicateRank.ARSENAL_FOLLOWER:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("ArsenalDonation");
                        break;
                    }
                    case SyndicateRank.CP_FOLLOWER:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("EmoneyDonation");
                        break;
                    }
                    case SyndicateRank.SILVER_FOLLOWER:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("SilverDonation");
                        break;
                    }
                    case SyndicateRank.GUIDE_FOLLOWER:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("GuideDonation");
                        break;
                    }
                    case SyndicateRank.PK_FOLLOWER:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("PkDonation");
                        break;
                    }
                    case SyndicateRank.ROSE_FOLLOWER:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("RedRoseDonation");
                        nMaxCount = 1;
                        break;
                    }
                    case SyndicateRank.LILY_FOLLOWER:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("WhiteRoseDonation");
                        nMaxCount = 1;
                        break;
                    }
                    case SyndicateRank.ORCHID_FOLLOWER:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("OrchidDonation");
                        nMaxCount = 1;
                        break;
                    }
                    case SyndicateRank.TULIP_FOLLOWER:
                    {
                        pInfo = typeof(SyndicateMember).GetProperty("TulipDonation");
                        nMaxCount = 1;
                        break;
                    }
                    default:
                        return 0;
                }

                if (pInfo != null && pos != SyndicateRank.DEPUTY_STEWARD)
                {
                    int nCount = Members.Values.Count(x => x.Position == pos);
                    return nMaxCount <= 0 || nMaxCount > nCount
                        ? 0
                        //: Members.Values.Where(x => x.Position == pos).OrderBy(x => pInfo).FirstOrDefault().TotalDonation;
                        : uint.Parse(pInfo.GetValue(Members.Values.OrderBy(x => pInfo).FirstOrDefault()).ToString());
                }

                return (uint) (pos == SyndicateRank.SENIOR_MEMBER ? 25000 : pos == SyndicateRank.DEPUTY_STEWARD ? 175000 : 0);
            }
            catch (Exception ex)
            {
                // of course will not throw shit
                // i hope
                Console.WriteLine("The unexpected happened! Pos:{0}", pos);
            }
            return 0;
        }

        #endregion

        #region Totem Pole
        public bool HeadgearTotem
        {
            get { return m_dbSyn.TotemHead != 0; }
            set
            {
                m_dbSyn.TotemHead = (byte) (value ? 1 : 0);
                Save();
            }
        }

        public bool NecklaceTotem
        {
            get { return m_dbSyn.TotemNeck != 0; }
            set
            {
                m_dbSyn.TotemNeck = (byte)(value ? 1 : 0);
                Save();
            }
        }

        public bool RingTotem
        {
            get { return m_dbSyn.TotemRing != 0; }
            set
            {
                m_dbSyn.TotemRing = (byte)(value ? 1 : 0);
                Save();
            }
        }

        public bool WeaponTotem
        {
            get { return m_dbSyn.TotemWeapon != 0; }
            set
            {
                m_dbSyn.TotemWeapon = (byte)(value ? 1 : 0);
                Save();
            }
        }

        public bool ArmorTotem
        {
            get { return m_dbSyn.TotemArmor != 0; }
            set
            {
                m_dbSyn.TotemArmor = (byte)(value ? 1 : 0); 
                Save();
            }
        }

        public bool BootsTotem
        {
            get { return m_dbSyn.TotemBoots != 0; }
            set
            {
                m_dbSyn.TotemBoots = (byte)(value ? 1 : 0);
                Save();
            }
        }

        public bool HeavenFanTotem
        {
            get { return m_dbSyn.TotemFan != 0; }
            set
            {
                m_dbSyn.TotemFan = (byte)(value ? 1 : 0);
                Save();
            }
        }

        public bool StarTowerTotem
        {
            get { return m_dbSyn.TotemTower != 0; }
            set
            {
                m_dbSyn.TotemTower = (byte)(value ? 1 : 0);
                Save();
            }
        }

        public uint LastTotemOpen
        {
            get { return m_dbSyn.LastTotem; }
            set
            {
                m_dbSyn.LastTotem = value;
                Save();
            }
        }

        public bool CanOpenTotem
        {
            get { return m_dbSyn.LastTotem < uint.Parse(DateTime.Now.ToString("yyyyMMdd")); }
        }
        #endregion

        #region Database
        public bool Save()
        {
            return m_dbSyn != null && Database.SyndicateRepository.SaveOrUpdate(m_dbSyn);
        }

        public bool Delete()
        {
            m_dbSyn.DelFlag = 1;
            return m_dbSyn != null && Database.SyndicateRepository.SaveOrUpdate(m_dbSyn);
        }
        #endregion
    }
}