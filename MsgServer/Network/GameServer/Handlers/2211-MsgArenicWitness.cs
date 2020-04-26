// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - 2211 - MsgArenicWitness.cs
// Last Edit: 2017/02/06 11:14
// Created: 2017/02/06 11:13

using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleArenicWitness(Character pUser, MsgArenicWitness pMsg)
        {
            switch (pMsg.Action)
            {
                case 0:
                    {
                        if (!pUser.IsAlive)
                            return;
                        ServerKernel.ArenaQualifier.WatchMatch(pUser, pMsg.UserIdentity);
                        break;
                    }
                default:
                    {
                        ServerKernel.ArenaQualifier.QuitWatch(pUser);
                        break;
                    }
            }
        }
    }
}