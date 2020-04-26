namespace ServerCore.Networking.Packets
{
    public class MsgRank : PacketStructure
    {
        public const uint RED_ROSE_TYPE = 0x1c9c382,
            WHITE_ROSE_TYPE = 0x1c9c3e6,
            ORCHIDS_TYPE = 0x1c9c44a,
            TULIPS_TYPE = 0x1c9c4ae;

        public MsgRank(byte[] pMsg)
            : base(pMsg)
        {

        }

        public MsgRank()
            : base(PacketType.MSG_RANK, 29, 21)
        {

        }

        public uint Mode
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint FlowerIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public ushort Unknown
        {
            get { return ReadUShort(12); }
            set { WriteUShort(value, 12); }
        }

        public ushort PageNumber
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        public ushort ObjectCount
        {
            get { return ReadUShort(16); }
            set
            {
                Resize(52 + value * 36);
                WriteHeader(Length - 8, PacketType.MSG_RANK);
                WriteUShort(value, 16);
            }
        }

        public void SetFlowers(string flowers)
        {
            Resize(29 + flowers.Length);
            WriteHeader(Length - 8, PacketType.MSG_FLOWER);
            WriteByte(1, 16);
            WriteString(flowers, flowers.Length, 18);
        }

        // according to sources, start writing the ranking data after 24
        public void AddUserData(uint rank, uint unknown0, uint flowers, uint unknown1)
        {

            WriteUInt(rank, 24);
            WriteUInt(0, 28);
            WriteUInt(flowers, 32);
            WriteUInt(0, 36);
            WriteUInt(2301694, 40);
        }

        public void AddToRanking(string szName)
        {
            try
            {
                ObjectCount += 1;
                int offset = 44 * ObjectCount;
                WriteString(szName, 16, offset);
                WriteString(szName, 16, offset + 20);
            }
            catch
            {

            }
        }
    }
}