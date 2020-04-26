// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2076 - MsgQuench.cs
// Last Edit: 2016/12/07 04:54
// Created: 2016/12/07 04:46

using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public const uint REFINERY_DEFAULT_TIME = 7; // in days
        public const uint ARTIFACT_DEFAULT_TIME = 7; // in days

        public static void HandleQuench(Character pUser, MsgQuench packet)
        {
            Item target, source;

            if (!pUser.Inventory.Items.TryGetValue(packet.ItemIdentity, out target))
            {
                if (!pUser.Equipment.TryGetEquipment(packet.ItemIdentity, out target))
                    return;
            }

            if (!pUser.Inventory.Items.TryGetValue(packet.TargetIdentity, out source))
                return;

            switch (packet.Mode)
            {
                case PurificationMode.PURIFY:
                    {
                        uint Type = target.Type;
                        uint refineryType = source.Type;
                        DbRefinery refinery;
                        if (!ServerKernel.Refineries.TryGetValue(refineryType, out refinery))
                        {
                            pUser.Send("You attempted to insert a invalid refinery into your item.");
                            ServerKernel.Log.GmLog("Missing Refinery",
                                string.Format("Non existent refinery [{0}] requested by the client ", refineryType));
                            return;
                        }
                        switch (refinery.Itemtype.ToString().Length)
                        {
                            // Position
                            case 1:
                                {
                                    if (Calculations.GetItemPosition(Type) == (ItemPosition)refinery.Itemtype
                                        || (Calculations.GetItemPosition(Type) == ItemPosition.RIGHT_HAND && refinery.Itemtype == 5
                                        && target.GetSort() == ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND))
                                        break;
                                    pUser.Send("This refinery isn't compatible with the requested item.");
                                    return;
                                }
                            // Type
                            case 2:
                                {
                                    if (Type / 10000 == refinery.Itemtype)
                                        break;
                                    pUser.Send("This refinery isn't compatible with the requested item.");
                                    return;
                                }
                            // Specific item
                            case 3:
                                {
                                    if (Type / 1000 == refinery.Itemtype)
                                        break;
                                    // Ring/HeavyRing Exception :)
                                    if (refinery.Itemtype == 150 && Type / 1000 == 151)
                                        break;
                                    pUser.Send("This refinery isn't compatible with the requested item.");
                                    return;
                                }
                            default:
                                return;
                        }

                        if (!pUser.Inventory.Remove(source.Identity))
                            return;

                        target.RefineryExpire = (uint)(UnixTimestamp.Timestamp() + (60 * 60 * 24 * REFINERY_DEFAULT_TIME));
                        target.RefineryLevel = (byte)refinery.Level;
                        target.RefineryStart = (uint)UnixTimestamp.Timestamp();
                        target.RefineryType = refinery.Id;
                        pUser.Send(packet);
                        target.Save();
                        target.LoadRefinery();
                        target.SendPurification();
                        pUser.Send(
                            string.Format(SuccessRefineryMessage, PrepareMessage(refinery.Type, refinery.Power),
                                target.Itemtype.Name),
                            ChatTone.CENTER);
                        ServerKernel.Log.GmLog("refinery",
                            string.Format("User[{0}] Refined[{1}]Identity[{2}] Into[{3}]", pUser.Identity,
                                source.Type, source.Identity, target.Identity));
                        break;
                    }
                case PurificationMode.ITEM_ARTIFACT:
                    {
                        // Ok, be careful with this
                        if (source.Type < 800000
                            || source.Type >= 899999)
                        {
                            pUser.Send("This item cannot be refined.");
                            return;
                        }

                        if (target.Itemtype.ReqLevel < source.Itemtype.ReqLevel)
                        {
                            pUser.Send(
                                string.Format("Your item need to be at least level {0} to be refined with this artifact.",
                                    source.Itemtype.ReqLevel));
                            return;
                        }

                        if (source.Itemtype.RequireWeaponType > 0)
                        {
                            switch (source.Itemtype.RequireWeaponType.ToString().Length)
                            {
                                case 1: // sort
                                {
                                    if (source.Itemtype.RequireWeaponType != (int) target.GetSort())
                                        return;
                                    break;
                                }
                                case 3: // specific type
                                {
                                    if (source.Itemtype.RequireWeaponType != target.GetItemSubtype())
                                        return;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            switch (source.GetItemSubtype())
                            {
                                case 800: // weapons
                                {
                                    switch ((source.Type%1000)/100)
                                    {
                                        case 0:
                                        case 1: // single hand
                                            if (target.GetSort() != ItemSort.ITEMSORT_WEAPON_SINGLE_HAND
                                                && target.GetSort() != ItemSort.ITEMSORT_WEAPON_SINGLE_HAND2)
                                                return;
                                            break;
                                        case 2:
                                        case 3: // double hand
                                            if (target.GetItemSubtype() == 500
                                                || target.GetSort() != ItemSort.ITEMSORT_WEAPON_DOUBLE_HAND)
                                                return;
                                            break;
                                        case 4: // shield
                                            if (target.GetSort() != ItemSort.ITEMSORT_WEAPON_SHIELD)
                                                return;
                                            break;
                                        case 5: // backsword
                                            if (target.GetItemSubtype() != 421)
                                                return;
                                            break;
                                        case 6: // bow
                                            if (target.GetItemSubtype() != 500)
                                                return;
                                            break;
                                        case 7: // beads
                                            if (target.GetItemSubtype() != 610)
                                                return;
                                            break;
                                    }
                                    break;
                                }
                                case 820: // headwear
                                {
                                    if (Calculations.GetItemPosition(target.Type) != ItemPosition.HEADWEAR)
                                        return;
                                    break;
                                }
                                case 821: // neck
                                {
                                    if (Calculations.GetItemPosition(target.Type) != ItemPosition.NECKLACE)
                                        return;
                                    break;
                                }
                                case 822: // armor
                                {
                                    if (Calculations.GetItemPosition(target.Type) != ItemPosition.ARMOR)
                                        return;
                                    break;
                                }
                                case 823: // ring
                                {
                                    if (Calculations.GetItemPosition(target.Type) != ItemPosition.RING)
                                        return;
                                    break;
                                }
                                case 824: // boots
                                {
                                    if (Calculations.GetItemPosition(target.Type) != ItemPosition.BOOTS)
                                        return;
                                    break;
                                }
                            }
                        }

                        if (!pUser.Inventory.ReduceMeteors(source.Itemtype.MeteorAmount, target.Bound, false))
                        {
                            pUser.Send(string.Format("You need {0} meteors to refine with this kind of artifact.", source.Itemtype.MeteorAmount));
                            return;
                        }

                        if (!pUser.Inventory.Remove(source.Identity))
                        {
                            pUser.Send("What happened to your artifact?", ChatTone.TALK);
                            ServerKernel.Log.GmLog("artifact_error", string.Format("User[{0}] LostMeteors[{1}] ArtifactAdded[false]", pUser.Identity, source.Itemtype.MeteorAmount));
                            return;
                        }

                        uint extraTime = 0;
                        if (pUser.Owner.VipLevel < 4)
                            extraTime += pUser.Owner.VipLevel;
                        else
                            extraTime += 7;

                        target.ArtifactType = source.Type;
                        target.ArtifactStart = (uint)UnixTimestamp.Timestamp();
                        target.ArtifactExpire = target.ArtifactStart + ((ARTIFACT_DEFAULT_TIME + extraTime) * UnixTimestamp.TIME_SECONDS_DAY);
                        target.Save();
                        target.LoadArtifact();
                        target.SendPurification();
                        pUser.Send(packet);
                        ServerKernel.Log.GmLog("artifact", string.Format("User[{0}] Refined[{1}]Identity[{2}] Into[{3}]", pUser.Identity, source.Type, source.Identity, target.Identity));
                        break;
                    }
            }
        }

        public static string SuccessRefineryMessage = "You successfully added {0} to your {1}.";

        public static string PrepareMessage(uint type, uint value)
        {
            switch (type)
            {
                case 1:
                    return value + "% M-Defense";
                case 2:
                    return value + "% Critical-Strike";
                case 3:
                    return value + "% Skill Critical-Strike";
                case 4:
                    return value + "% Immunity";
                case 5:
                    return value + "% Breakthrough";
                case 6:
                    return value + "% Counteraction";
                case 7:
                    return value + "% Detoxication";
                case 8:
                    return value + "% Block";
                case 9:
                    return value + "% Penetration";
                case 10:
                    return value + "Intensification";
                case 11:
                    return value + "% Fire Resist";
                case 12:
                    return value + "% Water Resist";
                case 13:
                    return value + "% Wood Resist";
                case 14:
                    return value + "% Metal Resist";
                case 15:
                    return value + "% Earth Resist";
                default:
                    return "Error";
            }
        }
    }
}