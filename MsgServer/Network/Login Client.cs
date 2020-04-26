// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Login Client.cs
// Last Edit: 2016/11/23 10:38
// Created: 2016/11/23 10:38

using System.Net.Sockets;
using ServerCore.Common.Enums;
using ServerCore.Interfaces;
using ServerCore.Networking.Sockets;

namespace MsgServer.Network
{
    public sealed class LoginClient : Passport
    {
        private AsynchronousClientSocket m_socket;

        public LoginClient(AsynchronousClientSocket server, Socket socket, ICipher cipher)
            : base(server, socket, cipher)
        {
            m_socket = server;
        }

        public InterServerState ConnectionState { get; set; }
    }
}