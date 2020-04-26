// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: FelipeVieira
// File Created by: Felipe Vieira Vendramini
// zfserver - Core - 12 - Login Authentication.cs
// File Created: 2015/08/20 02:22

using ServerCore.Common;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// This packet is sent by the MsgServer after receiving the Welcome message confirming the connection. After that, this packet
    /// will send the server information to the Login Server. The Username and Password are required to authenticate the servers
    /// and are filled in the Configuration files also. If the server doesn't meet the requirements, then the LoginServer will
    /// reply with the packet 13.
    /// </summary>
    public sealed class MsgLoginSvAuthentication : PacketStructure
    {
        /// <summary>
        /// This constructor builds the packet ready to be sent.
        /// </summary>
        /// <param name="szName">The username that will authenticate the Msg Server with the Login Server.</param>
        /// <param name="szPass">The password that will authenticate the Msg Server with the Login Server.</param>
        /// <param name="szServerName">The server name, so the LoginServer will be enabled to redirect the player to the right game server.</param>
        /// <param name="usMaxOnline">The max amount of players enabled on this server.</param>
        public MsgLoginSvAuthentication(string szName, string szPass, string szServerName, ushort usMaxOnline)
            : base(PacketType.LOGIN_AUTH_CONFIRM, 512, 504)
        {
            for (int i = 4; i < Length; i++)
                WriteByte((byte)ThreadSafeRandom.RandGet(1, 254), i);

            Username = szName.Substring(0, szName.Length > _SERVER_NAME_MAX_LENGTH ? _USERNAME_MAX_LENGTH - 1 : szName.Length);
            Password = szPass.Substring(0, szPass.Length > _PASSWORD_MAX_LENGTH ? _PASSWORD_MAX_LENGTH - 1 : szPass.Length);
            ServerName = szServerName.Substring(0, szServerName.Length > _PASSWORD_MAX_LENGTH ? _PASSWORD_MAX_LENGTH - 1 : szServerName.Length);
            MaxOnlinePlayers = usMaxOnline;
        }

        /// <summary>
        /// Gets a built packet and deserialize it for data reading.
        /// </summary>
        /// <param name="pMsg">The buffer that will be read.</param>
        public MsgLoginSvAuthentication(byte[] pMsg)
            : base(pMsg)
        {
            
        }

        public string ServerName
        {
            get { return ReadString(ReadByte(16), 17); }
            set { WriteStringWithLength(value, 16); }
        }

        public string Username
        {
            get { return ReadString(ReadByte(112), 113); }
            set { WriteStringWithLength(value, 112); }
        }

        public string Password
        {
            get { return ReadString(ReadByte(225), 226); }
            set { WriteStringWithLength(value, 225); }
        }

        public ushort MaxOnlinePlayers
        {
            get { return ReadUShort(500); }
            set { WriteUShort(value < _SERVER_MAX_ONLINE_PLAYERS ? value : _SERVER_MAX_ONLINE_PLAYERS, 500); }
        }

        private const int _USERNAME_MAX_LENGTH = 16;
        private const int _PASSWORD_MAX_LENGTH = 16;
        private const int _SERVER_NAME_MAX_LENGTH = 16;
        private const ushort _SERVER_MAX_ONLINE_PLAYERS = 5000;
    }
}
