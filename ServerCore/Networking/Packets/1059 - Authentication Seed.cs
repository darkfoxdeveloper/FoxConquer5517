// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1059 - Authentication Seed.cs
// Last Edit: 2016/11/23 08:39
// Created: 2016/11/23 08:39
namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 1059. This packet is sent to initialize the password cipher in the client. The server sends
    /// the player's seed for Rivest Cipher 5 so the client can encrypt the player's inputted password. This packet
    /// is only used in patches 5174 and above.
    /// </summary>
    public sealed class MsgEncryptCode : PacketStructure
    {
        /// <summary>
        /// Packet Type: 1059. This packet is sent to initialize the password cipher in the client. The server sends
        /// the player's seed for Rivest Cipher 5 so the client can encrypt the player's inputted password. This packet
        /// is only used in patches 5174 and above.
        /// </summary>
        /// <param name="seed">The seed to be sent to the client and used in Rivest Cipher 5.</param>
        public MsgEncryptCode(int seed)
            : base(8)
        {
            WriteHeader(8, PacketType.MSG_ENCRYPT_CODE);
            WriteInt(seed, 4);
        }

        /// <summary> Offset 4 - The random seed being sent to the client to initialize password ciphers. </summary>
        public int Seed
        {
            get { return ReadInt(4); }
            set { WriteInt(value, 4); }
        }
    }
}