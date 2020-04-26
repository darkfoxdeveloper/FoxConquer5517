// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Detained Object.cs
// Last Edit: 2016/12/26 18:09
// Created: 2016/12/26 17:14

using System;
using System.Linq;
using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public sealed class DetainedObject
    {
        public const uint DETAIN_TIME = 60 * 60 * 24 * 7;

        private DbDetainedItem m_dbItem;
        private DbPkReward m_dbReward;

        private MsgDetainItemInfo m_pPacket;

        public DetainedObject(bool isItem)
        {
            if (isItem)
                m_dbItem = new DbDetainedItem();
            else 
                m_dbReward = new DbPkReward();
        }

        public bool Create(DbDetainedItem dbItem)
        {
            m_dbItem = dbItem;

            //if (!HasExpired)
            {
                DbItem item = Database.Items.FetchAll(m_dbItem.TargetIdentity)
                                        .FirstOrDefault(x => x.Id == m_dbItem.ItemIdentity);
                if (item == null)
                {
                    ServerKernel.Log.GmLog("detain_reward_itemview",
                        string.Format("Could not fetch data to item id {0}", m_dbItem.ItemIdentity));
                    return false;
                }

                m_pPacket = new MsgDetainItemInfo
                {
                    Identity = m_dbItem.Identity,
                    ItemIdentity = item.Id,
                    Itemtype = item.Type,
                    Blessing = item.ReduceDmg,
                    Plus = item.Magic3,
                    Bound = item.Monopoly == 3,
                    Color = (ItemColor)item.Color,
                    Cost = m_dbItem.RedeemPrice,
                    Date =
                        uint.Parse(UnixTimestamp.ToDateTime((uint)m_dbItem.HuntTime).ToString("yyyyMMdd")),
                    DaysPast = RemainingTime,
                    SocketOne = (SocketGem)item.Gem1,
                    SocketTwo = (SocketGem)item.Gem2,
                    Enchantment = item.AddLife,
                    Effect = (ItemEffect)item.Magic1,
                    Durability = item.Amount,
                    MaximumDurability = item.AmountLimit,
                    Mode = DetainMode.CLAIM_PAGE,
                    SocketProgress = item.Data,
                    OwnerIdentity = m_dbItem.TargetIdentity,
                    OwnerName = m_dbItem.TargetName,
                    TargetIdentity = m_dbItem.HunterIdentity,
                    TargetName = m_dbItem.HunterName,
                    Expired = HasExpired
                };
            }
            return true;
        }

        public bool Create(DbPkReward dbReward)
        {
            m_dbReward = dbReward;
            m_pPacket = new MsgDetainItemInfo
            {
                Identity = m_dbReward.Identity,
                ItemIdentity = m_dbReward.Identity,
                Itemtype = 410339,
                Cost = m_dbReward.Bonus,
                Mode = DetainMode.CLAIM_PAGE,
                TargetIdentity = m_dbReward.HunterIdentity,
                TargetName = m_dbReward.HunterName,
                OwnerIdentity = m_dbReward.TargetIdentity,
                OwnerName = m_dbReward.TargetName
            };
            return true;
        }

        public uint Identity
        {
            get
            {
                if (IsItem())
                    return m_dbItem.Identity;
                if (IsMoney() || IsEmoney())
                    return m_dbReward.Identity;
                return 0;
            }
        }

        public uint ItemIdentity
        {
            get
            {
                if (IsItem())
                    return m_dbItem.ItemIdentity;
                return 0;
            }
        }

        public uint RemainingTime
        {
            get
            {
                int now = UnixTimestamp.Timestamp();
                if (now > m_dbItem.HuntTime + DETAIN_TIME)
                    return 0;
                return (uint)Math.Max(1,
                    (UnixTimestamp.ToDateTime((uint)m_dbItem.HuntTime) - UnixTimestamp.ToDateTime((uint)now)).Days);
            }
        }

        public uint TargetIdentity
        {
            get
            {
                if (m_dbItem != null) return m_dbItem.TargetIdentity;
                if (m_dbReward != null) return m_dbReward.TargetIdentity;
                return 0;
            }
        }

        public string TargetName
        {
            get
            {
                if (m_dbItem != null) return m_dbItem.TargetName;
                if (m_dbReward != null) return m_dbReward.TargetName;
                return "ERROR";
            }
        }

        public uint HunterIdentity
        {
            get
            {
                if (m_dbItem != null) return m_dbItem.HunterIdentity;
                if (m_dbReward != null) return m_dbReward.HunterIdentity;
                return 0;
            }
        }

        public string HunterName
        {
            get
            {
                if (m_dbItem != null) return m_dbItem.HunterName;
                if (m_dbReward != null) return m_dbReward.HunterName;
                return "ERROR";
            }
        }

        public uint Value
        {
            get
            {
                if (IsItem())
                    return m_dbItem.RedeemPrice;
                if (IsMoney() || IsEmoney())
                    return m_dbReward.Bonus;
                return 0;
            }
        }

        public bool HasExpired
        {
            get { return UnixTimestamp.Timestamp() > m_dbItem.HuntTime + DETAIN_TIME; }
        }

        public MsgDetainItemInfo GetPacket(DetainMode pMode = DetainMode.CLAIM_PAGE)
        {
            m_pPacket.Mode = pMode;
            return m_pPacket;
        }

        public MsgItem GetPrizePacket()
        {
            if (IsEmoney())
            {
                return new MsgItem
                {
                    Action = ItemAction.REDEEM_EQUIPMENT,
                    Identity = m_dbReward.Identity,
                    Param1 = m_dbReward.TargetIdentity,
                    Param3 = m_dbReward.Bonus
                };
            }
            return null;
        }

        public bool IsItem()
        {
            return m_dbItem != null;
        }

        public bool IsMoney()
        {
            if (m_dbReward == null) return false;
            return m_dbReward.BonusType == 0;
        }

        public bool IsEmoney()
        {
            if (m_dbReward == null) return false;
            return m_dbReward.BonusType == 1;
        }

        public void OnTimer()
        {
            if (IsItem() && HasExpired)
            {
                m_pPacket.Expired = HasExpired;
                Client pHunter, pTarget;
                if (ServerKernel.Players.TryGetValue(m_dbReward.HunterIdentity, out pHunter))
                {
                    pHunter.SendMessage(string.Format("An has expired and now can be claimed."), ChatTone.TALK);
                }
                if (ServerKernel.Players.TryGetValue(m_dbReward.TargetIdentity, out pTarget))
                {
                    pTarget.SendMessage("Your equipment has expired because it was not redeemed in time.", ChatTone.TALK);
                }
            }
        }

        public static uint GetDetainPrice(Item item)
        {
            uint dwPrice = 100;

            if (item.GetQuality() == 9) // if super +500CPs
                dwPrice += 500;

            switch (item.Plus) // (+n)
            {
                case 1: dwPrice += 10; break;
                case 2: dwPrice += 20; break;
                case 3: dwPrice += 50; break;
                case 4: dwPrice += 150; break;
                case 5: dwPrice += 300; break;
                case 6: dwPrice += 900; break;
                case 7: dwPrice += 2700; break;
                case 8: dwPrice += 6000; break;
                case 9:
                case 10:
                case 11:
                case 12: dwPrice += 12000; break;
            }

            if (item.IsWeapon()) // if weapon
            {
                if (item.SocketTwo > SocketGem.NO_SOCKET)
                    dwPrice += 1000;
                else if (item.SocketOne > SocketGem.NO_SOCKET)
                    dwPrice += 100;
            }
            else // if not
            {
                if (item.SocketTwo > SocketGem.NO_SOCKET)
                    dwPrice += 1500;
                else if (item.SocketOne > SocketGem.NO_SOCKET)
                    dwPrice += 5000;
            }

            if (item.ArtifactIsPermanent())
            {
                switch (item.Artifact.ArtifactLevel)
                {
                    case 1: dwPrice += 300; break;
                    case 2: dwPrice += 900; break;
                    case 3: dwPrice += 1800; break;
                    case 4: dwPrice += 3000; break;
                    case 5: dwPrice += 4500; break;
                    case 6: dwPrice += 6000; break;
                }
            }

            if (item.RefineryIsPermanent())
            {
                switch (item.RefineryLevel)
                {
                    case 1: dwPrice += 300; break;
                    case 2: dwPrice += 900; break;
                    case 3: dwPrice += 2000; break;
                    case 4: dwPrice += 4000; break;
                    case 5: dwPrice += 6000; break;
                }
            }

            return dwPrice;
        }

        public static bool ContainsReward(uint idHunter)
        {
            if (ServerKernel.DetainedObjects.Values.FirstOrDefault(
                    x => x.HasExpired && x.HunterIdentity == idHunter) != null)
                return true;
            return ServerKernel.DetainedObjects.Values.FirstOrDefault(x => x.HunterIdentity == idHunter) != null;
        }

        public static bool ContainsDetained(uint idTarget)
        {
            return ServerKernel.DetainedObjects.Values.FirstOrDefault(x => x.TargetIdentity == idTarget) != null;
        }

        /// <summary>
        /// Find Items that owner detained.
        /// </summary>
        public static DetainedObject[] FindByHunter(uint idHunter)
        {
            DetainedObject[] items = new DetainedObject[100];
            int idx = 0;
            foreach (var item in ServerKernel.DetainedObjects.Values.Where(x => x.HunterIdentity == idHunter))
            {
                if (idx > 99) break;
                items[idx++] = item;
            }
            if (idx < 99)
                Array.Resize(ref items, idx);
            return items;
        }

        /// <summary>
        /// Find items that account has to recover.
        /// </summary>
        public static DetainedObject[] FindByTarget(uint idTarget)
        {
            DetainedObject[] items = new DetainedObject[100];
            int idx = 0;
            foreach (var item in ServerKernel.DetainedObjects.Values.Where(x => x.TargetIdentity == idTarget))
            {
                if (idx > 99) break;
                items[idx++] = item;
            }
            if (idx < 99)
                Array.Resize(ref items, idx);
            return items;
        }

        /// <summary>
        /// Only CPs to claim.
        /// </summary>
        public static DetainedObject[] FindRewards(uint idHunter)
        {
            DetainedObject[] items = new DetainedObject[100];
            int idx = 0;
            foreach (var item in ServerKernel.DetainedObjects.Values.Where(x => x.HunterIdentity == idHunter && x.IsEmoney()))
            {
                if (idx > 99) break;
                items[idx++] = item;
            }
            if (idx < 99)
                Array.Resize(ref items, idx);
            return items;
        }

        public bool Save()
        {
            if (m_dbReward != null)
                return new PkBonusRepository().SaveOrUpdate(m_dbReward);
            if (m_dbItem != null)
                return new DetainedItemRepository().SaveOrUpdate(m_dbItem);
            return false;
        }

        public bool Delete()
        {
            if (m_dbReward != null)
                return new PkBonusRepository().Delete(m_dbReward);
            if (m_dbItem != null)
                return new DetainedItemRepository().Delete(m_dbItem);
            return false;
        }
    }
}