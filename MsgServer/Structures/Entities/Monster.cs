// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Monster.cs
// Last Edit: 2016/12/06 14:14
// Created: 2016/12/06 14:13

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Enums;
using DB.Entities;
using MsgServer.Network.GameServer.Handlers;
using MsgServer.Structures.Actions;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Entities
{
    public class Monster : IRole, IOnTimer, IScreenObject
    {
        private TimeOutMS m_statusCheck = new TimeOutMS(1000);

        private MsgPlayer m_pPacket;
        private FacingDirection m_pDirection;
        private DbMonstertype m_dbMonster;
        private Generator m_pGenerator;
        
        private BattleSystem m_pBattleSystem;
        private MagicData m_pMagics;
        private StatusSet m_pStatus;
        private GameAction m_pGameAction;
        private Magic m_pDefaultMagic;
        private Map m_pMap;
        private Screen m_pScreen;
        private PetData m_pData;
        private SpecialDrop m_pDrop;

        private List<DbMonsterMagic> m_dbMonsterMagics = new List<DbMonsterMagic>();

        private uint m_dwIdentity, m_dwMapIdentity, m_dwLookface;
        private string m_szName;
        private ushort m_usMapX, m_usMapY;
        private byte m_pLevel;
        private int m_nBattlePower, m_nMinAttack, m_nMaxAttack, m_nMagicAttack;
        private int m_nDodge;
        private int m_nAttackHitRate;
        private int m_nDexterity;
        private int m_nDefense, m_nMagicDefense, m_nAddFinalAttack, m_nAddFinalMagicAttack, m_nAddFinalDefense, m_nAddFinalMagicDefense;
        private uint m_nLife;
        private uint m_nMaxLife;
        private ushort m_nMana;
        private ushort m_nMaxMana;
        private ulong m_ulFlag1, m_ulFlag2;
        private short m_sElevation;
        private bool m_isAlive;
        private ushort m_profession;
        private uint m_idMoveTarget;
        private uint m_idActTarget;
        private uint m_nActMode;
        private TimeOutMS m_tLocked;
        private TimeOutMS m_tAttackMs;
        private bool m_bAtkFirst;
        private IRole m_pRoleTarget;
        private uint m_idAtkMe;
        private TimeOutMS m_tAction;
        private int m_viewRange;
        private bool m_bAttackFlag;
        private TimeOutMS m_tMoveMs;
        private uint m_moveSpeed;
        private EntityAction m_pAction;
        private int m_nAttackRange;
        private TimeOut m_tRevive;
        private TimeOut m_tDisappear = new TimeOut(5);
        private ushort m_usUseMagicType = 0;
        private Character m_pOwner;
        private int m_nTargetCount = 0;

        public int TargetCount { get { return m_nTargetCount; } set { m_nTargetCount = value < 0 ? 0 : value; } }

        public BattleSystem BattleSystem
        {
            get { return m_pBattleSystem ?? (m_pBattleSystem = new BattleSystem(this)); }
        }

        public Generator Generator { get { return m_pGenerator; } }

        public Monster(DbMonstertype dbMonster, uint idMonster, Generator gen)
        {
            m_dbMonster = dbMonster;
            m_dwIdentity = idMonster;
            m_pGenerator = gen;

            m_pPacket = new MsgPlayer(idMonster);
            m_nActMode = MODE_IDLE;

            Name = m_dbMonster.Name;

            MinAttack = dbMonster.AttackMin;
            MaxAttack = dbMonster.AttackMax;
            Defense = dbMonster.Defence;
            MagicDefense = dbMonster.MagicDef;
            MaxLife = (uint)dbMonster.Life;
            Action = EntityAction.STAND;
            AttackHitRate = (ushort)m_dbMonster.Dexterity;
            AttackRange = m_dbMonster.AttackRange;
            Life = (uint)m_dbMonster.Life;
            Mana = (ushort)m_dbMonster.Mana;
            ViewRange = m_dbMonster.ViewRange;
            Level = (byte)m_dbMonster.Level;
            Lookface = m_dbMonster.Lookface;

            if (IsBoss)
                m_pPacket.IsBoss = true;

            m_nTargetCount = 0;

            if (m_dbMonster.MagicType > 0)
            {
                m_pDefaultMagic = new Magic(this);
                if (!m_pDefaultMagic.Create(m_dbMonster.MagicType))
                    m_pDefaultMagic = null;
                else
                {
                    Magics.Magics.TryAdd(m_pDefaultMagic.Type, m_pDefaultMagic);
                }
            }

            var mgcList = ServerKernel.MonsterMagics.Where(x => x.OwnerIdentity == m_dbMonster.Id);
            foreach (var mgc in mgcList)
            {
                m_dbMonsterMagics.Add(mgc);
            }

            m_pDrop = ServerKernel.SpecialDrop.FirstOrDefault(x => x.MonsterIdentity == m_dbMonster.Id);

            m_tAttackMs = new TimeOutMS(dbMonster.AttackSpeed);
            m_tLocked = new TimeOutMS(0);
            m_tAction = new TimeOutMS(0);
            m_tMoveMs = new TimeOutMS(1000);
            m_tRevive = new TimeOut(gen.RestSeconds);
            m_statusCheck.Update();
        }

        /// <summary>
        /// Use to create pets
        /// </summary>
        public bool Create(Character pOwner)
        {
            if (IsCallPet())
            {
                // todo synchro
                m_pOwner = pOwner;

                m_pMap = pOwner.Map;
                m_usMapX = pOwner.MapX;
                m_usMapY = pOwner.MapY;

                //MsgPetInfo pMsg = new MsgPetInfo
                //{
                //    AiType = m_dbMonster.AiType,
                //    Identity = m_dwIdentity,
                //    Lookface = m_dbMonster.Lookface,
                //    MapX = m_usMapX,
                //    MapY = m_usMapY,
                //    Name = m_dbMonster.Name
                //};
                //pOwner.Send(pMsg);

                return true;
            }
            return false;
        }

        public uint Identity
        {
            get { return m_dwIdentity; }
        }

        public uint Type
        {
            get { return m_dbMonster.Id; }
        }

        public uint OwnerIdentity { get; set; }

        public Character Owner { get { return m_pOwner; } set { m_pOwner = value; } }

        public uint AttackUser
        {
            get { return m_dbMonster.AttackUser; }
        }

        public string Name
        {
            get { return m_szName; }
            set
            {
                m_szName = value;
                m_pPacket.Name = value;
            }
        }

        public Screen Screen
        {
            get { return m_pScreen; }
            set { m_pScreen = value; }
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

        public int ViewRange
        {
            get { return m_dbMonster.ViewRange; }
            set { m_dbMonster.ViewRange = value; }
        }

        public int MoveSpeed
        {
            get { return m_dbMonster.MoveSpeed; }
            set { m_dbMonster.MoveSpeed = value; }
        }

        public uint RunSpeed
        {
            get { return m_dbMonster.RunSpeed; }
            set { m_dbMonster.RunSpeed = value; }
        }

        public int AttackRange
        {
            get { return m_dbMonster.AttackRange; }
            set { m_dbMonster.AttackRange = value; }
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

        #region Update Packet

        public void SendEffect(string effectName)
        {
            var sPacket = new MsgName();
            sPacket.Identity = Identity;
            sPacket.Action = StringAction.ROLE_EFFECT;
            sPacket.Append(effectName);
            m_pMap.SendToRange(sPacket, MapX, MapY);
        }

        /// <summary>
        /// Send a update packet containing a byte value.
        /// </summary>
        public void UpdateClient(ClientUpdateType type, byte value)
        {
            var MsgUserAttrib = new MsgUserAttrib();
            MsgUserAttrib.Identity = Identity;
            MsgUserAttrib.Append(type, value);
            m_pMap.SendToRange(MsgUserAttrib, MapX, MapY);
        }

        /// <summary>
        /// Send a update packet containing a uint value.
        /// </summary>
        public void UpdateClient(ClientUpdateType type, uint value)
        {
            var MsgUserAttrib = new MsgUserAttrib();
            MsgUserAttrib.Identity = Identity;
            MsgUserAttrib.Append(type, value);
            m_pMap.SendToRange(MsgUserAttrib, MapX, MapY);
        }

        /// <summary>
        /// Send a update packet containing a ushort value.
        /// </summary>
        public void UpdateClient(ClientUpdateType type, ushort value)
        {
            var MsgUserAttrib = new MsgUserAttrib();
            MsgUserAttrib.Identity = Identity;
            MsgUserAttrib.Append(type, value);
            m_pMap.SendToRange(MsgUserAttrib, MapX, MapY);
        }

        /// <summary>
        /// Send a update packet containing a ulong value.
        /// </summary>
        public void UpdateClient(ClientUpdateType type, ulong value)
        {
            var MsgUserAttrib = new MsgUserAttrib();
            MsgUserAttrib.Identity = Identity;
            MsgUserAttrib.Append(type, value);
            m_pMap.SendToRange(MsgUserAttrib, MapX, MapY);
        }

        public void UpdateClient(ClientUpdateType type, ulong value1, ulong value2)
        {
            var MsgUserAttrib = new MsgUserAttrib();
            MsgUserAttrib.Identity = Identity;
            MsgUserAttrib.Append(type, value1, value2);
            m_pMap.SendToRange(MsgUserAttrib, MapX, MapY);
        }
        #endregion

        #region IRole

        public bool IsCallPet() { return Identity >= IdentityRange.CALLPETID_FIRST && Identity <= IdentityRange.CALLPETID_LAST; }
        public MagicData Magics { get { return m_pMagics ?? (m_pMagics = new MagicData(this)); } }
        public byte Stamina { get; set; }
        public bool SpendEquipItem(uint useItem, uint useItemNum, bool bSynchro) { return true; }
        public bool DecEquipmentDurability(bool bAttack, int hitByMagic, ushort useItemNum) { return true; }
        public bool CheckWeaponSubType(uint idType, uint dwAmount) { return true; }
        public bool ProcessMagicAttack(ushort usMagicType, uint idTarget, ushort x, ushort y, byte ucAutoActive = 0)
        {
            if (Magics != null)
                Magics.MagicAttack(usMagicType, idTarget, x, y, ucAutoActive);
            return true;
        }

        public bool IsBoss
        {
            get { return m_dbMonster.Boss > 0; }
        }

        public int GetSizeAdd() { return 0; }

        public StatusSet Status
        {
            get { return m_pStatus ?? (m_pStatus = new StatusSet(this)); }
        }

        public GameAction GameAction
        {
            get { return m_pGameAction ?? (m_pGameAction = new GameAction(this)); }
        }

        public uint Lookface
        {
            get { return m_dwLookface; }
            set
            {
                m_dwLookface = value;
                m_pPacket.Mesh = value;
            }
        }

        public byte Level
        {
            get { return m_pLevel; }
            set
            {
                m_pLevel = value;
                m_pPacket.MonsterLevel = value;
            }
        }

        public int BattlePower
        {
            get { return m_nBattlePower; }
        }

        public int MinAttack
        {
            get { return m_nMinAttack; }
            set { m_nMinAttack = value; }
        }

        public int MaxAttack
        {
            get { return m_nMaxAttack; }
            set { m_nMaxAttack = value; }
        }

        public int MagicAttack
        {
            get { return m_dbMonster.AttackMax; }
            set { m_dbMonster.AttackMax = value; }
        }

        public int Dodge
        {
            get { return (int) m_dbMonster.Dodge; }
            set { m_nDodge = value; }
        }

        public int AttackHitRate
        {
            get { return (int) m_dbMonster.Dexterity; }
            set { m_nAttackHitRate = value; }
        }

        public int Dexterity
        {
            get { return m_nDexterity; }
            set { m_nDexterity = value; }
        }

        public int Defense
        {
            get { return m_nDefense; }
            set { m_nDefense = value; }
        }

        public int MagicDefense
        {
            get { return m_nMagicDefense; }
            set { m_nMagicDefense = value; }
        }

        public int AddFinalAttack
        {
            get { return m_nAddFinalAttack; }
            set { m_nAddFinalAttack = value; }
        }

        public int AddFinalMagicAttack
        {
            get { return m_nAddFinalMagicAttack; }
            set { m_nAddFinalMagicAttack = value; }
        }

        public int AddFinalDefense
        {
            get { return m_nAddFinalDefense; }
            set { m_nAddFinalDefense = value; }
        }

        public int AddFinalMagicDefense
        {
            get { return m_nAddFinalMagicDefense; }
            set { m_nAddFinalMagicDefense = value; }
        }

        public uint CriticalStrike { get { return 0; } }
        public uint SkillCriticalStrike { get { return 0; } }
        public uint Breakthrough { get { return 0; } }
        public uint Penetration { get { return 0; } }
        public uint Immunity { get { return 0; } }
        public uint Counteraction { get { return 0; } }
        public uint Block { get { return 0; } }
        public uint Detoxication { get { return 0; } }
        public uint FireResistance { get { return 0; } }
        public uint WaterResistance { get { return 0; } }
        public uint EarthResistance { get { return 0; } }
        public uint WoodResistance { get { return 0; } }
        public uint MetalResistance { get { return 0; } }
        public int MagicDefenseBonus { get { return 0; } }

        public uint Life
        {
            get { return m_nLife; }
            set
            {
                m_nLife = value;
                if (IsBoss)
                    m_pPacket.Life = (ushort)((float) (m_nLife / (float)MaxLife) * 10000);
                else
                    m_pPacket.Life = (ushort) value;
            }
        }

        public uint MaxLife
        {
            get { return m_nMaxLife; }
            set { m_nMaxLife = value; }
        }

        public ushort Mana
        {
            get { return m_nMana; }
            set { m_nMana = value; }
        }

        public ushort MaxMana
        {
            get { return m_nMaxMana; }
            set { m_nMaxMana = value; }
        }

        public ulong Flag1
        {
            get { return m_ulFlag1; }
            set
            {
                m_ulFlag1 = value;
                m_pPacket.Flag1 = value;
                UpdateClient(ClientUpdateType.STATUS_FLAG, m_ulFlag1);
            }
        }

        public ulong Flag2
        {
            get { return m_ulFlag2; }
            set { m_ulFlag2 = value; }
        }

        public bool IsPlayer()
        {
            return false;
        }

        public bool IsNpc()
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

        public bool IsAlive { get { return Life > 0; } }

        public ushort Profession
        {
            get { return m_profession; }
            set { m_profession = value; }
        }

        public int AdjustDefense(int nRawDef)
        {
            return nRawDef;
        }

        public int AdjustWeaponDamage(int nDamage)
        {
            nDamage = (int)Calculations.MulDiv(nDamage, m_dbMonster.Defence2, Calculations.DEFAULT_DEFENCE2);
            return nDamage;
        }

        public int GetAttackRange(int nTargetSizeAdd)
        {
            return m_dbMonster.AttackRange + nTargetSizeAdd;
        }

        public int GetDistance(IScreenObject pObj)
        {
            return (int)Calculations.GetDistance(pObj.MapX, pObj.MapY, m_usMapX, m_usMapY);
        }

        public void Attack()
        {
            if (m_pRoleTarget == null || !m_tAttackMs.ToNextTime())
                return;

            if (m_dbMonster.MagicType <= 0)
            {
                BattleSystem.CreateBattle(m_pRoleTarget.Identity);
                BattleSystem.ProcAttack_Hand2Hand();
            }
            else
            {
                if (m_usUseMagicType > 0)
                {
                    ProcessMagicAttack(m_usUseMagicType, m_pRoleTarget.Identity, m_pRoleTarget.MapX, m_pRoleTarget.MapY);
                }
                else if (m_pDefaultMagic != null)
                    ProcessMagicAttack((ushort)m_dbMonster.MagicType, m_pRoleTarget.Identity, m_pRoleTarget.MapX, m_pRoleTarget.MapY);
            }

            if (m_idAtkMe == m_idActTarget)
                m_bAtkFirst = true;

            m_tAttackMs.Update();
        }

        public int Attack(IRole pTarget, ref InteractionEffect special)//, ref List<InteractionEffect> pEffects)
        {
            if (pTarget == null || pTarget.Identity == Identity)
                return 0;

            int nTempAdjust = 0;
            int nDamage = BattleSystem.CalcPower(IsSimpleMagicAtk() ? 1 : 0, this, pTarget, ref special, nTempAdjust, true);

            return nDamage;
        }

        public int AdjustExperience(IRole pTarget, int nRawExp, bool bNewbieBonusMsg)
        {
            return nRawExp;
        }

        public int CalculateFightRate()
        {
            return 0;
        }

        public int GetExpGemEffect()
        {
            return 0;
        }

        public int GetAtkGemEffect()
        {
            return 0;
        }

        public int GetMAtkGemEffect()
        {
            return 0;
        }

        public int GetSkillGemEffect()
        {
            return 0;
        }

        public int GetTortoiseGemEffect()
        {
            return 0;
        }

        public int AdjustMagicDamage(int nDamage)
        {
            return nDamage;
        }

        public float GetReduceDamage()
        {
            return 0;
        }

        public bool BeAttack(int bMagic, IRole pRole, int nPower, bool bReflectEnable)
        {
            AddAttribute(ClientUpdateType.HITPOINTS, -1 * nPower, true);

            if (!IsAlive && !m_tDisappear.IsActive())
            {
                BeKill(pRole);
                return true;
            }

            if (m_tDisappear.IsActive())
                return false;

            if (!IsMoveEnable())
                return false;

            if (IsEscapeEnable())
            {
                if ((AttackUser & 1) == 0)
                {
                    m_idActTarget = pRole.Identity;
                    m_idMoveTarget = m_idActTarget;
                    ChangeMode(MODE_ESCAPE);
                    return true;
                }

                if (Life <= m_dbMonster.EscapeLife)
                {
                    ChangeMode(MODE_ESCAPE);
                    return true;
                }
            }

            int nDistance = GetDistance(pRole as IScreenObject);
            if (IsFarWeapon() && !pRole.IsFarWeapon() && nDistance <= 2)
            {
                m_idActTarget = pRole.Identity;
                m_idMoveTarget = m_idActTarget;

                int nSteps = GetAttackRange(0) - nDistance;

                FindPath(nSteps);

                ChangeMode(MODE_FORWARD);
                return true;
            }

            if (!IsAttackEnable(pRole))
                return false;

            if (m_nActMode < MODE_FORWARD)
            {
                if (m_nActMode == MODE_ESCAPE && Calculations.ChanceCalc(20f))
                    return false;

                m_idActTarget = pRole.Identity;
                m_idMoveTarget = m_idActTarget;

                if (Calculations.ChanceCalc(80f))
                {
                    ChangeMode(MODE_FORWARD);
                    FindPath(pRole.MapX, pRole.MapY);

                    if (m_nNextDir >= 0)
                        return true;
                }

                if (IsEscapeEnable())
                {
                    ChangeMode(MODE_ESCAPE);
                    FindPath(pRole.MapX, pRole.MapY);
                }
            }
            return true;
        }

        private readonly DateTime m_pEventStart = new DateTime(2016, 09, 24, 00, 00, 00);
        private readonly DateTime m_pEventEnd = new DateTime(2016, 06, 25, 23, 59, 59);
        private readonly uint[] m_pNormalGem = { 700001, 700011, 700021, 700031, 700041, 700051, 700061, 700071, 700101, 7000121 };

        public void BeKill(IRole pRole)
        {
            // todo pet handle

            if (m_tDisappear.IsActive()) return;

            try
            {
                DetachAllStatus(this);
                AttachStatus(this, FlagInt.DEAD, 0, 20, 0, 0);
                AttachStatus(this, FlagInt.GHOST, 0, 20, 0, 0);
                AttachStatus(this, FlagInt.FADE, 0, 20, 0, 0);

                uint dieType = pRole is Character ? ((pRole as Character).KoCount * 65541) : 1;
                var msg = new MsgInteract
                {
                    Action = InteractionType.ACT_ITR_KILL,
                    EntityIdentity = pRole != null ? pRole.Identity : 0,
                    TargetIdentity = Identity,
                    CellX = MapX,
                    CellY = MapY,
                    MagicType = 0,
                    MagicLevel = 0,
                    Data = dieType
                };

                Map.SendToRange(msg, MapX, MapY);
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString());
            }

            m_tDisappear.Startup(5);

            Character pActionUser = null;
            if (pRole != null && pRole is Character)
                pActionUser = pRole as Character;

            Life = 0;

            uint idMapItemOwner = 0;
            if (pActionUser != null)
            {
                idMapItemOwner = pActionUser.Identity;
                if (pActionUser.BattleSystem.IsActived())
                    pActionUser.BattleSystem.ResetBattle();
            }

            // do action
            if (IsDieAction())
                GameAction.ProcessAction(m_dbMonster.Action, pActionUser, this, null, null);

            if (m_pDrop != null)
            {
                for (int i = 0; i < m_pDrop.DropNum; i++)
                {
                    bool bExe = false;
                    foreach (var dropRule in m_pDrop.Actions.OrderBy(x => x.Value))
                    {
                        if (Calculations.ChanceCalc(dropRule.Value/100f))
                        {
                            GameAction.ProcessAction(dropRule.Key, pActionUser, this, null, null);
                            bExe = true;
                            break;
                        }
                    }
                    if (!bExe && m_pDrop.DefaultAction > 0)
                        GameAction.ProcessAction(m_pDrop.DefaultAction, pActionUser, this, null, null);
                }
            }

            if (IsGuard())
                return;

            int nChanceAdjust = 30;
            if (pActionUser != null && GetNameType(pRole.Level, Level) == BattleSystem.NAME_GREEN)
                nChanceAdjust = 10;

            if (Calculations.ChanceCalc(nChanceAdjust))
            {
                int dwMoneyMin = (int)(m_dbMonster.DropMoney * 0.1f);
                int dwMoneyMax = (int)(m_dbMonster.DropMoney * 0.3f);
                uint dwMoney = (uint)(dwMoneyMin + Calculations.Random.Next(dwMoneyMax - dwMoneyMin) + 1);

                int nHeapNum = 1 + Calculations.Random.Next(3);
                uint dwMoneyAve = (uint)(dwMoney / nHeapNum);

                for (int i = 0; i < nHeapNum; i++)
                {
                    uint dwMoneyTmp = (uint)Calculations.MulDiv((int)dwMoneyAve, 90 + Calculations.Random.Next(21), 100);
                    if (pActionUser != null)
                    {
                        switch (pActionUser.Owner.VipLevel)
                        {
                            case 1: dwMoneyTmp = (uint)(dwMoneyTmp * .02f); break;
                            case 2: dwMoneyTmp = (uint)(dwMoneyTmp * .04f); break;
                            case 3: dwMoneyTmp = (uint)(dwMoneyTmp * .06f); break;
                            case 4: dwMoneyTmp = (uint)(dwMoneyTmp * .08f); break;
                            case 5: dwMoneyTmp = (uint)(dwMoneyTmp * .10f); break;
                            case 6: dwMoneyTmp = (uint)(dwMoneyTmp * .12f); break;
                        }
                    }
                    DropMoney(dwMoneyTmp, idMapItemOwner);
                    //pActionUser.AwardMoney(dwMoneyTmp);
                }
            }

            int nDropNum = 0;
            int nAtkLev = Level;
            if (pRole != null)
                nAtkLev = pRole.Level;

            int nRate = Calculations.Random.Next(1000);

            float dropRate = 1.00f;
            float dropRateDB = .10f;
            if (Calculations.ChanceCalc(dropRateDB))
            {
                DropItem(SpecialItem.TYPE_DRAGONBALL, idMapItemOwner, 0, 0, 0, 0);
                if (pRole != null && pRole.IsPlayer())
                    (pRole as Character).Send(string.Format(ServerString.DRAGON_BALL_DROP, pRole.Name));
            }

            if (Calculations.ChanceCalc(.3f) && pRole.IsPlayer())
            {
                (pRole as Character).AwardBoundEmoney(10);
            }

            if (pRole is Character && Calculations.ChanceCalc(dropRate))
            {
                Character pUser = pRole as Character;
                int nAmount = 1;
                int nBonus = 0;
                int DropCPBag = 1; // 1 CPs base

                #region Calculate nAmount with level diference of player and monster
                if (m_dbMonster.Level >= pUser.Level)
                {
                    int levelDif = m_dbMonster.Level - pUser.Level;
                    if (levelDif > 20)
                    {
                        DropCPBag = 3;
                        nAmount = 3; // 3 CPs base
                    }
                    if (levelDif > 40)
                    {
                        DropCPBag = 3;
                        nAmount = 6; // 6 CPs base
                    }
                }
                #endregion

                string szMsg = "You found {0} CPs while looting.";
                if (pUser.Owner.VipLevel > 0)
                {
                    switch (pUser.Owner.VipLevel)
                    {
                        case 1:
                            nBonus = 2;
                            break;
                        case 2:
                            nBonus = 4;
                            break;
                        case 3:
                            nBonus = 6;
                            break;
                        case 4:
                            nBonus = 8;
                            break;
                        case 5:
                            nBonus = 10;
                            break;
                        case 6:
                            nBonus = 12;
                            break;
                    }
                    szMsg = "You received {0} CPs while looting plus {1} CPs for being Vip level {2}.";
                    pUser.Send(string.Format(szMsg, nAmount, nBonus, pUser.Owner.VipLevel));
                }
                else
                {
                    switch(DropCPBag)
                    {
                        case 1:
                            {
                                DropItem(729910, idMapItemOwner, 0, 0, 0, 0);
                                break;
                            }
                        case 2:
                            {
                                DropItem(729911, idMapItemOwner, 0, 0, 0, 0);
                                break;
                            }
                        case 3:
                            {
                                DropItem(729912, idMapItemOwner, 0, 0, 0, 0);
                                break;
                            }
                        default:
                            {
                                DropItem(729910, idMapItemOwner, 0, 0, 0, 0);
                                break;
                            }
                    }
                    //pUser.Send(string.Format(szMsg, nAmount));
                }
                pUser.AwardEmoney(nAmount + nBonus);
            }

            if (Calculations.ChanceCalc(0.005f)) // Fire of hell
                DropItem(1060101, idMapItemOwner, 0, 0, 0, 0);

            if (Calculations.ChanceCalc(0.005f)) // BombScroll
                DropItem(1060100, idMapItemOwner, 0, 0, 0, 0);

            if (Calculations.ChanceCalc(0.8f))
                DropItem(SpecialItem.TYPE_METEOR, idMapItemOwner, 0, 0, 0, 0); // meteor

            if (Calculations.ChanceCalc(.1f)) // DiligenceBook
                DropItem(723340, idMapItemOwner, 0, 0, 0, 0);

            if (Calculations.ChanceCalc(.05f)) // EnduranceBook
                DropItem(723341, idMapItemOwner, 0, 0, 0, 0);

            if (Calculations.ChanceCalc(.003f)) // Oblivion Dew
                DropItem(711083, idMapItemOwner, 0, 0, 0, 0);

            if (Calculations.ChanceCalc(.03f)) // DragonPill
                DropItem(720598, idMapItemOwner, 0, 0, 0, 0);

            if (!IsPkKiller() && !IsGuard() && !IsEvilKiller() && !IsDynaMonster() && !IsDynaNpc())
            {
                if (Calculations.ChanceCalc(.1f))
                {
                    uint dGem = m_pNormalGem[ThreadSafeRandom.RandGet(m_pNormalGem.Length) % m_pNormalGem.Length];
                    DropItem(dGem, idMapItemOwner, 0, 0, 0, 0); // normal gems
                }
            }

            if ((m_dbMonster.Id == 15 || m_dbMonster.Id == 74) && Calculations.ChanceCalc(2f))
            {
                DropItem(1080001, idMapItemOwner, 0, 0, 0, 0); // emerald
            }

            int nChance = BattleSystem.AdjustDrop(200, nAtkLev, Level);
            if (nRate < Math.Min(1000, nChance))
            {
                nDropNum = 1 + Calculations.Random.Next(1, 3); // drop 10-16 items
            }
            else
            {
                nChance += BattleSystem.AdjustDrop(1000, nAtkLev, Level);
                if (nRate < Math.Min(1000, nChance))
                    nDropNum = 1; // drop 1 item
            }

            for (int i = 0; i < nDropNum; i++)
            {
                uint idItemtype = GetDropItem();

                DbItemtype itemtype;
                if (!ServerKernel.Itemtype.TryGetValue(idItemtype, out itemtype))
                    continue;

                byte nDmg = 0;
                byte nPlus = 0;

                if (Calculations.ChanceCalc(.5f))
                {
                    // bless
                    nDmg = (byte)(Calculations.ChanceCalc(10) ? 5 : 3);
                }
                if (Calculations.ChanceCalc(.3f))
                {
                    // plus
                    nPlus = 1;
                }

                if (!DropItem(itemtype, idMapItemOwner, nPlus, nDmg, 0, (short)Calculations.Random.Next(-200, 300)))
                    break;
            }

            #region CloudSaint's Jar Kills
            if (pRole.IsPlayer())
            {
                Character pUser = pRole as Character;
                QuestJar quest = ServerKernel.PlayerQuests.Where(x => x.Player.Identity == pUser.Identity && x.Monster.Id == m_dbMonster.Id).FirstOrDefault();
                if (quest == null)
                {
                    quest = new QuestJar(pUser, m_dbMonster) { };
                    quest.AddKills();
                    ServerKernel.PlayerQuests.Add(quest);
                }
                else
                {
                    quest.AddKills();
                }
            }
            #endregion
        }

        public bool DropItem(uint idItemtype, uint idOwner, byte nMagic3, byte nBless, byte nEnchant, short sExtraDura)
        {
            DbItemtype db;
            if (ServerKernel.Itemtype.TryGetValue(idItemtype, out db))
            {
                return DropItem(db, idOwner, nMagic3, nBless, nEnchant, sExtraDura);
            }
            return false;
        }

        public bool DropItem(DbItemtype idItemtype, uint idOwner, byte nMagic3, byte nBless, byte nEnchant, short sExtraDura)
        {
            if (idItemtype == null) return true;

            var pos = new Point(MapX, MapY);
            if (Map.FindDropItemCell(4, ref pos))
            {
                var pMapItem = new MapItem();

                if (pMapItem.Create((uint)Map.FloorItem, Map, pos, idItemtype, idOwner, nMagic3, nBless, sExtraDura))
                {
                    return true;
                }
            }
            return false;
        }

        public bool DropMoney(uint dwMoney, uint idOwner)
        {
            if (dwMoney <= 0)
                return false;

            if (Level >= 80 && dwMoney < 10)
                return true;

            var pos = new Point(MapX, MapY);
            if (Map.FindDropItemCell(2, ref pos))
            {
                var pMapItem = new MapItem();
                if (pMapItem.CreateMoney((uint)Map.FloorItem, Map, pos, dwMoney, idOwner))
                {
                    Map.AddItem(pMapItem);
                    return true;
                }
            }

            return false;
        }

        private readonly int[] m_dropHeadgear = { 111000, 112000, 113000, 114000, 143000, 118000, 123000, 141000, 142000, 117000 };
        private readonly int[] m_dropNecklace = { 120000, 121000 };
        private readonly int[] m_dropArmor = { 130000, 131000, 133000, 134000, 135000, 136000 };
        private readonly int[] m_dropRing = { 150000, 151000, 152000 };
        private readonly int[] m_dropWeapon =
        {
            410000, 420000, 421000, 430000, 440000, 450000, 460000, 480000, 481000,
            490000, 500000, 510000, 530000, 540000, 560000, 561000, 580000, 601000, 610000
        };

        public uint GetDropItem()
        {
            /*
             * 0 = armet
             * 1 = necklace
             * 2 = armor
             * 3 = ring
             * 4 = weapon
             * 5 = shield
             * 6 = shoes
             * 7 = hp
             * 8 = mp
             */
            var possibleDrops = new List<int>();
            if (m_dbMonster.DropArmet != 0)
                possibleDrops.Add(0);
            if (m_dbMonster.DropNecklace != 0)
                possibleDrops.Add(1);
            if (m_dbMonster.DropArmor != 0)
                possibleDrops.Add(2);
            if (m_dbMonster.DropRing != 0)
                possibleDrops.Add(3);
            if (m_dbMonster.DropWeapon != 0)
                possibleDrops.Add(4);
            if (m_dbMonster.DropShield != 0)
                possibleDrops.Add(5);
            if (m_dbMonster.DropShoes != 0)
                possibleDrops.Add(6);
            if (m_dbMonster.DropHp != 0)
                possibleDrops.Add(7);
            if (m_dbMonster.DropMp != 0)
                possibleDrops.Add(8);

            if (possibleDrops.Count <= 0)
                return 0;

            int type = possibleDrops[Calculations.Random.Next(possibleDrops.Count) % possibleDrops.Count];
            uint dwItemId = 0;

            switch (type)
            {
                case 0:
                    dwItemId += (uint)m_dropHeadgear[Calculations.Random.Next(m_dropHeadgear.Length - 1)];
                    dwItemId += (uint)(m_dbMonster.DropArmet * 10);
                    break;
                case 1:
                    dwItemId += (uint)m_dropNecklace[Calculations.Random.Next(m_dropNecklace.Length - 1)];
                    dwItemId += (uint)(m_dbMonster.DropNecklace * 10);
                    break;
                case 2:
                    dwItemId += (uint)m_dropArmor[Calculations.Random.Next(m_dropArmor.Length - 1)];
                    dwItemId += (uint)(m_dbMonster.DropArmor * 10);
                    break;
                case 3:
                    dwItemId += (uint)m_dropRing[Calculations.Random.Next(m_dropRing.Length - 1)];
                    dwItemId += (uint)(m_dbMonster.DropRing * 10);
                    break;
                case 4:
                    dwItemId += (uint)m_dropWeapon[Calculations.Random.Next(m_dropWeapon.Length - 1)];
                    dwItemId += (uint)(m_dbMonster.DropWeapon * 10);
                    break;
                case 5:
                    dwItemId += 900000;
                    dwItemId += (uint)(m_dbMonster.DropShield * 10);
                    break;
                case 6:
                    dwItemId += 160000;
                    dwItemId += (uint)(m_dbMonster.DropShoes * 10);
                    break;
                case 7:
                    return m_dbMonster.DropHp;
                case 8:
                    return m_dbMonster.DropMp;
                default:
                    return 0;
            }

            uint nNewLev = (uint)(((dwItemId % 100) / 10) + Calculations.Random.Next(-2, 2));

            switch (type)
            {
                case 0:
                case 2:
                case 5:
                    if (nNewLev > 0 && nNewLev <= 10)
                        dwItemId += (nNewLev) * 10;
                    break;
                case 1:
                case 3:
                case 6:
                    if (nNewLev > 0 && nNewLev <= 24)
                        dwItemId += (nNewLev) * 10;
                    break;
                case 4:
                    if (nNewLev > 0 && nNewLev <= 33)
                        dwItemId += (nNewLev) * 10;
                    break;
            }

            if (Calculations.ChanceCalc(0.01f)) dwItemId += 9; // super
            else if (Calculations.ChanceCalc(0.05f)) dwItemId += 8; // elite
            else if (Calculations.ChanceCalc(0.1f)) dwItemId += 7; // unique
            else if (Calculations.ChanceCalc(0.2f)) dwItemId += 6; // refined
            else if (Calculations.ChanceCalc(0.1f) && type == 4) dwItemId += 0; // fixed
            else dwItemId += (uint)Calculations.Random.Next(3, 5); // normal

            return dwItemId;
        }

        public void DelMonster(bool bNow)
        {
            if (IsDeleted())
                return;

            if (m_pData != null) // may be null, check if pet
            {
                m_pData.Life = 0;
            }

            if (bNow)
            {
                m_tDisappear.Startup(0);
                SendLeaveFromBlock();
            }
            else
            {
                m_tDisappear.Startup(10);
            }
        }

        public bool IsDeleted()
        {
            return m_tDisappear.IsActive();
        }

        public bool IsDead()
        {
            if (Life <= 0)
            {
                if (m_tDisappear.IsActive())
                    return true;
                m_tDisappear.Startup(5);
                return true;
            }
            return false;
        }

        public bool CanDisappear
        {
            get
            {
                if (m_tDisappear.IsActive() && m_tDisappear.IsTimeOut())
                {
                    m_tDisappear.Clear();
                    return true;
                }
                return false;
            }
        }

        private bool IsDieAction()
        {
            return m_dbMonster.Action > 0;
        }

        public bool AddAttribute(ClientUpdateType type, long data, bool synchro)
        {
            switch (type)
            {
                case ClientUpdateType.HITPOINTS:
                    {
                        var remainingLife = (int)(Life + data);
                        if (remainingLife <= 0)
                            Life = 0;
                        else
                            Life = (uint)Math.Min(MaxLife, remainingLife);
                        return true;
                    }
            }
            return false;
        }

        public bool IsFarWeapon() { return false; }
        public bool AutoSkillAttack(IRole pTarget) { return false; }
        public bool SetAttackTarget(IRole pTarget) { return true; }
        public bool CheckCrime(IRole pRole) { return false; }
        public bool IsBlinking() { return false; }
        public void AwardBattleExp(int nExp, bool bGemEffect) { }
        public bool IsImmunity(IRole pTarget) { return false; }

        public bool IsWellStatus1(ulong nStatus) { return true; }
        public bool IsBadlyStatus1(ulong nStatus) { return true; }

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
                case FlagInt.HEAVEN_BLESS:
                case FlagInt.AZURE_SHIELD:
                case FlagInt.CARYING_FLAG:
                case FlagInt.EARTH_AURA:
                case FlagInt.FEND_AURA:
                case FlagInt.FIRE_AURA:
                case FlagInt.METAL_AURA:
                case FlagInt.TYRANT_AURA:
                case FlagInt.WATER_AURA:
                case FlagInt.WOOD_AURA:
                case FlagInt.OBLIVION:
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
                case Effect0.HEAVEN_BLESS:
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
                if (nTimes > 0)
                {
                    var pNewStatus = new StatusMore();
                    if (pNewStatus.Create(pRole, nStatus, nPower, nSecs, nTimes, wCaster))
                    {
                        pRole.Status.AddObj(pNewStatus);
                        return true;
                    }
                }
                else
                {
                    var pNewStatus = new StatusOnce();
                    if (pNewStatus.Create(pRole, nStatus, nPower, nSecs, 0, wCaster))
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

            return pStatusSet.DelObj(StatusSet.InvertFlag(nType, b64));
        }

        public IStatus QueryStatus(int nType)
        {
            return Status == null ? null : Status.GetObjByIndex(nType);
        }

        public IStatus QueryStatus(Effect0 nType)
        {
            return Status == null ? null : Status.GetObj((ulong)nType);
        }

        public IStatus QueryStatus(Effect1 nType)
        {
            return Status == null ? null : Status.GetObj((ulong)nType, true);
        }

        public bool IsInFan(Point pos, Point posSource, int nRange, int nWidth, Point posCenter)
        {
            return Calculations.IsInFan(pos, posSource, nRange, nWidth, posCenter);
        }

        public bool IsAttackable(IRole pTarget)
        {
            if (!IsAlive)
                return false;
            return true;
        }

        public bool SynchroPosition(int x, int y, int nDistance = 8)
        {
            return true;
        }

        public bool IsBeAttackable() { return true; }
        public bool IsBowman() { return false; }
        public bool IsSimpleMagicAtk() { return false; }
        public bool IsEvil() { return (AttackUser & (4 | 8192)) == 0; }
        public bool IsMonster()
        {
            return !IsSynPet() && !IsGuard() && !IsPkKiller() && !IsEvilKiller();
        }

        public void Kill(IRole pTarget, ulong dwDieWay)
        {
            if (pTarget == null) return;

            MsgInteract pMsg = new MsgInteract
            {
                Action = InteractionType.ACT_ITR_KILL,
                EntityIdentity = Identity,
                TargetIdentity = pTarget.Identity,
                CellX = pTarget.MapX,
                CellY = pTarget.MapY,
                Data = (uint)dwDieWay
            };
            m_pMap.SendToRange(pMsg, pTarget.MapX, pTarget.MapY);

            pTarget.BeKill(this);
        }

        public bool AdditionMagic(int nLifeLost, int nDamage) { return false; }
        public void SendDamageMsg(uint pTarget, uint nDamage, InteractionEffect special)
        {
            m_pMap.SendToRange(new MsgInteract
            {
                EntityIdentity = Identity,
                TargetIdentity = pTarget,
                Action = InteractionType.ACT_ITR_ATTACK,
                CellX = MapX,
                CellY = MapY,
                Data = nDamage,
                //ActivationType = special,
                //ActivationValue = nDamage
            }, m_usMapX, m_usMapY);
        }
        public bool IsGoal() { return false; }

        public bool IsAttackEnable(IRole pTarget)
        {
            if (pTarget.IsWing() && !IsWing() && IsCloseAttack())
                return false;
            return true;
        }

        public bool IsWing()
        {
            return false;
        }

        public bool IsOpposedSyn(uint pRoleId)
        {
            if (!IsSynPet()) return false;
            return false;
        }

        public bool IsSynPet()
        {
            return false;
        }

        public bool IsLockUser()
        {
            return (AttackUser & 256) != 0;
        }

        public bool IsRighteous()
        {
            return (AttackUser & 4) != 0;
        }

        public bool IsGuard()
        {
            return (AttackUser & 8) != 0;
        }

        public bool IsPkKiller()
        {
            return (AttackUser & 16) != 0;
        }

        public bool IsWalkEnable()
        {
            return (AttackUser & 64) == 0;
        }

        public bool IsJumpEnable()
        {
            return (AttackUser & 32) != 0;
        }

        public bool IsFastBack()
        {
            return (AttackUser & 128) != 0;
        }

        public bool IsLockOne()
        {
            return (AttackUser & 512) != 0;
        }

        public bool IsAddLife()
        {
            return (AttackUser & 1024) != 0;
        }

        public bool IsEvilKiller()
        {
            return (AttackUser & 2048) != 0;
        }

        public bool IsDormancyEnable()
        {
            return (AttackUser & 256) == 0;
        }

        public bool IsCloseAttack()
        {
            return !IsBowman() && !IsSimpleMagicAtk();
        }

        public int GetNameType(int nAtkLev, int nDefLev)
        {
            int nDeltaLev = nAtkLev - nDefLev;

            if (nDeltaLev >= 3)
                return BattleSystem.NAME_GREEN;
            if (nDeltaLev >= 0)
                return BattleSystem.NAME_WHITE;
            if (nDeltaLev >= -5)
                return BattleSystem.NAME_RED;
            return BattleSystem.NAME_BLACK;
        }

        #endregion

        #region AI Handle

        public bool CheckMagicAttack()
        {
            m_usUseMagicType = 0;

            if (m_pRoleTarget == null)
                return false;

            if (Magics.Magics.Count > 0)
            {
                foreach (var magic in m_dbMonsterMagics.OrderBy(x => x.Chance))
                {
                    if (Magics.CheckType(magic.MagicIdentity) && Calculations.ChanceCalc(magic.Chance / 100f))
                    {
                        m_usUseMagicType = magic.MagicIdentity;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ProcessAttack() // felipe apr/10/2015
        {
            if (m_pRoleTarget != null)
            {
                if (m_pRoleTarget.IsAlive
                    && (CheckMagicAttack()
                    || GetDistance(m_pRoleTarget as IScreenObject) <= GetAttackRange(m_pRoleTarget.GetSizeAdd())))
                {
                    if (m_tAction.ToNextTime())
                    {
                        if (Map.IsSuperposition(this))
                        {
                            m_bAheadPath = false;
                            DetectPath(-1);
                            m_bAheadPath = true;
                            if (m_nNextDir >= 0) PathMove(WALKMODE_SHIFT);
                        }

                        if (m_bAttackFlag)
                            m_bAttackFlag = false;
                        else
                        {
                            ChangeMode(MODE_FORWARD);
                            return true;
                        }
                    }
                    return true;
                }
            }

            // no target
            ChangeMode(MODE_IDLE);
            return true;
        }

        public bool ProcessForward()
        {
            // test attack enable
            //IRole pRole = m_pRoleTarget ?? Map.QueryRole(m_idMoveTarget) as IRole;
            if (m_pRoleTarget != null)
            {
                int nDistance = GetDistance(m_pRoleTarget as IScreenObject);
                if (CheckMagicAttack() || (m_pRoleTarget.IsAlive && nDistance <= GetAttackRange(m_pRoleTarget.GetSizeAdd())))
                {
                    if (!IsGuard() && !IsMoveEnable() && !IsFarWeapon() && !m_bAheadPath && m_nNextDir >= 0)
                    {
                        if (PathMove(WALKMODE_RUN)) return true;
                    }

                    //m_tAttackMs.Update();
                    ChangeMode(MODE_ATTACK);
                    return true;
                }
            }

            // process forward
            if (IsGuard() || IsPkKiller() || IsFastBack() && m_pGenerator.IsTooFar(MapX, MapY, 48))
            {
                Point pos = m_pGenerator.GetCenter();
                m_idActTarget = 0;
                m_idMoveTarget = m_idActTarget;
                m_pRoleTarget = null;
                FarJump(pos.X, pos.Y, (int)Direction);
                ClearPath();
                ChangeMode(MODE_IDLE);
                return true;
            }

            // guard jump
            if ((IsGuard() || IsPkKiller() || IsEvilKiller()) && m_pRoleTarget != null &&
                GetDistance(m_pRoleTarget.MapX, m_pRoleTarget.MapY) >= 6)
            {
                JumpPos(m_pRoleTarget.MapX, m_pRoleTarget.MapY, (int)Direction);
                return true;
            }

            // forward
            if (m_nNextDir < 0)
            {
                if (FindNewTarget())
                {
                    if (m_nNextDir < 0)
                    {
                        if (IsJumpEnable())
                        {
                            JumpBlock(m_pRoleTarget.MapX, m_pRoleTarget.MapY, (int)Direction);
                            return true;
                        }
                        ChangeMode(MODE_IDLE);
                        return true;
                    }
                    return false;
                }
                ChangeMode(MODE_IDLE);
                return true;
            }

            //if (m_idMoveTarget != 0)
            if (m_pRoleTarget != null)
            {
                if (m_pRoleTarget != null && GetDistance(m_pRoleTarget.MapX, m_pRoleTarget.MapY) <= ViewRange)
                {
                    FindPath();
                    m_tAttackMs.Update();
                    PathMove(WALKMODE_RUN);
                }
                else
                {
                    //m_idMoveTarget = 0;
                    m_pRoleTarget = null;
                }
            }
            else
            {
                //PathMove(WALKMODE_MOVE);
                ChangeMode(MODE_IDLE);
            }
            return true;
        }

        /// <summary>
        /// This process the monster when it's stand still and doesn't have a target. This method will lookup
        /// through the screen and will look out for targets.
        /// </summary>
        /// <returns></returns>
        public bool ProcessIdle()
        {
            // change to other mode
            m_idAtkMe = 0;
            m_bAtkFirst = false;
            if (IsActive && FindNewTarget()) // find a new target to attack
            {
                if (m_pRoleTarget == null) return false;

                int nDistance = GetDistance(m_pRoleTarget.MapX, m_pRoleTarget.MapY);
                if (nDistance <= GetAttackRange(m_pRoleTarget.GetSizeAdd()))
                {
                    ChangeMode(MODE_ATTACK);
                    return false;
                }
                if (IsMoveEnable())
                {
                    if (m_nNextDir < 0)
                    {
                        if (!IsEscapeEnable() || Calculations.ChanceCalc(80f)) return true;

                        ChangeMode(MODE_ESCAPE);
                        return false;
                    }
                    ChangeMode(MODE_FORWARD);
                    return false;
                }
            }

            if (IsGuard() || IsPkKiller() || IsEvilKiller())
            {
                if (m_nNextDir < 0)
                {
                    PathMove(WALKMODE_MOVE);
                    return true;
                }

                if (m_tAction.ToNextTime())
                    return true;
            }

            if (!IsMoveEnable())
                return true;

            if (m_pGenerator.IsInRegion(MapX, MapY))
            {
                if (IsGuard() || IsPkKiller() || IsEvilKiller())
                {
                    if (Calculations.Random.Next(75) == 5
                        && m_pGenerator.GetWidth() > 1 || m_pGenerator.GetHeight() > 1)
                    {
                        int x = Calculations.Random.Next(m_pGenerator.GetWidth() + m_pGenerator.GetPosX());
                        int y = Calculations.Random.Next(m_pGenerator.GetHeight() + m_pGenerator.GetPosY());

                        if (FindPath((ushort)x, (ushort)y))
                            PathMove(WALKMODE_MOVE);
                    }
                }
                else
                {
                    if (Calculations.Random.Next(50) == 0)
                    {
                        int nDir = Calculations.Random.Next(0, 7);
                        if (TestPath(nDir))
                            PathMove(WALKMODE_MOVE);

                    }
                }
            }
            else // out of range, move back
            {
                if ((IsGuard() || IsPkKiller() || IsFastBack()) || Calculations.Random.Next(5) == 0)
                {
                    if (!m_pGenerator.IsInRegion(MapX, MapY))
                    {
                        Point pos = m_pGenerator.GetCenter();
                        if (FindPath((ushort)pos.X, (ushort)pos.Y))
                            PathMove(WALKMODE_MOVE);
                        else if (/*IsGuard() || IsPkKiller() || */IsJumpEnable())
                            JumpBlock(pos.X, pos.Y, (int)Direction);
                    }
                }
            }

            return true;
        }

        public bool ProcessEscape()
        {
            if (!IsEscapeEnable())
            {
                ChangeMode(MODE_IDLE);
                return true;
            }

            //IRole pRole = m_pRoleTarget ?? Map.QueryRole(m_idActTarget) as IRole;
            if ((IsGuard() || IsPkKiller()) && m_pRoleTarget != null)
            {
                JumpPos(m_pRoleTarget.MapX, m_pRoleTarget.MapY, (int)Direction);
                ChangeMode(MODE_FORWARD);
                return true;
            }

            if (m_nNextDir < 0)
                FindPath(ViewRange * 2);

            if (m_pRoleTarget == null)
            {
                ChangeMode(MODE_IDLE);
                return true;
            }

            if (m_pRoleTarget != null && m_nNextDir < 0)
            {
                ChangeMode(MODE_FORWARD);
                return true;
            }
            PathMove(WALKMODE_RUN);
            return true;
        }

        public bool IsEscapeEnable()
        {
            return true;
        }

        public bool ChangeMode(int nNewMode)
        {
            switch (nNewMode)
            {
                case MODE_DORMANCY:
                    Life = (uint)m_dbMonster.Life;
                    break;
                case MODE_ATTACK:
                    Attack();
                    break;
            }
            if (nNewMode != MODE_FORWARD)
                ClearPath();
            m_nActMode = (uint)nNewMode;
            return true;
        }

        public bool CheckTarget()
        {
            //if (m_idActTarget == 0)
            //{
            //    m_idMoveTarget = 0;
            //    return false;
            //}
            //if (m_pRoleTarget == null)
            //{
            //    m_idMoveTarget = 0;
            //    return false;
            //}

            //var pRole = Map.QueryRole(m_idActTarget) as IRole;
            if (m_pRoleTarget == null || !m_pRoleTarget.IsAlive)
            {
                m_idActTarget = 0;
                m_idMoveTarget = 0;
                return false;
            }

            if (m_pRoleTarget.IsWing() && !IsWing() && !IsCloseAttack())
                return false;

            int nDistance = ViewRange;
            int nAtkDistance = Calculations.CutOverflow(nDistance, GetAttackRange(0));
            int nDist = GetDistance(m_pRoleTarget as IScreenObject);

            if (!(nDist <= nDistance) || (nDist <= nAtkDistance && GetAttackRange(0) > 1))
            {
                m_idActTarget = 0;
                m_idMoveTarget = 0;
                return false;
            }

            return true;
        }

        public bool SynchroPos(int nSourX, int nSourY, int nTargX, int nTargY)
        {
            if (MapX != nSourX || MapY != nSourY)
            {
                Map.SendToRange(
                        new MsgAction(Identity, (ushort)nSourX, (ushort)nSourY, GeneralActionType.NEW_COORDINATES),
                        MapX, MapY);
            }
            else if (MapX != nTargX || MapY != nTargY)
            {
                if (!Map.IsValidPoint(nTargX, nTargY))
                    return false;
                ClearPath();
                SetPos(nTargX, nTargY);
            }

            return true;
        }

        public void SetPos(int nPosX, int nPosY)
        {
            MapX = (ushort)nPosX;
            MapY = (ushort)nPosY;
        }

        public bool FindPath(ushort x, ushort y)
        {
            if (x == MapX && y == MapY)
                return false;

            int nDir = Calculations.GetDirection(MapX, MapY, x, y);

            Point pos = new Point(MapX, MapY);

            for (int i = 0; i < 8; i++)
            {
                nDir += i;
                if (TestPath(nDir))
                {
                    m_nNextDir = nDir;
                    break;
                }
            }

            return m_nNextDir >= 0;
        }

        public bool FindPath(int nScapeSteps = 0)
        {
            if (Map == null || m_idMoveTarget == 0) return false;

            m_bAheadPath = (nScapeSteps == 0);
            ClearPath();

            var pRole = Map.QueryRole(m_idMoveTarget) as IRole;
            if (pRole == null || !pRole.IsAlive || GetDistance(pRole.MapX, pRole.MapY) > ViewRange)
            {
                m_idActTarget = 0;
                m_idMoveTarget = m_idActTarget;
                ClearPath();
                return m_nNextDir < 0;
            }

            if (!FindPath(pRole.MapX, pRole.MapY))
                return false;

            if (m_nNextDir >= 0)
            {
                int nDir = m_nNextDir % 8;
                // int nDir = Calculations.GetDirectionSector(MapX, MapY, (ushort) pTarget.X, (ushort) pTarget.Y);
                if (!Map.IsMoveEnable(MapX, MapY, nDir, 0, 0) && m_pMap[m_usMapX, m_usMapY].Access > TileType.MONSTER)
                {
                    DetectPath(nDir);
                    return m_nNextDir >= 0;
                }

            }

            return m_nNextDir >= 0;
        }

        public bool FindNewTarget()
        {
            if (Map == null) return false;

            // lock target
            if (IsLockUser() || IsLockOne())
            {
                if (CheckTarget())
                {
                    if (IsLockOne())
                        return true;
                }
                else
                {
                    if (IsLockUser())
                        return false;
                }
            }

            uint idOldTarget = m_idActTarget;
            m_idActTarget = 0;
            m_idMoveTarget = 0;
            Character pTargetUser = null;
            int nDistance = m_dbMonster.ViewRange;

            var poss = Map.CollectMapThing(ViewRange, new Point(MapX, MapY));

            if (poss == null || poss.Count <= 0)
                return false;

            //var pUserSet = Map.Players.Values.Where(x => x != null && x.MapX == pPos.X && x.Character.MapY == pPos.Y).ToList();
            foreach (var pUser in poss.Where(x => x != null && Calculations.InScreen(x.MapX, x.MapY, m_usMapX, m_usMapY)))//.OfType<Character>())
            {
                if (pUser is Character)
                {
                    var pRoleUser = pUser as Character;
                    if (pRoleUser.IsAlive && pRoleUser.Identity != Identity)
                    {
                        if ((IsGuard() && pRoleUser.IsCrime())
                            || (IsPkKiller() && pRoleUser.IsPker())
                            || (IsEvilKiller() && pRoleUser.IsVirtuous())
                            || (IsEvil() && !(IsPkKiller() || IsEvilKiller()))
                            || (IsSynPet() && IsOpposedSyn(pRoleUser.Identity)))
                        {
                            if (!IsAttackEnable(pRoleUser)) continue;

                            int nAtkDistance = Calculations.CutOverflow(nDistance, GetAttackRange(0));
                            int nDist = GetDistance(pRoleUser);

                            if (nDist <= nDistance)
                            {
                                nDistance = nDist;

                                m_idActTarget = pRoleUser.Identity;
                                m_idMoveTarget = pRoleUser.Identity;
                                m_pRoleTarget = pRoleUser;

                                pTargetUser = pRoleUser;
                            }
                        }
                    }
                }
                else if (pUser is Monster)
                {
                    var pNpc = pUser as Monster;

                    if (pNpc.IsAlive && pNpc.Identity != Identity
                    && (IsEvil() && pNpc.IsRighteous()
                        || IsRighteous() && pNpc.IsEvil()))
                    {
                        if (pNpc.IsWing() && !IsWing() && IsCloseAttack()) continue;

                        int nDist = GetDistance(pNpc);
                        if (nDist <= nDistance)
                        {
                            nDistance = nDist;
                            m_idActTarget = pNpc.Identity;
                            m_idMoveTarget = m_idActTarget;
                            m_pRoleTarget = pNpc;
                        }
                    }
                }
            }
            //var pNpcSet = Map.GameObjects.Values.Where(x => Calculations.InScreen(x.MapX, x.MapY, MapX, MapY)).ToList();
            //foreach (var pNpc in poss.OfType<Monster>())
            //{
            //    if (pNpc.IsAlive && pNpc.Identity != Identity
            //        && (IsEvil() && pNpc.IsRighteous()
            //            || IsRighteous() && pNpc.IsEvil()))
            //    {
            //        if (pNpc.IsWing() && !IsWing() && IsCloseAttack()) continue;

            //        int nDist = GetDistance(pNpc);
            //        if (nDist <= nDistance)
            //        {
            //            nDistance = nDist;
            //            m_idActTarget = pNpc.Identity;
            //            m_idMoveTarget = m_idActTarget;
            //            m_pRoleTarget = pNpc;
            //        }
            //    }
            //}


            if (m_idActTarget != 0)
            {
                if (pTargetUser != null && idOldTarget != pTargetUser.Identity)
                {
                    if (IsGuard() && pTargetUser.IsCrime())
                    {
                        // send guard message
                    }
                    else if (IsPkKiller() && pTargetUser.IsPker() && m_nActMode == MODE_IDLE)
                    {
                        // send guard message
                    }
                }

                FindPath();
                return m_idActTarget != 0;
            }

            m_nNextDir = -1;
            return false;
        }

        #endregion

        #region IOnTimer

        public void StatusTimer()
        {
            if (m_statusCheck.ToNextTime())
            {
                if (Status.Status.Count <= 0)
                    return;

                foreach (var stts in Status.Status.Values)
                {
                    stts.OnTimer();
                    if (!stts.IsValid && stts.Identity != FlagInt.GHOST && stts.Identity != FlagInt.DEAD)
                    {
                        Status.DelObj(stts.Identity);
                    }
                }
            }
        }

        public bool IsActive
        {
            get
            {
                //return m_pGenerator.ContainsTarget();
                return TargetCount > 0;
            }
        }

        public void OnTimer()
        {
            try
            {
                if (!IsAlive || (m_tLocked.IsActive() && m_tLocked.IsTimeOut()))
                    return;
                switch (m_nActMode)
                {
                    case MODE_ESCAPE:
                        ProcessEscape();
                        return;
                    case MODE_ATTACK:
                        ProcessAttack();
                        return;
                    case MODE_FORWARD:
                        ProcessForward();
                        return;
                    case MODE_IDLE:
                        ProcessIdle();
                        return;
                }
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
            }
        }

        #endregion

        #region IScreenObject

        public IScreenObject FindAroundRole(uint idRole)
        {
            return Map.FindAroundRole(this, idRole);
        }

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
            set { m_sElevation = value; }
        }

        public void SendSpawnTo(Character pObj)
        {
            pObj.Send(m_pPacket);
        }
        #endregion

        #region Generator Methods

        public bool IsReviveEnable
        {
            get
            {
                return !IsAlive && m_tRevive.IsTimeOut();
            }
        }

        //public void Revive()
        //{
        //    if (!IsReviveEnable)
        //        return;

        //    DetachStatus(FlagInt.DEAD);
        //    DetachStatus(FlagInt.GHOST);
        //    DetachStatus(FlagInt.FADE);
        //    DetachBadlyStatus(this);

        //    Life = (uint) m_dbMonster.Life;
        //}

        #endregion

        #region Movement

        private int m_nNextDir = -1;
        private bool m_bAheadPath;

        public bool PathMove(int nMode)
        {
            if (nMode == 0)
            {
                if (!m_tMoveMs.ToNextTime(m_dbMonster.MoveSpeed))
                    return true;
            }
            else
            {
                if (!m_tMoveMs.ToNextTime((int)(m_dbMonster.RunSpeed)))
                    return true;
            }

            int nDir = (int)(m_nNextDir % 8);

            if (MoveFoward(nDir, nMode))
                return true;

            if (DetectPath(nDir) && m_nNextDir >= 0)
            {
                return MoveFoward(nDir, nMode);
            }

            if (IsJumpEnable())
            {
                var pos = m_pGenerator.GetCenter();
                return JumpBlock(pos.X, pos.Y, (int)Direction);
            }

            return false;
        }

        private void ClearPath()
        {
            m_nNextDir = -1;
        }

        private bool DetectPath(int nNoDir)
        {
            ClearPath();

            var posTarget = new Point();
            if (m_pRoleTarget != null)
            {
                posTarget.X = m_pRoleTarget.MapX;
                posTarget.Y = m_pRoleTarget.MapY;
            }
            else if (m_idMoveTarget != 0)
            {
                IRole pRole = Map.QueryRole(m_idMoveTarget) as IRole;
                if (pRole != null)
                {
                    posTarget.X = pRole.MapX;
                    posTarget.Y = pRole.MapY;
                }
            }
            else
            {
                posTarget = m_pGenerator.GetCenter();
            }

            int nDistOld = GetDistance((ushort)posTarget.X, (ushort)posTarget.X);
            int nBestDist = nDistOld;
            int nBestDir = -1; // -1: invalid dir
            int nFirstDir = Calculations.Random.Next(8);

            for (int i = nFirstDir; i < 8; i++)
            {
                int nDir = nFirstDir % 8;
                if (nDir != nNoDir)
                {
                    if (Map != null && Map.IsMoveEnable(MapX, MapY, nDir, 0, 0))
                    {
                        ushort nNewX = (ushort)(MapX + Handlers.WALK_X_COORDS[nDir]);
                        ushort nNewY = (ushort)(MapY + Handlers.WALK_Y_COORDS[nDir]);
                        int nDist = GetDistance(nNewX, nNewY);
                        if ((nBestDist - nDist) * (m_bAheadPath ? 1 : -1) > 0)
                        {
                            nBestDist = nDist;
                            nBestDir = nDir;
                        }
                    }
                }
            }

            if (nBestDir != -1)
            {
                m_nNextDir = nBestDir;
                return true;
            }

            return false;
        }

        public bool TestPath(int nDir)
        {
            if (nDir < 0 || nDir > 7) return false;

            if (Map != null && Map.IsMoveEnable(MapX, MapY, nDir, 0, 0))
            {
                m_nNextDir = nDir;
                return true;
            }

            return false;
        }

        public bool MoveFoward(int nDir, int nMode = 0)
        {
            if (nDir < 0) return false;

            Direction = (FacingDirection)(nDir % 8);

            ushort nNewPosX = (ushort)(MapX + Handlers.WALK_X_COORDS[nDir]);
            ushort nNewPosY = (ushort)(MapY + Handlers.WALK_Y_COORDS[nDir]);

            if (!Map.IsValidPoint(nNewPosX, nNewPosY))
                return false;

            IScreenObject obj = Map.GameObjects.Values.FirstOrDefault(x => x.MapX == nNewPosX && x.MapY == nNewPosY);
            if (Map.IsMoveEnable(MapX, MapY, nDir, 0, 0) && obj == null)
            {
                Map.SendToRange(new MsgWalk((uint)nDir, Identity, (MovementType)nMode, MapIdentity), MapX, MapY);
                MapX = nNewPosX;
                MapY = nNewPosY;
                return true;
            }

            return false;
        }

        public bool JumpBlock(int x, int y, int nDir)
        {
            int nSteps = (int)GetDistance((ushort)x, (ushort)y);

            if (nSteps <= 0) return false;

            for (int i = 1; i < nSteps; i++)
            {
                Point pos = new Point()
                {
                    X = MapX + (x - MapX) * i / nSteps,
                    Y = MapY + (y - MapY) * i / nSteps
                };

                if (Map.IsStandEnable((ushort)pos.X, (ushort)pos.Y))
                {
                    JumpPos(pos.X, pos.Y, nDir);
                    return true;
                }
            }

            if (Map.IsStandEnable((ushort)x, (ushort)y))
            {
                JumpPos(x, y, nDir);
                return true;
            }

            return false;
        }

        public int GetDistance(ushort x, ushort y)
        {
            return (int)Calculations.GetDistance(MapX, MapY, x, y);
        }

        public void JumpPos(int x, int y, int nDir)
        {
            if (!Map.IsValidPoint(x, y))
                return;

            Map.SendToRange(new MsgAction(Identity, (ushort)x, (ushort)y, GeneralActionType.JUMP), MapX, MapY);

            ClearPath();
            MapX = (ushort)x;
            MapY = (ushort)y;
            Direction = (FacingDirection)(nDir % 8);
        }

        public bool FarJump(int x, int y, int nDir)
        {
            int nSteps = (int)Distance(MapX, MapY, x, y);

            if (nSteps <= 0) return true;

            if (Map.IsStandEnable((ushort)x, (ushort)y))
            {
                JumpPos(x, y, nDir);
                return true;
            }

            for (int i = 1; i < nSteps; i++)
            {
                var pos = new Point
                {
                    X = x + (MapX - x) * i / nSteps,
                    Y = y + (MapY - y) * i / nSteps
                };
                if (Map.IsStandEnable((ushort)pos.X, (ushort)pos.Y))
                {
                    JumpPos(pos.X, pos.Y, nDir);
                    return true;
                }
            }

            return false;
        }

        public bool IsMoveEnable()
        {
            return (!m_tLocked.IsActive() || m_tLocked.IsTimeOut()) && (IsWalkEnable() || IsJumpEnable());
        }

        public bool KeepDistance(int x0, int y0, int x1, int y1, int nDist)
        {
            if (nDist == 0) return true;

            return Distance(x0, y0, x1, y1) < nDist;
        }

        public double Distance(int x0, int y0, int x1, int y1)
        {
            return Calculations.GetDistance((ushort)x0, (ushort)y0, (ushort)x1, (ushort)y1);
        }

        public bool CalcFarthestPos(ref Point pPos, ref Point pos, ref Point posTarget, int nRange)
        {
            if (nRange <= 0) return false;
            if (pos.X == posTarget.X && pos.Y == posTarget.Y) return false;

            if (Distance(pos.X, pos.Y, posTarget.X, posTarget.Y) <= nRange)
            {
                pPos = posTarget;
                return true;
            }

            int nDeltaX = posTarget.X - pos.X;
            int nDeltaY = posTarget.Y - pos.Y;

            if (Math.Abs(nDeltaX) > Math.Abs(nDeltaY))
            {
                pPos.X = pos.X + nRange * ((nDeltaX > 0) ? 1 : -1);
                pPos.Y = pos.Y + Calculations.MulDiv(nRange, nDeltaY, Math.Abs(nDeltaX));
            }
            else
            {
                pPos.X = pos.X + Calculations.MulDiv(nRange, nDeltaX, Math.Abs(nDeltaY));
                pPos.Y = pos.Y + nRange * ((nDeltaY > 0) ? 1 : -1);
            }

            return true;
        }

        public bool Move(ref Point pos, Point target)
        {
            return Move(ref pos,
                Calculations.GetDirectionSector((ushort)pos.X, (ushort)pos.Y, (ushort)target.X, (ushort)target.Y));
        }

        public bool Move(ref Point pos, int nDir)
        {
            if (nDir < 0) return false;
            if (nDir > 7) nDir = nDir % 8;

            pos.X += Handlers.WALK_X_COORDS[nDir];
            pos.Y += Handlers.WALK_Y_COORDS[nDir];

            MapX = (ushort)pos.X;
            MapY = (ushort)pos.Y;

            Map.SendToRange(new MsgWalk((uint)nDir, Identity, MovementType.WALK, MapIdentity), MapX, MapY);

            return true;
        }

        #endregion

        #region General Checks

        public bool IsMagicAtk()
        {
            return m_pMagics.Magics.Count > 0 || m_pDefaultMagic != null;
        }

        #endregion

        public void SendLeaveFromBlock()
        {
            Map.Send(new MsgAction(Identity, 0, MapX, MapY, GeneralActionType.REMOVE_ENTITY));
        }

        public void Send(byte[] pMsg)
        {

        }

        public const int MODE_DORMANCY = 0,
            MODE_IDLE = 1,
            MODE_WALK = 2,
            MODE_GOTO = 3,
            MODE_TALK = 4,
            MODE_FOLLOW = 5,
            MODE_ESCAPE = 6,
            MODE_FORWARD = 7,
            MODE_ATTACK = 8,
            WALKMODE_MOVE = 0,
            WALKMODE_RUN = 1,
            WALKMODE_SHIFT = 2,
            WALKMODE_JUMP = 3,
            WALKMODE_RUN_DIR0 = 20,
            WALKMODE_RUN_DIR7 = 27;
    }
}