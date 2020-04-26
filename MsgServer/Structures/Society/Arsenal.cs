// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Arsenal.cs
// Last Edit: 2016/11/27 21:45
// Created: 2016/11/25 00:09

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DB.Entities;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Society
{
    public sealed class Arsenal
    {
        public Syndicate Owner;
        public ConcurrentDictionary<TotemPoleType, TotemPole> Poles;
        public byte BattlePower;

        public Arsenal(Syndicate owner)
        {
            Owner = owner;
            Poles = new ConcurrentDictionary<TotemPoleType, TotemPole>();
        }

        /// <summary>
        /// Used to add items to the totem pole on the server startup.
        /// </summary>
        /// <param name="item">The item that will be inserted into the arsenal.</param>
        /// <param name="totem">The totem object.</param>
        /// <returns>If the item has been inscribed successfully.</returns>
        public bool AddItem(Item item, Totem totem)
        {
            TotemPoleType type = GetArsenalType(item);
            if (type == TotemPoleType.TOTEM_NONE)
                return false;

            if (!Poles.ContainsKey(type))
                return false; // doesn't contain the required type

            if (Poles[type].Locked)
                return false;

            if (item.GetQuality() < 8 || item.IsArrowSort())
                return false;

            if (Poles[type].Items.ContainsKey(item.Identity))
                return false; // that item is already inscribed?

            if (!Poles[type].Items.TryAdd(item.Identity, totem))
                return false;

            item.Inscribed = true;
            item.Save();
            return true;
        }

        public bool InscribeItem(Item pItem, Character pUser)
        {
            TotemPoleType pType = GetArsenalType(pItem);

            if (pType == TotemPoleType.TOTEM_NONE) // invalid item
                return false;

            if (!Poles.ContainsKey(pType)) // arsenal probably closed
                return false;

            if (!pUser.Inventory.Items.ContainsKey(pItem.Identity)) // item should be in the user inventory
                return false;

            if (pItem.GetQuality() < 8 || pItem.IsArrowSort())
                return false;

            if (Poles[pType].Items.ContainsKey(pItem.Identity)) // already inscribed
                return false;

            int nTotal = Poles[pType].Items.Values.Count(x => x.PlayerIdentity == pUser.Identity);
            if (nTotal >= MaxPerType(pUser)) // inscribed max items
                return false;

            var totem = new Totem(pItem, pUser);
            if (!Poles[pType].Items.TryAdd(pItem.Identity, totem))
                return false;

            uint dwOldBp = BattlePower;

            pItem.Inscribed = true;
            pUser.Send(pItem.InformationPacket(true));
            totem.Save();
            UpdatePoles();
            pUser.SyndicateMember.ArsenalDonation += totem.Donation();

            if (dwOldBp != BattlePower)
                SendBattlePower();

            SendArsenal(pUser);
            return true;
        }

        public bool UninscribeItem(uint idItem, Character pUser)
        {
            DbItem dbItem = Database.Items.FetchByIdentity(idItem);
            if (dbItem == null)
                return false;

            TotemPoleType pType = GetArsenalType(Calculations.GetItemPosition(dbItem.Type));
            if (pType == TotemPoleType.TOTEM_NONE)
                return false;

            Totem pPole;
            if (!Poles.ContainsKey(pType) || !Poles[pType].Items.TryGetValue(idItem, out pPole))
                return false;

            return UninscribeItem(pPole.Item, pUser);
        }

        public bool UninscribeItem(Item pItem, Character pUser)
        {
            TotemPoleType pType = GetArsenalType(pItem);
            if (pType == TotemPoleType.TOTEM_NONE) // shit happens
                return false;

            if (!Poles.ContainsKey(pType))
                return false;

            if (!Poles[pType].Items.ContainsKey(pItem.Identity))
                return false;

            uint dwOldBp = BattlePower;

            Totem pTotem;
            if (!Poles[pType].Items.TryRemove(pItem.Identity, out pTotem))
                return false;

            pTotem.Delete();
            pItem.Inscribed = false;
            pUser.Send(pItem.InformationPacket(true));
            UpdatePoles();

            if (dwOldBp != BattlePower)
                SendBattlePower();

            SendArsenal(pUser);
            return true;
        }

        public void RemoveAllFromUser(uint idUser)
        {
            uint oldBp = BattlePower;
            foreach (var pole in Poles.Values)
            {
                List<uint> remove = new List<uint>();
                foreach (var totem in pole.Items.Values.Where(x => x.PlayerIdentity == idUser))
                {
                    totem.Item.Inscribed = false;
                    totem.Remove();
                    totem.Delete();
                    remove.Add(totem.Identity);
                }
                Totem trash;
                for (int i = 0; i < remove.Count; i++)
                    pole.Items.TryRemove(remove[i], out trash);
                trash = null;
            }

            Client pUser;
            if (ServerKernel.Players.TryGetValue(idUser, out pUser))
            {
                foreach (var bag in pUser.Character.Inventory.Items.Values.Where(x => x.Inscribed))
                {
                    bag.Inscribed = false;
                    bag.Save();
                }
                foreach (var equip in pUser.Character.Equipment.Items.Values.Where(x => x.Inscribed))
                {
                    equip.Inscribed = false;
                    equip.Save();
                }
                foreach (var wh in pUser.Character.Warehouses.Values)
                {
                    foreach (var whItem in wh.Items.Values.Where(x => x.Inscribed))
                    {
                        whItem.Inscribed = false;
                        whItem.Save();
                    }
                }
            }

            //Database.TotemPoleRepository.ClearUserTotem(idUser);

            UpdatePoles();
            if (oldBp != BattlePower)
                SendBattlePower();
        }

        private bool OpenArsenal(TotemPoleType pType)
        {
            Owner.LastTotemOpen = uint.Parse(DateTime.Now.ToString("yyyyMMdd"));

            switch (pType)
            {
                case TotemPoleType.TOTEM_HEADGEAR:
                    Owner.HeadgearTotem = true;
                    break;
                case TotemPoleType.TOTEM_NECKLACE:
                    Owner.NecklaceTotem = true;
                    break;
                case TotemPoleType.TOTEM_RING:
                    Owner.RingTotem = true;
                    break;
                case TotemPoleType.TOTEM_WEAPON:
                    Owner.WeaponTotem = true;
                    break;
                case TotemPoleType.TOTEM_ARMOR:
                    Owner.ArmorTotem = true;
                    break;
                case TotemPoleType.TOTEM_BOOTS:
                    Owner.BootsTotem = true;
                    break;
                case TotemPoleType.TOTEM_FAN:
                    Owner.HeavenFanTotem = true;
                    break;
                case TotemPoleType.TOTEM_TOWER:
                    Owner.StarTowerTotem = true;
                    break;
            }

            return Poles.TryAdd(pType, new TotemPole(pType) { Locked = false });
        }

        public bool UnlockArsenal(TotemPoleType pType, Character pUser)
        {
            if (pUser.SyndicateMember == null || pUser.Syndicate == null || pUser.Syndicate != Owner
                ||
                (pUser.SyndicateMember.Position != SyndicateRank.GUILD_LEADER &&
                 pUser.SyndicateMember.Position != SyndicateRank.LEADER_SPOUSE &&
                 pUser.SyndicateMember.Position != SyndicateRank.DEPUTY_LEADER &&
                 pUser.SyndicateMember.Position != SyndicateRank.HONORARY_DEPUTY_LEADER))
            {
                pUser.Send("You don`t have permission to open arsenals.");
                return false;
            }

            if (pType == TotemPoleType.TOTEM_NONE)
            {
                pUser.Send("Invalid arsenal type.");
                return false;
            }

            if (Poles.ContainsKey(pType))
            {
                pUser.Send("This arsenal is already open.");
                return false;
            }

            uint dwPrice = UnlockValue();

            if (Owner.SilverDonation < dwPrice)
            {
                pUser.Send("Not enough funds.");
                return false;
            }

            if (uint.Parse(DateTime.Now.ToString("yyyyMMdd")) <= Owner.LastTotemOpen)
            {
                pUser.Send("Your guild already oppened an arsenal today.");
                return false;
            }

            if (!Owner.ChangeFunds((int) UnlockValue()*-1))
                return false;

            OpenArsenal(pType);
            SendArsenal(pUser);
            pUser.SyndicateMember.SendSyndicate();
            return true;
        }

        public void SendBattlePower()
        {
            foreach (var member in Owner.Members.Values.Where(x => x.IsOnline))
            {
                member.Owner.Character.RecalculateAttributes();
                member.Owner.Character.UpdateClient(ClientUpdateType.GUILD_BATTLEPOWER,
                    SharedBattlePower(member.Position), false);
            }
        }

        public int MaxPerType(Character pUser)
        {
            if (pUser.Metempsychosis == 0)
            {
                if (pUser.Level < 100)
                    return 7;
                return 14;
            }
            if (pUser.Metempsychosis == 1)
                return 21;
            return 30;
        }

        public static TotemPoleType GetArsenalType(Item item)
        {
            return GetArsenalType(Calculations.GetItemPosition(item.Type));
        }

        public static TotemPoleType GetArsenalType(ItemPosition pos)
        {
            switch (pos)
            {
                case ItemPosition.HEADWEAR: return TotemPoleType.TOTEM_HEADGEAR;
                case ItemPosition.NECKLACE: return TotemPoleType.TOTEM_NECKLACE;
                case ItemPosition.RING: return TotemPoleType.TOTEM_RING;
                case ItemPosition.LEFT_HAND:
                case ItemPosition.RIGHT_HAND: return TotemPoleType.TOTEM_WEAPON;
                case ItemPosition.ARMOR: return TotemPoleType.TOTEM_ARMOR;
                case ItemPosition.BOOTS: return TotemPoleType.TOTEM_BOOTS;
                case ItemPosition.ATTACK_TALISMAN: return TotemPoleType.TOTEM_FAN;
                case ItemPosition.DEFENCE_TALISMAN: return TotemPoleType.TOTEM_TOWER;
                default: return TotemPoleType.TOTEM_NONE;
            }
        }

        public void SendArsenal(Character pUser)
        {
            var pMsg = new MsgTotemPoleInfo();
            UpdatePoles();
            var orderedTotem = Poles.Values.OrderByDescending(x => x.Donation).ToList();
            for (TotemPoleType i = TotemPoleType.TOTEM_HEADGEAR; i < TotemPoleType.TOTEM_NONE; i++)
            {
                TotemPole totem = orderedTotem.FirstOrDefault(x => x.Type == i);
                if (totem == null)
                {
                    pMsg.AddTotemPole(i, 0, 0, 0, false);
                    continue;
                }
                pMsg.AddTotemPole(totem.Type, totem.BattlePower, 0, (uint) totem.Donation, true);
                //switch (totem.Type)
                //{
                //    case TotemPoleType.TOTEM_HEADGEAR:
                //        pMsg.HearwearIdentity = (uint)totem.Type;
                //        pMsg.TotemHeadwearIsOpen = pUser.Syndicate.HeadgearTotem;
                //        pMsg.HeadwearBattlePower = totem.BattlePower;
                //        pMsg.HeadwearTotalDonation = (uint)totem.Donation;
                //        break;
                //    case TotemPoleType.TOTEM_NECKLACE:
                //        pMsg.NecklaceIdentity = (uint)totem.Type;
                //        pMsg.TotemNecklaceIsOpen = pUser.Syndicate.NecklaceTotem;
                //        pMsg.NecklaceBattlePower = totem.BattlePower;
                //        pMsg.NecklaceTotalDonation = (uint)totem.Donation;
                //        break;
                //    case TotemPoleType.TOTEM_RING:
                //        pMsg.RingIdentity = (uint)totem.Type;
                //        pMsg.TotemRingIsOpen = pUser.Syndicate.RingTotem;
                //        pMsg.RingBattlePower = totem.BattlePower;
                //        pMsg.RingTotalDonation = (uint)totem.Donation;
                //        break;
                //    case TotemPoleType.TOTEM_WEAPON:
                //        pMsg.WeaponIdentity = (uint)totem.Type;
                //        pMsg.TotemWeaponIsOpen = pUser.Syndicate.WeaponTotem;
                //        pMsg.WeaponBattlePower = totem.BattlePower;
                //        pMsg.WeaponTotalDonation = (uint)totem.Donation;
                //        break;
                //    case TotemPoleType.TOTEM_ARMOR:
                //        pMsg.ArmorIdentity = (uint)totem.Type;
                //        pMsg.TotemArmorIsOpen = pUser.Syndicate.ArmorTotem;
                //        pMsg.ArmorBattlePower = totem.BattlePower;
                //        pMsg.ArmorTotalDonation = (uint)totem.Donation;
                //        break;
                //    case TotemPoleType.TOTEM_BOOTS:
                //        pMsg.BootsIdentity = (uint)totem.Type;
                //        pMsg.TotemBootsIsOpen = pUser.Syndicate.BootsTotem;
                //        pMsg.BootsBattlePower = totem.BattlePower;
                //        pMsg.BootsTotalDonation = (uint)totem.Donation;
                //        break;
                //    case TotemPoleType.TOTEM_FAN:
                //        pMsg.FanIdentity = (uint)totem.Type;
                //        pMsg.TotemFanIsOpen = pUser.Syndicate.HeavenFanTotem;
                //        pMsg.FanBattlePower = totem.BattlePower;
                //        pMsg.FanTotalDonation = (uint)totem.Donation;
                //        break;
                //    case TotemPoleType.TOTEM_TOWER:
                //        pMsg.TowerIdentity = (uint)totem.Type;
                //        pMsg.TotemTowerIsOpen = pUser.Syndicate.StarTowerTotem;
                //        pMsg.TowerBattlePower = totem.BattlePower;
                //        pMsg.TowerTotalDonation = (uint)totem.Donation;
                //        break;
                //}
            }
            pMsg.SharedBattlePower = BattlePower;
            pMsg.BattlePower = SharedBattlePower(pUser.SyndicateMember.Position);
            pMsg.TotemDonation = pUser.SyndicateMember.ArsenalDonation;

            if (pUser.Syndicate.Arsenal != null)
                pUser.UpdateClient(ClientUpdateType.GUILD_BATTLEPOWER, pUser.Syndicate.Arsenal.SharedBattlePower(pUser));

            pUser.Send(pMsg);
        }

        public void UpdatePoles()
        {
            foreach (var pole in Poles.Values)
            {
                pole.BattlePower = 0;
                pole.Donation = 0;
                foreach (var totem in pole.Items.Values)
                    pole.Donation += totem.Donation();

                if (pole.Donation >= 2000000)
                {
                    pole.BattlePower++;
                    if (pole.Donation >= 4000000)
                    {
                        pole.BattlePower++;
                        if (pole.Donation >= 10000000)
                            pole.BattlePower++;
                    }
                }
            }

            BattlePower = 0;
            int nCount = 0;
            foreach (var pole in Poles.Values.OrderByDescending(x => x.Donation))
            {
                if (nCount++ >= 5)
                    break;
                BattlePower += pole.BattlePower;
            }
        }

        public uint GetBattlePower()
        {
            uint ret = 0;
            foreach (var totem in Poles.Values.Where(x => !x.Locked))
                ret += totem.BattlePower;
            return ret;
        }

        public uint SharedBattlePower(Client client) { return SharedBattlePower(client.Character.SyndicateRank); }

        public uint SharedBattlePower(Character client) { return SharedBattlePower(client.SyndicateRank); }

        public uint SharedBattlePower(SyndicateRank rank)
        {
            uint totalBp = BattlePower;

            switch (rank)
            {
                case SyndicateRank.GUILD_LEADER:
                case SyndicateRank.LEADER_SPOUSE:
                    return totalBp;
                case SyndicateRank.DEPUTY_LEADER:
                case SyndicateRank.HONORARY_DEPUTY_LEADER:
                    return (uint)(totalBp * 0.9f);
                case SyndicateRank.MANAGER:
                case SyndicateRank.HONORARY_MANAGER:
                    return (uint)(totalBp * 0.8f);
                case SyndicateRank.SUPERVISOR:
                case SyndicateRank.ARSENAL_SUPERVISOR:
                case SyndicateRank.CP_SUPERVISOR:
                case SyndicateRank.GUIDE_SUPERVISOR:
                case SyndicateRank.HONORARY_SUPERVISOR:
                case SyndicateRank.LILY_SUPERVISOR:
                case SyndicateRank.ORCHID_SUPERVISOR:
                case SyndicateRank.PK_SUPERVISOR:
                case SyndicateRank.ROSE_SUPERVISOR:
                case SyndicateRank.SILVER_SUPERVISOR:
                case SyndicateRank.TULIP_SUPERVISOR:
                    return (uint)(totalBp * 0.7f);
                case SyndicateRank.STEWARD:
                case SyndicateRank.DEPUTY_LEADER_SPOUSE:
                case SyndicateRank.LEADER_SPOUSE_AIDE:
                case SyndicateRank.DEPUTY_LEADER_AIDE:
                    return (uint)(totalBp * 0.5f);
                case SyndicateRank.DEPUTY_STEWARD:
                    return (uint)(totalBp * 0.4f);
                case SyndicateRank.AGENT:
                case SyndicateRank.SUPERVISOR_SPOUSE:
                case SyndicateRank.MANAGER_SPOUSE:
                case SyndicateRank.SUPERVISOR_AIDE:
                case SyndicateRank.MANAGER_AIDE:
                case SyndicateRank.ARSENAL_AGENT:
                case SyndicateRank.CP_AGENT:
                case SyndicateRank.GUIDE_AGENT:
                case SyndicateRank.LILY_AGENT:
                case SyndicateRank.ORCHID_AGENT:
                case SyndicateRank.PK_AGENT:
                case SyndicateRank.ROSE_AGENT:
                case SyndicateRank.SILVER_AGENT:
                case SyndicateRank.TULIP_AGENT:
                    return (uint)(totalBp * 0.3f);
                case SyndicateRank.STEWARD_SPOUSE:
                case SyndicateRank.FOLLOWER:
                case SyndicateRank.ARSENAL_FOLLOWER:
                case SyndicateRank.CP_FOLLOWER:
                case SyndicateRank.GUIDE_FOLLOWER:
                case SyndicateRank.LILY_FOLLOWER:
                case SyndicateRank.ORCHID_FOLLOWER:
                case SyndicateRank.PK_FOLLOWER:
                case SyndicateRank.ROSE_FOLLOWER:
                case SyndicateRank.SILVER_FOLLOWER:
                case SyndicateRank.TULIP_FOLLOWER:
                    return (uint)(totalBp * 0.2f);
                case SyndicateRank.SENIOR_MEMBER:
                    return (uint)(totalBp * 0.15f);
                case SyndicateRank.MEMBER:
                    return (uint)(totalBp * 0.1f);
                default:
                    return 0;
            }
        }

        public uint UnlockValue()
        {
            switch (Poles.Count)
            {
                case 0:
                case 1: return 5000000;
                case 2:
                case 3:
                case 4: return 10000000;
                case 5:
                case 6: return 15000000;
                case 7: return 20000000;
            }
            return 0;
        }
    }
}