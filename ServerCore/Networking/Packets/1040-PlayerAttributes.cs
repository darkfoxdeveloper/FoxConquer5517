// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1040 - Player Attributes.cs
// Last Edit: 2016/11/23 08:34
// Created: 2016/11/23 08:34
namespace ServerCore.Networking.Packets
{
    public sealed class MsgPlayerAttribInfo : PacketStructure
    {
        public MsgPlayerAttribInfo()
            : base(PacketType.MSG_PLAYER_ATTRIB_INFO, 144, 136)
        {

        }

        public MsgPlayerAttribInfo(byte[] packet)
            : base(packet)
        {

        }

        /// <summary>
        /// The user identity.
        /// </summary>
        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Life
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint Mana
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint MaxAttack
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint MinAttack
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public uint PhysicalDefense
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint MagicalAttack
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public uint MagicDefense
        {
            get { return ReadUInt(32); }
            set { WriteUInt(value, 32); }
        }

        public uint Dodge
        {
            get { return ReadUInt(36); }
            set { WriteUInt(value, 36); }
        }

        public uint Agility
        {
            get { return ReadUInt(40); }
            set { WriteUInt(value, 40); }
        }

        public uint Accuracy
        {
            get { return ReadUInt(44); }
            set { WriteUInt(value, 44); }
        }

        public uint DragonGemBonus
        {
            get { return ReadUInt(48); }
            set { WriteUInt(value, 48); }
        }

        public uint PhoenixGemBonus
        {
            get { return ReadUInt(52); }
            set { WriteUInt(value, 52); }
        }

        public uint MagicDefenseBonus
        {
            get { return ReadUInt(56); }
            set { WriteUInt(value, 56); }
        }

        public uint TortoiseGemBonus
        {
            get { return ReadUInt(60); }
            set { WriteUInt(value, 60); }
        }

        public uint Bless
        {
            get { return ReadUInt(64); }
            set { WriteUInt(value, 64); }
        }

        public uint CriticalStrike
        {
            get { return ReadUInt(68); }
            set { WriteUInt(value, 68); }
        }

        public uint SkillCriticalStrike
        {
            get { return ReadUInt(72); }
            set { WriteUInt(value, 72); }
        }

        public uint Immunity
        {
            get { return ReadUInt(76); }
            set { WriteUInt(value, 76); }
        }

        public uint Penetration
        {
            get { return ReadUInt(80); }
            set { WriteUInt(value, 80); }
        }

        public uint Block
        {
            get { return ReadUInt(84); }
            set { WriteUInt(value, 84); }
        }

        public uint Breakthrough
        {
            get { return ReadUInt(88); }
            set { WriteUInt(value, 88); }
        }

        public uint Counteraction
        {
            get { return ReadUInt(92); }
            set { WriteUInt(value, 92); }
        }

        public uint Detoxication
        {
            get { return ReadUInt(96); }
            set { WriteUInt(value, 96); }
        }

        public uint FinalPhysicalDamage
        {
            get { return ReadUInt(100); }
            set { WriteUInt(value, 100); }
        }

        public uint FinalMagicDamage
        {
            get { return ReadUInt(104); }
            set { WriteUInt(value, 104); }
        }

        public uint FinalDefense
        {
            get { return ReadUInt(108); }
            set { WriteUInt(value, 108); }
        }

        public uint FinalMagicDefense
        {
            get { return ReadUInt(112); }
            set { WriteUInt(value, 112); }
        }

        public uint MetalDefense
        {
            get { return ReadUInt(116); }
            set { WriteUInt(value, 116); }
        }

        public uint WoodDefense
        {
            get { return ReadUInt(120); }
            set { WriteUInt(value, 120); }
        }

        public uint WaterDefense
        {
            get { return ReadUInt(124); }
            set { WriteUInt(value, 124); }
        }

        public uint FireDefense
        {
            get { return ReadUInt(128); }
            set { WriteUInt(value, 128); }
        }

        public uint EarthDefense
        {
            get { return ReadUInt(132); }
            set { WriteUInt(value, 132); }
        }
    }
}