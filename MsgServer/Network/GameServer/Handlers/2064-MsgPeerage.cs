// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2064 - MsgPeerage.cs
// Last Edit: 2016/11/24 10:26
// Created: 2016/11/24 10:25

using System;
using System.Linq;
using DB.Entities;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleNobility(Character pRole, MsgPeerage pMsg)
        {
            switch (pMsg.Action)
            {
                #region List
                case NobilityAction.LIST:
                    {
                        int maxPages = 5;
                        int page = pMsg.Data;
                        int maxPerPage = 10;
                        int value = 0;
                        int amount = 0;

                        int minValue = page * maxPerPage;
                        int maxValue = (page + 1) * maxPerPage;

                        MsgPeerage nPacket = new MsgPeerage(32 + 48);
                        nPacket.Action = NobilityAction.LIST;
                        ushort maxCount = (ushort)Math.Ceiling((double)ServerKernel.Nobility.Count / 10);
                        nPacket.DataShort = (ushort)(maxCount > maxPages ? maxPages : maxCount);

                        foreach (DbDynaRankRec dynaRank in ServerKernel.Nobility.Values.OrderByDescending(x => x.Value))
                        {
                            if (value >= maxPages * maxPerPage)
                                break;

                            if (value < minValue)
                            {
                                value++;
                                continue;
                            }
                            if (value >= maxValue)
                                break;

                            uint lookface = 0;
                            Client pPlayer = ServerKernel.Players.Values.FirstOrDefault(x => x.Identity == dynaRank.UserIdentity);
                            if (pPlayer != null && pPlayer.Character != null)
                            {
                                lookface = pPlayer.Character.Body;
                            }

                            NobilityLevel rank = Nobility.GetNobilityLevel((uint)value);

                            nPacket.WriteNobilityData(dynaRank.UserIdentity, lookface, dynaRank.Username,
                                dynaRank.Value, rank, value);
                            value++;
                            amount++;
                        }

                        nPacket.DataHighLow = (ushort)amount;

                        nPacket.WriteByte((byte)amount, 28);
                        pRole.Send(nPacket);
                        break;
                    }
                #endregion
                #region Query Remaining Silver
                case NobilityAction.QUERY_REMAINING_SILVER:
                    {
                        var nPacket = new MsgPeerage
                        {
                            Action = NobilityAction.QUERY_REMAINING_SILVER,
                            DataLong = GetRemainingSilver((NobilityLevel)pMsg.Data,
                                pRole.NobilityDonation),
                            Data3 = 60, // The max amount of players in the ranking
                            Data4 = (uint)pRole.Nobility.Level
                        };
                        pRole.Send(nPacket);
                        break;
                    }
                #endregion
                #region Donate
                case NobilityAction.DONATE:
                    {
                        if (pRole.Level < 70)
                        {
                            pRole.Send(ServerString.STR_PEERAGE_DONATE_ERR_BELOW_LEVEL);
                            return;
                        }

                        long donation = pMsg.DataLong;

                        if (donation < 3000000)
                        {
                            pRole.Send(ServerString.STR_PEERAGE_DONATE_ERR_BELOW_UNDERLINE);
                            return;
                        }

                        if (donation > int.MaxValue)
                            donation = int.MaxValue;

                        NobilityLevel oldRank = pRole.NobilityRank;
                        int oldPos = pRole.Nobility.GetRanking;

                        // donate cps
                        if (pMsg.Data2 >= 1)
                        {
                            uint cps = (uint) donation/50000;
                            if (cps > pRole.BoundEmoney)
                            {
                                pRole.Send(ServerString.STR_NOT_ENOUGH_EMONEY2);
                                return;
                            }

                            if (!pRole.ReduceBoundEmoney(cps, true)) return;

                            pRole.Nobility.Donate(donation);
                            //pRole.Send("You can only donate silvers to the empire.");
                            //return;
                        }
                        if (pMsg.Data2 == 0) // donate silvers
                        {
                            if (!pRole.ReduceMoney(donation, true)) return;

                            pRole.Nobility.Donate(donation);
                        }

                        if (!ServerKernel.Nobility.ContainsKey(pRole.Identity))
                            ServerKernel.Nobility.TryAdd(pRole.Identity, pRole.Nobility.Database);

                        pRole.Nobility.UpdateRanking();
                        pRole.Nobility.UpdateLevel();

                        if (pRole.NobilityRank > NobilityLevel.EARL && oldPos > pRole.Nobility.GetRanking)
                            foreach (var plr in ServerKernel.Players.Values.Where(x => x.Character.NobilityRank > NobilityLevel.EARL))
                                plr.Character.Nobility.SendNobilityIcon();
                        else if (pRole.NobilityRank > NobilityLevel.EARL)
                            pRole.Nobility.SendNobilityIcon();

                        if (pRole.NobilityRank > oldRank)
                        {
                            switch (pRole.NobilityRank)
                            {
                                case NobilityLevel.KING:
                                    if (pRole.Gender == 1)
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_KING, pRole.Name), ChatTone.CENTER);
                                    else
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_QUEEN, pRole.Name), ChatTone.CENTER);
                                    break;
                                case NobilityLevel.PRINCE:
                                    if (pRole.Gender == 1)
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_DUKE, pRole.Name), ChatTone.CENTER);
                                    else
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_DUCHESS, pRole.Name), ChatTone.CENTER);
                                    break;
                                case NobilityLevel.DUKE:
                                    if (pRole.Gender == 1)
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_MARQUIS, pRole.Name), ChatTone.CENTER);
                                    else
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_MARQUISE, pRole.Name), ChatTone.CENTER);
                                    break;
                                case NobilityLevel.EARL:
                                    if (pRole.Gender == 1)
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_EARL, pRole.Name), ChatTone.TOP_LEFT);
                                    else
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_COUNTESS, pRole.Name), ChatTone.TOP_LEFT);
                                    break;
                                case NobilityLevel.BARON:
                                    if (pRole.Gender == 1)
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_VISCOUNT, pRole.Name), ChatTone.TOP_LEFT);
                                    else
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_VISCOUNTESS, pRole.Name), ChatTone.TOP_LEFT);
                                    break;
                                case NobilityLevel.KNIGHT:
                                    if (pRole.Gender == 1)
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_BARON, pRole.Name), ChatTone.TOP_LEFT);
                                    else
                                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_PEERAGE_PROMPT_BARONESS, pRole.Name), ChatTone.TOP_LEFT);
                                    break;
                            }
                        }
                        break;
                    }
                #endregion
            }
        }

        public static long GetRemainingSilver(NobilityLevel level, long donation)
        {
            switch (level)
            {
                case NobilityLevel.KING:
                    return (GetDonation(3) + 1) - donation;
                case NobilityLevel.PRINCE:
                    return (GetDonation(15) + 1) - donation;
                case NobilityLevel.DUKE:
                    return (GetDonation(50) + 1) - donation;
                case NobilityLevel.EARL:
                    return 200000000 - donation;
                case NobilityLevel.BARON:
                    return 100000000 - donation;
                case NobilityLevel.KNIGHT:
                    return 30000000 - donation;
                default:
                    return 0;
            }
        }

        public static long GetDonation(int position)
        {
            int ranking = 1;
            foreach (DbDynaRankRec dynaRank in ServerKernel.Nobility.Values.OrderByDescending(x => x.Value))
            {
                if (position == ranking)
                    return dynaRank.Value;
                ranking++;
            }
            return 0;
        }
    }
}