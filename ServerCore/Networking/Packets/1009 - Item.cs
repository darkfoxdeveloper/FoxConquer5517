// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1009 - Item.cs
// Last Edit: 2016/11/23 08:24
// Created: 2016/11/23 08:21
namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 1009
    /// This class handles the item usage packet.
    /// </summary>
    public sealed class MsgItem : PacketStructure
    {
        /// <summary>
        /// This will deserialize the input packet into an array of bytes.
        /// </summary>
        /// <param name="arrayBytes">The 1009 packet to be deserialized.</param>
        public MsgItem(byte[] arrayBytes)
            : base(arrayBytes)
        {

        }

        /// <summary>
        /// This will create a new instance of the packet.
        /// </summary>
        public MsgItem()
            : base(84 + 8)
        {
            WriteHeader(Length - 8, PacketType.MSG_ITEM);
        }

        /// <summary>
        /// The target of the action. May be the Shop ID that is being used.
        /// </summary>
        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        /// <summary>
        /// The item ID.
        /// </summary>
        public uint Param1
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        /// <summary>
        /// The action that is being called.
        /// </summary>
        public ItemAction Action
        {
            get { return (ItemAction)ReadUInt(12); }
            set { WriteUInt((uint)value, 12); }
        }

        /// <summary>
        /// The time when you called the action usually.
        /// </summary>
        public uint Timestamp
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        /// <summary>
        /// The amount of items you choose to buy on shops.
        /// </summary>
        public uint Param2
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public uint Param3
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint Param4
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public uint Headgear { get { return ReadUInt(32); } set { WriteUInt(value, 32); } }
        public uint Necklace { get { return ReadUInt(36); } set { WriteUInt(value, 36); } }
        public uint Armor { get { return ReadUInt(40); } set { WriteUInt(value, 40); } }
        public uint RightHand { get { return ReadUInt(44); } set { WriteUInt(value, 44); } }
        public uint LeftHand { get { return ReadUInt(48); } set { WriteUInt(value, 48); } }
        public uint Ring { get { return ReadUInt(52); } set { WriteUInt(value, 52); } }
        public uint Talisman { get { return ReadUInt(56); } set { WriteUInt(value, 56); } }
        public uint Boots { get { return ReadUInt(60); } set { WriteUInt(value, 60); } }
        public uint Garment { get { return ReadUInt(64); } set { WriteUInt(value, 64); } }
        public uint RightAccessory { get { return ReadUInt(68); } set { WriteUInt(value, 68); } }
        public uint LeftAccessory { get { return ReadUInt(72); } set { WriteUInt(value, 72); } }
        public uint MountArmor { get { return ReadUInt(76); } set { WriteUInt(value, 76); } }
        public uint Crop { get { return ReadUInt(80); } set { WriteUInt(value, 80); } }
    }

    /// <summary>
    /// The item actions.
    /// </summary>
    public enum ItemAction : uint
    {
        NONE = 0,
        BUY = 1,
        SELL = 2,
        REMOVE = 3,
        USE = 4,
        EQUIP = 5,
        UNEQUIP = 6,
        QUERY_MONEY_SAVED = 9,
        SAVE_MONEY = 10,
        DRAW_MONEY = 11,
        DROP_MONEY = 12,
        SPEND_MONEY = 13,
        REPAIR = 14,
        REPAIR_ALL = 15,
        IDENT = 16,
        DURABILITY = 17,
        DROP_EQUIPMENT = 18,
        IMPROVE = 19,
        UPLEV = 20,
        BOOTH_QUERY = 21,
        BOOTH_ADD = 22,
        BOOTH_DELETE = 23,
        BOOTH_BUY = 24,
        SYNCHRO_AMOUNT = 25,
        FIREWORKS = 26,
        PING = 27,
        ENCHANT = 28,
        BOOTH_ADD_CP = 29,
        REDEEM_EQUIPMENT = 32,
        PK_ITEM_REDEEM = 33,
        PK_ITEM_CLOSE = 34,
        TALISMAN_SOCKET_PROGRESS = 35,
        TALISMAN_SOCKET_PROGRESS_CPS = 36,
        DROP = 37,
        TORTOISE_COMPOSE = 40,
        ACTIVATE_ACCESSORY = 41,
        SOCKET_EQUIPMENT = 43,
        DISPLAY_GEARS = 46,
        MERGE_ITEMS = 48,
        SPLIT_ITEMS = 49,
        REQUEST_ITEM_TOOLTIP = 52,
        DEGRADE_EQUIPMENT = 54
    }
}