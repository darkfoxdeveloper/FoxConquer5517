using System;
using System.Net.Sockets;
using MsgServer.Network.GameServer;
using ServerCore.Common;
using ServerCore.Networking.Packets;
using ServerCore.Networking.Sockets;

namespace MsgServer.Network.LoginServer
{
    public sealed class LoginSocket : AsynchronousClientSocket
    {
        PacketProcessor<PacketHandlerType, PacketType, Action<LoginClient, byte[]>> m_pProcessor;

        public LoginSocket()
            : base("", AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            OnClientConnect = Connect;
            OnClientReceive = Receive;
            OnClientDisconnect = Disconnect;

            m_pProcessor = new PacketProcessor<PacketHandlerType, PacketType, Action<LoginClient, byte[]>>(new LoginPacketHandlers());
        }

        /// <summary>
        /// This method is invoked when the client has been approved of connecting to the server. The client should
        /// be constructed in this method, and cipher algorithms should be initialized. If any packets need to be
        /// sent in the connection state, they should be sent here.
        /// </summary>
        /// <param name="pState">Represents the status of an asynchronous operation.</param>
        public void Connect(AsynchronousState pState)
        {
            var pServer = new LoginClient(this, pState.Socket, null);
            pState.Client = pServer;
        }

        /// <summary>
        /// This method is invoked when the client has data ready to be processed by the server. The server will
        /// switch between the packet type to find an appropriate function for processing the packet. If the
        /// packet was not found, the packet will be outputted to the console as an error.
        /// </summary>
        /// <param name="pState">Represents the status of an asynchronous operation.</param>
        public void Receive(AsynchronousState pState)
        {
            var pServer = pState.Client as LoginClient;
            if (pServer != null && pServer.Packet != null)
            {
                var type = (PacketType)BitConverter.ToUInt16(pServer.Packet, 2);
                Action<LoginClient, byte[]> action = m_pProcessor[type];

                // Process the client's packet:
                if (action != null) action(pServer, pServer.Packet);
                else GamePacketHandler.Report(pServer.Packet);
            }
        }

        /// <summary>
        /// This method is invoked when the client is disconnecting from the server. It disconnects the client
        /// from the map server and disposes of game structures. Upon disconnecting from the map server, the
        /// character will be removed from the map and screen actions will be terminated. Then, character
        /// data will be disposed of.
        /// </summary>
        /// <param name="pState">Represents the status of an asynchronous operation.</param>
        public void Disconnect(object pState)
        {
            var pObj = pState as LoginClient;
            if (pObj == null)
            {
                // something went wrong
                return;
            }

            ServerKernel.Log.SaveLog("Disconnected from the account server...", true, LogType.WARNING);
            ServerKernel.LoginServer = null;
        }
    }
}