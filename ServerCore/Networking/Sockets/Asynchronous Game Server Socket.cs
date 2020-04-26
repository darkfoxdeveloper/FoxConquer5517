// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Asynchronous Game Server Socket.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:52

using System;
using System.Net;
using System.Net.Sockets;
using ServerCore.Common;
using ServerCore.Interfaces;
using ServerCore.Networking.Packets;

namespace ServerCore.Networking.Sockets
{
    /// <summary>
    /// The asynchronous socket class encapsulates an asynchronous server socket. It allows multiple clients to connect 
    /// to the server and process packets. It makes use of Microsoft's .NET socket class to control asynchronous 
    /// packet processing requests. The class must be inherited with defined socket events to be implemented.
    /// </summary>
    public abstract unsafe class AsynchronousGameServerSocket : Socket, IAsynchronousSocket
    {
        // Socket Events - Pure virtual functions (Defined when the abstract class is constructed):
        protected delegate void AsynchronousConnect(AsynchronousState state);      // Invoked on a successful client accept.
        protected delegate void AsynchronousExchange(AsynchronousState state);     // Invoked to exchange keys with the client.
        protected delegate void AsynchronousReceive(AsynchronousState state);      // Invoked to receive data from the client.
        protected delegate void AsynchronousDisconnect(object state);              // Invoked when the client has disconnected.

        // Protected Variable Declarations:
        protected AsynchronousConnect OnClientConnect;         // Invoked on a successful client accept.
        protected AsynchronousExchange OnClientExchange;       // Invoked to exchange keys with the client.
        protected AsynchronousReceive OnClientReceive;         // Invoked to receive data from the client.
        protected AsynchronousDisconnect OnClientDisconnect;   // Invoked when the client has disconnected.

        // Global-Scope Properties & Constants.
        public string Name { get; set; }                // The name of the server.
        public int FooterLength { get; set; }           // The length of the footer for each packet.
        public string Footer { get; set; }              // The text for the footer at the end of each packet.
        public const int EXCHANGE_BUFFER_SIZE = 1024;   // The largest possible length of the exchange buffer.
        public const int MAX_PACKET_SIZE = 2048;        // The largest possible length of a data packet.
        public const int NPC_PACKET_SIZE = 32767;        // The largest possible length of a data packet.
        public const int MAX_PACKET_ID = 1000;         // The largest possible packet identity of a data packet.

        /// <summary>
        /// The asynchronous socket class encapsulates an asynchronous server socket. It allows multiple clients to connect 
        /// to the server and process packets. It makes use of Microsoft's .NET socket class to control asynchronous 
        /// packet processing requests. The class must be inherited with defined socket events to be implemented.
        /// </summary>
        /// <param name="name">The name of the server.</param>
        /// <param name="family">Specifies the addressing scheme that the socket instance will use.</param>
        /// <param name="socketType">Specifies the socket type that the instance represents.</param>
        /// <param name="protocol">Specifies the protocol the socket instance will support.</param>
        public AsynchronousGameServerSocket(string name, AddressFamily family, SocketType socketType, ProtocolType protocol)
            : base(family, socketType, protocol)
        {
            Name = name;
            FooterLength = 0;
            Footer = "";
            base.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            base.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }

        /// <summary>
        /// The asynchronous socket class encapsulates an asynchronous server socket. It allows multiple clients to connect 
        /// to the server and process packets. It makes use of Microsoft's .NET socket class to control asynchronous 
        /// packet processing requests. The class must be inherited with defined socket events to be implemented.
        /// </summary>
        /// <param name="name">The name of the server.</param>
        /// <param name="footer">The footer for each packet.</param>
        /// <param name="family">Specifies the addressing scheme that the socket instance will use.</param>
        /// <param name="socketType">Specifies the socket type that the instance represents.</param>
        /// <param name="protocol">Specifies the protocol the socket instance will support.</param>
        public AsynchronousGameServerSocket(string name, string footer, AddressFamily family, SocketType socketType,
            ProtocolType protocol)
            : base(family, socketType, protocol)
        {
            Name = name;
            FooterLength = footer.Length;
            Footer = footer;
            base.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            base.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }

        /// <summary>
        /// This method associates the socket with an IP address and a port. It does this by creating an IP endpoint
        /// using the address and port, then assigning that IP endpoint to the socket. Only one socket can be
        /// bound to an IP address and port at the same time.
        /// </summary>
        /// <param name="ipAddress">The IP address the socket will bind and listen to.</param>
        /// <param name="port">The port the socket will bind to.</param>
        public bool Bind(string ipAddress, int port)
        {
            try
            {
                // Create the endpoint and bind the socket:
                base.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
                return true;
            }
            catch (SocketException exception) { Console.WriteLine(exception); }
            return false;
        }

