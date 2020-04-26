// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Character.cs
// Last Edit: 2017/01/24 21:47
// Created: 2016/12/29 21:31

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Enums;
using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Network.GameServer.Handlers;
using MsgServer.Structures.Actions;
using MsgServer.Structures.Events;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using MsgServer.Structures.Qualifier;
using MsgServer.Structures.Society;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Entities
{
    public sealed class Character : IScreenObject, IRole, IOnTimer
    {
        private TimeOut m_tXp = new TimeOut(3);
        private TimeOut m_tXpStop = new TimeOut(15);
        private TimeOut m_tStamina = new TimeOut(ADD_ENERGY_STAND_SECS);
        private TimeOut m_pkDecrease = new TimeOut(PK_DEC_TIME);
        private TimeOut m_lifeRecover = new TimeOut(AUTOHEALLIFE_TIME);
        private TimeOut m_timePacket = new TimeOut(TICK_SECS);
        private TimeOut m_tRespawn = new TimeOut(CHGMAP_LOCK_SECS);
        private TimeOut m_tSilence = new TimeOut(0);
        private TimeOut m_tWorldChat = new TimeOut(0);
        private TimeOut m_tCool = new TimeOut(5);
        private TimeOutMS m_tJumpUnable = new TimeOutMS(0);
        private TimeOut m_tRevive = new TimeOut(20);
        private TimeOut m_tDoGhost = new TimeOut(3);
        private TimeOut m_tTransformation = new TimeOut(0);
        private TimeOutMS m_tMine = new TimeOutMS(3000);
        private TimeOut m_tOnlineTime = new TimeOut(1800);
        private TimeOut m_tTeamPos = new TimeOut(TIME_TEAMPRC);
        private TimeOut m_tHeavenBlessing = new TimeOut(60);
        private TimeOut m_tLuckyTime = new TimeOut(1);
        private TimeOutMS m_tStatusCheck = new TimeOutMS(500);
        private TimeOutMS m_tDazed = new TimeOutMS(500);
        private TimeOut m_tVigor = new TimeOut(1);
        private TimeOut m_tVipPortalTele = new TimeOut(180);
        private TimeOut m_tVipCityTele = new TimeOut(180);

        private Map m_pMap;
        private FacingDirection m_pDirection = FacingDirection.EAST;
        private Client m_pOwner;
        private DbUser m_dbUser;
        private MsgPlayer m_pPacket;
        private EntityAction m_pAction;
        private StatusSet m_pStatus;
        private Inventory m_pInventory;
        private Equipment m_pEquipment;
        private IScreenObject m_interactingNpc;
        private Item m_taskItem;
        private WeaponSkill m_pWeaponSkill;
        private Nobility m_pNobility;
        private SubClass m_pSubClass;
        private SyndicateMember m_pSynMember;
        private Syndicate m_pSyndicate;
        private Family m_pFamily;
        private FamilyMember m_pFamilyMember;
        private BattleSystem m_pBattleSystem;
        private GameAction m_pGameAction;
        private MagicData m_pMagics;
        private Transformation m_pTransformation;
        private PlayerBooth m_pPlayerBooth;
        private PkModeType m_pPkMode;
        private CaptchaBox m_pCaptchaBox;
        private PkExploit m_pkExploit;
        
        // captcha count
        private uint m_dwCount;
        // names
        private string m_szUserName;
        private string m_szSecondName = "None";
        private string m_szFamilyName = string.Empty;
        // position
        private ushort m_usMapX;
        private ushort m_usMapY;
        private uint m_dwMapIdentity;
        private short m_sElevation;
        // equipment
        private uint m_helmet;
        private ushort m_helmetColor;
        private uint m_armor;
        private ushort m_armorColor;
        private uint m_rightHand;
        private uint m_leftHand;
        private ushort m_shieldColor;
        private uint m_garment;
        // attributes
        private ushort m_usLife;
        private ushort m_usMaxLife;
        private ushort m_usMana;
        private ushort m_usMaxMana;
        private byte m_pXpPoints;
        private uint m_dwLookface;
        private ushort m_usTransformation;
        private ushort m_usAvatar;
        private ushort m_usBody;
        private ushort m_usHair;
        private byte m_pStamina;
        // battle attributes
        private int m_nMinAttack;
        private int m_nMinAttackEx;
        private int m_nMaxAttack;
        private int m_nMaxAttackEx;
        private int m_nMagicAttack;
        private int m_nMagicAttackEx;
        private int m_nDexterity;
        private int m_nAgility;
        private int m_nDefense;
        private int m_nMagicDefense;
        private int m_nAddFinalAttack;
        private int m_nAddFinalMagicAttack;
        private int m_nAddFinalDefense;
        private int m_nAddFinalMagicDefense;
        private int m_nMagicDefenseBonus;
        private int m_nDodge;
        private int m_nVioletGem;
        private int m_nKylinGem;
        private int m_nFuryGem;
        private int m_nRainbowGem;
        private int m_nPhoenixGem;
        private int m_nDragonGem;
        private int m_nMoonGem;
        private int m_nTortoiseGem;
        private int m_nAttackHitRate;
        private int m_nBlessing;
        private uint m_nCriticalStrike;
        private uint m_nSkillCritical;
        private uint m_nImmunity;
        private uint m_nBreak;
        private uint m_nCounteration;
        private uint m_nBlock;
        private uint m_nDetoxication;
        private uint m_nPenetration;
        private uint m_nMetalResist;
        private uint m_nWaterResist;
        private uint m_nFireResist;
        private uint m_nWoodResist;
        private uint m_nEarthResist;
        // requests
        private uint m_dwFriendRequest = 0;
        private uint m_dwMarryRequest = 0;
        private uint m_dwTradeRequest = 0;
        private uint m_dwTeamJoin = 0;
        private uint m_dwTeamInvite = 0;
        private uint m_dwSynJoin = 0;
        private uint m_dwSynInvite = 0;
        private uint m_dwSynAlly = 0;
        private uint m_dwTradeBuddy = 0;
        private uint m_dwGuideRequest = 0;
        private uint m_dwStudentRequest = 0;
        private uint m_dwFamilyRequest = 0;
        private uint m_dwJoinFamilyRequest = 0;
        // game action
        private uint m_lastItemResource;
        private uint m_lastUsedItem;
        private uint m_lastUsedItemTime;
        private uint m_lastUsedItemtype;
        // event
        private int m_nQuizCancelTime;
        private int m_nMineCount;
        // guide and event
        private float m_fMentorExp;
        private ushort m_usMentorBless;
        private ushort m_usMentorCompose;
        private uint m_dwStudentExp;
        private ushort m_usStudentBless;
        private uint m_dwStudentCompose;
        // battle
        private int m_nFightPause;
        private uint m_nKoCount;
        private long m_lAccumulateExp;
        private uint m_dwVigor;
        private uint m_dwMaxVigor;
        // status
        private byte m_pBlessPoints;
        private uint m_dwBlessEndTime;
        private uint m_dwLuckyTime;
        // qualifiers
        

        #region %iterator %iter_var_data %iter_var_str

        public string IterString0 = "",
            IterString1 = "",
            IterString2 = "",
            IterString3 = "",
            IterString4 = "",
            IterString5 = "",
            IterString6 = "",
            IterString7 = "";

        public long IterData0 = 0,
            IterData1 = 0,
            IterData2 = 0,
            IterData3 = 0,
            IterData4 = 0,
            IterData5 = 0,
            IterData6 = 0,
            IterData7 = 0;

        public ulong Iterator = 0;

        #endregion

        public ConcurrentDictionary<byte, DbTitle> Titles;
        public ConcurrentDictionary<uint, Relationship> Friends = new ConcurrentDictionary<uint, Relationship>();
        public ConcurrentDictionary<uint, Relationship> Enemies = new ConcurrentDictionary<uint, Relationship>();
        public ConcurrentDictionary<ulong, DbStatistic> Statistics = new ConcurrentDictionary<ulong, DbStatistic>();
        public ConcurrentDictionary<uint, TradePartner> TradePartners = new ConcurrentDictionary<uint, TradePartner>();
        public Dictionary<uint, INextAction> NextActions = new Dictionary<uint, INextAction>(16);
        public Dictionary<uint, Warehouse> Warehouses = new Dictionary<uint, Warehouse>();
        public ConcurrentDictionary<uint, Student> Apprentices = new ConcurrentDictionary<uint, Student>();
        public Guide Mentor;
        public Team Team;
        public Trade Trade;
        public QualifierRankObj ArenaQualifier;
        public RequestBox RequestBox = null;
        public CaptchaBox CaptchaBox
        {
            get { return m_pCaptchaBox; }
            set
            {
                RequestBox = null;
                m_pCaptchaBox = value;
            }
        }
        public string TemporaryString;

        public Character(MsgUserInfo pMsg, DbUser dbUser, Client pClient)
        {
            m_pOwner = pClient;
            m_dbUser = dbUser;
            m_pPacket = new MsgPlayer(pMsg);

            Name = dbUser.Name;
            Mate = dbUser.Mate;

            MapIdentity = dbUser.MapId;
            MapX = dbUser.MapX;
            MapY = dbUser.MapY;

            Lookface = dbUser.Lookface;
            Hair = dbUser.Hair;
            Body = (ushort)(dbUser.Lookface % 10000);
            Avatar = (ushort)(((dbUser.Lookface % 10000000) - Body) / 10000);

            Life = dbUser.Life;
            Mana = dbUser.Mana;

            Experience = dbUser.Experience;

            Silver = m_dbUser.Money;
            Emoney = m_dbUser.Emoney;
            MoneySaved = m_dbUser.MoneySaved;
            CoinMoney = m_dbUser.CoinMoney;

            Level = m_dbUser.Level;
            Metempsychosis = m_dbUser.Metempsychosis;
            Strength = m_dbUser.Strength;
            Agility = m_dbUser.Agility;
            Vitality = m_dbUser.Vitality;
            Spirit = m_dbUser.Spirit;
            AdditionalPoints = m_dbUser.AdditionalPoints;

            Profession = (byte)m_dbUser.Profession;
            LastProfession = (byte)m_dbUser.LastProfession;
            FirstProfession = (byte)m_dbUser.FirstProfession;

            Action = EntityAction.STAND;
            Direction = FacingDirection.WEST;

            QuizPoints = m_dbUser.StudentPoints;
            AutoAllot = m_dbUser.AutoAllot != 0;

            CurrentLayout = m_dbUser.CurrentLayout;

            m_tXp.Update();
            m_tXpStop.Update();
            m_tStamina.Update();
            m_pkDecrease.Update();
            m_lifeRecover.Update();
            m_timePacket.Update();
            m_tRespawn.Update();
            m_tSilence.Update();
            m_tWorldChat.Update();
            m_tCool.Update();
            m_tJumpUnable.Clear();
            m_tRespawn.Startup(CHGMAP_LOCK_SECS);
            m_tDoGhost.Clear();
            m_tRevive.Clear();
            m_tMine.Clear();
            m_tOnlineTime.Startup(1800);
            m_tHeavenBlessing.Update();
            m_tTeamPos.Update();
            m_tLuckyTime.Update();
            m_tDazed.Clear();
            m_tVigor.Update();

            Titles = new ConcurrentDictionary<byte, DbTitle>();
            Title = m_dbUser.SelectedTitle;
        }

        public uint Identity
        {
            get { return m_dbUser.Identity; }
        }

        public string Name
        {
            get { return m_dbUser.Name; }
            set
            {
                m_szUserName = value;
                m_dbUser.Name = value;
                SetNames();
            }
        }

        public string Mate
        {
            get { return m_dbUser.Mate; }
            set
            {
                m_dbUser.Mate = value;
                Save();
            }
        }

        public FacingDirection Direction
        {
            get { return m_pDirection; }
            set
            {
                m_pDirection = value;
                m_pPacket.Direction = value;
            }
        }

        public EntityAction Action
        {
            get { return m_pAction; }
            set
            {
                m_pAction = value;
                m_pPacket.Action = value;
            }
        }

        public Client Owner
        {
            get { return m_pOwner; }
        }

        public Screen Screen
        {
            get { if (m_pOwner == null) return null; return m_pOwner.Screen ?? (m_pOwner.Screen = new Screen(this)); }
        }

        public StatusSet Status
        {
            get { return m_pStatus ?? (m_pStatus = new StatusSet(this)); }
        }

        public Inventory Inventory
        {
            get { return m_pInventory ?? (m_pInventory = new Inventory(this)); }
        }

        public Equipment Equipment
        {
            get { return m_pEquipment ?? (m_pEquipment = new Equipment(this)); }
        }

        public WeaponSkill WeaponSkill
        {
            get { return m_pWeaponSkill ?? (m_pWeaponSkill = new WeaponSkill(this)); }
        }

        public SubClass SubClass
        {
            get { return m_pSubClass ?? (m_pSubClass = new SubClass(this)); } 
        }

        public BattleSystem BattleSystem
        {
            get { return m_pBattleSystem ?? (m_pBattleSystem = new BattleSystem(this)); }
        }

        public GameAction GameAction
        {
            get { return m_pGameAction ?? (m_pGameAction = new GameAction(this)); }
        }

        public MagicData Magics
        {
            get { return m_pMagics ?? (m_pMagics = new MagicData(this)); }
        }

        public Transformation QueryTransformation
        {
            get { return m_pTransformation; }
            set { m_pTransformation = value; }
        }

        public PlayerBooth Booth
        {
            get { return m_pPlayerBooth ?? (m_pPlayerBooth = new PlayerBooth(this)); }
            set { m_pPlayerBooth = value; }
        }

        public PkExploit PkExploit
        {
            get { return m_pkExploit ?? (m_pkExploit = new PkExploit(this)); }
        }

        public PkModeType PkMode
        {
            get { return m_pPkMode; }
            set
            {
                MsgAction pMsg = new MsgAction(Identity, (ushort) value, 0, GeneralActionType.CHANGE_PK_MODE);
                Send(pMsg);
                switch (value)
                {
                    case PkModeType.CAPTURE:
                        Send(
                            new MsgTalk(
                                "Restrictive PK mode. You can only attack monsters, black-name and red-name players.",
                                ChatTone.TOP_LEFT));
                        break;
                    case PkModeType.PEACE:
                        Send(
                            new MsgTalk(
                                "Peace mode. You can only attack monsters and won't hurt other players.",
                                ChatTone.TOP_LEFT));
                        break;
                    case PkModeType.PK_MODE:
                        Send(new MsgTalk("Free PK mode. You can attack anyone.",
                            ChatTone.TOP_LEFT));
                        break;
                    case PkModeType.TEAM:
                        Send(
                            new MsgTalk(
                                "Team PK mode. You can attack monsters and players except for your teammates.",
                                ChatTone.TOP_LEFT));
                        break;
                    default:
                        return;
                }
                m_pPkMode = value;
            }
        }

        public bool LoginComplete { get; set; }

        public uint Lookface
        {
            get { return m_dwLookface; }
            set
            {
                m_dwLookface = value;
                m_pPacket.Mesh = value;
                m_dbUser.Lookface = value;
                UpdateClient(ClientUpdateType.MESH, m_dwLookface, true);
                if (LoginComplete)
                    Save();
            }
        }

        public ushort Transformation
        {
            get { return m_usTransformation; }
            set
            {
                m_usTransformation = value;
                m_dwLookface = (uint)((value * 10000000) + m_usAvatar * 10000) + m_usBody;
                m_pPacket.Mesh = m_dwLookface;
                UpdateClient(ClientUpdateType.MESH, m_dwLookface, true);
            }
        }

        public ushort Body
        {
            get { return m_usBody; }
            set
            {
                m_usBody = value;
                m_dwLookface = (uint)((m_usTransformation * 10000000) + (m_usAvatar * 10000) + value);
                m_pPacket.Mesh = m_dwLookface;
                UpdateClient(ClientUpdateType.MESH, m_dwLookface, true);
                if (LoginComplete)
                {
                    m_dbUser.Lookface = (m_dwLookface % 10000000);
                    Save();
                }
            }
        }

        public ushort Avatar
        {
            get { return m_usAvatar; }
            set
            {
                m_usAvatar = value;
                m_dwLookface = (uint)((m_usTransformation * 10000000) + (value * 10000) + m_usBody);
                m_pPacket.Mesh = m_dwLookface;
                UpdateClient(ClientUpdateType.MESH, m_dwLookface, true);
                if (LoginComplete)
                {
                    m_dbUser.Lookface = (m_dwLookface % 10000000);
                    Save();
                }
            }
        }

        public ushort Hair
        {
            get { return m_usHair; }
            set
            {
                m_usHair = value;
                m_pPacket.Hairstyle = value;
                m_dbUser.Hair = value;
                UpdateClient(ClientUpdateType.HAIR_STYLE, value, true);
                Save();
            }
        }

        public byte Gender
        {
            get { return (byte)((m_dwLookface % 10000) / 1000); }
        }

        public ushort PkPoints
        {
            get { return m_dbUser.PkPoints; }
            set
            {
                m_dbUser.PkPoints = value;
                UpdateClient(ClientUpdateType.PK_POINTS, value, true);

                if (!LoginComplete) return;

                if (value > 99)
                {
                    if (Status.GetObjByIndex(FlagInt.BLACK_NAME) == null)
                    {
                        if (Status.GetObjByIndex(FlagInt.RED_NAME) != null)
                            Status.DelObj(FlagInt.RED_NAME);

                        var pStatus = new StatusOnce(this);
                        pStatus.Create(this, FlagInt.BLACK_NAME, 0, int.MaxValue / 1000, 0);
                        Status.AddObj(pStatus);
                    }
                }
                else if (value > 29)
                {
                    if (value < 100 && Status.GetObjByIndex(FlagInt.BLACK_NAME) != null)
                        Status.DelObj(FlagInt.BLACK_NAME);
                    if (Status.GetObjByIndex(FlagInt.RED_NAME) == null)
                    {
                        var pStatus = new StatusOnce(this);
                        pStatus.Create(this, FlagInt.RED_NAME, 0, int.MaxValue / 1000, 0);
                        Status.AddObj(pStatus);
                    }
                }
                else
                {
                    if (value < 100 && Status.GetObjByIndex(FlagInt.BLACK_NAME) != null)
                        Status.DelObj(FlagInt.BLACK_NAME);
                    if (value < 30 && Status.GetObjByIndex(FlagInt.RED_NAME) != null)
                        Status.DelObj(FlagInt.RED_NAME);
                }
                Save();
            }
        }

        public byte Stamina
        {
            get { return m_pStamina; }
            set
            {
                m_pStamina = value;
                UpdateClient(ClientUpdateType.STAMINA, value);
            }
        }

        /// <summary>
        /// The actual profession of the actor
        /// </summary>
        public ushort Profession
        {
            get { return m_dbUser.Profession; }
            set
            {
                if (Enum.IsDefined(typeof(ProfessionType), value))
                {
                    m_dbUser.Profession = value;

                    if (LoginComplete)
                    {
                        UpdateClient(ClientUpdateType.CLASS, value);
                        Save();
                    }
                }
            }
        }

        /// <summary>
        /// The last profession of the actor (First Rebirth)
        /// </summary>
        public ushort LastProfession
        {
            get { return m_dbUser.LastProfession; }
            set
            {
                if (Enum.IsDefined(typeof(ProfessionType), value))
                {
                    m_dbUser.LastProfession = value;
                    m_pPacket.LastProfession = (ProfessionType) value;
                    Save();
                }
            }
        }

        /// <summary>
        /// The first profession of the actor (First Life)
        /// </summary>
        public ushort FirstProfession
        {
            get { return m_dbUser.FirstProfession; }
            set
            {
                if (Enum.IsDefined(typeof(ProfessionType), value))
                {
                    m_dbUser.FirstProfession = value;
                    m_pPacket.FirstProfession = (ProfessionType) value;
                    Save();
                }
            }
        }

        public long Experience
        {
            get { return m_dbUser.Experience; }
            set
            {
                m_dbUser.Experience = value;
                if (LoginComplete)
                    UpdateClient(ClientUpdateType.EXPERIENCE, (ulong)value);
                Save();
            }
        }

        public byte Level
        {
            get { return m_dbUser.Level; }
            set
            {
                m_pPacket.Level = value;
                m_dbUser.Level = value;
                if (LoginComplete)
                    UpdateClient(ClientUpdateType.LEVEL, value);
                Save();
            }
        }

        public ushort AdditionalPoints
        {
            get { return m_dbUser.AdditionalPoints; }
            set
            {
                m_dbUser.AdditionalPoints = value;
                Save();
                if (LoginComplete)
                    UpdateClient(ClientUpdateType.ATRIBUTES, value);
            }
        }

        public ushort Strength
        {
            get { return m_dbUser.Strength; }
            set
            {
                m_dbUser.Strength = value;
                if (LoginComplete)
                {
                    UpdateClient(ClientUpdateType.STRENGTH, value);
                    Save();
                }
            }
        }

        public ushort Agility
        {
            get { return m_dbUser.Agility; }
            set
            {
                m_dbUser.Agility = value;
                if (LoginComplete)
                {
                    UpdateClient(ClientUpdateType.AGILITY, value);
                    Save();
                }
            }
        }

        public ushort Vitality
        {
            get { return m_dbUser.Vitality; }
            set
            {
                m_dbUser.Vitality = value;
                if (LoginComplete)
                {
                    UpdateClient(ClientUpdateType.VITALITY, value);
                    Save();
                }
            }
        }

        public ushort Spirit
        {
            get { return m_dbUser.Spirit; }
            set
            {
                m_dbUser.Spirit = value;
                if (LoginComplete)
                {
                    UpdateClient(ClientUpdateType.SPIRIT, value);
                    Save();
                }
            }
        }

        public byte XpPoints
        {
            get { return m_pXpPoints; }
            set
            {
                m_pXpPoints = (byte)(value > 100 ? 100 : value);
                UpdateClient(ClientUpdateType.XP_CIRCLE, value, false);
            }
        }

        public byte Metempsychosis
        {
            get { return m_dbUser.Metempsychosis; }
            set
            {
                m_pPacket.Metempsychosis = value;
                m_dbUser.Metempsychosis = value;
                if (LoginComplete)
                {
                    UpdateClient(ClientUpdateType.REBORN, value, true);
                    Save();
                }
            }
        }

        public uint Silver
        {
            get { return m_dbUser.Money; }
            set
            {
                if (value > int.MaxValue) value = int.MaxValue;

                m_dbUser.Money = value;
                if (LoginComplete)
                {
                    UpdateClient(ClientUpdateType.MONEY, value);
                    Save();
                }
            }
        }

        public uint Emoney
        {
            get { return m_dbUser.Emoney; }
            set
            {
                m_dbUser.Emoney = value;
                if (LoginComplete)
                {
                    UpdateClient(ClientUpdateType.CONQUER_POINTS, value);
                    Save();
                }
            }
        }

        public uint BoundEmoney
        {
            get { return m_dbUser.BoundEmoney; }
            set
            {
                m_dbUser.BoundEmoney = value;
                if (LoginComplete)
                {
                    UpdateClient(ClientUpdateType.BOUND_CONQUER_POINTS, value);
                    Save();
                }
            }
        }

        /// <summary>
        /// Warehouse money :)
        /// </summary>
        public uint MoneySaved
        {
            get { return m_dbUser.MoneySaved; }
            set
            {
                m_dbUser.MoneySaved = value;
                Save();
            }
        }

        public uint CoinMoney
        {
            get { return m_dbUser.CoinMoney; }
            set
            {
                m_dbUser.CoinMoney = value;
                Save();
            }
        }

        public uint Life
        {
            get
            {
                if (QueryTransformation != null)
                    return QueryTransformation.Life;
                return m_usLife;
            }
            set
            {
                if (QueryTransformation != null)
                {
                    ushort life = (ushort)(value > ushort.MaxValue ? ushort.MaxValue : value);
                    QueryTransformation.Life = life;
                    return;
                }
                m_usLife = (ushort)(value > ushort.MaxValue ? ushort.MaxValue : value);
                m_pPacket.Life = m_usLife;
                UpdateClient(ClientUpdateType.HITPOINTS, m_usLife, true);
                if (m_usLife > 0)
                {
                    m_dbUser.Life = m_usLife;
                    Save();
                }
            }
        }

        public uint MaxLife
        {
            get
            {
                if (QueryTransformation != null)
                    return QueryTransformation.MaxLife;
                return m_usMaxLife;
            }
            set
            {
                if (value > 0)
                {
                    m_usMaxLife = (ushort)(value < Life ? Life = value : value);
                    if (m_usMaxLife > ushort.MaxValue)
                        m_usMaxLife = ushort.MaxValue;
                    UpdateClient(ClientUpdateType.MAX_HITPOINTS, m_usMaxLife, true);
                }
            }
        }

        public ushort Mana
        {
            get { return m_usMana; }
            set
            {
                m_usMana = value;
                m_dbUser.Mana = value;
                if (LoginComplete)
                {
                    UpdateClient(ClientUpdateType.MANA, value);
                }
                Save();
            }
        }

        public ushort MaxMana
        {
            get { return m_usMaxMana; }
            set
            {
                m_usMaxMana = value;
                UpdateClient(ClientUpdateType.MAX_MANA, value);
            }
        }

        public ulong Flag1
        {
            get { return m_pPacket.Flag1; }
            set
            {
                m_pPacket.Flag1 = value;
                UpdateClient(ClientUpdateType.STATUS_FLAG, m_pPacket.Flag1, true);
            }
        }

        public ulong Flag2
        {
            get { return m_pPacket.Flag2; }
            set
            {
                m_pPacket.Flag2 = value;
                UpdateClient(ClientUpdateType.STATUS_FLAG, m_pPacket.Flag1, m_pPacket.Flag2, true);
            }
        }

        public bool AutoAllot
        {
            get { return m_dbUser.AutoAllot > 0; }
            set
            {
                m_dbUser.AutoAllot = (byte)(value ? 1 : 0);
                Save();
            }
        }

        public uint QuizPoints
        {
            get { return m_dbUser.StudentPoints; }
            set
            {
                m_dbUser.StudentPoints = value;
                m_pPacket.QuizPoints = value;
                if (m_pOwner != null)
                    UpdateClient(ClientUpdateType.QUIZ_POINTS, value, true);
                Save();
            }
        }

        public uint RedRoses
        {
            get { return m_dbUser.RedRoses; }
            set
            {
                m_dbUser.RedRoses = value;
                Save();
            }
        }

        public uint WhiteRoses
        {
            get { return m_dbUser.WhiteRoses; }
            set
            {
                m_dbUser.WhiteRoses = value;
                Save();
            }
        }

        public uint Orchids
        {
            get { return m_dbUser.Orchids; }
            set
            {
                m_dbUser.Orchids = value;
                Save();
            }
        }

        public uint Tulips
        {
            get { return m_dbUser.Tulips; }
            set
            {
                m_dbUser.Tulips = value;
                Save();
            }
        }

        public uint VirtuePoints
        {
            get { return m_dbUser.Virtue; }
            set
            {
                m_dbUser.Virtue = value;
                Save();
            }
        }

        public byte Title
        {
            get { return m_dbUser.SelectedTitle; }
            set
            {
                m_dbUser.SelectedTitle = value;
                m_pPacket.Title = value;
                Save();
            }
        }

        public Nobility Nobility
        {
            get { return m_pNobility ?? (m_pNobility = new Nobility(this)); }
            set { m_pNobility = value; }
        }

        public NobilityLevel NobilityRank
        {
            get { return m_pNobility == null ? NobilityLevel.SERF : m_pNobility.Level; }
            set { m_pPacket.Nobility = (byte) value; }
        }

        public long NobilityDonation
        {
            get { return m_dbUser.Donation; }
            set
            {
                m_dbUser.Donation = value;
                Save();
            }
        }

        public byte ExpBallAmount
        {
            get { return m_dbUser.ChkSum; }
            set
            {
                m_dbUser.ChkSum = value;
                Save();
            }
        }

        public uint LastUsedExpBall
        {
            get { return m_dbUser.ExpBallUsage; }
            set
            {
                m_dbUser.ExpBallUsage = value;
                Save();
            }
        }

        public bool Away
        {
            get { return m_pPacket.Away; }
            set { m_pPacket.Away = value; }
        }

        public uint StudyPoints
        {
            get { return m_dbUser.StudyPoints; }
            set
            {
                m_dbUser.StudyPoints = value;
                Save();
            }
        }

        public SubClasses ActiveSubclass
        {
            get { return (SubClasses) m_dbUser.ActiveSubclass; }
            set
            {
                m_dbUser.ActiveSubclass = (byte) value;
                Save();
            }
        }

        public SyndicateRank SyndicateRank
        {
            get
            {
                if (m_pSynMember == null) return SyndicateRank.NONE;
                return m_pSynMember.Position;
            }
            set { m_pPacket.GuildRank = value; }
        }

        public Syndicate Syndicate
        {
            get { return m_pSyndicate; }
            set
            {
                m_pSyndicate = value;
                m_pPacket.GuildIdentity = (ushort) (value == null ? 0 : value.Identity);
            }
        }

        public SyndicateMember SyndicateMember
        {
            get { return m_pSynMember; }
            set
            {
                m_pSynMember = value;
                m_pPacket.GuildIdentity = (ushort) (value == null ? 0 : value.SyndicateIdentity);
                m_pPacket.GuildRank = (SyndicateRank) (value == null ? 0 : value.Position);
            }
        }

        public uint SyndicateIdentity
        {
            get
            {
                if (m_pSyndicate == null) return 0;
                return m_pSyndicate.Identity;
            }
            set { m_pPacket.GuildIdentity = (ushort) value; }
        }

        public string SyndicateName
        {
            get
            {
                if (m_pSyndicate == null) return "None";
                return m_pSyndicate.Name;
            }
        }

        public byte SyndicateBattlePower
        {
            get
            {
                if (m_pSyndicate == null || m_pSyndicate.Arsenal == null) return 0;
                return (byte)m_pSyndicate.Arsenal.SharedBattlePower(this);
            }
        }

        public Family Family
        {
            get { return m_pFamily; }
            set { m_pFamily = value; }
        }

        public FamilyMember FamilyMember
        {
            get { return m_pFamilyMember; }
            set { m_pFamilyMember = value; }
        }

        public FamilyRank FamilyPosition
        {
            get { return m_pFamilyMember != null ? m_pFamilyMember.Position : FamilyRank.NONE; }
            set { m_pPacket.FamilyRank = value; }
        }

        public uint FamilyIdentity
        {
            get { return m_pFamily != null ? m_pFamily.Identity : 0; }
            set { m_pPacket.FamilyIdentity = value; }
        }

        public string FamilyName
        {
            get { return m_pFamily != null ? m_pFamily.Name : "None"; }
            set
            {
                m_szFamilyName = value;
                SetNames();
            }
        }

        public byte FamilyBattlePower
        {
            get
            {
                //if (Team != null && Family != null)
                //{
                //    Character member = Team.Members.Values.Where(x => x.Identity != Identity && x.MapIdentity == MapIdentity)
                //        .OrderByDescending(x => x.PureBattlePower).FirstOrDefault();
                //    if (member == null || member.Family == null || member.FamilyIdentity != FamilyIdentity) return 0;
                //    if (member.PureBattlePower > PureBattlePower)
                //    {
                //        return (byte) Math.Max(0, (member.PureBattlePower - PureBattlePower)*(Family.SharedPercent/100f));
                //    }
                //}
                return 0;
            }
        }

        public byte MentorBattlePower
        {
            get
            {
                if (Mentor == null)
                    return 0;
                return (byte) Mentor.SharedBattlePower;
            }
        }

        public byte SharedBattlePower
        {
            get { return Math.Max(FamilyBattlePower, MentorBattlePower); }
        }

        public bool IsWorldChatEnable
        {
            get
            {
                if (Level < 70)
                    return false;
                if (Metempsychosis > 0)
                {
                    if (Level < 100)
                        return m_tWorldChat.ToNextTime(WORLD_CHAT_DELAY_100);
                    if (Level < 110)
                        return m_tWorldChat.ToNextTime(WORLD_CHAT_DELAY_110);
                    return m_tWorldChat.ToNextTime(WORLD_CHAT_DELAY_120);
                }
                else
                {
                    if (Level < 100)
                        return m_tWorldChat.ToNextTime(WORLD_CHAT_DELAY_100 + WORLD_CHAT_ADD_DELAY);
                    if (Level < 110)
                        return m_tWorldChat.ToNextTime(WORLD_CHAT_DELAY_110 + WORLD_CHAT_ADD_DELAY);
                    return m_tWorldChat.ToNextTime(WORLD_CHAT_DELAY_120 + WORLD_CHAT_ADD_DELAY);
                }
            }
        }

        public int WorldChatWaitTime
        {
            get
            {
                if (Level < 70)
                    return 999999;
                if (Metempsychosis > 0)
                {
                    if (Level < 100)
                        return WORLD_CHAT_DELAY_100;
                    if (Level < 110)
                        return WORLD_CHAT_DELAY_110;
                    return WORLD_CHAT_DELAY_120;
                }

                if (Level < 100)
                    return WORLD_CHAT_DELAY_100 + WORLD_CHAT_ADD_DELAY;
                if (Level < 110)
                    return WORLD_CHAT_DELAY_110 + WORLD_CHAT_ADD_DELAY;
                return WORLD_CHAT_DELAY_120 + WORLD_CHAT_ADD_DELAY;
            }
        }

        public bool IsCoolEnabled
        {
            get { return m_tCool.ToNextTime(5); }
        }

        public bool IsPm
        {
            get { return m_dbUser.Name.Contains("[PM]"); }
        }

        public bool IsGm
        {
            get { return m_dbUser.Name.Contains("[GM]") || IsPm; }
        }

        public uint KoCount
        {
            get { return m_nKoCount; }
        }

        public ulong WarehousePassword
        {
            get { return m_dbUser.LockKey; }
            set
            {
                m_dbUser.LockKey = value;
                Save();
            }
        }

        public uint Vigor
        {
            get { return m_dwVigor; }
            set
            {
                m_dwVigor = value > m_dwMaxVigor ? m_dwMaxVigor : value;

                MsgData pMsg = new MsgData();
                pMsg.WriteUInt(2, 4);
                pMsg.WriteUInt(m_dwVigor, 8);
                Send(pMsg);
            }
        }

        public uint MaxVigor
        {
            get { return m_dwMaxVigor; }
        }

        public ushort EnlightmentPoints
        {
            get { return m_dbUser.EnlightPoints; }
            set
            {
                m_dbUser.EnlightPoints = value;
                UpdateClient(ClientUpdateType.ENLIGHT_POINTS, value, true);
                Save();
            }
        }

        public void SetNames()
        {
            m_pPacket.StringCount = 2;
            m_pPacket.Name = m_szUserName;
            m_pPacket.SecondName = m_szSecondName;
            if (m_szFamilyName != string.Empty)
            {
                m_pPacket.StringCount = 3;
                m_pPacket.FamilyName = m_szFamilyName;
            }
        }

        public byte CurrentLayout
        {
            get { return m_dbUser.CurrentLayout; }
            set
            {
                m_dbUser.CurrentLayout = value;
                m_pPacket.CurrentLayout = value;
                Save();
            }
        }

        public bool Invisible { get; set; }

        #region Battle Calculation

        public void LoadMagics()
        {
            m_pMagics = new MagicData(this);
        }

        public bool Scapegoat { get; set; }

        public void RecalculateAttributes()
        {
            #region Reset
            m_nAddFinalAttack = 0;
            m_nAddFinalDefense = 0;
            m_nAddFinalMagicAttack = 0;
            m_nAddFinalMagicDefense = 0;

            m_nMinAttack = 0;
            m_nMinAttackEx = 0;
            m_nMaxAttack = 0;
            m_nMaxAttackEx = 0;
            m_nDefense = 0;
            m_nMagicDefense = 0;
            m_nMagicAttack = 0;
            m_nMagicAttackEx = 0;
            m_nMagicDefenseBonus = 0;

            m_nDexterity = 0;
            m_nAgility = 0;
            m_nAttackHitRate = 0;
            m_nDodge = 0;

            m_nDragonGem = 0;
            m_nPhoenixGem = 0;
            m_nTortoiseGem = 0;
            m_nRainbowGem = 0;
            m_nVioletGem = 0;
            m_nKylinGem = 0;
            m_nFuryGem = 0;
            m_nMoonGem = 0;

            m_nBlessing = 0;

            m_nFireResist = 0;
            m_nWaterResist = 0;
            m_nWoodResist = 0;
            m_nEarthResist = 0;
            m_nMetalResist = 0;

            m_nCriticalStrike = 0;
            m_nSkillCritical = 0;
            m_nImmunity = 0;
            m_nBreak = 0;
            m_nPenetration = 0;
            m_nCounteration = 0;
            m_nBlock = 0;
            m_nDetoxication = 0;

            m_dwMaxVigor = 0;
            #endregion

            int nAddLife = 0, nAddMana = 0;
            m_nMinAttack = Strength;
            m_nMaxAttack = Strength;

            nAddLife += (ushort)(Vitality * GetLifeMultiplier());
            nAddMana += (ushort)(Spirit * GetManaMultiplier());
            nAddLife += (ushort)((Strength + Agility + Spirit) * 3);

            bool isSkillPk = Map.IsSkillMap();

            if (!isSkillPk)
            {
                foreach (var sClass in SubClass.Professions.Values)
                {
                    switch (sClass.Class)
                    {
                        case SubClasses.APOTHECARY:
                        {
                            m_nDetoxication += SubClass.POWER_APOTHECARY[sClass.Promotion];
                            continue;
                        }
                        case SubClasses.CHI_MASTER:
                        {
                            m_nImmunity += SubClass.POWER_CHI_MASTER[sClass.Promotion];
                            continue;
                        }
                        case SubClasses.MARTIAL_ARTIST:
                        {
                            m_nCriticalStrike += SubClass.POWER_MARTIAL_ARTIST[sClass.Promotion];
                            continue;
                        }
                        case SubClasses.PERFORMER:
                        {
                            m_nMinAttack += (int) SubClass.POWER_PERFORMER[sClass.Promotion];
                            m_nMinAttackEx += (int) SubClass.POWER_PERFORMER[sClass.Promotion];
                            m_nMaxAttack += (int) SubClass.POWER_PERFORMER[sClass.Promotion];
                            m_nMaxAttackEx += (int) SubClass.POWER_PERFORMER[sClass.Promotion];
                            m_nMagicAttack += (int) SubClass.POWER_PERFORMER[sClass.Promotion];
                            m_nMagicAttackEx += (int) SubClass.POWER_PERFORMER[sClass.Promotion];
                            continue;
                        }
                        case SubClasses.SAGE:
                        {
                            m_nPenetration += SubClass.POWER_SAGE[sClass.Promotion];
                            continue;
                        }
                        case SubClasses.WARLOCK:
                        {
                            m_nSkillCritical += SubClass.POWER_WARLOCK[sClass.Promotion];
                            continue;
                        }
                        case SubClasses.WRANGLER:
                        {
                            nAddLife += (ushort) SubClass.POWER_WRANGLER[sClass.Promotion];
                            continue;
                        }
                    }
                }
            }

            if (Equipment != null)
            {
                foreach (Item item in Equipment.Items.Values.Where(x => x.Position <= ItemPosition.CROP))
                {
                    if (item.IsBroken()) continue;

                    uint equipType = item.Type / 1000;
                    bool isSingleHandWeap = item.Position == ItemPosition.RIGHT_HAND
                            && (item.GetItemSubtype() == 421 || item.GetSort() == ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND);

                    if (isSingleHandWeap
                                    && Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND)
                                    && item.GetSort() == ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND)
                        isSingleHandWeap = false;

                    if (equipType == 201
                        || equipType == 202)
                    {
                        m_nAddFinalAttack += item.Itemtype.AttackMax;
                        m_nAddFinalMagicAttack += item.Itemtype.MagicAtk;
                        m_nAddFinalDefense += item.Itemtype.Defense;
                        m_nAddFinalMagicDefense += item.Itemtype.MagicDef;
                    }
                    else
                    {
                        if (item.Position == ItemPosition.LEFT_HAND
                            &&
                            (item.GetSort() == ItemSort.ITEMSORT_WEAPON_SINGLE_HAND ||
                             item.GetSort() == ItemSort.ITEMSORT_WEAPON_SINGLE_HAND2))
                        {
                            m_nMinAttack += (item.Itemtype.AttackMin / 2);
                            m_nMaxAttack += (item.Itemtype.AttackMax / 2);
                            m_nMinAttackEx += (item.Itemtype.AttackMin / 2);
                            m_nMaxAttackEx += (item.Itemtype.AttackMax / 2);
                        }
                        else
                        {
                            m_nMinAttack += item.Itemtype.AttackMin;
                            m_nMaxAttack += item.Itemtype.AttackMax;
                            m_nMinAttackEx += item.Itemtype.AttackMin;
                            m_nMaxAttackEx += item.Itemtype.AttackMax;
                        }

                        m_nMagicAttack += item.Itemtype.MagicAtk;
                        m_nMagicAttackEx += item.Itemtype.MagicAtk;
                        m_nMagicDefense += item.Itemtype.MagicDef;

                        if (item.Itemtype.Defense < 0)
                        {
                            if (item.Itemtype.Defense * -1 > m_nDefense)
                            {
                                m_nDefense = 0;
                            }
                            else
                            {
                                m_nDefense += item.Itemtype.Defense;
                            }
                        }
                        else
                        {
                            m_nDefense += item.Itemtype.Defense;
                        }
                    }

                    if (item.Position >= ItemPosition.RIGHT_HAND && item.Position <= ItemPosition.RING)
                        m_nAttackHitRate += (ushort)(item.Itemtype.AtkSpeed);

                    m_nDodge += (ushort)item.Itemtype.Dodge;
                    nAddLife += (ushort)item.Itemtype.Life;
                    nAddMana += (ushort)item.Itemtype.Mana;

                    int multiplyAttr = isSingleHandWeap ? 2 : 1;
                    if (equipType != 300 && equipType != 203)
                    {
                        m_nDexterity += (ushort)item.Itemtype.Dexterity;
                        m_nAgility += (ushort) (item.Itemtype.Dexterity* multiplyAttr);
                    }

                    if (Enum.IsDefined(typeof(SocketGem), item.SocketOne))
                    {
                        var gem1 = item.SocketOne;
                        if ((byte)item.SocketOne > 100
                            && (byte)item.SocketOne < 110)
                        {
                            m_nAddFinalAttack += (int)Calculations.GetTalismanGemAttr(gem1) * multiplyAttr;
                            m_nAddFinalMagicAttack += (int)Calculations.GetTalismanGemAttr(gem1) * multiplyAttr;
                        }
                        else if ((byte)item.SocketOne > 120
                                 && (byte)item.SocketOne < 130)
                        {
                            m_nAddFinalDefense += (int)Calculations.GetTalismanGemAttr(gem1) * multiplyAttr;
                            m_nAddFinalMagicDefense += (int)Calculations.GetTalismanGemAttr(gem1) * multiplyAttr;
                        }
                        else if ((byte)item.SocketOne > 11
                                 && (byte)item.SocketOne < 20)
                        {
                            m_nDragonGem += Calculations.CalculateGemPercentage(gem1) * multiplyAttr;
                        }
                        else if ((byte)item.SocketOne > 0
                                 && (byte)item.SocketOne < 10)
                        {
                            m_nPhoenixGem += Calculations.CalculateGemPercentage(gem1) * multiplyAttr;
                        }
                        else if ((byte)item.SocketOne > 70
                                 && (byte)item.SocketOne < 80)
                        {
                            m_nTortoiseGem += Calculations.CalculateGemPercentage(gem1) * multiplyAttr;
                        }
                        else if (item.SocketOne >= SocketGem.NORMAL_MOON_GEM
                                  && item.SocketOne <= SocketGem.SUPER_MOON_GEM)
                        {
                            m_nMoonGem += Calculations.CalculateGemPercentage(gem1) * multiplyAttr;
                        }
                    }

                    if (Enum.IsDefined(typeof(SocketGem), item.SocketTwo) && !isSkillPk)
                    {
                        var gem1 = item.SocketTwo;
                        if ((byte)item.SocketTwo > 100
                            && (byte)item.SocketTwo < 110)
                        {
                            m_nAddFinalAttack += (int)Calculations.GetTalismanGemAttr(gem1) * multiplyAttr;
                            m_nAddFinalMagicAttack += (int)Calculations.GetTalismanGemAttr(gem1) * multiplyAttr;
                        }
                        else if ((byte)item.SocketTwo > 120
                                 && (byte)item.SocketTwo < 130)
                        {
                            m_nAddFinalDefense += (int)Calculations.GetTalismanGemAttr(gem1) * multiplyAttr;
                            m_nAddFinalMagicDefense += (int)Calculations.GetTalismanGemAttr(gem1) * multiplyAttr;
                        }
                        else if ((byte)item.SocketTwo > 11
                                 && (byte)item.SocketTwo < 20)
                        {
                            m_nDragonGem += Calculations.CalculateGemPercentage(gem1) * multiplyAttr;
                        }
                        else if ((byte)item.SocketTwo > 0
                                 && (byte)item.SocketTwo < 10)
                        {
                            m_nPhoenixGem += Calculations.CalculateGemPercentage(gem1) * multiplyAttr;
                        }
                        else if ((byte)item.SocketTwo > 70
                                 && (byte)item.SocketTwo < 80)
                        {
                            m_nTortoiseGem += Calculations.CalculateGemPercentage(gem1) * multiplyAttr;
                        }
                        else if (item.SocketOne >= SocketGem.NORMAL_MOON_GEM
                                  && item.SocketOne <= SocketGem.SUPER_MOON_GEM)
                        {
                            m_nMoonGem += Calculations.CalculateGemPercentage(gem1) * multiplyAttr;
                        }
                    }

                    ItemPosition pos = Calculations.GetItemPosition(item.Type);
                    if (pos == ItemPosition.ARMOR || pos == ItemPosition.HEADWEAR)
                        m_nMagicDefenseBonus += item.Itemtype.MagicDef;
                    else
                        m_nMagicDefense += item.Itemtype.MagicDef;

                    if (!isSkillPk)
                        m_nBlessing += (item.ReduceDamage*multiplyAttr);
                    else
                        m_nBlessing += item.ReduceDamage > 1 ? 1 : item.ReduceDamage;

                    nAddLife += item.Enchantment;
                    m_nCriticalStrike += item.Itemtype.CritStrike;
                    m_nSkillCritical += item.Itemtype.SkillCritStrike;
                    m_nImmunity += item.Itemtype.Immunity;
                    m_nPenetration += item.Itemtype.Penetration;
                    m_nBlock += item.Itemtype.Block;
                    m_nBreak += item.Itemtype.Breakthrough;
                    m_nCounteration += item.Itemtype.Counteraction;
                    m_nMetalResist += item.Itemtype.ResistMetal;
                    m_nMetalResist += item.Itemtype.ResistWood;
                    m_nWaterResist += item.Itemtype.ResistWater;
                    m_nFireResist += item.Itemtype.ResistFire;
                    m_nEarthResist += item.Itemtype.ResistEarth;

                    if (!isSkillPk)
                    {
                        if (item.Artifact.Avaiable)
                        {
                            m_nMinAttack += item.Artifact.Artifact.AttackMin;
                            m_nMinAttackEx += item.Artifact.Artifact.AttackMin;
                            m_nMaxAttack += item.Artifact.Artifact.AttackMax;
                            m_nMaxAttackEx += item.Artifact.Artifact.AttackMax;
                            m_nDefense += item.Artifact.Artifact.Defense;
                            m_nMagicAttack += item.Artifact.Artifact.MagicAtk;
                            m_nMagicAttackEx += item.Artifact.Artifact.MagicAtk;
                            m_nMagicDefense += item.Artifact.Artifact.MagicDef;
                            nAddLife += item.Artifact.Artifact.Life;

                            m_nCriticalStrike += item.Artifact.Artifact.CritStrike;
                            m_nSkillCritical += item.Artifact.Artifact.SkillCritStrike;
                            m_nImmunity += item.Artifact.Artifact.Immunity;
                            m_nBreak += item.Artifact.Artifact.Breakthrough;
                            m_nCounteration += item.Artifact.Artifact.Counteraction;
                            m_nBlock += item.Artifact.Artifact.Block;
                            m_nPenetration += item.Artifact.Artifact.Penetration;
                            m_nDetoxication += item.Artifact.Artifact.Detoxication;

                            m_nEarthResist += item.Artifact.Artifact.ResistEarth;
                            m_nWaterResist += item.Artifact.Artifact.ResistWater;
                            m_nFireResist += item.Artifact.Artifact.ResistFire;
                            m_nWoodResist += item.Artifact.Artifact.ResistWood;
                            m_nMetalResist += item.Artifact.Artifact.ResistMetal;
                        }

                        if (item.Refinery.Avaiable)
                        {
                            switch (item.Refinery.Mode)
                            {
                                case RefineryType.REF_MDEFENSE:
                                    m_nMagicDefenseBonus += (int) item.Refinery.RefineryPercent;
                                    break;
                                case RefineryType.REF_CRITICAL_STRIKE:
                                    m_nCriticalStrike += item.Refinery.RefineryPercent*100;
                                    break;
                                case RefineryType.REF_SCRITICAL_STRIKE:
                                    m_nSkillCritical += item.Refinery.RefineryPercent*100;
                                    break;
                                case RefineryType.REF_IMMUNITY:
                                    m_nImmunity += item.Refinery.RefineryPercent*100;
                                    break;
                                case RefineryType.REF_BREAKTHROUGH:
                                    m_nBreak += item.Refinery.RefineryPercent*10;
                                    break;
                                case RefineryType.REF_COUNTERACTION:
                                    m_nCounteration += item.Refinery.RefineryPercent*10;
                                    break;
                                case RefineryType.REF_DETOXICATION:
                                    m_nDetoxication += item.Refinery.RefineryPercent;
                                    break;
                                case RefineryType.REF_BLOCK:
                                    m_nBlock += item.Refinery.RefineryPercent*100;
                                    break;
                                case RefineryType.REF_PENETRATION:
                                    m_nPenetration += item.Refinery.RefineryPercent*100;
                                    break;
                                case RefineryType.REF_INTENSIFICATION:
                                    nAddLife += (ushort) item.Refinery.RefineryPercent;
                                    break;
                                case RefineryType.REF_FIRE_RESIST:
                                    m_nFireResist += item.Refinery.RefineryPercent;
                                    break;
                                case RefineryType.REF_WATER_RESIST:
                                    m_nWaterResist += item.Refinery.RefineryPercent;
                                    break;
                                case RefineryType.REF_WOOD_RESIST:
                                    m_nWoodResist += item.Refinery.RefineryPercent;
                                    break;
                                case RefineryType.REF_METAL_RESIST:
                                    m_nMetalResist += item.Refinery.RefineryPercent;
                                    break;
                                case RefineryType.REF_EARTH_RESIST:
                                    m_nEarthResist += item.Refinery.RefineryPercent;
                                    break;
                            }
                        }
                    }

                    byte plus = item.Plus;
                    if (plus > 5 && isSkillPk)
                        plus = 5;

                    if (equipType == 300)
                    {
                        m_dwMaxVigor += (uint) item.Itemtype.Dexterity;
                    }

                    DbItemAddition dbAdd = Item.GetItemAddition(item.Type, plus);
                    if (dbAdd == null) continue;

                    if (equipType != 300)
                    {
                        m_nAgility += dbAdd.Dexterity;
                        m_nDodge += dbAdd.Dodge;
                    }
                    else
                    {
                        m_dwMaxVigor += dbAdd.Dexterity;
                    }

                    if (equipType == 201
                        || equipType == 202)
                    {
                        m_nAddFinalAttack += dbAdd.AttackMax;
                        m_nAddFinalDefense += dbAdd.Defense;
                        m_nAddFinalMagicAttack += dbAdd.MagicAtk;
                        m_nAddFinalMagicDefense += dbAdd.MagicDef;
                    }
                    else
                    {
                        m_nMinAttack += dbAdd.AttackMin;
                        m_nMaxAttack += dbAdd.AttackMax;
                        m_nMinAttackEx += dbAdd.AttackMin;
                        m_nMaxAttackEx += dbAdd.AttackMax;
                        m_nDefense += dbAdd.Defense;
                        m_nMagicAttack += dbAdd.MagicAtk;
                        m_nMagicAttackEx += dbAdd.MagicAtk;
                        m_nMagicDefense += dbAdd.MagicDef;
                    }

                    nAddLife += dbAdd.Life;
                }
            }

            for (int i = 98; i <= 111; i++)
            {
                var pStatus = QueryStatus(i);
                if (pStatus != null)
                {
                    switch (pStatus.Identity)
                    {
                        case FlagInt.TYRANT_AURA:
                            m_nCriticalStrike += (uint)pStatus.Power * 100;
                            continue;
                        case FlagInt.FEND_AURA:
                            m_nImmunity += (uint)pStatus.Power * 100;
                            continue;
                        case FlagInt.EARTH_AURA:
                            m_nEarthResist += (uint)pStatus.Power;
                            continue;
                        case FlagInt.METAL_AURA:
                            m_nMetalResist += (uint)pStatus.Power;
                            continue;
                        case FlagInt.WOOD_AURA:
                            m_nWoodResist += (uint)pStatus.Power;
                            continue;
                        case FlagInt.WATER_AURA:
                            m_nWaterResist += (uint)pStatus.Power;
                            continue;
                        case FlagInt.FIRE_AURA:
                            m_nFireResist += (uint)pStatus.Power;
                            continue;
                    }
                }
            }

            if (QueryStatus(FlagInt.SUPER_SHIELD_HALO) != null)
            {
                m_nBlock += (uint) (QueryStatus(FlagInt.SUPER_SHIELD_HALO).Power*100);
            }

            m_nMinAttack = (int)(m_nMinAttack * (1 + (GetAtkGemEffect() / 100f)));
            m_nMaxAttack = (int)(m_nMaxAttack * (1 + (GetAtkGemEffect() / 100f)));
            m_nMagicAttack = (int)(m_nMagicAttack * (1 + (GetMAtkGemEffect() / 100f)));
            //m_nMagicDefense += m_nMagicDefenseBonus;

            if (nAddLife > ushort.MaxValue)
                nAddLife = ushort.MaxValue;
            if (nAddMana > ushort.MaxValue)
                nAddMana = ushort.MaxValue;

            MaxLife = (ushort)nAddLife;
            MaxMana = (ushort)nAddMana;
        }

        public int ItemPowerSum
        {
            get
            {
                int sum = 0;
                foreach (var item in Equipment.Items.Values)
                {
                    switch (item.Position)
                    {
                        case ItemPosition.HEADWEAR:
                        case ItemPosition.NECKLACE:
                        case ItemPosition.RING:
                        case ItemPosition.RIGHT_HAND:
                        case ItemPosition.LEFT_HAND:
                        case ItemPosition.BOOTS:
                        case ItemPosition.ARMOR:
                        case ItemPosition.ATTACK_TALISMAN:
                        case ItemPosition.DEFENCE_TALISMAN:
                        case ItemPosition.CROP:
                            {
                                bool bDouble = item.Position == ItemPosition.RIGHT_HAND
                                               && (item.IsBow()
                                                   || item.GetItemSubtype() == 421
                                                   || item.GetSort() == ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND);

                                if (bDouble
                                    && Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND)
                                    && item.GetSort() == ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND)
                                    bDouble = false;

                                switch (item.Type % 10)
                                {
                                    case 9:
                                        sum += bDouble ? 8 : 4;
                                        break;
                                    case 8:
                                        sum += bDouble ? 6 : 3;
                                        break;
                                    case 7:
                                        sum += bDouble ? 4 : 2;
                                        break;
                                    case 6:
                                        sum += bDouble ? 2 : 1;
                                        break;
                                }
                                break;
                            }
                    }
                }
                return sum;
            }
        }

        public void SendStatus(Character pTarget = null)
        {
            var pMsg = new MsgPlayerAttribInfo
            {
                Identity = Identity,
                Accuracy = (uint) m_nAgility,
                Agility = (uint) Dexterity,
                Bless = (uint) m_nBlessing,
                Block = m_nBlock,
                Breakthrough = m_nBreak,
                Counteraction = m_nCounteration,
                CriticalStrike = m_nCriticalStrike,
                SkillCriticalStrike = m_nSkillCritical,
                Detoxication = m_nDetoxication,
                Dodge = (uint) m_nDodge,
                DragonGemBonus = (uint) m_nDragonGem,
                EarthDefense = m_nEarthResist,
                FinalDefense = (uint) m_nAddFinalDefense,
                FinalMagicDamage = (uint) m_nAddFinalMagicAttack,
                FinalMagicDefense = (uint) m_nAddFinalMagicDefense,
                FinalPhysicalDamage = (uint) m_nAddFinalAttack,
                FireDefense = m_nFireResist,
                Immunity = m_nImmunity, // value * 100 ( 1000 == 10.00% )
                Life = MaxLife,
                Mana = MaxMana,
                MagicDefense = (uint) m_nMagicDefense,
                MagicalAttack = (uint) m_nMagicAttackEx,
                MaxAttack = (uint) m_nMaxAttackEx,
                MinAttack = (uint) m_nMinAttackEx,
                MetalDefense = m_nMetalResist,
                Penetration = m_nPenetration, // value * 100 ( 1000 == 10.00% )
                PhoenixGemBonus = (uint) m_nPhoenixGem,
                PhysicalDefense = (uint) m_nDefense,
                MagicDefenseBonus = (uint) m_nMagicDefenseBonus,
                TortoiseGemBonus = (uint) m_nTortoiseGem,
                WaterDefense = m_nWaterResist,
                WoodDefense = m_nWoodResist
            };
            if(pTarget != null) pTarget.Send(pMsg);
            else m_pOwner.Send(pMsg);
        }

        #endregion

        #region Game Action

        public Item TaskItem
        {
            get { return m_taskItem; }
            set { m_taskItem = value; }
        }

        //public Dictionary<uint, INextAction> NextActions
        //{
        //    get { return m_nextActions; }
        //    set { m_nextActions = value; }
        //}

        public uint LastItemResource
        {
            get { return m_lastItemResource; }
            set { m_lastItemResource = value; }
        }

        public uint LastUsedItem
        {
            get { return m_lastUsedItem; }
            set { m_lastUsedItem = value; }
        }

        public uint LastUsedItemTime
        {
            get { return m_lastUsedItemTime; }
            set { m_lastUsedItemTime = value; }
        }

        public uint LastUsedItemtype
        {
            get { return m_lastUsedItemtype; }
            set { m_lastUsedItemtype = value; }
        }

        public IScreenObject InteractingNpc
        {
            get { return m_interactingNpc; }
            set { m_interactingNpc = value; }
        }

        //public GameAction GameAction
        //{
        //    get { return m_pGameAction ?? (m_pGameAction = new GameAction(this)); }
        //}

        #endregion

        #region IScreenObject

        public Map Map
        {
            get { return m_pMap; }
            set { m_pMap = value; }
        }

        public uint MapIdentity
        {
            get { return m_dwMapIdentity; }
            set { m_dwMapIdentity = value; }
        }

        private uint m_oldMapId;
        private ushort m_usOldX, m_usOldY;

        public uint RecordMapIdentity
        {
            get { return m_oldMapId; }
            set { m_oldMapId = value; }
        }

        public ushort MapX
        {
            get { return m_usMapX; }
            set
            {
                m_usMapX = value;
                if (IsAlive && Map != null && !Map.IsRecordDisable())
                    m_dbUser.MapX = value;
                m_pPacket.MapX = value;
            }
        }

        public ushort RecordMapX
        {
            get { return m_usOldX; }
            set { m_usOldX = value; }
        }

        public ushort MapY
        {
            get { return m_usMapY; }
            set
            {
                m_usMapY = value;
                if (IsAlive && Map != null && !Map.IsRecordDisable())
                    m_dbUser.MapY = value;
                m_pPacket.MapY = value;
            }
        }

        public ushort RecordMapY
        {
            get { return m_usOldY; }
            set { m_usOldY = value; }
        }

        public short Elevation
        {
            get { return m_sElevation; }
            set { m_sElevation = value; }
        }

        public IScreenObject FindAroundRole(uint idRole)
        {
            return null;
        }

        #endregion

        #region Battle Handle

        public int GetInterAtkRate()
        {
            if (QueryTransformation != null)
                return (int) QueryTransformation.AttackHitRate;

            int nRate = USER_ATTACK_SPEED;
            int nRateR = 0, nRateL = 0;

            if (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND))
                nRateR = Equipment.Items[ItemPosition.RIGHT_HAND].Itemtype.AtkSpeed;
            if (Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND))
                nRateL = Equipment.Items[ItemPosition.LEFT_HAND].Itemtype.AtkSpeed;

            if (nRateR > 0 && nRateL > 0)
                nRate = (nRateR + nRateL)/2;
            else if (nRateR > 0)
                nRate = nRateR;
            else if (nRateL > 0)
                nRate = nRateL;

            if (QueryStatus(FlagInt.CYCLONE) != null)
            {
                nRate = Calculations.CutTrail(0,
                    Calculations.AdjustData(nRate, QueryStatus(FlagInt.CYCLONE).Power));
                if (IsPm)
                    Send(string.Format("atack speed+: {0}", nRate));
            }
            else if (QueryStatus(FlagInt.OBLIVION) != null)
            {
                nRate /= 2;
                if (IsPm)
                    Send(string.Format("atack speed+: {0}", nRate));
            }

            return nRate;
        }

        public void ProcessAutoAttack()
        {
            int nFightPause = m_nFightPause -  m_nDexterity;
            if (QueryStatus(FlagInt.FATAL_STRIKE) != null)
                nFightPause /= 7;
            //if (QueryStatus(FlagInt.CYCLONE) != null)
            //    nFightPause /= 5;
            if (BattleSystem != null && BattleSystem.IsActived()
                && BattleSystem.NextAttack(nFightPause)
                && (BattleSystem.QueryMagic() == null))// || !BattleSystem.IsActived()))
            {
                BattleSystem.ProcAttack_Hand2Hand();

                if (QueryStatus(FlagInt.FATAL_STRIKE) != null)
                    BattleSystem.ResetBattle();

                if (BattleSystem.IsActived())
                {
                    // TODO xp handle? energy? idk
                }
            }
        }

        #endregion

        #region IRole

        #region Battle Attributes

        public bool CheckScapegoat(IRole pTarget)
        {
            Magic scapegoat = Magics.Magics.Values.FirstOrDefault(x => x.Type == 6003);
            if (scapegoat != null && Scapegoat && scapegoat.IsReady())
            {
                return ProcessMagicAttack(scapegoat.Type, pTarget.Identity, pTarget.MapX, pTarget.MapY, 0);
            }
            return false;
        }

        public int PureBattlePower
        {
            get
            {
                int num = 0;
                int level = Level; //skill && Level > 130 ? 130 : Level;
                //byte nobility = (byte)(NobilityLevel > NobilityLevel.EARL && skill ? NobilityLevel.EARL : NobilityLevel);
                num += level + Metempsychosis * 5 + (int)NobilityRank;
                if (m_pMap.IsSkillMap())
                {
                    num = 130 + Metempsychosis*5 + 5;
                }

                if (Equipment != null)
                {
                    foreach (var item in Equipment.Items.Values.Where(x => x.Position <= ItemPosition.CROP))
                    {
                        if (item.IsBow()
                            || (item.GetItemSubtype() == 421 && !Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND))
                            || (item.GetSort() == ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND && !Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND)))
                            num += item.CalculateItemBattlePower(/*skill*/) * 2;
                        else
                            num += item.CalculateItemBattlePower(/*skill*/);
                    }
                }
                return num;
            }
        }

        public int BattlePower
        {
            get
            {
                if (m_pMap.IsSkillMap())
                    return 1;
                int num = 0;
                num += (Level + Metempsychosis*5 + (int) NobilityRank);

                if (Equipment != null)
                {
                    foreach (var item in Equipment.Items.Values.Where(x => x.Position <= ItemPosition.CROP))
                    {
                        if (item.IsBow()
                            || (item.GetItemSubtype() == 421 && !Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND))
                            ||
                            (item.GetSort() == ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND &&
                             !Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND)))
                            num += item.CalculateItemBattlePower()*2;
                        else
                            num += item.CalculateItemBattlePower();
                    }
                }

                if ((Mentor != null && Mentor.IsOnline) || (Family != null && Team != null))
                    num += SharedBattlePower;

                if (Syndicate != null)
                    num += SyndicateBattlePower;
                return num;
            }
        }

        public int MinAttack { get { return m_nMinAttack; } }
        public int MaxAttack { get { return m_nMaxAttack; } }
        public int MagicAttack { get { return m_nMagicAttack; } }
        public int Dodge { get { return m_nDodge; } }
        public int AttackHitRate { get { return ((m_nAgility + Agility) / 3); } }
        public int Dexterity { get { return m_nAttackHitRate + m_nDexterity; } }
        public int Defense { get { return m_nDefense; } }
        public int Defense2 { get { return Metempsychosis > 1 ? 7000 : Calculations.DEFAULT_DEFENCE2; } }
        public int MagicDefense { get { return m_nMagicDefense; } }
        public int MagicDefenseBonus { get { return m_nMagicDefenseBonus; } }
        public int AddFinalAttack { get { return m_nAddFinalAttack; } }
        public int AddFinalMagicAttack { get { return m_nAddFinalMagicAttack; } }
        public int AddFinalDefense { get { return m_nAddFinalDefense; } }
        public int AddFinalMagicDefense { get { return m_nAddFinalMagicDefense; } }
        public uint CriticalStrike { get { return m_nCriticalStrike; } }
        public uint SkillCriticalStrike { get { return m_nSkillCritical; } }
        public uint Breakthrough { get { return m_nBreak; } }
        public uint Penetration { get { return m_nPenetration; } }
        public uint Immunity { get { return m_nImmunity; } }
        public uint Counteraction { get { return m_nCounteration; } }
        public uint Block { get { return m_nBlock; } } 
        public uint Detoxication { get { return m_nDetoxication; } }
        public uint FireResistance { get { return m_nFireResist; } }
        public uint WaterResistance { get { return m_nWaterResist; } }
        public uint EarthResistance { get { return m_nEarthResist; } }
        public uint WoodResistance { get { return m_nWoodResist; } }
        public uint MetalResistance { get { return m_nMetalResist; } }

        #endregion

        public bool IsVirtuous()
        {
            return (Flag1 & KEEP_EFFECT_NOT_VIRTUOUS) == 0;
        }

        public int GetSizeAdd()
        {
            return 0;
        }

        public float GetReduceDamage()
        {
            return m_nBlessing;
        }

        public int AdjustMagicDamage(int nDamage)
        {
            nDamage = Calculations.MulDiv(nDamage, Defense2, Calculations.DEFAULT_DEFENCE2);
            return nDamage;
        }

        public bool CheckWeaponSubType(uint idItem, uint dwNum = 0)
        {
            if (idItem <= 0) return false;

            if (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND) && Equipment.Items[ItemPosition.RIGHT_HAND].GetItemSubtype() == idItem &&
                Equipment.Items[ItemPosition.RIGHT_HAND].Durability >= dwNum)
                return true;
            if (Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND) && Equipment.Items[ItemPosition.LEFT_HAND].GetItemSubtype() == idItem &&
                Equipment.Items[ItemPosition.LEFT_HAND].Durability >= dwNum)
                return true;

            ushort[] set1Hand = { 410, 420, 421, 430, 440, 450, 460, 480, 481, 490 };
            ushort[] set2Hand = { 510, 530, 540, 560, 561, 580 };
            ushort[] setSword = { 420, 421 };
            ushort[] setSpecial = { 601, 610 };

            if (idItem == 4 || idItem == 400)
            {
                if (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND))
                {
                    Item item = Equipment.Items[ItemPosition.RIGHT_HAND];
                    for (int i = 0; i < set1Hand.Length; i++)
                    {
                        if (item.GetItemSubtype() == set1Hand[i] && item.Durability >= dwNum)
                            return true;
                    }
                }
            }

            if (idItem == 5)
            {
                if (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND))
                {
                    Item item = Equipment.Items[ItemPosition.RIGHT_HAND];
                    for (int i = 0; i < set2Hand.Length; i++)
                    {
                        if (item.GetItemSubtype() == set2Hand[i] && item.Durability >= dwNum)
                            return true;
                    }
                }
            }

            if (idItem == 50) // arrow
            {
                if (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND) && Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND))
                {
                    Item item = Equipment.Items[ItemPosition.RIGHT_HAND];
                    Item arrow = Equipment.Items[ItemPosition.LEFT_HAND];
                    if (arrow.GetItemSubtype() == 1050 && arrow.Durability >= dwNum)
                        return true;
                }
            }

            if (idItem == 500)
            {
                if (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND) && Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND))
                {
                    Item item = Equipment.Items[ItemPosition.RIGHT_HAND];
                    if (item.GetItemSubtype() == idItem && item.Durability >= dwNum)
                        return true;
                }
            }

            if (idItem == 420)
            {
                if (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND))
                {
                    Item item = Equipment.Items[ItemPosition.RIGHT_HAND];
                    for (int i = 0; i < setSword.Length; i++)
                    {
                        if (item.GetItemSubtype() == setSword[i] && item.Durability >= dwNum)
                            return true;
                    }
                }
            }

            return false;
        }

        public bool Transform(uint dwLook, int nKeepSecs, bool bSynchro)
        {
            bool bBack = false;

            if (QueryTransformation != null)
            {
                QueryTransformation = null;
                ClearTransformation();
                bBack = true;
            }

            DbMonstertype pType;
            if (!ServerKernel.Monsters.TryGetValue(dwLook, out pType))
                return false;

            Transformation pTransform = new Transformation(this);
            if (pTransform.Create(pType))
            {
                QueryTransformation = pTransform;
                Transformation = (ushort)pTransform.Lookface;
                m_tTransformation = new TimeOut(nKeepSecs);
                m_tTransformation.Startup(nKeepSecs);
                if (bSynchro)
                    SynchroTransform();
            }
            else
            {
                pTransform = null;
            }

            StopMine();

            if (bBack)
                SynchroTransform();

            return false;
        }

        public bool SynchroTransform()
        {
            //var msg = new MsgUserAttrib { Identity = Identity };
            //msg.Append(ClientUpdateType.MESH, Lookface);
            //msg.Append(ClientUpdateType.MAX_HITPOINTS, MaxLife);
            //msg.Append(ClientUpdateType.HITPOINTS, Life);
            //Screen.Send(msg, true);
            UpdateClient(ClientUpdateType.MESH, Lookface, true);
            UpdateClient(ClientUpdateType.HITPOINTS, Life, true);
            UpdateClient(ClientUpdateType.MAX_HITPOINTS, MaxLife, true);
            return true;
        }

        public void Reborn(bool chgMap, bool isSpell = false)
        {
            if (IsAlive || (!CanRevive() && !isSpell))// || Status[FlagInt.SHACKLED] != null) // shackle in 5103 is funny
            {
                if (QueryStatus(FlagInt.GHOST) != null)
                {
                    DetachStatus(FlagInt.GHOST);
                }

                if (QueryStatus(FlagInt.DEAD) != null)
                {
                    DetachStatus(FlagInt.DEAD);
                }

                if (Transformation == 98 || Transformation == 99)
                    ClearTransformation();
                return;
            }

            BattleSystem.ResetBattle();
            DetachStatus(FlagInt.GHOST);
            DetachStatus(FlagInt.DEAD);
            ClearTransformation();

            Stamina = 100;
            Life = (ushort)MaxLife;
            Mana = MaxMana;

            if (chgMap || (!chgMap && !IsBlessed && !isSpell))
            {
                if (m_pMap.Identity == 7503 && ServerKernel.ScorePkEvent.State == EventState.RUNNING) // score pk war exception
                {
                    Point pos;
                    ServerKernel.ScorePkEvent.GetRebornPos(out pos);
                    ChangeMap((ushort) pos.X, (ushort) pos.Y, 7503);
                }
                else
                {
                    ChangeMap(m_dbUser.MapX, m_dbUser.MapY, m_dbUser.MapId);
                }
            }
            else
            {
                if (!isSpell && (m_pMap.IsPrisionMap()
                     || m_pMap.IsPkField()
                     || m_pMap.IsPkGameMap()
                     || m_pMap.IsSynMap()))
                {
                    ChangeMap(m_dbUser.MapX, m_dbUser.MapY, m_dbUser.MapId);
                }
                else
                {
                    ChangeMap(m_usMapX, m_usMapY, m_dwMapIdentity);
                }
            }

            m_pGameAction.ProcessAction(8000002, this, this, null, null);

            m_tRespawn.SetInterval(CHGMAP_LOCK_SECS);
            m_tRespawn.Update();
        }

        public void ClearTransformation()
        {
            //Lookface = Lookface % 10000000;
            Transformation = 0;
            QueryTransformation = null;
            m_tTransformation.Clear();
        }

        public bool CanRevive()
        {
            return !IsAlive && m_tRespawn.IsTimeOut() && m_tRevive.IsTimeOut() && QueryStatus(FlagInt.SHACKLED) == null;
        }

        public void AwardCtfScore(DynamicNpc pNpc, int nScore)
        {
            if (pNpc == null || nScore <= 0)
                return;

            if (Syndicate == null || SyndicateMember == null || pNpc.OwnerIdentity == Syndicate.Identity)
                return;

            Syndicate.AddSynWarScore(pNpc, (uint)nScore);
        }

        public void AwardSynWarScore(DynamicNpc pNpc, int nScore)
        {
            if (pNpc == null || nScore <= 0)
                return;

            if (Syndicate == null || SyndicateMember == null || pNpc.OwnerIdentity == Syndicate.Identity)
                return;

            int nAddProffer = Calculations.MulDiv(nScore, SYNWAR_PROFFER_PERCENT, 100);

            if (nAddProffer > 0)
                Syndicate.ChangeFunds(nAddProffer);

            int nAddMoney = Calculations.MulDiv(nScore, SYNWAR_MONEY_PERCENT, 100);
            if (nAddMoney > 0)
            {
                Syndicate syn = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Identity == pNpc.OwnerIdentity);
                if (syn != null)
                {
                    nAddMoney = (int)Calculations.CutOverflow(nAddMoney, (long)syn.SilverDonation);
                    syn.ChangeFunds(nAddMoney*-1);
                    AwardMoney((uint)nAddMoney);
                }
            }

            // if (Syndicate != null) 2015-02-18 removed for being redundant #felipe
            Syndicate.AddSynWarScore(pNpc, (uint)nScore);
        }

        public void CheckStatus()
        {
            for (int i = 1; i <= 128; i++)
            {
                if (Status[i] != null)
                {
                    var stats = Status[i];
                    if (!stats.IsValid && stats.Identity != FlagInt.DEAD && stats.Identity != FlagInt.GHOST)
                        DetachStatus(i);
                }
            }
        }

        public void SendGemEffect()
        {
            var setGem = new List<SocketGem>();

            foreach (var item in Equipment.Items.Values.Where(x => x.SocketOne != SocketGem.NO_SOCKET))
            {
                setGem.Add(item.SocketOne);
                if (item.SocketTwo != SocketGem.NO_SOCKET)
                    setGem.Add(item.SocketTwo);
            }

            int nGems = setGem.Count;
            if (nGems <= 0)
                return;

            string strEffect = "";
            switch (setGem[ThreadSafeRandom.RandGet(0, nGems)])
            {
                case SocketGem.SUPER_PHOENIX_GEM:
                    strEffect = "phoenix";
                    break;
                case SocketGem.SUPER_DRAGON_GEM:
                    strEffect = "goldendragon";
                    break;
                case SocketGem.SUPER_FURY_GEM:
                    strEffect = "fastflash";
                    break;
                case SocketGem.SUPER_RAINBOW_GEM:
                    strEffect = "rainbow";
                    break;
                case SocketGem.SUPER_KYLIN_GEM:
                    strEffect = "goldenkylin";
                    break;
                case SocketGem.SUPER_VIOLET_GEM:
                    strEffect = "purpleray";
                    break;
                case SocketGem.SUPER_MOON_GEM:
                    strEffect = "moon";
                    break;
            }

            SendEffect(strEffect, true);
        }

        public void SendWeaponMagic2(IRole pTarget = null)
        {
            if (Equipment == null)
                return;

            Item item = null;

            if (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND) && Equipment.Items[ItemPosition.RIGHT_HAND].Effect != ItemEffect.NONE)
                item = Equipment.Items[ItemPosition.RIGHT_HAND];
            if (Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND) && Equipment.Items[ItemPosition.LEFT_HAND].Effect != ItemEffect.NONE)
                if ((item != null && Calculations.ChanceCalc(50f)) || item == null)
                    item = Equipment.Items[ItemPosition.LEFT_HAND];

            if (item != null)
            {
                switch (item.Effect)
                {
                    case ItemEffect.LIFE:
                        {
                            if (!Calculations.ChanceCalc(15f))
                                return;
                            AddAttribute(ClientUpdateType.HITPOINTS, 310, true);
                            var msg = new MsgMagicEffect
                            {
                                Identity = m_pOwner.Identity,
                                SkillIdentity = 1005
                            };
                            msg.AppendTarget(Identity, 310, false, 0, 0);
                            Screen.Send(msg, true);
                            break;
                        }
                    case ItemEffect.MANA:
                        {
                            if (!Calculations.ChanceCalc(17.5f))
                                return;
                            AddAttribute(ClientUpdateType.MANA, 310, true);
                            var msg = new MsgMagicEffect
                            {
                                Identity = Identity,
                                SkillIdentity = 1195
                            };
                            msg.AppendTarget(Identity, 310, false, 0, 0);
                            Screen.Send(msg, true);
                            break;
                        }
                    case ItemEffect.POISON:
                        {
                            if (pTarget == null || pTarget is DynamicNpc)
                                return;

                            if (!Calculations.ChanceCalc(5f))
                                return;

                            var msg = new MsgMagicEffect
                            {
                                Identity = Identity,
                                SkillIdentity = 1320
                            };
                            msg.AppendTarget(pTarget.Identity, 210, true, 0, 0);
                            Screen.Send(msg, true);

                            pTarget.AttachStatus(pTarget, FlagInt.POISONED, 310, 2, 20, 0, Identity);
                            var special = InteractionEffect.NONE;
                            int nTargetLifeLost = Attack(pTarget, ref special);
                            SendDamageMsg(pTarget.Identity, (uint)nTargetLifeLost, special);

                            if (!pTarget.IsAlive)
                            {
                                int dwDieWay = 1;
                                if (nTargetLifeLost > pTarget.MaxLife / 3)
                                    dwDieWay = 2;

                                Kill(pTarget, IsBowman() ? 5 : (uint)dwDieWay);
                            }
                            break;
                        }
                }
            }
        }

        public bool IsCrime()
        {
            return QueryStatus(FlagInt.BLUE_NAME) != null;
        }

        public bool IsPker()
        {
            return QueryStatus(FlagInt.BLACK_NAME) != null;
        }

        public bool IsMentor(Character pTarget)
        {
            return Mentor != null && Mentor.Identity == pTarget.Identity;
        }

        public bool IsApprentice(Character pTarget)
        {
            return Apprentices.ContainsKey(pTarget.Identity);
        }

        public bool IsMate(Character target)
        {
            return target.Mate == Name && Mate == target.Name;
        }

        public bool IsTradePartner(Character pTarget)
        {
            return TradePartners.ContainsKey(pTarget.Identity) && TradePartners[pTarget.Identity].IsActive;
        }

        public bool IsNewbie()
        {
            return Level < 70;
        }

        public bool IsMarried()
        {
            return Mate != "None";
        }

        public bool IsBlinking()
        {
            return QueryStatus(FlagInt.BLUE_NAME) != null;
        }

        public bool IsAlive
        {
            get { return Life > 0; }
        }

        public bool IsPlayer()
        {
            return true;
        }

        public bool IsMonster()
        {
            return false;
        }

        public bool IsNpc()
        {
            return false;
        }

        public bool IsCallPet()
        {
            return false;
        }

        public bool IsDynaNpc()
        {
            return false;
        }

        public bool IsDynaMonster()
        {
            return false;
        }

        public bool IsBeAttackable()
        {
            return true;
        }

        public bool IsAttackable(IRole pTarget)
        {
            // obj is being attacked target is attacker
            if (pTarget == null)
                return false;

            if (!IsAlive || !m_tRespawn.IsTimeOut() || !LoginComplete || (m_pMap.IsPkDisable() && pTarget is Character))
                return false;

            ArenaMatch match = ServerKernel.ArenaQualifier.FindUser(pTarget.Identity);
            ArenaMatch mine = ServerKernel.ArenaQualifier.FindUser(Identity);
            if (m_pMap.Identity > 900000
                && match != null
                && !match.IsAttackEnable)
                return false;
            if (m_pMap.Identity > 900000
                && (match == null || mine == null))
                return false;
            return true;
        }

        public void BeKill(IRole pRole)
        {
            BattleSystem.ResetBattle();
            BattleSystem.DestroyAutoAttack();
            if (BattleSystem.QueryMagic() != null)
                BattleSystem.QueryMagic().AbortMagic(true);

            m_tDoGhost.Startup(3);
            m_tDoGhost.Update();
            m_tRevive.Startup(20);
            m_tRevive.Update();

            DetachStatus(FlagInt.BLUE_NAME);
            DetachAllStatus(this);
            AttachStatus(this, FlagInt.DEAD, 0, 20, 0, 0);
            AttachStatus(this, FlagInt.GHOST, 0, 20, 0, 0);

            var pos = new Point(MapX, MapY);

            uint _map = 0;
            if (Map.GetRebornMap(ref _map, ref pos))
                SetRecordPos(_map, (ushort)pos.X, (ushort)pos.Y);


            if (Map.IsPkField() || Map.IsSynMap())
            {
                if (!Map.IsDeadIsland())
                    Inventory.RandDropItem(1, 30);

                if (Map.IsSynMap() && !Map.IsWarTime())
                    SetRecordPos(1002, 438, 398);
                return;
            }

            if (Map.IsPrisionMap())
            {
                if (!Map.IsDeadIsland())
                {
                    int nChance = Math.Min(90, 20 + PkPoints / 2);
                    Inventory.RandDropItem(3, nChance);
                }
                return;
            }

            if (pRole == null)
                return;

            if (!Map.IsDeadIsland())
            {
                if (pRole is Character && pRole != this)
                    CreateEnemy(pRole as Character);

                int nChance = 0;
                if (PkPoints < 30)
                    nChance = 10 + ThreadSafeRandom.RandGet(40);
                else if (PkPoints < 100)
                    nChance = 50 + ThreadSafeRandom.RandGet(50);
                else
                    nChance = 100;

                int nItems = Inventory.Items.Count;
                int nDropItem = (nItems * nChance) / 100;

                Inventory.RandDropItem(nDropItem);

                Character pTarget = pRole as Character;
                if (pTarget != null && pTarget != this && !pTarget.IsBlessed && IsBlessed)
                {
                    if (pTarget.QueryStatus(FlagInt.CURSED) == null)
                        pTarget.AttachStatus(pTarget, FlagInt.CURSED, 0, 300, 0, 0, Identity);
                    else
                        pTarget.QueryStatus(FlagInt.CURSED).IncTime(300, 3600);
                }

                if (pTarget != null && pTarget != this)
                {
                    int nLossPercent = 0;
                    if (PkPoints < 30)
                        nLossPercent = 1;
                    else if (PkPoints < 100)
                        nLossPercent = 2;
                    else nLossPercent = 3;

                    long nLevExp = Experience;

                    long nExpLost = nLevExp * (nLossPercent / 100);
                    if (nExpLost > 0)
                    {
                        AddAttribute(ClientUpdateType.EXPERIENCE, -1 * nExpLost, true);
                        pTarget.AddAttribute(ClientUpdateType.EXPERIENCE, nExpLost, true);
                    }

                    int nLevDiff = pTarget.Level - Level;
                    if (Syndicate != null && pTarget.Syndicate != null/* && Syndicate.IsHostile((ushort) pTarget.Syndicate.Identity)*/
                        && nLevDiff <= 5)
                    {
                        Syndicate.Send(string.Format("The {0} {1} has been killed by {2} {3} of {4} in {5}.",
                            SyndicateMember.GetRankName(), Name, pTarget.Name, pTarget.SyndicateMember.GetRankName(), pTarget.Syndicate.Name,
                            Map.Name));
                        pTarget.Syndicate.Send(string.Format("Our {0} {1} has killed {2} {3} of {4} in {5}",
                            pTarget.SyndicateMember.GetRankName(), pTarget.Name, Name, SyndicateMember.GetRankName(),
                            SyndicateName, Map.Name));

                        SyndicateMember.DecreasePkDonation(3);
                        pTarget.SyndicateMember.IncreasePkDonation(3);
                    }
                }

                // detain
                //if (pTarget != null && pTarget != this && ((PkPoints > 29 && Calculations.ChanceCalc(30f)) || PkPoints > 100))
                //{
                //    pTarget.DetainEquipment(this);
                //    if (PkPoints > 299)
                //        pTarget.DetainEquipment(this);
                //}

                if (PkPoints > 99 && pTarget != null)
                {
                    SetRecordPos(6000, 31, 72);
                    ChangeMap(31, 72, 6000, false, true);
                    ServerKernel.SendMessageToAll(string.Format(ServerString.STR_GOTO_JAIL_S, pTarget.Name, Name), ChatTone.TALK);
                }
            }
            else if (pRole is Character && Map.IsDeadIsland())
            {
                if (pRole != this)
                    CreateEnemy(pRole as Character);
            }
            else
            {
                // TODO handle kill by monster
                if ((pRole is Monster))
                {
                    Monster pMonster = pRole as Monster;
                    if (pMonster.IsGuard() || pMonster.IsPkKiller())
                    {
                        if (PkPoints > 99)
                        {
                            SetRecordPos(6000, 31, 72);
                            ChangeMap(31, 72, 6000, false, true);
                            ServerKernel.SendMessageToAll(string.Format(ServerString.STR_GOTO_JAIL, Name), ChatTone.TALK);
                        }
                    }
                }
            }

            m_pGameAction.ProcessAction(8000001, this, this, null, null);

            if (Scapegoat)
            {
                Scapegoat = false;
                MsgInteract pMsg = new MsgInteract();
                pMsg.Action = InteractionType.ACT_ITR_COUNTER_KILL_SWITCH;
                pMsg.TargetIdentity = pMsg.EntityIdentity = pRole.Identity;
                pMsg.Damage = 0;
                pMsg.CellX = pRole.MapX;
                pMsg.CellY = pRole.MapY;
                Send(pMsg);
            }
        }

        public void Kill(IRole pTarget, ulong dwDieWay)
        {
            if (pTarget == null)
                return;

            ushort usRemainingTime = 0;

            Character pTargetUser = null;
            if (pTarget is Character)
            {
                pTargetUser = pTarget as Character;

                var msg = new MsgInteract
                {
                    Action = InteractionType.ACT_ITR_KILL,
                    EntityIdentity = Identity,
                    TargetIdentity = pTarget.Identity,
                    CellX = pTarget.MapX,
                    CellY = pTarget.MapY,
                    MagicType = (ushort)dwDieWay,
                    MagicLevel = usRemainingTime
                };
                Screen.Send(msg, true);
            }

            //if (BattleSystem.QueryMagic() != null)
            //    BattleSystem.QueryMagic().UserKillTarget(pTarget);

            if (pTargetUser == null)
            {
                if (CaptchaBox == null && m_dwCount >= 2000 && Calculations.ChanceCalc(0.5f))
                {
                    CaptchaBox = new CaptchaBox();
                    CaptchaBox.Generate();
                    CaptchaBox.Send(this);
                    m_dwCount = 0;
                }

                if (QueryStatus(FlagInt.CYCLONE) != null || QueryStatus(FlagInt.SUPERMAN) != null)
                {
                    m_nKoCount += 1;
                    var status = QueryStatus(FlagInt.CYCLONE);
                    if (status == null)
                    {
                        status = QueryStatus(FlagInt.SUPERMAN);
                    }
                    if (status != null)
                    {
                        status.IncTime(700, 30000);
                    }
                }
                if (QueryStatus(FlagInt.OBLIVION) != null)
                {
                    if (m_nKoCount < 31)
                    {
                        m_nKoCount += 1;
                    }
                    else
                    {
                        DetachStatus(FlagInt.OBLIVION);
                        AwardOblivion();
                    }
                }
                XpPoints += 1;
                if (CaptchaBox == null)
                    m_dwCount += 1;
                pTarget.BeKill(this);
                return;
            }
            else
            {
                m_pGameAction.ProcessAction(8000000, this, this, null, null);

                if (ServerKernel.ScorePkEvent.IsParticipating(Identity))
                {
                    ServerKernel.ScorePkEvent.AlterPoints(Identity, 15);
                }
                else if (ServerKernel.CaptureTheFlag.IsRunning && ServerKernel.CaptureTheFlag.IsInside(Identity))
                {
                    ServerKernel.CaptureTheFlag.AddPoints(this, 1);
                }

                if (ServerKernel.ArenaQualifier.IsInsideMatch(Identity))
                {
                    ArenaMatch pMatch = ServerKernel.ArenaQualifier.FindUser(Identity);
                    if (pMatch != null && pMatch.IsRunning() && pMatch.HasStarted())
                    {
                        pMatch.Finish(this, pTargetUser, false);
                    }
                }

                if (Level-pTarget.Level <= 5)
                    PkExploit.Kill(pTargetUser);
            }

            if (Magics.GetSort() != MagicSort.MAGICSORT_ACTIVATESWITCH)
                ProcessPk(pTargetUser);

            pTarget.BeKill(this);
        }

        public void ResetOblivion()
        {
            if (QueryStatus(FlagInt.OBLIVION) != null)
                DetachStatus(FlagInt.OBLIVION);
            m_nKoCount = 0;
            m_lAccumulateExp = 0;
        }

        public void AwardOblivion(bool bFull = true)
        {
            if (m_lAccumulateExp == 0)
                return;
            m_nKoCount = 0;
            if (bFull)
                AwardExperience(m_lAccumulateExp * 2, true);
            else
                AwardExperience(m_lAccumulateExp, true);
        }

        public void ProcessPk(Character pTargetUser)
        {
            if (!Map.IsPkField() && !Map.IsPkGameMap() && !Map.IsSynMap() && !Map.IsPrisionMap())
            {
                // Innocent kill
                if (!Map.IsDeadIsland() && pTargetUser.Status[FlagInt.BLUE_NAME] == null &&
                    pTargetUser.PkPoints < 100)
                {
                    int nPkValue = 10;
                    if (pTargetUser.Syndicate != null && Syndicate != null && pTargetUser.Syndicate.IsHostile((ushort)Syndicate.Identity))
                        nPkValue = 3;
                    else if (ContainsEnemy(pTargetUser.Identity))
                        nPkValue = 5;

                    if (pTargetUser.PkPoints > 30)
                        nPkValue /= 2;

                    PkPoints += (ushort)nPkValue;
                    SetCrimeStatus(60);

                    if (PkPoints > 19 && PkPoints < 100)
                    {
                        Send(ServerString.STR_KILLING_TO_MUCH);
                    }
                    else
                    {
                        Send(ServerString.STR_KILLING_TO_MUCH);
                    }
                }
            }
        }

        public void SetCrimeStatus(uint nTime, int flag = FlagInt.BLUE_NAME)
        {
            AttachStatus(this, flag, 0, (int)nTime, 0, 0);
        }

        public void SendDamageMsg(uint idTarget, uint nDamage, InteractionEffect special)
        {
            if (IsBowman())
            {
                Owner.Screen.Send(new MsgInteract
                {
                    EntityIdentity = Identity,
                    TargetIdentity = idTarget,
                    Action = InteractionType.ACT_ITR_SHOOT,
                    CellX = MapX,
                    CellY = MapY,
                    Data = nDamage,
                    ActivationType = special,
                    ActivationValue = nDamage
                }, true);
            }
            else if (QueryStatus(FlagInt.FATAL_STRIKE) != null)
            {
                Owner.Screen.Send(new MsgInteract
                {
                    EntityIdentity = Identity,
                    TargetIdentity = idTarget,
                    Action = InteractionType.ACT_ITR_FATAL_STRIKE,
                    CellX = MapX,
                    CellY = MapY,
                    Data = nDamage,
                    ActivationType = special,
                    ActivationValue = nDamage
                }, true);
            }
            else
            {
                MsgInteract pMsg = new MsgInteract
                {
                    EntityIdentity = Identity,
                    TargetIdentity = idTarget,
                    Action = InteractionType.ACT_ITR_ATTACK,
                    CellX = MapX,
                    CellY = MapY,
                    Data = nDamage,
                    ActivationType = special,
                    ActivationValue = nDamage
                };
                Owner.Screen.Send(pMsg, true);
            }
        }

        public void AwardBattleExp(int nExp, bool bGemEffect)
        {
            if (Metempsychosis == 2)
                nExp /= 3;

            float mult = 1f;
            if (IsBlessed)
                mult += .3f;

            nExp = (int)(nExp * mult);

            if (nExp == 0)
                return;

            if (nExp < 0)
            {
                AddAttribute(ClientUpdateType.EXPERIENCE, nExp, true);
                return;
            }

            if (bGemEffect)
            {
                int nAddPercent = GetExpGemEffect();
                nExp += nExp * (nAddPercent / 100);

                if (nAddPercent > 0 && IsPm)
                    Send("got gem exp add percent: " + nAddPercent);
            }

            if (Level >= ServerKernel.MAX_UPLEVEL)
                return;

            if (Level >= 120)
                nExp /= 2;

            if (IsPm)
                Send("got battle exp: " + nExp);

            AwardExperience(nExp);

            if (Mentor != null && Level >= 30)
            {
                AddMentorExperience((uint)(nExp * 0.4f));
            }
        }

        public bool IsInFan(Point pos, Point posSource, int nRange, int nWidth, Point posCenter)
        {
            return Calculations.IsInFan(pos, posSource, nRange, nWidth, posCenter);
        }

        public int AdjustDefense(int nRawDef)
        {
            return nRawDef;
        }

        public int AdjustWeaponDamage(int nDamage)
        {
            nDamage = Calculations.MulDiv(nDamage, Defense2, Calculations.DEFAULT_DEFENCE2);

            int type1 = 0, type2 = 0;
            if (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND))
                type1 = Equipment.Items[ItemPosition.RIGHT_HAND].GetItemSubtype();
            if (Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND))
                type2 = Equipment.Items[ItemPosition.LEFT_HAND].GetItemSubtype();

            if (type1 > 0 && WeaponSkill.Skills.ContainsKey((ushort)type1) && WeaponSkill.Skills[(ushort)type1].Level > 12)
            {
                nDamage = (int)(nDamage * (1 + ((20 - WeaponSkill.Skills[(ushort)type1].Level) / 100f)));
            }
            else if (type2 > 0 && WeaponSkill.Skills.ContainsKey((ushort)type2) && WeaponSkill.Skills[(ushort)type2].Level > 12)
            {
                nDamage = (int)(nDamage * (1 + ((20 - WeaponSkill.Skills[(ushort)type2].Level) / 100f)));
            }

            return nDamage;
        }

        public int GetAttackRange(int nTargetSizeAdd)
        {
            if (QueryStatus(FlagInt.FATAL_STRIKE) != null)
            {
                Magic fatal;
                if (Magics.Magics.TryGetValue(6011, out fatal))
                {
                    return (int)fatal.Distance;
                }
            }

            int nRange = 1;
            int nRangeR = 0, nRangeL = 0;
            if (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND) && Equipment.Items[ItemPosition.RIGHT_HAND].IsWeapon()) // not arrow
                nRangeR = Equipment.Items[ItemPosition.RIGHT_HAND].Itemtype.AtkRange;
            if (Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND) && Equipment.Items[ItemPosition.LEFT_HAND].IsWeapon()
                && !Equipment.Items[ItemPosition.LEFT_HAND].IsShield() 
                && !Equipment.Items[ItemPosition.LEFT_HAND].IsArrow())
                nRangeL = Equipment.Items[ItemPosition.LEFT_HAND].Itemtype.AtkRange;

            if (nRangeR > 0
                && nRangeL > 0)
                nRange = (nRangeR + nRangeL) / 2;
            else if (nRangeR > 0)
                nRange = nRangeR;
            else if (nRangeL > 0)
                nRange = nRangeL;

            nRange += (GetSizeAdd() + nTargetSizeAdd + 1) / 2;

            return nRange + 1;
        }

        public int GetDistance(IScreenObject pObj)
        {
            return (int) Calculations.GetDistance(MapX, MapY, pObj.MapX, pObj.MapY);
        }

        public int Attack(IRole pTarget, ref InteractionEffect special)
        {
            if (pTarget == null)
                return 0;

            DynamicNpc pNpc = null;
            if (pTarget is DynamicNpc)
                pNpc = pTarget as DynamicNpc;

            int nDamage = BattleSystem.CalcPower(0, this, pTarget, ref special);

            if (RemainingLuckyTime > 0 && Calculations.ChanceCalc(10f))
            {
                nDamage *= 2;
                SendEffect("LuckyGuy", true);
                Send("Lucky! Your damage has been multiplied by 2.");
            }
            else if (pTarget is Character)
            {
                Character pRole = pTarget as Character;
                if (pRole.RemainingLuckyTime > 0 && Calculations.ChanceCalc(10f))
                {
                    nDamage = 1;
                    pRole.SendEffect("LuckyGuy", true);
                    pRole.Send("Lucky! The damage dealt to you has been filtered.");
                }
            }

            if (pTarget is Character && pTarget.QueryStatus(FlagInt.VORTEX) != null)
            {
                nDamage = 1;
            }

            var nLoseLife = (int)Calculations.CutOverflow(nDamage, pTarget.Life);

            //if (nLoseLife > 0)
            //    pTarget.AddAttribute(ClientUpdateType.HITPOINTS, -1 * nLoseLife, true);

            if (pNpc != null && pNpc.IsSynFlag() && pNpc.IsSynMoneyEmpty())
                nDamage *= SYNWAR_NOMONEY_DAMAGETIMES;

            if (nDamage > 0)
                if (Calculations.ChanceCalc(10))
                    SendGemEffect();

            //pTarget.BeAttack(0, this, nDamage, true);

            if ((!IsEvil()) && Map.IsDeadIsland() || (pTarget is Monster && (pTarget as Monster).IsGuard()))
            {
                SetCrimeStatus(15);
            }

            return nDamage;
        }

        public int AdjustExperience(IRole pTarget, int nRawExp, bool bNewbieBonusMsg)
        {
            if (pTarget == null) return 0;
            int nExp = nRawExp;
            nExp = BattleSystem.AdjustExp(nExp, Level, pTarget.Level);
            DynamicNpc pNpc = null;
            if (pTarget is DynamicNpc)
                pNpc = pTarget as DynamicNpc;

            if (pNpc != null && pNpc.IsGoal()) // TODO handle monsters
            {
                // TODO handle tutor
            }

            if (m_nRainbowGem > 0)
                nExp = (int)(nExp * (1 + m_nRainbowGem / 100f));

            return nExp;
        }

        public int CalculateFightRate()
        {
            return m_nFightPause = GetInterAtkRate();
        }

        public int GetExpGemEffect()
        {
            return m_nRainbowGem;
        }

        public int GetAtkGemEffect()
        {
            return m_nDragonGem;
        }

        public int GetMAtkGemEffect()
        {
            return m_nPhoenixGem;
        }

        public int GetSkillGemEffect()
        {
            return m_nMoonGem;
        }

        public int GetTortoiseGemEffect()
        {
            return Math.Min(72, m_nTortoiseGem);
        }

        public bool IsFarWeapon()
        {
            return GetAttackRange(GetSizeAdd()) > 2;
        }

        public bool AutoSkillAttack(IRole pTarget)
        {
            // TODO Handle pets, mobs,w/eeeeee
            foreach (var magic in Magics.Magics.Values)
            {
                float percent = magic.Percent;
                if (magic.Type == 10490 && pTarget is Character) // triple exception
                    percent = 100/3f;
                if (magic.AutoActive > 0
                    /* TODO Check if transformed, if so, return */
                    && (magic.WeaponSubtype == 0
                        || CheckWeaponSubType(magic.WeaponSubtype, magic.UseItemNum))
                    && Calculations.ChanceCalc(percent))
                {
                    return ProcessMagicAttack(magic.Type, pTarget.Identity, pTarget.MapX, pTarget.MapY, magic.AutoActive);
                }
            }
            return false;
        }

        public bool IsWing()
        {
            return QueryStatus(FlagInt.FLY) != null;
        }

        public bool IsBowman()
        {
            return (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND) && Equipment.Items[ItemPosition.RIGHT_HAND].Type / 1000 == 500);
            //return false;
        }   

        public bool IsSimpleMagicAtk()
        {
            return true;
        }

        public bool IsGoal()
        {
            return false;
        }

        public bool IsEvil()
        {
            return PkPoints > 99 || Status[FlagInt.BLUE_NAME] != null;
        }

        public bool BeAttack(int bMagic, IRole pRole, int nPower, bool bReflectEnable)
        {
            if (pRole == null)
                return false;

            StopMine();

            if ((LastProfession == 25 || FirstProfession == 25) && nPower > 0 && bReflectEnable)
            {
                if (Calculations.ChanceCalc(15f))
                {
                    nPower = Math.Min(1700, nPower);
                    pRole.BeAttack(bMagic, pRole, nPower, false);
                    var msg = new MsgInteract
                    {
                        Action = InteractionType.ACT_ITR_REFLECT_MAGIC,
                        EntityIdentity = Identity,
                        TargetIdentity = pRole.Identity,
                        CellX = MapX,
                        CellY = MapY,
                        Data = (uint)nPower
                    };
                    Owner.Screen.Send(msg, true);
                    nPower = 0;
                }
            }

            IStatus pStatus = null;
            StatusInfoStruct pInfo = default(StatusInfoStruct);
            if ((pStatus = QueryStatus(FlagInt.AZURE_SHIELD)) != null
                && pStatus.GetInfo(ref pInfo)
                && bReflectEnable)
            {
                uint dwReduceShield = 0u;
                if (pStatus.Power > nPower)
                {
                    pStatus.Power -= nPower;
                    dwReduceShield = (uint) nPower;
                    nPower = 0;
                    UpdateAzureShield(pInfo.Seconds, pStatus.Power, pStatus.Level);
                }
                else
                {
                    dwReduceShield = (uint) pStatus.Power;
                    nPower += pStatus.Power*-1;
                    pStatus.Power = 0;
                    DetachStatus(FlagInt.AZURE_SHIELD);
                }
                MsgInteract pMsg = new MsgInteract
                {
                    EntityIdentity = Identity,
                    TargetIdentity = Identity,
                    CellX = MapX,
                    CellY = MapY,
                    Action = InteractionType.ACT_ITR_AZURE_DMG,
                    Data = dwReduceShield
                };
                Screen.Send(pMsg, true);
            }

            if (nPower > 0)
            {
                var nLoseLife = (int)Calculations.CutOverflow(Life, nPower);
                //if (!CheckScapegoat(pRole))
                {
                    AddAttribute(ClientUpdateType.HITPOINTS, -1 * nLoseLife, true);
                    //if (pRole.IsPlayer())
                    //{
                    //    ServerKernel.Log.SaveLog(
                    //        string.Format(
                    //            "Attacker(bp:{0},id{1},name[{2}]) Attacked(bp:{3},id{4},name[{5}]) Damage:{6}",
                    //            pRole.BattlePower, pRole.Identity, pRole.Name, BattlePower, Identity, Name, nPower),
                    //        false, "DamageLog", LogType.DEBUG);
                    //}
                }

                if (ServerKernel.ArenaQualifier.IsInsideMatch(Identity))
                {
                    var pMatch = ServerKernel.ArenaQualifier.FindUser(Identity);
                    if (pMatch != null)
                    {
                        if (pMatch.Identity1 == Identity)
                            pMatch.Points2 += (uint)nLoseLife;
                        else
                            pMatch.Points1 += (uint)nLoseLife;
                        pMatch.SendToMap();
                    }
                } 
                else if (ServerKernel.SyndicateScoreWar.IsRunning && MapIdentity == 7600 && pRole is Character)
                {
                    ServerKernel.SyndicateScoreWar.AwardPoints(pRole as Character, (uint) nLoseLife);
                }
            }

            if (!pRole.IsAlive/* && pRole.IsAttackable(this)*/)
                pRole.BeKill(this);

            if (!Map.IsTrainingMap())
                DecEquipmentDurability(true, bMagic, (ushort)((nPower > MaxLife / 4) ? 2 : 1));

            if (Action == EntityAction.SIT)
                Stamina /= 2;

            return true;
        }

        public bool SetAttackTarget(IRole pTarget)
        {
            if (BattleSystem == null)
                return false;

            if (pTarget == null)
            {
                BattleSystem.ResetBattle();
                return true;
            }

            if (pTarget is Character && Map.QueryRegion(RegionType.REGION_PK_PROTECTED, pTarget.MapX, pTarget.MapY))
                return false;

            if (!pTarget.IsAttackable(this))
                return false;

            if (pTarget.IsWing() && !IsWing() && !(IsBowman() || IsSimpleMagicAtk()))
                return false;

            DynamicNpc pNpc = null;
            if (pTarget is DynamicNpc)
                pNpc = pTarget as DynamicNpc;

            if (pNpc != null && pNpc.IsGoal() && (pNpc.Level > Level || pNpc.Kind != 21))
                return false;

            if (GetDistance(pTarget as IScreenObject) > GetAttackRange(0) + 1)
            {
                BattleSystem.ResetBattle();
                return false;
            }

            BattleSystem.CreateBattle(pTarget.Identity);

            if (BattleSystem.QueryMagic() != null)
                BattleSystem.QueryMagic().AbortMagic(true);

            CalculateFightRate();

            return true;
        }

        public bool IsImmunity(IRole pRole)
        {
            if (pRole == null) return true;

            if (pRole.Identity == Identity)
                return true;

            if (pRole is DynamicNpc)
                return !pRole.IsAlive && !pRole.IsAttackable(this);

            /* else if (IsMonster()) */

            if (pRole is Character)
            {
                switch (PkMode)
                {
                    case PkModeType.PK_MODE:
                        return false;
                    case PkModeType.TEAM:
                        {
                            if (Team != null)
                                if (Team.IsTeamMember(pRole.Identity)) return true;

                            if (ContainsFriend(pRole.Identity))
                                return true;

                            var pUser = pRole as Character;
                            if (Mate == pUser.Name)
                                return true;

                            if (Syndicate != null)
                            {
                                if (Syndicate.Members.ContainsKey(pRole.Identity)) return true;
                                if (pUser.Syndicate != null && Syndicate.IsFriendly((ushort)pUser.Syndicate.Identity)) return true;
                            }

                            return false;
                        }
                    case PkModeType.CAPTURE:
                        return !pRole.IsEvil();
                    case PkModeType.PEACE:
                        return true;
                }
            }
            if (pRole is Monster)
            {
                Monster obj = pRole as Monster;
                switch (PkMode)
                {
                    case PkModeType.TEAM:
                    case PkModeType.CAPTURE:
                    case PkModeType.PEACE:
                        {
                            if (obj.IsGuard())
                                return true;
                            return false;
                        }
                    case PkModeType.PK_MODE:
                        {
                            return false;
                        }
                }
                return false;
            }
            ServerKernel.Log.SaveLog("failed to handle Character.IsImmunity(IScreenObject)", false, LogType.WARNING);
            return true;
        }

        public bool CheckCrime(IRole pTarget)
        {
            if (pTarget == null) return false;
            if (!pTarget.IsEvil() && !pTarget.IsMonster())
            {
                if (Map == null ||
                    (!Map.IsTrainingMap() && !Map.IsDeadIsland() && !Map.IsSynMap() && !Map.IsPrisionMap() &&
                     !Map.IsFamilyMap() && !Map.IsPkGameMap() && !Map.IsPkField()))
                    SetCrimeStatus(15);
            }
            return false;
        }

        public bool AdditionMagic(int nLifeLost, int nDamage)
        {
            return true;
        }

        public bool AddAttribute(ClientUpdateType type, long data, bool bSynchro)
        {
            switch (type)
            {
                case ClientUpdateType.HITPOINTS:
                    {
                        if (Life + data <= 0)
                            Life = 0;
                        else
                            Life = (ushort)Math.Min(MaxLife, Life + data);
                        return true;
                    }
                case ClientUpdateType.MANA:
                    {
                        if (Mana + data < 0)
                            return false;
                        Mana = (ushort)Math.Min(MaxMana, Mana + data);
                        return true;
                    }
                case ClientUpdateType.STAMINA:
                    {
                        if (Stamina + data < 0)
                            return false;
                        Stamina = (byte)Math.Min(100, Stamina + data);
                        return true;
                    }
                case ClientUpdateType.XP_CIRCLE:
                    {
                        if (XpPoints + data < 0)
                            return false;
                        XpPoints = (byte)Math.Min(100, XpPoints + data);
                        return true;
                    }
            }
            return false;
        }

        public bool DetachWellStatus(IRole pRole)
        {
            for (int i = 1; i < 128; i++)
            {
                if (Status[i] != null)
                    if (IsWellStatus(i))
                        DetachStatus(i);
            }
            return true;
        }

        public bool DetachBadlyStatus(IRole pRole)
        {
            for (int i = 1; i < 128; i++)
            {
                if (Status[i] != null)
                    if (IsBadlyStatus(i))
                        DetachStatus(i);
            }
            return true;
        }

        public bool DetachAllStatus(IRole pRole)
        {
            DetachBadlyStatus(this);
            DetachWellStatus(this);
            return true;
        }

        public bool IsWellStatus(int stts)
        {
            switch (stts)
            {
                case FlagInt.RIDING:
                case FlagInt.FULL_INVIS:
                case FlagInt.LUCKY_DIFFUSE:
                case FlagInt.STIG:
                case FlagInt.SHIELD:
                case FlagInt.STAR_OF_ACCURACY:
                case FlagInt.START_XP:
                case FlagInt.INVISIBLE:
                case FlagInt.SUPERMAN:
                case FlagInt.PARTIALLY_INVISIBLE:
                case FlagInt.LUCKY_ABSORB:
                case FlagInt.VORTEX:
                case FlagInt.POISON_STAR:
                case FlagInt.FLY:
                case FlagInt.FATAL_STRIKE:
                case FlagInt.AZURE_SHIELD:
                case FlagInt.SUPER_SHIELD_HALO:
                case FlagInt.CARYING_FLAG:
                case FlagInt.EARTH_AURA:
                case FlagInt.FEND_AURA:
                case FlagInt.FIRE_AURA:
                case FlagInt.METAL_AURA:
                case FlagInt.TYRANT_AURA:
                case FlagInt.WATER_AURA:
                case FlagInt.WOOD_AURA:
                case FlagInt.OBLIVION:
                case FlagInt.CTF_FLAG:
                    return true;
            }
            return false;
        }

        public bool IsBadlyStatus(int stts)
        {
            switch (stts)
            {
                case FlagInt.POISONED:
                case FlagInt.CONFUSED:
                case FlagInt.ICE_BLOCK:
                case FlagInt.HUGE_DAZED:
                case FlagInt.DAZED:
                case FlagInt.SHACKLED:
                case FlagInt.TOXIC_FOG:
                    return true;
            }
            return false;
        }

        public bool IsWellStatus0(ulong nStatus)
        {
            switch ((Effect0)nStatus)
            {
                case Effect0.FULL_INVIS:
                case Effect0.LUCKY_TIME:
                case Effect0.STIG:
                case Effect0.SHIELD:
                case Effect0.STAR_OF_ACCURACY:
                case Effect0.START_XP:
                case Effect0.INVISIBLE:
                case Effect0.SUPERMAN:
                case Effect0.PARTIALLY_INVISIBLE:
                case Effect0.PRAY:
                    return true;
            }
            return false;
        }

        public bool IsBadlyStatus0(ulong nStatus)
        {
            switch ((Effect0)nStatus)
            {
                case Effect0.POISONED:
                case Effect0.CONFUSED:
                case Effect0.ICE_BLOCK:
                case Effect0.HUGE_DAZED:
                case Effect0.DAZED:
                    return true;
            }
            return false;
        }

        public bool AppendStatus(StatusInfoStruct pInfo)
        {
            if (pInfo.Times > 0)
            {
                var pStatus = new StatusMore();
                if (pStatus.Create(this, pInfo.Status, pInfo.Power, pInfo.Seconds, pInfo.Times))
                    Status.AddObj(pStatus);
            }
            else
            {
                var pStatus = new StatusOnce();
                if (pStatus.Create(this, pInfo.Status, pInfo.Power, pInfo.Seconds, pInfo.Times))
                    Status.AddObj(pStatus);
            }
            return true;
        }

        public bool AttachStatus(IRole pRole, int nStatus, int nPower, int nSecs, int nTimes, byte pLevel, uint wCaster = 0)
        {
            if (pRole.Map == null)
                return false;

            if (nStatus == FlagInt.BLUE_NAME &&
                (pRole.Map.IsPkField() || pRole.Map.IsSynMap() || pRole.Map.IsDeadIsland()))
                return false;

            if (nStatus == FlagInt.DAZED)
                m_tDazed.Startup(500);

            switch (nStatus)
            {
                case FlagInt.DAZED:
                case FlagInt.HUGE_DAZED:
                case FlagInt.ICE_BLOCK:
                case FlagInt.CONFUSED:
                    {
                        BattleSystem.ResetBattle();
                        break;
                    }
            }

            IStatus pStatus = QueryStatus(nStatus);
            if (pStatus != null)
            {
                bool bChangeData = false;
                if (pStatus.Power == nPower)
                    bChangeData = true;
                else
                {
                    int nMinPower = Math.Min(nPower, pStatus.Power);
                    int nMaxPower = Math.Max(nPower, pStatus.Power);

                    if (nPower <= 30000)
                        bChangeData = true;
                    else
                    {
                        if (nMinPower >= 30100 || nMinPower > 0 && nMaxPower < 30000)
                        {
                            if (nPower > pStatus.Power)
                                bChangeData = true;
                        }
                        else if (nMaxPower < 0 || nMinPower > 30000 && nMaxPower < 30100)
                        {
                            if (nPower < pStatus.Power)
                                bChangeData = true;
                        }
                    }
                }

                if (bChangeData)
                {
                    pStatus.ChangeData(nPower, nSecs, nTimes, wCaster);
                }
                return true;
            }
            else
            {
                if (nTimes > 1)
                {
                    var pNewStatus = new StatusMore();
                    if (pNewStatus.Create(pRole, nStatus, nPower, nSecs, nTimes, wCaster, pLevel))
                    {
                        pRole.Status.AddObj(pNewStatus);
                        return true;
                    }
                }
                else
                {
                    var pNewStatus = new StatusOnce();
                    if (pNewStatus.Create(pRole, nStatus, nPower, nSecs, 0, wCaster, pLevel))
                    {
                        pRole.Status.AddObj(pNewStatus);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool DetachStatus(int nType)
        {
            StatusSet pStatusSet = Status;
            if (pStatusSet == null)
                return false;

            return pStatusSet.DelObj(nType);
        }

        public bool DetachStatus(Effect0 nType)
        {
            StatusSet pStatusSet = Status;
            if (pStatusSet == null)
                return false;

            if (nType == Effect0.DAZED)
                m_tDazed.Clear();

            return pStatusSet.DelObj(nType);
        }

        public bool DetachStatus(Effect1 nType)
        {
            StatusSet pStatusSet = Status;
            if (pStatusSet == null)
                return false;

            return pStatusSet.DelObj(nType);
        }

        public bool DetachStatus(ulong nType, bool b64)
        {
            StatusSet pStatusSet = Status;
            if (pStatusSet == null)
                return false;

            if (nType == FlagInt.DAZED)
                m_tDazed.Clear();

            return pStatusSet.DelObj(StatusSet.InvertFlag(nType, b64));
        }

        public IStatus QueryStatus(Effect1 nType)
        {
            return Status == null ? null : Status.GetObj((ulong)nType + 64);
        }

        public IStatus QueryStatus(int nType)
        {
            return Status == null ? null : Status.GetObjByIndex(nType);
        }

        public IStatus QueryStatus(Effect0 nType)
        {
            return Status == null ? null : Status.GetObj((ulong)nType);
        }

        #endregion

        #region Rebirth

        public void FixAttributes()
        {
            if (Metempsychosis < 1 || Level < 15)
            {
                Send("You should have at least 1 rebirth in order to use this function.");
                return;
            }

            byte prof = (byte)(Profession / 10 >= 10 ? 10 : ((Profession - (Profession % 10)) / 10));
            var pData = ServerKernel.PointAllot.Values.FirstOrDefault(x => x.Profession == prof && x.Level == 1);
            if (pData == null)
            {
                Console.WriteLine("Could not fetch attribute points data. ResetAttrPoints for user {0}", Identity);
                return;
            }

            Strength = pData.Strength;
            Agility = pData.Agility;
            Vitality = pData.Vitality;
            Spirit = pData.Spirit;

            ushort usAdd = 0;
            if (Metempsychosis == 1)
            {
                usAdd = (ushort)(30 + Math.Min((1 + (130 - 120)) * (130 - 120) / 2, 55));
                usAdd += (ushort)((Level - 15) * 3);
            }
            else if (Metempsychosis == 2)
            {
                byte met1 = (byte)((m_dbUser.MeteLevel - (m_dbUser.MeteLevel % 10000)) / 10000);
                usAdd = (ushort)(30 + Math.Min((1 + (met1 - 120)) * (met1 - 120) / 2, 55));
                usAdd += (ushort)(52 + Math.Min((1 + (met1 - 120)) * (met1 - 120) / 2, 55));
                usAdd += (ushort)((Level - 15) * 3);
            }

            if (Statistics != null)
            {
                DbStatistic dbSta;
                if (Statistics.TryGetValue(10, out dbSta))
                {
                    usAdd += 50;
                }

                if (Statistics.TryGetValue(61 + (1 << 32), out dbSta) && dbSta.Data == 4)
                {
                    usAdd += 100;
                }
            }

            AdditionalPoints = (ushort)(usAdd - 10);
        }

        public int GetRebirthAddPoint(int nOldProf, int nOldLevel, int metempsychosis)
        {
            ushort usAdd = 0;
            if (metempsychosis == 0)
            {
                if (nOldProf == HIGHEST_WATER_WIZARD_PROF)
                    usAdd = (ushort)(30 + Math.Min((1 + (nOldLevel - 110) / 2) * ((nOldLevel - 110) / 2) / 2, 55));
                else
                    usAdd = (ushort)(30 + Math.Min((1 + (nOldLevel - 120)) * (nOldLevel - 120) / 2, 55));
            }
            else if (metempsychosis == 1)
            {
                byte firstMetempsychosisLev = (byte)((m_dbUser.MeteLevel - (m_dbUser.MeteLevel % 10000)) / 10000);
                if (FirstProfession == HIGHEST_WATER_WIZARD_PROF)
                    usAdd = (ushort)(30 + Math.Min((1 + (firstMetempsychosisLev - 110) / 2) * ((firstMetempsychosisLev - 110) / 2) / 2, 55));
                else
                    usAdd = (ushort)(30 + Math.Min((1 + (firstMetempsychosisLev - 120)) * (nOldLevel - 120) / 2, 55));
                if (nOldProf == HIGHEST_WATER_WIZARD_PROF)
                    usAdd += (ushort)(52 + Math.Min((1 + (nOldLevel - 110) / 2) * ((nOldLevel - 110) / 2) / 2, 55));
                else
                    usAdd += (ushort)(52 + Math.Min((1 + (nOldLevel - 120)) * (nOldLevel - 120) / 2, 55));
            }
            return usAdd;
        }

        public void ResetAttrPoints()
        {
            byte prof = (byte)(Profession >= 100 ? 10 : ((Profession - (Profession % 10)) / 10));
            if (prof > 10) prof = 10;
            var pData = ServerKernel.PointAllot.Values.FirstOrDefault(x => x.Profession == prof && x.Level == 1);
            if (pData == null)
            {
                Send("Could not fetch attribute points data. ResetAttrPoints::");
                return;
            }
            ushort totalPoints = (ushort)(AdditionalPoints + Strength + Agility + Vitality + Spirit - 10);

            Strength = pData.Strength;
            Agility = pData.Agility;
            Vitality = pData.Vitality;
            Spirit = pData.Spirit;
            AdditionalPoints = totalPoints;
        }

        public bool Rebirth(ushort nProf, ushort nLook)
        {
            byte rebirth = (byte)(Metempsychosis == 2 ? 2 : Metempsychosis + 1); ;
            var pData = ServerKernel.Rebirths.Values.FirstOrDefault(x => x.NeedProfession == Profession && x.NewProfession == nProf && x.Metempsychosis == rebirth);

            if (pData != null)
            {
                if (Level < pData.NeedLevel)
                {
                    Send("You didn't reach the required level.");
                    return false;
                }

                if (Level >= 130) // old stats (mete_lev) level * 10000 + exp percent
                {
                    var leveXp = ServerKernel.Levelxp.Values.FirstOrDefault(x => x.Level == Level);
                    if (leveXp != null)
                    {
                        string exp = "0";
                        long fExp = Experience / (long)leveXp.Exp;
                        uint metLev = (uint)Level * 10000 + uint.Parse(exp);
                        if (metLev > m_dbUser.MeteLevel)
                            m_dbUser.MeteLevel = metLev;
                    }
                    else if (Level == ServerKernel.MAX_UPLEVEL)
                        m_dbUser.MeteLevel = (uint)ServerKernel.MAX_UPLEVEL * 10000;
                }

                ushort oldProf = Profession;
                ResetUserAttribute(Profession, Level, (byte)(Metempsychosis == 2 ? 2 : rebirth - 1), nProf, nLook);

                foreach (var item in Equipment.Items.Values)
                    item.DegradeItem(false);

                var removeSkills = ServerKernel.Magictypeops.Values.FirstOrDefault(x => x.ProfessionAgo == oldProf && x.ProfessionNow / 10 == Profession / 10 && x.Operation == 0);
                var reloadSkills = ServerKernel.Magictypeops.Values.FirstOrDefault(x => x.ProfessionAgo == oldProf && x.ProfessionNow / 10 == Profession / 10 && x.Operation == 2);
                var learnSkills = ServerKernel.Magictypeops.Values.FirstOrDefault(x => x.ProfessionAgo == oldProf && x.ProfessionNow / 10 == Profession / 10 && x.Operation == 7);

                if (removeSkills != null)
                {
                    foreach (var magic in removeSkills.Magics)
                    {
                        Magics.Delete(magic);
                    }
                }

                if (reloadSkills != null)
                {
                    foreach (var magic in reloadSkills.Magics)
                    {
                        Magic _magic = Magics[magic];
                        if (_magic == null)
                            continue;
                        _magic.OldLevel = _magic.Level;
                        _magic.Level = 0;
                    }
                }

                if (learnSkills != null)
                {
                    foreach (var magic in learnSkills.Magics)
                        Magics.Create(magic, 0);
                }

                if (Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND) && !Equipment.Items[ItemPosition.LEFT_HAND].IsArrowSort())
                    Equipment.Remove(ItemPosition.LEFT_HAND, Equipment.ItemRemoveMethod.REMOVE_TO_INVENTORY);

                return true;
            }
            return false;
        }

        public bool Reincarnate(ushort nProf, ushort nLook)
        {
            const byte rebirth = 2;
            var pData = ServerKernel.Rebirths.Values.FirstOrDefault(x => x.NeedProfession == Profession && x.NewProfession == nProf && x.Metempsychosis == rebirth);

            if (pData != null)
            {
                if (Level < 110)
                {
                    Send("You didn't reach the required level.");
                    return false;
                }

                if (Level >= 110) // old stats (mete_lev) level * 10000 + exp percent
                {
                    var leveXp = ServerKernel.Levelxp.Values.FirstOrDefault(x => x.Level == Level);
                    if (leveXp != null)
                    {
                        string exp = "0";
                        long fExp = Experience / (long)leveXp.Exp;
                        uint metLev = (uint)Level * 10000 + uint.Parse(exp);
                        if (metLev > m_dbUser.MeteLevel)
                            m_dbUser.MeteLevel = metLev;
                    }
                    else if (Level == ServerKernel.MAX_UPLEVEL)
                        m_dbUser.MeteLevel = (uint)ServerKernel.MAX_UPLEVEL * 10000;
                }

                ushort oldProf = Profession;
                ushort firstProf = (ushort) ((FirstProfession-(FirstProfession%10))/10);
                ResetUserAttribute(Profession, Level, 2, nProf, nLook);
                firstProf = (ushort) (firstProf*10 + 1);
                if (firstProf > 100)
                    firstProf += 1;

                foreach (var item in Equipment.Items.Values)
                    item.DegradeItem(false);

                var removeSkills = ServerKernel.Magictypeops.Values.FirstOrDefault(x => x.ProfessionAgo == oldProf && x.ProfessionNow / 10 == Profession / 10 && x.Operation == 0);
                var reloadSkills = ServerKernel.Magictypeops.Values.FirstOrDefault(x => x.ProfessionAgo == oldProf && x.ProfessionNow / 10 == Profession / 10 && x.Operation == 2);
                var learnSkills = ServerKernel.Magictypeops.Values.FirstOrDefault(x => x.ProfessionAgo == oldProf && x.ProfessionNow / 10 == Profession / 10 && x.Operation == 7);
                var firstLifeSkills = ServerKernel.Magictypeops.Values.FirstOrDefault(x => x.ProfessionNow == firstProf && x.Operation == 4);

                if (removeSkills != null)
                {
                    foreach (var magic in removeSkills.Magics)
                    {
                        Magics.Delete(magic);
                    }
                }

                if (firstLifeSkills != null && reloadSkills != null)
                {
                    foreach (var magic in firstLifeSkills.Magics)
                    {
                        if (!reloadSkills.Magics.Contains(magic))
                            Magics.Delete(magic);
                    }
                }

                if (reloadSkills != null)
                {
                    foreach (var magic in reloadSkills.Magics)
                    {
                        Magic _magic = Magics[magic];
                        if (_magic == null)
                            continue;
                        _magic.OldLevel = _magic.Level;
                        _magic.Level = 0;
                    }
                }

                if (learnSkills != null)
                {
                    foreach (var magic in learnSkills.Magics)
                        Magics.Create(magic, 0);
                }

                if (Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND) && !Equipment.Items[ItemPosition.LEFT_HAND].IsArrowSort())
                    Equipment.Remove(ItemPosition.LEFT_HAND, Equipment.ItemRemoveMethod.REMOVE_TO_INVENTORY);

                ServerKernel.Log.GmLog("reincarnation", string.Format("{0} has reincarnated from {1} to {2}", Name, LastProfession, Profession));

                ServerKernel.SendMessageToAll(string.Format(ServerString.STR_METE_SPECIAL_SUCCESS, Name), ChatTone.TALK);
                return true;
            }
            return false;
        }

        public void ResetUserAttribute(ushort nOldProf, byte nOldLev, byte nMete, ushort nNewProf, ushort nNewLook)
        {
            if (nNewProf == 0) nNewProf = (ushort)(Profession / 10 * 10 + 1);
            byte prof = (byte)(nNewProf > 100 ? 10 : nNewProf / 10);

            var pointAllot = ServerKernel.PointAllot.Values.FirstOrDefault(x => x.Profession == prof && x.Level == 1);
            if (pointAllot != null)
            {
                Strength = pointAllot.Strength;
                Agility = pointAllot.Agility;
                Vitality = pointAllot.Vitality;
                Spirit = pointAllot.Spirit;
            }
            else if (prof == 1)
            {
                Strength = 5;
                Agility = 2;
                Vitality = 3;
                Spirit = 0;
            }
            else if (prof == 2)
            {
                Strength = 5;
                Agility = 2;
                Vitality = 3;
                Spirit = 0;
            }
            else if (prof == 4)
            {
                Strength = 2;
                Agility = 7;
                Vitality = 1;
                Spirit = 0;
            }
            else if (prof == 5)
            {
                Strength = 7;
                Agility = 2;
                Vitality = 4;
                Spirit = 0;
            }
            else if (prof == 6)
            {
                Strength = 7;
                Agility = 2;
                Vitality = 4;
                Spirit = 0;
            }
            else if (prof > 10)
            {
                Strength = 0;
                Agility = 2;
                Vitality = 3;
                Spirit = 5;
            }

            AutoAllot = false;
            int attrPoints = GetRebirthAddPoint(Profession, Level, nMete >= 2 ? 1 : nMete);
            AdditionalPoints = (ushort)attrPoints;
            if (nNewLook > 0 && nNewLook != Lookface)
                Lookface = (Lookface - (Lookface % 10)) + nNewLook;
            Level = 15;
            Experience = 0;
            RecalculateAttributes();
            Life = MaxLife;
            Mana = MaxMana;
            Stamina = 100;

            if (nMete == 0)
            {
                FirstProfession = Profession;
                Profession = nNewProf;
                Metempsychosis = 1;
            }
            else if (nMete == 1)
            {
                LastProfession = Profession;
                Profession = nNewProf;
                Metempsychosis = 2;
            }
            else
            {
                FirstProfession = LastProfession;
                LastProfession = Profession;
                Profession = nNewProf;
            }

            if (Profession >= 60 && Profession < 66)
                Hair = 0;

            UpdateClient(ClientUpdateType.REBORN, Metempsychosis);
            Save();
        }

        #endregion

        #region Equipment Offset

        public uint Helmet
        {
            get { return m_helmet; }
            set
            {
                m_helmet = value;
                m_pPacket.Helmet = value;
            }
        }

        public ushort HelmetColor
        {
            get { return m_helmetColor; }
            set
            {
                m_helmetColor = value;
                m_pPacket.Helmet = value;
            }
        }

        public uint Armor
        {
            get { return m_armor; }
            set
            {
                m_armor = value;
                m_pPacket.Armor = value;
            }
        }

        public ushort ArmorColor
        {
            get { return m_armorColor; }
            set
            {
                m_armorColor = value;
                m_pPacket.ArmorColor = value;
            }
        }

        public uint RightHand
        {
            get { return m_rightHand; }
            set
            {
                m_rightHand = value;
                m_pPacket.RightHand = value;
            }
        }

        public uint LeftHand
        {
            get { return m_leftHand; }
            set
            {
                m_leftHand = value;
                m_pPacket.LeftHand = value;
            }
        }

        public ushort ShieldColor
        {
            get { return m_shieldColor; }
            set
            {
                m_shieldColor = value;
                m_pPacket.ShieldColor = value;
            }
        }

        public uint Garment
        {
            get { return m_garment; }
            set
            {
                m_garment = value;
                m_pPacket.Garment = value;
            }
        }

        public uint MountType
        {
            get { return m_pPacket.MountType; }
            set { m_pPacket.MountType = value; }
        }

        public byte MountPlus
        {
            get { return m_pPacket.MountPlus; }
            set { m_pPacket.MountPlus = value; }
        }

        public uint MountColor
        {
            get { return m_pPacket.MountColor; }
            set { m_pPacket.MountColor = value; }
        }

        public uint MountArmor
        {
            get { return m_pPacket.MountArmor; }
            set { m_pPacket.MountArmor = value; }
        }

        public uint RightHandArtifact
        {
            get { return m_pPacket.RightHandArtifact; }
            set { m_pPacket.RightHandArtifact = value; }
        }

        public uint LeftHandArtifact
        {
            get { return m_pPacket.LeftHandArtifact; }
            set { m_pPacket.LeftHandArtifact = value; }
        }

        public uint ArmorArtifact
        {
            get { return m_pPacket.ArmorArtifact; }
            set { m_pPacket.ArmorArtifact = value; }
        }

        public uint HelmetArtifact
        {
            get { return m_pPacket.HelmetArtifact; }
            set { m_pPacket.HelmetArtifact = value; }
        }

        public uint RightAccessory
        {
            get { return m_pPacket.RightAccessory; }
            set { m_pPacket.RightAccessory = value; }
        }

        public uint LeftAccessory
        {
            get { return m_pPacket.LeftAccessory; }
            set { m_pPacket.LeftAccessory = value; }
        }

        #endregion

        #region Syndicate
        public void CreateSyndicate(string szName, uint dwMoney = 1000000)
        {
            if (Syndicate != null || SyndicateMember != null)
                return;

            if (!Handlers.CheckName(szName))
            {
                Send(ServerString.STR_FORBIDDEN_GUILD_NAME);
                return;
            }

            if (ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Name == szName) != null)
            {
                Send(ServerString.STR_INVALID_GUILD_NAME);
                return;
            }

            if (!ReduceMoney(dwMoney, true))
            {
                return;
            }

            var newSyn = new Syndicate();
            if (!newSyn.Create(szName, this))
            {
                ServerKernel.Log.GmLog("synerror",
                    string.Format("Could not create syn {0} - {1}", szName, Identity));
                AwardMoney(dwMoney);
                return;
            }
            newSyn.Save();
            var newLeader = new SyndicateMember(newSyn);
            if (!newLeader.Create(this))
            {
                ServerKernel.Log.GmLog("synerror",
                    string.Format("Could not create synmember {0} - {1} - {2}", newSyn.Identity, newSyn.Name, Identity));
                newSyn.Delete();
                AwardMoney(dwMoney);
                return;
            }
            newLeader.Position = SyndicateRank.GUILD_LEADER;
            newLeader.IncreaseMoney(500000);
            newLeader.Save();

            newSyn.Members.TryAdd(newLeader.Identity, newLeader);

            ServerKernel.SendMessageToAll(string.Format(ServerString.STR_SYNDICATE_CREATE, szName, Name), ChatTone.TALK);
            if (!ServerKernel.Syndicates.TryAdd(newSyn.Identity, newSyn))
            {
                Send(ServerString.STR_SYN_ADD_ERROR);
            }

            Syndicate = newSyn;
            SyndicateMember = newLeader;
            SyndicateIdentity = newSyn.Identity;
            SyndicateRank = newLeader.Position;

            SendSyndicate();
            Syndicate.SendName(this, true);

            Screen.RefreshSpawnForObservers();
        }

        public void DisbandSyndicate()
        {
            if (Syndicate == null || SyndicateMember == null)
                return;

            if (Syndicate.LeaderIdentity != Identity)
                return;

            if (Syndicate.MemberCount > 1)
            {
                Send(ServerString.STR_NO_DISBAND);
                return;
            }
            Syndicate.DisbandSyndicate(this);
        }

        public void SendSyndicate()
        {
            if (Syndicate != null && SyndicateMember != null)
            {
                SyndicateMember.SendSyndicate();
                var announce = new MsgTalk(Syndicate.Announcement, ChatTone.GUILD_ANNOUNCEMENT);
                Send(announce);
            }
        }

        public bool QuitSyndicate()
        {
            if (Syndicate == null || SyndicateMember == null)
                return false;

            if (SyndicateMember.Position == SyndicateRank.GUILD_LEADER)
            {
                Send(ServerString.STR_SYN_LEADER_CANNOT_QUIT);
                return false;
            }

            if (SyndicateMember.SilverDonation < 20000)
            {
                Send(ServerString.STR_SYN_QUIT_NO_DONATION);
                return false;
            }

            SyndicateMember target;
            if (!Syndicate.Members.TryRemove(Identity, out target))
                return false;

            Syndicate.MemberCount -= 1;
            ushort usId = (ushort)SyndicateIdentity;
            SyndicateIdentity = 0;
            Syndicate = null;
            SyndicateRank = SyndicateRank.NONE;
            SyndicateMember.Delete();
            SyndicateMember = null;

            Send(new MsgSyndicate
            {
                Action = SyndicateRequest.SYN_DISBAND,
                Param = usId
            });

            Screen.RefreshSpawnForObservers();

            ServerKernel.Log.GmLog("syndicate", string.Format("(userid:{0}, name:{1}) has quit (synid:{2})"
                , Identity, Name, usId));
            return true;
        }
        #endregion

        #region Family

        public bool CreateFamily(string szName, uint dwPrice, uint dwDonation)
        {
            if (Family != null || FamilyMember != null)
                return false;

            if (!Handlers.CheckName(szName))
            {
                Send(ServerString.STR_FAMILY_INVALID_NAME);
                return false;
            }

            if (ServerKernel.Families.Values.FirstOrDefault(x => x.Name == szName) != null)
            {
                Send(ServerString.STR_FAMILY_NAME_USED);
                return false;
            }

            if (Silver < dwPrice)
            {
                Send(ServerString.STR_NOT_ENOUGH_MONEY);
                return false;
            }

            Family fam = new Family();
            if (!fam.Create(this, szName))
            {
                return false;
            }

            FamilyMember mem = new FamilyMember(fam);
            if (!mem.Create(this))
            {
                return false;
            }
            mem.Position = FamilyRank.CLAN_LEADER;

            ReduceMoney(dwPrice);
            fam.MoneyFunds += dwDonation;
            mem.Donation += dwDonation;

            fam.Members.TryAdd(Identity, mem);

            fam.Save();
            mem.Save();

            FamilyIdentity = fam.Identity;
            FamilyName = fam.Name;
            FamilyPosition = mem.Position;
            SetNames();
            Family = fam;
            FamilyMember = mem;

            fam.SendFamily(this);
            fam.SendRelation(this);

            try
            {
                Screen.RefreshSpawnForObservers();
            }
            catch
            {
                ServerKernel.Log.SaveLog("Error refreshing screen after family creation");
            }
            return ServerKernel.Families.TryAdd(fam.Identity, fam);
        }

        public bool DisbandFamily()
        {
            if (Family == null || FamilyMember == null)
                return false;

            if (FamilyMember.Position != FamilyRank.CLAN_LEADER)
            {
                Send(ServerString.STR_FAMILY_NOT_LEADER);
                return false;
            }

            if (Family.MembersCount > 1)
            {
                Send(ServerString.STR_FAMILY_TOO_BIG);
                return false;
            }

            Family.Delete();
            FamilyMember.Delete();

            FamilyIdentity = 0;
            FamilyName = "";
            FamilyPosition = 0;
            SetNames();

            Screen.RefreshSpawnForObservers();

            SendEmptyFamily();

            Family = null;
            FamilyMember = null;
            return true;
        }

        public void SendEmptyFamily()
        {
            MsgFamily pMsg = new MsgFamily
            {
                Identity = Identity,
                Type = FamilyType.INFO
            };
            pMsg.AddString(string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0));
            pMsg.AddString("");
            pMsg.AddString(Name);
            Send(pMsg);

            pMsg.Type = FamilyType.QUIT;
            Send(pMsg);
        }

        #endregion

        #region Status

        public void SetGhost()
        {
            if (IsAlive) return;
            ushort trans = 98;
            if (Gender == 2)
                trans = 99;
            Transformation = trans;
        }

        #region Lucky Time

        public uint RemainingLuckyTime
        {
            get { return m_dbUser.LuckyTime * 1000; }
        }

        #endregion

        #region Multiple Exp

        public bool HasMultipleExp
        {
            get { return m_dbUser.ExperienceMultipler > 1 && m_dbUser.ExperienceExpires >= UnixTimestamp.Timestamp(); }
        }

        public float ExperienceMultiplier
        {
            get { return (!HasMultipleExp || m_dbUser.ExperienceMultipler <= 1) ? 1f : m_dbUser.ExperienceMultipler; }
        }

        public uint RemainingExperienceSeconds
        {
            get
            {
                int now = UnixTimestamp.Timestamp();
                if (m_dbUser.ExperienceExpires < now)
                {
                    m_dbUser.ExperienceMultipler = 1;
                    return m_dbUser.ExperienceExpires = 0;
                }
                return (uint)(m_dbUser.ExperienceExpires - now);
            }
        }

        public bool SetExperienceMultiplier(uint nSeconds, float nMultiplier = 2f)
        {
            m_dbUser.ExperienceExpires = (uint)(UnixTimestamp.Timestamp() + nSeconds);
            m_dbUser.ExperienceMultipler = nMultiplier;
            UpdateClient(ClientUpdateType.DOUBLE_EXP_TIMER, RemainingExperienceSeconds, false);
            return true;
        }

        #endregion

        #region Heaven Blessing

        public void SendBless()
        {
            if (IsBlessed)
            {
                int now = UnixTimestamp.Timestamp();
                UpdateClient(ClientUpdateType.HEAVENS_BLESSING, (uint)(HeavenBlessingExpires - now), false);

                if (Map != null && !Map.IsTrainingMap())
                    UpdateClient(ClientUpdateType.ONLINE_TRAINING, 0, false);
                else
                    UpdateClient(ClientUpdateType.ONLINE_TRAINING, 1, false);

                AttachStatus(this, FlagInt.HEAVEN_BLESS, 0, (int)(HeavenBlessingExpires - now), 0, 0, Identity);
            }
        }

        /// <summary>
        /// This method will update the user blessing time.
        /// </summary>
        /// <param name="amount">The amount of minutes to be added.</param>
        /// <returns>If the heaven blessing has been added successfully.</returns>
        public bool AddBlessing(uint amount)
        {
            int now = UnixTimestamp.Timestamp();
            if (m_dbUser.HeavenBlessing > 0 && m_dbUser.HeavenBlessing >= now)
                m_dbUser.HeavenBlessing += 60 * 60 * amount;
            else
                m_dbUser.HeavenBlessing = (uint)(now + 60 * 60 * amount);

            SendBless();

            //if (Mentor != null)
            //    AddMentorBlessing((ushort)(amount / 10));
            return true;
        }

        public uint HeavenBlessingExpires
        {
            get { return m_dbUser.HeavenBlessing; }
        }

        public bool IsBlessed
        {
            get { return m_dbUser.HeavenBlessing > UnixTimestamp.Timestamp(); }
        }

        #endregion

        public uint GetStatisticValue(uint type, uint datatype)
        {
            ulong ulType = type + (datatype << 32);
            DbStatistic dbStatistic;
            return Statistics.TryGetValue(ulType, out dbStatistic) ? dbStatistic.Data : 0;
        }

        #endregion

        #region Detain Equipment

        public bool DetainEquipment(Character pTarget)
        {
            List<Item> items = new List<Item>(20);

            foreach (var item in pTarget.Equipment.Items.Values)
            {
                switch (Calculations.GetItemPosition(item.Type))
                {
                    case ItemPosition.HEADWEAR:
                    case ItemPosition.NECKLACE:
                    case ItemPosition.RING:
                    case ItemPosition.RIGHT_HAND:
                    case ItemPosition.LEFT_HAND:
                    case ItemPosition.ARMOR:
                    case ItemPosition.BOOTS:
                    case ItemPosition.ATTACK_TALISMAN:
                    case ItemPosition.DEFENCE_TALISMAN:
                    case ItemPosition.CROP:
                        if (item.IsArrowSort())
                            continue;
                        items.Add(item);
                        continue;
                }
            }

            Item detain = items[(ThreadSafeRandom.RandGet(0, items.Count) % items.Count)];

            DbDetainedItem dbDetainedItem = new DbDetainedItem
            {
                HunterIdentity = Identity,
                HunterName = Name,
                TargetIdentity = pTarget.Identity,
                TargetName = pTarget.Name,
                RedeemPrice = DetainedObject.GetDetainPrice(detain),
                HuntTime = UnixTimestamp.Timestamp(),
                ItemIdentity = detain.Identity
            };
            new DetainedItemRepository().SaveOrUpdate(dbDetainedItem);

            DetainedObject pObj = new DetainedObject(true);
            if (!pObj.Create(dbDetainedItem))
                return false;

            pTarget.Equipment.Remove((byte)detain.Position);
            pTarget.Inventory.Remove(detain.Identity, ItemRemovalType.TAKE_OUT_FROM_INVENTORY_ONLY);

            MsgAction pMsg = new MsgAction(pTarget.Identity, detain.Identity, pTarget.MapX, pTarget.MapY, GeneralActionType.ITEMS_DETAINED);
            pTarget.Screen.Send(pMsg, true);
            pMsg = new MsgAction(Identity, 0, MapX, MapY, GeneralActionType.DETAIN_ITEM);
            Send(pMsg);

            MsgMapItem msg =
                new MsgMapItem((uint)pTarget.Map.FloorItem,
                    detain.Type,
                    (ushort)(pTarget.MapX + 2),
                    pTarget.MapY, 4);
            pTarget.Screen.Send(msg, true);

            MsgDetainItemInfo nMsg = new MsgDetainItemInfo
            {
                Identity = dbDetainedItem.Identity,
                ItemIdentity = detain.Identity,
                Itemtype = detain.Type,
                Durability = detain.Durability,
                MaximumDurability = detain.MaximumDurability,
                Mode = DetainMode.CLAIM_PAGE,
                SocketProgress = detain.SocketProgress,
                SocketOne = detain.SocketOne,
                SocketTwo = detain.SocketTwo,
                Effect = detain.Effect,
                Plus = detain.Plus,
                Blessing = detain.ReduceDamage,
                Bound = detain.Bound,
                Enchantment = detain.Enchantment,
                Suspicious = false,
                Locked = detain.IsLocked(),
                Color = detain.Color,
                OwnerIdentity = Identity,
                OwnerName = Name,
                TargetIdentity = pTarget.Identity,
                TargetName = pTarget.Name,
                Date = uint.Parse(DateTime.Now.ToString("yyyyMMdd")),
                DaysPast = 7,
                Expired = false,
                Cost = dbDetainedItem.RedeemPrice
            };
            Send(nMsg);
            nMsg.Mode = DetainMode.DETAIN_PAGE;
            nMsg.OwnerIdentity = pTarget.Identity;
            nMsg.OwnerName = pTarget.Name;
            nMsg.TargetIdentity = Identity;
            nMsg.TargetName = Name;
            pTarget.Send(nMsg);

            detain.Position = ItemPosition.DETAINED;
            detain.Save();

            return ServerKernel.DetainedObjects.TryAdd(pObj.Identity, pObj);
        }

        #endregion

        #region Currency Methods

        public void FillLife()
        {
            Life = MaxLife;
        }

        public void FillMana()
        {
            Mana = MaxMana;
        }

        public bool ChangeEp(short sEp)
        {
            if (sEp < 0)
            {
                sEp *= -1;
                if (sEp > Stamina)
                    return false;
                Stamina -= (byte)sEp;
                return true;
            }
            byte maxStamina = 100;
            if (IsBlessed)
                maxStamina = 180;
            if (Stamina + sEp > maxStamina)
                sEp = (short)(maxStamina - Stamina);
            Stamina += (byte)sEp;
            return true;
        }

        /// <summary>
        /// This method will increase or decrease the user money amount. It checks if the money
        /// can be deducted and then reduces it. Also, the max money amount can't exceed the
        /// int.MaxValue.
        /// </summary>
        /// <param name="amount">The amount of silver you want to increase or decrease.</param>
        /// <returns>If the silver amount has been changed.</returns>
        public bool ChangeMoney(long amount)
        {
            if (amount < 0)
            {
                amount *= -1;
                if (amount > Silver)
                    return false;
                Silver -= (uint)amount;
                return true;
            }
            if ((amount + Silver) > int.MaxValue)
                return false;
            Silver += (uint)amount;
            return true;
        }

        public bool ChangeStorageMoney(long amount)
        {
            if (amount < 0)
            {
                amount *= -1;
                if (amount > MoneySaved)
                    return false;
                MoneySaved -= (uint)amount;
                return true;
            }
            if ((amount + MoneySaved) > int.MaxValue)
                return false;
            MoneySaved += (uint)amount;
            return true;
        }

        /// <summary>
        /// This method will increase or decrease the user emoney amount. It checks if the emoney
        /// can be deducted and then reduces it. Also, the max emoney amount can't exceed the
        /// int.MaxValue.
        /// </summary>
        /// <param name="amount">The amount of CPs you want to increase or decrease.</param>
        /// <returns>If the CPs amount has been changed.</returns>
        public bool ChangeEmoney(long amount, bool onlyChange = false)
        {
            if (amount < 0)
            {
                amount *= -1;
                if (amount > Emoney)
                    return false;
                Emoney -= (uint)amount;
                return true;
            }
            if ((amount + Emoney) > int.MaxValue)
                return false;
            if (onlyChange)
            {
                Emoney = (uint)amount;
            } else
            {
                Emoney += (uint)amount;
            }
            return true;
        }

        public bool AwardMoney(long amount, bool bMsg = false)
        {
            uint dwAmount = (uint)(amount > int.MaxValue ? int.MaxValue : amount);

            Silver += dwAmount;
            return true;
        }

        public bool ReduceMoney(long amount, bool bMsg = false)
        {
            uint dwAmount = (uint)(amount > int.MaxValue ? int.MaxValue : amount);

            if (dwAmount > Silver)
            {
                if (bMsg) Send(ServerString.STR_NOT_ENOUGH_MONEY);
                return false;
            }

            Silver -= dwAmount;
            return true;
        }

        public bool ChangeStudyPoints(long amount)
        {
            if (amount < 0)
            {
                amount *= -1;
                if (amount > StudyPoints)
                    return false;
                StudyPoints -= (uint)amount;
                return true;
            }
            if ((amount + StudyPoints) > int.MaxValue)
                return false;
            StudyPoints += (uint)amount;

            MsgSubPro pMsg = new MsgSubPro();
            pMsg.Action = SubClassActions.UPDATE_STUDY;
            pMsg.AwardedStudy = (uint) amount;
            Send(pMsg);

            return true;
        }

        public bool AwardStudyPoints(long amount, bool bMsg = false)
        {
            uint dwAmount = (uint)(amount > int.MaxValue ? int.MaxValue : amount);

            MsgSubPro pMsg = new MsgSubPro();
            pMsg.Action = SubClassActions.UPDATE_STUDY;
            pMsg.AwardedStudy = dwAmount;
            Send(pMsg);

            StudyPoints += dwAmount;
            return true;
        }

        public bool ReduceStudyPoints(long amount, bool bMsg = false)
        {
            uint dwAmount = (uint)(amount > int.MaxValue ? int.MaxValue : amount);

            if (dwAmount > StudyPoints)
            {
                if (bMsg) Send(ServerString.STR_NOT_ENOUGH_STUDY);
                return false;
            }

            StudyPoints -= dwAmount;
            return true;
        }

        public bool AwardEmoney(long amount, bool bMsg = false)
        {
            uint dwAmount = (uint)(amount > int.MaxValue ? int.MaxValue : amount);

            Emoney += dwAmount;
            return true;
        }

        public bool ReduceEmoney(long amount, bool bMsg = false)
        {
            uint dwAmount = (uint)(amount > int.MaxValue ? int.MaxValue : amount);

            if (dwAmount > Emoney)
            {
                if (bMsg) Send(ServerString.STR_NOT_ENOUGH_EMONEY);
                return false;
            }

            Emoney -= dwAmount;
            return true;
        }

        public bool AwardBoundEmoney(long amount, bool bMsg = false)
        {
            uint dwAmount = (uint)(amount > int.MaxValue ? int.MaxValue : amount);

            BoundEmoney += dwAmount;
            return true;
        }

        public bool ReduceBoundEmoney(long amount, bool bMsg = false)
        {
            uint dwAmount = (uint)(amount > int.MaxValue ? int.MaxValue : amount);

            if (dwAmount > BoundEmoney)
            {
                if (bMsg) Send(ServerString.STR_NOT_ENOUGH_EMONEY2);
                return false;
            }

            BoundEmoney -= dwAmount;
            return true;
        }

        private const int _MAX_MONEY_SAVED = 999999999;
        public bool AwardMoneySaved(long amount, bool bMsg = false)
        {
            if (amount + MoneySaved > _MAX_MONEY_SAVED)
            {
                if (bMsg)
                    Send(ServerString.STR_WAREHOUSE_MONEY_NO_MORE);
                return false;
            }

            uint dwAmount = (uint)(amount > _MAX_MONEY_SAVED ? _MAX_MONEY_SAVED : amount);

            MoneySaved += dwAmount;
            return true;
        }

        public bool ReduceMoneySaved(long amount, bool bMsg = false)
        {
            uint dwAmount = (uint)(amount > _MAX_MONEY_SAVED ? _MAX_MONEY_SAVED : amount);

            if (dwAmount > MoneySaved)
            {
                if (bMsg) Send(ServerString.STR_NOT_ENOUGH_MONEY);
                return false;
            }

            MoneySaved -= dwAmount;
            return true;
        }

        public int BonusCount()
        {
            return new BonusRepository().BonusAmount(Owner.AccountIdentity);
        }

        public bool DoBonus()
        {
            if (Inventory.RemainingSpace() < 10)
            {
                Send("You need at least 10 empty spaces in your inventory.");
                return false;
            }

            var repo = new BonusRepository();
            var bonus = repo.CatchBonus(Owner.AccountIdentity);
            if (bonus == null || bonus.Flag > 0)
            {
                Send(ServerString.STR_NO_BONUS);
                return false;
            }

            ActionStruct action;
            if (!ServerKernel.GameActions.TryGetValue((uint)bonus.Action, out action))
                return false;

            if (!GameAction.ProcessAction(action.Id, this, this, null, null))
            {
                bonus.Flag = 1;
                return repo.SaveOrUpdate(bonus);
            }

            ServerKernel.Log.GmLog("bonus", string.Format("AccountID({0}), Player({1}) take bonus ({2}) action ({3})", bonus.AccountIdentity, Identity, bonus.Identity, bonus.Action));

            bonus.Time = UnixTimestamp.Timestamp();
            bonus.Flag = 1;
            return repo.SaveOrUpdate(bonus);
        }

        public bool ChangePkPoints(int amount)
        {
            if (amount < 0)
            {
                if (PkPoints - amount < 0)
                    amount = 0;

                amount *= -1;
                if ((ushort)amount > PkPoints)
                    PkPoints = 0;
                PkPoints -= (ushort)amount;
                return true;
            }
            if ((ushort)amount + PkPoints > ushort.MaxValue)
                return false;
            PkPoints += (ushort)amount;
            return true;
        }

        public bool SetProfession(ushort prof)
        {
            if (prof <= 0
                || prof > 255)
                return false;
            Profession = (byte)prof;
            return true;
        }

        public bool AwardLevel(ushort amount)
        {
            if (Level >= ServerKernel.MAX_UPLEVEL)
                return false;

            if (Level + amount <= 0)
                return false;

            int addLev = amount;
            if (addLev + Level > ServerKernel.MAX_UPLEVEL)
                addLev = ServerKernel.MAX_UPLEVEL - Level;

            if (addLev < 0)
                return false;

            AdditionalPoints += (ushort)(addLev * 3);
            Level += (byte)addLev;
            Owner.Screen.Send(new MsgAction(Identity, 0, 0, 0, GeneralActionType.LEVELED), true);
            return true;
        }

        public bool SetExperience(long amount)
        {
            Experience = Experience + amount;
            return true;
        }

        public bool AwardExperience(long amount, bool nContribute = false, bool bAction = false)
        {
            if (Level > ServerKernel.Levelxp.Count)
                return true;

            float multiply = 1f;

            if (IsBlessed && !bAction)
                multiply = multiply + .2f;
            if (HasMultipleExp && !bAction)
                multiply = multiply + ExperienceMultiplier;
            if (QueryStatus(FlagInt.OBLIVION) != null)
                multiply = multiply + .5f;

            amount = (long)(amount * multiply);

            m_lAccumulateExp += amount;

            amount += Experience;
            bool leveled = false;
            uint pointAmount = 0;
            byte newLevel = Level;
            while (newLevel < ServerKernel.MAX_UPLEVEL && amount >= (long)ServerKernel.Levelxp[newLevel].Exp)
            {
                amount -= (long)ServerKernel.Levelxp[newLevel].Exp;
                leveled = true;
                newLevel++;
                if (!AutoAllot
                    || Level > 120)
                {
                    pointAmount += 3;
                    continue;
                }

                if (newLevel < ServerKernel.MAX_UPLEVEL) continue;
                amount = 0;
                break;
            }

            if ((newLevel >= 130 && Metempsychosis > 0 && m_dbUser.MeteLevel > newLevel * 10000)
                || (newLevel >= 110 && Metempsychosis >= 2 && m_dbUser.MeteLevel > newLevel * 10000))
            {
                byte extra = 0;

                // if (m_dbUser.MeteLevel > newLevel * 10000)
                {
                    var mete = m_dbUser.MeteLevel / 10000;
                    extra += (byte)(mete - newLevel);
                    pointAmount += (uint)(extra * 3);
                    leveled = true;
                }

                newLevel += extra;

                long newExp = 0;
                if (newLevel >= ServerKernel.MAX_UPLEVEL)
                {
                    newLevel = ServerKernel.MAX_UPLEVEL;
                    amount = 0;
                }
                else if (m_dbUser.MeteLevel / 100 >= newLevel)
                {
                    newExp = (long)(ServerKernel.Levelxp[newLevel].Exp * ((m_dbUser.MeteLevel % 10000) / 100));
                    amount = newExp;
                }
            }

            Experience = amount;

            if (leveled)
            {
                byte job;
                if (Profession > 100)
                    job = 10;
                else
                    job = (byte)((Profession - (Profession % 10)) / 10);

                var allot = GetPointAllot(job, newLevel);
                Level = newLevel;
                if (AutoAllot && allot != null)
                {
                    Strength = allot.Strength;
                    Agility = allot.Agility;
                    Vitality = allot.Vitality;
                    Spirit = allot.Spirit;
                }
                if (pointAmount > 0)
                    AwardAttributePoints((int)pointAmount);

                RecalculateAttributes();
                Life = MaxLife;
                Mana = MaxMana;
                Owner.Screen.Send(new MsgAction(Identity, 0, 0, 0, GeneralActionType.LEVELED), true);

                if (Team != null && Team.Leader != this && Level <= 70)
                {
                    ushort vp = (ushort)((Level * 17 / 13 * 12 / 2) + Level * 3);
                    Team.Leader.VirtuePoints += vp;

                    if (Team.Leader.Syndicate != null && Syndicate != null
                        && Team.Leader.Syndicate == Syndicate)
                        SyndicateMember.IncreaseGuideDonation(1);

                    Syndicate.Send(string.Format("{0} has awarded 1 Guide Donation for helping newbies."));
                    Team.Send(new MsgTalk(string.Format("{0} has awarded {1} virtue points as reward for helping newbies.",
                            Team.Leader.Name, vp), ChatTone.TEAM));
                }

                if (Level >= 3)
                {
                    Magic pMgc;
                    switch (Profession)
                    {
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                            if (Magics.Create(1110, 0))
                            {
                                SetXp(100);
                                BurstXp();
                            }
                            break;
                        case 20:
                        case 21:
                        case 22:
                        case 23:
                        case 24:
                        case 25:
                            if (Magics.Create(1025, 0))
                            {
                                SetXp(100);
                                BurstXp();
                            }
                            break;
                        case 40:
                        case 41:
                        case 42:
                        case 43:
                        case 44:
                        case 45:
                            if (Magics.Create(8002, 0))
                            {
                                SetXp(100);
                                BurstXp();
                            }
                            break;
                        case 50:
                        case 51:
                        case 52:
                        case 53:
                        case 54:
                        case 55:
                            if (Magics.Create(6011, 0))
                            {
                                SetXp(100);
                                BurstXp();
                            }
                            break;
                        case 60:
                        case 61:
                        case 62:
                        case 63:
                        case 64:
                        case 65:
                        {
                            if (Magics.Create(10390, 0))
                            {
                                SetXp(100);
                                BurstXp();
                            }
                            break;
                        }
                        case 100:
                            if (Magics.Create(1010, 0))
                            {
                                SetXp(100);
                                BurstXp();
                            }
                            break;
                    }
                }
            }
            return true;
        }

        public void SetAttributePoints(ushort point)
        {
            AdditionalPoints = point;
        }

        public bool AwardAttributePoints(int pointAmount)
        {
            if (pointAmount < 0) return false;

            if (pointAmount + m_dbUser.AdditionalPoints > short.MaxValue)
                pointAmount = short.MaxValue;

            AdditionalPoints += (ushort)pointAmount;
            return true;
        }

        public bool SpendAttributePoints(int pointAmount)
        {
            if (pointAmount < 0) pointAmount *= -1;
            if (m_dbUser.AdditionalPoints < pointAmount) return false;
            if (pointAmount > ushort.MaxValue) pointAmount = ushort.MaxValue;
            AdditionalPoints -= (ushort)pointAmount;
            return true;
        }

        private DbPointAllot GetPointAllot(byte job, byte newLevel)
        {
            return ServerKernel.PointAllot.Values.FirstOrDefault(x => x.Level == newLevel && x.Profession == job);
        }

        public bool ChangeForce(int amount)
        {
            if (amount < 0)
            {
                amount *= -1;
                if ((ushort)amount > Strength)
                    return false;
                Strength -= (ushort)amount;
                return true;
            }
            if ((ushort)amount + Strength > ushort.MaxValue)
                return false;
            Strength += (ushort)amount;
            return true;
        }

        public bool ChangeSpeed(int amount)
        {
            if (amount < 0)
            {
                amount *= -1;
                if ((ushort)amount > Agility)
                    return false;
                Agility -= (ushort)amount;
                return true;
            }
            if ((ushort)amount + Agility > ushort.MaxValue)
                return false;
            Agility += (ushort)amount;
            return true;
        }

        public bool ChangeHealth(int amount)
        {
            if (amount < 0)
            {
                amount *= -1;
                if ((ushort)amount > Vitality)
                    return false;
                Vitality -= (ushort)amount;
                return true;
            }
            if ((ushort)amount + Vitality > ushort.MaxValue)
                return false;
            Vitality += (ushort)amount;
            return true;
        }

        public bool ChangeSoul(int amount)
        {
            if (amount < 0)
            {
                amount *= -1;
                if ((ushort)amount > Spirit)
                    return false;
                Spirit -= (ushort)amount;
                return true;
            }
            if ((ushort)amount + Spirit > ushort.MaxValue)
                return false;
            Spirit += (ushort)amount;
            return true;
        }

        public float GetLifeMultiplier()
        {
            if (Profession > 9
                && Profession < 16)
            {
                float multiplier = 1f;
                switch (Profession)
                {
                    case 11:
                        multiplier = 1.05f;
                        break;
                    case 12:
                        multiplier = 1.08f;
                        break;
                    case 13:
                        multiplier = 1.1f;
                        break;
                    case 14:
                        multiplier = 1.12f;
                        break;
                    case 15:
                        multiplier = 1.15f;
                        break;
                }
                return 24 * multiplier;
            }
            if (Profession > 19
                && Profession < 26)
                return 24;
            if (Profession > 39
                && Profession < 46)
                return 24;
            if (Profession > 49
                && Profession < 56)
                return 24;
            if (Profession > 59
                && Profession < 66)
                return 24;
            if (Profession > 99)
                return 24;
            return 20;
        }

        public float GetManaMultiplier()
        {
            switch (Profession)
            {
                case 132:
                case 142:
                    return 15;
                case 133:
                case 143:
                    return 20;
                case 144:
                case 134:
                    return 25;
                case 145:
                case 135:
                    return 30;
            }
            return 5;
        }

        public bool IsPureClass()
        {
            return Metempsychosis == 2 && Profession / 10 == LastProfession / 10 && Profession / 10 == FirstProfession / 10;
        }

        public void ProcXpVal()
        {
            if (!IsAlive)
            {
                ClsXpVal();
                return;
            }

            IStatus pStatus = QueryStatus(Effect0.START_XP);
            if (pStatus != null)
                return;

            if (m_pXpPoints >= 100)
            {
                BurstXp();
                SetXp(0);
                m_tXp.Update();
            }
            else
            {
                if (Map != null && Map.IsBoothEnable())
                    return;
                AddXp(1);
            }
        }

        public bool BurstXp()
        {
            if (m_pXpPoints < 100)
                return false;

            IStatus pStatus = QueryStatus(Effect0.START_XP);
            if (pStatus != null)
                return true;

            AttachStatus(this, FlagInt.START_XP, 0, 20, 0, 0);
            return true;
        }

        public void SetXp(byte nXp)
        {
            if (nXp > 100 || QueryStatus(Effect0.START_XP) != null)
                return;
            XpPoints = nXp;
        }

        public void AddXp(byte nXp)
        {
            if (nXp <= 0 || nXp + m_pXpPoints > 100)
                return;
            XpPoints += nXp;
        }

        public void ClsXpVal()
        {
            XpPoints = 0;
            Status.DelObj(Effect0.START_XP);
        }

        #endregion

        #region Update Packet

        public void SendEffect(string effectName, bool screen)
        {
            if (m_pOwner == null || m_pOwner.Screen == null)
                return;

            var sPacket = new MsgName();
            sPacket.Identity = Identity;
            sPacket.Action = StringAction.ROLE_EFFECT;
            sPacket.Append(effectName);
            Send(sPacket);
            if (screen && LoginComplete)
                if (Screen != null)
                    Screen.Send(sPacket, true);
        }

        /// <summary>
        /// Send a update packet containing a byte value.
        /// </summary>
        public void UpdateClient(ClientUpdateType type, byte value, bool broadcast = true)
        {
            if (Owner == null || Owner.Screen == null)
                return;

            var updatePacket = new MsgUserAttrib();
            updatePacket.Identity = Identity;
            updatePacket.Append(type, value);
            Send(updatePacket);
            if (broadcast && LoginComplete)
                if (Screen != null)
                    Screen.Send(updatePacket, true);
        }

        /// <summary>
        /// Send a update packet containing a uint value.
        /// </summary>
        public void UpdateClient(ClientUpdateType type, uint value, bool broadcast = false)
        {
            if (Owner == null || Owner.Screen == null)
                return;

            var updatePacket = new MsgUserAttrib();
            updatePacket.Identity = Identity;
            updatePacket.Append(type, value);
            Send(updatePacket);
            if (broadcast && LoginComplete)
                if (Screen != null)
                    Screen.Send(updatePacket, true);
        }

        /// <summary>
        /// Send a update packet containing a ushort value.
        /// </summary>
        public void UpdateClient(ClientUpdateType type, ushort value, bool broadcast = false)
        {
            if (Owner == null || Owner.Screen == null)
                return;

            var updatePacket = new MsgUserAttrib();
            updatePacket.Identity = Identity;
            updatePacket.Append(type, value);
            Send(updatePacket);
            if (broadcast && LoginComplete)
                if (Screen != null)
                    Screen.Send(updatePacket, true);
        }

        /// <summary>
        /// Send a update packet containing a ulong value.
        /// </summary>
        public void UpdateClient(ClientUpdateType type, ulong value, bool broadcast = false)
        {
            if (Owner == null || Owner.Screen == null)
                return;

            var updatePacket = new MsgUserAttrib();
            updatePacket.Identity = Identity;
            updatePacket.Append(type, value);
            Send(updatePacket);
            if (broadcast && LoginComplete)
                if (Screen != null)
                    Screen.Send(updatePacket, true);
        }

        public void UpdateClient(ClientUpdateType type, ulong value1, ulong value2, bool broadcast = false)
        {
            if (Owner == null || Owner.Screen == null)
                return;

            var updatePacket = new MsgUserAttrib();
            updatePacket.Identity = Identity;
            updatePacket.Append(type, value1, value2);
            Owner.Send(updatePacket);
            if (broadcast && LoginComplete)
                if (Screen != null)
                    Screen.Send(updatePacket, true);
        }

        public void UpdateSoulShackle(int nTime)
        {
            if (Owner == null || Owner.Screen == null)
                return;

            var updatePacket = new MsgUserAttrib();
            updatePacket.Identity = Identity;
            updatePacket.Append(ClientUpdateType.SOUL_SHACKLE_TIMER, 111u, (uint) nTime);
            Owner.Send(updatePacket);
            if (Screen != null)
                Screen.Send(updatePacket, true);
        }

        public void UpdateAzureShield(int nTime, int nPower, byte level)
        {
            if (Owner == null || Owner.Screen == null)
                return;

            var updatePacket = new MsgUserAttrib();
            updatePacket.Identity = Identity;
            updatePacket.Append(ClientUpdateType.AZURE_SHIELD, 93u, (uint)nTime, (uint) nPower, level);
            Owner.Send(updatePacket);
            if (Screen != null)
                Screen.Send(updatePacket, true);
        }

        #endregion

        #region Application Handle

        public void SetFriendRequest(uint dwTarget) { m_dwFriendRequest = dwTarget; }
        public bool FetchFriendRequest(uint dwTarget) { return m_dwFriendRequest == dwTarget; }
        public void ClearFriendRequest() { m_dwFriendRequest = 0; }

        public void SetMarryRequest(uint dwTarget) { m_dwMarryRequest = dwTarget; }
        public bool FetchMarryRequest(uint dwTarget) { return m_dwMarryRequest == dwTarget; }
        public void ClearMarryRequest() { m_dwMarryRequest = 0; }

        public void SetTradeRequest(uint dwTarget) { m_dwTradeRequest = dwTarget; }
        public bool FetchTradeRequest(uint dwTarget) { return m_dwTradeRequest == dwTarget; }
        public bool HasRequestedTrade() { return m_dwTradeRequest != 0; }
        public void ClearTradeRequest() { m_dwTradeRequest = 0; }

        public void SetTeamJoin(uint dwTarget) { m_dwTeamJoin = dwTarget; }
        public bool FetchTeamJoin(uint dwTarget) { return m_dwTeamJoin == dwTarget; }
        public void ClearTeamJoin() { m_dwTeamJoin = 0; }

        public void SetTeamInvite(uint dwTarget) { m_dwTeamInvite = dwTarget; }
        public bool FetchTeamInvite(uint dwTarget) { return m_dwTeamInvite == dwTarget; }
        public void ClearTeamInvite() { m_dwTeamInvite = 0; }

        public void SetSynJoin(uint dwTarget) { m_dwSynJoin = dwTarget; }
        public bool FetchSynJoin(uint dwTarget) { return m_dwSynJoin == dwTarget; }
        public void ClearSynJoin() { m_dwSynJoin = 0; }

        public void SetSynInvite(uint dwTarget) { m_dwSynInvite = dwTarget; }
        public bool FetchSynInvite(uint dwTarget) { return m_dwSynInvite == dwTarget; }
        public void ClearSynInvite() { m_dwSynInvite = 0; }

        public void SetSynAllyRequest(uint dwTarget) { m_dwSynAlly = dwTarget; }
        public bool FetchSynAllyRequest(uint dwTarget) { return m_dwSynAlly == dwTarget; }
        public void ClearSynAllyRequest() { m_dwSynAlly = 0; }

        public void SetTradeBuddyRequest(uint dwTarget) { m_dwTradeBuddy = dwTarget; }
        public bool FetchTradeBuddyRequest(uint dwTarget) { return m_dwTradeBuddy == dwTarget; }
        public void ClearTradeBuddyRequest() { m_dwTradeBuddy = 0; }

        public void SetGuideRequest(uint dwTarget) { m_dwGuideRequest = dwTarget; }
        public bool FetchGuideRequest(uint dwTarget) { return m_dwGuideRequest == dwTarget; }
        public void ClearGuideRequest() { m_dwGuideRequest = 0; }

        public void SetStudentRequest(uint dwTarget) { m_dwStudentRequest = dwTarget; }
        public bool FetchStudentRequest(uint dwTarget) { return m_dwStudentRequest == dwTarget; }
        public void ClearStudentRequest() { m_dwStudentRequest = 0; }

        public void SetFamilyRecruitRequest(uint dwTarget) { m_dwFamilyRequest = dwTarget; }
        public bool FetchFamilyRecruitRequest(uint dwTarget) { return m_dwFamilyRequest == dwTarget; }
        public void ClearFamilyRecruitRequest() { m_dwFamilyRequest = 0; }

        public void SetFamilyJoinRequest(uint dwTarget) { m_dwJoinFamilyRequest = dwTarget; }
        public bool FetchFamilyJoinRequest(uint dwTarget) { return m_dwJoinFamilyRequest == dwTarget; }
        public void ClearFamilyJoinRequest() { m_dwJoinFamilyRequest = 0; }

        #endregion
        
        #region Socket

        public void Ban()
        {
            AccountRepository repo = new AccountRepository();
            DbAccount dbAcc = repo.SearchByIdentity(Owner.AccountIdentity);
            if (dbAcc == null) return;
            switch (dbAcc.Lock)
            {
                case 0: dbAcc.LockExpire = (uint) (UnixTimestamp.Timestamp() + UnixTimestamp.TIME_SECONDS_DAY*3); break;
                case 1: dbAcc.LockExpire = (uint) (UnixTimestamp.Timestamp() + UnixTimestamp.TIME_SECONDS_DAY*7); break;
                case 2: dbAcc.LockExpire = (uint) (UnixTimestamp.Timestamp() + UnixTimestamp.TIME_SECONDS_DAY*15); break;
            }
            dbAcc.Lock = 1;
            repo.SaveOrUpdate(dbAcc);
            Disconnect("USER_BANNED");
        }

        /// <summary>
        /// This method sends the character's spawn packet to another player. It is called by the screen system when 
        /// the players appear in each others' screens. By default, the actor of the screen change loads the spawn 
        /// data for both players.
        /// </summary>
        /// <param name="pTarget">The observer being sent the spawn packet.</param>
        public void SendSpawnTo(Character pTarget)
        {
            if (Invisible && !pTarget.IsGm) return;
            if (IsWatcher) return;

            m_pPacket.WindowSpawn = false;
            m_pPacket.SharedBattlePower = (uint) (SharedBattlePower + SyndicateBattlePower);
            pTarget.Send(m_pPacket);
            if (Syndicate != null)
            {
                Syndicate.SendName(pTarget);
            }
        }

        public void SendWindowSpawnTo(Character pTarget)
        {
            m_pPacket.WindowSpawn = true;
            m_pPacket.SharedBattlePower = (uint)(SharedBattlePower + SyndicateBattlePower);
            pTarget.Send(m_pPacket);
        }

        public void Send(byte[] pBuffer)
        {
            if (m_pOwner != null)
                m_pOwner.Send(pBuffer);
        }

        public void Send(string szMessage, ChatTone pTone = ChatTone.TOP_LEFT)
        {
            if (m_pOwner != null)
                m_pOwner.Send(new MsgTalk(szMessage, pTone));
        }

        public void Send(string szMessage, ChatTone pTone, Color pColor)
        {
            if (m_pOwner != null)
                m_pOwner.Send(new MsgTalk(szMessage, pTone, pColor));
        }

        public void SendExtraBattlePower()
        {
            if (SyndicateBattlePower > 0)
            {
                UpdateClient(ClientUpdateType.GUILD_BATTLEPOWER, SyndicateBattlePower, true);
            }
            if (SharedBattlePower > 0)
            {
                UpdateClient(ClientUpdateType.EXTRA_BATTLE_POWER, SharedBattlePower, true);
            }
        }

        public void Disconnect()
        {
            if (m_pOwner != null)
                m_pOwner.Disconnect();
        }

        public void Disconnect(string szMsg)
        {
            if (m_pOwner != null)
            {
                m_pOwner.Disconnect();
                ServerKernel.Log.SaveLog(string.Format("kickoutsocket {0}:{1}", Name, szMsg), true, LogType.MESSAGE);
            }
        }

        /// <summary>
        /// This method exchanges a character's spawn packet with another character's spawn packet. The packets of
        /// opposing characters are sent as long as each character is visible. If the actor is invisible, the observer
        /// will not be sent the packet.
        /// </summary>
        /// <param name="pPlayer">The observer of the spawn transaction.</param>
        public void ExchangeSpawnPackets(Character pPlayer)
        {
            SendSpawnTo(pPlayer);
            pPlayer.SendSpawnTo(this);
        }

        public void RemoveFromMapThread(bool isDelete = false)
        {
            LoginComplete = false;
            if (TradePartners != null)
            {
                foreach (var partner in TradePartners.Values)
                {
                    if (partner.TargetOnline)
                    {
                        var packet = partner.ToArray(TradePartnerType.BREAK_PARTNERSHIP);
                        partner.Owner.Send(packet);
                        packet.Online = false;
                        packet.Identity = Identity;
                        packet.Name = Name;
                        partner.Owner.Send(packet);
                    }
                }
            }

            if (Map != null && Map.IsRecordDisable())
            {
                Map map;
                if (ServerKernel.Maps.TryGetValue(m_dbUser.MapId, out map))
                {
                    if (map.IsRecordDisable())
                    {
                        SetRecordPos(1002, 430, 378);
                    }
                    else
                    {
                        if (map[m_dbUser.MapX, m_dbUser.MapY].Access > TileType.MONSTER)
                        {
                            SetRecordPos(m_dbUser.MapId, m_dbUser.MapX, m_dbUser.MapY);
                        }
                        else
                        {
                            SetRecordPos(1002, 430, 378);
                        }
                    }
                }
                else
                {
                    SetRecordPos(1002, 430, 378);
                }
            }
            else
            {
                if (Map != null && Map[MapX, MapY].Access <= TileType.MONSTER)
                {
                    SetRecordPos(1002, 430, 378);
                }
            }

            if (ServerKernel.ArenaQualifier.IsInsideMatch(Identity))
            {
                ArenaMatch pMatch = ServerKernel.ArenaQualifier.FindUser(Identity);
                //if (pMatch.User1 == this)
                //    pMatch.Finish(pMatch.User2, this, false);
                //else
                //    pMatch.Finish(this, pMatch.User1, false);
                pMatch.Finish(this);
            } 
            else if (ServerKernel.ArenaQualifier.IsWaitingMatch(Identity))
            {
                ServerKernel.ArenaQualifier.Uninscribe(this);
            }

            if (ArenaQualifier != null)
            {
                ArenaQualifier.Status = ArenaWaitStatus.NOT_SIGNED_UP;
            }

            if (Team != null && Team.Leader == this)
                Team.Destroy(this, new MsgTeam
                {
                    Type = TeamActionType.DISMISS,
                    Target = Identity
                });
            else if (Team != null)
                Team.LeaveTeam(this, null);

            if (Trade != null)
                Trade.CloseWindow();

            if (Booth != null && Booth.Vending)
                Booth.Destroy();

            if (Map != null)
                Map.RemoveClient(Identity);

            if (Friends != null)
            {
                var msg = new MsgFriend
                {
                    Identity = Identity,
                    Name = Name,
                    Online = false,
                    Mode = RelationAction.SET_OFFLINE_FRIEND
                };
                foreach (var fr in Friends.Values.Where(x => x.IsOnline))
                    fr.User.Send(msg);
            }

            if (Enemies != null)
            {
                var msg = new MsgFriend
                {
                    Identity = Identity,
                    Name = Name,
                    Online = false,
                    Mode = RelationAction.SET_OFFLINE_ENEMY
                };
                foreach (var en in Enemies.Values.Where(x => x.IsOnline))
                    en.User.Send(msg);
            }

            if (Mentor != null && Mentor.IsOnline)
            {
                Mentor.Role.Send(string.Format(ServerString.STR_GUIDE_STUDENT_OFFLINE, Name));
            }

            if (Apprentices != null && Apprentices.Count > 0)
            {
                foreach (var appr in Apprentices.Values.Where(x => x.IsOnline))
                {
                    appr.Role.Send(string.Format(ServerString.STR_GUIDE_TUTOR_OFFLINE));
                    appr.Role.UpdateClient(ClientUpdateType.EXTRA_BATTLE_POWER, 0, true);
                    appr.Role.RecalculateAttributes();
                }
            }

            m_dbUser.LastLogout = (uint) UnixTimestamp.Timestamp();

            if (!isDelete)
                Save();

            //MapIdentity = 0;
            Map = null;
            Booth = null;
            Trade = null;
            Syndicate = null;
            Nobility = null;
            Owner.Screen = null;

            MapX = 0;
            MapY = 0;

            if (Inventory != null && Equipment != null &&
                Equipment.Items != null && Inventory.Items != null)
            {
                foreach (var item in Equipment.Items.Values)
                    item.Save();
                foreach (var item in Inventory.Items.Values)
                    item.Save();
            }

            ServerKernel.Log.SaveLog(string.Format("User [{0}] has disconnected.", Name), true, LogType.MESSAGE);
        }

        #endregion

        #region Database

        public bool Save()
        {
            return Database.Characters.SaveOrUpdate(m_dbUser);
        }

        public bool Delete()
        {
            if (GetMateRole() != null)
                GetMateRole().Mate = "None";
            else
            {
                DbUser mate = new CharacterRepository().SearchByName(Mate);
                if (mate != null)
                {
                    mate.Mate = "None";
                    Database.Characters.SaveOrUpdate(mate);
                }
            }

            return new DeletedCharacterRepository().DeleteUser(Identity) > 0;
        }

        #endregion

        #region Map Item

        public void SaveStatus(uint dwStatus, uint dwTime, uint dwTimes, uint dwInterval)
        {
            uint now = (uint)UnixTimestamp.Timestamp();
            DbStatus dbTemp = new DbStatus
            {
                OwnerId = Identity,
                EndTime = now + dwTime,
                IntervalTime = dwInterval,
                LeaveTimes = dwTimes,
                Power = 0,
                RemainTime = dwTime,
                Sort = 0,
                Status = dwStatus
            };
            new StatusRepository().SaveOrUpdate(dbTemp);
        }

        public bool DropItem(uint idItem, int x, int y)
        {
            var pos = new Point(x, y);
            if (!Map.FindDropItemCell(9, ref pos))
                return false;

            Item pItem = null;
            bool bDropItem = false;
            if (!Inventory.Items.TryGetValue(idItem, out pItem))
                return false;

            ServerKernel.Log.GmLog("drop_item", string.Format("{0}({1}) drop item:[id={2}, type={3}], dur={4}, max_dur={5}", Name, Identity, pItem.Identity, pItem.Type, pItem.Durability, pItem.MaximumDurability));

            if (pItem.CanBeDropped() && pItem.DisappearWhenDropped())
                return Inventory.Remove(pItem.Identity, ItemRemovalType.DELETE);

            if (pItem.CanBeDropped())
            {
                Inventory.Remove(pItem.Identity, ItemRemovalType.DROP_ITEM);
                bDropItem = true;
            }
            else
            {
                Send(ServerString.STR_ITEM_CANNOT_DISCARD);
                return false;
            }

            pItem.PlayerIdentity = 0;
            pItem.OwnerIdentity = Identity;

            if (bDropItem)
            {
                var pMapItem = new MapItem();
                if (pMapItem.Create((uint)Map.FloorItem, Map, pos, pItem, Identity))
                {
                    pItem.Save();
                    //Map.AddItem(pMapItem);
                }
                else
                {
                    Send(ServerString.STR_ITEM_FAILD_TO_CREATE);
                    pMapItem = null;
                }
            }

            return true;
        }

        public bool PickMapItem(uint idItem)
        {
            var pMapItem = Map.QueryRole(idItem) as MapItem;
            if (pMapItem == null) return false;

            if (GetDistance(pMapItem) > 0)
            {
                Send(ServerString.STR_TARGET_NOT_IN_RANGE);
                return false;
            }

            if (!pMapItem.IsMoney()
                && Inventory.RemainingSpace() <= 0)
            {
                Send(ServerString.STR_INVENTORY_FULL);
                return false;
            }

            uint idOwner = pMapItem.GetPlayerId();
            if (pMapItem.IsPriv() && idOwner != Identity)
            {
                var pOwner = Map.QueryRole(idOwner) as Character;
                if (pOwner != null && !IsMate(pOwner))
                {
                    if (Team != null && Team.IsTeamMember(pOwner))
                    {
                        if ((pMapItem.IsMoney() && Team.IsCloseMoney) ||
                            ((pMapItem.Type == SpecialItem.TYPE_DRAGONBALL ||
                              pMapItem.Type == SpecialItem.TYPE_METEOR) && Team.IsCloseGem)
                            || (!pMapItem.IsMoney() && pMapItem.Type != SpecialItem.TYPE_DRAGONBALL &&
                                pMapItem.Type != SpecialItem.TYPE_METEOR && Team.IsCloseItem))
                        {
                            Send(ServerString.STR_CANT_PICKUP_OTHER_ITEMS);
                            return false;
                        }
                    }
                    else
                    {
                        Send(ServerString.STR_CANT_PICKUP_OTHER_ITEMS);
                        return false;
                    }
                } 
                else if (pOwner != null && pOwner != this)
                {
                    Send(ServerString.STR_CANT_PICKUP_OTHER_ITEMS);
                    return false;
                }
            }

            if (pMapItem.IsMoney())
            {
                AwardMoney(pMapItem.GetAmount());
                if (pMapItem.GetAmount() > 1000)
                    Send(new MsgAction(Identity, pMapItem.GetAmount(), MapX, MapY, GeneralActionType.GET_MONEY));
                Send(string.Format(ServerString.STR_PICK_MONEY, pMapItem.GetAmount()));
            }
            else
            {
                var pItem = pMapItem.GetInfo(this);
                if (pItem != null)
                {
                    Inventory.Add(pItem);

                    if (pItem.Type == SpecialItem.TYPE_DRAGONBALL
                        && Owner.VipLevel >= 2
                        && Inventory.ContainsMultiple(SpecialItem.TYPE_DRAGONBALL, SpecialItem.TYPE_DRAGONBALL, 10))
                    {
                        Inventory.DeleteMultiple(SpecialItem.TYPE_DRAGONBALL, SpecialItem.TYPE_DRAGONBALL, 10);
                        Inventory.Create(SpecialItem.TYPE_DRAGONBALL_SCROLL);
                    }

                    if (pItem.Type == SpecialItem.TYPE_METEOR
                        && Owner.VipLevel >= 2
                        && Inventory.ContainsMultiple(SpecialItem.TYPE_METEOR, SpecialItem.TYPE_METEOR, 10))
                    {
                        Inventory.DeleteMultiple(SpecialItem.TYPE_METEOR, SpecialItem.TYPE_METEOR, 10);
                        Inventory.Create(SpecialItem.TYPE_METEOR_SCROLL);
                    }
                    ServerKernel.Log.GmLog("pick_item", string.Format("{0}({1}) pick item:[id={2}, type={3}], dur={4}, max_dur={5}", Name, Identity, pItem.Identity, pItem.Type, pItem.Durability, pItem.MaximumDurability));
                    Send(string.Format(ServerString.STR_GOT_ITEM, pItem.Itemtype.Name));
                }
            }

            Map.RemoveItem(pMapItem, false);
            var msg = new MsgMapItem
            {
                DropType = 3,
                Identity = pMapItem.Identity,
                MapX = pMapItem.MapX,
                MapY = pMapItem.MapY
            };
            Map.SendToRange(msg, pMapItem.MapX, pMapItem.MapY);
            msg.DropType = 2;
            Map.SendToRange(msg, pMapItem.MapX, pMapItem.MapY);

            return true;
        }

        #endregion

        #region Event

        public void CancelQuiz()
        {
            m_nQuizCancelTime = UnixTimestamp.Timestamp();
            ServerKernel.QuizShow.Cancel(Identity);
        }

        public bool QuizCanceled
        {
            get { return m_nQuizCancelTime + 60 * 60 * 30 > UnixTimestamp.Timestamp(); }
        }

        #endregion

        #region Equipment

        public void AddEquipmentDurability(ItemPosition pos, int nInc)
        {
            if (nInc >= 0)
                return;

            Item item;
            if (!Equipment.Items.TryGetValue(pos, out item)
                || !item.IsEquipment()
                || item.GetItemSubtype() == 2100)
                return;

            ushort nOldDur = item.Durability;
            ushort nDurability = (ushort)Math.Max(0, item.Durability + nInc);

            if (nDurability < 100)
            {
                if ((nDurability % 10) == 0)
                    Send(string.Format(ServerString.STR_DAMAGED_REPAIR, item.Itemtype.Name));
            }
            else if (nDurability < 200)
            {
                if (nDurability % 10 == 0)
                    Send(string.Format(ServerString.STR_DURABILITY_REPAIR, item.Itemtype.Name));
            }

            item.Durability = nDurability;

            int noldDur = (int)Math.Floor(nOldDur / 100f);
            int nnewDur = (int)Math.Floor(nDurability / 100f);

            if (noldDur != nnewDur
                || nDurability <= 0)
            {
                MsgItem msg = item.UsagePacket(ItemAction.DURABILITY);
                Send(msg);
            }
        }

        #endregion

        #region Movement

        public void ProcessOnMove()
        {
            Action = EntityAction.STAND;

            if (Booth != null && Booth.Vending)
                Booth.Destroy();

            BattleSystem.DestroyAutoAttack();
            BattleSystem.ResetBattle();

            if (BattleSystem.QueryMagic() != null && Map.IsTrainingMap())
            {
                BattleSystem.QueryMagic().SetMagicState(0);
                BattleSystem.QueryMagic().BreakAutoAttack();
                BattleSystem.QueryMagic().AbortMagic(true);
            }

            if (QueryStatus(FlagInt.LUCKY_DIFFUSE) != null)
                DetachStatus(FlagInt.LUCKY_DIFFUSE);
            if (QueryStatus(FlagInt.LUCKY_ABSORB) != null)
                DetachStatus(FlagInt.LUCKY_ABSORB);

            m_tRespawn.Clear();

            if (m_tMine.IsActive())
                StopMine();

            ProcessAfterMove();
        }

        public void ProcessOnAttack()
        {
            Action = EntityAction.STAND;

            if (Booth != null && Booth.Vending)
                Booth.Destroy();

            //if (BattleSystem.QueryMagic() != null && Map.IsTrainingMap())
            //{
            //    BattleSystem.QueryMagic().SetMagicState(0);
            //    BattleSystem.QueryMagic().BreakAutoAttack();
            //    BattleSystem.QueryMagic().AbortMagic(true);
            //}

            Item temp;
            if (QueryStatus(FlagInt.RIDING) != null && !Equipment.Items.TryGetValue(ItemPosition.CROP, out temp))
                DetachStatus(FlagInt.RIDING);
            if (QueryStatus(FlagInt.LUCKY_DIFFUSE) != null)
                DetachStatus(FlagInt.LUCKY_DIFFUSE);
            if (QueryStatus(FlagInt.LUCKY_ABSORB) != null)
                DetachStatus(FlagInt.LUCKY_ABSORB);

            m_tRespawn.Clear();

            if (m_tMine.IsActive())
                StopMine();
        }

        public void ProcessOnJump()
        {
            Action = EntityAction.STAND;

            if (Booth != null && Booth.Vending)
                Booth.Destroy();

            BattleSystem.DestroyAutoAttack();
            BattleSystem.ResetBattle();

            if (BattleSystem.QueryMagic() != null && Map.IsTrainingMap())
            {
                BattleSystem.QueryMagic().SetMagicState(0);
                BattleSystem.QueryMagic().BreakAutoAttack();
                BattleSystem.QueryMagic().AbortMagic(true);
            }

            if (QueryStatus(FlagInt.LUCKY_DIFFUSE) != null)
                DetachStatus(FlagInt.LUCKY_DIFFUSE);
            if (QueryStatus(FlagInt.LUCKY_ABSORB) != null)
                DetachStatus(FlagInt.LUCKY_ABSORB);

            m_tRespawn.Clear();

            if (m_tMine.IsActive())
                StopMine();

            ProcessAfterMove();
        }

        public void ProcessAfterMove()
        {
            if (Screen.ContainsTrap())
            {
                var traps = Screen.GetTraps;
                foreach (var trap in traps)
                {
                    if (GetDistance(trap) <= 1 && trap is EventFlag)
                    {
                        EventFlag flag = trap as EventFlag;
                        if (flag.Grab(this))
                            ServerKernel.CaptureTheFlag.AddPoints(this, 3);
                    }
                }
            }
        }

        public void ChangeMap(ushort x, ushort y, uint map, bool saveOld = false, bool ignoreInvalid = false)
        {
            if (map == 0 || x == 0 || y == 0)
                return;

            Map _map;

            if (!ServerKernel.Maps.TryGetValue(map, out _map))
                return;

            if (!_map.Loaded)
                if (!_map.Load())
                {
                    ServerKernel.Log.SaveLog(string.Format("Could not load map [{0}]", map), false, LogType.WARNING);
                    return;
                }

            if (_map.IsRecordDisable() && saveOld)
                SetRecordPos(m_dwMapIdentity, m_usMapX, m_usMapY);

            try
            {
                Tile tile = _map[x, y];
                if (tile.Access <= TileType.MONSTER)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        ushort newX = (ushort)(x + Handlers.DELTA_WALK_X_COORDS[i]),
                            newY = (ushort)(y + Handlers.DELTA_WALK_Y_COORDS[i]);
                        if ((tile = _map[x, y]).Access > TileType.MONSTER)
                        {
                            x = newX;
                            y = newY;
                        }
                    }

                    if (tile.Access <= TileType.MONSTER)
                    {
                        Owner.Send(new MsgTalk("You can't stand there.", ChatTone.TOP_LEFT));
                        return;
                    }
                }
            }
            catch
            {
                Owner.Send(new MsgTalk("You can't stand there.", ChatTone.TOP_LEFT));
                return;
            }

            bool same = true;
            if (map != MapIdentity || !Calculations.InScreen(x, y, MapX, MapY))
            {
                //if (ServerKernel.SkillPkTournament.IsParticipant(Identity))
                //{
                //    ServerKernel.SkillPkTournament.Expell(Identity);
                //}

                Map.RemoveClient(Identity);
                MapIdentity = map;
                same = false;
            }
            
            Action = EntityAction.STAND;

            if (!same)
            {
                Owner.Send(new MsgAction(Identity, _map.BaseIdentity, x, y,
                    GeneralActionType.USE_PORTAL)
                {
                    Details = _map.Type,
                    Map = _map.MapDoc
                });
            }

            var broad = new MsgAction(Identity, _map.BaseIdentity, x, y,
                same ? GeneralActionType.NEW_COORDINATES : GeneralActionType.CHANGE_MAP)
            {
                Details = (byte)(_map.Type),
                Map = Owner.Identity
            };
            Owner.Send(broad);
            Owner.Send(new MsgMapInfo(_map.Identity, _map.BaseIdentity, _map.Type));

            if (!same)
            {
                _map.AddClient(this);
                Owner.Screen.LoadSurroundings();
            }
            else
                Owner.Screen.RefreshSpawnForObservers();

            if (!Map.IsRecordDisable())
                Save();

            MapX = x;
            MapY = y;
            m_tRespawn.SetInterval(CHGMAP_LOCK_SECS);
            m_tRespawn.Update();

            ProcessOnMove();

            RecalculateAttributes();
        }

        public void SetRecordPos(uint mapId, ushort mapX, ushort mapY)
        {
            m_dbUser.MapId = mapId;
            m_dbUser.MapX = mapX;
            m_dbUser.MapY = mapY;

            RecordMapIdentity = mapId;
            RecordMapX = mapX;
            RecordMapY = mapY;

            Save();
        }

        public bool Walk(FacingDirection pDir)
        {
            m_pDirection = pDir;
            int nDirX = Handlers.WALK_X_COORDS[((int)pDir % 8)];
            int nDirY = Handlers.WALK_Y_COORDS[((int)pDir % 8)];

            if (m_pMap == null)
                return false;

            ushort nNewX = (ushort)(m_usMapX + nDirX);
            ushort nNewY = (ushort)(m_usMapY + nDirY);

            if (!m_pMap.IsValidPoint(nNewX, nNewY))
                return false;

            if (!m_pMap.IsValidPoint(nNewX, nNewY))
            {
                Send(ServerString.STR_INVALID_COORDINATE);
                Kickback(m_usMapX, m_usMapY);
                return false;
            }

            // todo check weather
            m_usMapX = nNewX;
            m_usMapY = nNewY;

            MsgWalk pMsg = new MsgWalk((uint) pDir, Identity, MovementType.WALK, MapIdentity);
            Screen.SendMovement(pMsg);

            ProcessAfterMove();
            return true;
        }

        public bool MoveToward(FacingDirection pDir, bool bSync)
        {
            m_pDirection = pDir;
            int nDirX = Handlers.WALK_X_COORDS[((int)pDir % 8)];
            int nDirY = Handlers.WALK_Y_COORDS[((int)pDir % 8)];

            if (m_pMap == null)
                return false;

            ushort nNewX = (ushort)(m_usMapX + nDirX);
            ushort nNewY = (ushort)(m_usMapY + nDirY);

            if (!m_pMap.IsValidPoint(nNewX, nNewY))
                return false;

            if (!m_pMap.IsValidPoint(nNewX, nNewY))
            {
                Send(ServerString.STR_INVALID_COORDINATE);
                Kickback(m_usMapX, m_usMapY);
                return false;
            }

            // todo check weather
            m_usMapX = nNewX;
            m_usMapY = nNewY;

            ProcessAfterMove();
            return true;
        }

        public bool SynchroPosition(int nPosX, int nPosY, int nMaxDislocation = 8)
        {
            if (nMaxDislocation <= 0 || (nPosX == 0 && nPosY == 0))	// ignore in this condition
                return true;

            int nDislocation = GetDistance((ushort)nPosX, (ushort)nPosY);
            if (nDislocation >= nMaxDislocation)
                return false;

            if (nDislocation <= 0)
                return true;

            if (!Map.IsValidPoint(nPosX, nPosY))
                return false;

            Kickback((ushort)nPosX, (ushort)nPosY);
            return true;
        }

        public void Kickback(ushort x, ushort y)
        {
            MapX = x;
            MapY = y;
            if (m_pOwner != null)
                m_pOwner.Send(new MsgAction(m_dbUser.Identity, 0, x, y, GeneralActionType.NEW_COORDINATES));
        }

        public bool IsJumpPass(int x, int y, int nAlt)
        {
            var setLine = new List<Point>();
            Calculations.DDALineEx(MapX, MapY, x, y, ref setLine);
            int nSize = setLine.Count;
            if (nSize <= 2)
                return true;

            if (x != setLine[nSize - 1].X)
                return false;
            if (y != setLine[nSize - 1].Y)
                return false;

            try
            {
                float fAlt = (m_pMap[MapX, MapY].Elevation + nAlt + 0.5f);
                for (int i = 0; i < nSize; i++)
                    if (m_pMap.IsAltOver(setLine[i], (int)fAlt))
                        return false;
            }
            catch { ServerKernel.Log.SaveLog(string.Format("ERROR: Could not jump from {0}/{1} to {2}/{3} on map {4}. [{5}]", m_usMapX, m_usMapY, x, y, m_dwMapIdentity, Identity), false); }

            return true;
        }

        public int GetDistance(ushort x, ushort y)
        {
            return (int)Calculations.GetDistance(MapX, MapY, x, y);
        }

        public bool CanUserPortalTeleport
        {
            get
            {
                if (m_tVipPortalTele.IsActive())
                    return m_tVipPortalTele.ToNextTime();
                return m_tVipPortalTele.Update();
            }
        }

        public int PortalTeleportWaitTime
        {
            get { return m_tVipPortalTele.GetRemain(); }
        }

        public bool CanUseCityTeleport
        {
            get
            {
                if (m_tVipCityTele.IsActive())
                    return m_tVipCityTele.ToNextTime();
                return m_tVipCityTele.Update();
            }
        }

        public int CityTeleportWaitTime
        {
            get { return m_tVipCityTele.GetRemain(); }
        }

        public bool SpendEquipItem(uint dwItem, uint dwAmount, bool bSynchro)
        {
            if (dwItem <= 0)
                return false;

            Item item = null;
            if (Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND) && Equipment.Items[ItemPosition.RIGHT_HAND].GetItemSubtype() == dwItem &&
                Equipment.Items[ItemPosition.RIGHT_HAND].Durability >= dwAmount)
                item = Equipment.Items[ItemPosition.RIGHT_HAND];
            else if (Equipment.Items.ContainsKey(ItemPosition.LEFT_HAND) && Equipment.Items[ItemPosition.LEFT_HAND].GetItemSubtype() == dwItem
                     && Equipment.Items[ItemPosition.LEFT_HAND].Durability >= dwAmount)
                item = Equipment.Items[ItemPosition.LEFT_HAND];

            if (item == null)
                return false;

            if (!item.IsExpend() && item.Durability < dwAmount)
                return false;

            if (item.IsExpend() && item.Durability >= dwAmount)
            {
                item.Durability -= (ushort)dwAmount;
                if (bSynchro)
                    Send(item.InformationPacket(true));
            }
            else
            {
                if (item.IsNonsuchItem())
                {
                    ServerKernel.Log.GmLog("SpendEquipItem", string.Format("{0}({1}) Spend item:[id={2}, type={3}], dur={4}, max_dur={5}",
                        Name,
                        Identity,
                        item.Identity,
                        item.Type,
                        item.Durability,
                        item.MaximumDurability));
                }

                // todo break equipment
            }

            return true;
        }

        public bool DecEquipmentDurability(bool bAttack, int hitByMagic, ushort useItemNum)
        {
            int nInc = -1*useItemNum;

            for (ItemPosition i = ItemPosition.HEADWEAR; i <= ItemPosition.CROP; i++)
            {
                if (i == ItemPosition.GARMENT || i == ItemPosition.BOTTLE || i == ItemPosition.STEED
                    || i == ItemPosition.STEED_ARMOR || i == ItemPosition.ACCESSORY_L || i == ItemPosition.ACCESSORY_R)
                    continue;
                if (hitByMagic == 1)
                {
                    if (i == ItemPosition.RING
                        || i == ItemPosition.RIGHT_HAND
                        || i == ItemPosition.LEFT_HAND
                        || i == ItemPosition.BOOTS)
                    {
                        if (!bAttack)
                            AddEquipmentDurability(i, nInc);
                    }
                    else
                    {
                        if (bAttack)
                            AddEquipmentDurability(i, nInc);
                    }
                }
                else
                {
                    if (i == ItemPosition.RING
                        || i == ItemPosition.RIGHT_HAND
                        || i == ItemPosition.LEFT_HAND
                        || i == ItemPosition.BOOTS)
                    {
                        if (!bAttack)
                            AddEquipmentDurability(i, -1);
                    }
                    else
                    {
                        if (bAttack)
                            AddEquipmentDurability(i, nInc);
                    }
                }
            }

            return true;
        }

        public bool ProcessMagicAttack(ushort usMagicType, uint idTarget, ushort x, ushort y, byte ucAutoActive = 0)
        {
            if (Magics != null)
                return Magics.MagicAttack(usMagicType, idTarget, x, y, ucAutoActive);
            return false;
        }

        #endregion

        #region Silence

        public void Silence(int nSeconds) { m_tSilence.Startup(nSeconds); }
        public bool IsSilenced { get { return !m_tSilence.IsTimeOut(); } }
        public void RemoveSilence() { m_tSilence.Clear(); }
        public int SilenceRemaining { get { return m_tSilence.GetRemain(); } }

        #endregion

        #region Freeze

        public bool IsFreeze { get { return !m_tJumpUnable.IsTimeOut(); } }
        public void Freeze(int nMs) { m_tJumpUnable.Startup(nMs); }
        public void RemoveFreeze() { m_tJumpUnable.Clear(); }
        public int FreezeRemaining { get { return m_tJumpUnable.GetRemain(); } }

        #endregion

        #region Arena Qualifier

        public bool IsWatcher
        {
            get { return MapIdentity > 900000 && !IsInsideQualifier; }
        }

        public bool IsInsideQualifier
        {
            get { return ServerKernel.ArenaQualifier.IsInsideMatch(Identity); }
        }

        public ArenaWaitStatus ArenaStatus
        {
            get { return ArenaQualifier == null ? ArenaWaitStatus.NOT_SIGNED_UP : ArenaQualifier.Status; }
        }

        public void SendArenaStatus()
        {
            try
            {
                MsgQualifyingDetailInfo pMsg = new MsgQualifyingDetailInfo();
                if (ArenaQualifier == null)
                    if (!ServerKernel.ArenaQualifier.GenerateFirstData(this))
                        return;

                pMsg.Ranking = ArenaQualifier.Ranking;
                pMsg.ArenaPoints = ArenaQualifier.Points;
                pMsg.CurrentHonor = ArenaQualifier.HonorPoints;
                pMsg.TotalHonor = ArenaQualifier.TotalHonorPoints;
                pMsg.TotalLose = ArenaQualifier.TotalLoses;
                pMsg.TodayWins = ArenaQualifier.TodayWins;
                pMsg.TodayLose = ArenaQualifier.TodayLoses;
                pMsg.TotalWins = ArenaQualifier.TotalWins;
                pMsg.Status = ArenaQualifier.Status;
                Send(pMsg);

                MsgQualifyingFightersList pFightList = new MsgQualifyingFightersList
                {
                    MatchesCount = ServerKernel.ArenaQualifier.Matches,
                    PlayerAmount = ServerKernel.ArenaQualifier.Participants,
                    Showing = 0
                };
                int i = 0;
                foreach (var match in ServerKernel.ArenaQualifier.ArenaMatches)
                {
                    if (i++ >= 6)
                        break;
                    pFightList.AddMatch(match.User1.Identity, match.User1.Lookface, match.User1.Name, match.User1.Level,
                        match.User1.Profession, match.User1.ArenaQualifier.Ranking, match.User1.ArenaQualifier.Points,
                        match.User1.ArenaQualifier.TodayWins, match.User1.ArenaQualifier.TodayLoses,
                        match.User1.ArenaQualifier.HonorPoints,
                        match.User1.ArenaQualifier.TotalHonorPoints, match.User2.Identity, match.User2.Lookface,
                        match.User2.Name, match.User2.Level,
                        match.User2.Profession, match.User2.ArenaQualifier.Ranking, match.User2.ArenaQualifier.Points,
                        match.User2.ArenaQualifier.TodayWins, match.User2.ArenaQualifier.TodayLoses,
                        match.User2.ArenaQualifier.HonorPoints,
                        match.User2.ArenaQualifier.TotalHonorPoints);
                }
                Send(pFightList);
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString());
            }
        }

        public void SendArenaInformation(Character pTarget)
        {
            MsgQualifyingInteractive pMsg = new MsgQualifyingInteractive
            {
                Type = ArenaType.START_THE_FIGHT
            };
            pMsg.Name = Name;
            pMsg.Level = Level;
            pMsg.Profession = Profession;
            pMsg.ArenaPoints = ArenaQualifier.Points;
            pMsg.Rank = ArenaQualifier.Ranking;
            pTarget.Send(pMsg);
        }

        #endregion

        #region Relationship (Friends, Trade Partner, Mentor)

        public void SendRelation(Character pTarget)
        {
            Send(new MsgRelation
            {
                BattlePower = (uint) pTarget.BattlePower,
                IsApprentice = IsMentor(pTarget),
                IsSpouse = pTarget.IsMate(this),
                IsTradePartner = IsTradePartner(pTarget),
                Level = pTarget.Level,
                SenderIdentity = pTarget.Identity,
                TargetIdentity = Identity
            });
        }

        public DbUser GetMate()
        {
            return Mate == "None" ? null : new CharacterRepository().SearchByName(Mate);
        }

        public Character GetMateRole()
        {
            var firstOrDefault = ServerKernel.Players.Values.FirstOrDefault(x => x.Character.Mate == Mate);
            return firstOrDefault == null ? null : firstOrDefault.Character;
        }

        #region Friends
        public bool ContainsFriend(uint idRole) { return Friends.ContainsKey(idRole); }

        public bool CreateFriend(Character pTarget)
        {
            if (ContainsFriend(pTarget.Identity))
            {
                Send(string.Format(ServerString.STR_YOUR_FRIEND_ALREADY));
                return false;
            }

            if (Friends.Count > FRIEND_VIP_AMOUNT[m_pOwner.VipLevel])
            {
                Send(ServerString.STR_FRIEND_YOUR_LIST_FULL);
                return false;
            }
            if (pTarget.Friends.Count > FRIEND_VIP_AMOUNT[pTarget.Owner.VipLevel])
            {
                pTarget.Send(ServerString.STR_FRIEND_LIST_FULL);
                return false;
            }

            SetFriendRequest(0);
            pTarget.SetFriendRequest(0);

            var friend0 = new DbFriend
            {
                UserIdentity = Identity,
                Friend = pTarget.Identity,
                FriendName = pTarget.Name
            };
            var friend1 = new DbFriend
            {
                UserIdentity = pTarget.Identity,
                Friend = Identity,
                FriendName = Name
            };

            Database.Friends.SaveOrUpdate(friend0);
            Database.Friends.SaveOrUpdate(friend1);

            Screen.Send(string.Format(ServerString.STR_MAKE_FRIEND, Name, pTarget.Name), true);

            var newFr = new MsgFriend
            {
                Mode = RelationAction.ADD_FRIEND,
                Identity = pTarget.Identity,
                Name = pTarget.Name,
                Online = true
            };
            Send(newFr);
            newFr = new MsgFriend
            {
                Mode = RelationAction.ADD_FRIEND,
                Identity = Identity,
                Name = Name,
                Online = true
            };
            pTarget.Send(newFr);

            var relation0 = new Relationship
            {
                Database = friend0,
                Identity = pTarget.Identity,
                Name = pTarget.Name
            };
            var relation1 = new Relationship
            {
                Database = friend1,
                Identity = Identity,
                Name = Name
            };
            return Friends.TryAdd(pTarget.Identity, relation0) && pTarget.Friends.TryAdd(Identity, relation1);
        }

        public bool AddFriend(DbFriend dbFriend)
        {
            if (ContainsFriend(dbFriend.Friend)) return false;

            var relation = new Relationship
            {
                Database = dbFriend,
                Identity = dbFriend.Friend,
                Name = dbFriend.FriendName
            };

            var info = new MsgFriend
            {
                Mode = RelationAction.ADD_FRIEND,
                Identity = dbFriend.Friend,
                Name = dbFriend.FriendName,
                Online = relation.IsOnline
            };

            Send(info);

            return Friends.TryAdd(dbFriend.Friend, relation);
        }

        public bool DeleteFriend(uint idRole)
        {
            Relationship pTarget, trash;
            if (Friends.TryRemove(idRole, out pTarget) && pTarget.Database is DbFriend)
            {
                if (pTarget.IsOnline)
                {
                    MsgFriend pMsg = new MsgFriend
                    {
                        Mode = RelationAction.REMOVE_FRIEND,
                        Identity = Identity,
                        Name = Name,
                        Online = true
                    };
                    pTarget.User.Send(pMsg);
                    pTarget.User.Character.Friends.TryRemove(Identity, out trash);
                }

                MsgFriend msg = new MsgFriend
                {
                    Mode = RelationAction.REMOVE_FRIEND,
                    Identity = idRole,
                    Name = pTarget.Name,
                    Online = pTarget.IsOnline
                };
                Send(msg);
                Screen.Send(string.Format(ServerString.STR_BREAK_FRIEND, Name, pTarget.Name), true);
                Database.Friends.DeleteFriends(Identity, idRole);
            }
            return false;
        }

        #endregion

        #region Enemy
        public bool CreateEnemy(Character pTarget)
        {
            if (pTarget == null) return false;
            if (pTarget.Identity == Identity) return false;
            if (Enemies.ContainsKey(pTarget.Identity)) return false;

            var dbEnemy = new DbEnemy
            {
                EnemyIdentity = pTarget.Identity,
                Enemyname = pTarget.Name,
                Time = (uint)UnixTimestamp.Timestamp(),
                UserIdentity = Identity
            };
            Database.Enemies.SaveOrUpdate(dbEnemy);

            Relationship pObj = new Relationship();
            pObj.Identity = pTarget.Identity;
            pObj.Name = pTarget.Name;
            pObj.Database = dbEnemy;

            var pMsg = new MsgFriend
            {
                Identity = pTarget.Identity,
                Name = pTarget.Name,
                Mode = RelationAction.ADD_ENEMY,
                Online = pObj.IsOnline
            };
            Send(pMsg);
            return Enemies.TryAdd(pObj.Identity, pObj);
        }

        public bool AddEnemy(DbEnemy pTarget)
        {
            if (ContainsEnemy(pTarget.EnemyIdentity)) return false;
            if (pTarget.EnemyIdentity == Identity) return false;

            Relationship pObj = new Relationship();
            pObj.Identity = pTarget.EnemyIdentity;
            pObj.Name = pTarget.Enemyname;
            pObj.Database = pTarget;

            var pMsg = new MsgFriend
            {
                Identity = pTarget.EnemyIdentity,
                Name = pTarget.Enemyname,
                Mode = RelationAction.ADD_ENEMY,
                Online = pObj.IsOnline
            };
            Send(pMsg);

            return Enemies.TryAdd(pTarget.EnemyIdentity, pObj);
        }

        public bool DeleteEnemy(uint idTarget)
        {
            Relationship pTarget;
            if (Enemies.TryRemove(idTarget, out pTarget) && pTarget.Database is DbEnemy)
            {
                var pMsg = new MsgFriend
                {
                    Identity = idTarget,
                    Name = pTarget.Name,
                    Mode = RelationAction.REMOVE_ENEMY,
                    Online = pTarget.IsOnline
                };
                Send(pMsg);
                return Database.Enemies.Delete(pTarget.Database as DbEnemy);
            }
            return false;
        }

        public bool ContainsEnemy(uint idRole) { return Enemies.ContainsKey(idRole); }

        #endregion

        #region Mentor and Apprentice

        public void FetchMentorAndApprentice()
        {
            DbMentorAccess dbMentorReward = Database.MentorAccess.FetchByUser(Identity);
            if (dbMentorReward != null)
            {
                m_fMentorExp = (uint)(dbMentorReward.Experience);
                m_usMentorBless = dbMentorReward.Blessing;
                m_usMentorCompose = dbMentorReward.Composition;
            }

            if (Mentor != null)
            {
                MentorContribution dbStudentContribution = Database.MentorContribution.FetchInformation(Mentor.Identity, Identity);
                if (dbStudentContribution != null)
                {
                    m_dwStudentExp = (dbStudentContribution.Experience);
                    m_usStudentBless = dbStudentContribution.GodTime;
                    m_dwStudentCompose = dbStudentContribution.PlusStone;
                }
            }
        }

        /// <summary>
        /// The claimable experience
        /// </summary>
        public uint MentorExperience
        {
            get { return (uint)m_fMentorExp; }
        }

        public ushort MentorBlessing
        {
            get { return m_usMentorBless; }
        }

        public ushort MentorComposition
        {
            get { return m_usMentorCompose; }
        }

        public uint StudentExperience
        {
            get { return m_dwStudentExp; }
        }

        public ushort StudentBlessing
        {
            get { return m_usStudentBless; }
        }

        public uint StudentComposition
        {
            get { return m_dwStudentCompose; }
        }

        public void AddMentorExperience(uint ulAmount)
        {
            if (Mentor == null) return;

            DbMentorAccess dbMentorReward = null;
            MentorContribution dbStudentContribution = null;

            dbMentorReward = Database.MentorAccess.FetchByUser(Mentor.Identity);
            dbStudentContribution = Database.MentorContribution.FetchInformation(Mentor.Identity, Identity);

            if (dbMentorReward == null)
            {
                dbMentorReward = new DbMentorAccess
                {
                    GuideIdentity = Mentor.Identity
                };
                Database.MentorAccess.SaveOrUpdate(dbMentorReward);
            }

            if (dbStudentContribution == null)
            {
                dbStudentContribution = new MentorContribution
                {
                    TutorIdentity = Mentor.Identity,
                    StudentIdentity = Identity,
                    Experience = 0,
                    GodTime = 0,
                    PlusStone = 0,
                    StudentName = Name
                };
                Database.MentorContribution.SaveOrUpdate(dbStudentContribution);
            }

            uint expballUnity = (uint)(ServerKernel.GetExpBallExperience(Level) / 600f);
            float usAmount = (ulAmount / (float)expballUnity);

            m_fMentorExp += usAmount;
            dbMentorReward.Experience += (uint)usAmount;
            dbStudentContribution.Experience += (uint)usAmount;

            Database.MentorAccess.SaveOrUpdate(dbMentorReward);
            Database.MentorContribution.SaveOrUpdate(dbStudentContribution);
        }

        public void AddMentorComposing(ushort usAmount)
        {
            if (Mentor == null) return;

            DbMentorAccess dbMentorReward = null;
            MentorContribution dbStudentContribution = null;

            dbMentorReward = Database.MentorAccess.FetchByUser(Mentor.Identity);
            dbStudentContribution = Database.MentorContribution.FetchInformation(Mentor.Identity, Identity);

            if (dbMentorReward == null)
            {
                dbMentorReward = new DbMentorAccess
                {
                    GuideIdentity = Mentor.Identity
                };
                Database.MentorAccess.SaveOrUpdate(dbMentorReward);
            }

            if (dbStudentContribution == null)
            {
                dbStudentContribution = new MentorContribution
                {
                    TutorIdentity = Mentor.Identity,
                    StudentIdentity = Identity,
                    Experience = 0,
                    GodTime = 0,
                    PlusStone = 0,
                    StudentName = Name
                };
                Database.MentorContribution.SaveOrUpdate(dbStudentContribution);
            }

            dbMentorReward.Composition += usAmount;
            m_usMentorCompose += usAmount;
            dbStudentContribution.PlusStone += usAmount;

            Database.MentorAccess.SaveOrUpdate(dbMentorReward);
            Database.MentorContribution.SaveOrUpdate(dbStudentContribution);
        }

        public void AddMentorBlessing(ushort usAmount)
        {
            if (Mentor == null) return;

            DbMentorAccess dbMentorReward = null;
            MentorContribution dbStudentContribution = null;

            dbMentorReward = Database.MentorAccess.FetchByUser(Mentor.Identity);
            dbStudentContribution = Database.MentorContribution.FetchInformation(Mentor.Identity, Identity);

            if (dbMentorReward == null)
            {
                dbMentorReward = new DbMentorAccess
                {
                    GuideIdentity = Mentor.Identity
                };
                Database.MentorAccess.SaveOrUpdate(dbMentorReward);
            }

            if (dbStudentContribution == null)
            {
                dbStudentContribution = new MentorContribution
                {
                    TutorIdentity = Mentor.Identity,
                    StudentIdentity = Identity,
                    Experience = 0,
                    GodTime = 0,
                    PlusStone = 0,
                    StudentName = Name
                };
                Database.MentorContribution.SaveOrUpdate(dbStudentContribution);
            }

            dbMentorReward.Blessing += usAmount;
            m_usMentorBless += usAmount;
            dbStudentContribution.GodTime += usAmount;

            Database.MentorAccess.SaveOrUpdate(dbMentorReward);
            Database.MentorContribution.SaveOrUpdate(dbStudentContribution);
        }

        public void ClaimStudentExperience()
        {
            DbMentorAccess dbMentorReward = Database.MentorAccess.FetchByUser(Identity);
            if (dbMentorReward == null)
            {
                Send("You don\'t have any reward to claim.");
                return;
            }
            long expball = (long)(ServerKernel.GetExpBallExperience(Level) / 600f);
            long exp = (long)dbMentorReward.Experience * expball;
            AwardExperience(exp);
            dbMentorReward.Experience = 0;
            Database.MentorAccess.SaveOrUpdate(dbMentorReward);
        }

        public void ClaimStudentBlessing()
        {
            DbMentorAccess m_dbMentorReward = Database.MentorAccess.FetchByUser(Identity);
            if (m_dbMentorReward == null)
            {
                Send("You don\'t have any reward to claim.");
                return;
            }
            AddBlessing(m_dbMentorReward.Blessing);
            m_dbMentorReward.Blessing = 0;
            Database.MentorAccess.SaveOrUpdate(m_dbMentorReward);
        }

        public void ClaimStudentComposing()
        {
            DbMentorAccess m_dbMentorReward = Database.MentorAccess.FetchByUser(Identity);
            if (m_dbMentorReward == null)
            {
                Send("You don\'t have any reward to claim.");
                return;
            }
            int amount = m_dbMentorReward.Composition >= 1 ? m_dbMentorReward.Composition / 100 : 0;
            if (amount > 0 && Inventory.RemainingSpace() >= amount)
            {
                for (int i = 0; i < amount; i++)
                {
                    Inventory.Create(SpecialItem.TYPE_STONE1);
                }
                m_dbMentorReward.Composition -= (ushort)(amount * 100);
                Database.MentorAccess.SaveOrUpdate(m_dbMentorReward);
            }
        }

        #endregion

        #endregion

        #region Constants

        public const int ADD_ENERGY_STAND_SECS = 1; // Ã¿¶àÉÙÃëÔö¼ÓÒ»´Î
        public const int ADD_ENERGY_STAND = 7; // Õ¾×ÅÔö¼ÓÊýÁ¿
        public const int DEL_ENERGY_PELT_SECS = 3; // ¼²ÐÐ×´Ì¬Ã¿3Ãë¼õÒ»´Î
        public const int DEL_ENERGY_PELT = 2; // Ã¿´Î¿Û2µã
        public const int KEEP_STAND_MS = 1500; // Õ¾ÎÈÐèÒªµÄÊ±¼ä£¬Õ¾ÎÈºóÄÜµ²×¡Íæ¼Ò¡£
        public const int CRUSH_ENERGY_NEED = 100; // ¼·¿ªÐèÒª¶àÉÙµã
        public const int CRUSH_ENERGY_EXPEND = 100; // ¼·¿ªÏûºÄ¶àÉÙµã
        public const int SYNWAR_PROFFER_PERCENT = 1; // ´òÖù×ÓµÃ¾­ÑéÖµµÄ°Ù·Ö±È
        public const int SYNWAR_MONEY_PERCENT = 2; // ´òÖù×ÓµÃÇ®µÄ°Ù·Ö±È
        public const int MIN_SUPERMAP_KILLS = 25; // ÎÞË«ÁÐ±íµÍÏÞ
        public const int NEWBIE_LEVEL = 30; // ÐÂÊÖµÄµÈ¼¶
        public const int VETERAN_DIFF_LEVEL = 20; // ÀÏÊÖµÄµÈ¼¶²î
        public const int MIN_TUTOR_LEVEL = 50; // µ¼Ê¦×îµÍµÈ¼¶
        public const int HIGHEST_WATER_WIZARD_PROF = 135; // Ë®ÕæÈË
        public const int MOUNT_ADD_INTIMACY_SECS = 3600; // 
        public const int INTIMACY_DEAD_DEC = 20; // 
        public const int SLOWHEALLIFE_MS = 1000; // ³ÔÒ©²¹ÑªÂýÂý²¹ÉÏÈ¥µÄºÀÃëÊý¡£
        public const int AUTOHEALLIFE_TIME = 10; // Ã¿¸ô10Ãë×Ô¶¯²¹Ñª1´Î¡£
        public const int AUTOHEALLIFE_EACHPERIOD = 6; // Ã¿´Î²¹Ñª6¡£
        public const int AUTOHEAL_MAXLIFE_TIME = 60; // Ã¿¸ô60Ãë×Ô¶¯»Ö¸´maxlife
        public const int AUTOHEAL_MAXLIFE_FLESH_WOUND = 16; // ÇáÉË×´Ì¬ÏÂÃ¿´Î»Ö¸´maxlifeµÄÇ§·Ö±È
        public const int AUTOHEAL_MAXLIFE_GBH = 4; // ÖØÉË×´Ì¬ÏÂÃ¿´Î»Ö¸´maxlifeµÄÇ§·Ö±È
        public const int TICK_SECS = 10;
        public const int MAX_PKLIMIT = 10000; // PKÖµµÄ×î´ó×îÐ¡ÏÞÖÆ(·À»Ø¾í)  //change huang 2004.1.11
        public const int PILEMONEY_CHANGE = 5000; // ´óÁ¿ÏÖ½ð±ä¶¯(ÐèÒª¼´Ê±´æÅÌ)
        public const int ADDITIONALPOINT_NUM = 3; // Éý1¼¶¼Ó¶àÉÙÊôÐÔµã
        public const int PK_DEC_TIME = 180; // Ã¿¼ä¸ô60Ãë½µµÍpkÖµ     //change huang 2004.1.11   
        public const int PKVALUE_DEC_ONCE = 1; // °´Ê±¼ä¼õÉÙPKÖµ¡£        
        public const int PKVALUE_DEC_ONCE_IN_PRISON = 3; // °´Ê±¼ä¼õÉÙPKÖµ¡ª¡ªÔÚ¼àÓüÖÐµÄÇé¿ö
        public const int PKVALUE_DEC_PERDEATH = 10; // ±»PKËÀÒ»´Î¼õÉÙµÄPKÖµ
        public const int TIME_TEAMPRC = 5; // ¶ÓÎéÐÅÏ¢´¦ÀíµÄÊ±¼ä
        public const int DURABILITY_DEC_TIME = 12; // Ã¿12Ãë½µµÍÒ»´Î°´Ê±¼äÏûºÄµÄ×°±¸ÄÍ¾Ã¶È
        public const int USER_ATTACK_SPEED = 800; // Íæ¼ÒÍ½ÊÖ¹¥»÷ÆµÂÊ
        public const int POISONDAMAGE_INTERVAL = 2; // ÖÐ¶¾Ã¿2ÃëÉËÑªÒ»´Î
        public const int WARGHOST_CHECK_INTERVAL = 10; // Ã¿¸ô10Ãë¼ì²éÒ»´ÎÕ½»êµÈ¼¶(ÎäÆ÷¾­Ñé)
        public const int AUTO_REBORN_SECS = 30; // ¸´»î±¦Ê¯30Ãëºó×Ô¶¯Ê¹ÓÃ
        public const int INC_POTENTIAL_SECS = 6 * 60; // Ã¿¸ô6·ÖÖÓÔö¼ÓÒ»´ÎÇ±ÄÜ
        public const int INC_POTENTIAL_NUM = 1; // Ã¿´ÎÔö¼Ó1µãÇ±ÄÜ
        public const int ADD_POTENTIAL_RELATIONSHIP = 6; // ¹ØÏµÖµÖ®ºÍÃ¿´ïµ½6Ôö¼Ó1µãÇ±Á¦Öµ

        public const int WORLD_CHAT_MIN_LEVEL = 70;
        public const int WORLD_CHAT_DELAY_100 = 30; // 2016-11-10 world chat player under level 100
        public const int WORLD_CHAT_DELAY_110 = 15; // 2016-11-10 world chat player under level 110
        public const int WORLD_CHAT_DELAY_120 = 10; // 2016-11-10 world chat player under level 120
        public const int WORLD_CHAT_ADD_DELAY = 15; // 2016-11-10 world chat addition if player has no reborn

        public const int SPRITE_ADD_EXP_SECS = 60; //ÔªËØ¾«ÁéÃ¿·ÖÖÓÔö¼ÓÒ»´Î¾­Ñé
        public const int SPRITE_ADD_EXP = 1; //¾­ÑéÔö¼ÓÊýÁ¿

        //°ïÅÉµÈ¼¶ÐèÒªµÄÈËÊýµÄ×îµÍÏÞ¶È
        public const int SYNMEMSUM_LOWERLIMIT_TWO = 60;
        public const int SYNMEMSUM_LOWERLIMIT_THREE = 150;
        public const int SYNMEMSUM_LOWERLIMIT_FOUR = 380;

        public const int CHGMAP_LOCK_SECS = 10;							// ½øÈëµØÍ¼ºóµÄ±£»¤Ê±¼ä
        public const int SYNWAR_NOMONEY_DAMAGETIMES = 10;							// °ïÅÉÃ»Ç®Ê±Ôö¼ÓµÄÉËº¦±¶Êý
        public const int PICK_MORE_MONEY = 1000;							// ¼ñÇ®¶àÊ±£¬·¢ACTIONÏûÏ¢
        public const int SYNCHRO_SECS = 5;

        public const ulong KEEP_EFFECT_NOT_VIRTUOUS = (ulong)(Effect0.BLUE_NAME | Effect0.RED_NAME | Effect0.BLACK_NAME);

        private readonly uint[] FRIEND_VIP_AMOUNT = { 0, 60, 70, 80, 90, 100, 120 };

        #endregion

        #region Mining

        public void Mine()
        {
            if (!IsAlive)
                return;

            if (m_tMine.IsActive())
                return;

            if (QueryTransformation != null)
                return;

            if (!Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND) || !Equipment.Items[ItemPosition.RIGHT_HAND].IsPick)
            {
                Send(ServerString.STR_MINE_WITH_PECKER);
                return;
            }

            m_tMine.Startup(3000);
            m_nMineCount = 0;
        }

        public void StopMine()
        {
            m_tMine.Clear();
            m_nMineCount = 0;
        }

        public void ProcessMineTimer()
        {
            if (!IsAlive)
            {
                StopMine();
                return;
            }

            if (!Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND) || !Equipment.Items[ItemPosition.RIGHT_HAND].IsPick)
            {
                Send(ServerString.STR_NEED_PICK);
                StopMine();
                return;
            }

            float nChance = 10f;
            if (WeaponSkill.Skills.ContainsKey(562))
                nChance += ((float)WeaponSkill.Skills[562].Level / 2);

            Random pRand = new Random();

            uint idItem = 0;

            if (!Inventory.IsFull && Calculations.ChanceCalc(nChance))
            {
                // todo create and handle all mines
                // all mines will drop the same shit for now

                if (MapIdentity == 1028) // tc mine
                {
                    if (Calculations.ChanceCalc(5f)) // euxenite
                    {
                        Inventory.Create(SpecialItem.EUXINITE_ORE);
                        Send(string.Format(ServerString.STR_EUXENITE_ORE));
                    }
                    else if (Calculations.ChanceCalc(2f)) // Meteor
                    {
                        Inventory.Create(SpecialItem.TYPE_METEOR);
                        Send(ServerString.STR_METEOR);
                    }
                    else if (Calculations.ChanceCalc(1f)) // DragonBall
                    {
                        Inventory.Create(SpecialItem.TYPE_DRAGONBALL);
                        Send(ServerString.STR_DRAGON_BALL);
                    }
                    else if (Calculations.ChanceCalc(0.05f)) // Power Exp Ball
                    {
                        Inventory.Create(723744);
                        ServerKernel.SendMessageToAll(
                            string.Format("Congratulations! {0} has found a PowerExpBall while mining.", Name),
                            ChatTone.TALK);
                    }
                    else if (Calculations.ChanceCalc(1f)) // Triple Exp Potion
                    {
                        Inventory.Create(720393);
                        Send("You gained ExpPill.");
                    }
                    else if (Calculations.ChanceCalc(1f)) // +1 Stone
                    {
                        Inventory.Create(SpecialItem.TYPE_STONE1);
                        Send("You gained +1 Stone.");
                    }
                    else if (Calculations.ChanceCalc(1f)) // 500,000 silvers
                    {
                        AwardMoney(300000, true);
                        Send("You gained 300,000 silvers.");
                    }
                    else if (Calculations.ChanceCalc(1f)) // random quality tem
                    {
                        idItem = GenerateRandomGem();
                        Inventory.Create(idItem);
                        Send("You gained a random gem.");
                    }
                    else if (Calculations.ChanceCalc(5f)) // Copper Ore
                    {
                        idItem = (uint)(SpecialItem.COPPER_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_COPPER_ORE);
                    }
                    else if (Calculations.ChanceCalc(30f)) // Iron Ore
                    {
                        idItem = (uint)(SpecialItem.IRON_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_IRON_ORE);
                    }
                }
                else if (MapIdentity == 1025) // Phoenix
                {
                    if (Calculations.ChanceCalc(2f)) // silver
                    {
                        AwardMoney(500000);
                        Send("You gained 500,000 silvers");
                    }
                    else if (Calculations.ChanceCalc(2f))
                    {
                        Inventory.Create(SpecialItem.TYPE_STONE1);
                        Send("You gained +1 stone.");
                    }
                    else if (Calculations.ChanceCalc(2f)) // Meteor
                    {
                        Inventory.Create(SpecialItem.TYPE_METEOR);
                        Send(ServerString.STR_METEOR);
                    }
                    else if (Calculations.ChanceCalc(1f)) // DragonBall
                    {
                        Inventory.Create(SpecialItem.TYPE_DRAGONBALL);
                        Send(ServerString.STR_DRAGON_BALL);
                    }
                    else if (Calculations.ChanceCalc(10f)) // Copper
                    {
                        idItem = (uint)(SpecialItem.COPPER_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_COPPER_ORE);
                    }
                    else if (Calculations.ChanceCalc(5f)) // Gold
                    {
                        idItem = (uint)(SpecialItem.GOLD_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_GOLD_ORE);
                    }
                    else if (Calculations.ChanceCalc(30f)) // Iron
                    {
                        idItem = (uint)(SpecialItem.IRON_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_IRON_ORE);
                    }
                    else if (Calculations.ChanceCalc(1f)) // random quality tem
                    {
                        idItem = GenerateRandomGem();
                        Inventory.Create(idItem);
                        Send("You gained a random gem.");
                    }
                }
                else if (MapIdentity == 1026) // Ape
                {
                    if (Calculations.ChanceCalc(0.5f)) // silver
                    {
                        AwardMoney(1500000);
                        Send("You gained 1,500,000 silvers");
                    }
                    else if (Calculations.ChanceCalc(2f))
                    {
                        Inventory.Create(SpecialItem.TYPE_STONE1);
                        Send("You gained +1 stone.");
                    }
                    else if (Calculations.ChanceCalc(1f))
                    {
                        Inventory.Create(SpecialItem.TYPE_STONE2);
                        Send("You gained +2 stone.");
                    }
                    else if (Calculations.ChanceCalc(2f)) // Meteor
                    {
                        Inventory.Create(SpecialItem.TYPE_METEOR);
                        Send(ServerString.STR_METEOR);
                    }
                    else if (Calculations.ChanceCalc(1f)) // DragonBall
                    {
                        Inventory.Create(SpecialItem.TYPE_DRAGONBALL);
                        Send(ServerString.STR_DRAGON_BALL);
                    }
                    else if (Calculations.ChanceCalc(10f)) // Copper
                    {
                        idItem = (uint)(SpecialItem.COPPER_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_COPPER_ORE);
                    }
                    else if (Calculations.ChanceCalc(5f)) // Gold
                    {
                        idItem = (uint)(SpecialItem.GOLD_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_GOLD_ORE);
                    }
                    else if (Calculations.ChanceCalc(30f)) // Iron
                    {
                        idItem = (uint)(SpecialItem.IRON_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_IRON_ORE);
                    }
                    else if (Calculations.ChanceCalc(1f)) // random quality tem
                    {
                        idItem = GenerateRandomGem();
                        Inventory.Create(idItem);
                        Send("You gained a random gem.");
                    }
                }
                else if (MapIdentity == 1027) // Desert
                {
                    if (Calculations.ChanceCalc(0.5f)) // silver
                    {
                        AwardMoney(1500000);
                        Send("You gained 1,500,000 silvers");
                    }
                    else if (Calculations.ChanceCalc(2f))
                    {
                        Inventory.Create(SpecialItem.TYPE_STONE1);
                        Send("You gained +1 stone.");
                    }
                    else if (Calculations.ChanceCalc(1f))
                    {
                        Inventory.Create(SpecialItem.TYPE_STONE2);
                        Send("You gained +2 stone.");
                    }
                    else if (Calculations.ChanceCalc(2f)) // Meteor
                    {
                        Inventory.Create(SpecialItem.TYPE_METEOR);
                        Send(ServerString.STR_METEOR);
                    }
                    else if (Calculations.ChanceCalc(1f)) // DragonBall
                    {
                        Inventory.Create(SpecialItem.TYPE_DRAGONBALL);
                        Send(ServerString.STR_DRAGON_BALL);
                    }
                    else if (Calculations.ChanceCalc(10f)) // Copper
                    {
                        idItem = (uint)(SpecialItem.COPPER_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_COPPER_ORE);
                    }
                    else if (Calculations.ChanceCalc(5f)) // Gold
                    {
                        idItem = (uint)(SpecialItem.GOLD_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_GOLD_ORE);
                    }
                    else if (Calculations.ChanceCalc(30f)) // Iron
                    {
                        idItem = (uint)(SpecialItem.IRON_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_IRON_ORE);
                    }
                    else if (Calculations.ChanceCalc(5f))
                    {
                        idItem = (uint)(SpecialItem.SILVER_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_IRON_ORE);
                    }
                    else if (Calculations.ChanceCalc(0.3f))
                    {
                        int amount = pRand.Next(15, 215);
                        AwardEmoney(amount);
                        Send(string.Format("You awarded {0} CPs from mining.", amount), ChatTone.TALK);
                    }
                    else if (Calculations.ChanceCalc(1f)) // random quality tem
                    {
                        idItem = GenerateRandomGem();
                        Inventory.Create(idItem);
                        Send("You gained a random gem.");
                    }
                }
                else if (MapIdentity == 1218) // Adventure Zone
                {
                    if (Calculations.ChanceCalc(0.25f)) // silver
                    {
                        AwardMoney(1500000);
                        Send("You gained 5,000,000 silvers");
                    }
                    else if (Calculations.ChanceCalc(2f))
                    {
                        Inventory.Create(SpecialItem.TYPE_STONE1);
                        Send("You gained +1 stone.");
                    }
                    else if (Calculations.ChanceCalc(1f))
                    {
                        Inventory.Create(SpecialItem.TYPE_STONE2);
                        Send("You gained +2 stone.");
                    }
                    else if (Calculations.ChanceCalc(1f))
                    {
                        Inventory.Create(SpecialItem.TYPE_STONE3);
                        Send("You gained +3 stone.");
                    }
                    else if (Calculations.ChanceCalc(2f)) // Meteor
                    {
                        Inventory.Create(SpecialItem.TYPE_METEOR);
                        Send(ServerString.STR_METEOR);
                    }
                    else if (Calculations.ChanceCalc(1f)) // DragonBall
                    {
                        Inventory.Create(SpecialItem.TYPE_DRAGONBALL);
                        Send(ServerString.STR_DRAGON_BALL);
                    }
                    else if (Calculations.ChanceCalc(10f)) // Copper
                    {
                        idItem = (uint)(SpecialItem.COPPER_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_COPPER_ORE);
                    }
                    else if (Calculations.ChanceCalc(5f)) // Gold
                    {
                        idItem = (uint)(SpecialItem.GOLD_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_GOLD_ORE);
                    }
                    else if (Calculations.ChanceCalc(30f)) // Iron
                    {
                        idItem = (uint)(SpecialItem.IRON_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_IRON_ORE);
                    }
                    else if (Calculations.ChanceCalc(5f))
                    {
                        idItem = (uint)(SpecialItem.SILVER_ORE + pRand.Next(0, 9));
                        Inventory.Create(idItem);
                        Send(ServerString.STR_IRON_ORE);
                    }
                    else if (Calculations.ChanceCalc(0.2f))
                    {
                        int amount = pRand.Next(30, 430);
                        AwardEmoney(amount);
                        Send(string.Format("You awarded {0} CPs from mining.", amount), ChatTone.TALK);
                    }
                    else if (Calculations.ChanceCalc(0.2f))
                    {
                        Inventory.Create(SpecialItem.TYPE_METEOR_SCROLL);
                        Send(string.Format("You found a Meteor Scroll."));
                    }
                    else if (Calculations.ChanceCalc(0.2f))
                    {
                        Inventory.Create(SpecialItem.TYPE_DRAGONBALL_SCROLL);
                        Send(string.Format("You found a DragonBall Scroll."));
                    }
                    else if (Calculations.ChanceCalc(1f)) // random quality tem
                    {
                        idItem = GenerateRandomGem();
                        Inventory.Create(idItem);
                        Send("You gained a random gem.");
                    }
                }
            }

            Screen.Send(new MsgAction(Identity, 0, MapX, MapY, GeneralActionType.MINE), true);
        }

        private uint GenerateRandomGem()
        {
            uint idItem = (uint)(700000 + (ThreadSafeRandom.RandGet(0, 7) * 10));
            if (Calculations.ChanceCalc(5f))
                idItem += 3;
            else if (Calculations.ChanceCalc(20f))
                idItem += 2;
            else
                idItem += 1;
            return idItem;
        }

        #endregion

        #region IOnTimer

        public void DailyReset()
        {
            uint today = uint.Parse(DateTime.Now.ToString("yyyyMMdd"));
            if (today == m_dbUser.LastUpdate)
                return;

            // enlight
            ushort usEnlightmentPoints = 0;
            if (Level >= 90)
                usEnlightmentPoints += 100;
            switch (NobilityRank)
            {
                case NobilityLevel.KING:
                    usEnlightmentPoints += 400;
                    break;
                case NobilityLevel.PRINCE:
                    usEnlightmentPoints += 300;
                    break;
                case NobilityLevel.DUKE:
                case NobilityLevel.EARL:
                    usEnlightmentPoints += 200;
                    break;
                case NobilityLevel.BARON:
                case NobilityLevel.KNIGHT:
                    usEnlightmentPoints += 100;
                    break;
            }

            if (Owner.VipLevel <= 3)
                usEnlightmentPoints += 100;
            else if (Owner.VipLevel <= 5)
                usEnlightmentPoints += 200;
            else
                usEnlightmentPoints += 300;

            EnlightmentPoints = usEnlightmentPoints;

            // flower
            if (Gender == 1
                        && Level >= 40
                        && RedRoses < today)
            {
                Send(new MsgAction(Identity, 1244u, 0, 0, GeneralActionType.OPEN_CUSTOM));
            }

            m_dbUser.LastUpdate = uint.Parse(DateTime.Now.ToString("yyyyMMdd"));
        }

        public void OnBattleTimer()
        {
            if (Map == null)
                return;

            if (BattleSystem == null)
                return;

            try
            {
                if (BattleSystem.IsActived())
                    ProcessAutoAttack();
            }
            catch
            {
                Console.WriteLine("AUTOATTACK ERROR");
            }

            try
            {
                if (Magics.QueryMagic() != null)
                    Magics.OnTimer();
            }
            catch
            {
                Console.WriteLine("MAGIC ERROR");
            }

            if (Magics.QueryMagic() == null && ((Magics.IsInLaunch() || Magics.IsIntone()) && !m_pMap.IsTrainingMap()))
                Magics.SetMagicState(0);
        }

        public void OnTimer()
        {
            if (Map == null) return; // check if already on map

            if (CaptchaBox != null)
                CaptchaBox.OnTimer(this);

            //try
            //{
            //    OnBattleTimer();
            //}
            //catch (Exception ex)
            //{
            //    ServerKernel.Log.SaveLog("ERROR BATTLE TIMER", true, LogType.ERROR);
            //    ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.ERROR);
            //}

            try
            {
                if (m_tDoGhost.IsActive() && m_tDoGhost.IsTimeOut())
                {
                    m_tDoGhost.Clear();
                    if (QueryTransformation == null
                        || (QueryTransformation.Lookface != 98
                            && QueryTransformation.Lookface != 99))
                    {
                        SetGhost();
                    }
                }
            }
            catch
            {
                Console.WriteLine("ERROR GHOST TIMER");
            }

            try
            {
                // xp points handle
                if (m_tXp.ToNextTime())
                    ProcXpVal();
            }
            catch
            {
                Console.WriteLine("ERRORR CHECK XP");
            }

            // stamina recover
            byte maxStamina = 180;
            if (!IsBlessed)
                maxStamina = 100;

            try
            {
                if (Stamina < maxStamina && m_tStamina.ToNextTime() && !IsWing())
                {
                    byte add = 0;
                    if (Action == EntityAction.SIT)
                    {
                        add = ADD_ENERGY_STAND*2;
                    }
                    else if (Action == EntityAction.LIE)
                    {
                        add = ADD_ENERGY_STAND;
                    }
                    else
                    {
                        add = ADD_ENERGY_STAND/3;
                    }

                    if (Equipment.Items.ContainsKey(ItemPosition.CROP))
                        add += 3;

                    if (add + Stamina > maxStamina)
                        add = (byte) (maxStamina - Stamina);
                    Stamina += add;
                }
            }
            catch
            {
                Console.WriteLine("ERROR STAMINA USER");
            }

            try
            {
                // decrease pk points
                if (m_pkDecrease.ToNextTime() && PkPoints > 0)
                {
                    if (m_pMap.IsPrisionMap())
                        ChangePkPoints(-3);
                    else
                        ChangePkPoints(-1);
                }
            }
            catch
            {
                Console.WriteLine("ERROR DECREAS PKP");
            }

            try
            {
                // automatic life recover
                if (m_lifeRecover.ToNextTime()
                    && IsAlive
                    && Life > 0
                    && (Life < MaxLife))
                {
                    ushort add = (ushort) (MaxLife/300);
                    if (add + Life > MaxLife)
                        add = (ushort) (MaxLife - Life);
                    Life += add;
                }
            }
            catch
            {
                Console.WriteLine("ERROR RECOVER LIFE");
            }

            try
            {
                // send team leader position
                if (m_tTeamPos.ToNextTime() && Team != null && Team.Leader != this && Team.Leader.MapIdentity == MapIdentity)
                    Team.SendLeaderPosition(this);
            }
            catch
            {
                Console.WriteLine("ERROR TEAM LEADER POS");
            }

            try
            {
                if (IsBlessed && m_tHeavenBlessing.ToNextTime() && !m_pMap.IsTrainingMap())
                {
                    m_pBlessPoints++;
                    if (m_pBlessPoints >= 5)
                    {
                        m_pBlessPoints = 0;
                        long exp = ServerKernel.GetExpBallExperience(Level)/30;
                        AwardExperience(exp);
                        UpdateClient(ClientUpdateType.ONLINE_TRAINING, 5, true);
                        UpdateClient(ClientUpdateType.ONLINE_TRAINING, 0, true);
                    }
                    else
                    {
                        UpdateClient(ClientUpdateType.ONLINE_TRAINING, 4);
                        UpdateClient(ClientUpdateType.ONLINE_TRAINING, 3);
                    }
                }
            }
            catch
            {
                Console.WriteLine("ERROR BLESS ADD CHECK");
            }

            try
            {
                if (m_dwBlessEndTime > 0 && UnixTimestamp.Timestamp() > m_dwBlessEndTime)
                {
                    UpdateClient(ClientUpdateType.ONLINE_TRAINING, 5, false);
                }
            }
            catch
            {
                Console.WriteLine("ERROR BLESS CHECK TIME");
            }

            try
            {
                // lucky time handle
                if (m_tLuckyTime.ToNextTime() && Screen != null)
                {
                    var aroundPlayers = Screen.GetAroundPlayers;
                    IStatus status;
                    if (QueryStatus(FlagInt.LUCKY_DIFFUSE) != null) // if casting
                    {
                        if (aroundPlayers != null) // check players around
                        {
                            foreach (var plr in aroundPlayers)
                            {
                                if (GetDistance(plr as IScreenObject) <= 3
                                    && plr.QueryStatus(FlagInt.LUCKY_DIFFUSE) == null
                                    && plr.QueryStatus(FlagInt.LUCKY_ABSORB) == null
                                    && (plr as Character).Metempsychosis < 2)
                                {
                                    plr.AttachStatus(plr, FlagInt.LUCKY_ABSORB, 0, 1, 0, 0, Identity);
                                }
                                else if ((plr as Character).Metempsychosis < 2
                                         && GetDistance(plr as IScreenObject) >= 4
                                         && plr.QueryStatus(FlagInt.LUCKY_DIFFUSE) == null
                                         && plr.QueryStatus(FlagInt.LUCKY_ABSORB) != null)
                                {
                                    plr.DetachStatus(FlagInt.LUCKY_ABSORB);
                                }
                            }
                        }

                        status = QueryStatus(FlagInt.LUCKY_DIFFUSE);
                        status.IncTime(3000, 7200000);
                        if (m_dwLuckyTime < 7200)
                            m_dwLuckyTime += 3;
                        m_dbUser.LuckyTime = m_dwLuckyTime;
                    }
                    else if (QueryStatus(FlagInt.LUCKY_ABSORB) != null) // if on lucky time
                    {
                        status = QueryStatus(FlagInt.LUCKY_ABSORB);
                        status.IncTime(1000, 7200000);
                        if (m_dwLuckyTime < 7200)
                            m_dwLuckyTime++;
                        m_dbUser.LuckyTime = m_dwLuckyTime;
                    }
                    else if (m_dbUser.LuckyTime > 0) // not receiving lucky time
                    {
                        m_dwLuckyTime = m_dbUser.LuckyTime -= 1;
                        UpdateClient(ClientUpdateType.LUCKY_TIME_TIMER, m_dwLuckyTime*1000);
                    }
                }
            }
            catch
            {
                Console.WriteLine("ERROR LUCKY TIME");
            }

            try
            {
                // handle status timers
                if (m_tStatusCheck.ToNextTime())
                {
                    if (Team != null && Team.Leader == this) Team.CheckAuras();

                    foreach (var stts in Status.Status.Values)
                    {
                        stts.OnTimer();
                        if (!stts.IsValid && stts.Identity != FlagInt.GHOST && stts.Identity != FlagInt.DEAD)
                        {
                            m_nFightPause = GetInterAtkRate();
                            RecalculateAttributes();

                            if (stts.Identity == FlagInt.OBLIVION
                                && Status.DelObj(stts.Identity))
                            {
                                AwardOblivion(false);
                                m_nKoCount = 0;
                                m_lAccumulateExp = 0;

                            }
                            else if (Status.DelObj(stts.Identity) &&
                                     (stts.Identity == FlagInt.CYCLONE || stts.Identity == FlagInt.SUPERMAN))
                            {
                                if (m_nKoCount < MIN_SUPERMAP_KILLS)
                                    break;

                                int nPosition = 0;
                                if (ServerKernel.KoBoard.Count == 0)
                                    nPosition = 1;
                                foreach (
                                    var ko in
                                        ServerKernel.KoBoard.Values.OrderByDescending(x => x.KoCount)
                                            .ThenBy(x => x.Identity))
                                {
                                    if (nPosition++ >= 100)
                                        break;
                                    if (ko.KoCount > m_nKoCount)
                                        continue;
                                    break;
                                }

                                if (nPosition < 101)
                                {
                                    IKoCount temp;
                                    if (!ServerKernel.KoBoard.TryRemove(Identity, out temp))
                                    {
                                        DbSuperman pSuper = new DbSuperman
                                        {
                                            Identity = Identity,
                                            Amount = m_nKoCount,
                                            Name = Name
                                        };
                                        temp = new IKoCount(pSuper);
                                        ServerKernel.SendMessageToAll(
                                            string.Format(ServerString.STR_SUPERMAN_BROADCAST, Name, m_nKoCount,
                                                nPosition), ChatTone.TALK);
                                    }
                                    else
                                    {
                                        if (m_nKoCount > temp.KoCount)
                                        {
                                            temp.KoCount = m_nKoCount;
                                            ServerKernel.SendMessageToAll(
                                                string.Format(ServerString.STR_SUPERMAN_BROADCAST, Name, m_nKoCount,
                                                    nPosition), ChatTone.TALK);
                                        }
                                    }
                                    ServerKernel.KoBoard.TryAdd(Identity, temp);
                                }
                                m_nKoCount = 0;
                            }
                            else
                            {
                                Status.DelObj(stts.Identity);
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("ERROR STATUS CHECK");
            }

            try
            {
                // mining
                if (m_tMine.IsActive() && m_tMine.ToNextTime())
                {
                    ProcessMineTimer();
                }
            }
            catch
            {
                Console.WriteLine("ERROR MINING TIMER");
            }

            try
            {
                // handle transformation timing
                if (m_tTransformation.IsActive() && m_tTransformation.IsTimeOut())
                    ClearTransformation();
            }
            catch
            {
                Console.WriteLine("ERROR TRANSFORMATION CHECK");
            }

            // send date time packet
            if (m_timePacket.ToNextTime())
                Send(new MsgData());

            try
            {
                if (QueryStatus(FlagInt.DAZED) != null && m_tDazed.ToNextTime())
                {
                    Walk((FacingDirection) (ThreadSafeRandom.RandGet()%8));
                }
            }
            catch
            {
                Console.WriteLine("ERROR DAZED TIMER");
            }

            try
            {
                if (QueryStatus(FlagInt.RIDING) != null && Vigor < MaxVigor && m_tVigor.ToNextTime())
                {
                    uint dwRec = (uint) Math.Max(MaxVigor/150f, 3);
                    Vigor = Math.Min(MaxVigor, Vigor + dwRec);
                }
            }
            catch
            {
                Console.WriteLine("ERROR RESTORE VIGOR");
            }

            try
            {
                if (m_tOnlineTime.ToNextTime())
                {
                    AwardEmoney(215, false);
                    AwardMoney(150000, false);
                    AwardStudyPoints(200, false);
                    Send("You received 215 CPs, 150,000 silvers and 200 Study Points for staying 30 minutes online.",
                        ChatTone.TALK);
                }
            }
            catch
            {
                Console.WriteLine("ERROR AUTO REWARD");
            }
        }
        #endregion
    }
}