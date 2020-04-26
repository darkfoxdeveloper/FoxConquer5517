// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 10017 - User Update.cs
// Last Edit: 2016/11/23 08:17
// Created: 2016/11/23 08:17
namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 10017
    /// This packet updates the client interface.
    /// </summary>
    public sealed class MsgUserAttrib : PacketStructure
    {
        /// <summary>
        /// Create the basic packet with the minimum length.
        /// </summary>
        public MsgUserAttrib()
            : base(40)
        {
            WriteHeader(Length - 8, PacketType.MSG_USER_ATTRIB);
        }

        /// <summary>
        /// Not sure yet.. gotta update in a while.
        /// </summary>
        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        /// <summary>
        /// The number of updates that are being sent on this packet.
        /// </summary>
        public uint UpdateCount
        {
            get { return ReadUInt(8); }
            set
            {
                Resize((int)(32 + (value * 20) + 8));
                WriteHeader(Length - 8, PacketType.MSG_USER_ATTRIB);
                WriteUInt(value, 8);
            }
        }

        /// <summary>
        /// Append the request to the update packet.
        /// </summary>
        /// <param name="type">The action that will be updated on the client screen.</param>
        /// <param name="value">The new value.</param>
        public void Append(ClientUpdateType type, byte value)
        {
            UpdateCount += 1;
            var offset = (ushort)(UpdateCount * 12);
            WriteUInt((byte)type, offset);
            WriteULong(value, offset + 4);
        }

        /// <summary>
        /// Append the request to the update packet.
        /// </summary>
        /// <param name="type">The action that will be updated on the client screen.</param>
        /// <param name="value">The new value.</param>
        public void Append(ClientUpdateType type, ushort value)
        {
            UpdateCount += 1;
            var offset = (ushort)(UpdateCount * 12);
            WriteUInt((byte)type, offset);
            WriteULong(value, offset + 4);
        }

        /// <summary>
        /// Append the request to the update packet.
        /// </summary>
        /// <param name="type">The action that will be updated on the client screen.</param>
        /// <param name="value">The new value.</param>
        public void Append(ClientUpdateType type, uint value)
        {
            UpdateCount += 1;
            var offset = (ushort)(UpdateCount * 12);
            WriteUInt((byte)type, offset);
            WriteULong(value, offset + 4);
        }

        /// <summary>
        /// Append the request to the update packet.
        /// </summary>
        /// <param name="type">The action that will be updated on the client screen.</param>
        /// <param name="value1">The new value.</param>
        /// <param name="value2"></param>
        public void Append(ClientUpdateType type, uint value1, uint value2)
        {
            UpdateCount += 1;
            var offset = (ushort)(UpdateCount * 12);
            WriteUInt((byte)type, offset);
            WriteUInt(value1, offset + 4);
            WriteUInt(value2, offset + 8);
        }

        /// <summary>
        /// Append the request to the update packet.
        /// </summary>
        /// <param name="type">The action that will be updated on the client screen.</param>
        /// <param name="value1">The new value.</param>
        /// <param name="value2"></param>
        public void Append(ClientUpdateType type, uint value1, uint value2, uint value3, uint value4)
        {
            UpdateCount += 1;
            var offset = (ushort)(UpdateCount * 12);
            WriteUInt((byte)type, offset);
            WriteUInt(value1, offset + 4);
            WriteUInt(value2, offset + 8);
            WriteUInt(value3, offset + 12);
            WriteUInt(value4, offset + 16);
        }

        /// <summary>
        /// Append the request to the update packet.
        /// </summary>
        /// <param name="type">The action that will be updated on the client screen.</param>
        /// <param name="value">The new value.</param>
        public void Append(ClientUpdateType type, ulong value)
        {
            UpdateCount += 1;
            var offset = (ushort)(UpdateCount * 12);
            WriteUInt((byte)type, offset);
            WriteULong(value, offset + 4);
        }

        /// <summary>
        /// Append the request to the update packet. Used for flags.
        /// </summary>
        public void Append(ClientUpdateType type, ulong value1, ulong value2)
        {
            UpdateCount += 1;
            var offset = (ushort)(UpdateCount * 12);
            WriteUInt((byte)type, offset);
            WriteULong(value1, offset + 4);
            WriteULong(value2, offset + 12);
        }

        /// <summary>
        /// Clear the packet.
        /// </summary>
        public void Clear()
        {
            ClearPacket(32, PacketType.MSG_USER_ATTRIB);
        }
    }

    /// <summary>
    /// The attributes to be updated on the client.
    /// </summary>
    public enum ClientUpdateType : byte
    {
        HITPOINTS = 0,
        MAX_HITPOINTS = 1,
        MANA = 2,
        MAX_MANA = 3,
        MONEY = 4,
        EXPERIENCE = 5,
        PK_POINTS = 6,
        CLASS = 7,
        STAMINA = 8,
        WH_MONEY = 9,
        ATRIBUTES = 10,
        MESH = 11,
        LEVEL = 12,
        SPIRIT = 13,
        VITALITY = 14,
        STRENGTH = 15,
        AGILITY = 16,
        HEAVENS_BLESSING = 17,
        DOUBLE_EXP_TIMER = 18,
        CURSED_TIMER = 20,
        REBORN = 22,
        STATUS_FLAG = 25,
        HAIR_STYLE = 26,
        XP_CIRCLE = 27,
        LUCKY_TIME_TIMER = 28,
        CONQUER_POINTS = 29,
        ONLINE_TRAINING = 31,
        EXTRA_BATTLE_POWER = 36,
        UNKNOWN1 = 37,
        MERCHANT = 38,
        VIP_LEVEL = 39,
        QUIZ_POINTS = 40,
        ENLIGHT_POINTS = 41,
        HONOR_POINTS = 42,
        UNKNOWN0 = 43,
        GUILD_BATTLEPOWER = 44,
        BOUND_CONQUER_POINTS = 45,
        AZURE_SHIELD = 49,
        SOUL_SHACKLE_TIMER = 54
    }
}