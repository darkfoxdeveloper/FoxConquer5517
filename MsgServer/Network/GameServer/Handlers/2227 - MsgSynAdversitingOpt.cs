// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2227 - MsgSynAdversitingOpt.cs
// Last Edit: 2017/01/27 19:47
// Created: 2017/01/27 19:37

using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleSynAdvertisingOpt(Character pUser, MsgSynRecruitAdvertisingOpt pMsg)
        {
            switch (pMsg.Action)
            {
                case 1: // join syndicate
                {
                    ServerKernel.SyndicateRecruitment.JoinSyndicate(pUser, pMsg.EntityIdentity);
                    break;
                }
                case 2: // start or edit recruiting
                {
                    if (pUser.Syndicate == null)
                        return;

                    if (ServerKernel.SyndicateRecruitment.IsAdvertising(pUser.SyndicateIdentity))
                    {
                        ServerKernel.SyndicateRecruitment.SendEditScreen(pUser);
                        return;
                    }
                    MsgSynRecuitAdvertising pNewMsg = new MsgSynRecuitAdvertising();
                    pUser.Send(pNewMsg);
                    break;
                }
                default:
                {
                    ServerKernel.Log.SaveLog("MsgSynRecruitAdvertisingOpt::" + pMsg.Action);
                    break;
                }
            }
        }
    }
}