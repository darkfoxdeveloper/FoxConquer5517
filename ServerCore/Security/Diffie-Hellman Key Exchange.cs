// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Diffie-Hellman Key Exchange.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:53

using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace ServerCore.Security
{
    /// <summary>
    /// The Diffie–Hellman key exchange method allows two parties that have no prior knowledge of each 
    /// other to jointly establish a shared secret key over an insecure communications channel. This 
    /// key can then be used to encrypt subsequent communications using a symmetric key cipher.
    /// </summary>
    public unsafe class DiffieHellmanKeyExchange
    {
        // Exchange Variables:
        public BigInteger SecretKey;                    // The secret number computed by the exchange.
        private BigInteger _primeRoot;                  // Prime number used to configure the exchange on both sides.
        private BigInteger _generator;                  // Generating number used to configure the exchange on both sides.
        private static BigInteger _requestModInteger;   // The request's random integer used to mod the request key.
        private BigInteger _publicRequestKey;           // The request key being sent to the client.
        private BigInteger _publicResponseKey;          // The response key being received from the client.

        /// <summary>
        /// The Diffie–Hellman key exchange method allows two parties that have no prior knowledge of each 
        /// other to jointly establish a shared secret key over an insecure communications channel. This 
        /// key can then be used to encrypt subsequent communications using a symmetric key cipher.
        /// </summary>
        /// <param name="p">A prime number being used to configure the exchange on both sides.</param>
        /// <param name="g">A generating number being used to configure the exchange on both sides.</param>
        public DiffieHellmanKeyExchange(string p, string g)
        {
            _primeRoot = new BigInteger(p, 16);
            _generator = new BigInteger(g, 16);
        }

        /// <summary>
        /// This method generates the private and public key used in computing the shared secret key.
        /// If the keys have already been generated, this method will not execute and throw an exception.
        /// </summary>
        public string GenerateRequest()
        {
            // Error Check:
            if (_publicRequestKey != null)
                throw new MethodAccessException();

            // Assign Local Variables:
            BigInteger request = _generator;

            // Generate Request:
            _requestModInteger = BigInteger.ProbablePrime(256, new Random());
            request = request.ModPow(_requestModInteger, _primeRoot);
            _publicRequestKey = request;

            // Return the request key:
            return Hex.ToHexString(_publicRequestKey.ToByteArrayUnsigned());
        }

        /// <summary>
        /// This method handles the response from the client and generates the response key for the Blowfish
        /// cipher algorithm. It returns the key for Blowfish to schedule keys.
        /// </summary>
        /// <param name="publicKey">The public key from the client.</param>
        public byte[] GenerateResponse(string publicKey)
        {
            // Finish receiving the public key:
            _publicResponseKey = new BigInteger(publicKey, 16);

            // Generate the secret key:
            BigInteger response = _publicResponseKey;
            response = response.ModPow(_requestModInteger, _primeRoot);
            SecretKey = response;

            // Return result:
            return SecretKey.ToByteArrayUnsigned();
        }
    }
}