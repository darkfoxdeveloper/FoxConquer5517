// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - ServerCore - 10014 - Player Spawn.cs
// Last Edit: 2017/02/04 17:19
// Created: 2017/02/04 14:41

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 10014. This packet is sent to the observing clients on the map server when the actor enters their 
    /// screen or an acting client observes the character as they enter its screen. The packet contains the player's 
    /// character spawn information. This class only encapsulates constants related to writing data to the packet 
    /// buffer. The character class handles writing to the packet as data changes.
    /// </summary>
    public sealed class MsgPlayer : PacketStructure
    {
        /// <summary>
        /// This method should not be called, unless you're testing something. Has been created for packet research.
        /// </summary>
        public MsgPlayer(uint identity)
            : base(PacketType.MSG_PLAYER, 260, 252)
        {
            Identity = identity;
            StringCount = 1;
        }

        /// <summary>
        /// Packet Type: 10014. This packet is sent to the observing clients on the map server when the actor enters their 
        /// screen or an acting client observes the character as they enter its screen. The packet contains the player's 
        /// character spawn information. This class only encapsulates constants related to writing data to the packet 
        /// buffer. The character class handles writing to the packet as data changes.
        /// Updated to version 5517 by Felipe Vieira.
        /// </summary>
        /// <param name="characterInformation">The character initialization packet which contains all character information.</param>
        public MsgPlayer(MsgUserInfo characterInformation)
            : base(PacketType.MSG_PLAYER, 260 + characterInformation.Name.Length,
                252 + characterInformation.Name.Length)
        {
            Identity = characterInformation.Identity;
            Mesh = characterInformation.Mesh;
            Hairstyle = characterInformation.Hairstyle;
            Direction = FacingDirection.SOUTH_WEST;
            Action = EntityAction.STAND;
            Life = characterInformation.Health;
            Level = characterInformation.Level;
            Metempsychosis = characterInformation.Metempsychosis;
            Nobility = 0;
            FirstProfession = (ProfessionType) characterInformation.AncestorProfession;
            LastProfession = (ProfessionType) characterInformation.PreviousProfession;
            Profession = (ProfessionType) characterInformation.Profession;
            QuizPoints = characterInformation.QuizPoints;
            Title = 0;
            StringCount = 3;
            Name = characterInformation.Name;
            SecondName = "None";
            FamilyName = "None";
        }

        /// <summary>
        /// The mesh of the character, includes avatar, transformation, and body.
        /// </summary>
        public uint Mesh
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        /// <summary>
        /// The global identification number for the player.
        /// </summary>
        public uint Identity
        {
            get { return ReadUInt(8); }
            private set { WriteUInt(value, 8); }
        }

        /// <summary>
        /// The guild unique identification.
        /// </summary>
        public ushort GuildIdentity
        {
            get { return ReadUShort(12); }
            set { WriteUShort(value, 12); }
        }

        /// <summary>
        /// The user guild position.
        /// </summary>
        public SyndicateRank GuildRank
        {
            get { return (SyndicateRank) ReadUShort(16); }
            set { WriteUShort((ushort) value, 16); }
        }

        /// <summary>
        /// Effect and Status flag.
        /// </summary>
        public ulong Flag1
        {
            get { return ReadULong(22); }
            set { WriteULong(value, 22); }
        }

        /// <summary>
        /// Effect and Status flag.
        /// </summary>
        public ulong Flag2
        {
            get { return ReadULong(30); }
            set { WriteULong(value, 30); }
        }

        public byte CurrentLayout
        {
            get { return ReadByte(38); }
            set { WriteByte(value, 38); }
        }

        /// <summary>
        /// The helmet the character is wearing.
        /// </summary>
        public uint Helmet
        {
            get { return ReadUInt(40); }
            set { WriteUInt(value, 40); }
        }

        /// <summary>
        /// The garment the character is wearing.
        /// </summary>
        public uint Garment
        {
            get { return ReadUInt(44); }
            set { WriteUInt(value, 44); }
        }

        /// <summary>
        /// The armor the character is wearing.
        /// </summary>
        public uint Armor
        {
            get { return ReadUInt(48); }
            set { WriteUInt(value, 48); }
        }

        public uint LeftHand
        {
            get { return ReadUInt(52); }
            set { WriteUInt(value, 52); }
        }

        public uint RightHand
        {
            get { return ReadUInt(56); }
            set { WriteUInt(value, 56); }
        }

        public uint RightAccessory
        {
            get { return ReadUInt(60); }
            set { WriteUInt(value, 60); }
        }

        public uint LeftAccessory
        {
            get { return ReadUInt(64); }
            set { WriteUInt(value, 64); }
        }

        public uint MountType
        {
            get { return ReadUInt(68); }
            set { WriteUInt(value, 68); }
        }

        public uint MountArmor
        {
            get { return ReadUInt(72); }
            set { WriteUInt(value, 72); }
        }

        public ushort Life
        {
            get { return ReadUShort(80); }
            set { WriteUShort(value, 80); }
        }

        public byte MonsterLevel
        {
            get { return ReadByte(82); }
            set { WriteByte(value, 82); }
        }

        public ushort Hairstyle
        {
            get { return ReadUShort(84); }
            set { WriteUShort(value, 84); }
        } // 78  - The hairstyle for the character.

        public ushort MapX
        {
            get { return ReadUShort(86); }
            set { WriteUShort(value, 86); }
        } // 80  - The x-coordinate of the player's character on the map.

        public ushort MapY
        {
            get { return ReadUShort(88); }
            set { WriteUShort(value, 88); }
        } // 82  - The y-coordinate of the player's character on the map.

        public FacingDirection Direction
        {
            get { return (FacingDirection) ReadByte(90); }
            set { WriteByte((byte) value, 90); }
        } // 84  - The direction the character is facing in.

        public EntityAction Action
        {
            get { return (EntityAction) ReadUShort(91); }
            set { WriteUShort((byte) value, 91); }
        } // 85  - The action the character is currently performing.

        public byte Metempsychosis
        {
            get { return ReadByte(98); }
            set { WriteByte(value, 98); }
        }

        public byte Level
        {
            get { return ReadByte(99); }
            set { WriteByte(value, 99); }
        }

        public bool WindowSpawn
        {
            get { return ReadBoolean(101); }
            set { WriteBoolean(value, 101); }
        }

        public bool Away
        {
            get { return ReadBoolean(102); }
            set { WriteBoolean(value, 102); }
        }

        public uint SharedBattlePower
        {
            get { return ReadUInt(103); }
            set { WriteUInt(value, 103); }
        }

        public uint FlowerRanking
        {
            get { return ReadUInt(115); }
            set { WriteUInt(value, 115); }
        }

        public byte Nobility
        {
            get { return ReadByte(119); }
            set { WriteByte(value, 119); }
        }

        public ushort ArmorColor
        {
            get { return ReadUShort(123); }
            set { WriteUShort(value, 123); }
        }

        public ushort ShieldColor
        {
            get { return ReadUShort(125); }
            set { WriteUShort(value, 125); }
        }

        public ushort HelmetColor
        {
            get { return ReadUShort(127); }
            set { WriteUShort(value, 127); }
        }

        public uint QuizPoints
        {
            get { return ReadUInt(129); }
            set { WriteUInt(value, 129); }
        }

        public byte MountPlus
        {
            get { return ReadByte(133); }
            set { WriteByte(value, 133); }
        }

        public uint MountColor
        {
            get { return ReadUInt(139); }
            set { WriteUInt(value, 139); }
        }

        /// <summary>
        /// 100 Points == 1 Enlighten.
        /// Shows the icon above the entity head :) "Finger pls"
        /// </summary>
        public ushort EnlightenPoints
        {
            get { return ReadUShort(144); }
            set { WriteUShort(value, 144); }
        }

        public uint FamilyIdentity
        {
            get { return ReadUInt(155); }
            set { WriteUInt(value, 155); }
        }

        public FamilyRank FamilyRank
        {
            get { return (FamilyRank) ReadUInt(159); }
            set { WriteUInt((uint) value, 159); }
        }

        public byte Title
        {
            get { return ReadByte(167); }
            set { WriteByte(value, 167); }
        }

        public bool IsBoss
        {
            get { return ReadBoolean(181); }
            set { WriteBoolean(value, 181); }
        }

        public uint HelmetArtifact
        {
            get { return ReadUInt(182); }
            set { WriteUInt(value, 182); }
        }

        public uint ArmorArtifact
        {
            get { return ReadUInt(186); }
            set { WriteUInt(value, 186); }
        }

        public uint LeftHandArtifact
        {
            get { return ReadUInt(190); }
            set { WriteUInt(value, 190); }
        }

        public uint RightHandArtifact
        {
            get { return ReadUInt(194); }
            set { WriteUInt(value, 194); }
        }

        /// <summary>
        /// First life profession (0 rebirths)
        /// </summary>
        public ProfessionType FirstProfession
        {
            get { return (ProfessionType) ReadUShort(207); }
            set { WriteUShort((ushort) value, 207); }
        }

        /// <summary>
        /// Second life profession (1 Rebirth)
        /// </summary>
        public ProfessionType LastProfession
        {
            get { return (ProfessionType) ReadUShort(209); }
            set { WriteUShort((ushort) value, 209); }
        }

        /// <summary>
        /// Third life profession (2 rebirth)
        /// </summary>
        public ProfessionType Profession
        {
            get { return (ProfessionType) ReadUShort(211); }
            set { WriteUShort((ushort) value, 211); }
        }

        public byte StringCount
        {
            get { return ReadByte(218); }
            set { WriteByte(value, 218); }
        } // 212 - The amount of strings in the packet.

        /// <summary>
        /// The character name
        /// </summary>
        public string Name
        {
            get { return ReadString(ReadByte(219), 220); }
            set { WriteStringWithLength(value, 219); }
        } // 219 - The character's name and the length of the character's name.

        /// <summary>
        /// This function has been removed around 525+, So just leave this empty.
        /// </summary>
        public string SecondName
        {
            get
            {
                int length = Name.Length;
                return ReadString(ReadByte(220 + length), 221 + length);
            }
            set { WriteStringWithLength(value, 220 + Name.Length); }
        }

        public string FamilyName
        {
            get
            {
                int length = Name.Length + SecondName.Length;
                return ReadString(ReadByte(221 + length), 222 + length);
            }
            set { WriteStringWithLength(value, 221 + Name.Length + SecondName.Length); }
        }
    }
}