// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1066 - MsgMeteSpecial.cs
// Last Edit: 2016/12/16 15:16
// Created: 2016/12/16 15:14
namespace ServerCore.Networking.Packets
{
    public sealed class MsgMeteSpecial : PacketStructure
    {
        public MsgMeteSpecial()
            : base(PacketType.MSG_METE_SPECIAL, 20, 12)
        {
            
        }

        public MsgMeteSpecial(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public uint Profession
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Body
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }
    }
}