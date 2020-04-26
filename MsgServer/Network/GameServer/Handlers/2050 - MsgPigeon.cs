// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2050 - MsgPigeon.cs
// Last Edit: 2016/12/07 10:19
// Created: 2016/12/07 10:19

using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandlePigeon(Character pUser, MsgPigeon pMsg)
        {
            switch (pMsg.Type)
            {
                case 1: // next broadcasts list
                case 2: // next broadcast list (my)
                    {
                        ServerKernel.Broadcast.RequestNextPage(pUser, pMsg);
                        break;
                    }
                case 3: // send
                    {
                        var list = pMsg.ToList();
                        if (list.Count <= 0)
                            return;

                        var szMessage = list[0];

                        ServerKernel.Broadcast.Push(pUser, szMessage, true);
                        ServerKernel.Broadcast.RequestNextPage(pUser, pMsg);
                        break;
                    }
                case 4: // urgent 15 cps
                    {
                        ServerKernel.Broadcast.Addition(pUser, pMsg.DwParam, 15);
                        ServerKernel.Broadcast.RequestNextPage(pUser, pMsg);
                        break;
                    }
                case 5: // urgent 5 cps
                    {
                        ServerKernel.Broadcast.Addition(pUser, pMsg.DwParam, 5);
                        ServerKernel.Broadcast.RequestNextPage(pUser, pMsg);
                        break;
                    }
            }
        }
    }
}