// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - ServerCore - 1150 - Flower.cs
// Last Edit: 2017/02/15 18:13
// Created: 2017/02/04 14:38

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgFlower : PacketStructure
    {
        public MsgFlower()
            : base(PacketType.MSG_FLOWER, 64, 56)
        {

        }

        public MsgFlower(byte[] packet)
            : base(packet)
        {

        }

        // I guess it's the mode... I will check out when really building this
        public uint Mode
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Identity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint ItemIdentity
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint Flower
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public string Sender
        {
            get { return ReadString(16, 16); }
            set { WriteString(value, 16, 16); }
        }

        public uint RedRoses
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint Amount
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public uint RedRosesToday
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public FlowerType FlowerType
        {
            get { return (FlowerType) ReadUInt(24); }
            set { WriteUInt((uint) value, 24); }
        }

        public uint WhiteRoses
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint WhiteRosesToday
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public string Receptor
        {
            get { return ReadString(16, 32); }
            set { WriteString(value, 16, 32); }
        }

        public uint Orchids
        {
            get { return ReadUInt(32); }
            set { WriteUInt(value, 32); }
        }

        public uint OrchidsToday
        {
            get { return ReadUInt(36); }
            set { WriteUInt(value, 36); }
        }

        public uint Tulips
        {
            get { return ReadUInt(40); }
            set { WriteUInt(value, 40); }
        }

        public uint TulipsToday
        {
            get { return ReadUInt(44); }
            set { WriteUInt(value, 44); }
        }

        public uint SendAmount
        {
            get { return ReadUInt(48); }
            set { WriteUInt(value, 48); }
        }

        public FlowerType SendFlowerType
        {
            get { return (FlowerType)ReadUInt(52); }
            set { WriteUInt((uint)value, 52); }
        }

        public uint Unknown56
        {
            get { return ReadUInt(56); }
            set { WriteUInt(value, 56); }
        }
    }
}