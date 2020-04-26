// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1004 - MsgTalk.cs
// Last Edit: 2016/11/24 11:02
// Created: 2016/11/24 11:02

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DB.Entities;
using DB.Repositories;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Society;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    /// <summary>
    /// This class encapsulates packet handlers for the server. Received packets are processed by the packet
    /// processing red-black tree structure in the packet handler class. The packet handler directs the packet
    /// to the correct packet handling function, found in this class.
    /// </summary>
    public static partial class Handlers
    {
        /// <summary>
        /// This function handles the chat message for a character. If the message has not been modified, the
        /// message will be processed and distributed to the rest of the server. If the message is a command,
        /// it will be processed by the server.
        /// </summary>
        /// <param name="pClient">The client being processed.</param>
        /// <param name="pMsg">The packet being processed.</param>
        public static void ProcessChatMessage(Client pClient, MsgTalk pMsg)
        {
            // Error check the message to ensure it really is coming from the sender:
            if (pClient == null || !pClient.IsOnline())
            {
                if (pClient != null)
                    pClient.Disconnect();
                return;
            }

            // Is the message being sent a command for the server?
            if (/*(pClient.Character.IsGm || pClient.Character.IsPm) && */pMsg.Message.StartsWith(@"/"))
            {
                PerformCommand(pClient, pMsg);
                if (pClient.Character.IsGm)
                    return;
            }
            // The message is not a command. Handle it by the channel sent on:
            // If the sender name contains GM or PM, store his message on the log.
            if (pClient.Character.IsGm || pClient.Character.IsPm)
            {
                ServerKernel.Log.GmLog("gm_chat", string.Format("[{0}] -> {1} to {2} -> {3}",
                    pMsg.Tone, pClient.Character.Name, pMsg.Recipient, pMsg.Message), true);
            }
            else if (pMsg.Recipient.Contains("[GM]") || pMsg.Recipient.Contains("[PM]"))// If someone tries to contact a GM or PM, store his message on the log.
            {
                ServerKernel.Log.GmLog("gm_chat", string.Format("[{0}] -> {1} to {2} -> {3}",
                    pMsg.Tone, pMsg.Sender, pMsg.Recipient, pMsg.Message), true);
            }

            if (pClient.Character.IsSilenced)
            {
                pClient.SendMessage(string.Format(ServerString.STR_SILENCED_FOR_A_WHILE, pClient.Character.SilenceRemaining));
                return;
            }

            switch (pMsg.Tone)
            {
                case ChatTone.TALK:
                    ProcessTalk(pClient, pMsg);
                    ServerKernel.Log.GmLog("chat_talk", string.Format("{0} speaks to {1}: {2}", pMsg.Sender, pMsg.Recipient, pMsg.Message));
                    break;
                case ChatTone.GHOST:
                    ProcessGhost(pClient, pMsg);
                    ServerKernel.Log.GmLog("chat_ghost", string.Format("{0} speaks to {1}: {2}", pMsg.Sender, pMsg.Recipient, pMsg.Message));
                    break;
                case ChatTone.WHISPER:
                    ProcessWhisper(pClient, pMsg);
                    ServerKernel.Log.GmLog("chat_whisper", string.Format("{0} speaks to {1}: {2}", pMsg.Sender, pMsg.Recipient, pMsg.Message));
                    break;
                case ChatTone.TEAM:
                    ProcessTeam(pClient, pMsg);
                    ServerKernel.Log.GmLog("chat_team", string.Format("{0} speaks to {1}: {2}", pMsg.Sender, pMsg.Recipient, pMsg.Message));
                    break;
                case ChatTone.GUILD:
                    ProcessGuild(pClient, pMsg);
                    ServerKernel.Log.GmLog("chat_syndicate", string.Format("{0} speaks to {1}: {2}", pMsg.Sender, pMsg.Recipient, pMsg.Message));
                    break;
                case ChatTone.FRIEND:
                    ProcessFriend(pClient, pMsg);
                    ServerKernel.Log.GmLog("chat_friend", string.Format("{0} speaks to {1}: {2}", pMsg.Sender, pMsg.Recipient, pMsg.Message));
                    break;
                case ChatTone.SERVICE:
                    ProcessSystem(pClient, pMsg);
                    ServerKernel.Log.GmLog("chat_service", string.Format("{0} speaks to {1}: {2}", pMsg.Sender, pMsg.Recipient, pMsg.Message));
                    break;
                case ChatTone.FAMILY:
                    ProcessFamily(pClient, pMsg);
                    ServerKernel.Log.GmLog("chat_family", string.Format("{0} speaks to {1}: {2}", pMsg.Sender, pMsg.Recipient, pMsg.Message));
                    break;
                case ChatTone.GUILD_ANNOUNCEMENT:
                    if (pClient.Character.Syndicate != null)
                    {
                        Syndicate syn = pClient.Character.Syndicate;
                        if (pClient.Character.SyndicateRank != SyndicateRank.GUILD_LEADER)
                            return;
                        syn.Announcement = pMsg.Message;
                        syn.Send(pMsg);
                    }
                    break;
                case ChatTone.VENDOR_HAWK:
                    {
                        if (pClient.Character.Booth != null && pClient.Character.Booth.Vending)
                        {
                            pClient.Character.Booth.HawkMessage = pMsg.Message;
                            pClient.Screen.Send(pMsg, false);
                            ServerKernel.Log.GmLog("chat_vendor", string.Format("{0} speaks to {1}: {2}", pMsg.Sender, pMsg.Recipient, pMsg.Message));
                        }
                        break;
                    }
                case ChatTone.WORLD:
                {
                    ProcessWorld(pClient, pMsg);
                        ServerKernel.Log.GmLog("chat_world", string.Format("{0} speaks to {1}: {2}", pMsg.Sender, pMsg.Recipient, pMsg.Message));
                        break;
                }
                default:
                    ServerKernel.Log.SaveLog("Unhandled message type: " + pMsg.Tone, true, LogType.WARNING);
                    break;
            }
        }

        /// <summary>
        /// This function handles chat messages from the client for the talk channel. Speech from this channel is 
        /// distributed to the clients in the actor's screen.
        /// </summary>
        /// <param name="pClient">The client being processed.</param>
        /// <param name="pMsg">The message being processed.</param>
        private static void ProcessTalk(Client pClient, MsgTalk pMsg)
        {
            if (pClient == null || pClient.Character == null) return;
            pClient.Screen.Send(pMsg, false);
        }

        private static void ProcessGhost(Client pClient, MsgTalk pMsg)
        {
            if (pClient == null || pClient.Character == null) return;
            if (pClient.Character.IsAlive) return;

            List<IRole> around = pClient.Character.Screen.GetAroundPlayers;
            foreach (var player in around)
            {
                if (player.IsAlive && player.Profession/10 < 10 && player.Profession/10 != 6) continue;
                player.Send(pMsg);
            }
        }

        private static void ProcessWhisper(Client pClient, MsgTalk pMsg)
        {
            if (pClient == null || pClient.Character == null) return;

            var client = ServerKernel.Players.Values.FirstOrDefault(x => x.Character != null && x.Character.Name == pMsg.Recipient);
            if (client != null)
            {
                Character target = client.Character;
                if (target == null)
                {
                    pClient.SendMessage(ServerString.STR_TARGET_OFFLINE, ChatTone.TALK);
                    return;
                }

                pMsg.RecipientMesh = target.Lookface;
                pMsg.SenderMesh = pClient.Character.Lookface;
                target.Send(pMsg);
            }

            if (client == null)
            {
                pClient.SendMessage(ServerString.STR_TARGET_OFFLINE, ChatTone.TALK);
                return;
            }
        }

        private static void ProcessTeam(Client pClient, MsgTalk pMsg)
        {
            if (pClient.Character == null || pClient.Character.Team == null)
                return;
            pClient.Character.Team.Send(pMsg, pClient.Character.Identity);
        }

        private static void ProcessGuild(Client pClient, MsgTalk pMsg)
        {
            if (pClient.Character == null || pClient.Character.Syndicate == null)
                return;
            pClient.Character.Syndicate.Send(pMsg, pClient.Character.Identity);
        }

        private static void ProcessFriend(Client pClient, MsgTalk pMsg)
        {
            if (pClient == null || pClient.Character == null || pClient.Character.Friends.Count <= 0)
                return;

            foreach (var friend in pClient.Character.Friends.Values)
            {
                if (friend.IsOnline)
                    friend.User.Send(pMsg);
            }
        }

        private static void ProcessSystem(Client pClient, MsgTalk pMsg)
        {
            if (pClient == null || pClient.Character == null) return;

            int nAmount = 0;
            foreach (var target in ServerKernel.Players.Values.Where(x => x.Character.IsGm))
            {
                pMsg.Tone = ChatTone.WHISPER;
                pMsg.Recipient = target.Character.Name;
                ProcessWhisper(pClient, pMsg);
                nAmount++;
            }

            if (nAmount <= 0)
            {
                pClient.SendMessage(ServerString.STR_NO_GM_ONLINE, ChatTone.TIP);
                ServerKernel.Log.SaveLog(string.Format("Player({0}) left message to service: {1}", pClient.Character.Name, pMsg.Message), true, LogType.WARNING);
            }
        }

        private static void ProcessWorld(Client pClient, MsgTalk pMsg)
        {
            if (pClient == null || pClient.Character == null || !pClient.Character.IsAlive)
                return;

            if (!pClient.Character.IsWorldChatEnable)
            {
                //if (pClient.Character.Level < 70)
                //{
                //    return;
                //}
                return;
            }

            ServerKernel.SendMessageToAll(pMsg);
        }

        private static void ProcessFamily(Client pClient, MsgTalk pMsg)
        {
            if (pClient == null || pClient.Character == null || pClient.Character.Family == null)
                return;
            pClient.Character.Family.Send(pMsg, pClient.Character.Identity);
        }

        public static void PerformCommand(Client pClient, MsgTalk pMsg)
        {
            try
            {
                ServerKernel.Log.GmLog("gm_command", string.Format("{0} => {1}", pClient.Character.Name, pMsg.Message));

                string[] command = pMsg.Message.Split(new[] { ' ' }, 3); // Limited to 3. Not change this or the commands not working correctly.
                string[] commandFullParameters = pMsg.Message.Split(' '); // Use this for working with all parameters

                if (command.Length <= 0) return;
                Character pRole = pClient.Character;

                if (pRole.IsPm)
                {
                    switch (command[0])
                    {
                        #region Test War NPC
                        case "/testwarnpc":
                            {
                                Random temp = new Random();
                                int x = 0;
                                int y = 0;
                                for (int i = 52; i < 60; i++)
                                {
                                    MsgNpcInfoEx pBuffer = new MsgNpcInfoEx
                                    {
                                        Identity = (uint)(temp.Next(400000, 500000)),
                                    };
                                    pBuffer.Lookface = 8686;
                                    pBuffer.Life = 20000000;
                                    pBuffer.MaxLife = 20000000;
                                    pBuffer.MapX = (ushort)(pClient.Character.MapX - 9 + (x += 3));
                                    if (x > 6) { y++; x = 0; }
                                    pBuffer.MapY = (ushort)(pClient.Character.MapY - 9 + (y += 3));
                                    pBuffer.Flag = (ushort)i;
                                    pBuffer.Type = 17;
                                    pBuffer.Name = "Flag"+i;
                                    Console.WriteLine("{0} {1} {2}:{3} Offset: {4}", pBuffer.Identity, pBuffer.Name, pBuffer.MapX, pBuffer.MapY, i);
                                    pClient.Send(pBuffer);
                                }
                                pClient.Send(new MsgTalk(string.Format("Initial pos: {0}:{1}", pClient.Character.MapX, pClient.Character.MapY), ChatTone.TALK));
                                break;
                            }
                        #endregion
                        #region Attach Status
                        case "/attachstatus":
                            {
                                if (command.Length < 2) return;
                                byte status = byte.Parse(command[1]);
                                pRole.AttachStatus(pRole, status, 0, 30, 0, 0, pRole.Identity);
                                break;
                            }
                        #endregion
                        #region Test Spawn Packet
                        case "/spawnpacket":
                            {
                                //List<uint[]> flowerCharms = new List<uint[]>(4);
                                //uint[] roses =
                                //{
                                //    30010002, 30010002, 30020002, 30030002, 30040002, 30100002, 30110002, 30300002,
                                //    30010001, 30010001, 30020001, 30030001, 30040001, 30100001, 30110001, 30300001
                                //};
                                //uint[] white =
                                //{
                                //    30010102, 30010102, 30020102, 30030102, 30040102, 30100102, 30110102, 30300102,
                                //    30010101, 30010101, 30020101, 30030101, 30040101, 30100101, 30110101, 30300101
                                //};
                                //uint[] orchids =
                                //{
                                //    30010202, 30010202, 30020202, 30030202, 30040202, 30100202, 30110202, 30300202, 
                                //    30010201, 30010201, 30020201, 30030201, 30040201, 30100201, 30110201, 30300201
                                //};
                                //uint[] tulips =
                                //{
                                //    30010302, 30010302, 30020302, 30030302, 30040302, 30100302, 30110302, 30300302, 
                                //    30010301, 30010301, 30020301, 30030301, 30040301, 30100301, 30110301, 30300301
                                //};
                                //flowerCharms.Add(roses);
                                //flowerCharms.Add(white);
                                //flowerCharms.Add(orchids);
                                //flowerCharms.Add(tulips);

                                Random temp = new Random();

                                //for (int i = 0; i < 4; i++)
                                //{
                                //    for (int j = 0; j < 16; j++)
                                //    {
                                //        CharacterSpawn pBuffer = new CharacterSpawn((uint)(temp.Next(1100000, 2000000)));
                                //        pBuffer.Action = EntityAction.SIT;
                                //        pBuffer.Armor = 135109;
                                //        pBuffer.ArmorColor = 2;
                                //        pBuffer.Helmet = 113109;
                                //        pBuffer.HelmetColor = 2;
                                //        pBuffer.Garment = 181355;
                                //        pBuffer.Direction = FacingDirection.SOUTH_WEST;
                                //        pBuffer.GuildIdentity = 100;
                                //        pBuffer.GuildRank = SyndicateRank.GUILD_LEADER;
                                //        pBuffer.Mesh = 1031003;
                                //        pBuffer.Metempsychosis = 2;
                                //        pBuffer.FlowerRankingDict = flowerCharms[i][j];
                                //        pBuffer.Nobility = 12;
                                //        pBuffer.QuizPoints = 1500;
                                //        pBuffer.MapX = (ushort)(pClient.Character.MapX + i * 2);
                                //        pBuffer.MapY = (ushort)(pClient.Character.MapY + j);
                                //        pBuffer.Life = 10000;
                                //        pBuffer.Level = 130;
                                //        pBuffer.LeftHand = 601339;
                                //        pBuffer.RightHand = 601339;
                                //        pBuffer.StringCount = 1;
                                //        pBuffer.Name = string.Format("Test{0}-{1}[PM]", i, j);

                                //        pClient.Send(pBuffer);
                                //    }
                                //}

                                //MsgPlayer pBuffer = new MsgPlayer((uint)(900000 + pRole.MapX * 100 + pRole.MapY));
                                ////pBuffer.Action = EntityAction.SIT;
                                ////pBuffer.Armor = 0;
                                ////pBuffer.ArmorColor = 2;
                                ////pBuffer.Helmet = 0;
                                ////pBuffer.HelmetColor = 0;
                                ////pBuffer.Hairstyle = 200;
                                ////pBuffer.Garment = 0;
                                ////pBuffer.Direction = FacingDirection.SOUTH_WEST;
                                ////pBuffer.GuildIdentity = 80;
                                ////pBuffer.GuildRank = SyndicateRank.MANAGER;
                                //pBuffer.Mesh = 513;
                                ////pBuffer.Metempsychosis = 2;
                                ////pBuffer.Nobility = 9;
                                ////pBuffer.QuizPoints = 50000;
                                ////pBuffer.MapX = pClient.Character.MapX;
                                ////pBuffer.MapY = pClient.Character.MapY;
                                ////pBuffer.WriteUInt(3000000000, 80);
                                ////pBuffer.Life = 10000;
                                ////pBuffer.MonsterLevel = 250;
                                ////pBuffer.LeftHand = 0;
                                ////pBuffer.FlowerRanking = 30020302;
                                ////pBuffer.RightHand = 0;
                                //pBuffer.MapX = pRole.MapX;
                                //pBuffer.MapY = pRole.MapY;
                                //pBuffer.Level = pBuffer.MonsterLevel = 1;
                                //pBuffer.StringCount = 1;
                                //pBuffer.Name = string.Format("CTFlag", 0, 0);
                                ////pBuffer.SecondName = "312321312";
                                ////pBuffer.FamilyName = "Testando4";
                                //pBuffer.FamilyIdentity = 3;
                                //pBuffer.FamilyRank = FamilyRank.CLAN_LEADER;

                                //int x = 0;
                                //int y = 0;
                                //for (int i = 0; i < 4; i++)
                                //{
                                //    MsgPlayer pBuffer = new MsgPlayer((uint)(temp.Next(1000000, 2000000)));
                                //    //pBuffer.Action = EntityAction.DANCE7;
                                //    pBuffer.Armor = 130109;
                                //    pBuffer.ArmorColor = 5;
                                //    pBuffer.Helmet = 113109;
                                //    pBuffer.HelmetColor = 6;
                                //    //pBuffer.Flag1 = 1UL << 50;
                                //    //pBuffer.WriteByte(6, 109);
                                //    pBuffer.Hairstyle = 0;
                                //    pBuffer.Garment = 187465;
                                //    pBuffer.Direction = FacingDirection.SOUTH_EAST;
                                //    pBuffer.GuildIdentity = 100;
                                //    pBuffer.GuildRank = SyndicateRank.GUILD_LEADER;
                                //    pBuffer.Mesh = 1003;
                                //    pBuffer.Metempsychosis = 2;
                                //    pBuffer.Nobility = 12;
                                //    pBuffer.QuizPoints = 1500;
                                //    //pBuffer.MonsterLevel = 140;
                                //    //pBuffer.WriteUInt(140, 82);
                                //    //pBuffer.WriteUShort(10000, 80);
                                //    //pBuffer.Life = 10000;
                                //    //pBuffer.WriteByte(1, 181);
                                //    pBuffer.MapX = (ushort)(pClient.Character.MapX - 9 + x++ + x++);
                                //    if (x > 6) { y++; x = 0; }
                                //    pBuffer.MapY = (ushort)(pClient.Character.MapY - 9 + y);
                                //    //pBuffer.Life = 10000;
                                //    pBuffer.Level = 140;
                                //    pBuffer.LeftHand = 900108;
                                //    pBuffer.FlowerRanking = 30020302;
                                //    pBuffer.RightHand = 410058;
                                //    pBuffer.RightHandArtifact = 800110;
                                //    pBuffer.LeftHandArtifact = 800415;
                                //    pBuffer.ArmorArtifact = 822053;
                                //    pBuffer.WriteUInt((uint) i, 38);
                                //    //pBuffer.WriteUInt(100, i + 4);
                                //    pBuffer.StringCount = 1;
                                //    pBuffer.Name = string.Format("User[{0},{1}]", pBuffer.MapX, pBuffer.MapY);
                                //    //pBuffer.FamilyIdentity = 2;
                                //    //pBuffer.FamilyRank = FamilyRank.CLAN_LEADER;
                                //    //pBuffer.SecondName = "Second";
                                //    //pBuffer.FamilyName = "FamilyName";
                                //    Console.WriteLine("{0} {1} {2}:{3} Offset: {4}", pBuffer.Identity, pBuffer.Name, pBuffer.MapX, pBuffer.MapY, i);
                                //    pClient.Send(pBuffer);
                                //}
                                //pClient.Send(new MsgTalk(string.Format("Initial pos: {0}:{1}", pClient.Character.MapX, pClient.Character.MapY), ChatTone.TALK));

                                //MsgNpcInfo pBuffer = new MsgNpcInfo
                                //{
                                //    Identity = (uint)(900000+pRole.MapX*100+pRole.MapY),
                                //    Lookface = 531,
                                //    MapX = pRole.MapX,
                                //    MapY = pRole.MapY,
                                //    Sort = 0,
                                //    Kind = 0
                                //};
                                //pClient.Send(pBuffer);

                                return;
                            }
                        #endregion
                        #region Test Nobility Icon
                        case "/sendnobility":
                            {
                                string nobilityInfoString = string.Format("{0} {1} {2:d} {3}", pClient.Identity, 0, 0, -1);
                                var nPacket = new MsgPeerage(42 + nobilityInfoString.Length);
                                nPacket.WriteByte(1, 29);
                                nPacket.WriteStringWithLength(nobilityInfoString, 30);
                                nPacket.Action = NobilityAction.INFO;
                                nPacket.DataLow = pClient.Identity;
                                pClient.Send(nPacket);
                                return;
                            }
                        #endregion
                        #region Test Quiz Show Packet
                        case "/quizbuff":
                            {
                                var msg = new MsgQuiz()
                                {
                                    TimePerQuestion = 30,
                                    Type = QuizShowType.START_QUIZ
                                };
                                pRole.Send(msg);
                                msg = new MsgQuiz
                                {
                                    Type = QuizShowType.QUESTION_QUIZ,
                                    LastCorrectAnswer = 0,
                                    QuestionNumber = 1,
                                    TimeTaken = 10,
                                    ExperienceAwarded = 20,
                                    CurrentScore = 30
                                };
                                msg.WriteUShort(15, 16);
                                msg.AddString("Quem~tem~a~maior~rola~do~conquer?", "Rodrigo", "Teles", "Teles", "Rodrigo");
                                pRole.Send(msg);

                                break;
                            }
                        #endregion
                        #region Test Flower Packets
                        case "/test1150":
                            {
                                MsgFlower msg = new MsgFlower
                                {
                                    Identity = pRole.Identity,
                                    Mode = 1,
                                };
                                msg.WriteUInt(2, 16);
                                msg.WriteUInt(1, 17);
                                pRole.Send(msg);
                                break;
                            }
                        case "/test1151":
                            {
                                MsgRank msg = new MsgRank
                                {
                                    FlowerIdentity = pRole.Identity,
                                    Mode = 1
                                };
                                msg.WriteUInt(2, 12);
                                pRole.Send(msg);
                                break;
                            }
                        #endregion
                        #region Test Data Packet
                        case "/msggeneral":
                            {
                                string[] fullComm = pMsg.Message.Split(' ');
                                if (fullComm.Length < 4)
                                    return;

                                GeneralActionType genAct = (GeneralActionType)ushort.Parse(fullComm[1]);
                                uint identity = fullComm[2] == "self" ? pClient.Character.Identity : uint.Parse(fullComm[2]);
                                uint data = uint.Parse(fullComm[3]);
                                if (fullComm.Length == 4)
                                {
                                    pRole.Send(new MsgAction(identity, data, pClient.Character.MapX, pClient.Character.MapY,
                                        genAct));
                                }
                                else
                                {
                                    uint data2 = uint.Parse(fullComm[4]);
                                    var msg = new MsgAction(identity, data, pClient.Character.MapX, pClient.Character.MapY,
                                        genAct)
                                    {
                                        Details = data2
                                    };
                                    msg.WriteUInt(data, 20);
                                    pRole.Send(msg);
                                }
                                return;
                            }
                        #endregion
                        #region AddNPC
                        case "/addnpc":
                            {
                                if (commandFullParameters.Length >= 4)
                                {
                                    uint.TryParse(commandFullParameters[1], out uint NpcId);
                                    string NpcName = commandFullParameters[2];
                                    ushort.TryParse(commandFullParameters[3], out ushort NpcType);
                                    ushort.TryParse(commandFullParameters[4], out ushort NpcLookface);
                                    bool.TryParse(commandFullParameters[5], out bool NpcTemporal);

                                    DbNpc npc = new DbNpc
                                    {
                                        Id = NpcId,
                                        Name = commandFullParameters[2],
                                        Mapid = pRole.Map.Identity,
                                        Cellx = pRole.MapX,
                                        Celly = pRole.MapY,
                                        Type = NpcType,
                                        Lookface = NpcLookface,

                                    };
                                    GameNpc gameNPC = new GameNpc(npc);

                                    if (pRole.Map.GameObjects.TryAdd(gameNPC.Identity, gameNPC))
                                    {
                                        if (NpcTemporal)
                                        {
                                            pRole.Send("NPC Added (Temporal)");
                                        }
                                        else
                                        {
                                            pRole.Send("NPC Added (Temporal)");
                                            //if (gameNPC.Save())
                                            //{
                                            //    pRole.Send("NPC Added and saved in Database");
                                            //}
                                            //else
                                            //{
                                            //    pRole.Send("NPC Added. Cannot be added in Database due a error.");
                                            //}
                                        }
                                    }
                                    else
                                    {
                                        pRole.Send("NPC Cannot added. Error");
                                    }
                                } else
                                {
                                    pRole.Send("Command need parameters: /addnpc id name type lookface temporal(true/false)");
                                }
                                break;
                            }
                        #endregion
                        #region Msg Item
                        case "/msgitem":
                            {
                                if (command.Length < 3)
                                    return;

                                ItemAction pAct = (ItemAction)uint.Parse(command[1]);
                                uint idItem = uint.Parse(command[2]);

                                MsgMapItem msg = new MsgMapItem
                                {
                                    Identity = (uint)pClient.Character.Map.FloorItem,
                                    DropType = 4,
                                    ItemColor = 3,
                                    Itemtype = 410339,
                                    MapX = (ushort)(pClient.Character.MapX + 2),
                                    MapY = (ushort)(pClient.Character.MapY + 1)
                                };
                                pClient.Send(msg);
                                return;
                            }
                        #endregion
                        #region Uplev
                        case "/uplev":
                            {
                                if (command.Length < 2)
                                    return;

                                pClient.Character.AwardLevel(ushort.Parse(command[1]));
                                break;
                            }
                        #endregion
                        #region AwardMoney
                        case "/awardmoney":
                        case "/money":
                            {
                                if (command.Length < 2)
                                    return;
                                long parse = long.Parse(command[1]);
                                if (parse < 0)
                                    parse *= -1;
                                pClient.Character.ChangeMoney(parse);
                                break;
                            }
                        #endregion
                        #region Emoney/Cps
                        case "/emoney":
                            {
                                if (command.Length < 2)
                                    return;
                                long parse = long.Parse(command[1]);
                                if (parse < 0)
                                    parse *= -1;
                                pClient.Character.ChangeEmoney(parse);
                                break;
                            }
                        case "/cps":
                            {
                                if (command.Length < 2)
                                    return;
                                long parse = long.Parse(command[1]);
                                if (parse < 0)
                                    parse *= -1;
                                pClient.Character.ChangeEmoney(parse, true);
                                break;
                            }
                        #endregion
                        #region Superman
                        case "/superman":
                            {
                                // Add Strength
                                if ((pClient.Character.Strength + 500) > ushort.MaxValue)
                                    pClient.Character.ChangeForce((ushort)(ushort.MaxValue - pClient.Character.Strength));
                                else
                                    pClient.Character.ChangeForce(500);
                                // Add Agility
                                if ((pClient.Character.Agility + 500) > ushort.MaxValue)
                                    pClient.Character.ChangeSpeed((ushort)(ushort.MaxValue - pClient.Character.Agility));
                                else
                                    pClient.Character.ChangeSpeed(500);
                                //Add Vitality
                                if ((pClient.Character.Vitality + 500) > ushort.MaxValue)
                                    pClient.Character.ChangeHealth((ushort)(ushort.MaxValue - pClient.Character.Vitality));
                                else
                                    pClient.Character.ChangeHealth(500);
                                // Add Spirit
                                if ((pClient.Character.Spirit + 500) > ushort.MaxValue)
                                    pClient.Character.ChangeSoul((ushort)(ushort.MaxValue - pClient.Character.Spirit));
                                else
                                    pClient.Character.ChangeSoul(500);
                                break;
                            }
                        #endregion
                        #region Award Item
                        case "/awarditem":
                            {
                                if (command.Length < 2)
                                    return;

                                if (command.Length == 2)
                                {
                                    if (pClient.Character.Inventory.Create(uint.Parse(command[1])))
                                        pClient.Send(new MsgTalk("PM> Item created.", ChatTone.TOP_LEFT, Color.Yellow));
                                    else
                                        pClient.Send(new MsgTalk("PM> Failed to create item. " + command[1], ChatTone.TOP_LEFT, Color.Yellow));
                                }
                                break;
                            }
                        #endregion
                        #region Life
                        case "/life":
                            {
                                pClient.Character.RecalculateAttributes();
                                pClient.Character.Life = pClient.Character.MaxLife;
                                break;
                            }
                        #endregion
                        #region Mana
                        case "/mana":
                            {
                                pClient.Character.Mana = pClient.Character.MaxMana;
                                break;
                            }
                        #endregion
                        #region Action
                        case "/action":
                            {
                                if (command.Length < 2)
                                    return;
                                pClient.Character.GameAction.ProcessAction(uint.Parse(command[1]), pClient.Character, pClient.Character, null, null);
                                break;
                            }
                        #endregion
                        #region Profession
                        case "/pro":
                            {
                                if (command.Length < 2)
                                    return;

                                pClient.Character.SetProfession(ushort.Parse(command[1]));
                                break;
                            }
                        #endregion
                        #region Effect
                        case "/addeffect":
                            {
                                if (command.Length < 2)
                                    return;
                                MsgName sPacket = new MsgName();
                                sPacket.Identity = pClient.Character.Identity;
                                sPacket.Action = StringAction.ROLE_EFFECT;
                                sPacket.Append(command[1]);
                                pClient.Send(sPacket);
                                break;
                            }
                        #endregion
                        #region Stamina Points
                        case "/sp":
                            {
                                if (command.Length < 2)
                                    return;
                                byte stamina = byte.Parse(command[1]);
                                if (stamina > 150)
                                    stamina = 150;
                                pClient.Character.Stamina = stamina;
                                break;
                            }
                        #endregion
                        #region Xp
                        case "/xp":
                            {
                                pRole.SetXp(100);
                                pRole.BurstXp();
                                break;
                            }
                        #endregion
                        #region Open Dialog
                        case "/dialog":
                            {
                                if (command.Length < 2)
                                    return;
                                MsgAction act = new MsgAction(pClient.Identity, ushort.Parse(command[1]), 0, 0, GeneralActionType.OPEN_WINDOW);
                                pClient.Send(act);
                                break;
                            }
                        #endregion
                        #region Open Interface
                        case "/interface":
                            {
                                if (command.Length < 2)
                                    return;
                                MsgAction act = new MsgAction(pClient.Identity, ushort.Parse(command[1]), 0, 0, GeneralActionType.OPEN_CUSTOM);
                                pClient.Send(act);
                                break;
                            }
                        #endregion
                        #region Award Exp
                        case "/awardexp":
                            {
                                if (command.Length < 2)
                                    return;
                                pClient.Character.AwardExperience(long.Parse(command[1]));
                                break;
                            }
                        #endregion
                        #region Award Magic
                        case "/awardmagic":
                            {
                                if (command.Length < 2)
                                    return;
                                ushort magic = ushort.Parse(command[1]);
                                byte level = 0;
                                if (command.Length >= 3)
                                    level = byte.Parse(command[2]);

                                if (pClient.Character != null)
                                    pClient.Character.Magics.Create(magic, level);
                                return;
                            }
                        #endregion
                        #region Rebirth
                        case "/rebirth":
                            {
                                //if (command.Length == 1)
                                //    pClient.Character.Rebirth((ushort)(pClient.Character.Profession / 10 * 10 + 1), 0);
                                //if (command.Length == 2)
                                //    pClient.Character.Rebirth(ushort.Parse(command[1]), 0);
                                //if (command.Length >= 3)
                                //    pClient.Character.Rebirth(ushort.Parse(command[1]), ushort.Parse(command[2]));
                                return;
                            }
                        #endregion
                        #region Award Exp Ball

                        case "/awardexpball":
                            pRole.AwardExperience(ServerKernel.GetExpBallExperience(pRole.Level));
                            break;

                        #endregion
                        #region Check Char Status
                        case "/userstatus":
                            {
                                pRole.Send(string.Format("Total Battle Power: {0} Pure Battle Power: {1} Mentor Battle Power: {2}", pRole.BattlePower, pRole.BattlePower - 0, 0/*pRole.SharedBattlePower, pRole.SharedBattlePower*/), ChatTone.TALK);
                                pRole.Send(string.Format("HP:{0} MP:{1} MaxLife:{2} MaxMana:{3}", pRole.Life, pRole.Mana, pRole.MaxLife, pRole.MaxMana), ChatTone.TALK);
                                pRole.Send(string.Format("Atk:{0}/{5} MAtk: {4} Def:{1} MDef:{2} MDefBonus:{3}", pRole.MinAttack, pRole.Defense, pRole.MagicDefense, pRole.MagicDefenseBonus, pRole.MagicAttack, pRole.MaxAttack), ChatTone.TALK);
                                pRole.Send(string.Format("Dex:{0} HitRate:{1}", pRole.Dexterity, pRole.AttackHitRate), ChatTone.TALK);
                                pRole.Send(string.Format("Final Atk:{0} MAtk:{1} Def:{2} MDef:{3}", pRole.AddFinalAttack, pRole.AddFinalMagicAttack, pRole.AddFinalDefense, pRole.AddFinalMagicDefense), ChatTone.TALK);
                                break;
                            }
                        #endregion
                        #region Bless
                        case "/bless":
                            {
                                if (command.Length < 2) return;
                                pClient.Character.UpdateClient(ClientUpdateType.ONLINE_TRAINING, byte.Parse(command[1]), false);
                                return;
                            }
                        #endregion
                        #region Reload Action
                        case "/reloadaction":
                            {
                                if (command.Length < 2)
                                    return;

                                uint idAction = uint.Parse(command[1]);

                                ActionStruct pAct = ServerKernel.GameActions.Values.FirstOrDefault(x => x.Id == idAction);
                                if (pAct == null)
                                    return;

                                DbGameAction newAct = new GameActionRepo().GetById(idAction);
                                if (newAct == null) return;

                                if (ServerKernel.GameActions.Remove(idAction))
                                {
                                    pAct = new ActionStruct
                                    {
                                        Id = newAct.Identity,
                                        IdNext = newAct.IdNext,
                                        IdNextfail = newAct.IdNextfail,
                                        Type = newAct.Type,
                                        Data = newAct.Data,
                                        Param = newAct.Param
                                    };
                                    ServerKernel.GameActions.Add(pAct.Id, pAct);
                                }
                                return;
                            }
                        #endregion
                        #region Check Interaction
                        case "/msginteract":
                        {
                            if (command.Length < 3)
                            {
                                return;
                            }
                            InteractionType pType = (InteractionType) uint.Parse(command[1]);
                            uint dwVal = uint.Parse(command[2]);
                            MsgInteract pMsg0 = new MsgInteract
                            {
                                EntityIdentity = pRole.Identity,
                                TargetIdentity = pRole.Identity,
                                Action = pType,
                                Data = dwVal
                            };
                            pRole.Send(pMsg0);
                            break;
                        }
                        #endregion
                        #region Award Study
                        case "/awardstudy":
                        {
                            if (command.Length < 2)
                                return;
                            long parse = long.Parse(command[1]);
                            if (parse < 0)
                                parse *= -1;
                            pClient.Character.StudyPoints += (uint) parse;
                            MsgSubPro psmsm = new MsgSubPro();
                            psmsm.Action = SubClassActions.UPDATE_STUDY;
                            psmsm.WriteUShort((ushort) parse, 14);
                            pRole.Send(psmsm);
                            break;
                        }
                        #endregion
                        #region Create Generator
                        case "/creategen":
                        {
                            pRole.Send("Attention, use this command only on localhost tests or the generator thread may crash.");
                            // mobid mapid mapx mapy boundcx boundcy maxnpc rest maxpergen
                            string[] szComs = pMsg.Message.Split(' ');
                            if (szComs.Length < 10)
                            {
                                pRole.Send("/creategen mobid mapid mapx mapy boundcx boundcy maxnpc rest maxpergen");
                                return;
                            }

                            ushort idMob = ushort.Parse(szComs[1]);
                            uint idMap = uint.Parse(szComs[2]);
                            ushort mapX = ushort.Parse(szComs[3]);
                            ushort mapY = ushort.Parse(szComs[4]);
                            ushort boundcx = ushort.Parse(szComs[5]);
                            ushort boundcy = ushort.Parse(szComs[6]);
                            ushort maxNpc = ushort.Parse(szComs[7]);
                            ushort restSecs = ushort.Parse(szComs[8]);
                            ushort maxPerGen = ushort.Parse(szComs[9]);

                            DbGenerator newGen = new DbGenerator
                            {
                                Mapid = idMap,
                                Npctype = idMob,
                                BoundX = mapX,
                                BoundY = mapY,
                                BoundCx = boundcx,
                                BoundCy = boundcy, 
                                MaxNpc = maxNpc,
                                RestSecs = restSecs,
                                MaxPerGen = maxPerGen,
                                BornX = 0,
                                BornY = 0, 
                                TimerBegin = 0, 
                                TimerEnd = 0
                            };

                            if (!new GeneratorRepository().SaveOrUpdate(newGen))
                            {
                                pRole.Send("Could not save generator.", ChatTone.TALK);
                                return;
                            }

                            Generator pGen = new Generator(newGen);
                            pGen.Create();
                            ServerKernel.Generators.Add(pGen);
                            pGen.FirstGeneration();
                            break;
                        }
                        #endregion
                        #region Find Monster
                        case "/findmob":
                        {
                            foreach (var mob in pRole.Map.GameObjects.Values)
                            {
                                if (mob.Name == command[1])
                                    pRole.Send(string.Format("Name: {0}, MapX: {1}, MapY: {2}", mob.Name, mob.MapX, mob.MapY), ChatTone.TALK);
                            }
                            return;
                        }
                        #endregion
                        #region Test MsgTalk
                        case "/msgtalk":
                        {
                            if (command.Length < 3)
                                return;

                            ChatTone tone = (ChatTone)ushort.Parse(command[1]);
                            string msg = command[2];

                            pClient.Send(new MsgTalk(msg, tone, Color.Yellow));
                            return;
                        }
                        #endregion
                        #region Test WarFlag
                        case "/warflag":
                        {
                            if (command.Length < 2)
                                return;

                            MsgWarFlag pWarFlag = new MsgWarFlag();
                            pWarFlag.Type = (WarFlagType) uint.Parse(command[1]);
                            switch(pWarFlag.Type)
                            {
                                //case (WarFlagType)7:
                                //case WarFlagType.WAR_FLAG_BASE_RANK:
                                //case WarFlagType.WAR_FLAG_TOP_4:
                                default:
                                    {
                                        pWarFlag.IsWar = true;
                                        pWarFlag.WriteUShort(25001, 8);
                                        break;
                                    }
                                case (WarFlagType)6:
                                    {
                                        pWarFlag.WriteUInt(pClient.Identity, 8);
                                        break;
                                    }
                            }
                            pClient.Send(pWarFlag);
                            break;
                        }
                        #endregion
                        #region DailyReset
                        case "/dailyreset":
                        {
                            foreach (var plr in ServerKernel.Players.Values)
                            {
                                plr.Character.DailyReset();
                            }

                            foreach (var arena in ServerKernel.ArenaRecord.Values)
                            {
                                DbUser dbUsr = Database.Characters.SearchByIdentity(arena.PlayerIdentity);
                                if (dbUsr == null)
                                {
                                    arena.Delete();
                                    continue;
                                }

                                arena.LastRanking = arena.Ranking;
                                arena.LastSeasonPoints = arena.Points;
                                arena.LastSeasonWins = arena.TodayWins;
                                arena.LastSeasonsLoses = arena.TodayLoses;

                                arena.TodayWins = 0;
                                arena.TodayLoses = 0;
                                arena.Points = ServerKernel.ArenaQualifier.GetStartupPoints(dbUsr.Level);
                                arena.Level = dbUsr.Level;
                                arena.Lookface = dbUsr.Lookface;
                                arena.Profession = dbUsr.Profession;
                                arena.PlayerName = dbUsr.Name;
                                arena.Save();
                            }
                            return;
                        }
                        #endregion
                        #region Test MsgDetainItemInfo
                        case "/detaintest":
                        {
                            MsgDetainItemInfo pPacket = new MsgDetainItemInfo();
                            pPacket.WriteUInt(1, 4);
                            pPacket.WriteUInt(5, 8);
                            pPacket.WriteUInt(421439, 12);
                            pPacket.WriteUShort(4099, 16);
                            pPacket.WriteUShort(4099, 18);
                            pPacket.Mode = DetainMode.DETAIN_PAGE;
                            pPacket.WriteByte(13, 28);
                            pPacket.WriteByte(13, 29);
                            pPacket.WriteByte(37, 37); // plus
                            pPacket.WriteByte(38, 38); // bless
                            pPacket.WriteByte(1, 39); // bound
                            pPacket.WriteByte(40, 40); // Enchant
                            pPacket.WriteUInt(1000002, 56);
                            pPacket.WriteString("NUNUNUNU", 16, 60);
                            pPacket.WriteUInt(1000003, 76);
                            pPacket.WriteString("NUUNUNU2", 16, 80);
                            pPacket.WriteUInt(20161225, 96);
                            //pPacket.WriteUShort(3, 98);
                            //pPacket.WriteUShort(1, 100);
                            //pPacket.WriteUShort(4, 102);
                            pPacket.WriteUShort(104, 104); // value
                            pPacket.WriteUInt(2, 108);
                            pClient.Send(pPacket);
                            break;
                        }
                        #endregion
                    }
                }
                if (pRole.IsGm)
                {
                    switch (command[0])
                    {
                        #region Cmd
                        case "/cmd":
                            {
                                if (command.Length < 3) return;

                                switch (command[1])
                                {
                                    case "broadcast":
                                        ServerKernel.SendMessageToAll(command[2], ChatTone.CENTER);
                                        break;
                                }
                                return;
                            }
                        #endregion
                        #region Chgmap
                        case "/chgmap":
                            {
                                if (command.Length < 3)
                                {
                                    pRole.Send("/chgmap mapid x y");
                                    return;
                                }

                                string[] coords = command[2].Split(' ');
                                if (coords.Length < 2) return;

                                uint mapid = uint.Parse(command[1]);
                                ushort x = ushort.Parse(coords[0]);
                                ushort y = ushort.Parse(coords[1]);

                                pRole.ChangeMap(x, y, mapid, false);
                                break;
                            }
                        #endregion
                        #region Find
                        case "/find":
                            {
                                if (command.Length < 2 || command[1] == pRole.Name) return;

                                uint mapid = 0;
                                ushort x = 0;
                                ushort y = 0;

                                foreach (var plr in ServerKernel.Players.Values)
                                {
                                    if (plr.Character.Name == command[1])
                                    {
                                        mapid = plr.Character.MapIdentity;
                                        x = plr.Character.MapX;
                                        y = plr.Character.MapY;
                                        break;
                                    }
                                }

                                pRole.ChangeMap(x, y, mapid);
                                break;
                            }
                        #endregion
                        #region Kickout
                        case "/kickout":
                            {
                                if (command.Length < 2 || command[1] == pRole.Name) return;

                                Client plr = ServerKernel.Players.Values.FirstOrDefault(x => x.Character.Name == command[1]);
                                if (plr == null) return;

                                plr.Character.Send(string.Format("You have been kicked by {0}.", pRole.Name));
                                plr.Disconnect();
                                break;
                            }
                        #endregion
                        #region Ban
                        case "/ban":
                            {
                                if (command.Length < 2 || command[1] == pRole.Name) return;

                                Client target =
                                    ServerKernel.Players.Values.FirstOrDefault(x => x.Character.Name == command[1]);
                                if (target == null) return;

                                //AccountRepository repo = new AccountRepository();
                                //DbAccount dbAcc = repo.SearchByIdentity(target.AccountIdentity);

                                //if (dbAcc == null) return;

                                //dbAcc.Lock = 1;
                                //repo.SaveOrUpdate(dbAcc);
                                target.Character.Ban();

                                target.Character.Send(string.Format("You have been banned by {0}.", pRole.Name));
                                //pClient.Character.Send("User banned.");
                                //target.Disconnect();
                                ServerKernel.Log.SaveLog(string.Format("User {0} has been banned.", target.Character.Name), true,
                                    LogType.MESSAGE);
                                break;
                            }
                        #endregion
                        #region Bring
                        case "/bring":
                            {
                                if (command.Length < 2 || command[1] == pRole.Name) return;

                                if (command[1].ToLower() == "all")
                                {
                                    foreach (var plr0 in ServerKernel.Players.Values)
                                        if (plr0.Character.MapIdentity != 1036 && !plr0.Character.Map.IsTrainingMap())
                                            plr0.Character.ChangeMap(pClient.Character.MapX, pClient.Character.MapY, pClient.Character.MapIdentity);
                                    return;
                                }

                                Client plr = ServerKernel.Players.Values.FirstOrDefault(x => x.Character.Name == command[1]);
                                if (plr == null) return;

                                if (plr.Character == null || plr.Character.MapIdentity == 1036)
                                    return;

                                plr.Character.ChangeMap(pClient.Character.MapX, pClient.Character.MapY, pClient.Character.MapIdentity);
                                return;
                            }
                        #endregion
                        #region Broadcast
                        case "/broadcast":
                            {
                                string[] msg = pMsg.Message.Split(new[] { ' ' }, 2);

                                if (msg.Length < 2)
                                    return;

                                ServerKernel.Broadcast.Push(msg[1]);
                                break;
                            }
                        #endregion
                        #region Reload Generators
                        case "/reloadgen":
                            {
                                ServerKernel.SendMessageToAll("Server is reloading respawns! Please do not attack monsters."
                                    , ChatTone.TALK);
                                foreach (var gen in ServerKernel.Generators)
                                {
                                    gen.Deactivate();
                                }
                                foreach (var gen in ServerKernel.Generators)
                                {
                                    gen.FirstGeneration();
                                }
                                ServerKernel.SendMessageToAll("Server has reloaded respawns. You can continue your hunting."
                                    , ChatTone.TALK);
                                return;
                            }
                        #endregion
                        #region Player

                        case "/player":
                            {
                                if (command.Length < 2 || command[1] == pRole.Name) return;

                                int nCount = 0;
                                if (command[1].ToLower() == "all")
                                {
                                    nCount = ServerKernel.Players.Values.Count;
                                    pRole.Send(string.Format("Total players: {0} Max Players: {1}", nCount,
                                        ServerKernel.OnlineRecord));
                                    return;
                                }
                                if (command[1].ToLower() == "map")
                                {
                                    nCount = pRole.Map.Players.Count;
                                    pRole.Send(string.Format("Total map players: {0}", nCount));
                                    return;
                                }
                                pRole.Send("Invalid command param");
                                return;
                            }

                        #endregion
                        #region Botjail

                        case "/botjail":
                            {
                                if (command.Length < 2 || command[1] == pRole.Name) return;

                                Client plr = ServerKernel.Players.Values.FirstOrDefault(x => x.Character.Name == command[1]);
                                if (plr == null) return;

                                plr.Character.ChangeMap(31, 73, 6003);
                                plr.Character.SetRecordPos(6003, 31, 73);
                                return;
                            }

                        #endregion
                        #region State

                        case "/state":
                            {
                                if (command.Length < 2 || command[1] == pRole.Name) return;

                                int nCount = 0;
                                if (command[1].ToLower() == "visible")
                                {
                                    pRole.Invisible = false;
                                    pRole.Screen.RefreshSpawnForObservers();
                                    return;
                                }
                                if (command[1].ToLower() == "invisible")
                                {
                                    pRole.Invisible = true;
                                    pRole.Screen.RefreshSpawnForObservers();
                                    return;
                                }
                                pRole.Send("Invalid command param");
                                break;
                            }

                        #endregion
                        #region Check Char Status
                        case "/userstatus":
                            {
                                if (command.Length >= 2)
                                {
                                    Client pUserClient = ServerKernel.Players.Values.FirstOrDefault(x => x.Character != null && x.Character.Name == command[1]);
                                    if (pUserClient == null) return;
                                    Character pUser = pUserClient.Character;

                                    pRole.Send(string.Format("Total Battle Power: {0} Pure Battle Power: {1} Mentor Battle Power: {2}",
                                            pUser.BattlePower, pUser.BattlePower - pUser.SharedBattlePower,
                                            pUser.SharedBattlePower), ChatTone.TALK);
                                    pRole.Send(string.Format("HP:{0} MP:{1} MaxLife:{2} MaxMana:{3}", pUser.Life, pUser.Mana,
                                            pUser.MaxLife, pUser.MaxMana), ChatTone.TALK);
                                    pRole.Send(string.Format("Atk:{0}/{5} MAtk: {4} Def:{1} MDef:{2} MDefBonus:{3}",
                                            pUser.MinAttack, pUser.Defense, pUser.MagicDefense, pUser.MagicDefenseBonus,
                                            pUser.MagicAttack, pUser.MaxAttack), ChatTone.TALK);
                                    pRole.Send(string.Format("Dex:{0} HitRate:{1}", pUser.Dexterity, pUser.AttackHitRate),
                                        ChatTone.TALK);
                                    pRole.Send(string.Format("Final Atk:{0} MAtk:{1} Def:{2} MDef:{3}", pUser.AddFinalAttack,
                                            pUser.AddFinalMagicAttack, pUser.AddFinalDefense, pUser.AddFinalMagicDefense),
                                        ChatTone.TALK);
                                    return;
                                }
                                break;
                            }
                        #endregion
                        #region Silence
                        case "/silence":
                        {
                            if (command.Length < 2 || command[1] == pRole.Name) return;

                            Client plr = ServerKernel.Players.Values.FirstOrDefault(x => x.Character.Name == command[1]);
                            if (plr == null) return;

                            int nTime = 600;
                            if (command.Length >= 3)
                                int.TryParse(command[2], out nTime);

                            if (nTime < 600)
                                nTime = 600;

                            plr.Character.Silence(nTime);
                            plr.SendMessage(string.Format("You have been silenced for {0} seconds.", nTime), ChatTone.TALK);
                            return;
                        }

                        #endregion
                        #region Arena Ban
                        case "/qualifierban":
                        {
                            if (command.Length < 2 || command[1] == pRole.Name) return;

                            Client plr =
                                ServerKernel.Players.Values.FirstOrDefault(
                                    x => x.Character != null && x.Character.Name == command[1]);
                            if (plr == null) return;

                            plr.Character.ArenaQualifier.SetLock();
                            plr.SendMessage("You have been banned from Arena Qualifier.");
                            pRole.Send("You have banned the user from the arena qualifier.");
                            return;
                        }

                            #endregion
                    }
                }
                switch (command[0])
                {
                    #region Disconnection

                    case "/dc":
                    case "/disconnect":
                        pRole.Disconnect();
                        return;

                        #endregion
                    #region Revive

                    case "/revive":
                    {
                        //if (pClient.Character.CanRevive())
                        //    pClient.Character.Reborn(true);
                        return;
                    }

                        #endregion
                    #region Check Char Status

                    case "/userstatus":
                    {
                        pRole.Send(
                            string.Format("Total Battle Power: {0} Pure Battle Power: {1} Mentor Battle Power: {2}",
                                pRole.BattlePower, pRole.BattlePower - pRole.SharedBattlePower, pRole.SharedBattlePower),
                            ChatTone.TALK);
                        pRole.Send(
                            string.Format("HP:{0} MP:{1} MaxLife:{2} MaxMana:{3}", pRole.Life, pRole.Mana, pRole.MaxLife,
                                pRole.MaxMana), ChatTone.TALK);
                        pRole.Send(
                            string.Format("Atk:{0}/{5} MAtk: {4} Def:{1} MDef:{2} MDefBonus:{3}", pRole.MinAttack,
                                pRole.Defense, pRole.MagicDefense, pRole.MagicDefenseBonus, pRole.MagicAttack,
                                pRole.MaxAttack), ChatTone.TALK);
                        pRole.Send(string.Format("Dex:{0} HitRate:{1}", pRole.Dexterity, pRole.AttackHitRate),
                            ChatTone.TALK);
                        pRole.Send(
                            string.Format("Final Atk:{0} MAtk:{1} Def:{2} MDef:{3}", pRole.AddFinalAttack,
                                pRole.AddFinalMagicAttack, pRole.AddFinalDefense, pRole.AddFinalMagicDefense),
                            ChatTone.TALK);
                        break;
                    }

                        #endregion
                    #region Report

                    case "/report":
                    {
                        if (command.Length < 3 || command[1] == pRole.Name) return;
                        Client plr = ServerKernel.Players.Values.FirstOrDefault(x => x.Character.Name == command[1]);
                        if (plr == null)
                        {
                            pRole.Send(string.Format("Target {0} not found.", command[1]));
                            return;
                        }
                        ServerKernel.Log.GmLog("Report", string.Format("{0} has reported {1} for: {2}",
                            pRole.Name, command[1], command[2]));
                        pRole.Send("Your report request has been registered.");
                        break;
                    }

                        #endregion
                    #region Report Bug
                    case "/reportbug":
                    {
                        string[] report = pMsg.Message.Split(new[] { ' ' }, 2);
                        if (report.Length < 2)
                            return;
                        ServerKernel.Log.GmLog("BugReport", string.Format("{0} reported: {1}", pRole.Name, command[1]));
                        pRole.Send("Your bug report has been registered.");
                        break;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
            }
        }
    }
}