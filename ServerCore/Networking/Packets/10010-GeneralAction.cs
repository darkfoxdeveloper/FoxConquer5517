// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 10010 - General Action.cs
// Last Edit: 2016/11/23 08:11
// Created: 2016/11/23 08:09

using ServerCore.Common;
using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 1010. This packet encapsulates a general action to be performed by the server or by the 
    /// client. The action is performed by an actor of the screen or sends a general action to an observer.
    /// It can also be used to control actions in the client interface.
    /// </summary>
    public sealed class MsgAction : PacketStructure
    {
        /// <summary>
        /// Packet Type: 1010. This packet encapsulates a general action to be performed by the server or by the 
        /// client. The action is performed by an actor of the screen or sends a general action to an observer.
        /// It can also be used to control actions in the client interface.
        /// </summary>
        /// <param name="packet">The received packet from the client.</param>
        public MsgAction(byte[] packet)
            : base(packet) { }

        /// <summary>
        /// Packet Type: 1010. This packet encapsulates a general action to be performed by the server or by the 
        /// client. The action is performed by an actor of the screen or sends a general action to an observer.
        /// It can also be used to control actions in the client interface.
        /// </summary>
        /// <param name="identity">The identity of the actor.</param>
        /// <param name="left">The left data being sent through the packet.</param>
        /// <param name="right">The right data being sent through the packet.</param>
        /// <param name="action">The type of action being performed.</param>
        public MsgAction(uint identity, ushort left, ushort right, GeneralActionType action)
            : base(PacketType.MSG_ACTION, 44, 36)
        {
            TimeStamp = Time.Now;
            Identity = identity;
            LeftData = left;
            RightData = right;
            Action = action;
        }

        /// <summary>
        /// Packet Type: 1010. This packet encapsulates a general action to be performed by the server or by the 
        /// client. The action is performed by an actor of the screen or sends a general action to an observer.
        /// It can also be used to control actions in the client interface.
        /// </summary>
        /// <param name="identity">The identity of the actor.</param>=
        /// <param name="data">The data being sent through the action.</param>
        /// <param name="x">The x-coordinate of the character.</param>
        /// <param name="y">The y-coordinate of the character.</param>
        /// <param name="action">The type of action being performed.</param>
        public MsgAction(uint identity, uint data, ushort x, ushort y, GeneralActionType action)
            : base(PacketType.MSG_ACTION, 44, 36)
        {
            TimeStamp = Time.Now;
            Identity = identity;
            Data = data;
            X = x;
            Y = y;
            Action = action;
        }

        // Packet Structure Properties:
        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }              // 4  - The identity of the actor.
        public uint Data
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }                  // 8  - The data being sent through the action.
        public ushort LeftData
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }            // 8  - Left data inside the data parameter.
        public ushort RightData
        {
            get { return ReadUShort(10); }
            set { WriteUShort(value, 10); }
        }           // 10 - Right data inside the data parameter.
        public uint Details
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }               // 12 - The details for the action, used in defining sub-actions.
        public Time TimeStamp
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }             // 16 - The time stamp from the client for processing.
        public GeneralActionType Action
        {
            get { return (GeneralActionType)ReadUInt(20); }
            set { WriteUInt((uint)value, 20); }
        }   // 20 - The type of action being performed by the actor.
        public FacingDirection Direction
        {
            get { return (FacingDirection)ReadUShort(22); }
            set { WriteUShort((ushort)value, 22); }
        }  // 22 - The direction the player is facing in.
        public ushort X
        {
            get { return ReadUShort(24); }
            set { WriteUShort(value, 24); }
        }                   // 24 - The x-coordinate of the character.
        public ushort Y
        {
            get { return ReadUShort(26); }
            set { WriteUShort(value, 26); }
        }                   // 26 - The y-coordinate of the character.
        public uint Map
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }                   // 28 - The map of the action.
        public uint Hue
        {
            get { return ReadUInt(32); }
            set { WriteUInt(value, 32); }
        }                   // 32 - The RGB code for the color of the action.
    }

    /// <summary> This enumeration type defines the types of general actions that can be performed. </summary>
    public enum GeneralActionType : ushort
    {
        SET_LOCATION = 74,
        HOTKEYS = 75,
        CONFIRM_FRIENDS = 76,
        CONFIRM_PROFICIENCIES = 77,
        CONFIRM_SPELLS = 78,
        CHANGE_DIRECTION = 79,
        CHANGE_ACTION = 81,
        USE_PORTAL = 85,
        CHANGE_MAP = 86,
        LEVELED = 92,
        XP_CLEAR = 93,
        REVIVE = 94,
        DEL_ROLE = 95,
        CHANGE_PK_MODE = 96,
        CONFIRM_GUILD = 97,
        MINE = 99,
        TEAM_MEMBER_POS = 101,
        REQUEST_ENTITY_SPAWN = 102,
        ABORT_MAGIC = 103,
        MAP_ARGB = 104,
        MAP_STATUS = 105,
        QUERY_TEAM_MEMBER = 106,
        NEW_COORDINATES = 108,
        DROP_MAGIC = 109,
        DROP_SKILL = 110,
        CREATE_BOOTH = 111,
        SUSPEND_BOOTH = 112,
        RESUME_BOOTH = 113,
        GET_SURROUNDINGS = 114,
        OPEN_CUSTOM = 116,
        OBSERVE_EQUIPMENT = 117,
        ABORT_TRANSFORM = 118,
        END_FLY = 120,
        GET_MONEY = 121,
        VIEW_ENEMY_INFO = 123,
        OPEN_WINDOW = 126,
        GUARD_JUMP = 130,
        COMPLETE_LOGIN = 132,
        /// <summary>
        /// Data1 = EntityId,
        /// Data3Low = PositionX,
        /// Data3High = PositionY
        /// </summary>
        SPAWN_EFFECT = 134,
        REMOVE_ENTITY = 135,
        JUMP = 137,
        TELEPORT_REPLY = 138,
        DIE_QUESTION = 145,
        END_TELEPORT = 146,
        VIEW_FRIEND_INFO = 148,
        CHANGE_FACE = 151,
        VIEW_PARTNER_INFO = 152,
        DETAIN_ITEM = 153,
        ITEMS_DETAINED = 155,
        NINJA_STEP = 156,
        HIDE_INTERFACE = 158,
        OPEN_UPGRADE = 160,
        AWAY = 161,
        PATH_FINDING = 162,
        DRAGON_BALL_DROPPED = 165,
        CHANGE_LOOK = 178,
        TABLE_STATE = 233,
        TABLE_POT = 234,
        TABLE_PLAYER_COUNT = 235,
        UNKNOWN1 = 251,
        OBSERVE_FRIEND_EQUIPMENT = 310,
        UNKNOWN2 = 408
    }
}