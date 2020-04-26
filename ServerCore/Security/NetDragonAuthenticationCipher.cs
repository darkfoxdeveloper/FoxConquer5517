// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - NetDragon Authentication Cipher.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:53

using ServerCore.Interfaces;

namespace ServerCore.Security
{
    /// <summary>
    /// This class encapsulates an implementation of NetDragon Websoft, Inc. and TQ Digital Entertainment's 
    /// asymmetric authentication cipher for Conquer Online that is used to encrypt and decrypt transactions 
    /// between the client and the server during packet transactions. The algorithm is short and linear, making 
    /// it very weak. The cipher is only used in the Account Server on patches 5018 and higher. It is a
    /// Feistel cipher design.
    /// </summary>
    public unsafe sealed class NetDragonAuthenticationCipher : ICipher
    {
        // Local-Scope Constants:
        private const int PRIMITIVE_ROOT = 0x13FA0F9D;          // The prime number for iv generation.
        private const int IV_GENERATION_SEED = 0x6D5C7962;      // The generation seed used for iv generation.
        private const int VECTOR_SIZE = 0x200;                  // The size of the IV and key vectors.
        private readonly static byte[] _iv;                     // The initialization vector.

        // Local-Scope Variable Declarations:
        private int _encryptionIncrementor;         // An incrementor for the amount of bytes encrypted.
        private int _decryptionIncrementor;         // An incrementor for the amount of bytes decrypted.
        private byte[] _keyVector;                  // The key vector used to decrypt game server packets (< 5018).
        private readonly object _decryptionLock;    // The lock for decrypting packets.
        private readonly object _encryptionLock;    // The lock for encrypting packets.

        /// <summary>
        /// This class encapsulates an implementation of NetDragon Websoft, Inc. and TQ Digital Entertainment's 
        /// asymmetric authentication cipher for Conquer Online that is used to encrypt and decrypt transactions 
        /// between the client and the server during packet transactions. The algorithm is short and linear, making 
        /// it very weak. The cipher is only used in the Account Server on patches 5018 and higher. It is a
        /// Feistel cipher design.
        /// </summary>
        public NetDragonAuthenticationCipher()
        {
            _encryptionIncrementor = 0;
            _decryptionIncrementor = 0;
            _encryptionLock = new object();
            _decryptionLock = new object();
        }

        // Static Constructor: This constructor is called upon the first use of the class and initializes the
        // algorithm used by the cipher. Keys are generated (since they are not unique to any given client).
        static NetDragonAuthenticationCipher()
        {
            // Initialize variables for generating the initialization vector:
            int primativeRootValue = PRIMITIVE_ROOT;
            int generationSeedValue = IV_GENERATION_SEED;
            byte* primativeRootPtr = (byte*)(&primativeRootValue);
            byte* generationSeedPtr = (byte*)(&generationSeedValue);

            // Generate the initialization vector:
            _iv = new byte[VECTOR_SIZE];
            for (int index = 0; index < 0x100; index++)
            {
                _iv[index] = primativeRootPtr[0];
                _iv[index + 0x100] = generationSeedPtr[0];
                primativeRootPtr[0] = (byte)((primativeRootPtr[1] + (byte)(primativeRootPtr[0]
                    * primativeRootPtr[2])) * primativeRootPtr[0] + primativeRootPtr[3]);
                generationSeedPtr[0] = (byte)((generationSeedPtr[1] - (byte)(generationSeedPtr[0]
                    * generationSeedPtr[2])) * generationSeedPtr[0] + generationSeedPtr[3]);
            }
        }

        public void KeySchedule(byte[] buffer) { throw new System.NotImplementedException(); }

        /// <summary>
        /// This method generates keys for the message server after decrypting the first packet and authenticating
        /// the client. Using the account's unique identification number and authentication code from the account 
        /// server, new keys for the cipher are generated.
        /// </summary>
        /// <param name="account">The account holder's unique identification number.</param>
        /// <param name="authentication">The authentication code from the account server.</param>
        public void GenerateKeys(int account, int authentication)
        {
            // Initialize generation variables:
            uint generationSeed = (uint)(((account + authentication) ^ 0x4321) ^ authentication);
            uint squaredSeed = generationSeed * generationSeed;
            byte* generationSeedPtr = (byte*)(&generationSeed);
            byte* squaredSeedPtr = (byte*)(&squaredSeed);

            // Generate the key vector:
            _keyVector = new byte[VECTOR_SIZE];
            for (int index = 0; index < 0x100; index++)
            {
                _keyVector[index] = (byte)(_iv[index] ^ generationSeedPtr[index % 4]);
                _keyVector[index + 0x100] = (byte)(_iv[index + 0x100] ^ squaredSeedPtr[index % 4]);
            }
        }

