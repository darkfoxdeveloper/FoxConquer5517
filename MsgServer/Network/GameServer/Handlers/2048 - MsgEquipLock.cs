// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2048 - MsgEquipLock.cs
// Last Edit: 2016/11/24 10:57
// Created: 2016/11/24 10:56

using System;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleItemLock(Character pRole, MsgEquipLock pMsg)
        {
            Item item;
            if (pRole.Inventory.Items.TryGetValue(pMsg.Identity, out item)
                || pRole.Equipment.TryGetValue(pMsg.Identity, out item))
            {
                switch (pMsg.Mode)
                {
                    #region Lock item
                    case LockMode.REQUEST_LOCK:
                        {
                            if (item.IsLocked() && !item.IsUnlocking())
                            {
                                pRole.Send(ServerString.STR_EQUIP_LOCK_ALREADY_LOCKED);
                                return;
                            }

                            if (!item.CanBeLocked())
                            {
                                pRole.Send(ServerString.STR_EQUIP_LOCK_CANT_LOCK);
                                return;
                            }

                            item.LockTime = 1;
                            item.Save();
                            pRole.Send(pMsg);
                            pRole.Send(item.InformationPacket(true));
                            break;
                        }
                    #endregion
                    #region Unlock item
                    case LockMode.REQUEST_UNLOCK:
                        {
                            if (!item.IsLocked())
                            {
                                pRole.Send(ServerString.STR_EQUIP_LOCK_NOT_LOCKED);
                                return;
                            }

                            if (item.IsUnlocking())
                            {
                                pRole.Send(ServerString.STR_EQUIP_LOCK_ALREADY_UNLOCKING);
                                return;
                            }

                            item.LockTime = (uint)UnixTimestamp.Timestamp() + UnixTimestamp.TIME_SECONDS_DAY * 5;
                            item.Save();
                            pRole.Send(item.InformationPacket(true));
                            DateTime unlock = UnixTimestamp.ToDateTime(item.LockTime);
                            pMsg.Unknown = 3;
                            pMsg.Param = uint.Parse(unlock.ToString("yyyyMMdd"));
                            pRole.Send(pMsg);
                            break;
                        }
                    #endregion
                }
            }
        }
    }
}