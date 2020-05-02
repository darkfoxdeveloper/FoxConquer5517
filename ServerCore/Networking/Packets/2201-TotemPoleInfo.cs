// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2201 - Totem Pole Info.cs
// Last Edit: 2016/11/23 09:37
// Created: 2016/11/23 09:37

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public class MsgTotemPoleInfo : PacketStructure
    {
        public MsgTotemPoleInfo()
            : base(PacketType.MSG_TOTEM_POLE_INFO, 252, 244)
        {
            // The values below should be constants and match with the enum.
            //TotemAmount = 8;
            //HearwearIdentity = (uint)TotemPoleType.TOTEM_HEADGEAR;
            //NecklaceIdentity = (uint)TotemPoleType.TOTEM_NECKLACE;
            //ArmorIdentity = (uint)TotemPoleType.TOTEM_ARMOR;
            //WeaponIdentity = (uint)TotemPoleType.TOTEM_WEAPON;
            //RingIdentity = (uint)TotemPoleType.TOTEM_RING;
            //BootsIdentity = (uint)TotemPoleType.TOTEM_BOOTS;
            //FanIdentity = (uint)TotemPoleType.TOTEM_FAN;
            //TowerIdentity = (uint)TotemPoleType.TOTEM_TOWER;
        }

        public MsgTotemPoleInfo(byte[] packet)
            : base(packet)
        {

        }

        /// <summary>
        /// The total battle power that the guild is sharing.
        /// </summary>
        public uint BattlePower
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        /// <summary>
        /// The total totem donation of the entity.
        /// </summary>
        public uint TotemDonation
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        /// <summary>
        /// The amount of battle power that the totem shares with the entity.
        /// </summary>
        public uint SharedBattlePower
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        /// <summary>
        /// This will tell the client how many totem we have. (Not open) Should be eight.
        /// </summary>
        public uint TotemAmount
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public void AddTotemPole(TotemPoleType type, uint dwBp, uint dwBoost, uint dwTotalDonation, bool isOpen)
        {
            int offset = (int) (24 + (TotemAmount*24));
            TotemAmount += 1;
            WriteUInt((uint) type, offset);
            WriteUInt(dwBp, offset + 4);
            WriteUInt(dwBoost, offset + 8);
            WriteUInt(dwTotalDonation, offset + 12);
            WriteBoolean(isOpen, offset + 20);
        }

        /// <summary>
        /// This identity will be used when adding a new item to the totem.
        /// </summary>
        public uint HearwearIdentity
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        /// <summary>
        /// The total Headwear Battle Power
        /// </summary>
        public byte HeadwearBattlePower
        {
            get { return ReadByte(28); }
            set { WriteByte(value, 28); }
        }

        /// <summary>
        /// The total donation of the headwear totem
        /// </summary>
        public uint HeadwearTotalDonation
        {
            get { return ReadUInt(36); }
            set { WriteUInt(value, 36); }
        }

        /// <summary>
        /// Whether the totem headwear is open or not.
        /// </summary>
        public bool TotemHeadwearIsOpen
        {
            get { return ReadBoolean(44); }
            set { WriteBoolean(value, 44); }
        }

        /// <summary>
        /// This identity will be used when adding a new item to the totem.
        /// </summary>
        public uint ArmorIdentity
        {
            get { return ReadUInt(48); }
            set { WriteUInt(value, 48); }
        }

        /// <summary>
        /// The total Armor Battle Power
        /// </summary>
        public byte ArmorBattlePower
        {
            get { return ReadByte(52); }
            set { WriteByte(value, 52); }
        }

        /// <summary>
        /// The total donation of the Armor totem
        /// </summary>
        public uint ArmorTotalDonation
        {
            get { return ReadUInt(60); }
            set { WriteUInt(value, 60); }
        }

        /// <summary>
        /// Whether the totem Armor is open or not.
        /// </summary>
        public bool TotemArmorIsOpen
        {
            get { return ReadBoolean(68); }
            set { WriteBoolean(value, 68); }
        }

        /// <summary>
        /// This identity will be used when adding a new item to the totem.
        /// </summary>
        public uint WeaponIdentity
        {
            get { return ReadUInt(72); }
            set { WriteUInt(value, 72); }
        }

        /// <summary>
        /// The total Weapon Battle Power
        /// </summary>
        public byte WeaponBattlePower
        {
            get { return ReadByte(76); }
            set { WriteByte(value, 76); }
        }

        /// <summary>
        /// The total donation of the Weapon totem
        /// </summary>
        public uint WeaponTotalDonation
        {
            get { return ReadUInt(84); }
            set { WriteUInt(value, 84); }
        }

        /// <summary>
        /// Whether the totem Weapon is open or not.
        /// </summary>
        public bool TotemWeaponIsOpen
        {
            get { return ReadBoolean(92); }
            set { WriteBoolean(value, 92); }
        }

        /// <summary>
        /// This identity will be used when adding a new item to the totem.
        /// </summary>
        public uint RingIdentity
        {
            get { return ReadUInt(96); }
            set { WriteUInt(value, 96); }
        }

        /// <summary>
        /// The total Ring/HeavyRing/Bracelet Battle Power
        /// </summary>
        public byte RingBattlePower
        {
            get { return ReadByte(100); }
            set { WriteByte(value, 100); }
        }

        /// <summary>
        /// The total donation of the Ring/HeavyRing/Bracelet totem
        /// </summary>
        public uint RingTotalDonation
        {
            get { return ReadUInt(108); }
            set { WriteUInt(value, 108); }
        }

        /// <summary>
        /// Whether the totem Ring/HeavyRing/Bracelet is open or not.
        /// </summary>
        public bool TotemRingIsOpen
        {
            get { return ReadBoolean(116); }
            set { WriteBoolean(value, 116); }
        }

        /// <summary>
        /// This identity will be used when adding a new item to the totem.
        /// </summary>
        public uint BootsIdentity
        {
            get { return ReadUInt(120); }
            set { WriteUInt(value, 120); }
        }

        /// <summary>
        /// The total Boots Battle Power
        /// </summary>
        public byte BootsBattlePower
        {
            get { return ReadByte(124); }
            set { WriteByte(value, 124); }
        }

        /// <summary>
        /// The total donation of the Boots totem
        /// </summary>
        public uint BootsTotalDonation
        {
            get { return ReadUInt(132); }
            set { WriteUInt(value, 132); }
        }

        /// <summary>
        /// Whether the totem Boots is open or not.
        /// </summary>
        public bool TotemBootsIsOpen
        {
            get { return ReadBoolean(140); }
            set { WriteBoolean(value, 140); }
        }

        /// <summary>
        /// This identity will be used when adding a new item to the totem.
        /// </summary>
        public uint NecklaceIdentity
        {
            get { return ReadUInt(144); }
            set { WriteUInt(value, 144); }
        }

        /// <summary>
        /// The total Necklace Battle Power
        /// </summary>
        public byte NecklaceBattlePower
        {
            get { return ReadByte(148); }
            set { WriteByte(value, 148); }
        }

        /// <summary>
        /// The total donation of the Necklace totem
        /// </summary>
        public uint NecklaceTotalDonation
        {
            get { return ReadUInt(156); }
            set { WriteUInt(value, 156); }
        }

        /// <summary>
        /// Whether the totem Necklace is open or not.
        /// </summary>
        public bool TotemNecklaceIsOpen
        {
            get { return ReadBoolean(164); }
            set { WriteBoolean(value, 164); }
        }

        /// <summary>
        /// This identity will be used when adding a new item to the totem.
        /// </summary>
        public uint FanIdentity
        {
            get { return ReadUInt(168); }
            set { WriteUInt(value, 168); }
        }

        /// <summary>
        /// The total Fan Battle Power
        /// </summary>
        public byte FanBattlePower
        {
            get { return ReadByte(172); }
            set { WriteByte(value, 172); }
        }

        /// <summary>
        /// The total donation of the Fan totem
        /// </summary>
        public uint FanTotalDonation
        {
            get { return ReadUInt(180); }
            set { WriteUInt(value, 180); }
        }

        /// <summary>
        /// Whether the totem Fan is open or not.
        /// </summary>
        public bool TotemFanIsOpen
        {
            get { return ReadBoolean(188); }
            set { WriteBoolean(value, 188); }
        }

        /// <summary>
        /// This identity will be used when adding a new item to the totem.
        /// </summary>
        public uint TowerIdentity
        {
            get { return ReadUInt(192); }
            set { WriteUInt(value, 192); }
        }

        /// <summary>
        /// The total Tower Battle Power
        /// </summary>
        public byte TowerBattlePower
        {
            get { return ReadByte(196); }
            set { WriteByte(value, 196); }
        }

        /// <summary>
        /// The total donation of the Tower totem
        /// </summary>
        public uint TowerTotalDonation
        {
            get { return ReadUInt(204); }
            set { WriteUInt(value, 204); }
        }

        /// <summary>
        /// Whether the totem Tower is open or not.
        /// </summary>
        public bool TotemTowerIsOpen
        {
            get { return ReadBoolean(212); }
            set { WriteBoolean(value, 212); }
        }
    }
}