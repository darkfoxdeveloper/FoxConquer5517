// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2209 - MsgQualifyingDetailInfo.cs
// Last Edit: 2016/12/19 17:37
// Created: 2016/12/02 23:48

using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleQualifyingDetailInfo(Character pUser, MsgQualifyingDetailInfo pMsg)
        {
            pUser.SendArenaStatus();
        }
    }
}