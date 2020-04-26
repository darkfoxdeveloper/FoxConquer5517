// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1038 - Solidify.cs
// Last Edit: 2016/11/23 08:33
// Created: 2016/11/23 08:33

using System.Collections.Generic;

namespace ServerCore.Networking.Packets
{
    public enum SolidifyMode
    {
        // I still guessing those
        ARTIFACT = 1
    }

    public sealed class MsgSolidify : PacketStructure
    {
        public MsgSolidify()
            : base(PacketType.MSG_SOLIDIFY, 28, 20)
        {

        }

        public MsgSolidify(byte[] packet)
            : base(packet)
        {

        }

        public SolidifyMode Mode
        {
            get { return (SolidifyMode)ReadUInt(4); }
            set { WriteUInt((uint)value, 4); }
        }

        public uint TargetItem
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint StoneAmount
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public List<uint> StoneItems
        {
            get
            {
                var items = new List<uint>();
                for (int count = 16; count <= 12 + StoneAmount * 4; count += 4)
                {
                    items.Add(ReadUInt(count));
                }
                return items;
            }
        }
    }
}