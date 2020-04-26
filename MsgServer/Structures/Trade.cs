// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Trade.cs
// Last Edit: 2016/12/07 10:10
// Created: 2016/12/07 10:08

using System.Collections.Generic;
using System.Linq;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public class Trade
    {
        private Character m_pOwner;
        private Character m_pTarget;
        private bool m_bOwnerConfirmed;
        private bool m_bTargetConfirmed;
        private bool m_bAccepted;

        private uint m_dwOwnerMoney;
        private uint m_dwOwnerEmoney;
        private uint m_dwTargetMoney;
        private uint m_dwTargetEmoney;

        private Dictionary<uint, Item> m_dicOwner = new Dictionary<uint, Item>(20);
        private Dictionary<uint, Item> m_dicTarget = new Dictionary<uint, Item>(20);

        public Trade(Character owner, Character target)
        {
            m_pOwner = owner;
            m_pTarget = target;
            m_pOwner.Trade = this;
            m_pTarget.Trade = this;
        }

        public bool AddItem(uint id, Character pSender)
        {
            Item item;
            if (pSender.Inventory.Items.TryGetValue(id, out item))
            {
                if (item.Bound)
                {
                    var msg = new MsgTrade
                    {
                        Target = id,
                        Type = TradeType.REMOVE_ITEM
                    };
                    pSender.Send(msg);
                    pSender.Send(ServerString.STR_NOT_FOR_TRADE);
                    return false;
                }

                if (item.Itemtype.Monopoly == 3 || item.Itemtype.Monopoly == 9)
                {
                    var msg = new MsgTrade
                    {
                        Target = id,
                        Type = TradeType.REMOVE_ITEM
                    };
                    pSender.Send(msg);
                    pSender.Send(ServerString.STR_NOT_FOR_TRADE);
                    return false;
                }

                TradePartner partner;

                if (pSender == m_pOwner)
                {
                    if ((m_pTarget.Inventory.RemainingSpace() - m_dicTarget.Count - 1) >= 40)
                    {
                        var pmsg = new MsgTrade
                        {
                            Target = id,
                            Type = TradeType.REMOVE_ITEM
                        };
                        pSender.Send(pmsg);

                        pSender.Send(ServerString.STR_TRADE_TARGET_BAG_FULL);
                        m_pTarget.Send(ServerString.STR_TRADE_YOUR_BAG_IS_FULL);
                        return false;
                    }

                    if (item.IsLocked()
                        && (!pSender.TradePartners.TryGetValue(m_pTarget.Identity, out partner)
                            || !partner.IsActive))
                    {
                        var pmsg = new MsgTrade
                        {
                            Target = id,
                            Type = TradeType.REMOVE_ITEM
                        };
                        pSender.Send(pmsg);
                        pSender.Send(ServerString.STR_NOT_FOR_TRADE);
                        return false;
                    }

                    if (!item.CanBeSold)
                    {
                        var pmsg = new MsgTrade
                        {
                            Target = id,
                            Type = TradeType.REMOVE_ITEM
                        };
                        pSender.Send(pmsg);
                        pSender.Send(ServerString.STR_NOT_FOR_TRADE);
                        return false;
                    }

                    if (m_dicOwner.ContainsKey(id))
                    {
                        var pmsg = new MsgTrade
                        {
                            Target = id,
                            Type = TradeType.REMOVE_ITEM
                        };
                        pSender.Send(pmsg);
                        // item already exists
                        return false;
                    }

                    m_dicOwner.Add(id, item);

                    var msg = item.InformationPacket();
                    msg.ItemMode = ItemMode.TRADE;
                    m_pTarget.Send(msg);
                }
                else if (pSender == m_pTarget)
                {
                    if (m_pOwner.Inventory.RemainingSpace() - m_dicOwner.Count - 1 >= 40)
                    {
                        var pmsg = new MsgTrade
                        {
                            Target = id,
                            Type = TradeType.REMOVE_ITEM
                        };
                        pSender.Send(pmsg);

                        pSender.Send(ServerString.STR_TRADE_YOUR_BAG_IS_FULL);
                        m_pTarget.Send(ServerString.STR_TRADE_TARGET_BAG_FULL);
                        return false;
                    }

                    if (item.IsLocked()
                        && (!pSender.TradePartners.TryGetValue(m_pOwner.Identity, out partner)
                            || !partner.IsActive))
                    {
                        var pmsg = new MsgTrade
                        {
                            Target = id,
                            Type = TradeType.REMOVE_ITEM
                        };
                        pSender.Send(pmsg);
                        pSender.Send(ServerString.STR_NOT_FOR_TRADE);
                        return false;
                    }

                    if (!item.CanBeSold)
                    {
                        var pmsg = new MsgTrade
                        {
                            Target = id,
                            Type = TradeType.REMOVE_ITEM
                        };
                        pSender.Send(pmsg);
                        pSender.Send(ServerString.STR_NOT_FOR_TRADE);
                        return false;
                    }

                    if (m_dicTarget.ContainsKey(id))
                    {
                        // item already exists
                        var pmsg = new MsgTrade
                        {
                            Target = id,
                            Type = TradeType.REMOVE_ITEM
                        };
                        pSender.Send(pmsg);
                        return false;
                    }

                    m_dicTarget.Add(id, item);

                    var msg = item.InformationPacket();
                    msg.ItemMode = ItemMode.TRADE;
                    m_pOwner.Send(msg);
                }
                else
                {
                    return false; // who the fuck?
                }

                return true;
            }
            pSender.Send(ServerString.STR_NOT_FOR_TRADE);
            return false;
        }

        public bool AddMoney(uint dwAmount, Character pSender, MsgTrade pMsg)
        {
            if (dwAmount > pSender.Silver)
            {
                pSender.Send(ServerString.STR_NOT_ENOUGH_MONEY);
                return false;
            }

            if (pSender == m_pOwner)
            {
                if (m_pTarget.Silver + dwAmount > int.MaxValue)
                {
                    CloseWindow();
                    return false;
                }
                m_dwOwnerMoney = dwAmount;
                pMsg.Type = TradeType.SHOW_MONEY;
                m_pTarget.Send(pMsg);
            }
            else if (pSender == m_pTarget)
            {
                if (m_pOwner.Silver + dwAmount > int.MaxValue)
                {
                    CloseWindow();
                    return false;
                }
                m_dwTargetMoney = dwAmount;
                pMsg.Type = TradeType.SHOW_MONEY;
                m_pOwner.Send(pMsg);
            }
            else
            {
                // dude?
                return false;
            }

            return true;
        }

        public bool AddEmoney(uint dwAmount, Character pSender, MsgTrade pMsg)
        {
            if (dwAmount > pSender.Emoney)
            {
                pSender.Send(ServerString.STR_NOT_ENOUGH_EMONEY);
                return false;
            }

            if (pSender == m_pOwner)
            {
                if (m_pTarget.Emoney + dwAmount > int.MaxValue)
                {
                    CloseWindow();
                    return false;
                }
                m_dwOwnerEmoney = dwAmount;
                pMsg.Type = TradeType.SHOW_CONQUER_POINTS;
                m_pTarget.Send(pMsg);
            }
            else if (pSender == m_pTarget)
            {
                if (m_pOwner.Emoney + dwAmount > int.MaxValue)
                {
                    CloseWindow();
                    return false;
                }
                m_dwTargetEmoney = dwAmount;
                pMsg.Type = TradeType.SHOW_CONQUER_POINTS;
                m_pOwner.Send(pMsg);
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool AcceptTrade(Character pSender, MsgTrade pMsg)
        {
            if (!m_bAccepted)
            {
                if (pSender == m_pOwner)
                {
                    pMsg.Target = pSender.Identity;
                    m_pTarget.Send(pMsg);
                    m_bOwnerConfirmed = true;
                    if (!m_bTargetConfirmed)
                        return true;
                }
                else if (pSender == m_pTarget)
                {
                    pMsg.Target = pSender.Identity;
                    m_pOwner.Send(pMsg);
                    m_bTargetConfirmed = true;
                    if (!m_bOwnerConfirmed)
                        return true;
                }
                else
                {
                    return false;
                }
                m_bAccepted = true;
            }

            /*  
             Changed the steps a little bit from the old version. In the old version if p1 inserts items and
             send the acceptance packet twice it would confirm the trade :) 2016-04-19
             */
            if (!m_bAccepted) return false; // should not reach here if both didn't accept

            // checks if both accounts still has the items on the inventory
            bool success = m_dicOwner.Values.All(item => m_pOwner.Inventory.Items.ContainsKey(item.Identity));

            if (!success)
            {
                m_pOwner.Send(ServerString.STR_TRADE_FAIL);
                m_pTarget.Send(ServerString.STR_TRADE_FAIL);
                CloseWindow();
                return false;
            }

            success = m_dicTarget.Values.All(item => m_pTarget.Inventory.Items.ContainsKey(item.Identity));

            // checks if both accounts still have space in the inventory
            if (m_pOwner.Inventory.RemainingSpace() < m_dicTarget.Count)
                success = false;
            if (m_pTarget.Inventory.RemainingSpace() < m_dicOwner.Count)
                success = false;

            // checks for money and emoney
            if (m_dwOwnerMoney > m_pOwner.Silver)
                success = false;
            if (m_dwOwnerEmoney > m_pOwner.Emoney)
                success = false;
            if (m_dwTargetMoney > m_pTarget.Silver)
                success = false;
            if (m_dwTargetEmoney > m_pTarget.Emoney)
                success = false;

            if (!success)
            {
                m_pOwner.Send(ServerString.STR_TRADE_FAIL);
                m_pTarget.Send(ServerString.STR_TRADE_FAIL);
                CloseWindow();
                return false;
            }

            ServerKernel.Log.GmLog("trade_item",
                string.Format("user1_id[{0}] user1[{1}] lev1[{2}] money1[{3}] emoney1[{4}]", m_pOwner.Identity, m_pOwner.Name, m_pOwner.Level, m_dwOwnerMoney, m_dwOwnerEmoney));
            ServerKernel.Log.GmLog("trade_item",
                string.Format("user2_id[{0}] user2[{1}] lev2[{2}] money2[{3}] emoney2[{4}]", m_pTarget.Identity, m_pTarget.Name, m_pTarget.Level, m_dwTargetMoney, m_dwTargetEmoney));

            CloseWindow();
            foreach (var item in m_dicOwner.Values)
            {
                m_pOwner.Inventory.Remove(item.Identity, ItemRemovalType.TAKE_OUT_FROM_INVENTORY_ONLY);
                item.OwnerIdentity = m_pOwner.Identity;
                item.PlayerIdentity = m_pTarget.Identity;
                m_pTarget.Inventory.Add(item);

                ServerKernel.Log.GmLog("trade_item",
                    string.Format(
                        "user_id[{0}] name[{1}]| item[{2}][{3}] type[{4}] gem1[{5}] gem2[{6}] magic3[{7}] dur[{8}] max_dur[{9}] data[{10}], dec_dmg[{11}] add_life[{12}] color[{13}] Addlevel_exp[{14}] artifact_type[{15}] artifact_expire[{16}] artifact_stabilization[{17}] refinery_type[{18}] refinery_level[{19}] refinery_stabilization[{20}] stack_amount[{21}] remaining_time[{22}]",
                        m_pOwner.Identity, m_pOwner.Name, item.Itemtype.Name, item.Identity, item.Type, item.SocketOne,
                        item.SocketTwo, item.Plus, item.Durability, item.MaximumDurability, item.SocketProgress,
                        item.ReduceDamage, item.Enchantment, item.Color, item.CompositionProgress,
                        item.ArtifactType, item.Artifact.ArtifactExpireTime, item.ArtifactStabilization,
                        item.RefineryType, item.RefineryLevel, item.RefineryStabilization, item.StackAmount,
                        item.RemainingTime));
            }

            foreach (var item in m_dicTarget.Values)
            {
                m_pTarget.Inventory.Remove(item.Identity, ItemRemovalType.TAKE_OUT_FROM_INVENTORY_ONLY);
                item.OwnerIdentity = m_pTarget.Identity;
                item.PlayerIdentity = m_pOwner.Identity;
                m_pOwner.Inventory.Add(item);

                ServerKernel.Log.GmLog("trade_item",
                    string.Format("user_id[{0}] name[{1}]| item[{2}][{3}] type[{4}] gem1[{5}] gem2[{6}] magic3[{7}] dur[{8}] max_dur[{9}] data[{10}], dec_dmg[{11}] add_life[{12}] color[{13}] Addlevel_exp[{14}] artifact_type[{15}] artifact_expire[{16}] artifact_stabilization[{17}] refinery_type[{18}] refinery_level[{19}] refinery_stabilization[{20}] stack_amount[{21}] remaining_time[{22}]",
                        m_pTarget.Identity, m_pTarget.Name, item.Itemtype.Name, item.Identity, item.Type, item.SocketOne, item.SocketTwo, item.Plus, item.Durability, item.MaximumDurability, item.SocketProgress, item.ReduceDamage, item.Enchantment, item.Color, item.CompositionProgress, 0, 0, 0, 0, 0, 0, 0, 0));
            }

            m_pOwner.ReduceMoney(m_dwOwnerMoney);
            m_pOwner.ReduceEmoney(m_dwOwnerEmoney);
            m_pTarget.ReduceMoney(m_dwTargetMoney);
            m_pTarget.ReduceEmoney(m_dwTargetEmoney);

            m_pOwner.AwardMoney(m_dwTargetMoney);
            m_pOwner.AwardEmoney(m_dwTargetEmoney);
            m_pTarget.AwardMoney(m_dwOwnerMoney);
            m_pTarget.AwardEmoney(m_dwOwnerEmoney);

            m_pOwner.Send(ServerString.STR_TRADE_SUCCEED);
            m_pTarget.Send(ServerString.STR_TRADE_SUCCEED);
            return true;
        }

        public void CloseWindow()
        {
            var msg = new MsgTrade { Type = TradeType.HIDE_TABLE, Target = m_pTarget.Identity };
            m_pOwner.Send(msg);
            msg.Target = m_pOwner.Identity;
            m_pTarget.Send(msg);

            m_pTarget.Trade = null;
            m_pOwner.Trade = null;
        }

        public void CloseWindow(MsgTrade msg)
        {
            m_pOwner.Trade = null;
            if (m_pTarget != null)
            {
                msg.Type = TradeType.HIDE_TABLE;
                msg.Target = m_pTarget.Identity;
                m_pOwner.Send(msg);
                msg.Target = m_pOwner.Identity;
                m_pTarget.Owner.Send(msg);
                m_pTarget.Trade = null;
            }
        }

        public void ShowTable()
        {
            var msg = new MsgTrade
            {
                Type = TradeType.SHOW_TABLE,
                Target = m_pOwner.Identity
            };
            m_pTarget.Send(msg);
            msg.Target = m_pTarget.Identity;
            m_pOwner.Send(msg);
        }
    }
}