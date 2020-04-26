// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - Login Packet Handler.cs
// Last Edit: 2017/03/01 17:25
// Created: 2017/02/04 14:41

using System;
using MsgServer.Structures;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.LoginServer
{
    public sealed class LoginPacketHandlers
    {
        #region 11 - Auth Response
        [PacketHandlerType(PacketType.LOGIN_AUTH_REQUEST)]
        public void AuthRequest(LoginClient pServer, byte[] pBuffer)
        {
            var pMsg = new MsgLoginSvAuthRequest(pBuffer);
            if (pMsg.Message != ServerKernel.HelloReplyString)
            {
                pServer.Disconnect();
                return;
            }

            pServer.ConnectionState = InterServerState.MEETING_OK;
            pServer.Send(new MsgLoginSvAuthentication(ServerKernel.Username, ServerKernel.Password, ServerKernel.ServerName, ServerKernel.MaxOnlinePlayer));
            ServerKernel.Log.SaveLog("Connected to the Account Server, waiting for authentication...", true, LogType.MESSAGE);
        }
        #endregion
        #region 13 - Server State

        [PacketHandlerType(PacketType.LOGIN_COMPLETE_AUTHENTICATION)]
        public void CompleteAuthentication(LoginClient pClient, byte[] pBuffer)
        {
            var pMsg = new LoginSvResponsePacket(pBuffer);
            switch (pMsg.Response)
            {
                case LoginServerResponse.LOGIN_SUCCESSFUL:
                    // send server information packet
                    pClient.Send(new MsgServerInformation(ServerKernel.MaxOnlinePlayer,
                        (ushort)ServerKernel.Players.Count, ServerKernel.ServerName, ServerKernel.GamePort));
                    ServerKernel.Log.SaveLog("Authentication completed! Server is ready to go...", true, LogType.MESSAGE);

                    return;
                case LoginServerResponse.LOGIN_DENIED_DISCONNECTED:
                    ServerKernel.Log.SaveLog("Disconnected of the account server...", true, LogType.WARNING);
                    break;
                case LoginServerResponse.LOGIN_DENIED_SERVER_NOT_EXIST:
                    ServerKernel.Log.SaveLog("Access denied to the account server due to invalid server name...", true,
                        LogType.WARNING);
                    break;
                case LoginServerResponse.LOGIN_DENIED_IPADDRESS:
                    ServerKernel.Log.SaveLog("Access denied to the account server due to invalid ip address...", true,
                        LogType.WARNING);
                    break;
                case LoginServerResponse.LOGIN_DENIED_LOGIN:
                    ServerKernel.Log.SaveLog(
                        "Attempted to use an invalid username or password when connecting to the account server...",
                        true, LogType.WARNING);
                    break;
                default:
                    ServerKernel.Log.SaveLog("Invalid LOGIN_COMPLETE_AUTHENTICATION response " + pMsg.Response, true,
                        LogType.DEBUG);
                    break;
            }

            try
            {
                pClient.Disconnect();
            }
            catch (Exception ex)
            {
            }

        }

        #endregion
        #region 26 - Get User Incoming Login Request
        [PacketHandlerType(PacketType.LOGIN_REQUEST_USER_SIGNIN)]
        public void ProcessUserSignin(LoginClient pClient, byte[] pBuffer)
        {
            var pMsg = new MsgUsrLogin(pBuffer);
            LoginRequest pRequest;
            if (!ServerKernel.LoginQueue.TryRemove(pMsg.UserIdentity, out pRequest))
                ServerKernel.LoginQueue.TryAdd(pMsg.UserIdentity,
                    pRequest =
                        new LoginRequest(pMsg.UserIdentity, pMsg.PacketHash, pMsg.RequestTime, pMsg.IpAddress,
                            pMsg.VipLevel, pMsg.Authority, pMsg.MacAddress));
        }
        #endregion
    }
}