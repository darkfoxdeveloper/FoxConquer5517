// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Items Processing.cs
// Last Edit: 2016/12/26 19:25
// Created: 2016/12/07 00:21

using System.Linq;
using System.Threading;
using MsgServer.Network;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using MsgServer.Structures.World;
using ServerCore.Common;

namespace MsgServer.Threads
{
    public static partial class ThreadHandler
    {
        public static void ItemTasks()
        {
            while (true)
            {
                foreach (Client user in ServerKernel.Players.Values.ToList())
                {
                    if (user.Character == null)
                        continue;
                    if (!user.Character.LoginComplete || user.Character.Inventory == null ||
                        user.Character.Equipment == null)
                        continue;

                    foreach (Item item in user.Character.Inventory.Items.Values)
                    {
                        if (!item.ArtifactIsActive() || !item.RefineryIsActive())
                            item.CheckForPurificationExpired();
                        if (item.ItemExpired())
                        {
                            user.Character.Inventory.Remove(item.Identity);
                            ServerKernel.Log.GmLog("item_expire",
                                string.Format("Item[{0}] type[{1}] owner[{2}] expired-at[{3}]", item.Identity, item.Type,
                                    item.PlayerIdentity, UnixTimestamp.Timestamp()));
                        }
                        item.TryUnlockItem();
                    }
                    foreach (Item item in user.Character.Equipment.Items.Values)
                    {
                        if (!item.ArtifactIsActive() || !item.RefineryIsActive())
                            item.CheckForPurificationExpired();
                        if (item.ItemExpired())
                        {
                            user.Character.Equipment.Remove(item.Position, Equipment.ItemRemoveMethod.DELETE);
                            ServerKernel.Log.GmLog("item_expire",
                                string.Format("Item[{0}] type[{1}] owner[{2}] expired-at[{3}]", item.Identity, item.Type,
                                    item.PlayerIdentity, UnixTimestamp.Timestamp()));
                        }
                        item.TryUnlockItem();
                    }
                    try
                    {
                        foreach (Warehouse wh in user.Character.Warehouses.Values.ToList())
                        {
                            foreach (Item item in wh.Items.Values.ToList())
                            {
                                if (!item.ArtifactIsActive() || !item.RefineryIsActive())
                                    item.CheckForPurificationExpired();
                                if (item.ItemExpired())
                                {
                                    wh.Delete(item.Identity);
                                    ServerKernel.Log.GmLog("item_expire",
                                        string.Format("Item[{0}] type[{1}] owner[{2}] expired-at[{3}]", item.Identity,
                                            item.Type, item.PlayerIdentity, UnixTimestamp.Timestamp()));
                                }
                            }
                        }
                    }
                    catch
                    {
                        
                    }
                }

                foreach (var detained in ServerKernel.DetainedObjects.Values.Where(x => x.IsItem()))
                    detained.OnTimer();

                foreach (Map map in ServerKernel.Maps.Values)
                    foreach (IScreenObject _object in map.GameObjects.Values)
                        if (_object is MapItem)
                        {
                            var item = _object as MapItem;
                            if (item.IsDisappear())
                            {
                                map.RemoveItem(item);
                                item.SelfDelete();
                            }
                        }
                Thread.Sleep(2000);
            }
        }
    }
}
