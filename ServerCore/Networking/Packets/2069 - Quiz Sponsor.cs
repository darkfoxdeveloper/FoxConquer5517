// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2069 - Quiz Sponsor.cs
// Last Edit: 2016/11/23 09:17
// Created: 2016/11/23 09:16
namespace ServerCore.Networking.Packets
{
    public class MsgQuizSponsor : PacketStructure
    {
        public MsgQuizSponsor(uint dwData)
            : base(PacketType.MSG_QUIZ_SPONSOR, 16, 8)
        {
            WriteUInt(dwData, 4);
        }

        public uint Data
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }
    }
}