// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1015 - MsgName.cs
// Last Edit: 2016/11/24 11:11
// Created: 2016/11/24 11:10

using System.Linq;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleStringPacket(Character pUser, MsgName pMsg)
        {
            if (pUser == null) return;

            switch (pMsg.Action)
            {
                #region Query Mate
                case StringAction.QUERY_MATE:
                    {
                        Client target = null;
                        if (ServerKernel.Players.TryGetValue(pMsg.Identity, out target))
                        {
                            if (target.Character == null)
                                return;
                            pMsg.Append(target.Character.Mate);
                            pUser.Send(pMsg);
                        }
                        break;
                    }
                #endregion
                #region Whisper message
                case StringAction.WHISPER_WINDOW_INFO:
                    {
                        if (pMsg.Strings().Count <= 0) return;
                        string szTarget = pMsg.Strings()[0];
                        Client pTarget = ServerKernel.Players.Values.FirstOrDefault(x => x.Character != null && x.Character.Name == szTarget);
                        if (pTarget != null)
                        {
                            Character pTargetUser = pTarget.Character;
                            MsgName newmsg = new MsgName { Action = StringAction.WHISPER_WINDOW_INFO };
                            newmsg.Identity = pTargetUser.Identity;
                            newmsg.Append(pTargetUser.Name);
                            string msg = string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                                pTargetUser.Identity,
                                pTargetUser.Level,
                                pTargetUser.BattlePower,
                                "#" + pTargetUser.SyndicateName,
                                "#" + pTargetUser.FamilyName,
                                pTargetUser.Mate,
                                (byte)pTargetUser.NobilityRank,
                                pTargetUser.Gender == 0 ? "0" : "1");
                            newmsg.Append(msg);
                            pUser.Send(newmsg);
                        }
                        break;
                    }
                #endregion
                #region Guild
                case StringAction.GUILD:
                    {
                        //Syndicate syn;
                        //pMsg.Append(!ServerKernel.Syndicates.TryGetValue(pMsg.Identity, out syn) ? "Error" : syn.Name);
                        //pUser.Send(pMsg);
                        break;
                    }
                #endregion
                #region Members List
                case StringAction.MEMBER_LIST:
                    {
                        //if (pUser.Syndicate != null)
                        //    pUser.Syndicate.SendMembers(pUser);
                        break;
                    }
                #endregion
                default:
                    ServerKernel.Log.SaveLog("String packet missing handler " + pMsg.Action, true, LogType.WARNING);
                    break;
            }
        }
    }
}
