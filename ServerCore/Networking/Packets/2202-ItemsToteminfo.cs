// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2202 - Items Totem info.cs
// Last Edit: 2016/11/23 09:38
// Created: 2016/11/23 09:38
namespace ServerCore.Networking.Packets
{
    public sealed class MsgWeaponsInfo : PacketStructure
    {
        public MsgWeaponsInfo()
            : base(PacketType.MSG_WEAPONS_INFO, 92, 84)
        {

        }

        public MsgWeaponsInfo(byte[] packet)
            : base(packet)
        {

        }

        public uint Type
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint BeginAt
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint EndAt
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint ArsenalType
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint TotalInscribed
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public uint SharedBattlePower
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint Enchantment
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public uint EnchantmentExpire
        {
            get { return ReadUInt(32); }
            set { WriteUInt(value, 32); }
        }

        public uint Donation
        {
            get { return ReadUInt(36); }
            set { WriteUInt(value, 36); }
        }

        public uint Count
        {
            get { return ReadUInt(40); }
            set
            {
                Resize((int)(44 + (40 * value) + 8));
                WriteHeader(Length - 8, PacketType.MSG_WEAPONS_INFO);
                WriteUInt(value, 40);
            }
        }

        public void AppendItem(uint itemid, uint position, string name, uint itemtype, byte quality,
            byte plus, byte socketone, byte sockettwo, uint battlepower, uint donation)
        {
            Count += 1;
            int offset = (int)(44 + (40 * (Count - 1)));
            WriteUInt(itemid, offset);
            WriteUInt(position, offset + 4);
            WriteString(name, 16, offset + 8);
            WriteUInt(itemtype, offset + 24);
            WriteByte(quality, offset + 28);
            WriteByte(plus, offset + 29);
            WriteByte(socketone, offset + 30);
            WriteByte(sockettwo, offset + 31);
            WriteUInt(battlepower, offset + 32);
            WriteUInt(donation, offset + 36);
        }
    }
}