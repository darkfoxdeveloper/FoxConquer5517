// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Transfer Cipher.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:53

using System;
using System.Security.Cryptography;
using System.Text;

namespace ServerCore.Security
{
    /// <summary>
    /// This class encapsulates the project's transfer cipher. The transfer cipher encrypts and decrypts data sent
    /// across the network when transferring a player from the account server to the selected message server. It can
    /// be configured in the servers' configuration files and database tables. The cipher is protected by brute-force 
    /// attack protection by the socket system, but should be reconfigured regularly to protect the servers. Consider
    /// replacing the cipher algorithm entirely since this is open-source. Current implementation runs on Rivest 5.
    /// You can replace this entire system to use the database or server to server packets instead.
    /// </summary>
    public unsafe sealed class TransferCipher
    {
        // Global-Scope Variable Declaration:
        public static byte[] Key;
        public static byte[] Salt;

        // Local-Scope Compositions:
        private RivestCipher5 _rivest;

        /// <summary>
        /// This class encapsulates the project's transfer cipher. The transfer cipher encrypts and decrypts data sent
        /// across the network when transferring a player from the account server to the selected message server. It can
        /// be configured in the servers' configuration files and database tables. The cipher is protected by brute-force 
        /// attack protection by the socket system, but should be reconfigured regularly to protect the servers. Consider
        /// replacing the cipher algorithm entirely since this is open-source. Current implementation runs on Rivest 5.
        /// You can replace this entire system to use the database or server to server packets instead.
        /// </summary>
        /// <param name="unique">A unique identifier for the client transfer.</param>
        public TransferCipher(string unique)
        {
            // Create the hashed key using the Tiger Hash Algorithm:
            TigerHashAlgorithm tigerHash = new TigerHashAlgorithm();
            tigerHash.Hash(Encoding.ASCII.GetBytes(unique));
            tigerHash.Hash(Key);

            // Create the password:
            PasswordDeriveBytes password = new PasswordDeriveBytes(tigerHash.Final(0x10), Salt);
            _rivest = new RivestCipher5();
            _rivest.GenerateKeys(password.GetBytes(16));
        }

        /// <summary>
        /// This class encapsulates the project's transfer cipher. The transfer cipher encrypts and decrypts data sent
        /// across the network when transferring a player from the account server to the selected message server. It can
        /// be configured in the servers' configuration files and database tables. The cipher is protected by brute-force 
        /// attack protection by the socket system, but should be reconfigured regularly to protect the servers. Consider
        /// replacing the cipher algorithm entirely since this is open-source. Current implementation runs on Rivest 5.
        /// You can replace this entire system to use the database or server to server packets instead.
        /// </summary>
        /// <param name="key">The key from the server's configuration file.</param>
        /// <param name="salt">No idea</param>
        /// <param name="unique">A unique identifier for the client transfer.</param>
        public TransferCipher(string key, string salt, string unique)
        {
            // Create the hashed key using the Tiger Hash Algorithm:
            TigerHashAlgorithm tigerHash = new TigerHashAlgorithm();
            tigerHash.Hash(Encoding.ASCII.GetBytes(unique));
            tigerHash.Hash(Encoding.ASCII.GetBytes(key));

            // Create the password:
            PasswordDeriveBytes password = new PasswordDeriveBytes(tigerHash.Final(0x10), Encoding.ASCII.GetBytes(salt));
            _rivest = new RivestCipher5();
            _rivest.GenerateKeys(password.GetBytes(16));
        }

        /// <summary>
        /// This method encrypts the identity and authentication code from the account server to be sent to the message
        /// server thought the client authentication packet. Encrypts using Rivest Cipher 5.
        /// </summary>
        /// <param name="input">The input buffer.</param>
        public uint[] Encrypt(uint[] input)
        {
            // Initialize buffer using input:
            byte[] buffer = new byte[8];
            fixed (byte* ptr = buffer)
            {
                *(uint*)(ptr) = input[0];
                *(uint*)(ptr + 4) = input[1];
            }

            // Encrypt the buffer:
            _rivest.Encrypt(buffer);
            return new uint[2] { BitConverter.ToUInt32(buffer, 0), BitConverter.ToUInt32(buffer, 4) };
        }

        /// <summary>
        /// This method decrypts the identity and authentication code from client authentication packet, send by the
        /// client to the message server from the account server. Decrypts using Rivest Cipher 5.
        /// </summary>
        /// <param name="input">The input buffer.</param>
        public uint[] Decrypt(uint[] input)
        {
            // Initialize buffer using input:
            byte[] buffer = new byte[8];
            fixed (byte* ptr = buffer)
            {
                *(uint*)(ptr) = input[0];
                *(uint*)(ptr + 4) = input[1];
            }

            // Decrypt the buffer:
            _rivest.Decrypt(buffer);
            return new uint[2] { BitConverter.ToUInt32(buffer, 0), BitConverter.ToUInt32(buffer, 4) };
        }
    }
}