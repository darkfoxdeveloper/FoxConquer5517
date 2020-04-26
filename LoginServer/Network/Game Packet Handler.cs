// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Game Packet Handler.cs
// Last Edit: 2016/11/23 10:01
// Created: 2016/11/23 10:01

using System;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace LoginServer.Network
{
    public sealed class MsgPacketHandler
    {
        #region 11 - Hello World!
        [PacketHandlerType(PacketType.LOGIN_AUTH_REQUEST)]
        public void AuthRequest(GameServer pServer, byte[] pBuffer)
        {
            var pMsg = new MsgLoginSvAuthRequest(pBuffer);
            if (pMsg.Message != ServerKernel.HelloExpectedMsg)
            {
                pServer.Disconnect();
                return;
            }

            pServer.ConnectionState = InterServerState.MEETING_OK;
            pServer.Send(new MsgLoginSvAuthRequest(ServerKernel.HelloSendString));
            ServerKernel.Log.SaveLog(string.Format("Waiting for server [{0}] to accept connection.", pServer.IpAddress), true, LogType.WARNING);
        }
        #endregion
        #region 12 - Authentication
        [PacketHandlerType(PacketType.LOGIN_AUTH_CONFIRM)]
        public void AuthenticationRequest(GameServer pServer, byte[] pBuffer)
        {
            var pMsg = new MsgLoginSvAuthentication(pBuffer);
            if (pMsg.Username != ServerKernel.Username || pMsg.Password != ServerKernel.Password)
            {
                pServer.Send(new LoginSvResponsePacket(LoginServerResponse.LOGIN_DENIED_LOGIN));
                pServer.Disconnect();
                return;
            }

            pServer.ConnectionState = InterServerState.CONNECTED;
            pServer.Send(new LoginSvResponsePacket(LoginServerResponse.LOGIN_SUCCESSFUL));
        }
        #endregion
        #region 20 - Server Online Player
        [PacketHandlerType(PacketType.LOGIN_REQUEST_ONLINE_NUMBER)]
        public void ProcessMsgOnlineNum(GameServer pServer, byte[] pBuffer)
        {
            if (pServer.ConnectionState < InterServerState.CONNECTED)
                return;
            var pMsg = new MsgLoginSvPlayerAmount(pBuffer);
            switch (pMsg.Type)
            {
                case LoginPlayerAmountRequest.REPLY_ONLINE_AMOUNT:
                    {
                        pServer.OnlinePlayers = pMsg.Amount;
                        break;
                    }
                case LoginPlayerAmountRequest.REPLY_ONLINE_MAXAMOUNT:
                    {
                        pServer.MaxPermitedPlayers = pMsg.Amount;
                        break;
                    }
            }
        }
        #endregion
        #region 24 - Game Server Information
        [PacketHandlerType(PacketType.LOGIN_REQUEST_SERVER_INFO)]
        public void ServerInformation(GameServer pServer, byte[] pBuffer)
        {
            if (pServer.ConnectionState < InterServerState.CONNECTED)
            {
                pServer.Disconnect();
                ServerKernel.Log.SaveLog(string.Format("Disconnected[{0}]: Unauthorized.", pServer.IpAddress), true, LogType.WARNING);
                return;
            }

            var pMsg = new MsgServerInformation(pBuffer);
            pServer.ServerName = pMsg.ServerName;
            pServer.MaxPermitedPlayers = pMsg.MaxOnlinePlayers;
            pServer.OnlinePlayers = pMsg.OnlinePlayers;
            pServer.GamePort = pMsg.GamePort;

            if (!ServerKernel.OnlineServers.TryAdd(pServer.ServerName, pServer))
            {
                pServer.Disconnect();
                return;
            }

            ServerKernel.Log.SaveLog(string.Format("Server [{0}] has connected with the following information>\r\n\tPort: {1}, Online: {2}, MaxOnline: {3}", pServer.ServerName, pServer.GamePort, pServer.OnlinePlayers, pServer.MaxPermitedPlayers), true, LogType.MESSAGE);
        }
        #endregion

        public static void Report(byte[] packet)
        {
            ushort length = BitConverter.ToUInt16(packet, 0);
            ushort identity = BitConverter.ToUInt16(packet, 2);

            // Print the packet and the packet header:
            ServerKernel.Log.SaveLog(String.Format("Missing Packet Handler: {0} (Length: {1})", identity, length), true, LogType.DEBUG);
            string aPacket = "";

            for (int index = 0; index < length; index++)
                aPacket += String.Format("{0:X2} ", packet[index]);

            ServerKernel.Log.SaveLog(aPacket, false, LogType.DEBUG);
        }
    }
}