        /// <summary>
        /// This method accepts the packet buffer and the total length that will be decrypted in bytes. It then 
        /// decrypts the buffer using the algorithm defined in the function. It's used when receiving packets from 
        /// the client. The buffer is copied while being decrypted so that the original array is untouched.
        /// </summary>
        /// <param name="buffer">The buffer being decrypted.</param>
        /// <param name="length">The length being decrypted.</param>
        public byte[] Decrypt(byte[] buffer, int length)
        {
            // Decrypt the buffer. If the key vector is not null, use the key vector to decrypt packets; else, use
            // the initialization vector (generated in the constructor of the cipher class).
            byte[] result = new byte[length];
            if (_keyVector != null)
                lock (_decryptionLock)
                    for (int index = 0; index < length; index++)
                    {
                        result[index] = (byte)(buffer[index] ^ 0xAB);
                        result[index] = (byte)(result[index] >> 4 | result[index] << 4);
                        result[index] ^= (byte)(_keyVector[(byte)(_decryptionIncrementor & 0xFF)]);
                        result[index] ^= (byte)(_keyVector[(byte)(_decryptionIncrementor >> 8) + 0x100]);
                        _decryptionIncrementor++;
                    }
            else
                lock (_decryptionLock)
                    for (int index = 0; index < length; index++)
                    {
                        result[index] = (byte)(buffer[index] ^ 0xAB);
                        result[index] = (byte)(result[index] >> 4 | result[index] << 4);
                        result[index] ^= (byte)(_iv[(byte)(_decryptionIncrementor & 0xFF)]);
                        result[index] ^= (byte)(_iv[(byte)(_decryptionIncrementor >> 8) + 0x100]);
                        _decryptionIncrementor++;
                    }
            return result;
        }

        /// <summary>
        /// This method accepts the packet buffer and the total length that will be decrypted in bytes. It then 
        /// decrypts the buffer using the algorithm defined in the function. It's used when receiving packets from 
        /// the client. The buffer is decrypted into the packet array.
        /// </summary>
        /// <param name="packet">The pointed packet receiving new data to.</param>
        /// <param name="buffer">The encrypted buffer from the client's socket.</param>
        /// <param name="length">The length to decrypt bytes for.</param>
        /// <param name="offset">The position to begin writing data to in the pointed packet</param>
        public void Decrypt(byte[] packet, byte[] buffer, int length, int position)
        {
            // Decrypt the buffer. If the key vector is not null, use the key vector to decrypt packets; else, use
            // the initialization vector (generated in the constructor of the cipher class).
            if (_keyVector != null)
                lock (_decryptionLock)
                    for (int index = position; index < length + position; index++)
                    {
                        packet[index] = (byte)(buffer[index - position] ^ 0xAB);
                        packet[index] = (byte)(packet[index] >> 4 | packet[index] << 4);
                        packet[index] ^= (byte)(_keyVector[(byte)(_decryptionIncrementor & 0xFF)]);
                        packet[index] ^= (byte)(_keyVector[(byte)(_decryptionIncrementor >> 8) + 0x100]);
                        _decryptionIncrementor++;
                    }
            else
                lock (_decryptionLock)
                    for (int index = position; index < length + position; index++)
                    {
                        packet[index] = (byte)(buffer[index - position] ^ 0xAB);
                        packet[index] = (byte)(packet[index] >> 4 | packet[index] << 4);
                        packet[index] ^= (byte)(_iv[(byte)(_decryptionIncrementor & 0xFF)]);
                        packet[index] ^= (byte)(_iv[(byte)(_decryptionIncrementor >> 8) + 0x100]);
                        _decryptionIncrementor++;
                    }
        }

        /// <summary>
        /// This method encrypts a byte array to be sent to the client. The array passed is copied as the encryption 
        /// process takes place so that the original value is never altered.
        /// </summary>
        /// <param name="packet">The array of data being sent to the client as an encrypted packet.</param>
        /// <param name="length">The length to encrypt for.</param>
        public byte[] Encrypt(byte[] packet, int length)
        {
            // Encrypt the packet for the transaction:
            byte[] result = new byte[length];
            lock (_encryptionLock)
                for (int index = 0; index < length; index++)
                {
                    result[index] = (byte)(packet[index] ^ 0xAB);
                    result[index] = (byte)(result[index] >> 4 | result[index] << 4);
                    result[index] ^= (byte)(_iv[(byte)(_encryptionIncrementor & 0xFF)]);
                    result[index] ^= (byte)(_iv[(byte)(_encryptionIncrementor >> 8) + 0x100]);
                    _encryptionIncrementor++;
                }
            return result;
        }
    }
}