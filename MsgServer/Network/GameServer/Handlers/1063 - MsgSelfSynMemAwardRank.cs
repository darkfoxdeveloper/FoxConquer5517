// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1063 - MsgSelfSynMemAwardRank.cs
// Last Edit: 2016/12/26 16:46
// Created: 2016/12/02 10:21

using System;
using System.Linq;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Society;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleSelfSynMemAwardRank(Character pUser, MsgSelfSynMemAwardRank pMsg)
        {
            switch (pMsg.Type)
            {
                case 0: // CTF reward guilds ranking
                {
                    var rank = ServerKernel.Syndicates.Values.Where(x => x.MoneyPrize > 0 || x.EmoneyPrize > 0)
                        .OrderByDescending(x => x.MoneyPrize)
                        .ThenByDescending(x => x.EmoneyPrize)
                        .ToList();
                    int idx = 0;
                    for (; idx < 5; idx++)
                    {
                        if (idx >= rank.Count)
                            break;
                        pMsg.AddToRanking(rank[idx].Name, rank[idx].EmoneyPrize, rank[idx].MoneyPrize);
                    }
                    //pMsg.AddToRanking("Guild1", 50000, 500000000);
                    //pMsg.AddToRanking("Guild2", 40000, 450000000);
                    //pMsg.AddToRanking("Guild3", 30000, 400000000);
                    //pMsg.AddToRanking("Guild4", 20000, 300000000);
                    #region Testing Offsets
                    //pMsg.Count = 2;
                    //pMsg.WriteUInt(30, 30);
                    //pMsg.WriteLong(34, 34);
                    //pMsg.WriteString("NameOfGuild", 16, 42);
                    //pMsg.WriteUInt(82, 82);
                    //pMsg.WriteLong(86, 86);
                    //pMsg.WriteString("NameOfGuild2", 16, 94);
                    #endregion
                    pMsg.EmoneyPrize = pUser.Syndicate.EmoneyPrize;
                    pMsg.MoneyPrize = pUser.Syndicate.MoneyPrize;
                    pUser.Send(pMsg);
                    break;
                }
                case 1: // Syndicate last CTF reward ranking
                {
                    // offset 22 your syn next money reward
                    // offset 18 your syn next emoney reward
                    // offset 10 num page when receiving and result amount when sending
                    // pMsg.WriteUInt(uint.MaxValue, 22); 
                    // pMsg.WriteUInt(45546, 18);
                    pMsg.EmoneyPrize = pUser.Syndicate.EmoneyPrize;
                    pMsg.MoneyPrize = pUser.Syndicate.MoneyPrize;

                    var rank = ServerKernel.CaptureTheFlag.GetSyndicateMembers(pUser.SyndicateIdentity)
                        .Where(x => x.Value3 > 0)
                        .OrderByDescending(x => x.Value3).ToList();
                    int nRank = 0;
                    for (; nRank < rank.Count; nRank++)
                    {
                        if (rank[nRank].PlayerIdentity == pUser.Identity)
                            break;
                    }

                    if (rank.Count-1 >= nRank)
                    {
                        pMsg.AddToRanking((uint) (nRank + 1), pUser.Identity, pUser.Name, rank[nRank].Value5,
                        (uint) rank[nRank].Value6, (uint) rank[nRank].Value3);
                    }

                    int startIdx = (int) ((pMsg.Page-1)*4);
                    int idx = startIdx;
                    for (; idx < startIdx+4; idx++)
                    {
                        if (idx >= rank.Count)
                            break;
                        var res = rank[idx];
                        pMsg.AddToRanking((uint) (idx + 1), res.PlayerIdentity, res.PlayerName, res.Value5,
                            (uint) res.Value6, (uint) res.Value3);
                    }

                    //if (pMsg.ReadUShort(10) == 1)
                    //{
                    //    pMsg.AddToRanking(1, 1000, pUser.Name, 545468792, 32000, 8698);
                    //    pMsg.AddToRanking(2, 1000, "Ninja[PM]", 464564654, 32001, 8697);
                    //    pMsg.AddToRanking(3, 1000, "Archer[PM]", 42346688, 32002, 8696);
                    //    pMsg.AddToRanking(4, 1000, "Trojan[PM]", 12313436, 32003, 8695);
                    //}
                    //else if (pMsg.ReadUShort(10) == 2)
                    //{
                    //    pMsg.AddToRanking(5, 1000, "FireTaoist[PM]", 546463, 32004, 8694);
                    //    pMsg.AddToRanking(6, 1000, "WaterTaoist[PM]", 546463, 32004, 8694);
                    //    pMsg.AddToRanking(7, 1000, "Warrior[PM]", 546463, 32004, 8694);
                    //    pMsg.AddToRanking(8, 1000, "Trojan2[PM]", 546463, 32004, 8694);
                    //}
                    //else if (pMsg.ReadUShort(10) == 3)
                    //{
                    //    pMsg.AddToRanking(9, 1000, "Ninja2[PM]", 546463, 32004, 8694);
                    //    pMsg.AddToRanking(10, 1000, "WaterTao[PM]", 546463, 32004, 8694);
                    //    pMsg.AddToRanking(11, 1000, "WaterTaois[PM]", 546463, 32004, 8694);
                    //    pMsg.AddToRanking(12, 1000, "FiretTas[PM]", 546463, 32004, 8694);
                    //}

                    pMsg.ResultNum = (uint) rank.Count;
                    pUser.Send(pMsg);
                    break;
                }
                case 3: // set emoney prize
                {
                    Syndicate pSyn = pUser.Syndicate;
                    if (pSyn == null)
                        return;

                    if (pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return;
                    
                    if (pSyn.EmoneyPrize > 0 && pMsg.EmoneyPrize <= pSyn.EmoneyPrize)
                        return;

                    uint deductEmoney = pMsg.EmoneyPrize;
                    
                    if (pSyn.EmoneyPrize <= 0)
                    {
                        if (deductEmoney > pSyn.EmoneyDonation)
                        {
                            pUser.Send(ServerString.STR_SYNREWARD_NOT_ENOUGH_EMONEY);
                            return;
                        }
                    }
                    else
                    {
                        if (deductEmoney - pSyn.EmoneyPrize > pSyn.EmoneyDonation)
                        {
                            pUser.Send(ServerString.STR_SYNREWARD_NOT_ENOUGH_EMONEY);
                            return;
                        }
                        deductEmoney -= pSyn.EmoneyPrize;
                    }

                    pSyn.EmoneyDonation -= deductEmoney;
                    pSyn.EmoneyPrize = pMsg.EmoneyPrize;

                    string szEvent = ServerKernel.NextSyndicateEvent();
                    ServerKernel.SendMessageToAll(string.Format(ServerString.STR_SYNREWARD_SET, pUser.Name, pSyn.Name, pMsg.MoneyPrize, pMsg.EmoneyPrize, szEvent), ChatTone.TALK);
                    break;
                }
                case 4: // set money prize
                {
                    Syndicate pSyn = pUser.Syndicate;
                    if (pSyn == null)
                        return;

                    if (pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return;

                    if (pSyn.MoneyPrize > 0 && pMsg.MoneyPrize <= pSyn.MoneyPrize)
                        return;
                    
                    ulong deductMoney = (ulong)pMsg.MoneyPrize;

                    if (pSyn.MoneyPrize <= 0)
                    {
                        if (deductMoney > pSyn.SilverDonation)
                        {
                            pUser.Send(ServerString.STR_SYNREWARD_NOT_ENOUGH_MONEY);
                            return;
                        }
                    }
                    else
                    {
                        if (deductMoney - (ulong)pSyn.MoneyPrize > pSyn.SilverDonation)
                        {
                            pUser.Send(ServerString.STR_SYNREWARD_NOT_ENOUGH_MONEY);
                            return;
                        }
                        deductMoney -= (ulong)pSyn.MoneyPrize;
                    }

                    pSyn.SilverDonation -= deductMoney;
                    pSyn.MoneyPrize = pMsg.MoneyPrize;

                    string szEvent = ServerKernel.NextSyndicateEvent();
                    ServerKernel.SendMessageToAll(string.Format(ServerString.STR_SYNREWARD_SET, pUser.Name, pSyn.Name, pMsg.MoneyPrize, pMsg.EmoneyPrize, szEvent), ChatTone.TALK);
                    break;
                }
                case 5: // set money & emoney prize
                {
                    Syndicate pSyn = pUser.Syndicate;
                    if (pSyn == null)
                        return;

                    if (pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return;

                    if (pSyn.MoneyPrize > 0 && pMsg.MoneyPrize <= pSyn.MoneyPrize)
                        return;

                    if (pSyn.EmoneyPrize > 0 && pMsg.EmoneyPrize <= pSyn.EmoneyPrize)
                        return;

                    ulong deductMoney = (ulong) pMsg.MoneyPrize;
                    uint deductEmoney = pMsg.EmoneyPrize;

                    if (pSyn.MoneyPrize <= 0)
                    {
                        if (deductMoney > pSyn.SilverDonation)
                        {
                            pUser.Send(ServerString.STR_SYNREWARD_NOT_ENOUGH_MONEY);
                            return;
                        }
                    }
                    else
                    {
                        if (deductMoney - (ulong)pSyn.MoneyPrize > pSyn.SilverDonation)
                        {
                            pUser.Send(ServerString.STR_SYNREWARD_NOT_ENOUGH_MONEY);
                            return;
                        }
                        deductMoney -= (ulong) pSyn.MoneyPrize;
                    }

                    if (pSyn.EmoneyPrize <= 0)
                    {
                        if (deductEmoney > pSyn.EmoneyDonation)
                        {
                            pUser.Send(ServerString.STR_SYNREWARD_NOT_ENOUGH_EMONEY);
                            return;
                        }
                    }
                    else
                    {
                        if (deductEmoney - pSyn.EmoneyPrize > pSyn.EmoneyDonation)
                        {
                            pUser.Send(ServerString.STR_SYNREWARD_NOT_ENOUGH_EMONEY);
                            return;
                        }
                        deductEmoney -= pSyn.EmoneyPrize;
                    }

                    pSyn.SilverDonation -= deductMoney;
                    pSyn.EmoneyDonation -= deductEmoney;

                    pSyn.MoneyPrize = pMsg.MoneyPrize;
                    pSyn.EmoneyPrize = pMsg.EmoneyPrize;

                    string szEvent = ServerKernel.NextSyndicateEvent();
                    ServerKernel.SendMessageToAll(string.Format(ServerString.STR_SYNREWARD_SET, pUser.Name, pSyn.Name, pMsg.MoneyPrize, pMsg.EmoneyPrize, szEvent), ChatTone.TALK);
                    break;
                }
                case 8: // Current players from syndicate ranking on CTF
                {
                    // 18 is current exploit
                    //pMsg.WriteUInt(18, 18);
                    //pMsg.AddToRanking("PlayerName0", 500);
                    //pMsg.AddToRanking("PlayerName1", 400);
                    //pMsg.AddToRanking("PlayerName2", 300);
                    //pMsg.AddToRanking("PlayerName3", 200);
                    //pMsg.AddToRanking("PlayerName4", 100);
                    
                    var rank = ServerKernel.CaptureTheFlag.GetSyndicateMembers(pUser.SyndicateIdentity)
                        .Where(x => x.Value3 > 0)
                        .OrderByDescending(x => x.Value3)
                        .ToList();
                    int startIdx = (int)(pMsg.Page * 5);
                    int idx = startIdx;
                    for (; idx < startIdx + 5; idx++)
                    {
                        if (idx >= rank.Count)
                            break;
                        var res = rank[idx];
                        pMsg.AddToRanking(res.PlayerName, (uint) res.Value3);
                        if (res.PlayerIdentity == pUser.Identity)
                        {
                            pMsg.Exploits = (uint) res.Value3;
                        }
                    }
                    pMsg.ResultNum = (uint) rank.Count;
                    #region Testing Offsets
                    //pMsg.Count = 3;
                    //pMsg.WriteString("KOSKSOSK", 16, 30); // name
                    //pMsg.WriteUInt(46, 46); // exploit
                    //pMsg.WriteString("KOSKSOSK2", 16, 50);
                    //pMsg.WriteUInt(46, 66);
                    //pMsg.WriteString("KOSKSOSK3", 16, 70);
                    //pMsg.WriteUInt(46, 86);
                    #endregion
                    pUser.Send(pMsg);
                    break;
                }
                case 9: // CTF window (Arena)
                {
                    // offset 18 your syn point amount ctf running
                    ServerKernel.CaptureTheFlag.SendInterfaceRanking(pUser, pMsg);
                    break;
                }
                default:
                {
                    Console.WriteLine("Unhandled type 1063:{0}", pMsg.Type);
                    break;
                }
            }
        }
    }
}