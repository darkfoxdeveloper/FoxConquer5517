// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2079 - Server Status.cs
// Last Edit: 2016/11/23 09:20
// Created: 2016/11/23 09:20
namespace ServerCore.Networking.Packets
{
    public sealed class MsgServerInfo : PacketStructure
    {
        /// <summary>
        /// I still don't know why this packet is about, but without this the client
        /// wont let us signin.
        /// </summary>
        public MsgServerInfo()
            : base(20)
        {
            WriteHeader(Length - 8, PacketType.MSG_SERVER_INFO);
            WriteUInt(0, 4);
        }

        // 1 yes 0 no
        public ushort ClassicMode
        {
            get { return ReadUShort(4); }
            set { WriteUShort(value, 4); }
        }

        public ushort PotencyMode
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8) ;}
        }
    }
}