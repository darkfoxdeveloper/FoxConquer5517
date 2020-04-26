// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - 2223 - MsgElitePKGameRankInfo.cs
// Last Edit: 2017/03/01 22:47
// Created: 2017/02/15 18:21

using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleElitePKGameRankInfo(Character pUser, MsgElitePKGameRankInfo pMsg)
        {
            switch (pMsg.Type)
            {
                case 0:
                {
                        pMsg.Append(0, "Felipe[PM]", 1321003, 1000001);
                        pMsg.Append(1, "Felipe[PM]", 1321003, 1000001);
                        pMsg.Append(2, "Felipe[PM]", 1321003, 1000001);
                        pMsg.Append(3, "Felipe[PM]", 1321003, 1000001);
                        //pMsg.Append(1, "*~Maybe~*", 3022001, 1000003);
                        //pMsg.Append(2, "Shakaos", 251003, 1000004);
                        //pMsg.Append(3, "Nero", 951003, 1000005);
                        //pMsg.Append(4, "Lawliet", 991003, 1000007);
                        //pMsg.Append(5, "Krool[GM]", 2892002, 1000006);
                        //pMsg.Append(6, "*Mime*", 271003, 1000008);
                        //pMsg.Append(7, "~bigg", 81003, 1000010);
                        pUser.Send(pMsg);
                        break;
                }
                default:
                {
                    break;
                }
            }
        }
    }
}