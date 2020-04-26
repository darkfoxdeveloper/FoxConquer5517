using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public class MsgGuide : PacketStructure
    {
        public MsgGuide()
            : base(PacketType.MSG_GUIDE, 32, 24)
        {

        }

        public MsgGuide(byte[] pBuffer)
            : base(pBuffer)
        {

        }

        public MentorRequest Type
        {
            get { return (MentorRequest)ReadUShort(4); }
            set { WriteUShort((ushort)value, 4); }
        }

        public uint Identity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint Param
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public byte Dynamic
        {
            get { return ReadByte(16); }
            set { WriteByte(value, 16); }
        }

        public bool Online
        {
            get { return ReadBoolean(20); }
            set { WriteBoolean(value, 20); }
        }

        public string Name
        {
            get { return ReadString(ReadByte(21), 22); }
            set
            {
                Resize(32 + value.Length);
                WriteHeader(Length - 8, PacketType.MSG_GUIDE);
                WriteStringWithLength(value, 21);
            }
        }
    }
}