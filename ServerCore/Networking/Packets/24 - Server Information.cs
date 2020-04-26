// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: FelipeVieira
// File Created by: Felipe Vieira Vendramini
// zfserver - Core - 24 - Server Information.cs
// File Created: 2015/09/03 08:03

namespace ServerCore.Networking.Packets
{
    public sealed class MsgServerInformation : PacketStructure
    {
        public MsgServerInformation(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public MsgServerInformation(ushort usMaxOnline, ushort usOnline, string szServerName, int nPort)
            : base(PacketType.LOGIN_REQUEST_SERVER_INFO, 36, 28)
        {
            MaxOnlinePlayers = usMaxOnline;
            OnlinePlayers = usOnline;
            ServerName = szServerName;
            GamePort = nPort;
        }

        public ushort MaxOnlinePlayers { get { return ReadUShort(4); } set { WriteUShort(value, 4); } }

        public ushort OnlinePlayers { get { return ReadUShort(6); } set { WriteUShort(value, 6); } }

        public string ServerName { get { return ReadString(9, 10); } set { WriteStringWithLength(value, 9); } }

        public int GamePort { get { return ReadInt(26); } set { WriteInt(value, 26); } }
    }
}
