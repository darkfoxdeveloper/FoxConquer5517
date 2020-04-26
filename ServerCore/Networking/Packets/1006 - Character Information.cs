// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1006 - Character Information.cs
// Last Edit: 2016/11/23 08:11
// Created: 2016/11/23 08:11

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 1006. This packet is sent to the server after the player's character has been loaded from the 
    /// database. It is used to initialize the client with character data, updated by update packets then after. It
    /// is also sent to the map server to initialize the player's location and character information. This packet 
    /// sets the identity for the client and map server.
    /// </summary>
    public sealed class MsgUserInfo : PacketStructure
    {
        /// <summary>
        /// Packet Type: 1006. This packet is sent to the server after the player's character has been loaded from the 
        /// database. It is used to initialize the client with character data, updated by update packets then after. It
        /// is also sent to the map server to initialize the player's location and character information. This packet 
        /// sets the identity for the client and map server.
        /// </summary>
        public MsgUserInfo(string szName, string szSecondName, string szMate)
            : base(128 + szName.Length + szSecondName.Length + szMate.Length)
        {
            // Fill the packet structure using the database object:
            WriteHeader(Length - 8, PacketType.MSG_USER_INFO);
            NameDisplayed = true;
            StringCount = 3;
            Name = szName;
            SecondName = "None";
            Mate = szMate;
        }

        /// <summary>
        /// Packet Type: 1006. This packet is sent to the server after the player's character has been loaded from the 
        /// database. It is used to initialize the client with character data, updated by update packets then after. It
        /// is also sent to the map server to initialize the player's location and character information. This packet 
        /// sets the identity for the client and map server.
        /// </summary>
        /// <param name="array">The received packet from the message server.</param>
        public MsgUserInfo(byte[] array) : base(array) { }

        /// <summary>
        /// The character UID.
        /// </summary>
        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        /// <summary>
        /// The Mesh of the character. (Lookface * 10000) + BodyType
        /// </summary>
        public uint Mesh
        {
            get { return ReadUInt(10); }
            set { WriteUInt(value, 10); }
        }

        /// <summary>
        /// The hair style of the character.
        /// </summary>
        public ushort Hairstyle
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        /// <summary>
        /// The silver amount.
        /// </summary>
        public uint Silver
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        /// <summary>
        /// The e-money (CPs) amount that the character has.
        /// </summary>
        public uint ConquerPoints
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        /// <summary>
        /// The amount of experience the player has towards leveling up.
        /// </summary>
        public long Experience
        {
            get { return ReadLong(24); }
            set { WriteLong(value, 24); }
        }

        // offset 30 (ulong) reserved for instructor experience.
        // offset 38 (ulong) reserved for mercenary experience.
        // offset 46 (uint) reserved for potential.

        /// <summary>
        /// The strength attribute points.
        /// </summary>
        public ushort Strength
        {
            get { return ReadUShort(52); }
            set { WriteUShort(value, 52); }
        }

        /// <summary>
        /// The character's agility to dodge attacks and attack faster.
        /// </summary>
        public ushort Agility
        {
            get { return ReadUShort(54); }
            set { WriteUShort(value, 54); }
        }

        /// <summary>
        /// The character resistance to damage (increases health limit).
        /// </summary>
        public ushort Vitality
        {
            get { return ReadUShort(56); }
            set { WriteUShort(value, 56); }
        }

        /// <summary>
        /// The character's spirit energy (increases mana limit).
        /// </summary>
        public ushort Spirit
        {
            get { return ReadUShort(58); }
            set { WriteUShort(value, 58); }
        }

        /// <summary>
        /// The unspent attribute points used towards increasing attributes.
        /// </summary>
        public ushort Attributes
        {
            get { return ReadUShort(60); }
            set { WriteUShort(value, 60); }
        }

        /// <summary>
        /// The character's current health level.
        /// </summary>
        public ushort Health
        {
            get { return ReadUShort(62); }
            set { WriteUShort(value, 62); }
        }

        /// <summary>
        /// The character's current mana level.
        /// </summary>
        public ushort Mana
        {
            get { return ReadUShort(64); }
            set { WriteUShort(value, 64); }
        }

        public ushort PkPoints
        {
            get { return ReadUShort(66); }
            set { WriteUShort(value, 66); }
        }

        public byte Level
        {
            get { return ReadByte(68); }
            set { WriteByte(value, 68); }
        }

        /// <summary>
        /// The actual profession
        /// </summary>
        public byte Profession
        {
            get { return ReadByte(69); }
            set { WriteByte(value, 69); }
        }

        /// <summary>
        /// The second life profession.
        /// </summary>
        public byte PreviousProfession
        {
            get { return ReadByte(71); }
            set { WriteByte(value, 71); }
        }

        /// <summary>
        /// The amount of reborns.
        /// </summary>
        public byte Metempsychosis
        {
            get { return ReadByte(73); }
            set { WriteByte(value, 73); }
        }

        /// <summary>
        /// The first life profession.
        /// </summary>
        public byte AncestorProfession
        {
            get { return ReadByte(70); }
            set { WriteByte(value, 70); }
        }

        /// <summary>
        /// If the names will be displayed on login or not.
        /// </summary>
        public bool NameDisplayed
        {
            get { return ReadBoolean(74); }
            set { WriteBoolean(value, 74); }
        }

        /// <summary>
        /// The amount of Quiz Points that the user has.
        /// </summary>
        public uint QuizPoints
        {
            get { return ReadUInt(75); }
            set { WriteUInt(value, 75); }
        }

        /// <summary>
        /// The amount of Enlighten points. 100 equals to 1 point.
        /// </summary>
        public ushort Enlighten
        {
            get { return ReadUShort(79); }
            set { WriteUShort(value, 79); }
        }

        /// <summary>
        /// 1 point equals to 20 minutes. Not sure yet if it's Uint or byte.
        /// </summary>
        public byte EnlightenExp
        {
            get { return ReadByte(81); }
            set { WriteByte(value, 81); }
        }

        /// <summary>
        /// The amount of bound CPs the user have.
        /// </summary>
        public uint BoundEmoney
        {
            get { return ReadUInt(93); }
            set { WriteUInt(value, 93); }
        }

        /// <summary>
        /// The title that the player is holding. Example: Elite PK
        /// </summary>
        public byte PlayerTitle
        {
            get { return ReadByte(91); }
            set { WriteByte(value, 91); }
        }

        /// <summary>
        /// The amount of strings that will be sent. Actually 3, Name, SecondName (Empty) and the Spouse name.
        /// </summary>
        public byte StringCount
        {
            get { return ReadByte(110); }
            set { WriteByte(value, 110); }
        }

        /// <summary>
        /// The character name.
        /// </summary>
        public string Name
        {
            get { return ReadString(ReadByte(111), 112); }
            set { WriteStringWithLength(value, 111); }
        }             // 111 - The character's name and the length of the character's name.

        /// <summary>
        /// This function has been removed around 525+, So just leave this empty.
        /// </summary>
        public string SecondName
        {
            get { int length = Name.Length; return ReadString(ReadByte(112 + length), 113 + length); }
            set { WriteStringWithLength(value, 112 + Name.Length); }
        }

        /// <summary>
        /// The character spouse name.
        /// </summary>
        public string Mate
        {
            get { int length = Name.Length + SecondName.Length; return ReadString(ReadByte(113 + length), 114 + length); }
            set { WriteStringWithLength(value, 113 + Name.Length + SecondName.Length); }
        }
    }
}