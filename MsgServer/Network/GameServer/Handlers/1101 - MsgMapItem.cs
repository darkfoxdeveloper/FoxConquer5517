// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1101 - MsgMapItem.cs
// Last Edit: 2016/11/24 10:26
// Created: 2016/11/24 09:21

using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleMapItem(Character pRole, MsgMapItem pMsg)
        {
            if (!pRole.IsAlive/* || pRole.Trade != null*/)
                return;

            switch (pMsg.DropType)
            {
                case 3: // pick up
                {
                    if (pRole.SynchroPosition(pMsg.MapX, pMsg.MapY, 0))
                    {
                        pRole.PickMapItem(pMsg.Identity);
                    }
                    break;
                }
                default:
                {
                    ServerKernel.Log.SaveLog(string.Format("MsgMapItem::{0} no handle", pMsg.DropType));
                    break;
                }
            }
        }
    }
}