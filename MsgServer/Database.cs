// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Database.cs
// Last Edit: 2016/12/29 12:54
// Created: 2016/11/29 15:50

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using DB.Entities;
using DB.Repositories;
using FluentNHibernate.Cfg;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Events;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using MsgServer.Structures.Society;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;

namespace MsgServer
{
    public static class Database
    {
        // Global-Scope Constant Declarations:
        public const string DATABASE_LOCATION = "\\";
        public const string DMAPS_LOCATION = DATABASE_LOCATION + "DMaps\\";
        public const string MAPS_LOCATION = DATABASE_LOCATION + "Maps\\";

        public static CharacterRepository Characters { get; set; }
        public static ItemRepository Items { get; set; }
        public static ItemAdditionRepository ItemAddition { get; set; }
        public static MapRepository Maps { get; set; }
        public static DynamicMapRepository DynamicMapRepository { get; set; }
        public static PointAllotRepository PointAllot { get; set; }
        public static NobilityRepository Nobility { get; set; }
        public static LevelExperience Levexp { get; set; }
        public static MapRegion Region { get; set; }
        public static NpcRepository NpcRepository { get; set; }
        public static DynamicNpcRepository DynamicNpcRepository { get; set; }
        public static SyndicateRepository SyndicateRepository { get; set; }
        public static SyndicateMembersRepository SyndicateMembersRepository { get; set; }
        public static SyndicateAlliesRepository SyndicateAlliesRepository { get; set; }
        public static SyndicateEnemiesRepository SyndicateEnemiesRepository { get; set; }
        public static WeaponSkillRepository WeaponSkill { get; set; }
        public static CqSubclassRepository SubclassRepository { get; set; }
        public static TotemPoleRepository TotemPoleRepository { get; set; }
        public static FamilyRepository FamilyRepository { get; set; }
        public static FamilyMemberRepository FamilyMemberRepository { get; set; }
        public static MagicRepository Magics { get; set; }
        public static MagicTypeRepository MagicTypeRepository { get; set; }
        public static GameActionRepo ActionRepository { get; set; }
        public static TasksRespository TasksRepository { get; set; }
        public static FriendRepository Friends { get; set; }
        public static EnemyRepository Enemies { get; set; }
        public static SyndicateAlliesRepository SyndicateAllies { get; set; }
        public static SyndicateEnemiesRepository SyndicateEnemies { get; set; }
        public static MentorAccessRepository MentorAccess { get; set; }
        public static MentorContributionRepository MentorContribution { get; set; }
        public static StatusRepository Status { get; set; }
        public static ArenaRepository ArenaRepository { get; set; }
        public static DetainedItemRepository DetainedItems { get; set; }
        public static PkBonusRepository DetainedReward { get; set; }
        public static CarryRepository CarryRepository { get; set; }

