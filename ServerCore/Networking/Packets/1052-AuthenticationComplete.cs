// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1052 - Authentication Complete.cs
// Last Edit: 2016/11/23 08:34
// Created: 2016/11/23 08:34
namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 1052. This packet is sent as a response to the account server's authentication reply to 
    /// complete the authentication process. This packet contains the client's identification data and the client's
    /// region and patch number. The client should be authenticated by the message server and connected with the
    /// map server.
    /// </summary>
    public sealed class MsgConnect : PacketStructure
    {
        /// <summary>
        /// Packet Type: 1052. This packet is sent as a response to the account server's authentication reply to 
        /// complete the authentication process. This packet contains the client's identification data and the client's
        /// region and patch number. The client should be authenticated by the message server and connected with the
        /// map server.
        /// </summary>
        public MsgConnect(byte[] packet)
            : base(packet) { }

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

        /// <summary> Offset 12 - The player's client patch number. </summary>
        public ushort Version
        {
            get { return ReadUShort(12); }
        }

        /// <summary> Offset 14 - The client's language setting. </summary>
        public string Language
        {
            get { return ReadString(2, 14).Trim('\0'); }
            set { WriteString(value, 2, 14); }
        }

        /// <summary> Offset 24 - The client's RES file key. </summary>
        public uint ResKey
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }
    }
}