        /// <summary>
        /// This method places the socket into the listening socket state. It listens with a backlog of the
        /// specified amount and starts accepting incoming connections.
        /// </summary>
        /// <param name="backlog">The maximum length of the pending connections queue.</param>
        public new void Listen(int backlog)
        {
            try
            {
                // Start listening for incoming connections and start accepting connections:
                base.Listen(backlog);
                base.BeginAccept(AcceptConnection, null);
            }
            catch (SocketException exception) { Console.WriteLine(exception); }
        }

        /// <summary>
        /// This method begins accepting a client from the pending connections queue. In this method, the client is
        /// created and the IP address is checked for brute-force attacks. If the client is validated, then the server
        /// will begin receiving data packets from the client.
        /// </summary>
        /// <param name="result">Represents the status of an asynchronous operation.</param>
        public void AcceptConnection(IAsyncResult result)
        {
            try
            {
                // Accept the socket from the asynchronous result:
                Socket clientSocket = EndAccept(result);
                // Assign a new asynchronous state to the socket:
                AsynchronousState state = new AsynchronousState(clientSocket);
                if (OnClientConnect != null) OnClientConnect(state);


                // Begin Receiving data and handling the client. If the exchange socket event is not null, 
                // handle the key exchange for the packet cipher; else, accept data from the client.
                if (OnClientExchange != null)
                {
                    state.Buffer = new byte[EXCHANGE_BUFFER_SIZE];
                    state.Socket.BeginReceive(state.Buffer, 0, EXCHANGE_BUFFER_SIZE,
                        SocketFlags.None, PrepareReceive, state);
                }
                else if (OnClientReceive != null)
                {
                    // The server does not process key exchange. Start receiving packets immediately.
                    state.Buffer = new byte[MAX_PACKET_SIZE];
                    state.Socket.BeginReceive(state.Buffer, 0, MAX_PACKET_SIZE,
                        SocketFlags.None, AnnounceReceive, state);
                }
                else
                {
                    // There are no processing methods for this server!
                    Console.WriteLine("No processing methods defined for the " + Name);
                    clientSocket.Disconnect(false);
                }
            }
            catch (SocketException e)
            {
                // Was the connection issue a problem on the server's side or the client's side?
                if (e.SocketErrorCode != SocketError.Disconnecting &&
                    e.SocketErrorCode != SocketError.NotConnected &&
                    e.SocketErrorCode != SocketError.ConnectionReset &&
                    e.SocketErrorCode != SocketError.ConnectionAborted &&
                    e.SocketErrorCode != SocketError.Shutdown)
                    Console.WriteLine(e);
            }
            finally
            {
                BeginAccept(AcceptConnection, null);
            }
        }

        /// <summary>
        /// This method begins the client and server key exchange. The client sends key exchange data first, which is
        /// picked up by the server in this socket event and processed. Then, the exchange packet is sent back to the
        /// client so both the client and server have matching cipher keys, and packets are sent to the server.
        /// </summary>
        /// <param name="result">Represents the status of an asynchronous operation.</param>
        public void PrepareReceive(IAsyncResult result)
        {
            // Get the asynchronous state for the connection:
            AsynchronousState state = result.AsyncState as AsynchronousState;
            if (state != null && state.Socket != null && state.Socket.Connected)
                try
                {
                    // Get the length of the incoming packet:
                    int length = state.Socket.EndReceive(result);
                    Passport passport = state.Client as Passport;
                    if (length > 0 && passport != null)
                    {
                        // Decrypt the exchange packet and assign variables:
                        passport.Packet = passport.Cipher != null ?
                            passport.Cipher.Decrypt(state.Buffer, length) : state.Buffer;

                        // Process the exchange, then start accepting data packets:
                        OnClientExchange(state);
                        state.Buffer = new byte[sizeof(PacketHeader)];
                        state.Socket.BeginReceive(state.Buffer, 0, sizeof(PacketHeader),
                            SocketFlags.None, AnnounceReceive, state);
                    }
                    else state.Socket.Disconnect(false);
                }
                catch (SocketException e)
                {
                    // Was the connection issue a problem on the server's side or the client's side?
                    if (e.SocketErrorCode != SocketError.Disconnecting &&
                        e.SocketErrorCode != SocketError.NotConnected &&
                        e.SocketErrorCode != SocketError.ConnectionReset &&
                        e.SocketErrorCode != SocketError.ConnectionAborted &&
                        e.SocketErrorCode != SocketError.Shutdown)
                        Console.WriteLine(e);

                    // Is the client still connected?
                    if (state != null && state.Socket != null && state.Socket.Connected)
                        state.Socket.Disconnect(false);
                }
                catch (Exception e) { Console.WriteLine(e); }
            else Disconnect(state);
        }

