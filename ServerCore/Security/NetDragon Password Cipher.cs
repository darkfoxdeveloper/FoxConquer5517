// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - NetDragon Password Cipher.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:53

using ServerCore.Common;

namespace ServerCore.Security
{
    /// <summary>
    /// This cipher is used in the Account Server to decrypt the output of the decryption method for Rivest
    /// Cipher 5. The algorithm uses the account name for the player as a key for generating the key vector.
    /// This is the last cipher required for decrypting the player's password. The cipher algorithm does not
    /// work with num keys. Do not attempt to fix unless your players cannot have special symbols in their
    /// passwords.
    /// </summary>
    public sealed class NetDragonPasswordCipher
    {
        // Local-Scope Constants:
        private const int VECTOR_SIZE = 0x200;  // The size of the IV and key vectors.

        // Local-Scope Substitution Boxes:
        #region Substitution Boxes
        private static readonly byte[] _decryptionSubstitutionBox = {
             0, 0x1b, 0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 
            0x39, 0x30, 0xbd, 0xbb, 8, 9, 0x51, 0x57, 0x45, 0x52, 
            0x54, 0x59, 0x55, 0x49, 0x4f, 80, 0xdb, 0xdd, 13, 0x11, 
            0x41, 0x53, 0x44, 70, 0x47, 0x48, 0x4a, 0x4b, 0x4c, 0xba, 
            0xc0, 0xdf, 0x10, 0xde, 90, 0x58, 0x43, 0x56, 0x42, 0x4e, 
            0x4d, 0xbc, 190, 0xbf, 0x10, 0x6a, 0x12, 0x20, 20, 0x70, 
            0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 120, 0x79, 0x90, 
            0x91, 0x24, 0x26, 0x21, 0x6d, 0x25, 12, 0x27, 0x6b, 0x23, 
            40, 0x22, 0x2d, 0x2e, 0x2c, 0, 220, 0x7a, 0x7b, 12, 
            0xee, 0xf1, 0xea, 0xf9, 0xf5, 0xf3, 0, 0, 0xfb, 0x2f, 
            0x7c, 0x7d, 0x7e, 0x7f, 0x80, 0x81, 130, 0x83, 0x84, 0x85, 
            0x86, 0xed, 0, 0xe9, 0, 0xc1, 0, 0, 0x87, 0, 
            0, 0, 0, 0xeb, 9, 0, 0xc2, 0
        };
        private static readonly byte[] _encryptionSubstitutionBox = {
            0, 0, 0, 70, 0, 0, 0, 0, 14, 15, 0, 0, 0x4c, 0x1c, 0, 0, 
            0x2a, 0x1d, 0x38, 0, 0x3a, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 
            0x39, 0x49, 0x51, 0x4f, 0x47, 0x4b, 0x48, 0x4d, 80, 0, 0, 0, 0x54, 0x52, 0x53, 0x63, 
            11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0, 0, 0, 0, 
            0, 30, 0x30, 0x2e, 0x20, 0x12, 0x21, 0x22, 0x23, 0x17, 0x24, 0x25, 0x26, 50, 0x31, 0x18, 
            0x19, 0x10, 0x13, 0x1f, 20, 0x16, 0x2f, 0x11, 0x2d, 0x15, 0x2c, 0x5b, 0x5c, 0x5d, 0, 0x5f, 
            0x52, 0x4f, 80, 0x51, 0x4b, 0x4c, 0x4d, 0x47, 0x48, 0x49, 0x37, 0x4e, 0, 0x4a, 0x53, 0x35, 
            0x3b, 60, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44, 0x57, 0x58, 100, 0x65, 0x66, 0x67, 
            0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 110, 0x76, 0, 0, 0, 0, 0, 0, 0, 0, 
            0x45, 70, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0x2a, 0x36, 0x1d, 0x1d, 0x38, 0x38, 0x6a, 0x69, 0x67, 0x68, 0x65, 0x66, 50, 0x20, 0x2e, 0x30, 
            0x19, 0x10, 0x24, 0x22, 0x6c, 0x6d, 0x6b, 0x21, 0, 0, 0x27, 13, 0x33, 12, 0x34, 0x35, 
            40, 0x73, 0x7e, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x1a, 0x56, 0x1b, 0x2b, 0x29, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0x71, 0x5c, 0x7b, 0, 0x6f, 90, 0, 
            0, 0x5b, 0, 0x5f, 0, 0x5e, 0, 0, 0, 0x5d, 0, 0x62, 0, 0, 0, 0
        };
        #endregion

        // Local-Scope Variable Declarations:
        private byte[] _keyBuffer;  // The key buffer, used in encryption and decryption.

