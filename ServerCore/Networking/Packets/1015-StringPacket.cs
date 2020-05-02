// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1015 - String Packet.cs
// Last Edit: 2016/11/23 08:25
// Created: 2016/11/23 08:25

using System.Collections.Generic;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgName : PacketStructure
    {
        public MsgName()
            : base(PacketType.MSG_NAME, 19, 11)
        {

        }

        public MsgName(byte[] packet)
            : base(packet)
        {

        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public ushort PosX
        {
            get { return ReadUShort(4); }
            set { WriteUShort(value, 4); }
        }

        public ushort PosY
        {
            get { return ReadUShort(6); }
            set { WriteUShort(value, 6); }
        }

        public StringAction Action
        {
            get { return (StringAction)ReadByte(8); }
            set { WriteByte((byte)value, 8); }
        }

        public byte TextAmount
        {
            get { return ReadByte(9); }
            set
            {
                int newSize = 11 + value + _totalStringLength + 8;
                Resize(newSize);
                WriteHeader(newSize - 8, PacketType.MSG_NAME);
                WriteByte(value, 9);
            }
        }

        private int _totalStringLength = 0;

        public void Append(string text)
        {
            _totalStringLength += text.Length;
            TextAmount += 1;
            var offset = (ushort)(10 + (TextAmount - 1) + (_totalStringLength - text.Length));
            WriteStringWithLength(text, offset);
        }

        public List<string> Strings()
        {
            var strList = new List<string>();
            var offset = (ushort)10;
            for (int i = 0; i < TextAmount; i++)
            {
                string message = ReadString(ReadByte(offset++), offset);
                strList.Add(message);
                offset += (ushort)message.Length;
            }
            return strList;
        }
    }

    public enum StringAction : byte
    {
        NONE = 0,
        FIREWORKS,
        CREATE_GUILD,
        GUILD,
        CHANGE_TITLE,
        DELETE_ROLE = 5,
        MATE,
        QUERY_NPC,
        WANTED,
        MAP_EFFECT,
        ROLE_EFFECT = 10,
        MEMBER_LIST,
        KICKOUT_GUILD_MEMBER,
        QUERY_WANTED,
        QUERY_POLICE_WANTED,
        POLICE_WANTED = 15,
        QUERY_MATE,
        ADD_DICE_PLAYER,
        DELETE_DICE_PLAYER,
        DICE_BONUS,
        PLAYER_WAVE = 20,
        SET_ALLY,
        SET_ENEMY,
        WHISPER_WINDOW_INFO = 26
    }
}