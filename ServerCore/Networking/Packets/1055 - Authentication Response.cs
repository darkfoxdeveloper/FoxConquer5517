using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 1055. This packet is sent as a response to the previous authentication packet sent to request
    /// access to a server. The packet is sent to reject a client or authorize a client. If the client is being 
    /// rejected, the rejection error identity will be sent in the packet. If the client is being authenticated, 
    /// the message server's connection information will be sent in the packet.
    /// </summary>
    public sealed class MsgConnectEx : PacketStructure
    {
        /// <summary>
        /// Packet Type: 1055. This packet is sent as a response to the previous authentication packet sent to request
        /// access to a server. The packet is sent to reject a client or authorize a client. If the client is being 
        /// rejected, the rejection error identity will be sent in the packet. If the client is being authenticated, 
        /// the message server's connection information will be sent in the packet.
        /// </summary>
        /// <param name="type">The rejection message being sent to the client.</param>
        public MsgConnectEx(RejectionType type)
            : base(32)
        {
            WriteHeader(32, PacketType.MSG_CONNECT_EX);
            Authentication = (uint)type;
        }

        /// <summary>
        /// Packet Type: 1055. This packet is sent as a response to the previous authentication packet sent to request
        /// access to a server. The packet is sent to reject a client or authorize a client. If the client is being 
        /// rejected, the rejection error identity will be sent in the packet. If the client is being authenticated, 
        /// the message server's connection information will be sent in the packet.
        /// </summary>
        /// <param name="identity">The account's global unique identity.</param>
        /// <param name="authentication">The authority level of the client.</param>
        /// <param name="ip">The IP address of the message server.</param>
        /// <param name="port">The port of the message server.</param>
        public MsgConnectEx(uint identity, uint authentication, string ip, int port)
            : base(32)
        {
            WriteHeader(32, PacketType.MSG_CONNECT_EX);
            Identity = identity;
            Authentication = authentication;
            IPAddress = ip;
            Port = port;
        }

        /// <summary> Offset 4 - The player's unique identification number. </summary>
        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        /// <summary> Offset 8 - The player's authentication code. </summary>
        public uint Authentication
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        /// <summary> Offset 12 - The message server's port. </summary>
        public int Port
        {
            get { return ReadInt(12); }
            set { WriteInt(value, 12); }
        }

        public uint Hash
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        /// <summary> Offset 20 - The IP address of the message server. </summary>
        public string IPAddress
        {
            get { return ReadString(20, 12); }
            set { WriteString(value, 20, 12); }
        }
    }
}
