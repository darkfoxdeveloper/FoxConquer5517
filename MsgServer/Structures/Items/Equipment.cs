// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - Equipment.cs
// Last Edit: 2017/02/06 11:17
// Created: 2017/02/04 14:35

using System;
using System.Collections.Concurrent;
using System.Linq;
using Core.Common.Enums;
using DB.Entities;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Items
{
    public sealed class Equipment
    {
        private Character m_pOwner;
        public ConcurrentDictionary<ItemPosition, Item> Items;

        public Equipment(Character owner)
        {
            m_pOwner = owner;
            Items = new ConcurrentDictionary<ItemPosition, Item>();
        }

        /// <summary>
        /// Attach the item to the user interface getting it by the item identity.
        /// </summary>
        /// <param name="dwId">The id of the item on the inventory.</param>
        /// <param name="pPos"></param>
        /// <returns>If the item has been added or not.</returns>
        public bool Add(uint dwId, byte pPos)
        {
            Item item;
            if (!m_pOwner.Inventory.Items.TryGetValue(dwId, out item))
                return false;
            item.Position = (ItemPosition) pPos;
            return Add(item);
        }

        public bool Add(Item item, bool isLogin = false)
        {
            ItemPosition pos = Calculations.GetItemPosition(item.Type);

            #region Sanity Checks

            if (!isLogin)
            {
                // Level check
                if (item.Itemtype.ReqLevel > m_pOwner.Level)
                    return false;

                // Profession check
                if (m_pOwner.Metempsychosis > 0
                    && m_pOwner.Level >= 70
                    && item.Itemtype.ReqLevel <= 70)
                {
                }
                else
                {
                    if (item.Itemtype.ReqProfession > 0)
                    {
                        // item
                        int iProfession = item.Itemtype.ReqProfession/10;
                        int iProfessionLevel = item.Itemtype.ReqProfession%10;

                        if (iProfession == 19)
                            iProfession = 10;

                        // user
                        int uProfession = m_pOwner.Profession/10;
                        int uProfessionLevel = m_pOwner.Profession%10;

                        if (uProfession > 10 && iProfession == 10)
                            uProfession = 10;

                        if (iProfession == uProfession)
                        {
                            if (iProfessionLevel > uProfessionLevel)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    // Attribute check
                    if (item.Itemtype.ReqForce != 0
                        && item.Itemtype.ReqForce > m_pOwner.Strength)
                        return false;
                    if (item.Itemtype.ReqSpeed != 0
                        && item.Itemtype.ReqSpeed > m_pOwner.Agility)
                        return false;
                    if (item.Itemtype.ReqHealth != 0
                        && item.Itemtype.ReqHealth > m_pOwner.Vitality)
                        return false;
                    if (item.Itemtype.ReqSoul != 0
                        && item.Itemtype.ReqSoul > m_pOwner.Spirit)
                        return false;

                    ushort type = (ushort) (item.Type/1000);
                    if (pos == ItemPosition.RIGHT_HAND && !m_pOwner.WeaponSkill.Skills.ContainsKey(type) &&
                        item.Itemtype.ReqWeaponskill > 0)
                        return false;

                    if (m_pOwner.WeaponSkill.Skills.ContainsKey(type))
                        if (pos == ItemPosition.RIGHT_HAND &&
                            m_pOwner.WeaponSkill.Skills[type].Level < item.Itemtype.ReqWeaponskill)
                            return false;

                    if (type == 421
                        && item.Position < (ItemPosition) 20
                        && Items.ContainsKey(ItemPosition.LEFT_HAND))
                    {
                        m_pOwner.Send("Please remove the equipment in your left hand first.");
                        return false;
                    }

                    if (type == 900
                        && Items.ContainsKey(ItemPosition.RIGHT_HAND)
                        && Items[ItemPosition.RIGHT_HAND].GetItemSubtype() == 421)
                    {
                        m_pOwner.Send("You cannot equip a shield while wearing a backsword.");
                        return false;
                    }
                }

                // Gender check
                if (item.Itemtype.ReqSex != 0)
                    if (item.Itemtype.ReqSex != m_pOwner.Gender)
                        return false;
            }

            ItemSort itemSort = item.GetSort();

            if ((itemSort == ItemSort.ITEMSORT_USABLE
                 || itemSort == ItemSort.ITEMSORT_USABLE2
                 || itemSort == ItemSort.ITEMSORT_USABLE3)
                && pos == ItemPosition.INVENTORY && item.Type/1000 != 1050)
            {
                // Stores the item temporary data.
                m_pOwner.LastItemResource = 0;
                m_pOwner.LastUsedItem = item.Identity;
                m_pOwner.LastUsedItemTime = (uint) UnixTimestamp.Timestamp();
                m_pOwner.LastUsedItemtype = item.Type;

                if ((item.Type >= 1000000 && item.Type < 1050000) || item.Type == 725065 || item.Type == 725066)
                    // potion
                {
                    if (!m_pOwner.IsAlive)
                    {
                        m_pOwner.Send(ServerString.STR_NOT_ALIVE);
                        return false;
                    }

                    if (item.Itemtype.Life > 0 && m_pOwner.QueryStatus(FlagInt.POISON_STAR) != null)
                    {
                        //m_pOwner.Send(ServerString.STR_CANT_HEAL_POISON_STAR);
                        return false;
                    }

                    if (item.Itemtype.Life > 0 && m_pOwner.Life >= m_pOwner.MaxLife)
                    {
                        //m_pOwner.Send(ServerString.STR_YOUR_LIFE_FULL);
                        return false; // return false so it wont spend processing recalculating the user stts
                    }

                    if (item.Itemtype.Mana > 0 && m_pOwner.Mana >= m_pOwner.MaxMana)
                    {
                        //m_pOwner.Send(ServerString.STR_YOUR_MANA_FULL);
                        return false; // return false so it wont spend processing recalculating the user stts
                    }

                    if (m_pOwner.IsGm)
                    {
                        m_pOwner.FillLife();
                        m_pOwner.FillMana();
                        return false;
                    }

                    if (m_pOwner.Inventory.Remove(item.Type, 1))
                    {
                        m_pOwner.AddAttribute(ClientUpdateType.HITPOINTS, item.Itemtype.Life, true);
                        m_pOwner.AddAttribute(ClientUpdateType.MANA, item.Itemtype.Mana, true);
                    }
                    return false;
                }

                if (item.Type == SpecialItem.MEMORY_AGATE)
                {
                    item.SendCarry();
                    return false;
                }

                if (item.Type == 723726)
                {
                    if (m_pOwner.Inventory.Remove(item.Type, 1))
                    {
                        m_pOwner.FillLife();
                        m_pOwner.FillMana();
                    }
                    return false;
                }

                if (item.Type == 723790 && m_pOwner.Inventory.Remove(723790))
                {
                    m_pOwner.AddAttribute(ClientUpdateType.HITPOINTS, 500, true);
                    return false;
                }

                if (item.Type == SpecialItem.TYPE_EXP_BALL || item.Type == 723834)
                {
                    if (m_pOwner.ExpBallAmount >= 10)
                    {
                        int nDayOfYear = UnixTimestamp.ToDateTime(m_pOwner.LastUsedExpBall).DayOfYear;
                        int nDayOfYear2 = DateTime.Now.DayOfYear;
                        if (nDayOfYear == nDayOfYear2)
                            return false;
                        m_pOwner.ExpBallAmount = 0;
                    }

                    if (m_pOwner.Inventory.Remove(item.Identity))
                    {
                        m_pOwner.ExpBallAmount = (byte) (m_pOwner.ExpBallAmount + 1);
                        m_pOwner.LastUsedExpBall = (uint) UnixTimestamp.Timestamp();
                        m_pOwner.Save();
                        m_pOwner.AwardExperience(ServerKernel.GetExpBallExperience(m_pOwner.Level), true, true);
                    }
                    return false;
                }

                if ((item.Type >= 1060020 && item.Type <= 1060039) || item.Type == 1060102)
                {
                    if (m_pOwner.Map.IsChgMapDisable())
                        return false;
                    m_pOwner.Inventory.Remove(item.Identity);
                }
                else if (item.Type/1000 == 1060)
                    m_pOwner.Inventory.Remove(item.Identity);

                m_pOwner.TaskItem = item;
                m_pOwner.GameAction.ProcessAction(item.Itemtype.IdAction, m_pOwner, null, item, null);
                return false;
            }
            if (pos == ItemPosition.INVENTORY)
                return false;

            if (pos == ItemPosition.LEFT_HAND
                && itemSort == ItemSort.ITEMSORT_WEAPON_SHIELD)
            {
                if (Items.ContainsKey(ItemPosition.RIGHT_HAND)
                    && Items[ItemPosition.RIGHT_HAND].GetSort() == ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND)
                {
                    if (!m_pOwner.Magics.CheckType(10311))
                        return false;
                }
                if (!Items.ContainsKey(ItemPosition.RIGHT_HAND))
                    return false;
            }

            #endregion

            if (item.IsArrowSort())
                item.Position = ItemPosition.LEFT_HAND;

            switch (item.Position)
            {
                case ItemPosition.RIGHT_HAND:
                    if (pos != ItemPosition.RIGHT_HAND
                        || (itemSort != ItemSort.ITEMSORT_WEAPON_SINGLE_HAND
                            && itemSort != ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND
                            && itemSort != ItemSort.ITEMSORT_WEAPON_SINGLE_HAND2))
                    {
                        item.Position = 0;
                        return false;
                    }

                    if ((itemSort == ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND
                         && Items.ContainsKey(ItemPosition.LEFT_HAND)))
                    {
                        Remove(ItemPosition.LEFT_HAND);
                    }

                    if (Items.ContainsKey(ItemPosition.RIGHT_HAND) && item.Position == ItemPosition.RIGHT_HAND)
                        Remove(ItemPosition.RIGHT_HAND, ItemRemoveMethod.REMOVE_TO_INVENTORY);
                    break;
                case ItemPosition.LEFT_HAND:
                    if (itemSort != ItemSort.ITEMSORT_WEAPON_SINGLE_HAND
                        && itemSort != ItemSort.ITEMSORT_WEAPON_SHIELD
                        && itemSort != ItemSort.ITEMSORT_WEAPON_SINGLE_HAND2
                        && item.Type/1000 != 1050)
                    {
                        item.Position = 0;
                        return false;
                    }

                    if (m_pOwner.Profession >= 100)
                    {
                        if (Items.ContainsKey(ItemPosition.LEFT_HAND) && Items[ItemPosition.LEFT_HAND] == item)
                            Remove(ItemPosition.LEFT_HAND, ItemRemoveMethod.REMOVE_TO_INVENTORY);

                        item.Position = 0;
                        return false;
                    }

                    if (item.IsArrowSort()
                        && (!Items.ContainsKey(ItemPosition.RIGHT_HAND)
                            || !Items[ItemPosition.RIGHT_HAND].IsBow()))
                    {
                        item.Position = 0;
                        return false;
                    }

                    if (Items.ContainsKey(ItemPosition.LEFT_HAND))
                    {
                        Remove(ItemPosition.LEFT_HAND, ItemRemoveMethod.REMOVE_TO_INVENTORY);
                    }
                    break;
                case ItemPosition.ACCESSORY_R:
                {
                    if ((itemSort != ItemSort.ITEMSORT_ACCESSORY)
                        || item.Type/10000 == 38)
                    {
                        item.Position = 0;
                        return false;
                    }

                    switch (item.Type/10000)
                    {
                        case 38:
                            item.Position = 0;
                            return false;
                    }

                    if (Items.ContainsKey(ItemPosition.ACCESSORY_R))
                        Remove(ItemPosition.ACCESSORY_R);
                    break;
                }
                case ItemPosition.ACCESSORY_L:
                {
                    if (itemSort != ItemSort.ITEMSORT_ACCESSORY || item.Type/10000 == 37)
                    {
                        item.Position = 0;
                        return false;
                    }

                    switch (item.Type/10000)
                    {
                        case 37:
                            item.Position = 0;
                            return false;
                    }

                    if (Items.ContainsKey(ItemPosition.ACCESSORY_L))
                        Remove(ItemPosition.ACCESSORY_L);
                    break;
                }
                default:
                    if (pos == item.Position
                        && Items.ContainsKey(item.Position))
                    {
                        Remove(item.Position);
                    }
                    else if (pos != item.Position)
                    {
                        if (item.Position < ItemPosition.ALT_HEAD && pos != item.Position)
                        {
                            item.Position = 0;
                            return false;
                        }
                        if (item.Position >= ItemPosition.ALT_HEAD && item.Position <= ItemPosition.ALT_STEED)
                        {
                            switch (item.Position)
                            {
                                case ItemPosition.ALT_HEAD:
                                {
                                    if (pos != ItemPosition.HEADWEAR)
                                        return false;
                                    break;
                                }
                                case ItemPosition.ALT_NECKLACE:
                                {
                                    if (pos != ItemPosition.NECKLACE)
                                        return false;
                                    break;
                                }
                                case ItemPosition.ALT_RING:
                                {
                                    if (pos != ItemPosition.RING)
                                        return false;
                                    break;
                                }
                                case ItemPosition.ALT_WEAPON_R:
                                {
                                    if (pos != ItemPosition.RIGHT_HAND)
                                        return false;
                                    break;
                                }
                                case ItemPosition.ALT_WEAPON_L:
                                {
                                    if (pos != ItemPosition.LEFT_HAND && pos != ItemPosition.RIGHT_HAND)
                                        return false;
                                    break;
                                }
                                case ItemPosition.ALT_ARMOR:
                                {
                                    if (pos != ItemPosition.ARMOR)
                                        return false;
                                    break;
                                }
                                case ItemPosition.ALT_BOOTS:
                                {
                                    if (pos != ItemPosition.BOOTS)
                                        return false;
                                    break;
                                }
                                case ItemPosition.ALT_BOTTLE:
                                {
                                    if (pos != ItemPosition.BOTTLE)
                                        return false;
                                    break;
                                }
                                case ItemPosition.ALT_GARMENT:
                                {
                                    if (pos != ItemPosition.GARMENT)
                                        return false;
                                    break;
                                }
                                default:
                                    item.Position = 0;
                                    return false;
                            }

                            if (Items.ContainsKey(item.Position) && Items.Count >= 40)
                            {
                                item.Position = 0;
                                return false;
                            }
                            if (Items.ContainsKey(item.Position))
                            {
                                Remove(item.Position);
                            }
                        }
                        else
                        {
                            item.Position = 0;
                            return false;
                        }
                    }
                    break;
            }

            var itemEquip = new MsgItem
            {
                Identity = item.Identity,
                Action = ItemAction.EQUIP,
                Param1 = (uint) item.Position
            };

            // We build the item information packet
            MsgItemInformation itemInfo = item.InformationPacket(!isLogin);

            // If we are logging in, we set this as default, because the item hasnt been
            // created yet, otherwise, we send this as an update.
            if (isLogin)
            {
                itemInfo.ItemMode = ItemMode.DEFAULT;
                m_pOwner.Send(itemInfo);
            }
            else
            {
                //itemInfo.ItemMode = ItemMode.Update;
                if (!m_pOwner.Inventory.Contains(item.Identity))
                    return false;
                Item trash;
                m_pOwner.Inventory.Items.TryRemove(item.Identity, out trash);
                item.Save();
                m_pOwner.RecalculateAttributes();
            }

            m_pOwner.Send(itemEquip);
            Items.TryAdd(item.Position, item);
            SendEquipedItems();
            item.SendPurification();
            item.TryUnlockItem();
            item.SendItemLockTime();

            switch (item.Position)
            {
                case ItemPosition.HEADWEAR:
                    m_pOwner.Helmet = item.Type;
                    m_pOwner.HelmetColor = (ushort) item.Color;
                    break;
                case ItemPosition.ARMOR:
                    m_pOwner.Armor = item.Type;
                    m_pOwner.ArmorColor = (ushort) item.Color;
                    break;
                case ItemPosition.LEFT_HAND:
                    m_pOwner.LeftHand = item.Type;
                    m_pOwner.ShieldColor = (ushort) item.Color;
                    break;
                case ItemPosition.RIGHT_HAND:
                    m_pOwner.RightHand = item.Type;
                    break;
                case ItemPosition.GARMENT:
                    m_pOwner.Garment = item.Type;
                    break;
                case ItemPosition.ACCESSORY_R:
                    m_pOwner.RightAccessory = item.Type;
                    break;
                case ItemPosition.ACCESSORY_L:
                    m_pOwner.LeftAccessory = item.Type;
                    break;
                case ItemPosition.STEED:
                    m_pOwner.MountType = item.Type;
                    m_pOwner.MountColor = item.SocketProgress;
                    m_pOwner.MountPlus = item.Plus;
                    break;
                case ItemPosition.STEED_ARMOR:
                    m_pOwner.MountArmor = item.Type;
                    break;
            }

            return true;
        }

        public bool TryGetEquipment(uint identity, out Item value)
        {
            try
            {
                foreach (var item in Items.Values.Where(x => x.Identity == identity))
                {
                    value = item;
                    return true;
                }
                value = null;
                return false;
            }
            catch (Exception ex)
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Remove the item from the equipment checking by the item id.
        /// </summary>
        /// <param name="identity">The Unique ID of the item.</param>
        /// <returns>If the item has been removed successfully or not.</returns>
        public bool Remove(uint identity)
        {
            return
                (from item in Items.Values where item.Identity == identity select Remove(item.Position)).FirstOrDefault();
            //foreach (Item item in Items.Values)
            //{
            //    if(item.Identity == identity)
            //        return Remove(item.Position);
            //}
            //return false;
        }

        /// <summary>
        /// Remove the equipment from the status and move it to the inventory by default.
        /// </summary>
        /// <param name="position">The position you want to unequip.</param>
        /// <returns>If the item has been unequiped and sent to the inventory.</returns>
        public bool Remove(ItemPosition position)
        {
            return Remove(position, ItemRemoveMethod.REMOVE_TO_INVENTORY);
        }

        /// <summary>
        /// Remove the equipment from the status.
        /// </summary>
        /// <param name="position">The position you want to break free ♫♪</param>
        /// <param name="method">The item will be deleted?</param>
        /// <returns>If the item has been succesfully moved.</returns>
        public bool Remove(ItemPosition position, ItemRemoveMethod method)
        {
            // Check if we have this item really equiped.
            if (!Items.ContainsKey(position))
                return false;

            // Our inventory is full?
            if (method == ItemRemoveMethod.REMOVE_TO_INVENTORY
                && m_pOwner.Inventory.RemainingSpace() < 1)
            {
                return false;
            }

            // Get the item and then delete or update it on the database.
            Item updateItem;
            if (!Items.TryGetValue(position, out updateItem))
                return false;

            if (updateItem.Position == ItemPosition.RIGHT_HAND && Items.ContainsKey(ItemPosition.LEFT_HAND))
            {
                Remove(5);
            }

            Items[position].Position = 0;

            // Update the spawn packet
            switch (position)
            {
                case ItemPosition.HEADWEAR:
                {
                    // Headwear
                    m_pOwner.Helmet = 0;
                    m_pOwner.HelmetColor = 0;
                    break;
                }
                case ItemPosition.ARMOR:
                {
                    // Armor
                    m_pOwner.Armor = 0;
                    m_pOwner.ArmorColor = 0;
                    break;
                }
                case ItemPosition.RIGHT_HAND:
                {
                    // Right Hand
                    m_pOwner.RightHand = 0;
                    break;
                }
                case ItemPosition.LEFT_HAND:
                {
                    // Left hand
                    m_pOwner.LeftHand = 0;
                    m_pOwner.ShieldColor = 0;
                    break;
                }
                case ItemPosition.GARMENT:
                {
                    // Garment
                    m_pOwner.Garment = 0;
                    break;
                }
                case ItemPosition.STEED:
                {
                    // Mount
                    m_pOwner.MountColor = 0;
                    m_pOwner.MountPlus = 0;
                    m_pOwner.MountType = 0;
                    if (m_pOwner.QueryStatus(FlagInt.RIDING) != null)
                        m_pOwner.DetachStatus(FlagInt.RIDING);
                    break;
                }
                case ItemPosition.ACCESSORY_R:
                {
                    // Right Hand Accessory
                    m_pOwner.RightAccessory = 0;
                    break;
                }
                case ItemPosition.ACCESSORY_L:
                {
                    // Left Hand Accessory
                    m_pOwner.LeftAccessory = 0;
                    break;
                }
                case ItemPosition.STEED_ARMOR:
                {
                    // Mount Armor
                    m_pOwner.MountArmor = 0;
                    break;
                }
            }

            // Send the packet to remove the item from the user.
            MsgItem removeItem = updateItem.UsagePacket(ItemAction.UNEQUIP);
            removeItem.Param1 = (uint) position;
            removeItem.Identity = updateItem.Identity;
            m_pOwner.Send(removeItem);

            Item trash;
            Items.TryRemove(position, out trash);

            if (method == ItemRemoveMethod.REMOVE_TO_INVENTORY)
            {
                // Update the item to the inventory.
                m_pOwner.Send(trash.InformationPacket(true));
                m_pOwner.Inventory.Add(trash);
                trash.Save();
            }
            else
                updateItem.Delete();

            trash = null;
            updateItem = null;

            SendEquipedItems();
            return true;
        }

        public bool RepairAll()
        {
            foreach (Item item in Items.Values)
                item.RepairItem();
            return true;
        }

        /// <summary>
        /// Creates a item for the user and already equips this.
        /// </summary>
        /// <param name="item">The item that will be created.</param>
        /// <returns>If the item has been created successfully.</returns>
        public bool Create(DbItem item)
        {
            // If it's not going to be equiped, then we're not adding it.
            if (item.Position <= 0 || item.Position > 12)
                return false;
            // Insert item into the database.
            var newItem = new Item(m_pOwner, item);
            newItem.Save();
            // Finally, add the item to the equipments.
            // Reminder, this is used only for starter characters only, since this will
            // Insert to the database and also add to the character.
            Add(newItem);
            return true;
        }

        /// <summary>
        /// Send all equiped items to the character, otherwise the user wouldn't be able
        /// to see his equiped gears. Also refresh the spawn packet for the observers.
        /// </summary>
        public void SendEquipedItems()
        {
            #region Let's restart the spawn packet

            m_pOwner.Helmet = 0;
            m_pOwner.HelmetColor = 0;
            m_pOwner.Armor = 0;
            m_pOwner.ArmorColor = 0;
            m_pOwner.RightHand = 0;
            m_pOwner.ArmorArtifact = 0;
            m_pOwner.HelmetArtifact = 0;
            m_pOwner.RightHandArtifact = 0;
            m_pOwner.LeftHandArtifact = 0;
            m_pOwner.LeftHand = 0;
            m_pOwner.ShieldColor = 0;
            m_pOwner.Garment = 0;
            m_pOwner.MountColor = 0;
            m_pOwner.MountPlus = 0;
            m_pOwner.MountType = 0;
            m_pOwner.RightAccessory = 0;
            m_pOwner.LeftAccessory = 0;
            m_pOwner.MountArmor = 0;

            #endregion

            var pMsg = new MsgItem();
            pMsg.Identity = m_pOwner.Identity;
            pMsg.Action = ItemAction.DISPLAY_GEARS;

            if (Items.ContainsKey(ItemPosition.HEADWEAR))
            {
                // Headgear
                pMsg.Headgear = Items[ItemPosition.HEADWEAR].Identity;
                m_pOwner.Helmet = Items[ItemPosition.HEADWEAR].Type;
                m_pOwner.HelmetColor = (byte) Items[ItemPosition.HEADWEAR].Color;
                if (Items[ItemPosition.HEADWEAR].Artifact.Avaiable)
                    m_pOwner.HelmetArtifact = Items[ItemPosition.HEADWEAR].ArtifactType;
            }
            if (Items.ContainsKey(ItemPosition.NECKLACE)) pMsg.Necklace = Items[ItemPosition.NECKLACE].Identity;
            if (Items.ContainsKey(ItemPosition.ARMOR))
            {
                // Armor
                pMsg.Armor = Items[ItemPosition.ARMOR].Identity;
                m_pOwner.Armor = Items[ItemPosition.ARMOR].Type;
                m_pOwner.ArmorColor = (byte) Items[ItemPosition.ARMOR].Color;
                if (Items[ItemPosition.ARMOR].Artifact.Avaiable)
                    m_pOwner.ArmorArtifact = Items[ItemPosition.ARMOR].ArtifactType;
            }
            if (Items.ContainsKey(ItemPosition.RIGHT_HAND))
            {
                // Right Hand
                pMsg.RightHand = Items[ItemPosition.RIGHT_HAND].Identity;
                m_pOwner.RightHand = Items[ItemPosition.RIGHT_HAND].Type;
                if (Items[ItemPosition.RIGHT_HAND].Artifact.Avaiable)
                    m_pOwner.RightHandArtifact = Items[ItemPosition.RIGHT_HAND].ArtifactType;
            }
            if (Items.ContainsKey(ItemPosition.LEFT_HAND))
            {
                // Left hand
                pMsg.LeftHand = Items[ItemPosition.LEFT_HAND].Identity;
                m_pOwner.LeftHand = Items[ItemPosition.LEFT_HAND].Type;
                if (Items[ItemPosition.LEFT_HAND].Artifact.Avaiable)
                    m_pOwner.LeftHandArtifact = Items[ItemPosition.LEFT_HAND].ArtifactType;
                m_pOwner.ShieldColor = (byte) Items[ItemPosition.LEFT_HAND].Color;
            }
            if (Items.ContainsKey(ItemPosition.RING))
            {
                pMsg.Ring = Items[ItemPosition.RING].Identity;
            }
            if (Items.ContainsKey(ItemPosition.BOTTLE))
            {
                pMsg.Talisman = Items[ItemPosition.BOTTLE].Identity;
            }
            if (Items.ContainsKey(ItemPosition.BOOTS))
            {
                pMsg.Boots = Items[ItemPosition.BOOTS].Identity;
            }
            if (Items.ContainsKey(ItemPosition.GARMENT))
            {
                // Garment
                pMsg.Garment = Items[ItemPosition.GARMENT].Identity;
                m_pOwner.Garment = Items[ItemPosition.GARMENT].Type;
            }
            if (Items.ContainsKey(ItemPosition.STEED))
            {
                // Steed
                m_pOwner.MountType = Items[ItemPosition.STEED].Type;
                m_pOwner.MountPlus = Items[ItemPosition.STEED].Plus;
                m_pOwner.MountColor = Items[ItemPosition.STEED].SocketProgress;
            }
            if (Items.ContainsKey(ItemPosition.ACCESSORY_R))
            {
                // Right Hand Accessory
                pMsg.RightAccessory = Items[ItemPosition.ACCESSORY_R].Identity;
                m_pOwner.RightAccessory = Items[ItemPosition.ACCESSORY_R].Type;
            }
            if (Items.ContainsKey(ItemPosition.ACCESSORY_L))
            {
                // Left hand Accessory
                pMsg.LeftAccessory = Items[ItemPosition.ACCESSORY_L].Identity;
                m_pOwner.LeftAccessory = Items[ItemPosition.ACCESSORY_L].Type;
            }
            if (Items.ContainsKey(ItemPosition.STEED_ARMOR))
            {
                // Mount Armor
                pMsg.MountArmor = Items[ItemPosition.STEED_ARMOR].Identity;
                m_pOwner.MountArmor = Items[ItemPosition.STEED_ARMOR].Type;
            }
            if (Items.ContainsKey(ItemPosition.CROP))
            {
                pMsg.Crop = Items[ItemPosition.CROP].Identity;
            }

            m_pOwner.Send(pMsg);
            m_pOwner.Screen.RefreshSpawnForObservers();
        }

        public bool GetByIdentity(uint Identity, out Item item)
        {
            return (item = Items.Values.FirstOrDefault(x => x.Identity == Identity)) != null;
        }

        public void AddEquipmentDurability(ItemPosition position, int value)
        {
            if (value >= 0)
                return;

            if (Items.ContainsKey(position))
            {
                Item item = Items[position];
                if (!item.IsEquipment() || !item.IsWeapon())
                    return;
                int nOldDur = item.Durability;
                int nDurability = Math.Max(0, item.Durability + value);
                item.Durability = (ushort) nDurability;

                if (item.Durability < 100 && item.Durability%10 == 0)
                    m_pOwner.Send("Your item is damaged, please repair.");
                else if (item.Durability < 200 && item.Durability%20 == 0)
                    m_pOwner.Send("Your equipment durability is too low.");

                if (nOldDur/100 != nDurability/100 || nDurability <= 0)
                    m_pOwner.Send(item.InformationPacket(true));
            }
        }

        public enum ItemRemoveMethod
        {
            REMOVE_TO_INVENTORY,
            DELETE
        }

        public bool TryGetValue(uint idItem, out Item pItem)
        {
            try
            {
                pItem = Items.Values.FirstOrDefault(x => x.Identity == idItem);
                return true;
            }
            catch
            {
                pItem = null;
                return false;
            }
        }
    }
}