// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1100 - Mac Address.cs
// Last Edit: 2016/11/23 08:41
// Created: 2016/11/23 08:40
namespace ServerCore.Networking.Packets
{
    public class MsgMacAddr : PacketStructure
    {
        public MsgMacAddr(byte[] pMsg)
            : base(pMsg)
        {

        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public string MacAddress
        {
            get { return ReadString(12, 8); }
        }
    }
}