// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1024 - MsgAllot.cs
// Last Edit: 2016/11/24 10:44
// Created: 2016/11/24 10:41

using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleAllot(Character pUser, MsgAllot pMsg)
        {
            int nTotal = (int) (pMsg.Strength + pMsg.Agility + pMsg.Vitality + pMsg.Spirit);
            if (nTotal > pUser.AdditionalPoints)
            {
                pUser.Send(ServerString.STR_CHEAT);
                return;
            }

            pUser.AdditionalPoints -= (ushort) nTotal;
            pUser.Strength += (ushort) pMsg.Strength;
            pUser.Agility += (ushort) pMsg.Agility;
            pUser.Vitality += (ushort) pMsg.Vitality;
            pUser.Spirit += (ushort) pMsg.Spirit;
        }
    }
}