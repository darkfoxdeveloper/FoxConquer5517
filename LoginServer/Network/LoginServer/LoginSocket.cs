// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Login Socket.cs
// Last Edit: 2016/11/23 09:59
// Created: 2016/11/23 09:57

using System;
using System.Net.Sockets;
using ServerCore.Common;
using ServerCore.Networking.Packets;
using ServerCore.Networking.Sockets;
using ServerCore.Security;

namespace LoginServer.Network.LoginServer
{
    /// <summary>
    /// This class encapsulates the account server. It inherits functionality from the asynchronous server socket 
    /// class, allowing the server to be created and instantiated as a server socket system. It also contains the
    /// socket events used in processing clients, packets, and other socket events.
    /// </summary>
    public sealed class LoginSocket : AsynchronousServerSocket
    {
        PacketProcessor<PacketHandlerType, PacketType, Action<Client, byte[]>> m_pProcessor;
        /// <summary>
        /// This class encapsulates the account server. It inherits functionality from the asynchronous server socket 
        /// class, allowing the server to be created and instantiated as a server socket system. It also contains the
        /// socket events used in processing clients, packets, and other socket events.
        /// </summary>
        public LoginSocket()
            : base("Login Server", AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            OnClientConnect = Connect;
            OnClientReceive = Receive;

            m_pProcessor = new PacketProcessor<PacketHandlerType, PacketType, Action<Client, byte[]>>(new LoginPacketHandler());
        }

        /// <summary>
        /// This method is invoked when the client has been approved of connecting to the server. The client should
        /// be constructed in this method, and cipher algorithms should be initialized. If any packets need to be
        /// sent in the connection state, they should be sent here.
        /// </summary>
        /// <param name="state">Represents the status of an asynchronous operation.</param>
        public void Connect(AsynchronousState state)
        {
            var client = new Client(this, state.Socket, new NetDragonAuthenticationCipher());
            state.Client = client;
            client.Send(new MsgEncryptCode(client.IpAddress.GetHashCode()));
        }

