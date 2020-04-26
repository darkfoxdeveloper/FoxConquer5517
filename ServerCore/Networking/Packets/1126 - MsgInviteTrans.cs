// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - ServerCore - 1126 - MsgInviteTrans.cs
// Last Edit: 2017/02/15 17:10
// Created: 2017/02/15 17:10

namespace ServerCore.Networking.Packets
{
    public sealed class MsgInviteTrans : PacketStructure
    {
        public MsgInviteTrans()
            : base(PacketType.MSG_INVITE_TRANS, 28, 20)
        {
            
        }

        public MsgInviteTrans(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public uint Countdown
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint ResourceIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint Unknown
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }
    }
}