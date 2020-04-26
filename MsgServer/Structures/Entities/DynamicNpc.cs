// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Dynamic Npc.cs
// Last Edit: 2016/12/05 11:04
// Created: 2016/12/05 11:00

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Core.Common.Enums;
using DB.Entities;
using MsgServer.Network;
using MsgServer.Structures.Actions;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Society;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Entities
{
    public sealed class DynamicNpc : IRole, IScreenObject, INpc
    {
        private const int _OWNER_SYN = 2, _OWNER_USER = 1, _OWNER_NONE = 0;

        private DbDynamicNPC m_dbNpc;
        private MsgNpcInfoEx m_pPacket;
        private short m_sElevation;
        private uint m_dwMapId;
        private Character m_pRole = null;
        private TimeOutMS m_tDeathTime;
        private Map m_pMap;
        private GameAction m_pGameAction;
        private BattleSystem m_pBattleSystem;

        // public members
        public ConcurrentDictionary<uint, SynScore> Scores;

        public DynamicNpc(DbDynamicNPC dbNpc)
        {
            m_dbNpc = dbNpc;
            m_pPacket = new MsgNpcInfoEx
            {
                Flag = dbNpc.Type,
                Identity = dbNpc.Id,
                Lookface = dbNpc.Lookface,
                Life = dbNpc.Life,
                MaxLife = dbNpc.Maxlife,
                MapX = dbNpc.Cellx,
                MapY = dbNpc.Celly,
                Type = dbNpc.Type
            };

            try
            {
                m_sElevation = Map[MapX, MapY].Elevation;
            }
            catch
            {
                m_sElevation = 999;
            }

            if (OwnerType == _OWNER_USER && OwnerIdentity > 0)
            {
                Client temp = null;
                if (ServerKernel.Players.TryGetValue(OwnerIdentity, out temp))
                    if (temp.Character != null)
                    {
                        m_pRole = temp.Character;
                        m_pPacket.Name = temp.Character.Name;
                    }
            }

            if (OwnerType == _OWNER_SYN && OwnerIdentity > 0 && IsSynFlag())
            {
                Syndicate temp = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Identity == OwnerIdentity);
                if (temp != null)
                    m_pPacket.Name = temp.Name;
            }

            Scores = new ConcurrentDictionary<uint, SynScore>();
            m_tDeathTime = new TimeOutMS(500);
            m_tDeathTime.Startup(500);
        }

        public uint Identity
        {
            get { return m_dbNpc.Id; }
        }

        public uint MapIdentity
        {
            get { return m_dbNpc.Mapid; }
            set { m_dbNpc.Mapid = value; }
        }

        public string Name
        {
            get { return m_pPacket.Name; }
            set
            {
                m_pPacket.Name = value.Substring(0, value.Length > 16 ? 16 : value.Length);
            }
        }

        public ushort MapX
        {
            get { return m_dbNpc.Cellx; }
            set
            {
                m_dbNpc.Cellx = value;
                m_pPacket.MapX = value;
            }
        }

        public ushort MapY
        {
            get { return m_dbNpc.Celly; }
            set
            {
                m_dbNpc.Celly = value;
                m_pPacket.MapY = value;
            }
        }

        public short Elevation { get { return m_sElevation; } }

        public Map Map
        {
            get
            {
                if (m_pMap == null && !ServerKernel.Maps.TryGetValue(MapIdentity, out m_pMap))
                    return null;
                return m_pMap;
            }
            set { m_pMap = value; }
        }

        public IScreenObject FindAroundRole(uint idRole)
        {
            return Map.FindAroundRole(this, idRole);
        }

        public void SendSpawnTo(Character pUser)
        {
            pUser.Send(m_pPacket);
        }

        #region IRole

        public bool IsCallPet() { return false; }

        public Screen Screen { get { return null; } } // no screen for npcs :) much memory so wow
        public FacingDirection Direction { get; set; }
        public StatusSet Status { get { return null; } }  // dyna npcs don't need statuses (:
        public GameAction GameAction { get { return m_pGameAction ?? (m_pGameAction = new GameAction(this)); } }
        public StatusSet StatusSet { get { return null; } }

        public MagicData Magics { get { return null; } }
        public BattleSystem BattleSystem { get { return m_pBattleSystem ?? (m_pBattleSystem = new BattleSystem(this)); } }
        public byte Stamina { get; set; }
        public bool SpendEquipItem(uint useItem, uint useItemNum, bool bSynchro) { return true; }
        public bool DecEquipmentDurability(bool bAttack, int hitByMagic, ushort useItemNum) { return true; }
        public int GetDistance(ushort x, ushort y) { return (int)Calculations.GetDistance(MapX, MapY, x, y); }
        public bool CheckWeaponSubType(uint idType, uint dwAmount) { return true; }
        public bool ProcessMagicAttack(ushort usMagicType, uint idTarget, ushort x, ushort y, byte ucAutoActive = 0) { return true; }

        public int MagicDefenseBonus  { get { return 0; } }

        public float GetReduceDamage()
        {
            return 0;
        }

        public bool IsEvil()
        {
            return false;
        }

        public uint Lookface
        {
            get { return m_dbNpc.Lookface; }
            set
            {
                m_dbNpc.Lookface = (ushort)value;
                m_pPacket.Lookface = (ushort)value;
            }
        }

        public byte Level { get { return (byte) m_dbNpc.Data3; } set { m_dbNpc.Data3 = value; } }
        public int BattlePower { get { return 0; } }

        public int MinAttack { get { return 0; } set { } }
        public int MaxAttack { get { return 0; } set { } }
        public int MagicAttack { get { return 0; } set { } }
        public int Dodge { get { return 0; } set { } }
        public int AttackHitRate { get { return 0; } set { } }
        public int Dexterity { get { return 0; } set { } }

        public int Defense
        {
            get { return m_dbNpc.Defence; }
            set
            {
                m_dbNpc.Defence = (ushort)value;
            }
        }

        public int MagicDefense
        {
            get { return m_dbNpc.MagicDef; }
            set
            {
                m_dbNpc.MagicDef = (ushort)value;
            }
        }

        public bool IsCtfFlag()
        {
            return m_dbNpc.Type == NpcTypes.ROLE_CTFBASE_NPC;
        }

        public bool SynchroPosition(int x, int y, int nDistance = 8)
        {
            return true;
        }

        public int AddFinalAttack { get { return 0; } set { } }
        public int AddFinalMagicAttack { get { return 0; } set { } }
        public int AddFinalDefense { get { return 0; } set { } }
        public int AddFinalMagicDefense { get { return 0; } set { } }

        public uint Life
        {
            get { return m_dbNpc.Life; }
            set
            {
                if (value > MaxLife) MaxLife = value;
                m_dbNpc.Life = value;
                m_pPacket.Life = value;
                Save();
            }
        }

        public uint MaxLife
        {
            get { return m_dbNpc.Maxlife; }
            set
            {
                if (value < Life) Life = value;
                m_dbNpc.Maxlife = value;
                m_pPacket.MaxLife = value;
            }
        }

        public ushort Mana { get { return 0; } set { } }
        public ushort MaxMana { get { return 0; } set { } }

        public ulong Flag1 { get; set; }
        public ulong Flag2 { get; set; }

        public bool IsWing() { return false; }
        public bool IsBeAttackable() { return true; }
        public bool IsBowman() { return false; }
        public bool IsSimpleMagicAtk() { return false; }

        public bool IsPlayer() { return false; }
        public bool IsMonster() { return false; }
        public bool IsNpc() { return false; }
        public bool IsDynaNpc() { return true; }
        public bool IsDynaMonster() { return false; }
        public bool IsAlive { get { return Life > 0; } }
        public ushort Profession { get { return 0; } set { } }

        public int AdjustDefense(int nRawDef)
        {
            return nRawDef;
        }

        public int AdjustMagicDamage(int nDamage)
        {
            return nDamage;
        }

        public int AdjustWeaponDamage(int nDamage)
        {
            return nDamage;
        }

        public int GetAttackRange(int nTargetSizeAdd)
        {
            return Math.Max(1, nTargetSizeAdd);
        }

        public int GetDistance(IScreenObject pObj)
        {
            return (int)Calculations.GetDistance(MapX, MapY, pObj.MapX, pObj.MapY);
        }

        public int Attack(IRole pTarget, ref InteractionEffect pEffects)
        {
            return 0;
        }

        public int AdjustExperience(IRole pTarget, int nRawExp, bool bNewbieBonusMsg)
        {
            return 0;
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

        public void Send(byte[] pMsg)
        {
            // wtf? y server is sending a packet to a npc?
        }

        public bool BeAttack(int bMagic, IRole pRole, int nPower, bool bReflectEnable)
        {
            AddAttribute(ClientUpdateType.HITPOINTS, nPower * -1, true);
            if (IsDynaNpc())
            {
                if (IsSynNpc())
                {
                    var pUser = pRole as Character;
                    if (pUser != null && pUser.SyndicateIdentity > 0)
                    {
                        if (Map.IsWarTime() && OwnerIdentity != pUser.SyndicateIdentity)
                        {
                            pUser.SyndicateMember.IncreaseMoney((uint) Math.Max(0, nPower));
                            // todo
                        }
                    }
                } 
                else if (IsCtfFlag())
                {
                    var pUser = pRole as Character;
                    if (pUser != null && pUser.SyndicateIdentity > 0)
                    {
                        if (ServerKernel.SyndicateScoreWar.IsRunning)
                        {
                            ServerKernel.SyndicateScoreWar.AwardPoints(pUser, (uint) nPower);
                        }
                    }
                }
            }

            if (!IsAlive)
                BeKill(pRole);

            return true;
        }

        public void BeKill(IRole pRole)
        {
            // TODO

            if (IsDieAction())
            {
                GameAction.ProcessAction(m_dbNpc.Linkid, pRole as Character, this, null, null);
            }
            else
            {
                // TODO del npc
            }
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
                        MsgUserAttrib pMsg = new MsgUserAttrib
                        {
                            Identity = Identity
                        };
                        pMsg.Append(ClientUpdateType.HITPOINTS, Life);
                        m_pMap.SendToRange(pMsg, MapX, MapY);
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

        public bool DetachWellStatus(IRole pRole) { return true; }
        public bool DetachBadlyStatus(IRole pRole) { return true; }
        public bool DetachAllStatus(IRole pRole) { return true; }
        public bool IsWellStatus0(ulong nStatus) { return true; }
        public bool IsBadlyStatus0(ulong nStatus) { return true; }
        public bool IsWellStatus1(ulong nStatus) { return true; }
        public bool IsBadlyStatus1(ulong nStatus) { return true; }
        public IStatus QueryStatus(Effect0 flag) { return null; }
        public IStatus QueryStatus(Effect1 flag) { return null; }
        public IStatus QueryStatus(int nType) { return null; }
        public bool AppendStatus(StatusInfoStruct pInfo) { return true; }
        public bool AttachStatus(IRole pRole, int nStatus, int nPower, int nSecs, int nTimes, byte pLevel, uint wCaster = 0) { return pRole.Map != null; }
        public bool DetachStatus(int nType) { return true; }
        public bool DetachStatus(Effect0 nType) { return true; }
        public bool DetachStatus(Effect1 nType) { return true; }
        public bool DetachStatus(ulong nType, bool b64) { return true; }

        public bool IsInFan(Point pos, Point posSource, int nRange, int nWidth, Point posCenter)
        {
            return Calculations.IsInFan(pos, posSource, nRange, nWidth, posCenter);
        }

        public bool IsAttackable(IRole pTarget)
        {
            if (!IsSynFlag() && Sort != 21 && Sort != 22 && Sort != 26 && Sort != 17 && !IsCtfFlag())
                return false;

            if (Data1 != 0 && Data2 != 0)
            {
                var strNow = "";
                var now = DateTime.Now;
                strNow += (((int)now.DayOfWeek) == 0 ? 7 : (int)now.DayOfWeek).ToString(CultureInfo.InvariantCulture);
                strNow += now.Hour.ToString("00");
                strNow += now.Minute.ToString("00");
                strNow += now.Second.ToString("00");

                var now0 = int.Parse(strNow);
                if (Data1 >= 1000000)
                {
                    if ((now0 < Data1 || now0 >= Data2) && IsSynFlag())
                        return false;
                }
                else
                {
                    int nowHour = now0%1000000;
                    if (nowHour < Data1 || nowHour >= Data2 && (IsSynFlag() || IsCtfFlag()))
                        return false;
                }

                if ((!IsSynFlag() && !IsCtfFlag()) && Sort != 21) // not syn flag neither gate
                    return false;

                if ((IsSynFlag() || IsCtfFlag()) && pTarget is Character)
                {
                    Character pRole = pTarget as Character;
                    if (pRole.Syndicate != null && pRole.Syndicate.Identity == OwnerIdentity) return false;
                }

                if (!IsAlive && m_pMap.Identity == 7600)
                    return false;
            }

            Character pRoleUser = pTarget as Character;
            if (pRoleUser != null && pRoleUser.SyndicateIdentity > 0)
                if (OwnerType == 2 && pRoleUser.SyndicateIdentity == OwnerIdentity)
                    return false;

            if (!IsDynaNpc() && MaxLife <= 0)
                return false;

            return !IsActive();
        }

        public void Kill(IRole pTarget, ulong dwDieWay) { }
        public bool AdditionMagic(int nLifeLost, int nDamage) { return false; }
        public void SendDamageMsg(uint pTarget, uint nDamage, InteractionEffect special) { }
        public bool IsGoal() { return Kind == 21 || Kind == 22; }
        public int GetSizeAdd() { return 0; }

        public int Defense2 { get { return 10000; } }

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

        #endregion

        #region Dynamic Npc

        public bool IsSynMoneyEmpty()
        {
            if (_OWNER_SYN == OwnerType && OwnerIdentity > 0)
            {
                Syndicate temp;
                return ServerKernel.Syndicates.TryGetValue(OwnerIdentity, out temp) && temp.SilverDonation <= 0;
            }
            return false;
        }

        public bool IsDeleted() { return m_tDeathTime.IsActive(); }

        public bool IsActive() { return !m_tDeathTime.IsActive(); }

        public bool IsSceneNpc() { return (Kind & 4) != 0; }

        public void RemoveFromMap()
        {
            if (Identity == 0) return;

            foreach (var user in ServerKernel.Maps[MapIdentity].Players.Values)
                if (Calculations.InScreen(MapX, MapY, user.MapX, user.MapY))
                {
                    user.Screen.Delete(Identity);
                    // .-. something wrong is not right xD
                }
            IScreenObject trash;
            Map.GameObjects.TryRemove(Identity, out trash);
        }

        public bool ChangePos(uint idMap, ushort nPosX, ushort nPosY)
        {
            Map map;
            if (idMap > 0 && ServerKernel.Maps.TryGetValue(idMap, out map))
            {
                RemoveFromMap();
                MapX = nPosX;
                MapY = nPosY;
                MapIdentity = idMap;
                map.AddDynaNpc(this);
                return true;
            }
            return false;
        }

        public bool IsUserNpc()
        {
            return OwnerType == 1 && OwnerIdentity != 0;
        }

        public bool IsSynNpc()
        {
            return OwnerType == 2 && OwnerIdentity != 0;
        }

        public bool IsFamilyNpc()
        {
            return OwnerType == 3 && OwnerIdentity != 0;
        }

        public bool IsOwnerOf(Character pUser, bool bMateEnable)
        {
            if (pUser == null) return false;

            if (IsSynNpc() && OwnerIdentity == pUser.SyndicateIdentity && pUser.SyndicateRank == SyndicateRank.GUILD_LEADER)
                return true;

            if (IsUserNpc() && OwnerIdentity == pUser.Identity) return true;

            // todo if (bMateEnable && IsUserNpc() && OwnerIdentity)

            return false;
        }

        public bool CheckSortMutex()
        {
            var dwSort = (uint)(Sort & 88);

            if (Kind == 15 && dwSort != 0)
                return false;

            for (int i = 0; i < 32; i++)
            {
                if (dwSort == 0)
                    return true;

                if ((dwSort & 1) != 0 && (dwSort >>= 1) != 0)
                {
                    ServerKernel.Log.SaveLog("_TBL_NPC(_TBL_DYNANPC) link id mutex!!!", false, LogType.WARNING);
                    return false;
                }
            }
            return true;
        }

        public bool IsSynFlag() { return m_dbNpc.Type == NpcTypes.SYNFLAG_NPC && m_dbNpc.Ownertype == _OWNER_SYN; }

        public void CheckFightTime()
        {
            if (!IsSynFlag())
                return;

            if (Data1 == 0 || Data2 == 0)
                return;

            string strNow = "";
            DateTime now = DateTime.Now;
            strNow += ((int)now.DayOfWeek == 0 ? 7 : (int)now.DayOfWeek).ToString(CultureInfo.InvariantCulture);
            strNow += now.Hour.ToString("00");
            strNow += now.Minute.ToString("00");
            strNow += now.Second.ToString("00");

            int now0 = int.Parse(strNow);
            if (now0 < Data1 || now0 >= Data2)
            {
                if (Map.IsWarTime())
                    OnFightEnd();
                return;
            }

            if (!Map.IsWarTime())
            {
                Map.SetStatus(1, true);

                Map.SendMessageToMap(ServerString.STR_SYN_WAR_START, ChatTone.SYSTEM);
            }
        }

        public void OnFightEnd()
        {
            Map.SetStatus(1, false);
            Map.SendMessageToMap(ServerString.STR_SYN_WAR_END, ChatTone.SYSTEM);

            foreach (var usr in Map.Players.Values.Where(x => x.BattleSystem != null))
            {
                usr.SetAttackTarget(null);
                usr.BattleSystem.DestroyAutoAttack();
            }
        }

        public bool DelNpc()
        {
            SetAttribute(ClientUpdateType.HITPOINTS, 0, true);
            m_tDeathTime.Update();

            if (IsSynFlag())
            {
                Map.SetStatus(1, false);
                //Map.SetSynId(0, true);
                // todo
            }
            else
            {
                Database.DynamicNpcRepository.Delete(m_dbNpc);
            }
            RemoveFromMap();
            return true;
        }

        public bool SetOwnerIdentity(uint identity)
        {
            if (identity == 0)
            {
                OwnerIdentity = identity;
                return true;
            }
            Syndicate owner;// = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Identity == identity && !x.DelFlag);
            if (!ServerKernel.Syndicates.TryGetValue(identity, out owner))
                return false;

            OwnerIdentity = owner.Identity;
            if (IsSynNpc() && (IsSynFlag()  || IsCtfFlag()))
            {
                Name = owner.Name;
                m_pPacket.Name = owner.Name;
            }
            Save();
            return true;
        }

        public bool SetOwnerIdentity(Client owner)
        {
            if (IsSynFlag() && owner != null)
            {
                if (owner.Character.SyndicateIdentity > 0)
                    return true;
                OwnerIdentity = owner.Character.SyndicateIdentity;
                m_pRole = owner.Character;
                Name = m_pRole.Name;
                Save();
                return true;
            }
            return false;
        }

        public bool SetAttribute(string attr, long data)
        {
            switch (attr)
            {
                case "task0":
                    m_dbNpc.Task0 = (uint)data;
                    break;
                case "task1":
                    m_dbNpc.Task1 = (uint)data;
                    break;
                case "task2":
                    m_dbNpc.Task2 = (uint)data;
                    break;
                case "task3":
                    m_dbNpc.Task3 = (uint)data;
                    break;
                case "task4":
                    m_dbNpc.Task4 = (uint)data;
                    break;
                case "task5":
                    m_dbNpc.Task5 = (uint)data;
                    break;
                case "task6":
                    m_dbNpc.Task6 = (uint)data;
                    break;
                case "task7":
                    m_dbNpc.Task7 = (uint)data;
                    break;
                case "data0":
                    m_dbNpc.Data0 = (int)data;
                    break;
                case "data1":
                    m_dbNpc.Data1 = (int)data;
                    break;
                case "data2":
                    m_dbNpc.Data2 = (int)data;
                    break;
                case "data3":
                    m_dbNpc.Data3 = (int)data;
                    break;
                case "ownerid":
                    m_dbNpc.Ownerid = (uint)data;
                    break;
                case "lookface":
                    m_dbNpc.Lookface = (ushort)data;
                    Map.SendToRange(m_pPacket, MapX, MapY);
                    break;
                default:
                    return false;
            }
            Save();
            return true;
        }

        public bool SetAttribute(ClientUpdateType attr, long data, bool synchro)
        {
            switch (attr)
            {
                case ClientUpdateType.HITPOINTS:
                    {
                        if (data > MaxLife)
                            Life = MaxLife;
                        else
                            Life = (uint)data;
                        if (synchro)
                            Map.SendToRange(m_pPacket, MapX, MapY);
                        return true;
                    }
                case ClientUpdateType.MAX_HITPOINTS:
                    {
                        if (data <= 0)
                            return false;
                        if (data < Life)
                            MaxLife = Life;
                        else
                            MaxLife = (uint)data;
                        if (synchro)
                            Map.SendToRange(m_pPacket, MapX, MapY);
                        return true;
                    }
                case ClientUpdateType.MESH:
                    {
                        if (data <= 0)
                            return false;
                        if (data != Lookface)
                        {
                            var msg = new MsgUserAttrib
                            {
                                Identity = Identity
                            };
                            msg.Append(ClientUpdateType.MESH, (ushort)data);
                            Map.SendToRange(msg, MapX, MapY);
                        }
                        Lookface = (ushort)data;
                        return true;
                    }
            }
            return false;
        }

        public int GetData(string idx)
        {
            switch (idx)
            {
                case "data0":
                    return m_dbNpc.Data0;
                case "data1":
                    return m_dbNpc.Data1;
                case "data2":
                    return m_dbNpc.Data2;
                case "data3":
                    return m_dbNpc.Data3;
            }
            return 0;
        }

        public int Attack(IRole pTarget)//, ref InteractionEffect special)
        {
            return 0;
        }

        public bool IsInteractive() { return m_dbNpc.Task0 != 0; }

        public bool IsDieAction() { return m_dbNpc.Linkid != 0; }

        public bool IsAwardScore() { return m_dbNpc.Type == NpcTypes.SYNFLAG_NPC || m_dbNpc.Type == NpcTypes.ROLE_CTFBASE_NPC; }

        public int GetMaxFixMoney()
        {
            return (int)Calculations.CutRange(Calculations.MulDiv(MaxLife - 1, 1, 1) + 1, 0, 2000000000);
        }

        public int GetLostFixMoney()
        {
            int nLostLifeTmp = (int)(MaxLife - Life);
            return Calculations.CutRange(Calculations.MulDiv(nLostLifeTmp - 1, 1, 1) + 1, 0, 2000000000);
        }

        public void SendOwnerRanking()
        {
            int i = 0;

            if (!IsAwardScore() || !IsAttackable(null))
                return;

            foreach (var client in ServerKernel.Maps[MapIdentity].Players.Values)
            {
                if (IsAttackable(null))
                {
                    var nChannel = ChatTone.EVENT_RANKING;
                    foreach (var syn in Scores.Values.OrderByDescending(x => x.Score))
                        if (i++ < 5)
                        {
                            client.Send(string.Format("Nº{0}: {1} - {2}", i, syn.Name, syn.Score),
                                nChannel);
                            nChannel = ChatTone.EVENT_RANKING_NEXT;
                        }
                }
                i = 0;
            }
        }

        public void SendToRange()
        {
            Map.SendToRange(m_pPacket, MapX, MapY);
        }

        #endregion

        #region Npc Task and Data
        public uint OwnerIdentity
        {
            get { return m_dbNpc.Ownerid; }
            set
            {
                m_dbNpc.Ownerid = value;
            }
        }

        public byte OwnerType
        {
            get { return (byte)m_dbNpc.Ownertype; }
            set
            {
                m_dbNpc.Ownertype = value;
            }
        }

        public ushort Kind
        {
            get { return m_dbNpc.Type; }
            set
            {
                m_dbNpc.Type = value;
                m_pPacket.Flag = value;
            }
        }

        public ushort Sort
        {
            get { return m_dbNpc.Sort; }
            set
            {
                m_dbNpc.Sort = value;
            }
        }

        public bool Vending { get; set; }
        public uint Task0 { get { return m_dbNpc.Task0; } set { m_dbNpc.Task0 = value; Save(); } } 
        public uint Task1 { get { return m_dbNpc.Task1; } set { m_dbNpc.Task1 = value; Save(); } } 
        public uint Task2 { get { return m_dbNpc.Task2; } set { m_dbNpc.Task2 = value; Save(); } } 
        public uint Task3 { get { return m_dbNpc.Task3; } set { m_dbNpc.Task3 = value; Save(); } } 
        public uint Task4 { get { return m_dbNpc.Task4; } set { m_dbNpc.Task4 = value; Save(); } } 
        public uint Task5 { get { return m_dbNpc.Task5; } set { m_dbNpc.Task5 = value; Save(); } } 
        public uint Task6 { get { return m_dbNpc.Task6; } set { m_dbNpc.Task6 = value; Save(); } } 
        public uint Task7 { get { return m_dbNpc.Task7; } set { m_dbNpc.Task7 = value; Save(); } } 
        public int Data0 { get { return m_dbNpc.Data0; } set { m_dbNpc.Data0 = value; Save(); } }
        public int Data1 { get { return m_dbNpc.Data1; } set { m_dbNpc.Data1 = value; Save(); } }
        public int Data2 { get { return m_dbNpc.Data2; } set { m_dbNpc.Data2 = value; Save(); } }
        public int Data3 { get { return m_dbNpc.Data3; } set { m_dbNpc.Data3 = value; Save(); } }
        #endregion

        #region Database
        public bool Save()
        {
            return Database.DynamicNpcRepository.SaveOrUpdate(m_dbNpc);
        }

        public bool Delete()
        {
            return Database.DynamicNpcRepository.Delete(m_dbNpc);
        }
        #endregion
    }
}