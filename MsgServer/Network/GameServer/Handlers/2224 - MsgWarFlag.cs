// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2224 - MsgWarFlag.cs
// Last Edit: 2016/12/02 09:06
// Created: 2016/12/02 09:05

using System;
using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleWarFlag(Character pUser, MsgWarFlag pMsg)
        {
            switch (pMsg.Type)
            {
                case WarFlagType.SYNDICATE_REWARD_TAB: // reward tab syndicate
                {
                    // if is war, offset 8 is true
                    //pMsg.AddPoints("Felipezudo");
                    pMsg.IsWar = ServerKernel.CaptureTheFlag.IsRunning;
                    pUser.Send(pMsg);
                    break;
                }
                case WarFlagType.BASE_RANK_REQUEST:
                {
                    ServerKernel.CaptureTheFlag.DeliverFlag(pUser);
                    break;
                }
                default:
                    Console.WriteLine("Unhandled 2224:{0}", pMsg.Type);
                    break;
            }
        }
    }
}