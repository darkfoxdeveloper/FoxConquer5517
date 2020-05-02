// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2227 - MsgSynRecruitAdvertisingOpt.cs
// Last Edit: 2016/12/07 22:01
// Created: 2016/12/07 21:59
namespace ServerCore.Networking.Packets
{
    public sealed class MsgSynRecruitAdvertisingOpt : PacketStructure
    {
        public MsgSynRecruitAdvertisingOpt(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public uint Action
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint EntityIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint Identity
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }
    }
}