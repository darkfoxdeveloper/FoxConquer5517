namespace ServerCore.Networking.Packets
{
    public enum CompositionMode : uint
    {
        CMP_ITEM_PLUS = 512,
        CMP_STEED_PLUS = 514,
        CMP_STEED_PLUS_NEW = 515
    }

    public sealed class MsgDataArray : PacketStructure
    {
        public MsgDataArray()
            : base(PacketType.MSG_DATA_ARRAY, 24, 16)
        {

        }

        public MsgDataArray(byte[] packet)
            : base(packet)
        {

        }

        public CompositionMode Mode
        {
            get { return (CompositionMode)ReadUInt(4); }
            set { WriteUInt((uint)value, 4); }
        }

        public uint MainIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint MinorIdentity
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }
    }
}