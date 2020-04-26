// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Rivest Cipher.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:53

using ServerCore.Common;

namespace ServerCore.Security
{
    /// <summary>
    /// RC5 is a block cipher notable for its simplicity. Designed by Ronald Rivest in 1994, RC stands for "Rivest 
    /// Cipher". A key feature of RC5 is the use of data-dependent rotations; one of the goals of RC5 was to prompt 
    /// the study and evaluation of such operations as a cryptographic primitive. 12-round RC5 (with 64-bit blocks) 
    /// is susceptible to a differential attack using 2^44 chosen plaintexts. NetDragon Websoft uses 12-round RC5 
    /// for their password cipher.
    /// </summary>
    public unsafe sealed class RivestCipher5
    {
        // Local-Scope Constants:
        private const int KEY_SIZE = 4;
        private const int SEED_SIZE = 16;
        private const int SUBSTITUTION_SIZE = 26;
        private const int BITS_SHIFTED = 32;

        // Local-Scope Initialization Vector:
        private static byte[] _initializationVector = { 
            0x3C, 0xDC, 0xFE, 0xE8, 0xC4, 0x54, 0xD6, 0x7E, 
	        0x16, 0xA6, 0xF8, 0x1A, 0xE8, 0xD0, 0x38, 0xBE 
        };

        // Local-Scope Variable Declarations:
        private uint[] _keyBuffer;              // The key buffer, used in encryption and decryption.
        private uint[] _substitutionBuffer;     // The substitution box, generated at class construction.

        /// <summary>
        /// RC5 is a block cipher notable for its simplicity. Designed by Ronald Rivest in 1994, RC stands for "Rivest 
        /// Cipher". A key feature of RC5 is the use of data-dependent rotations; one of the goals of RC5 was to prompt 
        /// the study and evaluation of such operations as a cryptographic primitive. 12-round RC5 (with 64-bit blocks) 
        /// is susceptible to a differential attack using 2^44 chosen plaintexts. NetDragon Websoft uses 12-round RC5 
        /// for their password cipher.
        /// </summary>
        public RivestCipher5()
        {
            GenerateKeys(_initializationVector);
        }

        /// <summary>
        /// RC5 is a block cipher notable for its simplicity. Designed by Ronald Rivest in 1994, RC stands for "Rivest 
        /// Cipher". A key feature of RC5 is the use of data-dependent rotations; one of the goals of RC5 was to prompt 
        /// the study and evaluation of such operations as a cryptographic primitive. 12-round RC5 (with 64-bit blocks) 
        /// is susceptible to a differential attack using 2^44 chosen plaintexts. NetDragon Websoft uses 12-round RC5 
        /// for their password cipher.
        /// </summary>
        /// <param name="seed">The seed sent to the client in packet 1060 (if patch 5174 and above).</param>
        public RivestCipher5(int seed)
        {
            GenerateKeys(SeedGenerator.Generate(seed));
        }

        /// <summary>
        /// This method generates keys for the algorithm. It should be used before the encryption and decryption
        /// methods are used to initialize the key vector. This method is automatically called by the constructor.
        /// </summary>
        /// <param name="initializationVector">The initialization vector used to generate keys.</param>
        public void GenerateKeys(byte[] initializationVector)
        {
            // Initialize Cipher Buffers:
            _keyBuffer = new uint[KEY_SIZE];
            _substitutionBuffer = new uint[SUBSTITUTION_SIZE];
            fixed (uint* keyBufferPtr = _keyBuffer)
                NativeFunctionCalls.memcpy((byte*)keyBufferPtr, initializationVector, KEY_SIZE * sizeof(uint));

            // Generate the substitution box:
            _substitutionBuffer[0] = 0xB7E15163;
            for (int index = 1; index < SUBSTITUTION_SIZE; index++)
                _substitutionBuffer[index] = _substitutionBuffer[index - 1] + 0x9E3779B9;

            // Generate Key & Final Substitution Box:
            uint substitutionIndex = 0, keyIndex = 0, x = 0, y = 0;
            for (int loopControlIndex = 0; loopControlIndex < 3 * SUBSTITUTION_SIZE; loopControlIndex++)
            {
                _substitutionBuffer[substitutionIndex] = RotateLeft(_substitutionBuffer[substitutionIndex] + x + y, 3);
                x = _substitutionBuffer[substitutionIndex];
                substitutionIndex = (substitutionIndex + 1) % SUBSTITUTION_SIZE;
                _keyBuffer[keyIndex] = RotateLeft(_keyBuffer[keyIndex] + x + y, (int)(x + y));
                y = _keyBuffer[keyIndex];
                keyIndex = (keyIndex + 1) % KEY_SIZE;
            }
        }

