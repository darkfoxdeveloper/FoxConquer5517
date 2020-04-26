// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2208 - MsgQualifyingSeasonRankList.cs
// Last Edit: 2016/12/19 19:10
// Created: 2016/12/02 23:33

using System.Linq;
using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleQualifyingSeasonRankList(Character pUser, MsgQualifyingSeasonRankList pMsg)
        {
            int amount = 0;
            foreach (var plr in ServerKernel.ArenaRecord.Values.Where(x => x.LastRanking > 0 
                    && x.LastSeasonPoints > 0)
                    .OrderBy(x => x.LastRanking)
                    .ThenByDescending(x => x.LastSeasonPoints))
            {
                if (amount++ >= 10)
                    break;
                pMsg.AddPlayer(plr.Identity, plr.PlayerName, plr.Lookface, plr.Level, plr.Profession, plr.LastSeasonPoints, plr.LastRanking,
                    plr.LastSeasonWins, plr.LastSeasonsLoses);
            }
            pUser.Send(pMsg);
        }
    }
}