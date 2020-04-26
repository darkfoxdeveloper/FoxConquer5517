// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2202 - MsgWeaponsInfo.cs
// Last Edit: 2016/11/25 05:08
// Created: 2016/11/25 04:54

using System;
using System.Linq;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Society;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleWeaponsInfo(Character pUser, MsgWeaponsInfo pMsg)
        {
            TotemPoleType pType = (TotemPoleType) pMsg.ArsenalType;
            if (pType == TotemPoleType.TOTEM_NONE)
                pType = TotemPoleType.TOTEM_HEADGEAR;
            
            TotemPole pPole;
            if (!pUser.Syndicate.Arsenal.Poles.TryGetValue(pType, out pPole))
                return;

            uint beginAt = pMsg.BeginAt - 1;
            uint length = (uint) pPole.Items.Count;

            length = Math.Min(length, 8);
            int nAmount = 0;
            foreach (var item in pPole.Items.Values.OrderByDescending(x => x.Donation()))
            {
                if (nAmount < beginAt)
                {
                    nAmount++;
                    continue;
                }
                if (nAmount > beginAt + length)
                    break;
                pMsg.AppendItem(item.Identity, (uint) nAmount, item.PlayerName, item.Itemtype, (byte) (item.Itemtype%10),
                    item.Item.Plus, (byte) item.Item.SocketOne, (byte) item.Item.SocketTwo, (uint) item.Item.CalculateItemBattlePower(),
                    item.Donation());
                nAmount++;
            }
            pMsg.EndAt = length + pMsg.BeginAt - 1;
            pMsg.Donation = (uint) pPole.Donation;
            pMsg.Enchantment = 0; // todo
            pMsg.EnchantmentExpire = 0; // todo
            pMsg.SharedBattlePower = pPole.BattlePower;
            pMsg.TotalInscribed = (uint) pPole.Items.Count;
            pUser.Send(pMsg);
        }
    }
}