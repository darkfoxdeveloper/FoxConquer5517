// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - 1151 - MsgRank.cs
// Last Edit: 2017/02/15 17:47
// Created: 2017/02/15 17:36

using MsgServer.Structures;
using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleRank(Character pUser, MsgRank pMsg)
        {
            switch (pMsg.Mode)
            {
                case 2:
                {
                        if (pUser.Gender != 2)
                            return;

                        FlowerObject obj = ServerKernel.FlowerRanking.FetchUser(pUser.Identity);
                        if (obj == null)
                        {
                            ServerKernel.FlowerRanking.AddFlowers(FlowerType.RED_ROSE, 0, pUser.Identity);
                            obj = ServerKernel.FlowerRanking.FetchUser(pUser.Identity);
                            if (obj == null)
                                return;
                        }

                        string szFlowers = string.Format("{0} {1} {2} {3} {4} {5} {6} {7}"
                            , pUser.RedRoses, obj.RedRosesToday
                            , pUser.WhiteRoses, obj.WhiteRosesToday
                            , pUser.Orchids, obj.OrchidsToday
                            , pUser.Tulips, obj.TulipsToday);
                        pMsg.Mode = 1;
                        pMsg.SetFlowers(szFlowers);
                        pUser.Send(pMsg);
                        break;
                }
                default:
                    GamePacketHandler.Report(pMsg);
                    break;
            }
        }
    }
}