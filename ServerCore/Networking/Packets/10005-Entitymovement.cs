// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 10005 - Entity movement.cs
// Last Edit: 2016/11/23 08:19
// Created: 2016/11/23 08:19

using ServerCore.Common;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 1005. This packet encapsulates a character's ground movement on a map. The movement packet
    /// specifies the type of movement being performed and the direction the player as it moves on the map. The
    /// packet shows movements from actors on the server, and should be sent back to the actor and message server
    /// to complete the movement.
    /// </summary>
    public sealed class MsgWalk : PacketStructure
    {
        /// <summary>
        /// Packet Type: 1005. This packet encapsulates a character's ground movement on a map. The movement packet
        /// specifies the type of movement being performed and the direction the player as it moves on the map. The
        /// packet shows movements from actors on the server, and should be sent back to the actor and message server
        /// to complete the movement.
        /// </summary>
        /// <param name="packet">The packet received from the client.</param>
        public MsgWalk(byte[] packet)
            : base(packet)
        {
        }

        public MsgWalk(uint dwDir, uint dwId, MovementType pAction, uint dwMap)
            : base(PacketType.MSG_WALK, 32, 24)
        {
            Direction = dwDir;
            Identity = dwId;
            Action = pAction;
            Map = dwMap;
            TimeStamp = Time.Now;
        }

        // Packet Structure Properties:
        public uint Direction
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }             // 4  - The direction the actor is facing in as it takes a step.
        public uint Identity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }              // 8  - The identity of the actor.
        public MovementType Action
        {
            get { return (MovementType)ReadByte(12); }
            set { WriteByte((byte)value, 12); }
        }        // 12 - The type of movement being performed by the actor.
        public Time TimeStamp
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }             // 16 - The time stamp from the client for processing.
        public uint Map
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }                   // 20 - The identity of the map.
    }

    /// <summary> This enumeration type defines the types of movements the actor can perform. </summary>
    public enum MovementType : byte
    {
        WALK = 0,
        RUN = 1,
        SHIFT = 2,
        JUMP = 3,
        TRANS = 4,
        CHANGE_MAP = 5,
        JUMP_MAGIC_ATTACK = 6,
        COLLIDE = 7,
        SYNCHRO = 8,
        RIDE = 9
    }
}