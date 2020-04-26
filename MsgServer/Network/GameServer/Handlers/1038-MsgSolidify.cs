// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1038 - MsgSolidify.cs
// Last Edit: 2016/12/07 04:54
// Created: 2016/12/07 04:51

using System.Collections.Generic;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleSolidify(Character pUser, MsgSolidify packet)
        {
            switch (packet.Mode)
            {
                case SolidifyMode.ARTIFACT:
                    {
                        Item target;
                        if (pUser.Inventory.Items.TryGetValue(packet.TargetItem, out target))
                        {
                            if (target.ArtifactIsActive()
                                && !target.ArtifactIsPermanent())
                            {
                                List<uint> usableStones = new List<uint>();
                                List<uint> stones = packet.StoneItems;
                                uint sum = 0;
                                for (int i = 0; i < stones.Count; i++)
                                {
                                    Item item;
                                    if (pUser.Inventory.Items.TryGetValue(stones[i], out item))
                                    {
                                        if (item.Type == 723694)
                                        {
                                            usableStones.Add(stones[i]);
                                            sum += 10;
                                        }
                                        if (item.Type == 723695)
                                        {
                                            usableStones.Add(stones[i]);
                                            sum += 100;
                                        }
                                    }
                                }

                                if (sum >= target.ArtifactRemainingPoints())
                                {
                                    foreach (uint item in usableStones)
                                        if (!pUser.Inventory.Remove(item))
                                        {
                                            pUser.Send("Couldn't remove all stones from your inventory.");
                                            ServerKernel.Log.GmLog(@"syslog\error_log", string.Format("Couldn't remove [{0}] from user [{1}]. Item inexistent.", item, pUser.Identity));
                                            return;
                                        }
                                    target.ArtifactExpire = 0;
                                    target.ArtifactStabilization += sum;
                                    target.ArtifactStart = 0;
                                    target.Save();
                                    pUser.Send(packet);
                                    target.LoadArtifact();
                                    target.SendPurification();
                                    pUser.Send(packet);
                                    ServerKernel.Log.GmLog("artifact", string.Format("User:[{0}] has stabilized item:[{1}] with", pUser.Identity, target.Identity));
                                    foreach (uint item in usableStones)
                                        ServerKernel.Log.GmLog("artifact", string.Format("Stone identity: {0}", item));
                                }
                            }
                        }
                        break;
                    }
                case 0:
                    {
                        Item target;
                        if (pUser.Inventory.Items.TryGetValue(packet.TargetItem, out target))
                        {
                            if (target.RefineryIsActive()
                                && !target.RefineryIsPermanent())
                            {
                                List<uint> usableStones = new List<uint>();
                                List<uint> stones = packet.StoneItems;
                                uint sum = 0;
                                for (int i = 0; i < stones.Count; i++)
                                {
                                    Item item;
                                    if (pUser.Inventory.Items.TryGetValue(stones[i], out item))
                                    {
                                        if (item.Type == 723694)
                                        {
                                            usableStones.Add(stones[i]);
                                            sum += 10;
                                        }
                                        if (item.Type == 723695)
                                        {
                                            usableStones.Add(stones[i]);
                                            sum += 100;
                                        }
                                    }
                                }

                                if (sum >= target.RefineryRemainingPoints())
                                {
                                    foreach (uint item in usableStones)
                                        if (!pUser.Inventory.Remove(item))
                                        {
                                            pUser.Send("Couldn't remove all stones from your inventory.");
                                            ServerKernel.Log.GmLog(@"error_log", string.Format("Couldn't remove [{0}] from user [{1}]. Item inexistent.", item, pUser.Identity));
                                            return;
                                        }
                                    target.RefineryExpire = 0;
                                    target.RefineryStabilization += sum;
                                    target.RefineryStart = 0;
                                    target.Save();
                                    pUser.Send(packet);
                                    target.LoadRefinery();
                                    target.SendPurification();
                                    pUser.Send(packet);
                                    ServerKernel.Log.GmLog("refinery", string.Format("User:[{0}] has stabilized item:[{1}] with {2}", pUser.Identity, target.Identity, target.Type));
                                    foreach (uint item in usableStones)
                                        ServerKernel.Log.GmLog("refinery", string.Format("Stone identity: {0}", item));
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}