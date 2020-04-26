// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2203 - MsgTotemPole.cs
// Last Edit: 2016/11/25 04:46
// Created: 2016/11/25 04:36

using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using MsgServer.Structures.Society;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleTotemPole(Character pUser, MsgTotemPole pMsg)
        {
            if (pUser == null
                || pUser.Syndicate == null
                || pUser.SyndicateMember == null)
                return;

            if (pUser.Syndicate.Arsenal == null)
                pUser.Syndicate.Arsenal = new Arsenal(pUser.Syndicate);

            switch (pMsg.Type)
            {
                case 0: // unlock arsenal
                {
                    pUser.Syndicate.Arsenal.UnlockArsenal((TotemPoleType) pMsg.BeginAt, pUser);
                    break;
                }
                case 1: // inscribe item
                {
                    Item pItem;
                    if (!pUser.Inventory.Items.TryGetValue(pMsg.EndAt, out pItem))
                        return;

                    pUser.Syndicate.Arsenal.InscribeItem(pItem, pUser);
                    break;
                }
                case 2: // remove item
                {
                    Item pItem;
                    if (pUser.Inventory.Items.TryGetValue(pMsg.EndAt, out pItem))
                    {
                        pUser.Syndicate.Arsenal.UninscribeItem(pItem, pUser);
                    }
                    else
                    {
                        pUser.Syndicate.Arsenal.UninscribeItem(pMsg.EndAt, pUser);
                    }
                    break;
                }
                case 3: // enhance
                    /*
                     * ArsenalType = The amount of enhancement
                     * BeginAt = The Arsenal ID
                     */
                    break;
                case 4: // refresh
                {
                    pUser.Syndicate.Arsenal.SendArsenal(pUser);
                    break;
                }
                default:
                {
                    ServerKernel.Log.SaveLog(string.Format("MsgTotemPole::{0}", pMsg.Type));
                    break;
                }
            }
        }
    }
}