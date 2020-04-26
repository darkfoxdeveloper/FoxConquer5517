using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgInteract : PacketStructure
    {
        public MsgInteract()
            : base(PacketType.MSG_INTERACT, 48, 40)
        {

        }

        public MsgInteract(byte[] packet)
            : base(packet)
        {

        }

        public uint Timestamp
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint EntityIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint TargetIdentity
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public ushort CellX
        {
            get { return ReadUShort(16); }
            set { WriteUShort(value, 16); }
        }

        public ushort CellY
        {
            get { return ReadUShort(18); }
            set { WriteUShort(value, 18); }
        }

        public InteractionType Action
        {
            get { return (InteractionType)ReadUInt(20); }
            set { WriteUInt((uint)value, 20); }
        }

        public uint Data
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public ushort Damage
        {
            get { return (ushort)Data; }
            set { Data = (uint)((KoCount << 16) | value); }
        }
        public ushort KoCount
        {
            get { return (ushort)(Data >> 16); }
            set { Data = (uint)((value << 16) | Damage); }
        }
        public ushort MagicType
        {
            get { return (ushort)Data; }
            set { Data = (uint)((MagicLevel << 16) | value); }
        }

        public ushort MagicLevel
        {
            get { return (ushort)(Data >> 16); }
            set { Data = (uint)((value << 16) | MagicType); }
        }

        public uint Amount
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public InteractionEffect ActivationType
        {
            get { return (InteractionEffect)ReadByte(32); }
            set { WriteByte((byte)value, 32); }
        }

        public uint ActivationValue
        {
            get { return ReadUInt(36); }
            set { WriteUInt(value, 36); }
        }
    }
}