        /// <summary>
        /// This method begins the receiving of a packet from the client's socket. It receives the first four bytes
        /// of a packet, then requests to receive the body of the packet. If the length of the packet is less than
        /// the length of the packet header, the client will be disconnected from the server.
        /// </summary>
        /// <param name="result">Represents the status of an asynchronous operation.</param>
        public void AnnounceReceive(IAsyncResult result)
        {
            // Get the asynchronous state for the connection:
            AsynchronousState state = result.AsyncState as AsynchronousState;
            if (state != null && state.Socket != null && state.Socket.Connected)
                try
                {
                    // Get the length of the incoming buffer:
                    Passport passport = state.Client as Passport;
                    int length = 0;
                    try { length = state.Socket.EndReceive(result); }
                    catch { if (passport != null) Disconnect(state); return; }
                    if (passport != null)
                    {
                        // Decrypt the packet header:
                        byte[] buffer = new byte[length];

                        NativeFunctionCalls.memcpy(buffer, state.Buffer, length);

                        buffer = passport.Cipher != null ?
                            passport.Cipher.Decrypt(buffer, buffer.Length) : buffer;

                        PacketHeader header;

                        // Assign the packet header and process the header:
                        fixed (byte* bufferPtr = buffer) { header = *(PacketHeader*)bufferPtr; }
                        passport.ExpectedReceiveLength = header.Length - sizeof(PacketHeader) + FooterLength;

                        // Is the expected length and identity reasonable?
                        if (passport.ExpectedReceiveLength > MAX_PACKET_SIZE || (int)header.Identity > MAX_PACKET_ID)
                        {
                            // The expected length requested by the client isn't reasonable. Disconnect the client.
                            passport.Disconnect();
                            Console.WriteLine("Problem with receiving a packet from the client on the " + Name);
                            return;
                        }

                        // Construct the header for the current packet:
                        passport.Packet = new byte[passport.ExpectedReceiveLength + sizeof(PacketHeader)];
                        fixed (byte* packetPtr = passport.Packet) { *(PacketHeader*)packetPtr = header; }

                        fixed (byte* packet = passport.Packet)
                            NativeFunctionCalls.memcpy(packet, buffer, header.Length);

                        int difference = passport.ExpectedReceiveLength - length;

                        // If the difference between the expected receive length and the actual receive length is
                        // greater than zero, then the received data is a fragment of the packet.
                        if (difference > 0)
                        {
                            // The packet is a fragment, receive the rest of the body:
                            passport.ExpectedReceiveLength = difference;
                            passport.CurrentWritePosition += length;
                            state.Buffer = new byte[difference];
                            state.Socket.BeginReceive(state.Buffer, 0, passport.ExpectedReceiveLength,
                                SocketFlags.None, CompleteReceive, state);
                        }
                        else // Process the packet because it's complete.
                        {
                            // The packet has been completely received. Handle the packet using the server's 
                            // receive event (should be a packet processor):
                            OnClientReceive(state);
                            state.Buffer = new byte[MAX_PACKET_SIZE];
                            state.Socket.BeginReceive(state.Buffer, 0, MAX_PACKET_SIZE,
                                SocketFlags.None, AnnounceReceive, state);
                        }
                    }
                    else if (passport != null) passport.Disconnect();
                }
                catch (SocketException e)
                {
                    // Was the connection issue a problem on the server's side or the client's side?
                    if (e.SocketErrorCode != SocketError.Disconnecting &&
                        e.SocketErrorCode != SocketError.NotConnected &&
                        e.SocketErrorCode != SocketError.ConnectionReset &&
                        e.SocketErrorCode != SocketError.ConnectionAborted &&
                        e.SocketErrorCode != SocketError.Shutdown)
                        Console.WriteLine(e);

                    // Is the client still connected?
                    if (state != null && state.Socket != null && state.Socket.Connected)
                        state.Socket.Disconnect(false);
                }
                catch (Exception e) { Console.WriteLine(e); }
            else Disconnect(state);
        }

