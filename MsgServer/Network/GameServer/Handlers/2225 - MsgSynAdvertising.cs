// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2225 - MsgSynAdvertising.cs
// Last Edit: 2017/01/27 19:59
// Created: 2017/01/27 19:58

using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleSynRecruitAdvertising(Character pUser, MsgSynRecuitAdvertising pMsg)
        {
            if (pUser.Syndicate == null || pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
                return;
            ServerKernel.SyndicateRecruitment.AddSyndicate(pUser, pMsg);
        }
    }
}