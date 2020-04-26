// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - NetDragon Key Exchange.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:53

using ServerCore.Common;

namespace ServerCore.Security
{
    public class NetDragonDHKeyExchange : DiffieHellmanKeyExchange
    {
        // Global-Scope Constant Declarations:
        public const string PRIMATIVE_ROOT = "E7A69EBDF105F2A6BBDEAD7E798F76A209AD73FB466431E2E7352ED262F8C558F10BEFEA977DE9E21DCEE9B04D245F300ECCBBA03E72630556D011023F9E857F";
        public const string GENERATOR = "05";

        // Global-Scope Variable Declarations:
        public static ConcurrentRandom RandomGenerator;

        // Local-Scope Variable Declarations:
        private static string _serverRequestKey;
        private byte[] _encryptionIv;
        private byte[] _decryptionIv;

        /// <summary>
        /// The Diffie–Hellman key exchange method allows two parties that have no prior knowledge of each 
        /// other to jointly establish a shared secret key over an insecure communications channel. This 
        /// key can then be used to encrypt subsequent communications using a symmetric key cipher. This
        /// class controls communication between the client and server using NetDragon Websoft's exchange
        /// request packet. Once keys are set, Blowfish will be reinitialized and the message server will 
        /// begin to receive packets.
        /// </summary>
        static NetDragonDHKeyExchange()
        {
            DiffieHellmanKeyExchange exchange = new DiffieHellmanKeyExchange(PRIMATIVE_ROOT, GENERATOR);
            _serverRequestKey = exchange.GenerateRequest();
            RandomGenerator = new ConcurrentRandom();
        }

        /// <summary>
        /// The Diffie–Hellman key exchange method allows two parties that have no prior knowledge of each 
        /// other to jointly establish a shared secret key over an insecure communications channel. This 
        /// key can then be used to encrypt subsequent communications using a symmetric key cipher. This
        /// class controls communication between the client and server using NetDragon Websoft's exchange
        /// request packet. Once keys are set, Blowfish will be reinitialized and the message server will 
        /// begin to receive packets.
        /// </summary>
        public NetDragonDHKeyExchange()
            : base(PRIMATIVE_ROOT, GENERATOR)
        {
            _decryptionIv = new byte[8];
            _encryptionIv = new byte[8];
        }

        /// <summary>
        /// This method creates an exchange request packet which includes the server's public key and the 
        /// client's decryption and encryption initialization vectors (if they are initialized). It returns
        /// the created packet to be sent to the client.
        /// </summary>
        public byte[] Request()
        {
            return new KeyExchangeRequest(_serverRequestKey, _encryptionIv, _decryptionIv);
        }

        /// <summary>
        /// This method processes the client's response packet and responds back by configuring the client's
        /// remote Blowfish cipher implementation. The server computes the secret exchange key using the
        /// client's public key, then transfers that key to the Blowfish cipher. The client's decryption and
        /// encryption IVs are reset.
        /// </summary>
        /// <param name="publicKey">The client's public key from the exchange response.</param>
        /// <param name="cipher">The client's remote Blowfish cipher implementation.</param>
        public BlowfishCipher Respond(string publicKey, BlowfishCipher cipher)
        {
            cipher.KeySchedule(GenerateResponse(publicKey));
            cipher.SetDecryptionIV(_decryptionIv);
            cipher.SetEncryptionIV(_encryptionIv);
            return cipher;
        }
    }
}