        /// <summary>
        /// This cipher is used in the Account Server to decrypt the output of the decryption method for Rivest
        /// Cipher 5. The algorithm uses the account name for the player as a key for generating the key vector.
        /// This is the last cipher required for decrypting the player's password. The cipher algorithm does not
        /// work with num keys. Do not attempt to fix unless your players cannot have special symbols in their
        /// passwords.
        /// </summary>
        /// <param name="account">The player's account name.</param>
        public NetDragonPasswordCipher(string account)
        {
            // Generate the seed for the key generator:
            int seed = 0;
            foreach (byte character in account)
                seed += character;
            GenerateKeys(SeedGenerator.Generate(seed));
        }

        /// <summary>
        /// This method generates keys for the algorithm. It should be used before the encryption and decryption
        /// methods are used to initialize the key vector. This method is automatically called by the constructor.
        /// </summary>
        /// <param name="initializationVector">The initialization vector used to generate keys.</param>
        public void GenerateKeys(byte[] initializationVector)
        {
            // Initialize the key vector:
            _keyBuffer = new byte[VECTOR_SIZE];
            for (int index = 1; index < VECTOR_SIZE / 2; index++)
            {
                _keyBuffer[index * 2] = (byte)index;
                _keyBuffer[(index * 2) + 1] = (byte)(index ^ initializationVector[index & 15]);
            }

            // Generate the key vector:
            for (int k = 1; k < VECTOR_SIZE / 2; k++)
                for (int m = k + 1; m < VECTOR_SIZE / 2; m++)
                    if (_keyBuffer[(k * 2) + 1] < _keyBuffer[(m * 2) + 1])
                    {
                        _keyBuffer[k * 2] = (byte)(_keyBuffer[k * 2] ^ _keyBuffer[m * 2]);
                        _keyBuffer[m * 2] = (byte)(_keyBuffer[m * 2] ^ _keyBuffer[k * 2]);
                        _keyBuffer[k * 2] = (byte)(_keyBuffer[k * 2] ^ _keyBuffer[m * 2]);
                        _keyBuffer[(k * 2) + 1] = (byte)(_keyBuffer[(k * 2) + 1] ^ _keyBuffer[(m * 2) + 1]);
                        _keyBuffer[(m * 2) + 1] = (byte)(_keyBuffer[(m * 2) + 1] ^ _keyBuffer[(k * 2) + 1]);
                        _keyBuffer[(k * 2) + 1] = (byte)(_keyBuffer[(k * 2) + 1] ^ _keyBuffer[(m * 2) + 1]);
                    }
        }

        /// <summary>
        /// This method decrypts the specified data using the algorithm. This method should be used in checking the
        /// inputted password from the client, encrypted and passed in the authentication request packet.
        /// </summary>
        /// <param name="input">The inputted buffer to be decrypted.</param>
        public unsafe void Decrypt(byte[] input)
        {
            for (int index = 0; index < input.Length; index++)
            {
                // Is the algorithm done decrypting data?
                if (input[index] == 0)
                {
                    fixed (byte* ptr = input)
                        NativeFunctionCalls.memset(ptr + index, 0, input.Length - index);
                    return;
                }

                // Decrypt the byte at the current offset:
                byte position = _keyBuffer[input[index] * 2];
                if (position > 0x80)
                {
                    // Entered using the "SHIFT" key:
                    position = (byte)(_keyBuffer[input[index] * 2] - 0x80);
                    input[index] = _decryptionSubstitutionBox[position];
                }
                else
                {
                    input[index] = _decryptionSubstitutionBox[position];
                    if (input[index] >= 0x41 && input[index] <= 90)
                        input[index] = (byte)(input[index] + 0x20);
                }
            }
        }

        /// <summary>
        /// This method encrypts the specified data using the algorithm. This method should be used to check the 
        /// client's inputted password against the database password (if the hash algorithm is not in use). 
        /// Otherwise, this method should not be used unless checking the result of the decryption method.
        /// </summary>
        /// <param name="input">The inputted buffer to be decrypted.</param>
        public unsafe void Encrypt(byte[] input)
        {
            for (int index = 0; index < input.Length; index++)
            {
                // Decrypt the byte at the current offset:
                byte position = input[index];
                if (position >= 0x61 && position <= 0x7A)
                    input[index] = (byte)(input[index] - 0x20);
                byte vectorPosition = _encryptionSubstitutionBox[input[index]];
                if (vectorPosition >= 0x41 && vectorPosition <= 90)
                    vectorPosition = (byte)(vectorPosition + 0x80);
                for (byte j = 0; j <= 0xff; j = (byte)(j + 1))
                    if (_keyBuffer[j * 2] == vectorPosition)
                    {
                        input[index] = j;
                        break;
                    }
            }
        }
    }
}
