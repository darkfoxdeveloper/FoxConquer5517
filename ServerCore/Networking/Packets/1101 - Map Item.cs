// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1101 - Map Item.cs
// Last Edit: 2016/11/23 08:41
// Created: 2016/11/23 08:41
namespace ServerCore.Networking.Packets
{
    public sealed class MsgMapItem : PacketStructure
    {
        public MsgMapItem()
            : base(PacketType.MSG_MAP_ITEM, 32, 24)
        {

        }

        public MsgMapItem(byte[] packet)
            : base(packet)
        {

        }

        public MsgMapItem(uint itemIdentity, uint itemtype, ushort mapX, ushort mapY, ushort dropType)
            : base(PacketType.MSG_MAP_ITEM, 32, 24)
        {
            Identity = itemIdentity;
            Itemtype = itemtype;
            MapX = mapX;
            MapY = mapY;
            ItemColor = 3;
            DropType = dropType;
        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Itemtype
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public ushort MapX
        {
            get { return ReadUShort(12); }
            set { WriteUShort(value, 12); }
        }

        public ushort MapY
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        public ushort ItemColor
        {
            get { return ReadUShort(16); }
            set { WriteUShort(value, 16); }
        }

        public ushort DropType
        {
            get { return ReadUShort(18); }
            set { WriteUShort(value, 18); }
        }
    }

    public enum DropType
    {
        MONEY = 0,
        E_MONEY = 1,
        ITEM = 2
    }
}