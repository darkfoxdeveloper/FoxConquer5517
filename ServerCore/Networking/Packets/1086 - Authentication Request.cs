// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1086 - Authentication Request.cs
// Last Edit: 2016/11/23 08:40
// Created: 2016/11/23 08:40

using ServerCore.Security;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 1086. This packet is accepted by the Account Server as the first packet from the client. It 
    /// contains login information specified by the client, information such as the player's account name, encrypted 
    /// password, and requested message server. The packet should be handled by decrypting the password, checking it 
    /// with the database, and transferring the client to the message server (or rejecting access). The packet 
    /// will decrypt the password automatically.
    /// </summary>
    public sealed unsafe class MsgAccount : PacketStructure
    {
        /// <summary>
        /// Packet Type: 1086. This packet is accepted by the Account Server as the first packet from the client. It 
        /// contains login information specified by the client, information such as the player's account name, encrypted 
        /// password, and requested message server. The packet should be handled by decrypting the password, checking it 
        /// with the database, and transferring the client to the message server (or rejecting access). The packet 
        /// will decrypt the password automatically.
        /// </summary>
        /// <param name="packet">The received packet.</param>
        /// <param name="seed">The seed used for Rivest Cipher 5.</param>
        public MsgAccount(byte[] packet, int seed)
            : base(packet)
        {
            // Initialize Decryption Algorithms:
            var rivestCipher = new RivestCipher5(seed);
            var netdragonCipher = new NetDragonPasswordCipher(Account);
            byte[] encrypted = ReadArray(16, 132);

            // Decrypt Password:
            rivestCipher.Decrypt(encrypted);
            netdragonCipher.Decrypt(encrypted);
            fixed (byte* decrypted = encrypted)
                Password = new string((sbyte*)decrypted);
        }

        // Packet Structure:
        public string Account
        {
            get { return ReadString(16, 4); }
        }
        public string Password
        {
            private set { WriteString(value, 16, 132); }
            get { return ReadString(16, 132); }
        }
        public string Server
        {
            get { return ReadString(16, 260); }
        }
    }
}