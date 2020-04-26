// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2206 - MsgQualifyingFightersList.cs
// Last Edit: 2016/12/02 23:34
// Created: 2016/12/02 23:32

using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleQualifyingFightersList(Character pUser, MsgQualifyingFightersList pMsg)
        {
            int i = 0;
            pMsg.PlayerAmount = ServerKernel.ArenaQualifier.Participants;
            foreach (var match in ServerKernel.ArenaQualifier.ArenaMatches)
            {
                if (i++ >= 6)
                    break;
                pMsg.AddMatch(match.User1.Identity, match.User1.Lookface, match.User1.Name, match.User1.Level,
                    match.User1.Profession, match.User1.ArenaQualifier.Ranking, match.User1.ArenaQualifier.Points,
                    match.User1.ArenaQualifier.TodayWins, match.User1.ArenaQualifier.TodayLoses,
                    match.User1.ArenaQualifier.HonorPoints,
                    match.User1.ArenaQualifier.TotalHonorPoints, match.User2.Identity, match.User2.Lookface,
                    match.User2.Name, match.User2.Level,
                    match.User2.Profession, match.User2.ArenaQualifier.Ranking, match.User2.ArenaQualifier.Points,
                    match.User2.ArenaQualifier.TodayWins, match.User2.ArenaQualifier.TodayLoses,
                    match.User2.ArenaQualifier.HonorPoints,
                    match.User2.ArenaQualifier.TotalHonorPoints);
            }
            pMsg.Showing = pMsg.MatchesCount;
            pUser.Send(pMsg);
            //pUser.SendArenaStatus();
        }
    }
}