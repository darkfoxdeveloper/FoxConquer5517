// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1058 - Syndicate Donation.cs
// Last Edit: 2016/11/23 08:39
// Created: 2016/11/23 08:38
namespace ServerCore.Networking.Packets
{
    public sealed class MsgSynpOffer : PacketStructure
    {
        public MsgSynpOffer()
            : base(PacketType.MSG_SYNP_OFFER, 60, 52)
        {

        }

        public MsgSynpOffer(byte[] packet)
            : base(packet)
        {

        }

        public uint Flag
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public int MoneyDonation
        {
            get { return ReadInt(8); }
            set { WriteInt(value, 8); }
        }

        public uint EmoneyDonation
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint GuideDonation
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public int PkDonation
        {
            get { return ReadInt(20); }
            set { WriteInt(value, 20); }
        }

        public uint TotemDonation
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint RedRoseDonation
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public uint WhiteRoseDonation
        {
            get { return ReadUInt(32); }
            set { WriteUInt(value, 32); }
        }

        public uint OrchidDonation
        {
            get { return ReadUInt(36); }
            set { WriteUInt(value, 36); }
        }

        public uint TulipDonation
        {
            get { return ReadUInt(40); }
            set { WriteUInt(value, 40); }
        }

        public uint Exploits
        {
            get { return ReadUInt(44); }
            set { WriteUInt(value, 44); }
        }
    }
}