// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2036 - MsgDataArray.cs
// Last Edit: 2016/11/24 11:00
// Created: 2016/11/24 11:00

using System;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleComposition(Character pRole, MsgDataArray pMsg)
        {
            Item target, source;
            if (pRole.Inventory.Items.TryGetValue(pMsg.MainIdentity, out target)
                && pRole.Inventory.Items.TryGetValue(pMsg.MinorIdentity, out source))
            {
                if (target.Identity == source.Identity)
                    return;

                // Sort check
                uint oldPlus = target.Plus;

                if (!pRole.Inventory.Items.ContainsKey(source.Identity))
                    return;

                switch (pMsg.Mode)
                {
                    case CompositionMode.CMP_ITEM_PLUS:
                        {
                            if (source.Type < SpecialItem.TYPE_STONE1 || source.Type > SpecialItem.TYPE_STONE8)
                            {
                                if (target.GetSort() != source.GetSort())
                                {
                                    pRole.Send(ServerString.STR_COMPOSITION_NOT_MATCH);
                                    return;
                                }
                                if (source.Plus == 0 || source.Plus > 8)
                                {
                                    pRole.Send(ServerString.STR_COMPOSITION_NOT_MINOR_ITEM);
                                    return;
                                }
                            }

                            if (target.Plus >= 12)
                            {
                                pRole.Send(ServerString.STR_COMPOSITION_MAX);
                                return;
                            }

                            target.CompositionProgress += StonePlus(source.Plus, false);
                            while (target.CompositionProgress >= GetAddLevelExp(target.Plus, false) && target.Plus < 12)
                            {
                                if (target.Plus < 12)
                                {
                                    target.CompositionProgress -= GetAddLevelExp(target.Plus, false);
                                    target.Plus++;
                                }
                                else
                                    target.CompositionProgress = 0;
                            }
                            break;
                        }
                    case CompositionMode.CMP_STEED_PLUS:
                        {
                            target.CompositionProgress += StonePlus(source.Plus, true);
                            while (target.CompositionProgress >= GetAddLevelExp(target.Plus, true) && target.Plus < 12)
                            {
                                if (target.Plus < 12)
                                {
                                    target.CompositionProgress -= GetAddLevelExp(target.Plus, true);
                                    target.Plus++;
                                }
                            }
                            break;
                        }
                    case CompositionMode.CMP_STEED_PLUS_NEW:
                        {
                            target.CompositionProgress += StonePlus(source.Plus, true);
                            while (target.CompositionProgress >= GetAddLevelExp(target.Plus, true) && target.Plus < 12)
                            {
                                if (target.Plus < 12)
                                {
                                    target.CompositionProgress -= GetAddLevelExp(target.Plus, true);
                                    target.Plus++;
                                }
                            }

                            int color1 = (int)target.SocketProgress;
                            int color2 = (int)source.SocketProgress;
                            int B1 = color1 & 0xFF;
                            int B2 = color2 & 0xFF;
                            int G1 = (color1 >> 8) & 0xFF;
                            int G2 = (color2 >> 8) & 0xFF;
                            int R1 = (color1 >> 16) & 0xFF;
                            int R2 = (color2 >> 16) & 0xFF;
                            int newB = (int)Math.Floor(0.9 * B1) + (int)Math.Floor(0.1 * B2);
                            int newG = (int)Math.Floor(0.9 * G1) + (int)Math.Floor(0.1 * G2);
                            int newR = (int)Math.Floor(0.9 * R1) + (int)Math.Floor(0.1 * R2);
                            target.SocketProgress = (uint)(newB | (newG << 8) | (newR << 16));

                            break;
                        }
                }

                if (!pRole.Inventory.Remove(source.Identity))
                    return;

                //pRole.AddMentorComposing((ushort)(StonePlus(source.Plus, false) / 10));

                if (oldPlus < target.Plus && target.Plus >= 6)
                    ServerKernel.SendMessageToAll(pRole.Gender == 1
                        ? string.Format(ServerString.STR_COMPOSITION_OVERPOWER_MALE, pRole.Name,
                            target.Itemtype.Name, target.Plus)
                        : string.Format(ServerString.STR_COMPOSITION_OVERPOWER_FEMALE, pRole.Name,
                            target.Itemtype.Name, target.Plus), ChatTone.TOP_LEFT);
                target.Save();
                pRole.Send(target.InformationPacket(true));
            }
        }

        private static ushort StonePlus(uint plus, bool steed)
        {
            switch (plus)
            {
                case 0: if (steed) return 1; return 0;
                case 1: return 10;
                case 2: return 40;
                case 3: return 120;
                case 4: return 360;
                case 5: return 1080;
                case 6: return 3240;
                case 7: return 9720;
                case 8: return 29160;
                default: return 0;
            }
        }

        private static ushort GetAddLevelExp(uint plus, bool steed)
        {
            switch (plus)
            {
                case 0: return 20;
                case 1: return 20;
                case 2: if (steed) return 90; return 80;
                case 3: return 240;
                case 4: return 720;
                case 5: return 2160;
                case 6: return 6480;
                case 7: return 19440;
                case 8: return 58320;
                case 9: return 2700;
                case 10: return 5500;
                case 11: return 9000;
                default: return 0;
            }
        }
    }
}