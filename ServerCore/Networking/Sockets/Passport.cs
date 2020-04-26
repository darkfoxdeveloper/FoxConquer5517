// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Passport.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:52

using System;
using System.Net;
using System.Net.Sockets;
using ServerCore.Common;
using ServerCore.Interfaces;

namespace ServerCore.Networking.Sockets
{
    /// <summary>
    /// This class contains the player's network passport information, which includes the remote client socket, 
    /// packet buffer, player's IP address, a collection of variables for processing packets from the client, and 
    /// methods used for sending and processing packets.
    /// </summary>
    public unsafe class Passport
    {
        // Global-Scope Variables:
        public IAsynchronousSocket Server { get; private set; }      // The server the client is currently connected to.
        public Socket Socket { get; private set; }      // The client's remote socket.
        public ICipher Cipher { get; set; }             // The client's authentication cipher for packet processing.
        public byte[] Packet { get; set; }              // The current packet being constructed for processing.
        public string IpAddress { get; private set; }   // The IP address of the client.
        public int ExpectedReceiveLength { get; set; }  // The expected length for the packet being received.
        public int CurrentWritePosition { get; set; }   // The current write position in the packet.
        public object SendLock { get; private set; }    // The lock for sending packets in sequence to the client.

        /// <summary>
        /// This class contains the player's network passport information, which includes the remote client socket, 
        /// packet buffer, player's IP address, a collection of variables for processing packets from the client, and 
        /// methods used for sending and processing packets.
        /// </summary>
        /// <param name="server">The server the client is currently connected to.</param>
        /// <param name="socket">The client's remote socket.</param>
        /// <param name="cipher">The client's authentication cipher for packet processing.</param>
        public Passport(IAsynchronousSocket server, Socket socket, ICipher cipher)
        {
            Server = server;
            Socket = socket;
            IpAddress = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();
            Cipher = cipher;
            SendLock = new object();
        }

        /// <summary>
        /// This function sends a packet to the client using the client's remote socket defined above. The 
        /// packet is encrypted using the client's selected cipher algorithm and sent through the client's remote
        /// socket. If the server has a footer, it will write that footer to the end of the packet.
        /// </summary>
        /// <param name="packet">The packet being encrypted and sent to the client.</param>
        public void Send(byte[] packet)
        {
            try
            {
                lock (SendLock) // Locked to prevent two packets from being encrypted and sent at the same time.
                {
                    // Add the footer to the end of the packet:
                    if (Server.FooterLength > 0)
                        fixed (byte* packetPtr = packet)
                            NativeFunctionCalls.memcpy(packetPtr + packet.Length - 8,
                                Server.Footer, Server.FooterLength);

                    // Encrypt the packet and attempt to send it to the client:
                    byte[] encryptedPacket = Cipher != null ? Cipher.Encrypt(packet, packet.Length) : packet;
                    Socket.Send(encryptedPacket);
                }
            }
            catch (SocketException e)
            {
                // Was the connection issue a problem on our side or the client's side?
                if (e.SocketErrorCode != SocketError.Disconnecting &&
                    e.SocketErrorCode != SocketError.NotConnected &&
                    e.SocketErrorCode != SocketError.ConnectionReset &&
                    e.SocketErrorCode != SocketError.ConnectionAborted &&
                    e.SocketErrorCode != SocketError.Shutdown)
                    Console.WriteLine(e);
            }
        }

        /// <summary> 
        /// This method disconnects the client's remote socket, allowing for a natural disconnect. The socket is not
        /// reused for new connections to the server. If the socket has already been disconnected, this function will 
        /// catch the socket error and not allow it to run the disconnect function twice.
        /// </summary>
        public void Disconnect()
        {
            try // This function can fail if the client is already disconnecting.
            {
                if (Socket.Connected) // If the socket is connected, disconnect the client:
                    Socket.BeginDisconnect(true, Server.Disconnect, this);
            }
            catch (SocketException e)
            {
                // If the socket has already been disconnected, then don't display the error.
                if (e.SocketErrorCode != SocketError.Disconnecting &&
                    e.SocketErrorCode != SocketError.NotConnected &&
                    e.SocketErrorCode != SocketError.ConnectionReset &&
                    e.SocketErrorCode != SocketError.ConnectionAborted &&
                    e.SocketErrorCode != SocketError.Shutdown)
                    Console.WriteLine(e);
            }
        }
    }
}