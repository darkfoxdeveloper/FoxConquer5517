// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Whirlpool Hashing.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:53

using System;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;

namespace ServerCore.Security
{
    public static class WhirlpoolHash
    {
        public static string Hash(string message)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message))
                return string.Empty;

            //ASCIIEncoding encoding = new ASCIIEncoding();
            WhirlpoolDigest whirlpool = new WhirlpoolDigest();
            UTF8Encoding encoding = new UTF8Encoding();

            byte[] data = encoding.GetBytes(message);
            whirlpool.Reset();
            whirlpool.BlockUpdate(data, 0, data.Length);

            byte[] ret = new byte[whirlpool.GetDigestSize()];
            whirlpool.DoFinal(ret, 0);
            return ByteToString(ret);
        }

        private static string ByteToString(byte[] buffer)
        {
            string hex = BitConverter.ToString(buffer);
            return hex.Replace("-", "").ToLower();
        }
    }
}