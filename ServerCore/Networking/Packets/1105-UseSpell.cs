namespace ServerCore.Networking.Packets
{
    public sealed class MsgMagicEffect : PacketStructure
    {
        public MsgMagicEffect()
            : base(PacketType.MSG_MAGIC_EFFECT, 68, 60)
        {

        }

        public MsgMagicEffect(byte[] packet)
            : base(packet)
        {

        }

        /// <summary>
        /// The entity that is using the magic.
        /// </summary>
        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public ushort CellX
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        public ushort CellY
        {
            get { return ReadUShort(10); }
            set { WriteUShort(value, 10); }
        }

        public ushort SkillIdentity
        {
            get { return ReadUShort(12); }
            set { WriteUShort(value, 12); }
        }

        public ushort SkillLevel
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        public uint TargetCount
        {
            get { return ReadUInt(16); }
            set
            {
                Resize((int)(60 + (value * 32) + 8));
                WriteHeader(Length - 8, PacketType.MSG_MAGIC_EFFECT);
                WriteUInt(value, 16);
            }
        }

        public void AppendTarget(uint target, uint damage, bool hit, uint actType, uint actValue)
        {
            TargetCount += 1;
            var offset = (ushort)(20 + (TargetCount - 1) * 32);
            WriteUInt(target, offset);
            WriteUInt(damage, offset + 4);
            WriteBoolean(hit, offset + 8);
            WriteUInt(actType, offset + 12);
            WriteUInt(actValue, offset + 16);
            // todo verify activation types
        }
    }
}