// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1128 - MsgVipUserHandle.cs
// Last Edit: 2017/01/27 17:27
// Created: 2017/01/27 17:20

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgVipUserHandle : PacketStructure
    {
        public MsgVipUserHandle()
            : base(PacketType.MSG_VIP_USER_HANDLE, 40, 32)
        {
            
        }

        public MsgVipUserHandle(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public VIPTeleportTypes Type
        {
            get { return (VIPTeleportTypes) ReadUInt(4); }
            set { WriteUInt((uint) value, 4); }
        }

        public VIPTeleportLocations Location
        {
            get { return (VIPTeleportLocations) ReadUInt(8); }
            set { WriteUInt((uint) value, 8); }
        }

        public uint Countdown
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public string Name
        {
            get { return ReadString(ReadByte(16), 17); }
            set { WriteStringWithLength(value, 16); }
        }
    }
}