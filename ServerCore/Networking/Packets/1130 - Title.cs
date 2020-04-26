// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1130 - Title.cs
// Last Edit: 2016/11/23 08:52
// Created: 2016/11/23 08:52

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgTitle : PacketStructure
    {
        public MsgTitle()
            : base(PacketType.MSG_TITLE, 20, 12)
        {

        }

        public MsgTitle(byte[] packet)
            : base(packet)
        {

        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public UserTitle SelectedTitle
        {
            get { return (UserTitle)ReadByte(8); }
            set { WriteByte((byte)value, 8); }
        }

        public TitleAction Action
        {
            get { return (TitleAction)ReadByte(9); }
            set { WriteByte((byte)value, 9); }
        }

        public byte Count
        {
            get { return ReadByte(10); }
            set
            {
                Resize(20 + value);
                WriteHeader(Length - 8, PacketType.MSG_TITLE);
                WriteByte(value, 10);
            }
        }

        public void Append(byte title)
        {
            Count += 1;
            var offset = (ushort)(10 + Count);
            WriteByte(title, offset);
        }
    }
}