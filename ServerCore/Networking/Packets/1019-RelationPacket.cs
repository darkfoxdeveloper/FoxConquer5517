// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1019 - Relation Packet.cs
// Last Edit: 2016/11/23 08:26
// Created: 2016/11/23 08:26
namespace ServerCore.Networking.Packets
{
    public enum RelationAction : byte
    {
        REQUEST_FRIEND = 10,
        NEW_FRIEND = 11,
        SET_ONLINE_FRIEND = 12,
        SET_OFFLINE_FRIEND = 13,
        REMOVE_FRIEND = 14,
        ADD_FRIEND = 15,
        SET_ONLINE_ENEMY = 16,
        SET_OFFLINE_ENEMY = 17,
        REMOVE_ENEMY = 18,
        ADD_ENEMY = 19,
    }

    public class MsgFriend : PacketStructure
    {
        public MsgFriend()
            : base(PacketType.MSG_FRIEND, 48, 40)
        {

        }

        public MsgFriend(RelationAction mode, uint targetId, string targetName, bool online)
            : base(PacketType.MSG_FRIEND, 48, 40)
        {
            Mode = mode;
            Identity = targetId;
            Name = targetName;
            Online = online;
        }

        public MsgFriend(byte[] packet)
            : base(packet)
        {

        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public RelationAction Mode
        {
            get { return (RelationAction)ReadByte(8); }
            set { WriteByte((byte)value, 8); }
        }

        public bool Online
        {
            get { return ReadBoolean(9); }
            set { WriteBoolean(value, 9); }
        }

        public ushort Unknown0
        {
            get { return ReadUShort(10); }
            set { WriteUShort(value, 10); }
        }

        public uint Unknown1
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint Unknown2
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public string Name
        {
            get { return ReadString(16, 20); }
            set { WriteString(value, 16, 20); }
        }

        public uint Unknown3
        {
            get { return ReadUInt(36); }
            set { WriteUInt(value, 36); }
        }
    }
}