        public static void Initialize()
        {
            try
            {
                ServerKernel.MySqlFactory = new SessionFactory("Shell.ini", "Login.cfg", true);

                Characters = new CharacterRepository();
                ItemAddition = new ItemAdditionRepository();
                Maps = new MapRepository();
                NpcRepository = new NpcRepository();
                DynamicNpcRepository = new DynamicNpcRepository();
                DynamicMapRepository = new DynamicMapRepository();
                Items = new ItemRepository();
                Nobility = new NobilityRepository();
                PointAllot = new PointAllotRepository();
                SyndicateAlliesRepository = new SyndicateAlliesRepository();
                SyndicateEnemiesRepository = new SyndicateEnemiesRepository();
                SyndicateMembersRepository = new SyndicateMembersRepository();
                SyndicateRepository = new SyndicateRepository();
                Levexp = new LevelExperience();
                WeaponSkill = new WeaponSkillRepository();
                SubclassRepository = new CqSubclassRepository();
                TotemPoleRepository = new TotemPoleRepository();
                FamilyRepository = new FamilyRepository();
                FamilyMemberRepository = new FamilyMemberRepository();
                Magics = new MagicRepository();
                MagicTypeRepository = new MagicTypeRepository();
                ActionRepository = new GameActionRepo();
                TasksRepository = new TasksRespository();
                Friends = new FriendRepository();
                Enemies = new EnemyRepository();
                SyndicateEnemies = new SyndicateEnemiesRepository();
                SyndicateAllies = new SyndicateAlliesRepository();
                MentorContribution = new MentorContributionRepository();
                MentorAccess = new MentorAccessRepository();
                Status = new StatusRepository();
                ArenaRepository = new ArenaRepository();
                DetainedItems = new DetainedItemRepository();
                DetainedReward = new PkBonusRepository();
                CarryRepository = new CarryRepository();

                #region Name Change Load

                NameChangeLogRepo nRepo = new NameChangeLogRepo();
                var allChanges = nRepo.FetchAll();

                if (allChanges != null)
                {
                    foreach (var change in allChanges)
                    {
                        if (change.Changed == 0)
                        {
                            Characters.ChangeName(change.UserIdentity, change.NewName, change.OldName);
                            ServerKernel.Log.SaveLog(
                                string.Format("{0} has its name changed to {1}.", change.OldName, change.NewName), true,
                                LogType.DEBUG);
                            change.Changed = 1;
                            nRepo.SaveOrUpdate(change);
                        }
                    }
                }

                #endregion

                #region Load Map Data

                ServerKernel.Log.SaveLog("Decoding map files...", true, LogType.MESSAGE);
                DecodeGamemapFile();
                ServerKernel.Log.SaveLog("Filling server with map information...", true, LogType.MESSAGE);
                ICollection<DbMap> collection = Maps.FetchAll();
                ICollection<DbDynamicMap> dynaCollection = DynamicMapRepository.FetchAll();
                if (collection != null)
                {
                    // Load the maps since the directory seems to be there:
                    bool problem = false;
                    Parallel.ForEach(collection, (databaseMap) =>
                    {
                        // Was a problem detected?
                        if (!problem)
                        {
                            // Get the map:
                            var map = new Map(databaseMap);
                            map.RebornMapId = databaseMap.RebornMap ?? 1002;
                            map.MapRebornPoint = new Point((int)(databaseMap.PortalX ?? 430), (int)(databaseMap.PortalY ?? 380));

                            ServerKernel.Maps.TryAdd(map.Identity, map);

                            ServerKernel.Log.SaveLog(string.Format("Map [{0}] Loaded...", databaseMap.Identity), false);

                            // If the file does not exist, convert the dmap file into a compressed conquer map file:
                            if (!File.Exists(Environment.CurrentDirectory + Database.MAPS_LOCATION + map.Path)
                                && !map.Load()) problem = true;
                        }
                    });

                    if (problem)
                        return;

                    // Loading dynamic collection at startup
                    if (dynaCollection != null)
                    {
                        // Load the maps since the directory seems to be there:
                        Parallel.ForEach(dynaCollection, (databaseMap) =>
                        {
                            // Was a problem detected?
                            if (!problem)
                            {
                                // Get the map:
                                var map = new Map(databaseMap);
                                ServerKernel.Maps.TryAdd(map.Identity, map);

                                ServerKernel.Log.SaveLog(string.Format("Map [{0}] Loaded...", databaseMap.Identity), false);

                                // If the file does not exist, convert the dmap file into a compressed conquer map file:
                                if (!File.Exists(Environment.CurrentDirectory + MAPS_LOCATION + map.Path)
                                    && !map.Load()) problem = true;
                            }
                        });

                        if (problem) // Display an error message if there was a problem converting maps:
                            Console.WriteLine("Please ensure that you have placed the client's map folder contents,\n"
                                              + "specifically the map and scene folders, in the map server's \"DMaps\" folder, \n"
                                              + "located in the \"Database\" folder. Then restart the server and try again.");
                    }
                }

                var lreg = new RegionRepository().FetchAll();
                if (lreg != null)
                {
                    foreach (var reg in lreg)
                    {
                        if (ServerKernel.Maps.ContainsKey(reg.MapIdentity))
                            ServerKernel.Maps[reg.MapIdentity].Regions.TryAdd(reg.Identity, reg);
                    }
                }

                #endregion

                #region Loading Portals Data

                ServerKernel.Log.SaveLog("Loading portals data...", true);

                var passways = new PasswayRepository().GetAllPassways();
                if (passways != null)
                {
                    foreach (var passway in passways)
                    {
                        var portal = new PortalsRepository().GetByIndex(passway.TargetMapId, passway.TargetPortal);
                        if (portal != null)
                        {
                            IPassway pw = new IPassway
                            {
                                Identity = passway.Identity,
                                PasswayIndex = passway.MapIndex,
                                PasswayMap = passway.MapId,
                                PortalMap = portal.MapId,
                                PortalX = portal.PortalX,
                                PortaLy = portal.PortalY
                            };

                            Map map;
                            if (ServerKernel.Maps.TryGetValue(pw.PasswayMap, out map))
                            {
                                if (!map.Portals.TryAdd(pw.Identity, pw))
                                    ServerKernel.Log.SaveLog(string.Format("Could not add portal ({0}) to map ({1}). Index already exists", pw.Identity, map.Identity), false, LogType.WARNING);
                            }
                            else
                            {
                                ServerKernel.Log.SaveLog(string.Format("Map ({0}) doesnt exist for portal ({1})", pw.PasswayMap, pw.Identity), false, LogType.WARNING);
                            }
                        }
                        else
                        {
                            ServerKernel.Log.SaveLog(string.Format("Could not find portal for [{0}][{1}]", passway.MapId,
                                passway.MapIndex), false, LogType.WARNING);
                        }
                    }
                }

                #endregion

                #region Load Nobility

                ServerKernel.Log.SaveLog("Fetching nobility data...", true);

                var nobList = Nobility.FetchAll();

                if (nobList != null)
                {
                    foreach (DbDynaRankRec nob in nobList)
                    {
                        ServerKernel.Nobility.TryAdd(nob.UserIdentity, nob);
                    }
                }

                #endregion

                #region Load Item

                ServerKernel.Log.SaveLog("Loading item information...", true);

                var itemAdd = new ItemAdditionRepository().FetchAll();

                if (itemAdd != null)
                {
                    foreach (var itAdd in itemAdd)
                    {
                        ServerKernel.ItemAddition.Add(itAdd.Id, itAdd);
                    }
                }

                var itemType = new ItemtypeRepository().FetchAll();

                if (itemType != null)
                {
                    foreach (var itType in itemType)
                    {
                        ServerKernel.Itemtype.Add(itType.Type, itType);
                    }
                }

                var goods = new GoodsRepository().FetchAll();

                if (goods != null)
                {
                    foreach (var item in goods)
                    {
                        ServerKernel.Goods.Add(item.Identity, item);
                    }
                }

                ICollection<DbRefinery> allRefineries = new CqRefineryRepository().LoadAllRefineries();
                if (allRefineries != null)
                {
                    foreach (var refinery in allRefineries)
                    {
                        if (!ServerKernel.Refineries.ContainsKey(refinery.Id))
                            ServerKernel.Refineries.Add(refinery.Id, refinery);
                    }
                }

                var allItems = new ItemRepository().FetchAll();
                if (allItems != null)
                {
                    foreach (var item in allItems.Where(x => x.Inscribed > 0))
                    {
                        item.Inscribed = 0;
                        Items.SaveOrUpdate(item);
                    }
                }

                var allDetainedItems = DetainedItems.FetchAll();
                if (allDetainedItems != null)
                {
                    foreach (var item in allDetainedItems)
                    {
                        DetainedObject newObj = new DetainedObject(true);
                        if (!newObj.Create(item))
                        {
                            ServerKernel.Log.GmLog("detained_error", string.Format("ERROr loading item {0}", item.Identity));
                            continue;
                        }
                        ServerKernel.DetainedObjects.TryAdd(item.Identity, newObj);
                    }
                }

                var detainRewards = new PkBonusRepository().FetchAll();
                if (detainRewards != null)
                {
                    foreach (var item in detainRewards)
                    {
                        DetainedObject newObj = new DetainedObject(false);
                        if (!newObj.Create(item))
                        {
                            ServerKernel.Log.GmLog("detained_error", string.Format("ERROr loading item {0}", item.Identity));
                            continue;
                        }
                        ServerKernel.DetainedObjects.TryAdd(item.Identity, newObj);
                    }
                }

                #endregion

                #region Rebirth

                var rebirth = new RebirthRepository().FetchAll();
                if (rebirth != null)
                {
                    foreach (var rb in rebirth)
                        ServerKernel.Rebirths.Add(rb.Identity, rb);
                }

                #endregion

                #region Load Skills

                var mgcType = new MagicTypeRepository().FetchAll();
                if (mgcType != null)
                {
                    foreach (var mgc in mgcType)
                    {
                        ServerKernel.Magictype.Add(mgc.Id, mgc);
                    }
                }

                var mstMgc = new MonsterMagicRepository().FetchAll();
                if (mstMgc != null)
                {
                    foreach (var mgc in mstMgc)
                    {
                        ServerKernel.MonsterMagics.Add(mgc);
                    }
                }

                var mgcOp = new MagictypeOpRepository().FetchAll();
                if (mgcOp != null)
                {
                    foreach (var op in mgcOp)
                    {
                        MagicTypeOp pMgc = new MagicTypeOp(op);
                        ServerKernel.Magictypeops.TryAdd(op.Id, pMgc);
                    }
                }

                #endregion
                
                #region Guide Config
                var tutorType = new MentorTypeRepository().FetchAll();

                if (tutorType != null)
                {
                    foreach (var tt in tutorType)
                        ServerKernel.MentorTypes.Add(tt);
                }

                var tutorBl = new MentorBattleLimitRepository().FetchAll();

                if (tutorBl != null)
                {
                    foreach (var tt in tutorBl)
                        ServerKernel.MentorBattleLimits.Add(tt);
                }

                #endregion

                #region Load Levexp

                ServerKernel.Log.SaveLog("Loading level experience information...", true);

                var leveXp = Levexp.FetchAll();

                if (leveXp != null)
                {
                    ServerKernel.MAX_UPLEVEL = (byte)(leveXp.Count + 1);

                    foreach (DbLevexp lev in leveXp)
                    {
                        ServerKernel.Levelxp.Add(lev.Level, lev);
                    }
                }

                #endregion

                #region Load Superman

                var kotable = new KoBoardRepository().FetchAll();
                if (kotable != null)
                {
                    foreach (var ko in kotable)
                    {
                        IKoCount koc = new IKoCount(ko);
                        if (!ServerKernel.KoBoard.TryAdd(ko.Identity, koc))
                            ServerKernel.Log.SaveLog(string.Format("kocount(id:{0}) loading error", koc.Name));
                    }
                }

                #endregion

                #region Load Point Allot Data

                ServerKernel.Log.SaveLog("Loading auto allot information...", true, LogType.MESSAGE);

                ICollection<DbPointAllot> pointAllot = PointAllot.FetchAll();

                if (pointAllot != null)
                {
                    foreach (var pa in pointAllot)
                    {
                        ServerKernel.PointAllot.Add(pa.Identity, pa);
                    }
                }

                #endregion

                #region Loading Syndicates

                ICollection<DbSyndicate> allSyndicate = new SyndicateRepository().FetchAll();
                if (allSyndicate != null)
                {
                    foreach (var cqSyndicate in allSyndicate)
                    {
                        var syn = new Syndicate();
                        if (!syn.Create(cqSyndicate))
                            continue;
                        if (!ServerKernel.Syndicates.TryAdd(syn.Identity, syn))
                            ServerKernel.Log.SaveLog(
                                string.Format("Could not load syndicate [{0}]", syn.Identity));

                        ICollection<DbCqSynattr> allMembers = new SyndicateMembersRepository().FetchBySyndicate(syn.Identity);
                        foreach (var member in allMembers)
                        {
                            var user = Characters.SearchByIdentity(member.Id);
                            if (user == null)
                                continue;

                            var nMember = new SyndicateMember(syn);
                            if (!nMember.Create(member, user))
                                continue;

                            nMember.Name = user.Name;
                            nMember.Profession = user.Profession;
                            nMember.Level = user.Level;
                            nMember.ArsenalDonation = 0;

                            switch (nMember.Position)
                            {
                                case SyndicateRank.DEPUTY_LEADER:
                                    syn.DeputyLeaderCount += 1;
                                    break;
                                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                                    syn.HonoraryDeputyLeaderCount += 1;
                                    break;
                                case SyndicateRank.HONORARY_MANAGER:
                                    syn.HonoraryManagerCount += 1;
                                    break;
                                case SyndicateRank.HONORARY_SUPERVISOR:
                                    syn.HonorarySupervisorCount += 1;
                                    break;
                                case SyndicateRank.HONORARY_STEWARD:
                                    syn.HonoraryStewardCount += 1;
                                    break;
                                case SyndicateRank.AIDE:
                                    syn.AideCount += 1;
                                    break;
                                case SyndicateRank.LEADER_SPOUSE:
                                    syn.LeaderSpouseAideCount += 1;
                                    break;
                                case SyndicateRank.DEPUTY_LEADER_AIDE:
                                    syn.DeputyLeaderAideCount += 1;
                                    break;
                                case SyndicateRank.MANAGER_AIDE:
                                    syn.ManagerAideCount += 1;
                                    break;
                                case SyndicateRank.SUPERVISOR_AIDE:
                                    syn.SupervisorAideCount += 1;
                                    break;
                            }

                            if (!ServerKernel.Syndicates[syn.Identity].Members.ContainsKey(nMember.Identity))
                                if (!ServerKernel.Syndicates[syn.Identity].Members.TryAdd(nMember.Identity, nMember))
                                    ServerKernel.Log.SaveLog( string.Format("Could not load member [{0}] to syn [{1}]", nMember.Identity, nMember.SyndicateIdentity));
                        }

                        syn.MemberCount = (ushort)syn.Members.Count;
                        syn.Save();

                        // load arsenals
                        if (syn.Arsenal == null)
                            syn.Arsenal = new Arsenal(syn);

                        // open totem pole (arsenal holders)
                        if (syn.HeadgearTotem)
                            syn.Arsenal.Poles.TryAdd(TotemPoleType.TOTEM_HEADGEAR,
                                new TotemPole(TotemPoleType.TOTEM_HEADGEAR) { Locked = false });

                        if (syn.NecklaceTotem)
                            syn.Arsenal.Poles.TryAdd(TotemPoleType.TOTEM_NECKLACE,
                                new TotemPole(TotemPoleType.TOTEM_NECKLACE) { Locked = false });

                        if (syn.RingTotem)
                            syn.Arsenal.Poles.TryAdd(TotemPoleType.TOTEM_RING,
                                new TotemPole(TotemPoleType.TOTEM_RING) { Locked = false });

                        if (syn.WeaponTotem)
                            syn.Arsenal.Poles.TryAdd(TotemPoleType.TOTEM_WEAPON,
                                new TotemPole(TotemPoleType.TOTEM_WEAPON) { Locked = false });

                        if (syn.ArmorTotem)
                            syn.Arsenal.Poles.TryAdd(TotemPoleType.TOTEM_ARMOR,
                                new TotemPole(TotemPoleType.TOTEM_ARMOR) { Locked = false });

                        if (syn.BootsTotem)
                            syn.Arsenal.Poles.TryAdd(TotemPoleType.TOTEM_BOOTS,
                                new TotemPole(TotemPoleType.TOTEM_BOOTS) { Locked = false });

                        if (syn.HeavenFanTotem)
                            syn.Arsenal.Poles.TryAdd(TotemPoleType.TOTEM_FAN,
                                new TotemPole(TotemPoleType.TOTEM_FAN) { Locked = false });

                        if (syn.StarTowerTotem)
                            syn.Arsenal.Poles.TryAdd(TotemPoleType.TOTEM_TOWER,
                                new TotemPole(TotemPoleType.TOTEM_TOWER) { Locked = false });

                        ICollection<DbSyntotem> allTotem = new CqSyntotemRepository().GetBySyndicate(syn.Identity);
                        foreach (var totem in allTotem)
                        {
                            DbItem cqItem = Items.FetchByIdentity(totem.Itemid);
                            if (cqItem == null)
                                continue;

                            var item = new Item(null, cqItem);
                            var tClient = new Totem(totem, item);

                            SyndicateMember pMember;
                            if (syn.Members.TryGetValue(totem.Userid, out pMember))
                                pMember.ArsenalDonation += tClient.Donation();

                            syn.Arsenal.AddItem(item, tClient);
                        }

                        syn.Arsenal.UpdatePoles();

                        syn.Level = 1;

                        foreach (var pole in syn.Arsenal.Poles.Values)
                            if (pole.Donation >= 5000000)
                                syn.Level += 1;

                        // load member positions
                        var memberList = new List<SyndicateMember>(810);
                        foreach (var member in syn.Members.Values.Where(x => !x.IsUserSetPosition()))
                        {
                            member.Position = SyndicateRank.MEMBER;
                            memberList.Add(member);
                        }

                        uint amount = 0;
                        uint maxamount = syn.MaxPositionAmount(SyndicateRank.MANAGER);

                        List<SyndicateMember> remove = new List<SyndicateMember>();
                        #region Manager
                        foreach (var member in memberList.OrderByDescending(x => x.TotalDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.MANAGER;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion

                        #region Rose Supervisor

                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.ROSE_SUPERVISOR);

                        foreach (var member in memberList.OrderByDescending(c => c.RedRoseDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.ROSE_SUPERVISOR;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region White Rose Supervisor

                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.LILY_SUPERVISOR);

                        foreach (var member in memberList.OrderByDescending(x => x.WhiteRoseDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.LILY_SUPERVISOR;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Orchid Supervisor
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.ORCHID_SUPERVISOR);

                        foreach (var member in memberList.OrderByDescending(x => x.OrchidDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.ORCHID_SUPERVISOR;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Tulip Supervisor
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.TULIP_SUPERVISOR);

                        foreach (var member in memberList.OrderByDescending(x => x.TulipDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.TULIP_SUPERVISOR;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Pk Supervisor
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.PK_SUPERVISOR);

                        foreach (var member in memberList.OrderByDescending(x => x.PkDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.PK_SUPERVISOR;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Guide Supervisor
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.GUIDE_SUPERVISOR);

                        foreach (var member in memberList.OrderByDescending(x => x.GuideDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.GUIDE_SUPERVISOR;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Silver Supervisor
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.SILVER_SUPERVISOR);

                        foreach (var member in memberList.OrderByDescending(x => x.SilverDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.SILVER_SUPERVISOR;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region CPs Supervisor
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.CP_SUPERVISOR);

                        foreach (var member in memberList.OrderByDescending(x => x.EmoneyDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.CP_SUPERVISOR;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Arsenal Supervisor
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.ARSENAL_SUPERVISOR);

                        foreach (var member in memberList.OrderByDescending(x => x.ArsenalDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.ARSENAL_SUPERVISOR;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Supervisor
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.SUPERVISOR);

                        foreach (var member in memberList.OrderByDescending(x => x.TotalDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.SUPERVISOR;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion

                        #region Steward
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.STEWARD);

                        foreach (var member in memberList.OrderByDescending(x => x.TotalDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.STEWARD;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Deputy Steward

                        foreach (var member in memberList.Where(x => x.TotalDonation >= 170000))
                        {
                            member.Position = SyndicateRank.DEPUTY_STEWARD;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();

                        #endregion

                        #region Rose Agent

                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.ROSE_AGENT);

                        foreach (var member in memberList.OrderByDescending(x => x.RedRoseDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.ROSE_AGENT;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region White Rose Agent

                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.LILY_AGENT);

                        foreach (var member in memberList.OrderByDescending(x => x.WhiteRoseDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.LILY_AGENT;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Orchid Agent
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.ORCHID_AGENT);

                        foreach (var member in memberList.OrderByDescending(x => x.OrchidDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.ORCHID_AGENT;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Tulip Agent
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.TULIP_AGENT);

                        foreach (var member in memberList.OrderByDescending(x => x.TulipDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.TULIP_AGENT;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Pk Agent
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.PK_AGENT);

                        foreach (var member in memberList.OrderByDescending(x => x.PkDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.PK_AGENT;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Guide Agent
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.GUIDE_AGENT);

                        foreach (var member in memberList.OrderByDescending(x => x.GuideDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.GUIDE_AGENT;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Silver Agent
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.SILVER_AGENT);

                        foreach (var member in memberList.OrderByDescending(x => x.SilverDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.SILVER_AGENT;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region CPs Agent
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.CP_AGENT);

                        foreach (var member in memberList.OrderByDescending(x => x.EmoneyDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.CP_AGENT;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Arsenal Agent
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.ARSENAL_AGENT);

                        foreach (var member in memberList.OrderByDescending(x => x.ArsenalDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.ARSENAL_AGENT;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Agent
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.AGENT);

                        foreach (var member in memberList.OrderByDescending(x => x.TotalDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.AGENT;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion

                        #region Rose Follower

                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.ROSE_FOLLOWER);

                        foreach (var member in memberList.OrderByDescending(x => x.RedRoseDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.ROSE_FOLLOWER;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region White Rose Follower

                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.LILY_FOLLOWER);

                        foreach (var member in memberList.OrderByDescending(x => x.WhiteRoseDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.LILY_FOLLOWER;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }

                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Orchid Follower
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.ORCHID_FOLLOWER);

                        foreach (var member in memberList.OrderByDescending(x => x.OrchidDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.ORCHID_FOLLOWER;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Tulip Follower
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.TULIP_FOLLOWER);

                        foreach (var member in memberList.OrderByDescending(x => x.TulipDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.TULIP_FOLLOWER;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Pk Follower
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.PK_FOLLOWER);

                        foreach (var member in memberList.OrderByDescending(x => x.PkDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.PK_FOLLOWER;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Guide Follower
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.GUIDE_FOLLOWER);

                        foreach (var member in memberList.OrderByDescending(x => x.GuideDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.GUIDE_FOLLOWER;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Silver Follower
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.SILVER_FOLLOWER);

                        foreach (var member in memberList.OrderByDescending(x => x.SilverDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.SILVER_FOLLOWER;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region CPs Follower
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.CP_FOLLOWER);

                        foreach (var member in memberList.OrderByDescending(x => x.EmoneyDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.CP_FOLLOWER;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Arsenal Follower
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.ARSENAL_FOLLOWER);

                        foreach (var member in memberList.OrderByDescending(x => x.ArsenalDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.ARSENAL_FOLLOWER;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                        #region Follower
                        amount = 0;
                        maxamount = syn.MaxPositionAmount(SyndicateRank.FOLLOWER);

                        foreach (var member in memberList.OrderByDescending(x => x.TotalDonation))
                        {
                            if (amount >= maxamount)
                                break;

                            member.Position = SyndicateRank.FOLLOWER;
                            member.Save();
                            amount++;
                            remove.Add(member);
                        }
                        foreach (var rem in remove)
                            memberList.Remove(rem);
                        remove.Clear();
                        #endregion
                    }

                    // all the guilds loaded, we get the allies and enemies
                    foreach (var syn in ServerKernel.Syndicates.Values.Where(x => !x.Deleted))
                    {
                        ICollection<DbSynAlly> allies = new SyndicateAlliesRepository().FetchBySyndicate(syn.Identity);
                        ICollection<DbCqSynEnemy> enemies = new SyndicateEnemiesRepository().FetchBySyndicate(syn.Identity);

                        foreach (var ally in allies)
                        {
                            var sSyn = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Identity == ally.Allyid && !x.Deleted);
                            if (sSyn == null) continue;

                            if (!sSyn.Allies.TryAdd(syn.Identity, syn) || !syn.Allies.TryAdd(sSyn.Identity, sSyn))
                                ServerKernel.Log.SaveLog(
                                    string.Format("WARNING: Could not find ally [{0}][{1}] for guild [{2}][{3}]",
                                        ally.Allyid, ally.Allyname, syn.Identity, syn.Name), false);
                        }

                        foreach (var enemy in enemies)
                        {
                            var sSyn = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Identity == enemy.Enemyid && !x.Deleted);
                            if (sSyn == null) continue;

                            if (!syn.Enemies.TryAdd(sSyn.Identity, sSyn))
                                ServerKernel.Log.SaveLog(
                                    string.Format("WARNING: Could not find enemy [{0}][{1}] for guild [{2}][{3}]",
                                        enemy.Enemyid, enemy.Enemyname, syn.Identity, syn.Name), false);
                        }
                    }
                }

                #endregion

                #region Loading Families

                ICollection<DbFamily> allFamily = FamilyRepository.FetchAll();
                if (allFamily != null)
                {
                    foreach (var dbFamily in allFamily)
                    {
                        Family pFamily = new Family();
                        if (!pFamily.Create(dbFamily))
                            continue;

                        ICollection<DbFamilyMember> allMember = FamilyMemberRepository.FetchByFamily(pFamily.Identity);
                        if (allMember != null)
                        {
                            foreach (var member in allMember)
                            {
                                DbUser pUser = Characters.SearchByIdentity(member.Identity);
                                if (pUser == null)
                                {
                                    FamilyMemberRepository.Delete(member);
                                    continue;
                                }

                                FamilyMember pMember = new FamilyMember(pFamily);
                                if (!pMember.Create(pUser, member))
                                    continue;

                                if (!pFamily.Members.TryAdd(pMember.Identity, pMember))
                                {
                                    ServerKernel.Log.SaveLog(string.Format("ALERT: could not add member id:{0} to family:{1}",
                                        pMember.Identity, pFamily.Identity));
                                }
                            }
                        }
                        else
                        {
                            pFamily.Delete();
                            ServerKernel.Log.GmLog("family_delete",
                                string.Format("Family[id:{0},name:{1}] has been deleted because no members were found at server loading",
                                pFamily.Identity, pFamily.Name));
                            // should I delete allies and enemies? xDDD nah
                            continue;
                        }
                        ServerKernel.Families.TryAdd(pFamily.Identity, pFamily);
                    }
                }

                foreach (var family in ServerKernel.Families.Values)
                {
                    family.LoadRelations();
                }

                #endregion

                #region Load NPCs

                ServerKernel.Log.SaveLog("Loading NPCs and Dynamic NPCs...", true);

                var dynamic = DynamicNpcRepository.FetchAll();

                if (dynamic != null)
                {
                    foreach (var dyna in dynamic)
                    {
                        Map temp = null;
                        if (ServerKernel.Maps.TryGetValue(dyna.Mapid, out temp))
                            temp.GameObjects.TryAdd(dyna.Id, new DynamicNpc(dyna));
                    }
                }

                var npcs = NpcRepository.FetchAll();

                if (npcs != null)
                {
                    foreach (var npc in npcs)
                    {
                        Map temp = null;
                        if (ServerKernel.Maps.TryGetValue(npc.Mapid, out temp))
                            temp.GameObjects.TryAdd(npc.Id, new GameNpc(npc));
                    }
                }

                #endregion

                #region Action Loading

                ServerKernel.Log.SaveLog("Server is loading game action data...", true);

                var action = ActionRepository.FetchAll();

                foreach (var act in action)
                {
                    try
                    {
                        ServerKernel.GameActions.Add(act.Identity, new ActionStruct
                        {
                            Id = act.Identity,
                            IdNext = act.IdNext,
                            IdNextfail = act.IdNextfail,
                            Data = act.Data,
                            Type = act.Type,
                            Param = act.Param
                        });
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog(string.Format("Could not add action ({0}) to the dictionary", act.Identity), true, LogType.ERROR);
                    }
                }

                var tasks = TasksRepository.FetchAll();

                foreach (var tsk in tasks)
                {
                    try
                    {
                        if (!ServerKernel.GameActions.ContainsKey(tsk.IdNext))
                            ServerKernel.Log.SaveLog(string.Format("Action({0}) not found to task({1})", tsk.IdNext, tsk.Id));

                        ServerKernel.GameTasks.Add(tsk.Id, new TaskStruct
                        {
                            Id = tsk.Id,
                            IdNext = tsk.IdNext,
                            IdNextfail = tsk.IdNextfail,
                            Itemname1 = tsk.Itemname1,
                            Itemname2 = tsk.Itemname2,
                            ClientActive = tsk.ClientActive,
                            Marriage = tsk.Marriage,
                            MaxPk = tsk.MaxPk,
                            Metempsychosis = tsk.Metempsychosis,
                            MinPk = tsk.MinPk,
                            Profession = tsk.Profession,
                            Query = tsk.Query,
                            Money = tsk.Money,
                            Sex = tsk.Sex,
                            Team = tsk.Team
                        });
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog(string.Format("Could not add task ({0}) to the dictionary", tsk.Id), true, LogType.ERROR);
                    }
                }

                #endregion

                #region Arena

                var allArena = new ArenaRepository().FetchAll();
                if (allArena != null)
                {
                    foreach (var arena in allArena)
                        if (!ServerKernel.ArenaRecord.ContainsKey(arena.PlayerIdentity))
                            ServerKernel.ArenaRecord.TryAdd(arena.PlayerIdentity, new QualifierRankObj(arena));
                }

                IniFileName pReader = new IniFileName(Environment.CurrentDirectory + @"\ini\HonorRewards.ini");
                foreach (var pos in pReader.GetEntryNames("Rewards"))
                {
                    IHonorReward rew = new IHonorReward
                    {
                        Ranking = ushort.Parse(pos.ToString())
                    };
                    string[] split = pReader.GetEntryValue("Rewards", pos).ToString().Split('/');
                    if (split.Length < 2)
                        continue;
                    rew.DailyHonor = uint.Parse(split[0]);
                    rew.WeeklyHonor = uint.Parse(split[1]);

                    ServerKernel.HonorRewards.Add(rew.Ranking, rew);
                }

                StreamReader reader = new StreamReader(Environment.CurrentDirectory + @"\ini\HonorShop.ini");
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] value = line.Split(',');
                    if (value.Length < 2)
                        continue;
                    uint idItem = uint.Parse(value[0]);
                    uint dwPrice = uint.Parse(value[1]);

                    if (ServerKernel.Itemtype.ContainsKey(idItem))
                        ServerKernel.Itemtype[idItem].HonorPrice = dwPrice;
                }

                #endregion

                #region Monsters

                var monsters = new MonstersRepository().FetchAll();
                if (monsters != null)
                {
                    foreach (var mob in monsters)
                        ServerKernel.Monsters.Add(mob.Id, mob);
                }

                IniFileName quenchRule = new IniFileName(Environment.CurrentDirectory + @"\ini\QuenchDropRule.ini");
                foreach (var szMobId in quenchRule.GetSectionNames())
                {
                    uint idMob = 0;
                    if (!uint.TryParse(szMobId, out idMob))
                    {
                        ServerKernel.Log.SaveLog(string.Format("ALERT: QuenchDropRule could not parse mob id: {0}", szMobId), false, LogType.WARNING);
                        continue;
                    }

                    DbMonstertype dbMob;
                    if (!ServerKernel.Monsters.TryGetValue(idMob, out dbMob))
                    {
                        ServerKernel.Log.SaveLog(string.Format("ALERT: unexistent monstertype({0}) for droprule", idMob), false, LogType.WARNING);
                        continue;
                    }

                    string szName = "";
                    byte level = 0;
                    byte tolerance = 0;
                    byte dropNum = 0;
                    uint idDefault = 0;
                    int actionNum = 0;

                    try
                    {
                        szName = quenchRule.GetEntryValue(szMobId, "Name").ToString();
                        level = byte.Parse(quenchRule.GetEntryValue(szMobId, "Level").ToString());
                        tolerance = byte.Parse(quenchRule.GetEntryValue(szMobId, "LevelTolerance").ToString());
                        dropNum = byte.Parse(quenchRule.GetEntryValue(szMobId, "DropNum").ToString());
                        idDefault = uint.Parse(quenchRule.GetEntryValue(szMobId, "DefaultAction").ToString());
                        actionNum = int.Parse(quenchRule.GetEntryValue(szMobId, "Action").ToString());
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog(string.Format("EXCEPTION: could not parse data for drop rule {0}", idMob), false, LogType.EXCEPTION);
                        continue;
                    }

                    SpecialDrop pDrop = new SpecialDrop
                    {
                        MonsterName = szName,
                        MonsterIdentity = idMob,
                        Level = level,
                        LevelTolerance = tolerance,
                        DefaultAction = idDefault,
                        DropNum = dropNum
                    };
                    pDrop.Actions = new List<KeyValuePair<uint, ushort>>();
                    for (int i = 0; i < actionNum; i++)
                    {
                        uint idAction = 0;
                        ushort usChance = 10000;
                        string[] szInfo = quenchRule.GetEntryValue(szMobId, string.Format("Action{0}", i)).ToString().Split(' ');

                        if (szInfo.Length < 2
                            || !uint.TryParse(szInfo[0], out idAction)
                            || !ushort.TryParse(szInfo[1], out usChance))
                        {
                            continue;
                        }

                        pDrop.Actions.Add(new KeyValuePair<uint, ushort>(idAction, usChance));
                    }

                    ServerKernel.SpecialDrop.Add(pDrop);
                }

                #endregion

                #region Last Loading.. Generators!

                var generators = new GeneratorRepository().FetchAll();
                if (generators != null)
                {
                    foreach (var gen in generators)
                    {
                        Generator pGen = new Generator(gen);
                        if (!pGen.Create())
                        {
                            ServerKernel.Log.SaveLog(string.Format("Could not load generator (id:{0})", gen.Id), true, LogType.WARNING);
                            continue;
                        }
                        ServerKernel.Generators.Add(pGen);
                        pGen.FirstGeneration();
                    }
                }

                #endregion

                ServerKernel.FlowerRanking = new FlowerRanking();
                ServerKernel.QuizShow = new QuizShowEvent();
                ServerKernel.Broadcast = new Pigeon();
                if (!ServerKernel.ScorePkEvent.Create(ServerKernel.SCORE_PK_MAPID))
                {
                    ServerKernel.Log.SaveLog("Could not create Score PK Tournament event.", true, LogType.WARNING);
                }
                if (!ServerKernel.CaptureTheFlag.Create())
                {
                    ServerKernel.Log.SaveLog("Could not create capture the flag tournament", true, LogType.ERROR);
                }
                if (!ServerKernel.ArenaQualifier.Create())
                {
                    ServerKernel.Log.SaveLog("Could not create arena qualifier", true, LogType.ERROR);
                }
                if (!ServerKernel.SyndicateScoreWar.Create())
                {
                    ServerKernel.Log.SaveLog("Could not create syndicate score war", true, LogType.ERROR);
                }
                if (!ServerKernel.SyndicateRecruitment.Create())
                {
                    ServerKernel.Log.SaveLog("Could not create Syndicate recruitment", true, LogType.ERROR);
                }
                if (!ServerKernel.LineSkillPk.Create())
                {
                    ServerKernel.Log.SaveLog("Could not create line skill pk", true, LogType.ERROR);
                }
            }
            catch (FluentConfigurationException ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), true, "MySqlError", LogType.ERROR);
                Console.ReadLine();
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), true, "MysqlError", LogType.ERROR);
                Console.ReadLine();
                Environment.Exit(2);
            }
        }

        /// <summary>
        /// This function decodes the binary gamemap.dat file into a plain text file composed of MySQL insert 
        /// commands. These commands can be run by the MySQL service to add new maps to the database for the very
        /// first time. The file can be loaded in the map server's database folder.
        /// </summary>
        public static void DecodeGamemapFile()
        {
            // Initialize file stream:
            FileStream stream = File.OpenRead(Environment.CurrentDirectory + DATABASE_LOCATION + "GameMap.dat");
            var reader = new BinaryReader(stream);
            var values = new Dictionary<int, string>();

            // Read in the file:
            int amount = reader.ReadInt32();
            for (int index = 0; index < amount; index++)
            {
                try
                {
                    int identity = reader.ReadInt32();
                    string path = Encoding.ASCII.GetString(reader.ReadBytes(reader.ReadInt32())).Remove(0, 8);
                    reader.BaseStream.Seek(4L, SeekOrigin.Current); // puzzle
                    values.Add(identity, path.Replace(".DMap", ".cqm"));
                }
                catch (Exception ex)
                {
                    ServerKernel.Log.SaveLog(ex.Message, true);
                }
            }

            // Dispose of file stream:
            stream.Close();
            reader.Close();
            stream.Dispose();
            reader.Dispose();

            // Write the data to a plain text file:
            string plainTextPath = Environment.CurrentDirectory + DATABASE_LOCATION + "GameMap.txt";
            if (File.Exists(plainTextPath)) File.Delete(plainTextPath);
            StreamWriter writer = File.CreateText(plainTextPath);
            foreach (int identity in values.Keys)
                writer.WriteLine("INSERT INTO `cq_map` (`id`, `mapdoc`, `file_name`) VALUES"
                                 + "('" + identity + "', '" + identity + "', '" + values[identity] + "');");
            writer.Close();
            writer.Dispose();
        }
    }
}