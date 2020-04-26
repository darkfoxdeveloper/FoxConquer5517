// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2207 - MsgQualifyingRank.cs
// Last Edit: 2016/12/26 16:10
// Created: 2016/12/02 23:33

using System.Linq;
using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleQualifyingRank(Character pUser, MsgQualifyingRank pMsg)
        {
            switch (pMsg.RankType)
            {
                case ArenaRankType.QUALIFIER_RANK:
                {
                    var rank = ServerKernel.ArenaRecord.Values.Where(x => x.TodayWins > 0 || x.TodayLoses > 0)
                        .OrderByDescending(x => x.Points)
                        .ThenByDescending(x => x.TodayWins)
                        .ThenBy(x => x.TodayLoses)
                        .ToList();

                    pMsg.Count = (uint) rank.Count;

                    int idx = (pMsg.PageNumber-1)*10;
                    int nStartIdx = idx;

                    for (; idx < nStartIdx + 10; idx++)
                    {
                        if (idx >= rank.Count)
                            break;
                        var obj = rank[idx];
                        pMsg.AddPlayer((ushort) (idx+1), obj.PlayerName, 0, obj.Points, obj.Profession, obj.Level, obj.Identity);
                    }

                    //pMsg.Count = 8;
                    //pMsg.AddPlayer(1, "Player1", 0, 15000, 15, 140, 1000001);
                    //pMsg.AddPlayer(2, "Player2", 0, 15000, 55, 140, 1000002);
                    //pMsg.AddPlayer(3, "Player3", 0, 15000, 15, 140, 1000003);
                    //pMsg.AddPlayer(4, "Player4", 0, 15000, 45, 140, 1000004);
                    //pMsg.AddPlayer(5, "Player5", 0, 15000, 55, 140, 1000005);
                    //pMsg.AddPlayer(6, "Player6", 0, 15000, 45, 140, 1000006);
                    //pMsg.AddPlayer(7, "Player7", 0, 15000, 25, 140, 1000007);
                    //pMsg.AddPlayer(8, "Player8", 0, 15000, 15, 140, 1000008);
                    break;
                }
                case ArenaRankType.HONOR_HISTORY:
                {
                    var rank = ServerKernel.ArenaRecord.Values.Where(x => x.TotalHonorPoints > 0)
                        .OrderByDescending(x => x.TotalHonorPoints)
                        .ThenByDescending(x => x.TodayWins)
                        .ThenBy(x => x.TodayLoses)
                        .ToList();

                    pMsg.Count = (uint)rank.Count;

                    int idx = (pMsg.PageNumber-1) * 10;
                    int nStartIdx = idx;

                    for (; idx < nStartIdx + 10; idx++)
                    {
                        if (idx >= rank.Count)
                            break;
                        var obj = rank[idx];
                        pMsg.AddPlayer((ushort)(idx + 1), obj.PlayerName, 6004, obj.TotalHonorPoints, obj.Profession, obj.Level, obj.Identity);
                    }

                    //pMsg.Count = 8;
                    //pMsg.AddPlayer(1, "Player1", 6004, 15000, 15, 140, 33942209);
                    //pMsg.AddPlayer(2, "Player2", 6004, 15000, 55, 140, 33942209);
                    //pMsg.AddPlayer(3, "Player3", 6004, 15000, 15, 140, 33942209);
                    //pMsg.AddPlayer(4, "Player4", 6004, 15000, 45, 140, 33942209);
                    //pMsg.AddPlayer(5, "Player5", 6004, 15000, 55, 140, 33942209);
                    //pMsg.AddPlayer(6, "Player6", 6004, 15000, 45, 140, 33942209);
                    //pMsg.AddPlayer(7, "Player7", 6004, 15000, 25, 140, 33942209);
                    //pMsg.AddPlayer(8, "Player8", 6004, 15000, 15, 140, 33942209);
                    break;
                }
                default:
                    {
                        ServerKernel.Log.SaveLog(string.Format("MsgQualifyingRank::{0}", pMsg.RankType));
                        GamePacketHandler.Report(pMsg);
                        break;
                    }
            }
            pUser.Send(pMsg);
        }
    }
}