        /// <summary>
        /// This method completes the receiving of a packet from the client's socket. It attempts to receive the 
        /// remaining bytes of a packet, then requests to receive the header of the next packet. If the length of 
        /// the packet is zero, the client will be disconnected from the server. If the length of the buffer is
        /// not the expected length, then the packet is a fragment and the server will attempt to receive the
        /// completed packet again.
        /// </summary>
        /// <param name="result">Represents the status of an asynchronous operation.</param>
        public void CompleteReceive(IAsyncResult result)
        {
            // Get the asynchronous state for the connection:
            AsynchronousState state = result.AsyncState as AsynchronousState;
            if (state != null && state.Socket != null && state.Socket.Connected)
                try
                {
                    // Get the length of the incoming buffer:
                    int length = state.Socket.EndReceive(result);
                    Passport passport = state.Client as Passport;
                    if (length > 0 && passport != null)
                    {
                        // Decrypt the packet body and assign variables:
                        if (passport.Cipher != null)
                            passport.Cipher.Decrypt(passport.Packet, state.Buffer, length, passport.CurrentWritePosition);
                        else fixed (byte* packet = passport.Packet)
                                NativeFunctionCalls.memcpy(packet + passport.CurrentWritePosition, state.Buffer, length);
                        int difference = passport.ExpectedReceiveLength - length;

                        // If the difference between the expected receive length and the actual receive length is
                        // greater than zero, then the received data is a fragment of the packet.
                        if (difference > 0)
                        {
                            // The packet is a fragment, receive the rest of the body:
                            passport.ExpectedReceiveLength = difference;
                            passport.CurrentWritePosition += length;
                            state.Buffer = new byte[difference];
                            state.Socket.BeginReceive(state.Buffer, 0, passport.ExpectedReceiveLength,
                                SocketFlags.None, CompleteReceive, state);
                        }
                        else // Process the packet because it's complete.
                        {
                            // The packet has been completely received. Handle the packet using the server's 
                            // receive event (should be a packet processor):
                            OnClientReceive(state);
                            state.Buffer = new byte[sizeof(PacketHeader)];
                            state.Socket.BeginReceive(state.Buffer, 0, sizeof(PacketHeader),
                                SocketFlags.None, AnnounceReceive, state);
                        }
                    }
                    else if (passport != null) passport.Disconnect();
                }
                catch (SocketException e)
                {
                    // Was the connection issue a problem on the server's side or the client's side?
                    if (e.SocketErrorCode != SocketError.Disconnecting &&
                        e.SocketErrorCode != SocketError.NotConnected &&
                        e.SocketErrorCode != SocketError.ConnectionReset &&
                        e.SocketErrorCode != SocketError.ConnectionAborted &&
                        e.SocketErrorCode != SocketError.Shutdown)
                        Console.WriteLine(e);

                    // Is the client still connected?
                    if (state != null && state.Socket != null && state.Socket.Connected)
                        state.Socket.Disconnect(false);
                }
                catch (Exception e)
                {
                    // There must of been an error in handling the packet.
                    Console.WriteLine(e);
                    if (state != null && state.Socket.Connected)
                    {
                        state.Buffer = new byte[sizeof(PacketHeader)];
                        state.Socket.BeginReceive(state.Buffer, 0, sizeof(PacketHeader),
                            SocketFlags.None, AnnounceReceive, state);
                    }
                }
            else Disconnect(state);
        }

        /// <summary>
        /// This method is called once a previously connected and authenticated client has been disconnected 
        /// from the server. The method calls the disconnect socket event to dispose of the client's initialized
        /// game structures.
        /// </summary>
        /// <param name="result">Represents the status of an asynchronous operation.</param>
        public void Disconnect(IAsyncResult result)
        {
            // Dispose of the asynchronous state:
            if (result.AsyncState != null && OnClientDisconnect != null)
                OnClientDisconnect(result.AsyncState);
        }

        /// <summary>
        /// This method is called once a previously connected and authenticated client has been disconnected 
        /// from the server. The method calls the disconnect socket event to dispose of the client's initialized
        /// game structures.
        /// </summary>
        /// <param name="result">Represents the status of an asynchronous operation.</param>
        public void Disconnect(AsynchronousState state)
        {
            // Dispose of the asynchronous state:
            if (state != null && state.Client != null && OnClientDisconnect != null)
                OnClientDisconnect(state.Client);
        }
    }
}