// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1008 - Item Information.cs
// Last Edit: 2016/11/23 08:20
// Created: 2016/11/23 08:19

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 1008
    /// Contains the offsets to spawn items on the user inventory, equipments etc.
    /// </summary>
    public sealed class MsgItemInformation : PacketStructure
    {
        /// <summary>
        /// Contains the offsets to spawn items on the user inventory, equipments etc.
        /// </summary>
        /// <param name="packet">An array of bytes to be deserialized.</param>
        public MsgItemInformation(byte[] packet)
            : base(packet)
        {
        }

        /// <summary>
        /// Starts an empty instance of Item Information packet.
        /// </summary>
        public MsgItemInformation()
            : base(76)
        {
            WriteHeader(Length - 8, PacketType.MSG_ITEM_INFORMATION);
        }
        
        /// <summary>
        /// The unique identification of the item.
        /// </summary>
        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        /// <summary>
        /// The itemtype of the item.
        /// </summary>
        public uint Itemtype
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        /// <summary>
        /// The current durability of the item.
        /// </summary>
        public ushort Durability
        {
            get { return ReadUShort(12); }
            set { WriteUShort(value, 12); }
        }

        /// <summary>
        /// The maximum durability of the item.
        /// </summary>
        public ushort MaximumDurability
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        /// <summary>
        /// The actual mode of the item.
        /// </summary>
        public ItemMode ItemMode
        {
            get { return (ItemMode)ReadByte(16); }
            set { WriteByte((byte)value, 16); }
        }

        /// <summary>
        /// The current position of the item.
        /// </summary>
        public ItemPosition Position
        {
            get { return (ItemPosition)ReadUShort(18); }
            set { WriteUShort((ushort)value, 18); }
        }

        /// <summary>
        /// On talismans, the socketing progress. On Steeds holds the color.
        /// </summary>
        public uint SocketProgress
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        /// <summary>
        /// The first socket of the item.
        /// </summary>
        public SocketGem SocketOne
        {
            get { return (SocketGem) ReadByte(24); }
            set { WriteByte((byte) value, 24); }
        }

        /// <summary>
        /// The second socket of the item.
        /// </summary>
        public SocketGem SocketTwo
        {
            get { return (SocketGem) ReadByte(25); }
            set { WriteByte((byte) value, 25); }
        }

        /// <summary>
        /// The effect of the item. (Example: Mana)
        /// </summary>
        public ItemEffect Effect
        {
            get { return (ItemEffect)ReadByte(28); }
            set { WriteByte((byte)value, 28); }
        }

        /// <summary>
        /// The plus attribute of the equipment. Should be attached to stones.
        /// </summary>
        public byte Plus
        {
            get { return ReadByte(33); }
            set { WriteByte(value, 33); }
        }

        /// <summary>
        /// The blessing of the item.
        /// </summary>
        public byte Bless
        {
            get { return ReadByte(34); }
            set { WriteByte(value, 34); }
        }

        /// <summary>
        /// Bound items does have monopoly 3.
        /// </summary>
        public bool Bound
        {
            get { return ReadBoolean(35); }
            set { WriteBoolean(value, 35); }
        }

        /// <summary>
        /// The attached HP to the item.
        /// </summary>
        public byte Enchantment
        {
            get { return ReadByte(36); }
            set { WriteByte(value, 36); }
        }

        /// <summary>
        /// I will double check what is this, so i will rename. Guess it's something about
        /// monsters attribute.
        /// </summary>
        public uint GreenText
        {
            get { return ReadUInt(40); }
            set { WriteUInt(value, 40); }
        }

        /// <summary>
        /// If the item is suspicious or not.
        /// </summary>
        public bool Suspicious
        {
            get { return ReadBoolean(44); }
            set { WriteBoolean(value, 44); }
        }

        /// <summary>
        /// Gotta confirm where i'll store this shit.
        /// </summary>
        public bool Locked
        {
            get { return ReadBoolean(46); }
            set { WriteBoolean(value, 46); }
        }

        /// <summary>
        /// The color of the item.
        /// </summary>
        public ItemColor Color
        {
            get { return (ItemColor)ReadByte(48); }
            set { WriteByte((byte)value, 48); }
        }

        /// <summary>
        /// The composition status of the item. (Progress)
        /// </summary>
        public uint Composition
        {
            get { return ReadUInt(52); }
            set { WriteUInt(value, 52); }
        }

        /// <summary>
        /// If the item is inscribed or not on the guild arsenal or not.
        /// </summary>
        public bool Inscribed
        {
            get { return ReadBoolean(56); }
            set { WriteBoolean(value, 56); }
        }

        /// <summary>
        /// Gotta check out how to handle this.
        /// </summary>
        public uint RemainingTime
        {
            get { return ReadUInt(60); }
            set { WriteUInt(value, 60); }
        }

        /// <summary>
        /// The amount of items in 1 inventory block.
        /// </summary>
        public ushort PackageAmount
        {
            get { return ReadUShort(64); }
            set { WriteUShort(value, 64); }
        }
    }
}