// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1025 - Weapon Skill.cs
// Last Edit: 2016/11/23 08:28
// Created: 2016/11/23 08:28
namespace ServerCore.Networking.Packets
{
    public sealed class MsgWeaponSkill : PacketStructure
    {
        /// <summary>
        /// This method will create the empty packet.
        /// </summary>
        public MsgWeaponSkill()
            : base(PacketType.MSG_WEAPON_SKILL, 28, 20)
        {

        }

        /// <summary>
        /// This method will create the packet and set each value on his right place.
        /// </summary>
        /// <param name="type">The weapon subtype. (Id/1000)</param>
        /// <param name="level">The weapon level.</param>
        /// <param name="experience">The proficiency experience.</param>
        public MsgWeaponSkill(uint type, uint level, uint experience)
            : base(PacketType.MSG_WEAPON_SKILL, 28, 20)
        {
            Type = type;
            Level = level;
            Experience = experience;
            LevelExperience = 100; // >(o_O)< ué
        }

        public MsgWeaponSkill(byte[] packet)
            : base(packet)
        {
            // We never know xD hate editing dlls just to add minor things
        }

        public uint Type
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Level
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint Experience
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint LevelExperience
        {
            get { return Level > MAX_PROFICIENCY_LEVEL ? 0 : EXP_PER_LEVEL[(int)Level]; }
            set { WriteUInt(Level > MAX_PROFICIENCY_LEVEL ? 0 : EXP_PER_LEVEL[(int)Level], 16); }
        }

        public const uint MAX_PROFICIENCY_LEVEL = 20;

        public static readonly uint[] EXP_PER_LEVEL = new uint[21]
        {
            0,
            1200,
            68000,
            250000,
            640000,
            1600000,
            4000000,
            10000000,
            22000000,
            40000000,
            90000000,
            95000000,
            142500000,
            213750000,
            320625000,
            480937500,
            721406250,
            1082109375,
            1623164063,
            2100000000,
            0
        };
    }
}