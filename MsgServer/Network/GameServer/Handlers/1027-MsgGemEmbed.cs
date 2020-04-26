// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1027 - MsgGemEmbed.cs
// Last Edit: 2016/11/24 11:12
// Created: 2016/11/24 11:12

using System;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleEmbedGem(Character pRole, MsgGemEmbed pMsg)
        {
            Item pTarget, pSource;
            if (!pRole.Inventory.Items.TryGetValue(pMsg.MainIdentity, out pTarget)
                || (!pRole.Inventory.Items.TryGetValue(pMsg.MinorIdentity, out pSource) && pMsg.Mode == EmbedMode.GEM_ADD))
                return;

            switch (pMsg.Mode)
            {
                #region Gem Add
                case EmbedMode.GEM_ADD:
                    {
                        SocketGem gem = (SocketGem)(pSource.Type % 1000);

                        if (!Enum.IsDefined(typeof(SocketGem), (byte)gem) || pSource.GetItemSubtype() != 700)
                        {
                            pRole.Send("That item isn\'t a gem.");
                            return;
                        }

                        if (pTarget.GetItemSubtype() == 201)
                        {
                            switch (gem)
                            {
                                case SocketGem.NORMAL_THUNDER_GEM:
                                case SocketGem.REFINED_THUNDER_GEM:
                                case SocketGem.SUPER_THUNDER_GEM:
                                    break;
                                default:
                                    return;
                            }
                        }

                        if (pTarget.GetItemSubtype() == 202)
                        {
                            switch (gem)
                            {
                                case SocketGem.NORMAL_GLORY_GEM:
                                case SocketGem.REFINED_GLORY_GEM:
                                case SocketGem.SUPER_GLORY_GEM:
                                    break;
                                default:
                                    return;
                            }
                        }

                        if (pTarget.GetItemSubtype() != 201 && pTarget.GetItemSubtype() != 202)
                        {
                            switch (gem)
                            {
                                case SocketGem.NORMAL_THUNDER_GEM:
                                case SocketGem.REFINED_THUNDER_GEM:
                                case SocketGem.SUPER_THUNDER_GEM:
                                case SocketGem.NORMAL_GLORY_GEM:
                                case SocketGem.REFINED_GLORY_GEM:
                                case SocketGem.SUPER_GLORY_GEM:
                                    return;
                                default:
                                    break;
                            }
                        }

                        if (pMsg.HoleNum == 1 || (pMsg.HoleNum == 2 && pTarget.SocketOne == SocketGem.EMPTY_SOCKET))
                        {
                            if (pTarget.SocketOne == SocketGem.NO_SOCKET)
                            {
                                pRole.Send("The target item doesn' have a socket.");
                                return;
                            }

                            if (!pRole.Inventory.Remove(pSource.Identity, ItemRemovalType.DELETE))
                            {
                                pRole.Send("You don't have the required item.");
                                return;
                            }

                            pTarget.SocketOne = gem;
                            break;
                        }

                        if (pMsg.HoleNum == 2)
                        {
                            if (pTarget.SocketOne > SocketGem.NO_SOCKET
                                || pTarget.SocketOne < SocketGem.EMPTY_SOCKET)
                            {
                                if (pTarget.SocketTwo == SocketGem.NO_SOCKET)
                                {
                                    pRole.Send("The item doesn't have the second socket open.");
                                    return;
                                }

                                if (pTarget.SocketTwo != SocketGem.EMPTY_SOCKET)
                                {
                                    pRole.Send("This item already have a gem embed on the second hole.");
                                    return;
                                }

                                if (!pRole.Inventory.Remove(pSource.Identity, ItemRemovalType.DELETE))
                                {
                                    pRole.Send("You don't have the required item.");
                                    return;
                                }

                                pTarget.SocketTwo = gem;
                            }
                        }
                        break;
                    }
                #endregion
                #region Gem Remove
                case EmbedMode.GEM_REMOVE:
                    {
                        if (pMsg.HoleNum == 1)
                        {
                            if (pTarget.SocketOne == SocketGem.NO_SOCKET)
                            {
                                pRole.Send("This item has no socket.");
                                return;
                            }
                            if (pTarget.SocketOne == SocketGem.EMPTY_SOCKET)
                            {
                                pRole.Send("This item has no gem.");
                                return;
                            }

                            pTarget.SocketOne = SocketGem.EMPTY_SOCKET;

                            if (pTarget.SocketTwo > SocketGem.NO_SOCKET && pTarget.SocketTwo < SocketGem.EMPTY_SOCKET)
                            {
                                pTarget.SocketOne = pTarget.SocketTwo;
                                pTarget.SocketTwo = SocketGem.EMPTY_SOCKET;
                            }
                            break;
                        }

                        if (pMsg.HoleNum == 2)
                        {
                            if (pTarget.SocketTwo == SocketGem.NO_SOCKET)
                            {
                                pRole.Send("This item does not have the second socket open.");
                                return;
                            }

                            if (pTarget.SocketTwo == SocketGem.EMPTY_SOCKET)
                            {
                                pRole.Send("No gem to remove.");
                                return;
                            }

                            pTarget.SocketTwo = SocketGem.EMPTY_SOCKET;
                        }
                        break;
                    }
                #endregion
            }

            pTarget.Save();
            pRole.Send(pTarget.InformationPacket(true));
            pRole.Send(pMsg);
        }
    }
}