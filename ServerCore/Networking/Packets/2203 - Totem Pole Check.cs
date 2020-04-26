namespace ServerCore.Networking.Packets
{
    public sealed class MsgTotemPole : PacketStructure
    {
        public MsgTotemPole()
            : base(PacketType.MSG_TOTEM_POLE, 52, 44)
        {

        }

        public MsgTotemPole(byte[] packet)
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

        public int TotalInscribed
        {
            get { return ReadInt(20); }
            set { WriteInt(value, 20); }
        }

        public uint SharedBattlepower
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint Enchantment
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public int EnchantmentExpirationDate
        {
            get { return ReadInt(32); }
            set { WriteInt(value, 32); }
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
                Resize((int)(44 + (value * 40) + 8));
                WriteHeader(Length - 8, PacketType.MSG_TOTEM_POLE);
                WriteUInt(value, 40);
            }
        }

        public void AppendItem(uint itemUid, uint position, string ownerName, uint staticId,
            byte plus, byte socketOne, byte socketTwo, uint battlePower, uint donation)
        {
            Count += 1;
            var offset = (ushort)(44 + (40 * (Count - 1)));
            WriteUInt(itemUid, offset);
            WriteUInt(position, offset + 4);
            WriteString(ownerName, 16, offset + 8);
            WriteUInt(staticId, offset + 24);
            WriteByte((byte)(staticId % 10), offset + 28);
            WriteByte(plus, offset + 29);
            WriteByte(socketOne, offset + 30);
            WriteByte(socketTwo, offset + 31);
            WriteUInt(battlePower, offset + 32);
            WriteUInt(donation, offset + 36);
        }
    }
}