// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1129 - Vip Functions.cs
// Last Edit: 2016/11/23 08:51
// Created: 2016/11/23 08:51
namespace ServerCore.Networking.Packets
{
    public class MsgVipFunctionValidNotify : PacketStructure
    {
        public MsgVipFunctionValidNotify()
            : base(PacketType.MSG_VIP_FUNCTION_VALID_NOTIFY, 16, 8)
        {
            VipFunctions = 57255;
        }

        public MsgVipFunctionValidNotify(byte[] packet)
            : base(packet)
        {

        }

        /// <summary>
        /// Send 57255 for all.
        /// </summary>
        public uint VipFunctions
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }
    }
}