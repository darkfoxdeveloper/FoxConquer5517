// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: FelipeVieira
// File Created by: Felipe Vieira Vendramini
// zfserver - Core - 13 - Login Response.cs
// File Created: 2015/08/20 02:21

using Core.Common.Enums;
using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// This packet is sent by the LoginServer to tell the MsgServer after a successfull connection or not.
    /// </summary>
    public sealed class LoginSvResponsePacket : PacketStructure
    {
        /// <summary>
        /// This constructor build the ready to send packet.
        /// </summary>
        /// <param name="pMsg">The reply that will be sent to the server.</param>
        public LoginSvResponsePacket(LoginServerResponse pMsg)
            : base(PacketType.LOGIN_COMPLETE_AUTHENTICATION, 24, 16)
        {
            Response = pMsg;
        }

        /// <summary>
        /// Gets a built packet and deserialize it for data reading.
        /// </summary>
        /// <param name="pMsg">The buffer that will be read.</param>
        public LoginSvResponsePacket(byte[] pMsg)
            : base(pMsg)
        {
            
        }

        /// <summary>
        /// The response to the message server, telling if it's connected or not.
        /// </summary>
        public LoginServerResponse Response
        {
            get { return (LoginServerResponse) ReadByte(8); }
            set { WriteByte((byte) value, 8); }
        }
    }
}