        /// <summary>
        /// This method decrypts the specified data using the algorithm. This method should be used in checking the
        /// inputted password from the client, encrypted and passed in the authentication request packet.
        /// </summary>
        /// <param name="input">The inputted buffer to be decrypted.</param>
        public bool Decrypt(byte[] input)
        {
            // Error check length:
            if (input.Length % 8 != 0) return false;

            // Prepare the buffer:
            uint* buffer = null;
            fixed (byte* ptr = input)
                buffer = (uint*)ptr;

            // Decrypt the buffer:
            int rounds = input.Length / 8;
            for (int index = 0; index < rounds; index++)
            {
                // Using the left and right ints in the block, calculate the resultant substitution:
                uint left = buffer[2 * index];
                uint right = buffer[(2 * index) + 1];
                for (int subIndex = 12; subIndex > 0; subIndex--)
                {
                    right = RotateRight(right - _substitutionBuffer[(2 * subIndex) + 1], (int)left) ^ left;
                    left = RotateRight(left - _substitutionBuffer[2 * subIndex], (int)right) ^ right;
                }
                uint resultLeft = left - _substitutionBuffer[0];
                uint resultRight = right - _substitutionBuffer[1];

                // Replace the data with the substituted data:
                buffer[2 * index] = resultLeft;
                buffer[(2 * index) + 1] = resultRight;
            }
            return true;
        }

        /// <summary>
        /// This method encrypts the specified data using the algorithm. This method should be used to check the 
        /// client's inputted password against the database password (if the hash algorithm is not in use). 
        /// Otherwise, this method should not be used unless checking the result of the decryption method.
        /// </summary>
        /// <param name="input">The inputted buffer to be encrypted.</param>
        public bool Encrypt(byte[] input)
        {
            // Error check length:
            if (input.Length % 8 != 0) return false;

            // Prepare the buffer:
            uint* buffer = null;
            fixed (byte* ptr = input)
                buffer = (uint*)ptr;

            // Decrypt the buffer:
            int rounds = input.Length / 8;
            for (int index = 0; index < rounds; index++)
            {
                // Using the left and right ints in the block, calculate the resultant substitution:
                uint left = buffer[2 * index] + _substitutionBuffer[0];
                uint right = buffer[(2 * index) + 1] + _substitutionBuffer[1];
                for (int subIndex = 1; subIndex <= 12; subIndex++)
                {
                    left = RotateLeft(left ^ right, (int)right) + _substitutionBuffer[2 * subIndex];
                    right = RotateLeft(right ^ left, (int)left) + _substitutionBuffer[(2 * subIndex) + 1];
                }

                // Replace the data with the substituted data:
                buffer[2 * index] = left;
                buffer[(2 * index) + 1] = right;
            }
            return true;
        }

        /// <summary>
        /// This method is used when generating keys and encryption and decrypting data. It accepts an unsigned 
        /// integer value, then takes a high value by shifting to the right by 32 - count % 32. It uses the high 
        /// value to bitwise or it to the low value (bitwise shift left by count % 32).
        /// </summary>
        /// <param name="value">The value being rotated.</param>
        /// <param name="count">The count to rotate by (total shifting)</param>
        public uint RotateLeft(uint value, int count)
        {
            count %= BITS_SHIFTED;
            uint high = value >> (BITS_SHIFTED - count);
            return (value << count) | high;
        }

        /// <summary>
        /// This method is used when generating keys and encryption and decrypting data. It accepts an unsigned 
        /// integer value, then takes a low value by shifting to the left by 32 - count % 32. It uses the low value 
        /// to bitwise or it to the high value (bitwise shift right by count % 32).
        /// </summary>
        /// <param name="value">The value being rotated.</param>
        /// <param name="count">The count to rotate by (total shifting)</param>
        public uint RotateRight(uint value, int count)
        {
            count %= BITS_SHIFTED;
            uint low = value << (BITS_SHIFTED - count);
            return (value >> count) | low;
        }
    }
}