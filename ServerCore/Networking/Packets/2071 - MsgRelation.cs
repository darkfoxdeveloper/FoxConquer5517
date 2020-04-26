// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2071 - MsgRelation.cs
// Last Edit: 2016/12/14 17:27
// Created: 2016/12/14 17:24
namespace ServerCore.Networking.Packets
{
    public sealed class MsgRelation : PacketStructure
    {
        public MsgRelation()
            : base(PacketType.MSG_RELATION, 40, 32)
        {
            
        }

        public MsgRelation(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public uint SenderIdentity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint TargetIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint Level
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint BattlePower
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public bool IsSpouse
        {
            get { return ReadBoolean(20); }
            set { WriteBoolean(value, 20); }
        }

        public bool IsApprentice
        {
            get { return ReadBoolean(24); }
            set { WriteBoolean(value, 24); }
        }

        public bool IsTradePartner
        {
            get { return ReadBoolean(28); }
            set { WriteBoolean(value, 28); }
        }
    }
}