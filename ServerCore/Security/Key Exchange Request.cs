// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Key Exchange Request.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:53

using ServerCore.Networking.Packets;

namespace ServerCore.Security
{
    /// <summary>
    /// This packet is sent to the client to request the Diffie-Hellman Key Exchange. The server's public key 
    /// and the client's initialization vectors for decryption and encryption are sent through this packet. 
    /// Random padding is added.
    /// </summary>
    public sealed class KeyExchangeRequest : PacketStructure
    {
        // Local-Scope Constant Declarations:
        private const int PADDING_LENGTH = 11;
        private const int JUNK_LENGTH = 12;
        private const int PRIMATIVE_ROOT_LENGTH = 128;
        private const int PRIMARY_KEY_LENGTH = 128;
        private const int GENERATOR_LENGTH = 2;

        /// <summary>
        /// This packet is sent to the client to request the Diffie-Hellman Key Exchange. The server's public key 
        /// and the client's initialization vectors for decryption and encryption are sent through this packet. 
        /// Random padding is added.
        /// </summary>
        /// <param name="key">The public key from the server to be sent to the client.</param>
        /// <param name="encryptionIV">The initialization vector for encrypting data.</param>
        /// <param name="decryptionIV">The initialization vector for decrypting data.</param>
        public KeyExchangeRequest(string key, byte[] encryptionIV, byte[] decryptionIV)
            : base(333)
        {
            WriteArray(NetDragonDHKeyExchange.RandomGenerator.NextBytes(PADDING_LENGTH), 0);
            WriteInt(Length - PADDING_LENGTH, 11);
            WriteInt(JUNK_LENGTH, 15);
            WriteArray(NetDragonDHKeyExchange.RandomGenerator.NextBytes(JUNK_LENGTH), 19);
            WriteInt(BlowfishCipher.BF_BLOCK_SIZE, 31);
            WriteInt(BlowfishCipher.BF_BLOCK_SIZE, 43);
            WriteInt(PRIMATIVE_ROOT_LENGTH, 55);
            WriteString(NetDragonDHKeyExchange.PRIMATIVE_ROOT, PRIMATIVE_ROOT_LENGTH, 59);
            WriteInt(GENERATOR_LENGTH, 187);
            WriteString(NetDragonDHKeyExchange.GENERATOR, GENERATOR_LENGTH, 191);
            WriteInt(PRIMARY_KEY_LENGTH, 193);
            WriteString(key, PRIMARY_KEY_LENGTH, 197);
        }
    }
}