// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2067 - Mentor Contribution.cs
// Last Edit: 2016/11/23 09:15
// Created: 2016/11/23 09:15

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public class MsgContribute : PacketStructure
    {
        public MsgContribute()
            : base(PacketType.MSG_GUIDE_CONTRIBUTE, 48, 40)
        {

        }

        public MsgContribute(byte[] pbuffer)
            : base(pbuffer)
        {

        }

        public GuideContributionType Type
        {
            get { return (GuideContributionType)ReadUInt(4); }
            set { WriteUInt((uint)value, 4); }
        }

        public uint MentorIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint PrizeExperience
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public ushort HeavenBlessing
        {
            get { return ReadUShort(32); }
            set { WriteUShort(value, 32); }
        }

        public ushort Composing
        {
            get { return ReadUShort(34); }
            set { WriteUShort(value, 34); }
        }
    }
}