        /// <summary>
        /// This method is invoked when the client has data ready to be processed by the server. The server will
        /// switch between the packet type to find an appropriate function for processing the packet. If the
        /// packet was not found, the packet will be outputted to the console as an error.
        /// </summary>
        /// <param name="state">Represents the status of an asynchronous operation.</param>
        public void Receive(AsynchronousState state)
        {
            // Retrieve client information from the asynchronous state:
            var pClient = state.Client as Client;

            if (pClient != null && pClient.Packet != null) // check if it's alright
            {
                var type = (PacketType)BitConverter.ToUInt16(pClient.Packet, 2);
                Action<Client, byte[]> action = m_pProcessor[type];

                // Process the client's packet:
                if (action != null) action(pClient, pClient.Packet);
                else LoginPacketHandler.Report(pClient.Packet);

                #region Moved
                //byte[] pPacket = pClient.Packet;
                //var pType = (PacketType)BitConverter.ToInt16(pPacket, 2);

                //if (BitConverter.ToUInt16(pPacket, 0) == 276 &&
                //    (pType == PacketType.AUTHENTICATION_REQUEST))
                //{
                //    var pRequest = new AuthenticationRequest(pPacket, pClient.IpAddress.GetHashCode());

                //    // tells the console that the user x is trying to login on server y
                //    ServerKernel.Log.SaveLog(string.Format("User [{0}] is trying to login on server [{1}].",
                //        pClient.Account, pRequest.Server));

                //    // let's check if user is spamming login requests
                //    LoginAttemptRecord pLogin = null;
                //    if (!ServerKernel.LoginAttemptRecords.TryGetValue(pClient.IpAddress, out pLogin))
                //    {
                //        pLogin = new LoginAttemptRecord(pClient.IpAddress);
                //        ServerKernel.LoginAttemptRecords.TryAdd(pLogin.IpAddress, pLogin);
                //    }

                //    if (!pLogin.Enabled) // user spamming login?
                //    {
                //        pClient.Send(new AuthenticationResponse(RejectionType.MAXIMUM_LOGIN_ATTEMPTS));
                //        ServerKernel.Log.SaveLog(
                //            string.Format("User [{0}] has passport denied due to exceeding login limit on IP [{1}].",
                //                pRequest.Account, pClient.IpAddress), true, "LoginServer");
                //        pClient.Disconnect();
                //        return;
                //    }

                //    DbAccount pUser = Database.Accounts.SearchByName(pRequest.Account); // fetch user information
                //    if (pUser != null) // user exists?
                //    {
                //        // yes
                //        pClient.Account = pUser;

                //        // check uncommon characters
                //        var szPw = string.Empty;
                //        foreach (var c in pRequest.Password)
                //        {
                //            switch (c)
                //            {
                //                case '-':
                //                    szPw += '0';
                //                    break;
                //                case '#':
                //                    szPw += '1';
                //                    break;
                //                case '(':
                //                    szPw += '2';
                //                    break;
                //                case '"':
                //                    szPw += '3';
                //                    break;
                //                case '%':
                //                    szPw += '4';
                //                    break;
                //                case '\f':
                //                    szPw += '5';
                //                    break;
                //                case '\'':
                //                    szPw += '6';
                //                    break;
                //                case '$':
                //                    szPw += '7';
                //                    break;
                //                case '&':
                //                    szPw += '8';
                //                    break;
                //                case '!':
                //                    szPw += '9';
                //                    break;
                //                default:
                //                    szPw += c;
                //                    break;
                //            }
                //        }

                //        bool bSuccess = true;
                //        // check if user has input the right password
                //        if (pUser.Password != szPw)
                //        {
                //            // invalid pw
                //            pClient.Send(new AuthenticationResponse(RejectionType.INVALID_PASSWORD));
                //            ServerKernel.Log.SaveLog(
                //                string.Format("User [{0}] entered an invalid password [{1}].", pUser.Username,
                //                    pClient.IpAddress), true, "LoginServer");
                //            pClient.Disconnect();
                //            return;
                //        }

                //        if (pUser.Status == 1) // user is banned?
                //        {
                //            // banned
                //            pClient.Send(new AuthenticationResponse(RejectionType.ACCOUNT_BANNED));
                //            ServerKernel.Log.SaveLog(
                //                string.Format("User [{0}] has passport denied due to account lock status.",
                //                    pUser.Username), true, "LoginServer");
                //            pClient.Disconnect();
                //            return;
                //        }

                //        if (pUser.Status == -1) // user has activated account?
                //        {
                //            pClient.Send(new AuthenticationResponse(RejectionType.ACCOUNT_NOT_ACTIVATED));
                //            ServerKernel.Log.SaveLog(
                //                string.Format("User [{0}] has passport denied due to account inactive status.",
                //                    pUser.Username), true, "LoginServer");

                //            pClient.Disconnect();
                //            return;
                //        }

                //        GameServer pServer;
                //        if (!ServerKernel.OnlineServers.TryGetValue(pRequest.Server, out pServer))
                //        // server is not online
                //        {
                //            pClient.Send(new AuthenticationResponse(RejectionType.SERVER_MAINTENANCE));
                //            ServerKernel.Log.SaveLog(
                //                string.Format("User [{0}] tried to login on a invalid server [{1}].", pUser.Username,
                //                    pRequest.Server), true, "LoginServer");

                //            pClient.Disconnect();
                //            return;
                //        }

                //        uint dwHash = (uint)ThreadSafeRandom.RandGet(1000, int.MaxValue);

                //        var pTransferCipher = new TransferCipher(ServerKernel.LoginTransferKey,
                //            ServerKernel.LoginTransferSalt, pClient.IpAddress);
                //        var pCrypto = pTransferCipher.Encrypt(new[] { pUser.Identity, dwHash });

                //        string szAddress = "135.12.15.139"; // random ip just to connect
                //        if (!pServer.IpAddress.StartsWith("127") && pServer.IpAddress != "localhost")
                //            szAddress = pServer.IpAddress;

                //        pServer.Send(new MsgUsrLogin(pUser.Identity, dwHash) { IpAddress = pClient.IpAddress });

                //        pClient.Send(new AuthenticationResponse(pCrypto[0], pCrypto[1], szAddress, 5816));
                //        ServerKernel.Log.SaveLog(string.Format("User [{0}] has successfully logged into {1}.",
                //            pUser.Username, pRequest.Server), true, LogType.MESSAGE);
                //        return;
                //    }
                //    else
                //    {
                //        // no
                //        pClient.Send(new AuthenticationResponse(RejectionType.INVALID_ACCOUNT));
                //        ServerKernel.Log.SaveLog(
                //            string.Format("User [{0}] doesn't exist. Connection [{1}].", pRequest.Account,
                //                pClient.IpAddress), true, "LoginServer");
                //    }
                //}
                //else
                //{
                //    pClient.Send(new AuthenticationResponse(RejectionType.INVALID_AUTHENTICATION_PROTOCOL));
                //    ServerKernel.Log.SaveLog(string.Format("User has tried to connect with an invalid protocol at {0}.", pClient.IpAddress));
                //}

                //pClient.Disconnect();
                #endregion
            }
        }

        /// <summary>
        /// This method is invoked when the client is disconnecting from the server. It disconnects the client
        /// from server and disposes of game structures.
        /// </summary>
        /// <param name="state">Represents the status of an asynchronous operation.</param>
        public void Disconnect(object state)
        {
            try
            {
                var pClient = state as Client;
                if (pClient == null)
                {
                    ServerKernel.Log.SaveLog("Disconnection: pClient is null", true, "LoginServer", LogType.ERROR);
                    return; // hope it doesn't happen :)
                }

                Client trash;
                if (!ServerKernel.Players.TryRemove(pClient.Account.Identity, out trash))
                    return; // why this might happen?

                ServerKernel.Log.SaveLog(string.Format("User [{0}] has been disconnected", pClient.Account.Username));
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), true, "LoginServer", LogType.EXCEPTION);
            }
        }
    }
}