// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Login Packet Handler.cs
// Last Edit: 2016/11/23 10:21
// Created: 2016/11/23 10:02

using System;
using System.Linq;
using DB.Entities;
using DB.Repositories;
using LoginServer.Structures;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;
using ServerCore.Security;

namespace LoginServer.Network
{
    public class LoginPacketHandler
    {
        #region 1052 - Authentication Complete
        [PacketHandlerType(PacketType.MSG_CONNECT)]
        public void HandleAuthenticationComplete(Client pClient, byte[] pBuffer)
        {
            MsgConnect pMsg = new MsgConnect(pBuffer);

        }
        #endregion
        #region 1086 - Authentication Request
        [PacketHandlerType(PacketType.MSG_ACCOUNT1)]
        public void ProcessMsgAuth(Client pClient, byte[] pMsg)
        {
            if (pClient != null && pClient.Packet != null) // check if it's alright
            {
                byte[] pPacket = pClient.Packet;
                var pType = (PacketType)BitConverter.ToInt16(pPacket, 2);

                if (BitConverter.ToUInt16(pPacket, 0) == 276 &&
                    (pType == PacketType.MSG_ACCOUNT1))
                {
                    var pRequest = new MsgAccount(pPacket, pClient.IpAddress.GetHashCode());

                    // tells the console that the user x is trying to login on server y
                    ServerKernel.Log.SaveLog(string.Format("User [{0}] is trying to login on server [{1}].",
                        pClient.Account, pRequest.Server), false, "Login_Server");

                    // let's check if user is spamming login requests
                    LoginAttemptRecord pLogin = null;
                    if (!ServerKernel.LoginAttemptRecords.TryGetValue(pClient.IpAddress, out pLogin))
                    {
                        pLogin = new LoginAttemptRecord(pClient.IpAddress);
                        ServerKernel.LoginAttemptRecords.TryAdd(pLogin.IpAddress, pLogin);
                    }

                    if (!pLogin.Enabled) // user spamming login?
                    {
                        pClient.Send(new MsgConnectEx(RejectionType.MAXIMUM_LOGIN_ATTEMPTS));
                        ServerKernel.Log.SaveLog(
                            string.Format("User [{0}] has passport denied due to exceeding login limit on IP [{1}].",
                                pRequest.Account, pClient.IpAddress), true, "Login_Server");
                        pClient.Disconnect();
                        return;
                    }

                    DbAccount pUser = new AccountRepository().SearchByName(pRequest.Account); // fetch user information
                    if (pUser != null) // user exists?
                    {
                        // yes
                        pClient.Account = pUser;

                        // check uncommon characters
                        var szPw = string.Empty;
                        foreach (var c in pRequest.Password)
                        {
                            switch (c)
                            {
                                case '-':
                                    szPw += '0';
                                    break;
                                case '#':
                                    szPw += '1';
                                    break;
                                case '(':
                                    szPw += '2';
                                    break;
                                case '"':
                                    szPw += '3';
                                    break;
                                case '%':
                                    szPw += '4';
                                    break;
                                case '\f':
                                    szPw += '5';
                                    break;
                                case '\'':
                                    szPw += '6';
                                    break;
                                case '$':
                                    szPw += '7';
                                    break;
                                case '&':
                                    szPw += '8';
                                    break;
                                case '!':
                                    szPw += '9';
                                    break;
                                default:
                                    szPw += c;
                                    break;
                            }
                        }

                        bool bSuccess = true;
                        // check if user has input the right password
                        if (pUser.Password != WhirlpoolHash.Hash(szPw))
                        {
                            // invalid pw
                            pClient.Send(new MsgConnectEx(RejectionType.INVALID_PASSWORD));
                            ServerKernel.Log.SaveLog(
                                string.Format("User [{0}] entered an invalid password [{1}].", pUser.Username,
                                    pClient.IpAddress), true, "LoginServer");
                            pClient.Disconnect();
                            return;
                        }

                        if (pUser.Lock > 0) // user is banned?
                        {
                            if (pUser.Lock >= 3 || pUser.LockExpire == 0 || UnixTimestamp.Timestamp() < pUser.LockExpire)
                            {
                                // banned
                                pClient.Send(new MsgConnectEx(RejectionType.ACCOUNT_BANNED));
                                ServerKernel.Log.SaveLog(
                                    string.Format("User [{0}] has passport denied due to account lock status.",
                                        pUser.Username), true, "LoginServer");
                                pClient.Disconnect();
                                return;
                            }
                        }

                        //if (pUser.Lock == 2) // user has activated account?
                        //{
                        //    pClient.Send(new MsgConnectEx(RejectionType.ACCOUNT_NOT_ACTIVATED));
                        //    ServerKernel.Log.SaveLog(
                        //        string.Format("User [{0}] has passport denied due to account inactive status.",
                        //            pUser.Username), true, "LoginServer");

                        //    pClient.Disconnect();
                        //    return;
                        //}

                        // temporary just to leave people join using any server
                        GameServer pServer = ServerKernel.OnlineServers.Values.FirstOrDefault();
                        if (pServer == null)//!ServerKernel.OnlineServers.TryGetValue(pRequest.Server, out pServer))
                        // server is not online
                        {
                            pClient.Send(new MsgConnectEx(RejectionType.SERVER_MAINTENANCE));
                            ServerKernel.Log.SaveLog(
                                string.Format("User [{0}] tried to login on a invalid server [{1}].", pUser.Username,
                                    pRequest.Server), true, "LoginServer");

                            pClient.Disconnect();
                            return;
                        }

                        uint dwHash = (uint)ThreadSafeRandom.RandGet(1000, int.MaxValue);

                        var pTransferCipher = new TransferCipher(ServerKernel.LoginTransferKey,
                            ServerKernel.LoginTransferSalt, pClient.IpAddress);
                        var pCrypto = pTransferCipher.Encrypt(new[] { pUser.Identity, dwHash });

                        string szAddress = "135.12.15.139"; // random ip just to connect
                        if (!pServer.IpAddress.StartsWith("127") && pServer.IpAddress != "localhost")
                            szAddress = pServer.IpAddress;

                        pServer.Send(new MsgUsrLogin(pUser.Identity, dwHash) { IpAddress = pClient.IpAddress });

                        pClient.Send(new MsgConnectEx(pCrypto[0], pCrypto[1], szAddress, 5816));
                        ServerKernel.Log.SaveLog(string.Format("User [{0}] has successfully logged into {1}({2}:{3}).",
                            pUser.Username, pRequest.Server, szAddress, pServer.GamePort), true, "Login_Server", LogType.MESSAGE);

                        pUser.LastLogin = UnixTimestamp.Timestamp();
                        new AccountRepository().SaveOrUpdate(pUser);
                        return;
                    }
                    else
                    {
                        // no
                        pClient.Send(new MsgConnectEx(RejectionType.INVALID_PASSWORD));
                        ServerKernel.Log.SaveLog(
                            string.Format("User [{0}] doesn't exist. Connection [{1}].", pRequest.Account,
                                pClient.IpAddress), true, "Login_Server");
                    }
                }
                else
                {
                    pClient.Send(new MsgConnectEx(RejectionType.INVALID_AUTHENTICATION_PROTOCOL));
                    ServerKernel.Log.SaveLog(string.Format("User has tried to connect with an invalid protocol at {0}.", pClient.IpAddress));
                }

                pClient.Disconnect();
            }
        }
        #endregion
        #region 1100 - Mac Address
        [PacketHandlerType(PacketType.MSG_PC_NUM)]
        public void ProcessMacAddress(Client pClient, byte[] pBuffer)
        {
            var pMsg = new MsgMacAddr(pBuffer);
            new LoginRcdRepository().SaveOrUpdate(new DbLoginRcd
            {
                UserIdentity = pClient.Account.Identity,
                IpAddress = pClient.IpAddress,
                MacAddress = pMsg.MacAddress,
                LoginTime = UnixTimestamp.Timestamp(),
                ResourceSource = "2"
            });

            DbAccount pUser = new AccountRepository().SearchByIdentity(pClient.Account.Identity); // fetch user information
            if (pUser != null)
            {
                pUser.MacAddress = pMsg.MacAddress;
                new AccountRepository().SaveOrUpdate(pUser);
            }
        }
        #endregion

        /// <summary>
        /// This function reports a missing packet handler to the console. It writes the length and type of the
        /// packet, then a packet dump to the console.
        /// </summary>
        /// <param name="packet">The packet buffer being reported.</param>
        public static void Report(byte[] packet)
        {
            ushort length = BitConverter.ToUInt16(packet, 0);
            ushort identity = BitConverter.ToUInt16(packet, 2);

            // Print the packet and the packet header:
            ServerKernel.Log.SaveLog(String.Format("Missing Packet Handler: {0} (Length: {1})", identity, length), true, "missing_packet", LogType.DEBUG);
            string aPacket = "";

            for (int index = 0; index < length; index++)
                aPacket += String.Format("{0:X2} ", packet[index]);

            ServerKernel.Log.SaveLog(aPacket, true, "missing_packet", LogType.DEBUG);
        }
    }
}