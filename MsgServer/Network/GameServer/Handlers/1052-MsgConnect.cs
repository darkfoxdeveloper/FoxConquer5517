// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1052 - MsgConnect.cs
// Last Edit: 2016/11/24 10:26
// Created: 2016/11/23 11:35

using System;
using DB.Entities;
using DB.Repositories;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;
using ServerCore.Security;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        /// <summary>
        /// Packet Type: 1052. This packet is sent as a response to the account server's authentication reply to 
        /// complete the authentication process. This packet contains the client's identification data and the client's
        /// region and patch number. The client should be authenticated by the message server and connected with the
        /// map server.
        /// </summary>
        /// <param name="pClient">The client requesting access to the server.</param>
        /// <param name="pMsg">The authentication packet requesting processing.</param>
        public static void HandleConnect(Client pClient, MsgConnect pMsg)
        {
            if (pClient.Identity == 0) // not authenticated
            {
                if (ServerKernel.LoginServer == null) // if login server is down, no login allowed
                {
                    pClient.Disconnect();
                    return;
                }

                // Decrypt important client data from the Account Server:
                var pCipher = new PhoenixTransferCipher(ServerKernel.TransferKey, ServerKernel.TransferSalt, pClient.IpAddress);
                var pDecrypted = pCipher.Decrypt(new[] { pMsg.Identity, pMsg.Authentication });

                // let's check if the player has really requested the login into the account server
                LoginRequest pRequest;
                if (ServerKernel.LoginQueue.TryRemove(pDecrypted[0], out pRequest))
                {
                    // user found
                    if (!pRequest.IsValid(pDecrypted[0], pDecrypted[1], pClient.IpAddress))
                    {
                        // something is not right, disconnect.
                        pClient.Disconnect();
                        return;
                    }
                }
                else
                {
                    Client trash;
                    if (!ServerKernel.CharacterCreation.TryRemove(pDecrypted[0], out trash))
                    {
                        // user did not request a login token on the account server
                        pClient.Disconnect();
                        return;
                    }
                }

                // Assign authentication data to the client:
                pClient.AccountIdentity = pMsg.Identity = pDecrypted[0];
                pClient.Language = pMsg.Language;
                //pClient.VipLevel = pRequest.VipLevel;
                //pClient.Authority = pRequest.Authority;

                
                InitializeCharacter(pClient, pMsg);
            }
            else
            {
                InitializeCharacter(pClient, pMsg);
            }
        }

        /// <summary>
        /// This function is called once the client has been authenticated. If the character for the client 
        /// does not exist in the database, then this function will open the character creation screen; else,
        /// it will load character data and send the client information needed to log into the world. It will
        /// also connect the player to the map server.
        /// </summary>
        /// <param name="pClient">The client being processed.</param>
        /// <param name="pPacket">The authentication packet to be sent to the map server.</param>
        private static void InitializeCharacter(Client pClient, MsgConnect pPacket, bool bCreate = false)
        {
            // check if the server is full
            if (ServerKernel.Players.Count >= ServerKernel.MaxOnlinePlayer)
            {
                pClient.Send(ServerMessages.Login.ServerFull);
                pClient.Disconnect();
                return;
            }

            DbAccount pAccount = new AccountRepository().SearchByIdentity(pClient.AccountIdentity);
            DbUser pUser = new CharacterRepository().SearchByAccount(pClient.AccountIdentity);

            if (pAccount != null && pUser != null)
            {
                if (pUser.Life <= 0)
                    pUser.Life = 1;

                pClient.VipLevel = pAccount.Vip;
                pClient.Identity = pUser.Identity;
                pClient.AccountIdentity = pAccount.Identity;

                if (CheckGamePool(pClient, pClient.Identity)
                    && ServerKernel.Players.TryAdd(pClient.Identity, pClient))
                {
                    // The client is ready and authorized for login. Prepare the client for the game world: load
                    // spawn information and send it to the map server, and initialize game structures necessary
                    // to log into the server.
                    if (!bCreate)
                        pClient.Send(ServerMessages.Login.AnswerOk);

                    pClient.Send(new MsgUserIpInfo());
                    pClient.Send(new MsgServerInfo() { ClassicMode = 0, PotencyMode = 0 } );

                    if (pUser.Lookface < 10000)
                    {
                        ushort body = (ushort) (pUser.Lookface%10000);
                        uint lookface = 0;
                        if (body == (ushort) BodyType.THIN_MALE || body == (ushort) BodyType.HEAVY_MALE)
                        {
                            if ((pUser.Profession/10) == 5)
                            {
                                lookface = (uint) (new Random().Next(103, 107));
                            }
                            else if ((pUser.Profession/10) == 6)
                            {
                                lookface = (uint) (new Random().Next(109, 113));
                            }
                            else
                            {
                                lookface = (uint) (new Random().Next(1, 102));
                            }
                        }
                        else
                        {
                            if ((pUser.Profession/10) == 5)
                            {
                                lookface = (uint) (new Random().Next(291, 295));
                            }
                            else if ((pUser.Profession/10) == 6)
                            {
                                lookface = (uint) (new Random().Next(300, 304));
                            }
                            else
                            {
                                lookface = (uint) (new Random().Next(201, 290));
                            }
                        }
                        pUser.Lookface = lookface*10000 + pUser.Lookface;
                    }

                    var pInfo = new MsgUserInfo(pUser.Name, string.Empty, pUser.Mate)
                    {
                        Agility = pUser.Agility,
                        AncestorProfession = (byte)pUser.FirstProfession,
                        Attributes = pUser.AdditionalPoints,
                        BoundEmoney = pUser.BoundEmoney,
                        ConquerPoints = pUser.Emoney,
                        Enlighten = pUser.EnlightPoints,
                        EnlightenExp = 0,
                        Experience = pUser.Experience,
                        Hairstyle = pUser.Hair,
                        Health = pUser.Life,
                        Mana = pUser.Mana,
                        Identity = pUser.Identity,
                        Level = pUser.Level,
                        Mesh = pUser.Lookface,
                        Vitality = pUser.Vitality,
                        Spirit = pUser.Spirit,
                        Metempsychosis = pUser.Metempsychosis,
                        Strength = pUser.Strength,
                        QuizPoints = pUser.StudentPoints,
                        Silver = pUser.Money,
                        PreviousProfession = (byte)pUser.LastProfession,
                        Profession = (byte)pUser.Profession,
                        PlayerTitle = pUser.SelectedTitle,
                        PkPoints = pUser.PkPoints
                    };

                    pClient.Character = new Character(pInfo, pUser, pClient);

                    pClient.Send(pInfo);
                    pClient.Send(new MsgData());
                    pClient.Screen = new Screen(pClient.Character);

                    if (ServerKernel.Players.Count > ServerKernel.OnlineRecord)
                        ServerKernel.OnlineRecord = (ushort)ServerKernel.Players.Count;

                    ServerKernel.Log.SaveLog(string.Format("User [{0}] has logged in.", pClient.Character.Name), true, LogType.MESSAGE);
                    ServerKernel.LoginServer.Send(new MsgLoginSvPlayerAmount((ushort)ServerKernel.Players.Count, LoginPlayerAmountRequest.REPLY_ONLINE_AMOUNT));

                    pUser.LastLogin = (uint) UnixTimestamp.Timestamp();
                    Database.Characters.SaveOrUpdate(pUser);
                }
            }
            else if (pAccount != null
                && CheckGamePool(pClient, 0)
                && ServerKernel.CharacterCreation.TryAdd(pClient.AccountIdentity, pClient))
            {
                pClient.Send(ServerMessages.Login.NewRole);
            }
            else
            {
                pClient.Send(ServerMessages.Login.TransferFailed);
                pClient.Disconnect();
            }
        }

        /// <summary>
        /// This function checks the client pools on the server to ensure that the client is not already logged in.
        /// If the client is logged in, this function will send an error message. If the two clients have different
        /// IP addresses, the error message will reflect that with an "Account in Jeopardy" warning; else, a
        /// household warning will be sent. Both clients will be disconnected. It returns true if the client is 
        /// authenticated by the pool.
        /// </summary>
        /// <param name="pClient">The client being authenticated by the game pool.</param>
        /// <param name="dwCharacter">The character identity.</param>
        private static bool CheckGamePool(Client pClient, uint dwCharacter)
        {
            Client pOffending;
            if (ServerKernel.Players.TryRemove(dwCharacter, out pOffending)
                || ServerKernel.CharacterCreation.TryRemove(pClient.AccountIdentity, out pOffending)
                || ServerKernel.CharacterCreation.TryRemove(pClient.Identity, out pOffending))
            {
                if (pOffending.Character != null)
                {
                    pOffending.SendMessage(string.Format("Security Issue: The IP [{0}] is trying to login your account. Please change your account settings.", pClient.IpAddress), ChatTone.TALK);
                    pOffending.Disconnect();
                }
                pClient.Disconnect();
                return false;
            }
            return true;
        }

        /// <summary>
        /// This handler is located in the 1052 file. It is used to disconnect the client after being rejected from
        /// the server. It can also be used when authenticating clients during world security cycles.
        /// </summary>
        public static void DisconnectWithMsg(Client pClient, byte[] pBuffer)
        {
            pClient.Send(pBuffer);
            pClient.Disconnect();
        }
    }
}