// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1061 - MsgDutyMinContri.cs
// Last Edit: 2016/12/02 07:34
// Created: 2016/12/02 07:34

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// This packet is used to show the minimum donation a user needs to reach to claim a higher position on the guild.
    /// </summary>
    public sealed class MsgDutyMinContri : PacketStructure
    {
        public MsgDutyMinContri()
            : base(PacketType.MSG_DUTY_MIN_CONTRI, 24, 16)
        {
            
        }

        public MsgDutyMinContri(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public ushort Count
        {
            get { return ReadUShort(6); }
            set { WriteUShort(value, 6); }
        }

        public void Append(SyndicateRank pos, uint dwDonation)
        {
            int offset = 8+Count*8;
            Count += 1;
            Resize(24+Count*8);
            WriteHeader(Length-8, PacketType.MSG_DUTY_MIN_CONTRI);
            WriteUInt((uint) pos, offset);
            WriteUInt(dwDonation, offset + 4);
        }
    }
}