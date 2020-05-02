// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2078 - User Ip Info.cs
// Last Edit: 2016/11/23 09:20
// Created: 2016/11/23 09:19
namespace ServerCore.Networking.Packets
{
    public sealed class MsgUserIpInfo : PacketStructure
    {
        public MsgUserIpInfo()
            : base(272)
        {
            WriteHeader(Length - 8, PacketType.MSG_USER_IP_INFO);
            WriteUInt(0x4e591dba, 4);
        }
    }
}