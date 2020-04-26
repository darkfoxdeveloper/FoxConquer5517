// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - 1150 - MsgFlower.cs
// Last Edit: 2017/02/15 18:06
// Created: 2017/02/15 17:50

using System;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleFlowers(Character pUser, MsgFlower pMsg)
        {
            switch (pMsg.Mode)
            {
                case 0: // give flower
                    {
                        string szInfo = pMsg.ReadString(16, 18).Trim('\0');
                        string[] pInfo = szInfo.Split(' ');
                        if (pInfo.Length < 3) return;

                        uint idTarget = uint.Parse(pInfo[0]);
                        ushort usAmount = ushort.Parse(pInfo[1]);
                        var pType = (FlowerType)uint.Parse(pInfo[2][0].ToString());

                        Client pTargetClient;
                        if (!ServerKernel.Players.TryGetValue(idTarget, out pTargetClient))
                        {
                            pUser.Send("The target doesn't exist.");
                            return;
                        }

                        if (pTargetClient.Character == null)
                        {
                            // should not happen
                            return;
                        }

                        Character pTarget = pTargetClient.Character;

                        if (!pUser.IsAlive)
                        {
                            pUser.Send(ServerString.STR_FLOWER_SENDER_NOT_ALIVE);
                            return;
                        }

                        if (pUser.Gender != 1)
                        {
                            pUser.Send(ServerString.STR_FLOWER_SENDOR_NOT_MALE);
                            return;
                        }

                        if (pTarget.Gender != 2)
                        {
                            pUser.Send(ServerString.STR_FLOWER_RECEIVER_NOT_FEMALE);
                            return;
                        }

                        if (pUser.Level < 40)
                        {
                            pUser.Send(ServerString.STR_FLOWER_LEVEL_TOO_LOW);
                            return;
                        }

                        Item pFlower;
                        if (pMsg.ItemIdentity == 0)
                        {
                            if (pUser.RedRoses >= uint.Parse(DateTime.Now.ToString("yyyyMMdd")))
                            {
                                pUser.Send(ServerString.STR_FLOWER_HAVE_SENT_TODAY);
                                return;
                            }

                            // player sending red rose normal
                            switch (pUser.Owner.VipLevel)
                            {
                                case 0:
                                    usAmount = 1;
                                    break;
                                case 1:
                                    usAmount = 2;
                                    break;
                                case 2:
                                    usAmount = 5;
                                    break;
                                case 3:
                                    usAmount = 7;
                                    break;
                                case 4:
                                    usAmount = 9;
                                    break;
                                case 5:
                                    usAmount = 12;
                                    break;
                                default:
                                    usAmount = 15;
                                    break;
                            }

                            pType = FlowerType.RED_ROSE;
                            pUser.RedRoses = uint.Parse(DateTime.Now.ToString("yyyyMMdd"));
                        }
                        else if (pUser.Inventory.Items.TryGetValue(pMsg.ItemIdentity, out pFlower))
                        {
                            // player sending packs
                            switch (pFlower.GetItemSubtype())
                            {
                                case 751: // red rose
                                    pType = FlowerType.RED_ROSE;
                                    break;
                                case 752: // white rose
                                    pType = FlowerType.WHITE_ROSE;
                                    break;
                                case 753: // orchid rose
                                    pType = FlowerType.ORCHID;
                                    break;
                                case 754: // tulip rose
                                    pType = FlowerType.TULIP;
                                    break;
                                default:
                                    return;
                            }
                            usAmount = pFlower.Durability;

                            if (!pUser.Inventory.Remove(pMsg.ItemIdentity))
                                return;
                        }
                        else
                        {
                            pUser.Send("You don't have the required item.");
                            return;
                        }

                        switch (pType)
                        {
                            case FlowerType.RED_ROSE:
                                pTarget.RedRoses += usAmount;
                                break;
                            case FlowerType.WHITE_ROSE:
                                pTarget.WhiteRoses += usAmount;
                                break;
                            case FlowerType.ORCHID:
                                pTarget.Orchids += usAmount;
                                break;
                            case FlowerType.TULIP:
                                pTarget.Tulips += usAmount;
                                break;
                        }

                        switch (usAmount)
                        {
                            case 3:
                                switch (pType)
                                {
                                    case FlowerType.RED_ROSE:
                                        pTarget.Send(string.Format(ServerString.STR_FLOWER_GM_PROMPT_RED_3, pTarget.Name, pUser.Name));
                                        break;
                                    case FlowerType.WHITE_ROSE:
                                        pTarget.Send(string.Format(ServerString.STR_FLOWER_GM_PROMPT_WHITE_3, pTarget.Name, pUser.Name));
                                        break;
                                    case FlowerType.ORCHID:
                                        pTarget.Send(string.Format(ServerString.STR_FLOWER_GM_PROMPT_LILY_3, pTarget.Name, pUser.Name));
                                        break;
                                    case FlowerType.TULIP:
                                        pTarget.Send(string.Format(ServerString.STR_FLOWER_GM_PROMPT_TULIP_3, pTarget.Name, pUser.Name));
                                        break;
                                }
                                break;
                            case 9:
                                switch (pType)
                                {
                                    case FlowerType.RED_ROSE:
                                        pTarget.Send(string.Format(ServerString.STR_FLOWER_GM_PROMPT_RED_9, pTarget.Name, pUser.Name));
                                        break;
                                    case FlowerType.WHITE_ROSE:
                                        pTarget.Send(string.Format(ServerString.STR_FLOWER_GM_PROMPT_WHITE_9, pTarget.Name, pUser.Name));
                                        break;
                                    case FlowerType.ORCHID:
                                        pTarget.Send(string.Format(ServerString.STR_FLOWER_GM_PROMPT_LILY_9, pTarget.Name, pUser.Name));
                                        break;
                                    case FlowerType.TULIP:
                                        pTarget.Send(string.Format(ServerString.STR_FLOWER_GM_PROMPT_TULIP_9, pTarget.Name, pUser.Name));
                                        break;
                                }
                                break;
                            case 99:
                                switch (pType)
                                {
                                    case FlowerType.RED_ROSE:
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_FLOWER_GM_PROMPT_RED_99, pTarget.Name, pUser.Name), ChatTone.CENTER);
                                        break;
                                    case FlowerType.WHITE_ROSE:
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_FLOWER_GM_PROMPT_WHITE_99, pTarget.Name, pUser.Name), ChatTone.CENTER);
                                        break;
                                    case FlowerType.ORCHID:
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_FLOWER_GM_PROMPT_LILY_99, pTarget.Name, pUser.Name), ChatTone.CENTER);
                                        break;
                                    case FlowerType.TULIP:
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_FLOWER_GM_PROMPT_TULIP_99, pTarget.Name, pUser.Name), ChatTone.CENTER);
                                        break;
                                }
                                break;
                            case 999:
                                switch (pType)
                                {
                                    case FlowerType.RED_ROSE:
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_FLOWER_GM_PROMPT_RED_999, pTarget.Name, pUser.Name), ChatTone.CENTER);
                                        break;
                                    case FlowerType.WHITE_ROSE:
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_FLOWER_GM_PROMPT_WHITE_999, pTarget.Name, pUser.Name), ChatTone.CENTER);
                                        break;
                                    case FlowerType.ORCHID:
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_FLOWER_GM_PROMPT_LILY_999, pTarget.Name, pUser.Name), ChatTone.CENTER);
                                        break;
                                    case FlowerType.TULIP:
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_FLOWER_GM_PROMPT_TULIP_999, pTarget.Name, pUser.Name), ChatTone.CENTER);
                                        break;
                                }
                                break;
                            default:
                                pTarget.Send(string.Format(ServerString.STR_FLOWER_RECEIVER_PROMPT, pUser.Name));
                                break;
                        }

                        pUser.Send(pMsg);
                        pUser.Send(new MsgFlower
                        {
                            Mode = 0,
                            Identity = pUser.Identity
                        });
                        pTarget.Send(new MsgFlower
                        {
                            Sender = pUser.Name,
                            Receptor = pTarget.Name,
                            SendAmount = usAmount,
                            SendFlowerType = pType
                        });

                        ServerKernel.FlowerRanking.AddFlowers(pType, usAmount, pTarget.Identity);
                        break;
                    }
                default:
                    {
                        ServerKernel.Log.SaveLog(string.Format("Packet 1150:{0} not handled", pMsg.Mode),
                            true,
                            "p1150",
                            LogType.WARNING);
                        break;
                    }
            }
        }
    }
}