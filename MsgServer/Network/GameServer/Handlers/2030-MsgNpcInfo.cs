// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2030 - MsgNpcInfo.cs
// Last Edit: 2016/12/07 00:29
// Created: 2016/12/07 00:29

using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleNpcInfo(Character pUser, MsgNpcInfo pMsg)
        {
            pUser.TemporaryString = string.Format("{0} {1} {2} {3} {4}"
                            , pMsg.MapX, pMsg.MapY, pMsg.Lookface, pMsg.Identity, pMsg.Kind);
            pUser.GameAction.ProcessAction(pUser.LastUsedItem, pUser, null, null, pUser.TemporaryString); 
        }
    }
}