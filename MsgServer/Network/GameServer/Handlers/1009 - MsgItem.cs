// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1009 - MsgItem.cs
// Last Edit: 2016/11/24 10:26
// Created: 2016/11/23 13:38

using System.Drawing;
using System.Linq;
using DB.Entities;
using DB.Repositories;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleItem(Character pRole, MsgItem pMsg)
        {
            // todo 45 alternate equip
            switch (pMsg.Action)
            {
                #region 1 - Buy
                case ItemAction.BUY:
                    {
                        IScreenObject test = null; // just something temporary
                        GameNpc pNpc = null; // only Game Npcs are allowed to sell items :) Dynamic NPCs will not
                        bool isHonor = false;
                         //check if it's an NPC the user is already interacting
                        if ((pRole.InteractingNpc != null 
                            && pMsg.Identity == pRole.InteractingNpc.Identity
                             && !Calculations.InScreen(pRole.InteractingNpc.MapX, pRole.InteractingNpc.MapY, pRole.MapX, pRole.MapY)
                             && pRole.InteractingNpc.MapIdentity != 5000)) // if NPC is in a GM map, so it can be accessed
                        {
                            pNpc = pRole.InteractingNpc as GameNpc;
                        }
                        else if (pMsg.Identity != 2888 && pMsg.Identity != 6000)
                        {
                            if (!pRole.Map.GameObjects.TryGetValue(pMsg.Identity, out test))
                                return;
                            if (pMsg.Identity != 2888 &&
                                !Calculations.InScreen(test.MapX, test.MapY, pRole.MapX, pRole.MapY))
                                return;
                            pNpc = test as GameNpc;
                            // get the npc from the packet :) if don't exist we quit
                            if (pNpc == null) return;
                        } else if (pMsg.Identity == 6000)
                        {
                            isHonor = true;
                        }

                        var goods =
                            ServerKernel.Goods.Values.FirstOrDefault(
                                x => x.OwnerIdentity == pMsg.Identity && x.Itemtype == pMsg.Param1);
                        if (goods == null && !isHonor)
                        {
                            ServerKernel.Log.SaveLog(string.Format("Could not load item(type:{0}) for shop (id:{1})", pMsg.Param1, pMsg.Identity));
                            return;
                        }

                        DbItemtype itemtype;
                        if (!ServerKernel.Itemtype.TryGetValue(pMsg.Param1, out itemtype))
                        {
                            ServerKernel.Log.SaveLog(string.Format("User (id:{0}) probably edited a shop trying to buy non-existent (type:{1}) item", pRole.Identity, pMsg.Param1));
                            return;
                        }

                        uint stack = itemtype.StackLimit > 0 ? itemtype.StackLimit : 1u;
                        if (pRole.Inventory.IsFull || pRole.Inventory.RemainingSpace()*stack < pMsg.Param2)
                        {
                            pRole.Send(ServerString.STR_INVENTORY_FULL);
                            return;
                        }

                        byte monopoly = itemtype.Monopoly;
                        int amount = (int) pMsg.Param2;
                        if (pMsg.Param4 == 0)
                        {
                            if (!isHonor)
                            {
                                if (goods.Moneytype != 0) return; // invalid money type
                                if (!pRole.ReduceMoney(itemtype.Price*amount))
                                {
                                    pRole.Send(ServerString.STR_NOT_ENOUGH_MONEY);
                                    return;
                                }
                            }
                            else
                            {
                                if (itemtype.HonorPrice <= 0)
                                    return;
                                if (pRole.ArenaQualifier.HonorPoints < itemtype.HonorPrice)
                                {
                                    pRole.Send("Not enough Honor Points.");
                                    return;
                                }
                                pRole.ArenaQualifier.HonorPoints -= itemtype.HonorPrice;
                                pRole.SendArenaStatus();
                                monopoly = 3;
                            }
                        }
                        else if (pMsg.Param4 == 1)
                        {
                            if (goods.Moneytype != 1) return;
                            if (!pRole.ReduceEmoney(itemtype.EmoneyPrice * pMsg.Param2))
                            {
                                pRole.Send(ServerString.STR_NOT_ENOUGH_EMONEY);
                                return;
                            }
                            amount = (int)pMsg.Param2;
                        }
                        else if (pMsg.Param4 == 2)
                        {
                            if (goods.Moneytype != 1) return;
                            if (itemtype.BoundEmoneyPrice <= 0) return;

                            if (pRole.BoundEmoney < itemtype.BoundEmoneyPrice)
                            {
                                pRole.Send(ServerString.STR_NOT_ENOUGH_EMONEY);
                                return;
                            }

                            pRole.BoundEmoney -= itemtype.BoundEmoneyPrice;
                            monopoly = 3;
                        }

                        pRole.Inventory.Create(pMsg.Param1, (ushort) amount, monopoly);
                        break;
                    }
                #endregion
                #region 2 - Sell
                case ItemAction.SELL:
                    {
                        if (pMsg.Identity == 2888) return;


                        IScreenObject test;
                        if (!pRole.Map.GameObjects.TryGetValue(pMsg.Identity, out test))
                            return;
                        if (!Calculations.InScreen(test.MapX, test.MapY, pRole.MapX, pRole.MapY))
                            return;
                        //GameNpc pNpc = test as GameNpc;
                        //if (pNpc == null) return;
                        //if (pRole.GetDistance(pNpc) > 18) return;

                        Item item;
                        if (!pRole.Inventory.Items.TryGetValue(pMsg.Param1, out item))
                            return;

                        uint dwPrice = item.GetSellPrice();
                        pRole.AwardMoney(dwPrice, true);
                        pRole.Inventory.Remove(item.Identity, ItemRemovalType.DELETE);

                        ServerKernel.Log.GmLog("item_sell", string.Format("Item (id:{0}) has been sold to (npcid:{1}) by (userid:{2})", item.Identity, pMsg.Identity, pRole.Identity));
                        break;
                    }
                #endregion
                #region 4/5 - Use/Equip
                case ItemAction.USE:
                case ItemAction.EQUIP:
                    {
                        if (pRole.Equipment.Add(pMsg.Identity, (byte)pMsg.Param1))
                            pRole.RecalculateAttributes();
                        break;
                    }
                #endregion
                #region 6 - Unequip
                case ItemAction.UNEQUIP:
                    {
                        if (pRole.Equipment.Remove(pMsg.Identity))
                            pRole.RecalculateAttributes();
                        break;
                    }
                #endregion
                #region 9 - Query Money Saved
                case ItemAction.QUERY_MONEY_SAVED:
                    {
                        pMsg.Param1 = pRole.MoneySaved;
                        pRole.Send(pMsg);
                        break;
                    }
                #endregion
                #region 10 - Save Money
                case ItemAction.SAVE_MONEY:
                    {
                        if (pRole.ReduceMoney(pMsg.Param1, true))
                        {
                            if (!pRole.AwardMoneySaved(pMsg.Param1, true))
                            {
                                pRole.AwardMoney(pMsg.Param1);
                            }
                            else
                            {
                                MsgItem msg = new MsgItem
                                {
                                    Action = ItemAction.QUERY_MONEY_SAVED,
                                    Param1 = pMsg.Param1
                                };
                                pRole.Send(msg);
                            }
                        }
                        break;
                    }
                #endregion
                #region 11 - Draw Money
                case ItemAction.DRAW_MONEY:
                    {
                        if (pRole.ReduceMoneySaved(pMsg.Param1, true))
                        {
                            if (!pRole.AwardMoney(pMsg.Param1, true))
                            {
                                pRole.AwardMoneySaved(pMsg.Param1);
                            }
                            else
                            {
                                MsgItem msg = new MsgItem
                                {
                                    Action = ItemAction.QUERY_MONEY_SAVED,
                                    Param1 = pMsg.Param1
                                };
                                pRole.Send(msg);
                            }
                        }
                        break;
                    }
                #endregion
                #region 14 - Repair
                case ItemAction.REPAIR:
                    {
                        Item target;
                        if (pRole.Inventory.Items.TryGetValue(pMsg.Identity, out target))
                            target.RepairItem();
                        break;
                    }
                #endregion
                #region 15 - Repair All
                case ItemAction.REPAIR_ALL:
                    {
                        if (pRole.Owner.VipLevel < 2)
                        {
                            pRole.Send(ServerString.STR_VIP_REPAIR_FAILED);
                            return;
                        }
                        foreach (var item in pRole.Equipment.Items.Values)
                            item.RepairItem();
                        break;
                    }
                #endregion
                #region 19 - Improve
                case ItemAction.IMPROVE:
                    {
                        Item target;
                        if (pRole.Inventory.Items.TryGetValue(pMsg.Identity, out target))
                        {
                            if (target.Durability / 100 < target.MaximumDurability / 100)
                            {
                                pRole.Send(ServerString.STR_REPAIR_THEN_IMPROVE);
                                return;
                            }

                            if (target.Type % 10 == 0)
                            {
                                pRole.Send("Fixed items cannote be improved.");
                                return;
                            }

                            uint idNewType = 0;
                            double nChance = 0.00;

                            target.GetUpEpQualityInfo(out nChance, out idNewType);

                            if (target.Type % 10 < 6 && target.Type % 10 > 0)
                            {
                                nChance = 100.00;
                            }

                            if (idNewType == 0)
                            {
                                pRole.Send(ServerString.STR_CONNOT_IMPROVE);
                                return;
                            }

                            if (!pRole.Inventory.Remove(SpecialItem.TYPE_DRAGONBALL, 1))
                            {
                                pRole.Send(ServerString.STR_DRAGONBALL);
                                return;
                            }

                            if (Calculations.ChanceCalc((float)nChance))
                            {
                                target.Type = idNewType;
                                pRole.SendEffect("improveSuc", false);
                            }
                            else
                            {
                                target.Durability /= 2;
                            }

                            if (Calculations.ChanceCalc(1f))
                            {
                                bool changed = false;
                                if (target.SocketOne == SocketGem.NO_SOCKET)
                                {
                                    changed = true;
                                    target.SocketOne = SocketGem.EMPTY_SOCKET;
                                }
                                else if (target.SocketTwo == SocketGem.NO_SOCKET)
                                {
                                    changed = true;
                                    target.SocketTwo = SocketGem.EMPTY_SOCKET;
                                }
                                if (changed)
                                    pRole.Send(ServerString.STR_IMPROVE_UPGRADE_SOCKET);
                            }

                            target.Save();
                            pRole.Send(target.InformationPacket(true));
                            ServerKernel.Log.GmLog("Improve", string.Format("User[{0}] item [{1}] type [{2}] nChance[{3}] used[{4}]", pRole.Identity, target.Identity, target.Type, nChance, SpecialItem.TYPE_DRAGONBALL));
                        }
                        break;
                    }
                #endregion
                #region 20 - Uplevel
                case ItemAction.UPLEV:
                    {
                        Item item;
                        if (pRole.Inventory.Items.TryGetValue(pMsg.Identity, out item))
                        {
                            if (item.Durability / 100 < item.MaximumDurability / 100)
                            {
                                pRole.Send(ServerString.STR_REPAIR_THEN_UPGRADE);
                                return;
                            }

                            int idNewType = 0;
                            double nChance = 0.00;

                            if (!item.GetUpLevelChance((int)item.Type, out nChance, out idNewType))
                            {
                                pRole.Send(ServerString.STR_UPGRADE_NOMORE);
                                return;
                            }

                            DbItemtype dbNewType = ServerKernel.Itemtype.Values.FirstOrDefault(x => x.Type == idNewType);

                            if (dbNewType == null)
                            {
                                pRole.Send(ServerString.STR_UPGRADE_NOMORE);
                                return;
                            }

                            if (!pRole.Inventory.ReduceMeteors(1))
                            {
                                pRole.Send(ServerString.STR_ITEM_NOT_FOUND);
                                return;
                            }

                            if (Calculations.ChanceCalc((float)nChance))
                            {
                                item.Type = (uint)idNewType;
                                pRole.SendEffect("improveSuc", false);
                            }
                            else
                            {
                                item.Durability /= 2;
                            }

                            if (Calculations.ChanceCalc(1f))
                            {
                                bool changed = false;
                                if (item.SocketOne == SocketGem.NO_SOCKET)
                                {
                                    changed = true;
                                    item.SocketOne = SocketGem.EMPTY_SOCKET;
                                }
                                else if (item.SocketTwo == SocketGem.NO_SOCKET)
                                {
                                    changed = true;
                                    item.SocketTwo = SocketGem.EMPTY_SOCKET;
                                }
                                if (changed)
                                    pRole.Send(ServerString.STR_IMPROVE_UPGRADE_SOCKET);
                            }

                            item.Save();
                            pRole.Send(item.InformationPacket(true));
                            ServerKernel.Log.GmLog("Upgrade", string.Format("User[{0}] item [{1}] type [{2}] nChance[{3}] used[{4}]", pRole.Identity, item.Identity, item.Type, nChance, SpecialItem.TYPE_DRAGONBALL));
                        }
                        break;
                    }
                #endregion
                #region 21 - Query Booth
                case ItemAction.BOOTH_QUERY:
                    {
                        var pRoleList = pRole.Map.CollectMapThing(18, new Point(pRole.MapX, pRole.MapY));
                        Character pUser = null;
                        foreach (var role in pRoleList)
                        {
                            if (role is Character)
                            {
                                var tempUser = role as Character;
                                if (tempUser.Booth == null || !tempUser.Booth.Vending)
                                    continue;
                                if (tempUser.Booth.Identity == pMsg.Identity)
                                {
                                    pUser = tempUser;
                                    break;
                                }
                            }
                        }
                        if (pUser == null) return;

                        foreach (var item in pUser.Booth.Items.Values.OrderBy(x => x.Value))
                        {
                            BoothItem trash;
                            if (!pUser.Inventory.Items.ContainsKey(item.Item.Identity))
                            {
                                pUser.Booth.Items.TryRemove(item.Item.Identity, out trash);
                                return;
                            }
                            Item pItem = item.Item;
                            MsgItemInfoEx msg = new MsgItemInfoEx
                            {
                                Identity = pItem.Identity,
                                Bless = pItem.ReduceDamage,
                                Color = pItem.Color,
                                Durability = pItem.Durability,
                                Enchant = pItem.Enchantment,
                                Itemtype = pItem.Type,
                                MaximumDurability = pItem.MaximumDurability,
                                Plus = pItem.Plus,
                                Price = item.Value,
                                ViewType = (ushort)(item.IsSilver ? 1 : 3),
                                SocketOne = pItem.SocketOne,
                                SocketTwo = pItem.SocketTwo,
                                TargetIdentity = pMsg.Identity,
                                SocketProgress = pItem.SocketProgress,
                                StackAmount = pItem.StackAmount,
                                Position = ItemPosition.INVENTORY
                            };
                            pRole.Send(msg);
                            pItem.SendPurification(pRole);
                        }
                        break;
                    }
                #endregion
                #region 22 - Add item to booth
                case ItemAction.BOOTH_ADD:
                    {
                        if (pRole.Booth == null || !pRole.Booth.Vending)
                            return;
                        if (pRole.Booth.Items.ContainsKey(pMsg.Identity))
                            return;

                        Item item;
                        if (!pRole.Inventory.Items.TryGetValue(pMsg.Identity, out item))
                            return;

                        if (item.CanBeSold)
                        {
                            var pSale = new BoothItem();
                            if (!pSale.Create(item, pMsg.Param1, true))
                                return;
                            pRole.Booth.Items.TryAdd(item.Identity, pSale);
                            pRole.Send(pMsg);
                        }
                        break;
                    }
                #endregion
                #region 23 - Remove item from booth
                case ItemAction.BOOTH_DELETE:
                    {
                        if (pRole.Booth == null || !pRole.Booth.Vending)
                            return;

                        BoothItem item;
                        if (!pRole.Booth.Items.TryRemove(pMsg.Identity, out item))
                            return;

                        pRole.Send(pMsg);
                        break;
                    }
                #endregion
                #region 24 - Booth Buy
                case ItemAction.BOOTH_BUY:
                    {
                        if (pRole.Inventory.IsFull) return;

                        var pScreenObj = pRole.Map.QueryRole(pMsg.Param1);
                        if (pScreenObj == null)
                            return;

                        uint idTarget = ((pScreenObj.Identity - pScreenObj.Identity % 100000) * 10) +
                                        pScreenObj.Identity % 100000;
                        var pRoleTarget = pRole.Map.QueryRole(idTarget);
                        if (pRole.GetDistance(pRoleTarget) > Calculations.SCREEN_DISTANCE)
                            return;

                        var pUser = pRoleTarget as Character;
                        if (pUser == null)
                            return;
                        if (pUser.Booth == null || !pUser.Booth.Vending)
                            return;

                        BoothItem boothItem;
                        if (!pUser.Inventory.Items.ContainsKey(pMsg.Identity))
                            return; // user doesnt ahve the item anymore

                        if (pUser.Booth.Items.TryGetValue(pMsg.Identity, out boothItem))
                        {
                            if (boothItem.IsSilver)
                            {
                                if (pRole.ReduceMoney(boothItem.Value, true))
                                    pUser.AwardMoney(boothItem.Value);
                                else return;
                            }
                            else
                            {
                                if (pRole.ReduceEmoney(boothItem.Value, true))
                                    pUser.AwardEmoney(boothItem.Value);
                                else return;
                            }

                            var msg = new MsgItem(pMsg);
                            msg.Action = ItemAction.BOOTH_DELETE;
                            pUser.Send(msg);
                            pUser.Booth.Items.TryRemove(pMsg.Identity, out boothItem);
                            pUser.Inventory.Remove(pMsg.Identity, ItemRemovalType.TAKE_OUT_FROM_INVENTORY_ONLY);
                            pRole.Inventory.Add(boothItem.Item);
                            boothItem.Item.ChangeOwner(pRole);
                            pRole.Send(pMsg);
                            pUser.Send(pMsg);
                            msg.Action = ItemAction.REMOVE;
                            pUser.Send(msg);

                            pRole.Send(string.Format(ServerString.STR_BOOTH_BOUGHT,
                                boothItem.Item.Itemtype.Name, boothItem.Value, boothItem.IsSilver ? "silvers" : "CPs"));
                            pUser.Send(string.Format(ServerString.STR_BOOTH_USER_BOUTH,
                                pRole.Name, boothItem.Item.Itemtype.Name, boothItem.Value, boothItem.IsSilver ? "silvers" : "CPs"), ChatTone.TALK);

                            ServerKernel.Log.GmLog("booth",
                                string.Format("[userid({0}), bought({1}:{3}), from({2})]"
                                , pRole.Identity, boothItem.Item.Identity, pUser.Identity, boothItem.Item.Type));
                        }

                        break;
                    }
                #endregion
                #region 27 - Ping
                case ItemAction.PING:
                {
                    pRole.Send(pMsg);
                    break;
                }
                #endregion
                #region 28 - Enchant
                case ItemAction.ENCHANT:
                {
                    Item target, source;
                    if (pRole.Inventory.Items.TryGetValue(pMsg.Identity, out target)
                        && pRole.Inventory.Items.TryGetValue(pMsg.Param1, out source))
                    {
                        if (target.Enchantment >= 255)
                            return; // max enchant already

                        if (source.Type / 100 != 7000)
                            return;  // not a gem

                        pRole.Inventory.Remove(source.Identity);

                        byte gemId = (byte)(source.Type % 100);

                        byte enchant = 1;
                        if (gemId % 10 == 1) //Normal
                            enchant = (byte)ThreadSafeRandom.RandGet(1, 59);
                        else if (gemId == 2 || gemId == 52 || gemId == 62) //Reffined (Phoenix/Violet/Moon)
                            enchant = (byte)ThreadSafeRandom.RandGet(60, 109);
                        else if (gemId == 22 || gemId == 42 || gemId == 72) //Reffined (Fury/Kylin/Tortoise)
                            enchant = (byte)ThreadSafeRandom.RandGet(40, 89);
                        else if (gemId == 12) //Reffined (Dragon)
                            enchant = (byte)ThreadSafeRandom.RandGet(100, 159);
                        else if (gemId == 32) //Reffined (Rainbow)
                            enchant = (byte)ThreadSafeRandom.RandGet(80, 129);
                        else if (gemId == 3 || gemId == 33 || gemId == 73) //Super (Phoenix/Rainbow/Tortoise)
                            enchant = (byte)ThreadSafeRandom.RandGet(170, 229);
                        else if (gemId == 53 || gemId == 63) //Super (Violet/Moon)
                            enchant = (byte)ThreadSafeRandom.RandGet(140, 199);
                        else if (gemId == 13) //Reffined (Dragon)
                            enchant = (byte)ThreadSafeRandom.RandGet(200, 255);
                        else if (gemId == 23) //Reffined (Fury)
                            enchant = (byte)ThreadSafeRandom.RandGet(90, 149);
                        else if (gemId == 43) //Reffined (Kylin)
                            enchant = (byte)ThreadSafeRandom.RandGet(70, 119);

                        pMsg.Param1 = enchant;
                        pRole.Send(pMsg);

                        if (enchant > target.Enchantment)
                        {
                            target.Enchantment = enchant;
                            target.Save();
                            pRole.Send(target.InformationPacket(true));
                        }
                        ServerKernel.Log.GmLog("enchant", string.Format("User[{0}] Enchant[Gem: {1}|{2}][Target: {3}|{4}] with {5} points.", pRole.Identity, source.Type, source.Identity, target.Type, target.Identity, enchant));
                    }
                    break;
                }
                #endregion
                #region 29 - Add item to booth CPs
                case ItemAction.BOOTH_ADD_CP:
                {
                    if (pRole.Booth == null || !pRole.Booth.Vending)
                        return;
                    if (pRole.Booth.Items.ContainsKey(pMsg.Identity))
                        return;

                    Item item;
                    if (!pRole.Inventory.Items.TryGetValue(pMsg.Identity, out item))
                        return;

                    if (item.CanBeSold)
                    {
                        var pSale = new BoothItem();
                        if (!pSale.Create(item, pMsg.Param1, false))
                            return;
                        pRole.Booth.Items.TryAdd(item.Identity, pSale);
                        pRole.Send(pMsg);
                    }
                    break;
                }
                #endregion
                #region 32 - Redeem Equipment
                case ItemAction.REDEEM_EQUIPMENT:
                {
                    DetainedObject pObj;
                    if (!ServerKernel.DetainedObjects.TryGetValue(pMsg.Identity, out pObj))
                        return;

                    if (pObj.TargetIdentity != pRole.Identity)
                    {
                        pRole.Send("You are not the owner of the detained equipment.");
                        return;
                    }

                    if (pObj.HasExpired)
                    {
                        pRole.Send("This equipment has expired and you cannot claim it anymore.");
                        return;
                    }

                    if (pRole.Inventory.IsFull)
                    {
                        pRole.Send("Please leave one empty block in your inventory.");
                        return;
                    }

                    DbItem item = Database.Items.FetchByIdentity(pObj.ItemIdentity);
                    if (item == null)
                    {
                        ServerKernel.Log.SaveLog(pObj.Identity + " item disappeared REDEEM_EQUIPMENT", true, LogType.ERROR);
                        pRole.Send("Something went wrong with your request.");
                        return;
                    }

                    if (item.Position != (byte) ItemPosition.DETAINED)
                    {
                        pRole.Send("This item isn't detained.");
                        return;
                    }

                    if (!pRole.ReduceEmoney(pObj.Value))
                    {
                        pRole.Send("Not enough CPs.");
                        return;
                    }

                    Item pItem = new Item(pRole, item);
                    pItem.Position = ItemPosition.INVENTORY;
                    pRole.Inventory.Add(pItem);

                    ServerKernel.DetainedObjects.TryRemove(pObj.Identity, out pObj);
                    pObj.Delete();

                    DbPkReward pReward = new DbPkReward
                    {
                        HunterIdentity = pObj.HunterIdentity,
                        HunterName = pObj.HunterName,
                        TargetIdentity = pObj.TargetIdentity,
                        TargetName = pObj.TargetName,
                        Bonus = pObj.Value,
                        BonusType = 1
                    };
                    new PkBonusRepository().SaveOrUpdate(pReward);

                    DetainedObject newObj = new DetainedObject(false);
                    newObj.Create(pReward);
                    ServerKernel.DetainedObjects.TryAdd(newObj.Identity, newObj);

                    ServerKernel.SendMessageToAll(
                        string.Format("{0} has paid {1} CPs to {2} to claim his/her {3}.",
                        pObj.TargetName, pObj.Value, pObj.HunterName, pItem.Itemtype.Name), ChatTone.TALK);

                    pRole.Send(pMsg);

                    Client owner;
                    if (ServerKernel.Players.TryGetValue(pObj.HunterIdentity, out owner))
                    {
                        MsgItem pRemoveFromOwner = new MsgItem(pMsg)
                        {
                            Action = ItemAction.PK_ITEM_REDEEM
                        };
                        owner.Send(pRemoveFromOwner);
                        owner.Send(newObj.GetPacket());
                        owner.Send(newObj.GetPrizePacket());
                    }
                    break;
                }
                #endregion
                #region 33 - Claim Reward
                case ItemAction.PK_ITEM_REDEEM:
                {
                    // redeem obj id (identity)
                    DetainedObject pObj;
                    if (!ServerKernel.DetainedObjects.TryGetValue(pMsg.Identity, out pObj))
                        return;

                    if (pObj.HunterIdentity != pRole.Identity)
                    {
                        pRole.Send("You are not the owner of this reward.");
                        return;
                    }

                    if (pObj.IsItem())
                    {
                        if (pRole.Inventory.IsFull)
                        {
                            pRole.Send("Your inventory is full.");
                            return;
                        }

                        DbItem item = Database.Items.FetchByIdentity(pObj.ItemIdentity);
                        if (item == null)
                        {
                            ServerKernel.Log.SaveLog(pObj.Identity + " item disappeared pk_item_redeem", true, LogType.ERROR);
                            pRole.Send("Something went wrong with your request.");
                            return;
                        }

                        Item newItem = new Item(pRole, item);
                        newItem.ChangeOwner(pRole);
                        pRole.Inventory.Add(newItem);

                        string szItemName = newItem.Itemtype.Name;
                        if (newItem.Plus > 0)
                            szItemName += string.Format("(+{0})", newItem.Plus);
                        ServerKernel.SendMessageToAll(
                            string.Format("{0} has claimed a {1} for arresting {2}.", pRole.Name, szItemName,
                                pObj.TargetName),
                            ChatTone.TALK);

                        ServerKernel.DetainedObjects.TryRemove(pObj.Identity, out pObj);
                        pObj.Delete();

                        Client owner;
                        if (ServerKernel.Players.TryGetValue(pObj.TargetIdentity, out owner))
                        {
                            owner.Send(pMsg);
                        }
                        pRole.Send(pMsg);
                    } 
                    else if (pObj.IsEmoney())
                    {
                        ServerKernel.DetainedObjects.TryRemove(pObj.Identity, out pObj);
                        pObj.Delete();

                        pRole.AwardEmoney(pObj.Value);
                        ServerKernel.SendMessageToAll(
                            string.Format("{0} has claimed {1}CPs for detaining {2}'s equipment.", pRole.Name,
                                pObj.Value, pObj.TargetName), ChatTone.TALK);
                        pRole.Send(pMsg);
                    }
                    else
                    {
                        // ? silver reward only for jail should not be here
                        ServerKernel.Log.SaveLog("silver reward for pk_item_redeem??? " + pObj.Identity, true, LogType.DEBUG);
                        return;
                    }
                    break;
                }
                #endregion
                #region 35 - Talisman Socket Item
                case ItemAction.TALISMAN_SOCKET_PROGRESS:
                {
                    Item target, source;
                    if (pRole.Equipment.GetByIdentity(pMsg.Identity, out target)
                        && pRole.Inventory.Items.TryGetValue(pMsg.Param1, out source))
                    {
                        uint type = source.Type / 1000;
                        if (type == 201 || type == 202 || type == 203)
                            return;

                        if (!pRole.Inventory.Remove(source.Identity))
                            return;

                        target.SocketProgress += source.CalculateSocketingProgress();
                        if (target.SocketOne == SocketGem.NO_SOCKET && target.SocketProgress >= 8000)
                        {
                            target.SocketProgress = 0;
                            target.SocketOne = SocketGem.EMPTY_SOCKET;
                        }
                        else if (target.SocketTwo == SocketGem.NO_SOCKET
                                 && target.SocketProgress >= 20000)
                        {
                            target.SocketProgress = 0;
                            target.SocketTwo = SocketGem.EMPTY_SOCKET;
                        }
                        target.Save();
                        pRole.Send(target.InformationPacket(true));
                        pRole.Send(pMsg);
                    }
                    break;
                }
                #endregion
                #region 36 - Talisman Socket CPs
                case ItemAction.TALISMAN_SOCKET_PROGRESS_CPS:
                {
                    Item item;
                    uint dwPrice = 0;
                    if (!pRole.Equipment.GetByIdentity(pMsg.Identity, out item))
                        return;

                    if (item.SocketOne == SocketGem.NO_SOCKET)
                    {
                        if (item.SocketProgress < 2400) return;
                        dwPrice = (uint)(5600 * (1 - item.SocketProgress / 8000f));
                    }
                    else if (item.SocketTwo == SocketGem.NO_SOCKET)
                    {
                        if (item.SocketProgress < 6000) return;
                        dwPrice = (uint)(14000 * (1 - item.SocketProgress / 20000f));
                    }
                    else
                    {
                        return; // player has 2 sockets
                    }

                    if (!pRole.ReduceEmoney(dwPrice, true))
                        return;

                    item.SocketProgress = 0;
                    if (item.SocketOne == SocketGem.NO_SOCKET)
                        item.SocketOne = SocketGem.EMPTY_SOCKET;
                    else
                        item.SocketTwo = SocketGem.EMPTY_SOCKET;

                    pRole.Send(item.InformationPacket(true));
                    pRole.Send(pMsg);
                    break;
                }
                #endregion
                #region 37 - Drop Item
                case ItemAction.DROP:
                {
                    pRole.DropItem(pMsg.Identity, pRole.MapX, pRole.MapY);
                    break;
                }
                #endregion
                #region 40 - Blessing
                case ItemAction.TORTOISE_COMPOSE:
                {
                    Item target;
                    if (pRole.Inventory.Items.TryGetValue(pMsg.Identity, out target))
                    {
                        if (target.Durability == 0)
                            return;
                        if (target.ReduceDamage >= 7)
                        {
                            pRole.Send("This item already have the maximum blessing.");
                            return;
                        }
                        if (target.Durability / 100 < target.MaximumDurability / 100)
                        {
                            pRole.Send("Please repair your item.");
                            return;
                        }
                        ItemPosition position = Calculations.GetItemPosition(target.Type);
                        if (position <= ItemPosition.INVENTORY
                            || position == ItemPosition.BOTTLE
                            || position > ItemPosition.BOOTS)
                        {
                            pRole.Send("This item cannot be blessed.");
                            return;
                        }

                        switch (target.ReduceDamage)
                        {
                            case 0:
                                {
                                    if (!pRole.Inventory.DeleteMultiple(700073, 700073, 5))
                                    {
                                        pRole.Send("You don't have enough Tortoise Gems.");
                                        return;
                                    }
                                    target.ReduceDamage = 1;
                                    break;
                                }
                            case 1:
                                {
                                    if (!pRole.Inventory.Remove(700073, 1))
                                    {
                                        pRole.Send("You don't have a Tortoise Gem.");
                                        return;
                                    }
                                    target.ReduceDamage = 3;
                                    break;
                                }
                            case 3:
                                {
                                    if (!pRole.Inventory.DeleteMultiple(700073, 700073, 3))
                                    {
                                        pRole.Send("You don't have enough Tortoise Gems.");
                                        return;
                                    }
                                    target.ReduceDamage = 5;
                                    break;
                                }
                            case 5:
                                {
                                    if (!pRole.Inventory.DeleteMultiple(700073, 700073, 5))
                                    {
                                        pRole.Send("You don't have enough Tortoise Gems.");
                                        return;
                                    }
                                    target.ReduceDamage = 7;
                                    break;
                                }
                            default:
                                return;
                        }

                        target.Save();
                        pRole.Send(target.InformationPacket(true));
                        pMsg.Param1 = 1;
                        pRole.Send(pMsg);
                    }
                    break;
                }
                #endregion
                #region 41 - Activate Accessory
                case ItemAction.ACTIVATE_ACCESSORY:
                {
                    Item item;
                    if (pRole.Inventory.Items.TryGetValue(pMsg.Identity, out item))
                    {
                        if (item.RemainingTime > 0 || item.Itemtype.LifeTime <= 0)
                            return;
                        item.RemainingTime = (uint)UnixTimestamp.Timestamp() + item.Itemtype.LifeTime*60;
                        item.Save();
                        pRole.Send(item.InformationPacket(true));
                        pRole.Send(pMsg);
                        return;
                    }
                    return;
                }
                #endregion
                #region 43 - Socket Equipment
                case ItemAction.SOCKET_EQUIPMENT:
                {
                    Item target;
                    if (pRole.Inventory.Items.TryGetValue(pMsg.Identity, out target))
                    {
                        if (target.Durability / 100 < target.MaximumDurability / 100)
                        {
                            pRole.Send("Please repair your item.");
                            return;
                        }
                        ItemPosition position = Calculations.GetItemPosition(target.Type);
                        pMsg.Param1 = 0;
                        switch (pMsg.Param2)
                        {
                            // 1 DB or 1 Tough
                            case 1:
                                {
                                    if (position == ItemPosition.RIGHT_HAND
                                        && target.SocketOne == SocketGem.NO_SOCKET)
                                    {
                                        if (!pRole.Inventory.ReduceDragonBalls(1, target.Bound, false))
                                        {
                                            pRole.Send("You don't have enough Dragon Balls.");
                                            pRole.Send(pMsg);
                                            return;
                                        }

                                        target.SocketOne = SocketGem.EMPTY_SOCKET;
                                        target.Save();
                                        pRole.Send(target.InformationPacket(true));
                                        pMsg.Param1 = 1;
                                        pRole.Send(pMsg);
                                        ServerKernel.Log.GmLog("socket_item",
                                            string.Format("User[{0}] Id[{1}] OpenHole[1] ItemId[{2}] Itemtype[{3}]",
                                                pRole.Name, pRole.Identity, target.Identity, target.Itemtype.Type));
                                        return;
                                    }
                                    if (target.SocketOne > SocketGem.NO_SOCKET)
                                    {
                                        if (target.SocketTwo != SocketGem.NO_SOCKET)
                                        {
                                            pRole.Send("This item already have the max amount of socket.");
                                            pRole.Send(pMsg);
                                            return;
                                        }

                                        if (!pRole.Inventory.Remove(SpecialItem.TYPE_TOUGHDRILL, 1))
                                        {
                                            pRole.Send("You don't have enough ToughDrills.");
                                            pRole.Send(pMsg);
                                            return;
                                        }

                                        if (!Calculations.ChanceCalc(14.3f))
                                        {
                                            pRole.Send("I couldn't make your socket. Take this as compensation.");
                                            pRole.Send(pMsg);
                                            pRole.Inventory.Create(SpecialItem.TYPE_STARDRILL);
                                            return;
                                        }

                                        ServerKernel.SendMessageToAll(
                                            string.Format(
                                                "{0} has oppened a second socket in his/her equipment with a ToughDrill.",
                                                pRole.Name), ChatTone.TOP_LEFT);

                                        target.SocketTwo = SocketGem.EMPTY_SOCKET;
                                        target.Save();
                                        pRole.Send(target.InformationPacket(true));
                                        pMsg.Param1 = 1;
                                        pRole.Send(pMsg);
                                        return;
                                    }
                                    //target.UpdateTotemPole();
                                    pRole.Send(pMsg);
                                    ServerKernel.Log.GmLog("socket_item",
                                string.Format("User[{0}] Id[{1}] OpenHole[2] ItemId[{2}] Itemtype[{3}]",
                                    pRole.Name, pRole.Identity, target.Identity, target.Itemtype.Type));
                                    return;
                                }
                            // 5 Dragon Balls 2 Socket Weapon
                            case 5:
                                {
                                    if (target.SocketOne == SocketGem.NO_SOCKET)
                                    {
                                        pRole.Send("Your item has no socket.");
                                        pRole.Send(pMsg);
                                        return;
                                    }
                                    if (target.SocketTwo > SocketGem.NO_SOCKET)
                                    {
                                        pRole.Send("Your item already have the max amount of sockets.");
                                        pRole.Send(pMsg);
                                        return;
                                    }
                                    if (!pRole.Inventory.ReduceDragonBalls(5, target.Bound, false))
                                    {
                                        pRole.Send("You don't have enough DragonBalls.");
                                        pRole.Send(pMsg);
                                        return;
                                    }

                                    target.SocketTwo = SocketGem.EMPTY_SOCKET;
                                    target.Save();
                                    pRole.Send(target.InformationPacket(true));
                                    pMsg.Param1 = 1;
                                    pRole.Send(pMsg);
                                    //target.UpdateTotemPole();
                                    ServerKernel.Log.GmLog("socket_item",
                                string.Format("User[{0}] Id[{1}] OpenHole[2] ItemId[{2}] Itemtype[{3}]",
                                    pRole.Name, pRole.Identity, target.Identity, target.Itemtype.Type));
                                    return;
                                }
                            // 7 StarDrills 2 Sockets
                            case 7:
                                {
                                    if (target.SocketOne == SocketGem.NO_SOCKET)
                                    {
                                        pRole.Send("Your item has no socket.");
                                        pRole.Send(pMsg);
                                        return;
                                    }
                                    if (target.SocketTwo > SocketGem.NO_SOCKET)
                                    {
                                        pRole.Send("Your item already have the max amount of sockets.");
                                        pRole.Send(pMsg);
                                        return;
                                    }
                                    if (!pRole.Inventory.DeleteMultiple(SpecialItem.TYPE_STARDRILL,
                                            SpecialItem.TYPE_STARDRILL, 7))
                                    {
                                        pRole.Send("You don't have the required items.");
                                        pRole.Send(pMsg);
                                        return;
                                    }

                                    target.SocketTwo = SocketGem.EMPTY_SOCKET;
                                    target.Save();
                                    pRole.Send(target.InformationPacket(true));
                                    pMsg.Param1 = 1;
                                    pRole.Send(pMsg);
                                    //target.UpdateTotemPole();
                                    ServerKernel.Log.GmLog("socket_item",
                                string.Format("User[{0}] Id[{1}] OpenHole[2] ItemId[{2}] Itemtype[{3}]",
                                    pRole.Name, pRole.Identity, target.Identity, target.Itemtype.Type));
                                    return;
                                }
                            case 12:
                                {
                                    if (target.SocketOne > SocketGem.NO_SOCKET)
                                    {
                                        pRole.Send("Your item is already socketed.");
                                        pRole.Send(pMsg);
                                        return;
                                    }
                                    if (!pRole.Inventory.ReduceDragonBalls(12, target.Bound, false))
                                    {
                                        pRole.Send("You don't have the required items.");
                                        pRole.Send(pMsg);
                                        return;
                                    }

                                    target.SocketOne = SocketGem.EMPTY_SOCKET;
                                    target.Save();
                                    pRole.Send(target.InformationPacket(true));
                                    pMsg.Param1 = 1;
                                    pRole.Send(pMsg);
                                    //target.UpdateTotemPole();
                                    ServerKernel.Log.GmLog("socket_item",
                                string.Format("User[{0}] Id[{1}] OpenHole[1] ItemId[{2}] Itemtype[{3}]",
                                    pRole.Name, pRole.Identity, target.Identity, target.Itemtype.Type));
                                    return;
                                }
                        }
                    }
                    break;
                }
                #endregion
                #region 48 - Merge Items
                case ItemAction.MERGE_ITEMS:
                {
                    pRole.Inventory.StackItem(pMsg.Identity, pMsg.Param1);
                    break;
                }
                #endregion
                #region 49 - Split Items
                case ItemAction.SPLIT_ITEMS:
                {
                    pRole.Inventory.SplitItem(pMsg.Identity, (ushort)pMsg.Param1);
                    break;
                }
                #endregion
                #region 52 - Item tooltip
                case ItemAction.REQUEST_ITEM_TOOLTIP:
                {
                    DbItem dbItem = Database.Items.FetchByIdentity(pMsg.Identity);
                    if (dbItem == null)
                    {
                        pRole.Send("Could not get item information.");
                        return;
                    }
                    Item item = new Item(pRole, dbItem);
                    MsgItemInformation iPacket = item.InformationPacket();
                    iPacket.ItemMode = ItemMode.CHAT_ITEM;
                    pRole.Send(iPacket);
                    if (item.ArtifactIsActive() || item.RefineryIsActive())
                        item.SendPurification(pRole);
                    break;
                }
                #endregion
                #region 54 - Degrade Equipment
                case ItemAction.DEGRADE_EQUIPMENT:
                {
                    Item item;
                    if (pRole.Inventory.Items.TryGetValue(pMsg.Identity, out item))
                    {
                        if (pRole.Emoney < 54)
                        {
                            pRole.Send("You don't have enough CPs.");
                            return;
                        }

                        if (!item.DegradeItem())
                        {
                            pRole.Send("The item couldn't be degraded.");
                            return;
                        }

                        pRole.ReduceEmoney(54);
                        return;
                    }
                    break;
                }
                #endregion
            }
        }
    }
}