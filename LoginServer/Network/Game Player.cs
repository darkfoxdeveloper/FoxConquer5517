// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Game Player.cs
// Last Edit: 2016/11/23 10:05
// Created: 2016/11/23 10:05

using System.Net.Sockets;
using DB.Entities;
using ServerCore.Interfaces;
using ServerCore.Networking.Sockets;

namespace LoginServer.Network
{
    /// <summary>
    /// This class encapsulates the player's client. The client contains the player's information from all 
    /// initialized game structures. It also contains the player's passport, which contains the client's remote
    /// socket and variables for processing packets.
    /// </summary>
    public sealed class Client : Passport
    {
        public DbAccount Account; // the player's database information

        /// <summary>
        /// This class encapsulates the player's client. The client contains the player's information from all 
        /// initialized game structures. It also contains the player's passport, which contains the client's remote
        /// socket and variables for processing packets.
        /// </summary>
        /// <param name="server">The server that owns the client.</param>
        /// <param name="socket">The client's remote socket from connect.</param>
        /// <param name="cipher">The client's initialized cipher for packet processing.</param>
        public Client(AsynchronousServerSocket server, Socket socket, ICipher cipher)
            : base(server, socket, cipher)
        {
        }
    }
}