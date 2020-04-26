// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Inventory.cs
// Last Edit: 2016/11/24 00:09
// Created: 2016/11/23 21:46

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DB.Entities;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Items
{
    public sealed class Inventory
    {
        private Character m_pOwner;
        public readonly ConcurrentDictionary<uint, Item> Items;

        public Inventory(Character owner)
        {
            m_pOwner = owner;
            Items = new ConcurrentDictionary<uint, Item>();
        }

        public bool Create(uint dwType, ushort usAmount, byte pMonopoly = 0)
        {
            if (IsFull) return false;

            DbItemtype itemInfo;
            if (ServerKernel.Itemtype.TryGetValue(dwType, out itemInfo))
            {
                //while (usAmount > 0)
                {
                    //ushort stack = 1;
                    //if (itemInfo.StackLimit < usAmount)
                    //{
                    //    stack = itemInfo.StackLimit;
                    //    usAmount -= itemInfo.StackLimit;
                    //}
                    //else
                    //{
                    //    stack = usAmount;
                    //    usAmount = 0;
                    //}
                    if (usAmount <= 0)
                        usAmount = 1;
                    var pItem = new DbItem
                    {
                        AddLife = 0,
                        AddlevelExp = 0,
                        AntiMonster = 0,
                        ArtifactExpire = 0,
                        ArtifactType = 0,
                        ArtifactStabilization = 0,
                        ArtifactStart = 0,
                        ChkSum = 0,
                        Color = 3,
                        Data = 0,
                        Gem1 = 0,
                        Gem2 = 0,
                        Ident = 0,
                        Magic1 = 0,
                        Magic2 = 0,
                        ReduceDmg = 0,
                        Plunder = 0,
                        Specialflag = 0,
                        Inscribed = 0,
                        StackAmount = usAmount,
                        RefineryExpire = 0,
                        RefineryLevel = 0,
                        RefineryType = 0,
                        RefineryStart = 0,
                        RefineryStabilization = 0,
                        Type = itemInfo.Type,
                        Position = 0,
                        PlayerId = m_pOwner.Identity,
                        Monopoly = pMonopoly,
                        Magic3 = itemInfo.Magic3,
                        Amount = itemInfo.Amount,
                        AmountLimit = itemInfo.AmountLimit
                    };

                    var newItem = new Item(m_pOwner, pItem);
                    newItem.Save();
                    if (Add(newItem))
                    {
                        ServerKernel.Log.GmLog("item_creation",
                            string.Format(
                                "SYSTEM created for user item Item [Id:{0}][Type:{8}][Pos:{9}][Plus:{1}][Dura:{2}/{3}][Enchant:{4}][Bless:{5}][Plunder:{6}][Data:{7}]"
                                , pItem.Id, pItem.Magic3, pItem.Amount, pItem.AmountLimit, pItem.AddLife,
                                pItem.ReduceDmg,
                                pItem.Plunder, pItem.Data, pItem.Type, pItem.Position));
                    }
                    else
                    {
                        ServerKernel.Log.GmLog("item_creation_fail", string.Format("SYSTEM failed to create item {0} {1}", dwType, usAmount));
                    }
                }
            }
            return false;
        }

        public bool Create(uint dwType, byte pMonopoly = 0)
        {
            if (IsFull) return false;

            DbItemtype itemInfo;
            if (ServerKernel.Itemtype.TryGetValue(dwType, out itemInfo))
            {
                var pItem = new DbItem
                {
                    AddLife = 0,
                    AddlevelExp = 0,
                    AntiMonster = 0,
                    ArtifactExpire = 0,
                    ArtifactType = 0,
                    ArtifactStabilization = 0,
                    ArtifactStart = 0,
                    ChkSum = 0,
                    Color = 3,
                    Data = 0,
                    Gem1 = 0,
                    Gem2 = 0,
                    Ident = 0,
                    Magic1 = 0,
                    Magic2 = 0,
                    ReduceDmg = 0,
                    Plunder = 0,
                    Specialflag = 0,
                    Inscribed = 0,
                    StackAmount = 1,
                    RefineryExpire = 0,
                    RefineryLevel = 0,
                    RefineryType = 0,
                    RefineryStart = 0,
                    RefineryStabilization = 0,
                    Type = itemInfo.Type,
                    Position = 0,
                    PlayerId = m_pOwner.Identity,
                    Monopoly = pMonopoly,
                    Magic3 = itemInfo.Magic3,
                    Amount = itemInfo.Amount,
                    AmountLimit = itemInfo.AmountLimit
                };

                var newItem = new Item(m_pOwner, pItem);
                newItem.Save();
                if (Add(newItem))
                {
                    ServerKernel.Log.GmLog("item_creation", string.Format("SYSTEM created for user item Item [Id:{0}][Type:{8}][Pos:{9}][Plus:{1}][Dura:{2}/{3}][Enchant:{4}][Bless:{5}][Plunder:{6}][Data:{7}]"
                                , pItem.Id, pItem.Magic3, pItem.Amount, pItem.AmountLimit, pItem.AddLife, pItem.ReduceDmg,
                                pItem.Plunder, pItem.Data, pItem.Type, pItem.Position));
                    return true;
                }
            }
            return false;
        }

        public bool CreateJar(ushort typeMonster = 0, ushort requiredKills = 0)
        {
            if (IsFull) return false;

            DbItemtype itemInfo;
            if (ServerKernel.Itemtype.TryGetValue(SpecialItem.CLOUDSAINTS_JAIR, out itemInfo))
            {
                var pItem = new DbItem
                {
                    AddLife = 0,
                    AddlevelExp = 0,
                    AntiMonster = 0,
                    ArtifactExpire = 0,
                    ArtifactType = 0,
                    ArtifactStabilization = 0,
                    ArtifactStart = 0,
                    ChkSum = 0,
                    Color = 3,
                    Data = 0,
                    Gem1 = 0,
                    Gem2 = 0,
                    Ident = 0,
                    Magic1 = 0,
                    Magic2 = 0,
                    ReduceDmg = 0,
                    Plunder = 0,
                    Specialflag = 0,
                    Inscribed = 0,
                    StackAmount = 1,
                    RefineryExpire = 0,
                    RefineryLevel = 0,
                    RefineryType = 0,
                    RefineryStart = 0,
                    RefineryStabilization = 0,
                    Type = itemInfo.Type,
                    Position = 0,
                    PlayerId = m_pOwner.Identity,
                    Monopoly = 0,
                    Magic3 = itemInfo.Magic3,
                    Amount = requiredKills,
                    AmountLimit = typeMonster
                };

                var newItem = new Item(m_pOwner, pItem);
                newItem.Save();
                if (Add(newItem))
                {
                    ServerKernel.Log.GmLog("item_creation", string.Format("SYSTEM created for user item Item [Id:{0}][Type:{8}][Pos:{9}][Plus:{1}][Dura:{2}/{3}][Enchant:{4}][Bless:{5}][Plunder:{6}][Data:{7}]"
                                , pItem.Id, pItem.Magic3, pItem.Amount, pItem.AmountLimit, pItem.AddLife, pItem.ReduceDmg,
                                pItem.Plunder, pItem.Data, pItem.Type, pItem.Position));
                    return true;
                }
            }
            return false;
        }

        public bool Create(DbItem item)
        {
            if (IsFull)
                return false;

            var _item = new Item(m_pOwner, item);
            _item.Save();

            if (Add(_item))
            {
                ServerKernel.Log.GmLog("item_creation", string.Format("SYSTEM created for user item Item [Id:{0}][Type:{8}][Pos:{9}][Plus:{1}][Dura:{2}/{3}][Enchant:{4}][Bless:{5}][Plunder:{6}][Data:{7}]"
                            , item.Id, item.Magic3, item.Amount, item.AmountLimit, item.AddLife, item.ReduceDmg,
                            item.Plunder, item.Data, item.Type, item.Position));
                return true;
            }

            return false;
        }

        public Item CreateBased(Item source)
        {
            var item = new Item(m_pOwner)
            {
                OwnerIdentity = source.PlayerIdentity,
                PlayerIdentity = m_pOwner.Identity,
                Type = source.Type,
                Plus = source.Plus,
                Enchantment = source.Enchantment,
                CompositionProgress = source.CompositionProgress,
                ArtifactExpire = source.ArtifactExpire,
                ArtifactType = source.ArtifactType,
                Color = source.Color,
                StackAmount = source.StackAmount,
                Position = 0,
                Monopoly = source.Monopoly,
                Durability = source.Durability,
                MaximumDurability = source.MaximumDurability,
                ArtifactStabilization = source.ArtifactStabilization,
                ArtifactStart = source.ArtifactStart,
                Effect = source.Effect,
                RefineryExpire = source.RefineryExpire,
                RefineryLevel = source.RefineryLevel,
                RefineryStabilization = source.RefineryStabilization,
                RefineryStart = source.RefineryStart,
                RefineryType = source.RefineryType,
                Artifact = source.Artifact,
                Refinery = source.Refinery,
                ReduceDamage = source.ReduceDamage,
                SocketTwo = source.SocketTwo,
                SocketOne = source.SocketOne,
                SocketProgress = source.SocketProgress,
                RemainingTime = source.RemainingTime
            };
            return item;
        }

        public bool Add(Item pItem, bool stackItems = true)
        {
            if (IsFull) return false;

            // Let's make a check on the user inventory, to see if we have more of
            //that item.
            // uint amount = item.StackAmount;
            if (stackItems && pItem.Itemtype.StackLimit > 1)
            {
                foreach (Item item in Items.Values)
                {
                    // If we do, we check if we can insert plus itens into that stack.
                    // If not, we just keep looking.
                    if (item.Type == pItem.Type && pItem.Monopoly == item.Monopoly && !pItem.IsLocked())
                    {
                        if (item.StackAmount < pItem.Itemtype.StackLimit)
                        {
                            int maxamount = pItem.Itemtype.StackLimit - item.StackAmount;
                            if (pItem.StackAmount >= maxamount)
                            {
                                pItem.StackAmount -= (ushort)maxamount;
                                item.StackAmount += (ushort)maxamount;
                            }
                            else
                            {
                                item.StackAmount += pItem.StackAmount;
                                pItem.StackAmount = 0;
                            }

                            m_pOwner.Send(item.InformationPacket(true));
                            item.Save();
                        }
                    }
                }
            }

            pItem.PlayerIdentity = m_pOwner.Identity;
            pItem.Position = ItemPosition.INVENTORY;
            if (pItem.StackAmount > 0)
            {
                var iStack = (byte)(pItem.Itemtype.StackLimit == 0 ? 1 : pItem.Itemtype.StackLimit);
                while (pItem.StackAmount > iStack)
                {
                    Item newCqItem = CreateBased(pItem);
                    newCqItem.StackAmount = iStack;
                    newCqItem.Save();
                    Items.TryAdd(newCqItem.Identity, newCqItem);
                    m_pOwner.Send(newCqItem.InformationPacket());
                    pItem.StackAmount = (byte)(pItem.StackAmount - newCqItem.StackAmount);
                }

                if (Items.TryAdd(pItem.Identity, pItem))
                {
                    pItem.Save();
                    m_pOwner.Send(pItem.InformationPacket());
                    pItem.SendPurification(m_pOwner);
                    pItem.TryUnlockItem();
                    pItem.SendItemLockTime();
                    return true;
                }
            }
            return pItem.Delete();
        }

        public bool Remove(uint dwId, ItemRemovalType type)
        {
            if (type == ItemRemovalType.DELETE)
                return Remove(dwId);

            Item trash;
            if (!Items.TryRemove(dwId, out trash))
                return false;

            if (type == ItemRemovalType.DROP_ITEM)
                trash.Position = ItemPosition.FLOOR;

            var pMsg = new MsgItem
            {
                Action = ItemAction.REMOVE,
                Identity = trash.Identity
            };
            m_pOwner.Send(pMsg);
            return true;
        }

        public bool Remove(uint dwId)
        {
            Item item;
            if (!Items.TryRemove(dwId, out item))
                return false;

            var pMsg = new MsgItem
            {
                Action = ItemAction.REMOVE,
                Identity = dwId
            };
            m_pOwner.Send(pMsg);
            return item.Delete();
        }

        public bool Remove(uint dwId, uint dwAmount, int monopoly = -1)
        {
            if (dwAmount == 0)
                return false;

            var items = new List<Item>();
            ushort usAmount = 0;
            foreach (Item item in Items.Values.Where(x => x.Type == dwId))
            {
                if (usAmount >= dwAmount)
                    break;

                if (monopoly == -1 || item.Monopoly == monopoly)
                {
                    usAmount += item.StackAmount;
                    items.Add(item);
                }
            }

            if (usAmount < dwAmount)
                return false;

            foreach (Item item in items)
            {
                if (item.StackAmount > dwAmount)
                {
                    item.StackAmount -= (ushort)dwAmount;
                    m_pOwner.Send(item.InformationPacket(true));
                    return item.Save();
                }

                usAmount -= item.StackAmount;
                m_pOwner.Send(item.UsagePacket(ItemAction.REMOVE));

                Item trash;
                if (!Items.TryRemove(item.Identity, out trash))
                    return false;

                if (!item.Delete())
                    return false;

                if (usAmount == 0)
                    return true;
            }
            return true;
        }

        public bool Contains(uint dwId)
        {
            return Items.ContainsKey(dwId);
        }

        public bool Contains(uint dwId, byte amount)
        {
            int _amount = 0;
            foreach (var item in Items.Values.Where(x => x.Type == dwId))
            {
                int stack = item.StackAmount > 0 ? item.StackAmount : 1;
                _amount += stack;
                if (_amount >= amount)
                    return true;
            }
            return _amount >= amount;
        }

        public bool StackItem(uint sourceId, uint targetId)
        {
            Item source, target;
            if (Items.TryGetValue(sourceId, out source)
                && Items.TryGetValue(targetId, out target))
            {
                if (source.Type != target.Type)
                    return false;

                if (target.StackAmount >= target.Itemtype.StackLimit)
                    return false;

                int total = target.StackAmount + source.StackAmount;
                if (total <= target.Itemtype.StackLimit)
                {
                    // Delete source
                    target.StackAmount += source.StackAmount;

                    m_pOwner.Send(target.InformationPacket(true));
                    Remove(source.Identity, ItemRemovalType.DELETE);
                    target.Save();
                }
                else
                {
                    // Update source
                    target.StackAmount = target.Itemtype.StackLimit;
                    source.StackAmount = (ushort)(total - target.Itemtype.StackLimit);

                    m_pOwner.Send(target.InformationPacket(true));
                    m_pOwner.Send(source.InformationPacket(true));

                    source.Save();
                    target.Save();
                }
                return true;
            }
            return false;
        }

        public bool SplitItem(uint sourceId, ushort amount)
        {
            Item source, newItem;
            if (Items.TryGetValue(sourceId, out source))
            {
                if (source.StackAmount < amount)
                    return false;

                newItem = CreateBased(source);
                newItem.StackAmount = amount;

                if (!Add(newItem, false))
                    return false;

                source.StackAmount -= amount;

                m_pOwner.Send(source.InformationPacket(true));

                newItem.Save();
                source.Save();
                return true;
            }
            return false;
        }

        public Item GetByType(uint idItem)
        {
            return Items.Values.FirstOrDefault(x => x.Type == idItem);
        }

        public Item GetByID(uint uidItem)
        {
            return Items.Values.FirstOrDefault(x => x.Identity == uidItem);
        }

        public uint MeteorAmount(bool bound = true, bool onlyBound = false)
        {
            uint amount = 0;
            foreach (Item item in Items.Values.Where(item => item.Type == SpecialItem.TYPE_METEOR || item.Type == SpecialItem.TYPE_METEORTEAR || item.Type == SpecialItem.TYPE_METEOR_SCROLL))
            {
                if (!bound && item.Monopoly == 3 && (onlyBound && item.Monopoly != 3))
                    continue;

                if (item.Type == SpecialItem.TYPE_METEOR_SCROLL)
                {
                    amount += 10;
                    continue;
                }

                if (item.Type == SpecialItem.TYPE_METEOR || item.Type == SpecialItem.TYPE_METEORTEAR)
                    amount += 1;
            }
            return amount;
        }

        public uint NormalMeteorAmount(bool bound = true, bool onlyBound = false)
        {
            uint amount = 0;
            foreach (Item item in Items.Values.Where(item => item.Type == SpecialItem.TYPE_METEOR))
            {
                if (!bound && item.Monopoly == 3 && (onlyBound && item.Monopoly != 3))
                    continue;

                if (item.Type == SpecialItem.TYPE_METEOR)
                {
                    amount += 1;
                }
            }
            return amount;
        }

        public uint MeteorTearAmount(bool bound = true, bool onlyBound = false)
        {
            uint amount = 0;
            foreach (Item item in Items.Values.Where(item => item.Type == SpecialItem.TYPE_METEORTEAR))
            {
                if (!bound && item.Monopoly == 3 && (onlyBound && item.Monopoly != 3))
                    continue;

                if (item.Type == SpecialItem.TYPE_METEORTEAR)
                {
                    amount += 1;
                }
            }
            return amount;
        }

        public uint MeteorScrollAmount(bool bound = true, bool onlyBound = false)
        {
            uint amount = 0;
            foreach (Item item in Items.Values.Where(item => item.Type == SpecialItem.TYPE_METEOR_SCROLL))
            {
                if (!bound && item.Monopoly == 3 && (onlyBound && item.Monopoly != 3))
                    continue;

                if (item.Type == SpecialItem.TYPE_METEOR_SCROLL)
                {
                    amount += 1;
                }
            }
            return amount;
        }

        /// <summary>
        /// This method will get meteors from user inventory. If the user has MetScrolls, the
        /// server will target them to be taken first. Of course, if the user requested less
        /// than 10 meteors, the server wont look for them.
        /// </summary>
        public bool ReduceMeteors(uint amount, bool bound = true, bool onlyBound = false)
        {
            if (MeteorAmount(bound, onlyBound) < amount)
                return false;

            uint startAmount = amount;
            uint metScrolls = 0;
            uint meteor = 0;
            uint restMeteor = 0;
            uint meteortear = 0;

            uint remainingAmount = amount;

            // just in case...
            if (amount == 10)
                metScrolls = 1;
            if (amount > 10)
                metScrolls = (amount / 10) + 1;

            uint _amount = 0;

            // Can get bound item? Only bound? So monopoly 3, all kinds -1, only normal 0
            int monopoly = bound ? onlyBound ? 3 : -1 : 0;
            meteor = NormalMeteorAmount(bound, onlyBound);
            meteortear = MeteorTearAmount(bound, onlyBound);
            uint msAmount = MeteorScrollAmount(bound, onlyBound);

            // OK, first we'll see if we have enough meteors to be removed :)
            // Here, if we need an high amount of meteors, we will first see if we can get
            // a few ones using mets, and others using MetScrolls
            // We already know that we have enough meteors or meteor scrolls, so we remove
            // that little amount :)
            if (amount > 10)
            {
                uint mets = amount % 10;
                if (mets <= meteor)
                {
                    Remove(SpecialItem.TYPE_METEOR, mets, monopoly);
                    amount -= mets;
                    remainingAmount -= mets;
                }
                else if (mets <= meteor + meteortear)
                {
                    amount -= mets;
                    remainingAmount -= mets;
                    Remove(SpecialItem.TYPE_METEOR, meteor, monopoly);
                    mets -= meteor;
                    Remove(SpecialItem.TYPE_METEORTEAR, mets, monopoly);
                    // Now we should have *0 meteors required. So, we can grab meteor scrolls
                }
            }
            // Now we gotta check, if the remaining amount, is higher than 10...
            // And, if the amount wasn't higher than 10, we still need to grab a small
            // amount of meteors.
            // First we check if the amount is less than 10, if we have enough meteors,
            // and if we don't, so we will check for meteor scrolls. We dont want to open
            // Meteor Scrolls if we don't need to. :)
            if (((amount < 10 && msAmount >= 1) && (meteor < amount)) || msAmount >= metScrolls && metScrolls > 0)
            {
                // The met scrolls amount should be at least 1, this is the min amount to be taken.
                // We will set this as 0, just to make sure.
                if (metScrolls == 0)
                    metScrolls = 1;

                uint requiredAmount = 0;

                if (amount % 10 > 0)
                    requiredAmount = (amount / 10) + 1;
                else
                    requiredAmount = (amount / 10);

                if (metScrolls > requiredAmount)
                    metScrolls = amount / 10;
                // Let's see how many meteors we will have left.
                restMeteor = (metScrolls * 10) - amount;

                // Hummm, we have enough space to open the meteor scroll?
                if (restMeteor > RemainingSpace())
                {
                    m_pOwner.Send(string.Format(ServerString.STR_NOT_ENOUGH_SPACE_METSCROLL, restMeteor));
                    return false;
                }

                if (restMeteor == 10)
                    metScrolls -= 1;

                // Let's remove the met scroll and give back the meteors 
                if (!Remove(SpecialItem.TYPE_METEOR_SCROLL, metScrolls, monopoly))
                    return false;

                // Just in case, i have some issues that the system were taking 1 metscroll
                // and giving 10 meteors back... I will track this soon, it wouldn't even
                // get the meteor scroll if it won't use.
                if (restMeteor == 10)
                    return Create(SpecialItem.TYPE_METEOR_SCROLL);

                // Give back the remaining meteors.
                for (int i = 0; i < restMeteor; i++)
                    Create(SpecialItem.TYPE_METEOR);
                return true;
            }

            if (msAmount <= metScrolls && msAmount > 0)
            {
                if (!Remove(SpecialItem.TYPE_METEOR_SCROLL, msAmount, monopoly))
                    return false;

                for (int i = 0; i < msAmount; i++)
                {
                    _amount += 10;
                    remainingAmount -= 10;
                }
                if (remainingAmount == 0)
                    return true;
            }

            // Not sure if i still need this. But i will leave here.
            // We got all mets we need?
            if (meteor >= remainingAmount && remainingAmount > 0)
                return Remove(SpecialItem.TYPE_METEOR, remainingAmount, monopoly);
            // Lets see how many we have left...
            if (meteor < remainingAmount && meteor > 0)
            {
                if (!Remove(SpecialItem.TYPE_METEOR, meteor, monopoly))
                    return false;
                _amount += meteor;
                remainingAmount -= meteor;
            }

            if (meteortear >= remainingAmount && remainingAmount > 0)
                return Remove(SpecialItem.TYPE_METEORTEAR, remainingAmount, monopoly);
            // sure.. you don't have enough mets, and someone didn't make a check for it
            // before.. lets report :)
            if (_amount > 0)
                ServerKernel.Log.SaveLog(string.Format("Looks like someone forgot to make a check before trying to grab players meteors :("
                              + "User[{0}][{1}] has lost {2} meteors and the function returned false... tsc tsc",
                    m_pOwner.Identity, m_pOwner.Name, _amount), false);
            return _amount >= amount;
        }

        public int RemainingSpace()
        {
            return 40 - Items.Count;
        }

        public uint DragonBallAmount(bool bound = true, bool onlyBound = false)
        {
            uint amount = 0;
            foreach (Item item in Items.Values.Where(item => item.Type == SpecialItem.TYPE_DRAGONBALL || item.Type == SpecialItem.TYPE_DRAGONBALL_SCROLL))
            {
                if ((!bound && item.Monopoly == 3) && (onlyBound && item.Monopoly != 3))
                    continue;

                if (item.Type == SpecialItem.TYPE_DRAGONBALL_SCROLL)
                {
                    amount += 10;
                    continue;
                }

                if (item.Type == SpecialItem.TYPE_DRAGONBALL)
                    amount += 1;
            }
            return amount;
        }

        public uint NormalDragonBallAmount(bool bound = true, bool onlyBound = false)
        {
            uint amount = 0;
            foreach (Item item in Items.Values.Where(item => item.Type == SpecialItem.TYPE_DRAGONBALL))
            {
                if (!bound && item.Monopoly == 3 && (onlyBound && item.Monopoly != 3))
                    continue;

                if (item.Type == SpecialItem.TYPE_DRAGONBALL)
                    amount += 1;
            }
            return amount;
        }

        public uint DragonBallScrollAmount(bool bound = true, bool onlyBound = false)
        {
            uint amount = 0;
            foreach (Item item in Items.Values.Where(item => item.Type == SpecialItem.TYPE_DRAGONBALL_SCROLL))
            {
                if (!bound && item.Monopoly == 3 && (onlyBound && item.Monopoly != 3))
                    continue;

                if (item.Type == SpecialItem.TYPE_DRAGONBALL_SCROLL)
                    amount += 1;
            }
            return amount;
        }

        public bool ReduceDragonBalls(uint amount, bool bound = true, bool onlyBound = false)
        {
            if (DragonBallAmount(bound, onlyBound) < amount)
                return false;

            uint dbScrolls = 0;
            uint dragonBalls = 0;
            uint restDbs = 0;

            uint remainingAmount = amount;

            // just in case...
            if (amount == 10)
                dbScrolls = 1;
            if (amount > 10)
                dbScrolls = (amount / 10) + 1;

            uint _amount = 0;

            // Can get bound item? Only bound? So monopoly 3, all kinds -1, only normal 0
            int monopoly = bound ? onlyBound ? 3 : -1 : 0;
            dragonBalls = NormalDragonBallAmount(bound, onlyBound);
            uint msAmount = DragonBallScrollAmount(bound, onlyBound);

            // OK, first we'll see if we have enough meteors to be removed :)
            // Here, if we need an high amount of meteors, we will first see if we can get
            // a few ones using mets, and others using MetScrolls
            // We already know that we have enough meteors or meteor scrolls, so we remove
            // that little amount :)
            if (amount > 10 && msAmount >= dbScrolls)
            {
                uint dbs = amount % 10;
                if (dbs <= dragonBalls)
                {
                    Remove(SpecialItem.TYPE_DRAGONBALL, dbs, monopoly);
                    amount -= dbs;
                }
            }
            // Now we gotta check, if the remaining amount, is higher than 10...
            // And, if the amount wasn't higher than 10, we still need to grab a small
            // amount of meteors.
            // First we check if the amount is less than 10, if we have enough meteors,
            // and if we don't, so we will check for meteor scrolls. We dont want to open
            // Meteor Scrolls if we don't need to. :)
            if (((amount < 10 && msAmount >= 1) && (dragonBalls < amount)) || msAmount >= dbScrolls && dbScrolls > 0)
            {
                // The met scrolls amount should be at least 1, this is the min amount to be taken.
                // We will set this as 0, just to make sure.
                if (dbScrolls == 0)
                    dbScrolls = 1;

                if (dbScrolls * 10 > amount)
                    dbScrolls = amount / 10;
                // Let's see how many meteors we will have left.
                restDbs = (dbScrolls * 10) - amount;

                // Hummm, we have enough space to open the meteor scroll?
                if (restDbs > RemainingSpace())
                {
                    m_pOwner.Send(string.Format(ServerString.STR_NOT_ENOUGH_SPACE_DBSCROLL, restDbs));
                    return false;
                }

                // Let's remove the met scroll and give back the meteors 
                if (!Remove(SpecialItem.TYPE_DRAGONBALL_SCROLL, dbScrolls, monopoly))
                    return false;

                // Just in case, i have some issues that the system were taking 1 metscroll
                // and giving 10 meteors back... I will track this soon, it wouldn't even
                // get the meteor scroll if it won't use.
                if (restDbs == 10)
                    return Create(SpecialItem.TYPE_DRAGONBALL_SCROLL);

                // Give back the remaining meteors.
                for (int i = 0; i < restDbs; i++)
                    Create(SpecialItem.TYPE_DRAGONBALL);
                return true;
            }

            if (msAmount <= dbScrolls && msAmount > 0)
            {
                if (!Remove(SpecialItem.TYPE_DRAGONBALL_SCROLL, msAmount, monopoly))
                    return false;

                for (int i = 0; i < msAmount; i++)
                {
                    _amount += 10;
                    remainingAmount -= 10;
                }
            }

            // We got all dbs we need?
            if (dragonBalls >= remainingAmount && remainingAmount > 0)
                return Remove(SpecialItem.TYPE_DRAGONBALL, remainingAmount, monopoly);

            ServerKernel.Log.SaveLog(string.Format("Looks like someone forgot to make a check before trying to grab players db :("
                              + "User[{0}][{1}] has lost {2} db and the function returned false... tsc tsc",
                    m_pOwner.Identity, m_pOwner.Name, _amount), false);
            return _amount >= amount;
        }

        public bool ContainsMultiple(uint first, uint last, byte amount, bool bBound = false, bool bOnlyBound = false)
        {
            if (last < first)
            {
                uint temp = last;
                last = first;
                first = temp;
            }
            uint nCount = 0;
            foreach (Item item in Items.Values.Where(x => x.Type >= first && x.Type <= last))
            {
                if (bBound && bOnlyBound && !item.Bound)
                    continue;

                if (item.Bound && !bBound)
                    continue;

                nCount += (ushort)(item.StackAmount > 0 ? item.StackAmount : 1);
                // Since im so noob, let's not waste (more) resources.
                if (nCount >= amount)
                    return true;
            }
            return nCount >= amount;
        }

        /// <summary>
        /// This method will delete items in a range and will check for certain amount. Be sure to use
        /// ContainsMultiple() first, otherwise the player will lose items even if he doesnt have enough.
        /// The first item may be higher than the limit, or there may be a really big item loss.
        /// </summary>
        /// <param name="first">The first id of the check.</param>
        /// <param name="last">The range limit should be higher than the first item.</param>
        /// <param name="amount">The amount of items that will be checked. This wont get "amount" of each item, the amount is the total that will be taken.</param>
        /// <returns>If the system could get all of the required items.</returns>
        public bool DeleteMultiple(uint first, uint last, byte amount, bool bBound = false, bool bOnlyBound = false)
        {
            if (last < first)
            {
                uint temp = last;
                last = first;
                first = temp;
            }
            // '-'
            if (!ContainsMultiple(first, last, amount))
                return false;

            ushort nCount = 0;
            ushort nRest = amount;
            foreach (Item item in Items.Values.Where(x => x.Type >= first && x.Type <= last).OrderBy(x => x.Type))
            {
                // Double checking
                if (item.Type < first && item.Type > last)
                    return false;

                if (bBound && bOnlyBound && !item.Bound)
                    continue;

                if (item.Bound && !bBound)
                    continue;

                nCount += (ushort)(item.StackAmount > 0 ? item.StackAmount : 1);

                if (item.StackAmount >= amount)
                    return Remove(item.Type, amount);

                if (Contains(item.Type, amount))
                    return Remove(item.Type, amount);

                nCount += item.StackAmount;
                nRest -= item.StackAmount;

                m_pOwner.Send(item.UsagePacket(ItemAction.REMOVE));
                Item trash;
                Items.TryRemove(item.Identity, out trash);
                item.Delete();
            }
            return nCount >= amount;
        }

        public bool IsFull
        {
            get { return Items.Count >= 40; }
        }

        public int RandDropItem(int nMapType, int nChance)
        {
            if (m_pOwner == null)
                return 0;
            int nDropNum = 0;
            foreach (var item in Items.Values)
            {
                if (Calculations.ChanceCalc(nChance))
                {
                    if (item.IsNeverDropWhenDead())
                        continue;

                    switch (nMapType)
                    {
                        case 0: // NONE
                            break;
                        case 1: // PK
                        case 2: // SYN
                            if (!item.IsArmor())
                                continue;
                            break;
                        case 3: // PRISON
                            break;
                    }
                    var pos = new Point(m_pOwner.MapX, m_pOwner.MapY);
                    if (m_pOwner.Map.FindDropItemCell(5, ref pos))
                    {
                        if (!m_pOwner.Inventory.Remove(item.Identity, ItemRemovalType.DROP_ITEM))
                            continue;

                        item.PlayerIdentity = 0;
                        item.OwnerIdentity = m_pOwner.Identity;

                        var pMapItem = new MapItem();
                        if (pMapItem.Create((uint)m_pOwner.Map.FloorItem, m_pOwner.Map, pos, item, m_pOwner.Identity))
                        {
                            m_pOwner.Map.AddItem(pMapItem);
                            ServerKernel.Log.GmLog("drop_item", string.Format("{0}({1}) drop item:[id={2}, type={3}], dur={4}, max_dur={5}", m_pOwner.Name, m_pOwner.Identity, item.Identity, item.Type, item.Durability, item.MaximumDurability));
                        }

                        nDropNum++;
                    }
                }
            }
            return nDropNum;
        }

        public int RandDropItem(int nDropNum)
        {
            if (m_pOwner == null)
                return 0;

            var temp = new List<Item>();
            foreach (var item in Items.Values)
            {
                if (item.IsNeverDropWhenDead())
                    continue;
                temp.Add(item);
            }
            int nTotalItemCount = temp.Count;
            if (nTotalItemCount == 0)
                return 0;
            int nRealDropNum = 0;
            for (int i = 0; i < Math.Min(nDropNum, nTotalItemCount); i++)
            {
                int nIdx = Calculations.Random.Next(nTotalItemCount);
                Item item;
                try { item = temp[nIdx]; }
                catch { continue; }
                var pos = new Point(m_pOwner.MapX, m_pOwner.MapY);
                if (m_pOwner.Map.FindDropItemCell(5, ref pos))
                {
                    if (!m_pOwner.Inventory.Remove(item.Identity, ItemRemovalType.DROP_ITEM))
                        continue;

                    item.PlayerIdentity = 0;
                    item.OwnerIdentity = m_pOwner.Identity;

                    var pMapItem = new MapItem();
                    if (pMapItem.Create((uint)m_pOwner.Map.FloorItem, m_pOwner.Map, pos, item, m_pOwner.Identity))
                    {
                        m_pOwner.Map.AddItem(pMapItem);
                        ServerKernel.Log.GmLog("drop_item", string.Format("{0}({1}) drop item:[id={2}, type={3}], dur={4}, max_dur={5}", m_pOwner.Name, m_pOwner.Identity, item.Identity, item.Type, item.Durability, item.MaximumDurability));
                    }
                    /*if (!owner.Inventory.Remove(item.Identity, ItemRemovalType.DELETE))
                        continue;
                    var gItem = new GroundItem(owner, item);
                    gItem.SpawnItem();*/
                    nRealDropNum++;
                }
            }
            return nRealDropNum;
        }
    }
}