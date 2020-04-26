// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2205 - MsgQualifyingInteractive.cs
// Last Edit: 2016/12/22 18:48
// Created: 2016/12/02 23:32

using MsgServer.Structures.Entities;
using MsgServer.Structures.Qualifier;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleQualifyingInteractive(Character pUser, MsgQualifyingInteractive pMsg)
        {
            switch (pMsg.Type)
            {
                case ArenaType.ARENA_ICON_ON:
                {
                    if (ServerKernel.ArenaQualifier.Inscribe(pUser))
                    {
                        pUser.SendArenaStatus();
                    }
                    else
                    {
                        ServerKernel.ArenaQualifier.Uninscribe(pUser);
                        pUser.SendArenaStatus();
                    }
                    break;
                }
                case ArenaType.ARENA_ICON_OFF:
                {
                    if (ServerKernel.ArenaQualifier.Uninscribe(pUser))
                    {
                        pUser.SendArenaStatus();
                    }
                    break;
                }
                case ArenaType.ACCEPT_DIALOG:
                {
                    ArenaMatch pMatch = ServerKernel.ArenaQualifier.FindUser(pUser.Identity);

                    if (pMatch == null)
                    {
                        pUser.Send(ServerString.STR_ARENIC_NOT_JOINED);
                        if (ServerKernel.ArenaQualifier.IsWaitingMatch(pUser.Identity))
                        {
                            ServerKernel.ArenaQualifier.Uninscribe(pUser);
                        }
                        pUser.SendArenaStatus();
                        return;
                    }

                    if (pMsg.Option == 1) // accept
                    {
                        pMatch.Accept(pUser);
                        if (pMatch.ReadyToStart())
                        {
                            pMatch.Start();
                        }
                        pUser.SendArenaStatus();
                    } 
                    else if (pMsg.Option == 2) // give up
                    {
                        pMatch.GiveUp(pUser);
                        pUser.SendArenaStatus();
                    }
                    break;
                }
                case ArenaType.OPPONENT_GAVE_UP:
                {
                    ArenaMatch pMatch = ServerKernel.ArenaQualifier.FindUser(pUser.Identity);

                    if (pMatch == null)
                        return;

                    //Client pTarget;
                    //uint idTarget = pMatch.Identity1 == pUser.Identity ? pMatch.Identity2 : pMatch.Identity1;
                    //if (ServerKernel.Players.TryGetValue(idTarget, out pTarget))
                    {
                        if (pUser.Identity == pMatch.Identity1)
                            pMatch.Points2 = uint.MaxValue;
                        else
                            pMatch.Points1 = uint.MaxValue;
                        pMatch.Finish(/*pUser, pTarget.Character*/);
                        pUser.SendArenaStatus();
                    }
                    break;
                }
                case ArenaType.END_MATCH_JOIN: // rejoin
                {
                    if (ServerKernel.ArenaQualifier.Uninscribe(pUser))
                    {
                        ServerKernel.ArenaQualifier.Inscribe(pUser);
                        pUser.SendArenaStatus();
                    }
                    break;
                }
                default:
                {
                    pUser.Send(string.Format("The request {0} is not handled. Please contact the admin and provide an screenshot of the error.", pMsg.Type));
                    ServerKernel.Log.SaveLog(string.Format("MsgQualifyingInteractive::{0}", pMsg.Type), true);
                    GamePacketHandler.Report(pMsg);
                    break;
                }
            }
        }
    }
}