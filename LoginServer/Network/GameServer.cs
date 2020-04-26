// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Game Server.cs
// Last Edit: 2016/11/23 10:03
// Created: 2016/11/23 10:03

using System;
using System.Net.Sockets;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Interfaces;
using ServerCore.Networking.Sockets;

namespace LoginServer.Network
{
    /// <summary>
    /// This class encapsulates the GameServer client. This contains informations about the server
    /// and is responsible to the communication between Login Server and MsgServer.
    /// </summary>
    public sealed class GameServer : Passport
    {
        private AsynchronousGameServerSocket _serverSocket;

        /// <summary>
        /// The server name might be unique, you can't signin 2 servers with the same name.
        /// </summary>
        public string ServerName;
        /// <summary>
        /// The amount of players that are actually online on the game server.
        /// </summary>
        public int OnlinePlayers;
        /// <summary>
        /// The max amount of online players on the game.
        /// </summary>
        public int MaxPermitedPlayers = Constants.MinOnlinePlayer;

        public int GamePort = 5816;

        public InterServerState ConnectionState = InterServerState.WAITING_HELLO;

        /// <summary>
        /// Starts a new instance of GameServer.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="socket"></param>
        /// <param name="cipher"></param>
        public GameServer(AsynchronousGameServerSocket server, Socket socket, ICipher cipher)
            : base(server, socket, cipher)
        {
            _serverSocket = server;
        }

        /// <summary>
        /// Sends the buffer to the Game Server client.
        /// </summary>
        /// <param name="pMsg">The message buffer.</param>
        public new void Send(byte[] pMsg)
        {
            try
            {
                base.Send(pMsg);
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog("Could not send message to MsgServer\r\n" + ex,
                    true,
                    LogType.ERROR);
            }
        }

        /// <summary>
        /// Checks if the server is online or not.
        /// </summary>
        /// <returns></returns>
        public bool IsOnline()
        {
            bool part1 = _serverSocket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (_serverSocket.Available == 0);
            return !part1 || !part2;
        }
    }
}
