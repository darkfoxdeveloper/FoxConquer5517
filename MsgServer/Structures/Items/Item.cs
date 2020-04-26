// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Item.cs
// Last Edit: 2017/01/26 19:40
// Created: 2016/12/29 21:31

using System;
using System.Linq;
using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Items
{
    public sealed class Item
    {
        private readonly int[] ARTIFACT_STABILIZATION_POINTS = {0, 10, 30, 50, 100, 150, 200, 300};
        private readonly int[] REFINERY_STABILIZATION_POINTS = {0, 10, 30, 70, 150, 270, 500};

        private readonly uint[] TALISMAN_SOCKET_QUALITY_ADDITION = {0, 0, 0, 0, 0, 0, 5, 10, 40, 1000};

        private readonly uint[] TALISMAN_SOCKET_PLUS_ADDITION =
        {
            0, 6, 30, 80, 240, 740, 2220, 6660, 20000, 60000, 62000,
            64000, 72000
        };

        private readonly uint[] TALISMAN_SOCKET_HOLE_ADDITION0 = {0, 160, 800};
        private readonly uint[] TALISMAN_SOCKET_HOLE_ADDITION1 = {0, 2000, 6000};
        
        private const int _MAX_UPGRADE_CHECK = 10;
        // Single hand weapon define 400000
        public const int SWEAPON_NONE = 00000;
        public const int SWEAPON_BLADE = 10000;
        public const int SWEAPON_SWORD = 20000;
        public const int SWEAPON_BACKSWORD = 21000;
        public const int SWEAPON_HOOK = 30000;
        public const int SWEAPON_WHIP = 40000;
        public const int SWEAPON_AXE = 50000;
        public const int SWEAPON_HAMMER = 60000;
        public const int SWEAPON_CLUB = 80000;
        public const int SWEAPON_SCEPTER = 81000;
        public const int SWEAPON_DAGGER = 90000;
        // Double hand weapon define 500000
        public const int DWEAPON_BOW = 00000;
        public const int DWEAPON_GLAVE = 10000;
        public const int DWEAPON_POLEAXE = 30000;
        public const int DWEAPON_LONGHAMMER = 40000;
        public const int DWEAPON_SPEAR = 60000;
        public const int DWEAPON_WANT = 61000;
        public const int DWEAPON_HALBERT = 80000;
        // Other weapon define 600000
        public const int DWEAPON_KATANA = 01000;
        public const int DWEAPON_PRAYER_BEADS = 10000;

        private DbItem m_dbItem;
        private DbItemtype m_dbItemtype;
        private Character m_pOwner;
        private MsgItemInformation m_pMsgInfo;
        private Carry m_pCarry;
        public IRefinery Refinery;
        public IArtifact Artifact;

        public Item()
        {
            GenerateDefault();
        }

        public Item(Character pRole, DbItem item)
        {
            m_pOwner = pRole;
            m_dbItem = item;

            if (!ServerKernel.Itemtype.TryGetValue(item.Type, out m_dbItemtype))
            {
                ServerKernel.Log.SaveLog("ITEM(DbItem,Character)::Could not load item type:" + item.Type, true, LogType.WARNING);
                return;
            }

            if (m_pOwner != null)
            {
                LoadArtifact();
                LoadRefinery();
            }

            GeneratePacket(item);
        }

        public Item(Character owner)
        {
            m_pOwner = owner;
            GenerateDefault();
            GenerateDefaultPacket();
        }

        public Item(uint dwType, byte plus, byte redDmg, ushort nDura, ushort nMaxDura, byte monopoly = 0)
        {
            if (!ServerKernel.Itemtype.TryGetValue(dwType, out m_dbItemtype))
            {
                ServerKernel.Log.SaveLog("ITEM(uint,byte,byte,ushort,ushort,byte)::Could not load item type:" + dwType, true, LogType.WARNING);
                return;
            }

            m_pOwner = null;
            GenerateDefault();
            GenerateDefaultPacket();
        }

        public uint Identity
        {
            get { return m_dbItem.Id; }
        }

        public uint Type
        {
            get { return m_dbItem.Type; }
            set
            {
                if (ServerKernel.Itemtype.TryGetValue(value, out m_dbItemtype))
                {
                    m_dbItem.Type = value;
                    m_pMsgInfo.Itemtype = value;
                    m_dbItem.Amount = m_dbItemtype.AmountLimit;
                    m_dbItem.AmountLimit = m_dbItemtype.AmountLimit;
                    m_pMsgInfo.MaximumDurability = m_dbItemtype.AmountLimit;
                    Save();
                }
            }
        }

        public DbItemtype Itemtype
        {
            get { return m_dbItemtype; }
        }

        public uint OwnerIdentity
        {
            get { return m_dbItem.OwnerId; }
            set
            {
                m_dbItem.OwnerId = value;
                //Save();
            }
        }

        public uint PlayerIdentity
        {
            get { return m_dbItem.PlayerId; }
            set
            {
                m_dbItem.PlayerId = value;
                //Save();
            }
        }

        public ushort Durability
        {
            get { return m_dbItem.Amount; }
            set
            {
                m_dbItem.Amount = value;
                m_pMsgInfo.Durability = value;
                //Save();
            }
        }

        public ushort MaximumDurability
        {
            get { return m_dbItem.AmountLimit; }
            set
            {
                m_dbItem.AmountLimit = value;
                m_pMsgInfo.MaximumDurability = value;
                //Save();
            }
        }

        public ItemPosition Position
        {
            get { return (ItemPosition)m_dbItem.Position; }
            set
            {
                m_dbItem.Position = (byte)value;
                m_pMsgInfo.Position = value;
                //Save();
            }
        }

        public SocketGem SocketOne
        {
            get { return (SocketGem)m_dbItem.Gem1; }
            set
            {
                m_dbItem.Gem1 = (byte)value;
                m_pMsgInfo.SocketOne = value;
                //Save();
            }
        }

        public SocketGem SocketTwo
        {
            get { return (SocketGem)m_dbItem.Gem2; }
            set
            {
                m_dbItem.Gem2 = (byte)value;
                m_pMsgInfo.SocketTwo = value;
                //Save();
            }
        }

        public ItemEffect Effect
        {
            get { return (ItemEffect)m_dbItem.Magic1; }
            set
            {
                m_dbItem.Magic1 = (byte)value;
                m_pMsgInfo.Effect = value;
                //Save();
            }
        }

        public byte Plus
        {
            get { return m_dbItem.Magic3; }
            set
            {
                m_dbItem.Magic3 = value;
                m_pMsgInfo.Plus = value;
                //Save();
            }
        }

        public byte ReduceDamage
        {
            get { return m_dbItem.ReduceDmg; }
            set
            {
                m_dbItem.ReduceDmg = value;
                m_pMsgInfo.Bless = value;
                //Save();
            }
        }

        public byte Enchantment
        {
            get { return m_dbItem.AddLife; }
            set
            {
                m_dbItem.AddLife = value;
                m_pMsgInfo.Enchantment = value;
                //Save();
            }
        }

        public uint SocketProgress
        {
            get { return m_dbItem.Data; }
            set
            {
                m_dbItem.Data = value;
                m_pMsgInfo.SocketProgress = value;
                //Save();
            }
        }

        public uint LockTime
        {
            get { return m_dbItem.Plunder; }
            set
            {
                m_dbItem.Plunder = value;
                m_pMsgInfo.Locked = value > 0;
                //Save();
            }
        }

        public ItemColor Color
        {
            get { return (ItemColor)m_dbItem.Color; }
            set
            {
                m_dbItem.Color = (byte)value;
                m_pMsgInfo.Color = value;
                //Save();
            }
        }

        public uint CompositionProgress
        {
            get { return m_dbItem.AddlevelExp; }
            set
            {
                m_dbItem.AddlevelExp = value;
                m_pMsgInfo.Composition = value;
                //Save();
            }
        }

        public bool Bound
        {
            get { return m_dbItem.Monopoly == 3; }
            set
            {
                m_dbItem.Monopoly = (byte)(value ? 3 : 0);
                m_pMsgInfo.Bound = value;
                //Save();
            }
        }

        public byte Monopoly
        {
            get { return m_dbItem.Monopoly; }
            set
            {
                m_dbItem.Monopoly = value;
                //Save();
            }
        }

        public bool Inscribed
        {
            get { return m_dbItem.Inscribed > 0; }
            set
            {
                m_dbItem.Inscribed = (byte)(value ? 1 : 0);
                m_pMsgInfo.Inscribed = value;
                //Save();
            }
        }

        public uint ArtifactType
        {
            get { return m_dbItem.ArtifactType; }
            set
            {
                m_dbItem.ArtifactType = value;
                Artifact.ArtifactType = value;
                //Save();
            }
        }

        public uint ArtifactStart
        {
            get { return m_dbItem.ArtifactStart; }
            set
            {
                m_dbItem.ArtifactStart = value;
                Artifact.ArtifactStartTime = value;
                //Save();
            }
        }

        public uint ArtifactExpire
        {
            get { return m_dbItem.ArtifactExpire; }
            set
            {
                m_dbItem.ArtifactExpire = value;
                Artifact.ArtifactExpireTime = value;
                //Save();
            }
        }

        public uint ArtifactStabilization
        {
            get { return m_dbItem.ArtifactStabilization; }
            set
            {
                m_dbItem.ArtifactStabilization = value;
                //Save();
            }
        }

        public uint RefineryType
        {
            get { return m_dbItem.RefineryType; }
            set
            {
                m_dbItem.RefineryType = value;
                Refinery.RefineryType = value;
                //Save();
            }
        }

        public byte RefineryLevel
        {
            get { return m_dbItem.RefineryLevel; }
            set
            {
                m_dbItem.RefineryLevel = value;
                Refinery.RefineryLevel = value;
                //Save();
            }
        }

        public uint RefineryStart
        {
            get { return m_dbItem.RefineryStart; }
            set
            {
                m_dbItem.RefineryStart = value;
                Refinery.RefineryStartTime = value;
                //Save();
            }
        }

        public uint RefineryExpire
        {
            get { return m_dbItem.RefineryExpire; }
            set
            {
                m_dbItem.RefineryExpire = value;
                Refinery.RefineryExpireTime = value;
                //Save();
            }
        }

        public uint RefineryStabilization
        {
            get { return m_dbItem.RefineryStabilization; }
            set
            {
                m_dbItem.RefineryStabilization = value;
                //Save();
            }
        }

        public ushort StackAmount
        {
            get { return m_dbItem.StackAmount; }
            set
            {
                m_dbItem.StackAmount = value;
                m_pMsgInfo.PackageAmount = value;
                //Save();
            }
        }

        public uint RemainingTime
        {
            get { return m_dbItem.RemainingTime; }
            set
            {
                m_dbItem.RemainingTime = value;
                //Save();
            }
        }

        public uint HonorPrice { get; set; }

        #region Refinery and Artifacts

        public uint ArtifactRemainingPoints()
        {
            if (ArtifactIsPermanent())
                return 0;
            return (uint)(ARTIFACT_STABILIZATION_POINTS[Artifact.ArtifactLevel] - ArtifactStabilization);
        }

        public bool ArtifactIsPermanent()
        {
            return ArtifactType <= 0 || (ArtifactStabilization >= ARTIFACT_STABILIZATION_POINTS[Artifact.ArtifactLevel]);
        }

        public void LoadArtifact()
        {
            if (ArtifactType == 0)
                return;

            DbItemtype temp;
            if (!ServerKernel.Itemtype.TryGetValue(ArtifactType, out temp))
                return;

            Artifact = new IArtifact
            {
                Artifact = temp,
                ArtifactLevel = temp.Phase,
                ArtifactStartTime = ArtifactStart,
                ArtifactType = ArtifactType,
                Avaiable = true,
                ItemIdentity = Identity,
                StabilizationPoints = ArtifactStabilization,
                ArtifactExpireTime = ArtifactExpire
            };

            if (!ArtifactIsPermanent() && ArtifactExpire < UnixTimestamp.Timestamp())
                Artifact.Avaiable = false;
        }

        public uint RefineryRemainingPoints()
        {
            if (RefineryIsPermanent())
                return 0;
            return (uint)(REFINERY_STABILIZATION_POINTS[RefineryLevel] - RefineryStabilization);
        }

        public bool RefineryIsPermanent()
        {
            return RefineryType <= 0 || (RefineryStabilization >= REFINERY_STABILIZATION_POINTS[RefineryLevel]);
        }

        public void LoadRefinery()
        {
            if (RefineryType == 0 && !RefineryIsPermanent())
                return;

            DbRefinery refinery;
            if (!ServerKernel.Refineries.TryGetValue(RefineryType, out refinery))
            {
                if (RefineryType > 0)
                    ServerKernel.Log.SaveLog(
                        string.Format("Non existent refinery [{0}] requested by the client ", RefineryType), true,
                        "MissingRefinery", LogType.WARNING);
                return;
            }
            switch (refinery.Itemtype.ToString().Length)
            {
                // Position
                case 1:
                    {
                        if (Calculations.GetItemPosition(Type) == (ItemPosition)refinery.Itemtype)
                            break;
                        if ((Calculations.GetItemPosition(Type) == ItemPosition.RIGHT_HAND 
                            && refinery.Itemtype == 5 
                            && GetSort() == ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND))
                            break;
                        return;
                    }
                // Type
                case 2:
                    {
                        if (Type / 10000 == refinery.Itemtype)
                            break;
                        return;
                    }
                // Specific item
                case 3:
                    {
                        if (Type / 1000 == refinery.Itemtype)
                            break;
                        // Ring/HeavyRing Exception :)
                        if (refinery.Itemtype == 150
                            && Type / 1000 == 151)
                            break;
                        return;
                    }
                default:
                    return;
            }

            if (!Enum.IsDefined(typeof(RefineryType), refinery.Type))
                return;

            if (RefineryIsPermanent()
                || RefineryType > 0
                && RefineryExpire > UnixTimestamp.Timestamp())
            {
                Refinery = new IRefinery
                {
                    RefineryLevel = RefineryLevel,
                    RefineryExpireTime = RefineryExpire,
                    RefineryStartTime = RefineryStart,
                    RefineryType = RefineryType,
                    Avaiable = true,
                    ItemIdentity = Identity,
                    StabilizationPoints = RefineryStabilization,
                    RefineryPercent = refinery.Power,
                    Mode = (RefineryType)refinery.Type
                };
            }
            else
                ResetRefinery();
        }

        public void SendPurification()
        {
            if (m_pOwner == null)
                return;
            if (ArtifactType == 0 && RefineryType == 0)
                return;

            int now = UnixTimestamp.Timestamp();
            var artifact = new MsgItemStatus();

            #region Artifact (Dragon Soul)
            uint remainingArtifact = 0;
            if (ArtifactIsPermanent())
                ArtifactExpire = 0;
            else
                if (now < ArtifactExpire && !ArtifactIsPermanent())
                    remainingArtifact = (uint)(ArtifactExpire - now);

            if (Artifact.Avaiable && ArtifactIsPermanent())
                artifact.Append(Identity, 8, Artifact.ArtifactType, Artifact.Artifact.Phase, 0);
            else if (Artifact.Avaiable && remainingArtifact > 0)
                artifact.Append(Identity, 6, Artifact.ArtifactType, Artifact.Artifact.Phase, remainingArtifact);
            #endregion

            #region Refinery
            uint remainingRefinery = 0;
            if (RefineryIsPermanent())
                RefineryExpire = 0;
            else
                if (now < RefineryExpire && !RefineryIsPermanent())
                    remainingRefinery = (uint)(RefineryExpire - now);

            uint dwType = Refinery.RefineryType;
            if (RefineryType >= 724440 && RefineryType <= 724444)
                dwType = 301;

            if (Refinery.Avaiable && RefineryIsPermanent())
                artifact.Append(Identity, 3, dwType, Refinery.RefineryLevel, Refinery.RefineryPercent, 0);
            else if (Refinery.Avaiable && remainingRefinery > 0)
                artifact.Append(Identity, 2, dwType, Refinery.RefineryLevel, Refinery.RefineryPercent, remainingRefinery);
            #endregion

            m_pOwner.Send(artifact);
            m_pOwner.Send(InformationPacket(true));
        }

        public void SendPurification(Character pTarget)
        {
            if (ArtifactType == 0 && RefineryType == 0 || m_pOwner == null)
                return;

            int now = UnixTimestamp.Timestamp();

            var artifact = new MsgItemStatus();
            uint remainingArtifact = 0;
            if (ArtifactIsPermanent())
                ArtifactExpire = 0;
            else
                if (now < ArtifactExpire && !ArtifactIsPermanent())
                    remainingArtifact = (uint)(ArtifactExpire - now);

            if (Artifact.Avaiable && ArtifactIsPermanent())
                artifact.Append(Identity, 8, Artifact.ArtifactType, Artifact.Artifact.Phase, 0);
            else if (Artifact.Avaiable && remainingArtifact > 0)
                artifact.Append(Identity, 6, Artifact.ArtifactType, Artifact.Artifact.Phase, remainingArtifact);

            uint remainingRefinery = 0;
            if (RefineryIsPermanent())
                RefineryExpire = 0;
            else
                if (now < RefineryExpire && !RefineryIsPermanent())
                    remainingRefinery = (uint)(RefineryExpire - now);

            uint dwType = Refinery.RefineryType;
            if (RefineryType >= 724440 && RefineryType <= 724444)
                dwType = 301;

            if (Refinery.Avaiable && RefineryIsPermanent())
                artifact.Append(Identity, 3, dwType, Refinery.RefineryLevel, Refinery.RefineryPercent, 0);
            else if (Refinery.Avaiable && remainingRefinery > 0)
                artifact.Append(Identity, 2, dwType, Refinery.RefineryLevel, Refinery.RefineryPercent, remainingRefinery);

            pTarget.Send(artifact);
        }

        public void CheckForPurificationExpired()
        {
            var packet = new MsgItemStatus();
            if (!ArtifactIsActive())
            {
                packet.Append(Identity, 7, 0, 0, 0);
                ResetArtifact();
            }
            if (!RefineryIsActive())
            {
                packet.Append(Identity, 4, 0, 0, 0, 0);
                ResetRefinery();
            }
            m_pOwner.Send(packet);
        }

        public bool RefineryIsActive()
        {
            int time = UnixTimestamp.Timestamp();
            return (time < RefineryExpire || RefineryIsPermanent());
        }

        public bool ArtifactIsActive()
        {
            int time = UnixTimestamp.Timestamp();
            return (time < ArtifactExpire || ArtifactIsPermanent());
        }

        public void ResetRefinery()
        {
            RefineryType = 0;
            RefineryLevel = 0;
            RefineryStart = 0;
            RefineryExpire = 0;
            Save();
        }

        public void ResetArtifact()
        {
            ArtifactExpire = 0;
            ArtifactStart = 0;
            ArtifactType = 0;
            Save();
        }

        #endregion

        #region Carry

        public void SaveLocation()
        {
            if (Type == SpecialItem.MEMORY_AGATE)
            {
                if (m_pCarry == null)
                {
                    m_pCarry = new Carry(m_pOwner, this);
                }
                m_pCarry.AddLocation(m_pOwner.MapIdentity, m_pOwner.MapX, m_pOwner.MapY);
            }
        }

        public void UpdateLocation(int idx)
        {
            if (Type == SpecialItem.MEMORY_AGATE)
            {
                if (m_pCarry == null)
                {
                    m_pCarry = new Carry(m_pOwner, this);
                }
                m_pCarry.UpdateLocation(idx);
            }
        }

        public void CarrySetName(string name, int idx)
        {
            if (Type == SpecialItem.MEMORY_AGATE)
            {
                if (m_pCarry == null)
                {
                    m_pCarry = new Carry(m_pOwner, this);
                }
                m_pCarry.SaveName(name, idx);
            }
        }

        public void SendCarry()
        {
            if (Type == SpecialItem.MEMORY_AGATE)
            {
                if (m_pCarry == null)
                {
                    m_pCarry = new Carry(m_pOwner, this);
                }
                m_pCarry.Send();
            }
        }

        public bool GetTeleport(uint idCarry, out DbCarry ret)
        {
            if (SpecialItem.MEMORY_AGATE != Type)
            {
                ret = null;
                return false;
            }

            if (m_pCarry == null)
                m_pCarry = new Carry(m_pOwner, this);

            ret = m_pCarry?.Fetch(idCarry);
            return ret != null;
        }

        public int CarryCount
        {
            get
            {
                if (SpecialItem.MEMORY_AGATE != Type)
                    return 0;
                if (m_pCarry == null)
                    m_pCarry = new Carry(m_pOwner, this);
                return m_pCarry.Count;
            }
        }

        #endregion

        #region Item Check

        public bool ItemActive()
        {
            return RemainingTime > 0;
        }

        public bool ItemExpired()
        {
            return ItemActive() && RemainingTime < UnixTimestamp.Timestamp();
        }

        public int CalculateItemBattlePower(bool skill = false)
        {
            int battle = 0;

            if ((!IsEquipment((int)Type) && !IsWeapon() && Type != 300000)
                || IsBroken() || IsAccessory() || IsMountArmor())
                return 0;

            switch (Type % 10)
            {
                case 9: battle += 4; break;
                case 8: battle += 3; break;
                case 7: battle += 2; break;
                case 6: battle += 1; break;
            }

            battle += (byte)(Plus > 5 && skill ? 5 : Plus);
            battle += ((byte)SocketOne > 0 && (byte)SocketOne < 255) ? 1 : 0;

            if (!skill)
                battle += ((byte)SocketTwo > 0 && (byte)SocketTwo < 255) ? 1 : 0;

            if ((byte)SocketOne > 0 && (byte)SocketOne % 10 == 3) battle++;

            if (!skill)
                if ((byte)SocketTwo > 0 && (byte)SocketTwo % 10 == 3) battle++;

            //Console.WriteLine("{0} - {1}:{2}", m_pOwner.Name, m_dbItemtype.Name, battle);

            return battle;
        }

        public bool IsNonsuchItem()
        {
            switch (Type)
            {
                case SpecialItem.TYPE_DRAGONBALL:
                case SpecialItem.TYPE_METEOR:
                case SpecialItem.TYPE_METEORTEAR:
                    return true;
            }

            // precious gem
            if (IsGem() && Type % 10 >= 2)
                return true;

            // todo handle chests inside of inventory

            // other type
            if (GetSort() == ItemSort.ITEMSORT_USABLE || GetSort() == ItemSort.ITEMSORT_USABLE2)
                return false;

            // high quality
            if (GetQuality() >= 8)
                return true;

            int nGem1 = (int)SocketOne % 10;
            int nGem2 = (int)SocketTwo % 10;

            bool bIsnonsuch = false;

            if (IsWeapon())
            {
                if ((SocketOne != SocketGem.EMPTY_SOCKET && nGem1 >= 2)
                    || (SocketTwo != SocketGem.EMPTY_SOCKET && nGem2 >= 2))
                    bIsnonsuch = true;
            }
            else if (IsShield())
            {
                if (SocketOne != SocketGem.NO_SOCKET || SocketTwo != SocketGem.NO_SOCKET)
                    bIsnonsuch = true;
            }

            return bIsnonsuch;
        }

        public bool IsGem()
        {
            return GetItemSubtype() == 700;
        }

        public void ChangeOwner(Character pPicker)
        {
            m_pOwner = pPicker;
            PlayerIdentity = pPicker.Identity;
        }

        public bool IsPick
        {
            get { return Type / 1000 == 562; }
        }

        public bool CanBeSold
        {
            get { return !Bound; }
        }

        public bool CanBeDropped()
        {
            if (Plus > 5)
                return false;
            if (IsLocked())
                return false;
            if (m_dbItemtype.Monopoly == 9 || Monopoly == 9 || Inscribed)
                return false;
            return true;
        }

        public bool DisappearWhenDropped()
        {
            if (m_dbItemtype.Monopoly == 3 || m_dbItemtype.Monopoly == 9 || Bound)
                return true;
            return false;
        }

        public bool CanBeLocked()
        {
            if (GetSort((int)Type) == ItemSort.ITEMSORT_ACCESSORY && Type != 300000) return false;
            if (IsLocked() && !IsUnlocking()) return false;
            if (!IsEquipment()) return false;
            // TODO idk if there is anything else i should check
            return true;
        }

        /// <summary>
        /// This method will check if the equipment has already been unlocked after the 
        /// 5 days wait.
        /// </summary>
        public bool HasUnlocked()
        {
            return (!IsUnlocking() && !IsLocked() && LockTime > 1);
        }

        public bool IsUnlocking()
        {
            int time = UnixTimestamp.Timestamp();
            return time < LockTime;
        }

        public bool IsLocked()
        {
            int time = UnixTimestamp.Timestamp();
            return LockTime == 1 || LockTime > time;
        }

        public uint GetSellPrice()
        {
            if (Durability == 0)
                return 0;

            uint dwPrice = m_dbItemtype.Price;

            int nAmount = Durability;
            if (nAmount > m_dbItemtype.AmountLimit)
                nAmount = m_dbItemtype.AmountLimit;

            dwPrice = (uint)(dwPrice + Calculations.MulDiv((int)dwPrice, GetQuality() * 5, 100));
            dwPrice = (uint)Calculations.MulDiv((int)dwPrice / 3, nAmount, m_dbItemtype.AmountLimit);

            if (IsArrowSort())
                dwPrice = 0;

            return dwPrice;
        }

        public bool IsBroken() { return Durability == 0; }

        public bool IsNeverDropWhenDead() { return Monopoly == 3 || m_dbItemtype.Monopoly == 3 || m_dbItemtype.Monopoly == 9 || Plus > 5 || Bound || IsLocked(); }

        public int GetLevel() { return (int)((Type % 1000) / 10); }
        public int GetLevel(int nType) { return (nType % 1000) / 10; }

        public int GetQuality() { return (int)(Type % 10); }
        public int GetQuality(int nType) { return nType % 10; }

        public int GetItemSubtype() { return (int)(Type / 1000); }
        public int GetItemSubtype(int nType) { return nType / 1000; }

        public bool IsArrowSort() { return Type / 1000 == 1050; }
        public bool IsArrowSort(int nType) { return nType / 1000 == 1050; }

        public bool IsArrow() { return IsArrowSort(); }
        public bool IsArrow(int nType) { return IsArrowSort(nType); }

        public bool IsExpend() { return IsArrowSort() || Type < 50000; }
        public bool IsExpend(int nType) { return IsArrowSort(nType) || nType < 50000; }

        public bool IsBow() { return Type / 1000 == 500; }
        public bool IsBow(int nType) { return nType / 1000 == 500; }

        public bool IsCountable() { return IsArrowSort(); }
        public bool IsCountable(int nType) { return IsArrowSort(nType); }

        public bool IsMount() { return Type == 300000; }
        public bool IsMount(int nType) { return nType == 300000; }

        public bool IsEquipment() { return !IsArrowSort() && (int)GetSort() < 7 || IsShield() || GetItemSubtype() == 2100; }
        public bool IsEquipment(int nType) { return !IsArrowSort(nType) && (int)GetSort(nType) < 7 || IsShield(nType); }

        public bool IsArmor() { return Type / 10000 == 13; }
        public bool IsArmor(int nType) { return nType / 10000 == 13; }

        public bool IsShield(int nType) { return nType / 1000 == 900; }
        public bool IsShield() { return Type / 1000 == 900; }

        public ItemSort GetSort() { return (ItemSort)(Type / 100000); }
        public ItemSort GetSort(int nType) { return (ItemSort)((nType / 100000)); }

        public bool IsMountArmor() { return Type/1000 == 200; }

        public bool IsAccessory()
        {
            uint nType = Type/1000;
            return nType == 350 || nType == 360 || nType == 370 || nType == 380;
        }

        public bool GetUpLevelChance(int itemtype, out double nChance, out int nNextId)
        {
            nNextId = 0;
            nChance = 0;
            int sort = itemtype / 10000, subtype = itemtype / 1000;

            if (NextItemLevel(itemtype).Type == itemtype)
                return false;

            DbItemtype info = NextItemLevel(itemtype);
            nNextId = (int)info.Type;

            if (info.ReqLevel >= 120)
                return false;

            nChance = 100.00;
            if (sort == 11 || sort == 14 || sort == 13 || sort == 90 || subtype == 123) //Head || Armor || Shield
            {
                switch ((Int32)((info.Type % 100) / 10))
                {
                    case 5: nChance = 50.00; break;
                    case 6: nChance = 40.00; break;
                    case 7: nChance = 30.00; break;
                    case 8: nChance = 20.00; break;
                    case 9: nChance = 15.00; break;
                    default: nChance = 500.00; break;
                }

                switch (info.Type % 10)
                {
                    case 6: nChance = nChance * 0.90; break;
                    case 7: nChance = nChance * 0.70; break;
                    case 8: nChance = nChance * 0.30; break;
                    case 9: nChance = nChance * 0.10; break;
                }
            }
            else
            {
                switch ((Int32)((info.Type % 1000) / 10))
                {
                    case 11: nChance = 95.00; break;
                    case 12: nChance = 90.00; break;
                    case 13: nChance = 85.00; break;
                    case 14: nChance = 80.00; break;
                    case 15: nChance = 75.00; break;
                    case 16: nChance = 70.00; break;
                    case 17: nChance = 65.00; break;
                    case 18: nChance = 60.00; break;
                    case 19: nChance = 55.00; break;
                    case 20: nChance = 50.00; break;
                    case 21: nChance = 45.00; break;
                    case 22: nChance = 40.00; break;
                    default: nChance = 500.00; break;
                }

                switch (info.Type % 10)
                {
                    case 6: nChance = nChance * 0.90; break;
                    case 7: nChance = nChance * 0.70; break;
                    case 8: nChance = nChance * 0.30; break;
                    case 9: nChance = nChance * 0.10; break;
                }
            }
            return true;
        }

        public DbItemtype NextItemLevel(Int32 id)
        {
            // By CptSky
            Int32 NextId = id;

            var Sort = (Byte)(id / 100000);
            var Type = (Byte)(id / 10000);
            var SubType = (Int16)(id / 1000);

            if (Sort == 1) //!Weapon
            {
                if ((Type == 12 && (SubType == 120 || SubType == 121)) || Type == 15 || Type == 16) //Necklace || Ring || Boots
                {
                    var Level = (Byte)(((id % 1000) - (id % 10)) / 10);
                    if ((Type == 12 && Level < 8) || ((Type == 15 && SubType != 152) && Level > 0 && Level < 21) ||
                        ((Type == 15 && SubType == 152) && Level >= 4 && Level < 22) || (Type == 16 && Level > 0 && Level < 21))
                    {
                        //Check if it's still the same type of item...
                        if ((Int16)((NextId + 20) / 1000) == SubType)
                            NextId += 20;
                    }
                    else if ((Type == 12 && Level == 8) || (Type == 12 && Level >= 21) || ((Type == 15 && SubType != 152) && Level == 0)
                             || ((Type == 15 && SubType != 152) && Level >= 21) || ((Type == 15 && SubType == 152) && Level >= 22) || (Type == 16 && Level >= 21))
                    {
                        //Check if it's still the same type of item...
                        if ((Int16)((NextId + 10) / 1000) == SubType)
                            NextId += 10;
                    }
                    else if ((Type == 12 && Level >= 9 && Level < 21) || ((Type == 15 && SubType == 152) && Level == 1))
                    {
                        //Check if it's still the same type of item...
                        if ((Int16)((NextId + 30) / 1000) == SubType)
                            NextId += 30;
                    }
                }
                else if (Type == 11 || Type == 14 || Type == 13 || SubType == 123) //Head || Armor
                {
                    var Level = (Byte)(((id % 100) - (id % 10)) / 10);
                    var Quality = (Byte)(id % 10);

                    //Check if it's still the same type of item...
                    if ((Int16)((NextId + 10) / 1000) == SubType)
                        NextId += 10;
                }
            }
            else if (Sort == 4 || Sort == 5 || Sort == 6) //Weapon
            {
                //Check if it's still the same type of item...
                if ((Int16)((NextId + 10) / 1000) == SubType)
                    NextId += 10;

                //Invalid Backsword ID
                if ((Int32)(NextId / 10) == 42103 || (Int32)(NextId / 10) == 42105 || (Int32)(NextId / 10) == 42109 || (Int32)(NextId / 10) == 42111)
                    NextId += 10;
            }
            else if (Sort == 9)
            {
                var Level = (Byte)(((id % 100) - (id % 10)) / 10);
                if (Level != 30) //!Max...
                {
                    //Check if it's still the same type of item...
                    if ((Int16)((NextId + 10) / 1000) == SubType)
                        NextId += 10;
                }
            }

            DbItemtype itemtype;
            if (ServerKernel.Itemtype.TryGetValue((uint)NextId, out itemtype))
                return itemtype;
            return null;
        }

        public uint ChkUpEqQuality(uint type)
        {
            if (!UpgradableItems(type) || type == SpecialItem.TYPE_MOUNT_ID)
                return 0;

            uint nQuality = type % 10;

            if (nQuality < 3 || nQuality >= 9)
                return 0;

            nQuality++;
            if (nQuality < 5)
                nQuality = (nQuality + (5 - nQuality)) + 1;

            type = ((type - (type % 10)) + nQuality);

            if (!ServerKernel.Itemtype.ContainsKey(type))
                return 0;
            return type;
        }

        public bool GetUpEpQualityInfo(out double nChance, out uint idNewType)
        {
            nChance = 0;
            idNewType = 0;

            if (Type == 150000 || Type == 150310 || Type == 150320 || Type == 410301 || Type == 421301 || Type == 500301)
                return false;

            idNewType = ChkUpEqQuality(Type);
            nChance = 100;

            switch (Type % 10)
            {
                case 6:
                    nChance = 50;
                    break;
                case 7:
                    nChance = 33;
                    break;
                case 8:
                    nChance = 20;
                    break;
                default:
                    nChance = 100;
                    break;
            }
            DbItemtype itemtype;
            if (!ServerKernel.Itemtype.TryGetValue(Type, out itemtype))
                return false;
            uint nFactor = itemtype.ReqLevel;

            if (nFactor > 70)
                nChance = (uint)(nChance * (100 - (nFactor - 70) * 1.0) / 100);

            nChance = Math.Max(1, nChance);
            return true;
        }

        public bool UpgradableItems(uint type)
        {
            return Enum.IsDefined(typeof(ItemPosition), Calculations.GetItemPosition(type));
        }

        public int GetRecoverDurCost()
        {
            // TODO handle kylin gem
            if (Durability > 0
                && Durability < MaximumDurability)
            {
                var price = (int)m_dbItemtype.Price;
                double qualityMultiplier = 0;

                switch (Type % 10)
                {
                    case 9: qualityMultiplier = 1.125; break;
                    case 8: qualityMultiplier = 0.975; break;
                    case 7: qualityMultiplier = 0.9; break;
                    case 6: qualityMultiplier = 0.825; break;
                    default: qualityMultiplier = 0.75; break;
                }

                return (int)Math.Ceiling((price * ((MaximumDurability - Durability) / MaximumDurability)) * qualityMultiplier);
            }
            return 0;
        }

        public uint CalculateSocketingProgress()
        {
            uint total = 0;
            total += TALISMAN_SOCKET_QUALITY_ADDITION[Type % 10];
            total += TALISMAN_SOCKET_PLUS_ADDITION[Plus];
            if (IsWeapon())
            {
                if (SocketTwo > 0)
                    total += TALISMAN_SOCKET_HOLE_ADDITION0[2];
                else if (SocketOne > 0)
                    total += TALISMAN_SOCKET_HOLE_ADDITION0[1];
            }
            else
            {
                if (SocketTwo > 0)
                    total += TALISMAN_SOCKET_HOLE_ADDITION1[2];
                else if (SocketOne > 0)
                    total += TALISMAN_SOCKET_HOLE_ADDITION1[1];
            }
            return total;
        }

        public bool IsWeapon()
        {
            return Type >= 400000 && Type < 700000;
        }

        #endregion

        #region Alter Methods
        public bool TryUnlockItem()
        {
            if (HasUnlocked())
            {
                LockTime = 0;
                Save();
                var unlock = new MsgEquipLock
                {
                    Identity = Identity,
                    Mode = LockMode.UNLOCKED_ITEM
                };
                m_pOwner.Send(unlock);
                unlock.Mode = LockMode.REQUEST_UNLOCK;
                m_pOwner.Send(unlock);
            }
            return false;
        }

        public void SendItemLockTime()
        {
            DateTime unlock = UnixTimestamp.ToDateTime(LockTime);
            if (unlock > DateTime.Now)
            {
                var iPacket = new MsgEquipLock
                {
                    Identity = Identity,
                    Mode = LockMode.REQUEST_UNLOCK,
                    Unknown = 3,
                    Param = (uint)(unlock.Year * 10000 + unlock.Day * 100 + unlock.Month)
                };
                m_pOwner.Send(iPacket);
            }
        }

        public bool RecoverDurability()
        {
            MaximumDurability = m_dbItemtype.AmountLimit;
            m_pOwner.Send(InformationPacket(true));
            Save();
            return true;
        }

        public bool ChangeType(uint type)
        {
            DbItemtype itemtype;
            if (ServerKernel.Itemtype.TryGetValue(type, out itemtype))
            {
                Type = itemtype.Type;
                Durability = itemtype.Amount;
                MaximumDurability = itemtype.AmountLimit;
                m_pOwner.Send(InformationPacket(true));
                Save();
                m_pOwner.Screen.RefreshSpawnForObservers();
                m_pOwner.Send(InformationPacket(true));
                return true;
            }
            return false;
        }

        public uint GetFirstId()
        {
            uint firstId = Type;

            var sort = (byte)(Type / 100000);
            var type = (byte)(Type / 10000);
            var subType = (short)(Type / 1000);

            if (Type == 150000 || Type == 150310 || Type == 150320 || Type == 410301 || Type == 421301 || Type == 500301
                || Type == 601301 || Type == 610301)
                return Type;

            if (Type >= 120310 && Type <= 120319)
                return Type;

            if (sort == 1) //!Weapon
            {
                if (subType == 120 || subType == 121) //Necklace
                    firstId = (Type - (Type % 1000)) + (Type % 10);
                else if (type == 15 || type == 16) //Ring || Boots
                    firstId = (Type - (Type % 1000)) + 10 + (Type % 10);
                else if (type == 11 || subType == 114 || subType == 123 || type == 14) //Head
                {
                    if (subType != 112 && subType != 115 && subType != 116)
                        firstId = (Type - (Type % 1000)) + (Type % 10);
                    else
                    {
                        firstId = (Type - (Type % 1000)) + (Type % 10);
                    }
                }
                else if (type == 14)
                {
                    firstId = (Type - (Type % 1000)) + (Type % 10);
                }
                else if (type == 13) //Armor
                {
                    firstId = (Type - (Type % 1000)) + (Type % 10);
                }
            }
            else if (sort == 4 || sort == 5 || sort == 6) //Weapon
                firstId = (Type - (Type % 1000)) + 20 + (Type % 10);
            else if (sort == 9)
                firstId = (Type - (Type % 1000)) + (Type % 10);

            if (ServerKernel.Itemtype.ContainsKey(firstId))
                return firstId;
            return Type;
        }

        public uint GetUpQualityGemAmount()
        {
            double nChance = 0;
            uint newId = 0;
            if (!GetUpEpQualityInfo(out nChance, out newId))
                return 0;
            return (uint)(100 / nChance + 1) * 12 / 10;
        }

        public uint GetUpgradeGemAmount()
        {
            double nChance = 0;
            int newId = 0;
            if (!GetUpLevelChance((int)Type, out nChance, out newId))
                return 0;
            return (uint)(100 / nChance + 1) * 12 / 10;
        }

        public bool DegradeItem(bool bCheckDura = true)
        {
            if (!IsEquipment())
                return false;
            if (bCheckDura)
                if (Durability / 100 < MaximumDurability / 100)
                {
                    m_pOwner.Owner.SendMessage("Please repair your item before degrading.");
                    return false;
                }

            uint newId = GetFirstId();
            DbItemtype newType = ServerKernel.Itemtype.Values.FirstOrDefault(x => x.Type == newId);
            if (newType == null || newType.Type == Type)
                return false;
            return ChangeType(newType.Type);
        }

        public bool UpItemQuality()
        {
            if (Durability / 100 < MaximumDurability / 100)
            {
                m_pOwner.Owner.SendMessage("Please repair your item before upgrading.");
                return false;
            }

            double nChance = 0;
            uint newId = 0;
            if (!GetUpEpQualityInfo(out nChance, out newId))
            {
                m_pOwner.Owner.SendMessage("This item is already on maximum quality or cannot be upgraded.");
                return false;
            }

            DbItemtype newType;
            if (!ServerKernel.Itemtype.TryGetValue(newId, out newType))
            {
                m_pOwner.Send("Next item not found.");
                return false;
            }

            int gemCost = (int)(100 / nChance + 1) * 12 / 10;

            if (!m_pOwner.Inventory.ReduceDragonBalls((uint)gemCost, Bound))
            {
                m_pOwner.Owner.SendMessage(string.Format("You don't have {0} Dragon Balls.", gemCost));
                return false;
            }

            return ChangeType(newType.Type);
        }

        /// <summary>
        /// This method will upgrade an equipment level using meteors.
        /// </summary>
        /// <returns></returns>
        public bool UpEquipmentLevel()
        {
            if (Durability / 100 < MaximumDurability / 100)
            {
                m_pOwner.Owner.SendMessage("Please repair your item before upgrading.");
                return false;
            }

            double nChance = 0;
            int newId = 0;
            if (!GetUpLevelChance((int)Type, out nChance, out newId))
            {
                m_pOwner.Owner.SendMessage("This item is already on maximum level or cannot be upgraded.");
                return false;
            }

            DbItemtype newType;
            if (!ServerKernel.Itemtype.TryGetValue((uint)newId, out newType))
            {
                m_pOwner.Send("Next item not found.");
                return false;
            }

            if (newType.ReqLevel > m_pOwner.Level)
            {
                m_pOwner.Owner.SendMessage("The equipment level is higher than yours.");
                return false;
            }

            int gemCost = (int)(100 / nChance + 1) * 12 / 10;

            if (!m_pOwner.Inventory.ReduceMeteors((uint)gemCost, Bound))
            {
                m_pOwner.Owner.SendMessage(string.Format("You don't have {0} meteors or meteor tears.", gemCost));
                return false;
            }
            Durability = newType.AmountLimit;
            MaximumDurability = newType.AmountLimit;
            return ChangeType(newType.Type);
        }

        public bool UpUltraEquipmentLevel()
        {
            if (Durability / 100 < MaximumDurability / 100)
            {
                m_pOwner.Owner.SendMessage("Please repair your item before upgrading.");
                return false;
            }

            DbItemtype newType = NextItemLevel((int)Type);

            if (newType == null || newType.Type == Type)
                return false;

            DbItemtype itemtype;
            if (!ServerKernel.Itemtype.TryGetValue(newType.Type, out itemtype))
            {
                m_pOwner.Owner.SendMessage("This item is already on maximum level.");
                return false;
            }

            if (itemtype.ReqLevel > m_pOwner.Level)
            {
                m_pOwner.Owner.SendMessage("The equipment level is higher than yours.");
                return false;
            }

            if (!m_pOwner.Inventory.ReduceDragonBalls(1, Bound))
            {
                m_pOwner.Owner.SendMessage("You don't have a Dragon Ball.");
                return false;
            }

            return ChangeType(newType.Type);
        }

        public void RepairItem()
        {
            if (m_pOwner == null)
                return;

            if (!IsEquipment((int)Type) && !IsWeapon())
                return;

            if (IsBroken())
            {
                if (!m_pOwner.Inventory.ReduceMeteors(5, Bound, false))
                {
                    m_pOwner.Owner.SendMessage("Not enough meteors.");
                    return;
                }
                Durability = MaximumDurability;
                Save();
                m_pOwner.Send(InformationPacket(true));
                ServerKernel.Log.GmLog("Repair", string.Format("User [{2}] repaired broken [{0}][{1}] with 5 meteors.", Type, Identity, PlayerIdentity));
                return;
            }

            var nRecoverDurability = (ushort)(Math.Max(0u, MaximumDurability) - Durability);
            if (nRecoverDurability == 0)
                return;

            double nRepairCost = GetRecoverDurCost(); // ((Itemtype.Price * (Itemtype.AmountLimit / Itemtype.AmountLimit)) / Itemtype.AmountLimit);

            if (!m_pOwner.ReduceMoney((uint)Math.Max(1, nRepairCost)))
            {
                m_pOwner.Owner.SendMessage("Not enough money.");
                return;
            }
            Durability = MaximumDurability;
            Save();
            m_pOwner.Send(InformationPacket(true));
            ServerKernel.Log.GmLog("Repair", string.Format("User [{2}] repaired broken [{0}][{1}] with {3} silvers.", Type, Identity, PlayerIdentity, nRepairCost));
        }
        #endregion

        #region Static Methods

        public static DbItemAddition GetItemAddition(uint itemid, uint plus)
        {
            itemid -= (itemid % 10);
            ItemSort itemSort = (ItemSort)((itemid / 100000));
            if (itemSort == ItemSort.ITEMSORT_WEAPON_SINGLE_HAND)
            {
                itemid = (itemid - (itemid / 1000) * 1000) + 444000;
            }
            if (itemSort == ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND)
            {
                itemid = (itemid - (itemid / 1000) * 1000) + 555000;
            }
            return ServerKernel.ItemAddition.Values.FirstOrDefault(x => x.TypeId == itemid && x.Level == plus);
        }

        #endregion

        #region Socket and Packets

        public MsgItem UsagePacket(ItemAction action)
        {
            return new MsgItem { Identity = Identity, Action = action };
        }

        public MsgItemInfoEx BuildViewItem()
        {
            uint remainingTime = 0;
            int now = UnixTimestamp.Timestamp();
            if (RemainingTime > 0 && now < RemainingTime)
                remainingTime = (uint)(RemainingTime - now);
            else
                RemainingTime = 0;
            return new MsgItemInfoEx
            {
                Itemtype = Type,
                Bless = ReduceDamage,
                Enchant = Enchantment,
                Identity = Identity,
                Color = Color,
                Durability = Durability,
                MaximumDurability = Durability,
                Plus = Plus,
                Position = Position,
                SocketOne = SocketOne,
                SocketTwo = SocketTwo,
                SocketProgress = SocketProgress,
                ViewType = 4,
                TargetIdentity = m_pOwner == null ? 0 : m_pOwner.Identity,
                RemainingTime = remainingTime
            };
        }

        public MsgEquipLock ItemLockPacket()
        {
            DateTime unlock = UnixTimestamp.ToDateTime(LockTime);
            if (unlock > DateTime.Now)
            {
                return new MsgEquipLock
                {
                    Identity = Identity,
                    Mode = LockMode.REQUEST_UNLOCK,
                    Unknown = 3,
                    Param = (uint)(unlock.Year * 10000 + unlock.Day * 100 + unlock.Month)
                };
            }
            return null;
        }

        public MsgItemInformation InformationPacket(bool bUpdate = false)
        {
            m_pMsgInfo.ItemMode = bUpdate ? ItemMode.UPDATE : ItemMode.DEFAULT;
            m_pMsgInfo.Identity = Identity;
            if (m_dbItem.RemainingTime > 0)
                m_pMsgInfo.RemainingTime = (uint) (m_dbItem.RemainingTime - UnixTimestamp.Timestamp());
            return m_pMsgInfo;
        }

        #endregion

        #region Database

        public bool Save()
        {
            return m_dbItem != null && Database.Items.SaveOrUpdate(m_dbItem);
        }

        public bool Delete()
        {
            return m_dbItem != null && Database.Items.Delete(m_dbItem);
        }

        public void GenerateDefaultPacket()
        {
            m_pMsgInfo = new MsgItemInformation();
        }

        public void GeneratePacket(DbItem item)
        {
            uint remainingTime = 0;
            int now = UnixTimestamp.Timestamp();
            if (item.RemainingTime > 0 && now < item.RemainingTime)
                remainingTime = (uint)(item.RemainingTime - now);
            else
                item.RemainingTime = 0;
            m_pMsgInfo = new MsgItemInformation
            {
                Bless= item.ReduceDmg,
                Bound = item.Monopoly == 3,
                Color = (ItemColor)item.Color,
                Durability = item.Amount,
                MaximumDurability = item.AmountLimit,
                Effect = (ItemEffect)item.Magic1,
                Enchantment = item.AddLife,
                Identity = item.Id,
                Inscribed = item.Inscribed > 0,
                Locked = item.Plunder > 0,
                SocketOne = (SocketGem)item.Gem1,
                SocketTwo = (SocketGem)item.Gem2,
                SocketProgress = item.Data,
                Plus = item.Magic3,
                Composition = item.AddlevelExp,
                Position = (ItemPosition)item.Position,
                RemainingTime = remainingTime,
                Itemtype = item.Type,
                Suspicious = item.ChkSum > 0,
                ItemMode = ItemMode.DEFAULT,
                PackageAmount = item.StackAmount
            };
        }

        public void GenerateDefault()
        {
            m_pMsgInfo = new MsgItemInformation();
            m_dbItem = new DbItem
            {
                AddLife = 0,
                AddlevelExp = 0,
                Amount = 1,
                AmountLimit = 1,
                AntiMonster = 0,
                ArtifactExpire = 0,
                ArtifactStabilization = 0,
                ArtifactStart = 0,
                ArtifactType = 0,
                RefineryExpire = 0,
                RefineryLevel = 0,
                RefineryStabilization = 0,
                RefineryStart = 0,
                RefineryType = 0,
                RemainingTime = 0,
                ReduceDmg = 0,
                PlayerId = 0,
                Plunder = 0,
                Position = 0,
                ChkSum = 0,
                Color = 3,
                Data = 0,
                Gem1 = 0,
                Gem2 = 0,
                Ident = 0,
                Inscribed = 0,
                OwnerId = 0,
                Magic1 = 0,
                Magic2 = 0,
                Magic3 = 0,
                Monopoly = 0,
                Specialflag = 0,
                StackAmount = 0,
                Type = 0
            };
        }

        #endregion
    }
}