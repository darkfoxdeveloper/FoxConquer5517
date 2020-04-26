// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Client.cs
// Last Edit: 2016/11/23 10:26
// Created: 2016/11/23 10:25

using System.Net.Sockets;
using MsgServer.Structures.Entities;
using MsgServer.Structures.World;
using ServerCore.Common.Enums;
using ServerCore.Interfaces;
using ServerCore.Networking.Packets;
using ServerCore.Networking.Sockets;
using ServerCore.Security;

namespace MsgServer.Network
{
    /// <summary>
    /// This class encapsulates the player's client. The client contains the player's information from all 
    /// initialized game structures. It also contains the player's passport, which contains the client's remote
    /// socket and variables for processing packets.
    /// </summary>
    public sealed class Client : Passport
    {
        // Account Holder & Client Specific Variable Declarations:
        public uint Identity;               // The character's unique identification number from the database.
        public uint AccountIdentity; // The account's unique identification number from the database.
        public uint Authority;    // The amount of authority the account holder has over server actions. 
        public string Language;             // The language specified in the client's StrRes.dat file.
        public byte VipLevel = 0;
        public string MacAddress = "00-00-00-00-00-00";

        // Global-Scope Game Structure Initializations:
        public Character Character;             // The player's game character persona.
        public NetDragonDHKeyExchange Exchange; // The DH key exchange instance for the client.
        public Screen Screen;               // Encapsulates all other players in the character's screen.
        public Tile Tile;                   // The current tile the character is standing on.
        private AsynchronousServerSocket _socket;

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
            Exchange = new NetDragonDHKeyExchange();
            _socket = server;
        }

        /// <summary>
        /// This method will send a text message to the user interface. It can only be sent after a successful login.
        /// </summary>
        /// <param name="text">The text message that will be sent to the user.</param>
        /// <param name="tone">Where the message will be located.</param>
        public void SendMessage(string text, ChatTone tone = ChatTone.TOP_LEFT)
        {
            Send(new MsgTalk(text, tone));
        }

        public bool IsOnline()
        {
            bool part1 = _socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (_socket.Available == 0);
            if (part1 && part2)
                return false;
            return true;
        }
    }
}