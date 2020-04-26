// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: FelipeVieira
// File Created by: Felipe Vieira Vendramini
// zfserver - Core - 11 - Login Meeting.cs
// File Created: 2015/08/20 02:21

using ServerCore.Common;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// This packet contains the Welcome message to the Login and MsgServer. This message is random and set at the configuration file.
    /// It should be configured on both servers, since each one will have it's reply. The message offset should be set before sending
    /// the packet, otherwise the message will fail. This is the first packet on the connection between both servers.
    /// </summary>
    public sealed class MsgLoginSvAuthRequest : PacketStructure
    {
        /// <summary>
        /// Gets a built packet and deserialize it for data reading.
        /// </summary>
        /// <param name="pMsg">The buffer that will be read.</param>
        public MsgLoginSvAuthRequest(byte[] pMsg)
            : base(pMsg)
        {
            
        }

        /// <summary>
        /// Writes the message to the packet.
        /// </summary>
        /// <param name="helloMsg">The message should have 16 characters at max.</param>
        /// <param name="nMessageOffset">A value from 12 to 1000.</param>
        public MsgLoginSvAuthRequest(string helloMsg, int nMessageOffset = 48)
            : base(PacketType.LOGIN_AUTH_REQUEST, 1024, 1016)
        {
            for (int i = 4; i < Length; i++)
                if (i < 8 || i > 11)
                    WriteByte((byte) ThreadSafeRandom.RandGet(1, 254), i);

            MessageOffset = nMessageOffset;
            Message = helloMsg.Substring(0, _MESSAGE_LENGTH);
        }

        /// <summary>
        /// The offset which contains the welcome message to the server.
        /// </summary>
        public int MessageOffset
        {
            get { return ReadInt(8); }
            set { WriteInt(value, 8); }
        }

        /// <summary>
        /// The welcome message string.
        /// </summary>
        public string Message
        {
            get { return ReadString(ReadByte(MessageOffset), MessageOffset+1); }
            set { WriteStringWithLength(value, MessageOffset); }
        }

        /// <summary>
        /// The max length of the message.
        /// </summary>
        private const int _MESSAGE_LENGTH = 16;
    }
}