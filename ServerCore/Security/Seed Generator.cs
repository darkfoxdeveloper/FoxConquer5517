// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Seed Generator.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:53
namespace ServerCore.Security
{
    public static class SeedGenerator
    {
        /// <summary>
        /// This function encapsulates a seed generator for Rivest Cipher 5 and NetDragon Websoft's Password Cipher 
        /// addition. It takes in a seed and spits out a 16 byte array. The returned array is used as the ciphers'
        /// initialization vector.
        /// </summary>
        /// <param name="seed">The seed being used to initialize the initialization vector.</param>
        public static byte[] Generate(int seed)
        {
            byte[] initializationVector = new byte[0x10];
            for (int index = 0; index < 0x10; index++)
            {
                seed *= 0x343fd;
                seed += 0x269ec3;
                initializationVector[index] = (byte)((seed >> 0x10) & 0x7fff);
            }
            return initializationVector;
        }
    }
}
