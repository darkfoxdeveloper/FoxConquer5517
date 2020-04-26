// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - 2110 - MsgSuperFlag.cs
// Last Edit: 2017/02/06 21:11
// Created: 2017/02/06 08:56

using System;
using DB.Entities;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using MsgServer.Structures.World;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleSuperFlag(Character pUser, MsgSuperFlag pMsg)
        {
            switch (pMsg.Action)
            {
                case 1: // save location
                {
                    if (pUser.Map.IsDynamicMap())
                        return;
                    if (pUser.Map.IsRecordDisable())
                        return;
                    if (pUser.Map.IsPkField())
                        return;
                    if (pUser.Map.IsBoothEnable())
                        return;
                    if (pUser.Map.IsFamilyMap() || pUser.Map.IsSynMap())
                        return;
                    if (pUser.Map.IsPrisionMap())
                        return;
                    if (pUser.Map.Identity == 2055 || pUser.Map.Identity == 2056)
                        return;
                    Item item;
                    if (pUser.Inventory.Items.TryGetValue(pMsg.ItemIdentity, out item))
                    {
                        if (pMsg.CarryIdentity > 0 || item.CarryCount >= Carry.MAX_RECORDS)
                        {
                            item.UpdateLocation((int) pMsg.CarryIdentity);
                        }
                        else
                        {
                            if (item.CarryCount >= Carry.MAX_RECORDS)
                                return;
                            item.SaveLocation();
                        }
                    }
                    break;
                }
                case 2: // change name
                {
                    if (pMsg.Name.Length <= 0 || !CheckName(pMsg.Name))
                        return;
                    string name = pMsg.Name.Length > 32 ? pMsg.Name.Substring(0, 32) : pMsg.Name;
                        Item item;
                    if (pUser.Inventory.Items.TryGetValue(pMsg.ItemIdentity, out item))
                    {
                        item.CarrySetName(name, (int) pMsg.CarryIdentity);
                    }
                    break;
                }
                case 3: // goto location
                {
                    if (pUser.Map.IsPrisionMap())
                        return;
                    if (pUser.IsInsideQualifier)
                        return;
                    Item item;
                    if (pUser.Inventory.Items.TryGetValue(pMsg.ItemIdentity, out item))
                    {
                        DbCarry carry;
                        if (!item.GetTeleport(pMsg.CarryIdentity, out carry))
                            return;
                        Map map;
                        if (!ServerKernel.Maps.TryGetValue(carry.RecordMapId, out map))
                            return;

                        if (map.IsDynamicMap())
                            return;
                        if (map.IsRecordDisable())
                            return;
                        if (map.IsPkField())
                            return;
                        if (map.IsBoothEnable())
                            return;
                        if (map.IsFamilyMap() || map.IsSynMap())
                            return;
                        if (map.IsPrisionMap())
                            return;
                        if (item.Durability <= 0)
                            return;
                        item.Durability -= 1;
                        item.SendCarry();
                        pUser.ChangeMap(carry.RecordMapX, carry.RecordMapY, carry.RecordMapId);
                    }
                    break;
                }
                case 4: // refill
                {
                    Item item;
                    if (pUser.Inventory.Items.TryGetValue(pMsg.ItemIdentity, out item))
                    {
                        int nCost = Math.Max(1, item.MaximumDurability - item.Durability)/2;
                        if (!pUser.ReduceEmoney(nCost, true))
                            return;
                        item.Durability = item.MaximumDurability;
                        item.SendCarry();
                    }
                    break;
                }
            }
        }
    }
}