// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Asynchronous State.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:52

using System.Net.Sockets;
using System.Threading;

namespace ServerCore.Networking.Sockets
{
    /// <summary>
    /// This class encapsulates the asynchronous state from resulting asynchronous operations. It contains the
    /// player's client, packet buffer, and socket connection.
    /// </summary>
    public sealed class AsynchronousState
    {
        public object Client;           // The client objected used in managing the player's instantiated game structures.
        public byte[] Buffer;           // The client's packet buffer, used in managing packets.
        public Socket Socket;           // The client's remote socket on the server.
        public ManualResetEvent Event;  // A manual reset event for waiting on data.

        /// <summary>
        /// This class encapsulates the asynchronous state from resulting asynchronous operations. It contains the
        /// player's client, packet buffer, and socket connection.
        /// </summary>
        /// <param name="socket">The client's remote socket on the server.</param>
        public AsynchronousState(Socket socket)
        {
            Client = null;
            Buffer = null;
            Socket = socket;
            Event = new ManualResetEvent(false);
        }
    }
}
