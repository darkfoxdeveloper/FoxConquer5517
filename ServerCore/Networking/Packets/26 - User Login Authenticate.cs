// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - Core - 26 - User Login Authenticate.cs
// File Created: 2015/09/03 17:53

using ServerCore.Common;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgUsrLogin : PacketStructure
    {
        private const uint _ADDICTION_U = 1000000000;

        public MsgUsrLogin(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public MsgUsrLogin(uint dwUserId, uint dwPacketHash)
            : base(PacketType.LOGIN_REQUEST_USER_SIGNIN, 72, 64)
        {
            RequestTime = (uint) UnixTimestamp.Timestamp();
            UserIdentity = dwUserId;
            PacketHash = dwPacketHash;

            for (int i = 36; i < 64; i++)
                WriteByte((byte) ThreadSafeRandom.RandGet(1, 255), i);
        }

        public uint RequestTime
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint UserIdentity
        {
            get { return ReadUInt(8) - _ADDICTION_U; }
            set { WriteUInt(value + _ADDICTION_U, 8); }
        }

        public uint PacketHash
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public byte VipLevel
        {
            get { return ReadByte(16); }
            set { WriteByte(value, 16); }
        }

        public byte Authority
        {
            get { return ReadByte(17); }
            set { WriteByte(value, 17); }
        }

        public string IpAddress
        {
            get { return ReadString(ReadByte(19), 20); }
            set { WriteStringWithLength(value, 19); }
        }

        public string MacAddress
        {
            get { return ReadString(ReadByte(39), 40); }
            set { WriteStringWithLength(value, 39); }
        }
    }
}