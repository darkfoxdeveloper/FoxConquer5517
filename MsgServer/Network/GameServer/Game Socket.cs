using System;
using System.Net.Sockets;
using NHibernate.Mapping;
using ServerCore.Networking.Packets;
using ServerCore.Networking.Sockets;
using ServerCore.Security;

namespace MsgServer.Network.GameServer
{
    /// <summary>
    /// This class encapsulates the message server. It inherits functionality from the asynchronous server socket 
    /// class, allowing the server to be created and instantiated as a server socket system. It also contains the
    /// socket events used in processing clients, packets, and other socket events.
    /// </summary>
    public sealed class GameSocket : AsynchronousServerSocket
    {
        // Local-Scope Variable Declarations:
        PacketProcessor<PacketHandlerType, PacketType, Action<Client, byte[]>> _processor;

        /// <summary>
        /// This class encapsulates the message server. It inherits functionality from the asynchronous server socket 
        /// class, allowing the server to be created and instantiated as a server socket system. It also contains the
        /// socket events used in processing clients, packets, and other socket events.
        /// </summary>
        /// <param name="port">The port the server socket system will bind to and listen on.</param>
        public GameSocket()
            : base("MsgServer", "TQServer", AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            OnClientConnect = Connect;
            OnClientReceive = Receive;
            OnClientExchange = Exchange;
            OnClientDisconnect = Disconnect;

            // Initialize Handlers:
            _processor = new PacketProcessor<PacketHandlerType, PacketType, Action<Client, byte[]>>
                (new GamePacketHandler());
        }

        /// <summary>
        /// This method is invoked when the client has been approved of connecting to the server. The client should
        /// be constructed in this method, and cipher algorithms should be initialized. If any packets need to be
        /// sent in the connection state, they should be sent here.
        /// </summary>
        /// <param name="state">Represents the status of an asynchronous operation.</param>
        public void Connect(AsynchronousState state)
        {
            // Create the client for the asynchronous state:
            Client client = new Client(this, state.Socket, new BlowfishCipher());
            state.Client = client;
            client.Send(client.Exchange.Request());
        }

        /// <summary>
        /// This method is invoked when the client is exchanging keys with the server to generate a common key for
        /// cipher encryptions and decryptions. This method is only implemented in the message server after
        /// patch 5017 (when Blowfish and the DH Key Exchange was implemented).
        /// </summary>
        /// <param name="state">Represents the status of an asynchronous operation.</param>
        public unsafe void Exchange(AsynchronousState state)
        {
            // Retrieve client information from the asynchronous state:
            Client client = state.Client as Client;
            if (client != null && client.Packet != null)
                fixed (byte* packet = client.Packet)
                {
                    // Process the client's packet:
                    int position = 7;
                    int packetLength = *(int*)(packet + position); position += 4;
                    if (packetLength + 7 == client.Packet.Length)
                    {
                        // The exchange is valid. Process it:
                        int junkLength = *(int*)(packet + position); position += 4 + junkLength;
                        int keyLength = *(int*)(packet + position); position += 4;
                        string key = new string((sbyte*)packet, position, keyLength);

                        // Process the key and configure Blowfish:
                        client.Cipher = client.Exchange.Respond(key, client.Cipher as BlowfishCipher);
                    }
                    else client.Disconnect();
                }
        }

        /// <summary>
        /// This method is invoked when the client has data ready to be processed by the server. The server will
        /// switch between the packet type to find an appropriate function for processing the packet. If the
        /// packet was not found, the packet will be outputted to the console as an error.
        /// </summary>
        /// <param name="state">Represents the status of an asynchronous operation.</param>
        public void Receive(AsynchronousState state)
        {
            Client client = state.Client as Client;
            if (client != null && client.Packet != null)
            {
                // Get the packet handler from the packet processor:
                PacketType type = (PacketType)BitConverter.ToUInt16(client.Packet, 2);
                Action<Client, byte[]> action = _processor[type];

                // Process the client's packet:
                if (action != null) action(client, client.Packet);
                else GamePacketHandler.Report(client.Packet);
            }
        }

        /// <summary>
        /// This method is invoked when the client is disconnecting from the server. It disconnects the client
        /// from the map server and disposes of game structures. Upon disconnecting from the map server, the
        /// character will be removed from the map and screen actions will be terminated. Then, character
        /// data will be disposed of.
        /// </summary>
        /// <param name="state">Represents the status of an asynchronous operation.</param>
        public void Disconnect(object state)
        {
            Client client = state as Client;
            Client trash = null;

            // Remove player from the character creation pool:
            if (ServerKernel.CharacterCreation.TryRemove(client.Identity, out trash))
                return;

            // Remove the client from the client pool:
            if (client != null && ServerKernel.Players.TryRemove(client.Identity, out trash))
            {
                // Save the character:
                if (client.Character != null)
                {
                    try
                    {
                        client.Character.RemoveFromMapThread();
                    }
                    catch
                    {
                        
                    }
                }
            }
        }
    }
}
