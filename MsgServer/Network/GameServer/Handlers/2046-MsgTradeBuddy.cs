// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2046 - MsgTradeBuddy.cs
// Last Edit: 2016/12/28 19:29
// Created: 2016/12/07 10:17

using DB.Repositories;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleTradeBuddy(Character pRole, MsgTradeBuddy pMsg)
        {
            switch (pMsg.Type)
            {
                #region Request Partnership
                case TradePartnerType.REQUEST_PARTNERSHIP:
                    {
                        Client pTargetUser;
                        if (!ServerKernel.Players.TryGetValue(pMsg.Identity, out pTargetUser))
                        {
                            pRole.Send(ServerString.STR_TRADE_BUDDY_NOT_FOUND);
                            return;
                        }

                        Character pRoleTarget = pTargetUser.Character;
                        if (pRoleTarget == null) return;

                        if (pRole.FetchTradeBuddyRequest(pRoleTarget.Identity))
                        {
                            if (pRoleTarget.TradePartners.ContainsKey(pRole.Identity)
                                || pRole.TradePartners.ContainsKey(pRoleTarget.Identity))
                            {
                                pRole.Send(ServerString.STR_TRADE_BUDDY_ALREADY_ADDED);
                                return;
                            }

                            var pSender = new TradePartner(pRole);
                            var pTarget = new TradePartner(pRoleTarget);
                            if (!pSender.Create(pRoleTarget.Identity)
                                || !pTarget.Create(pRole.Identity))
                            {
                                pRole.Send(ServerString.STR_TRADE_BUDDY_SOMETHING_WRONG);
                                return;
                            }

                            pRole.Screen.Send(string.Format("{0} and {1} accepted a trade partnership that will be approved within 3 days.", pRole.Name, pRoleTarget.Name), true);
                        }
                        else
                        {
                            pRoleTarget.SetTradeBuddyRequest(pRole.Identity);
                            pMsg.Identity = pRole.Identity;
                            pMsg.Name = pRole.Name;
                            pTargetUser.Send(pMsg);
                            pTargetUser.Character.SendRelation(pRole);
                        }
                        break;
                    }
                #endregion
                #region Reject Request
                case TradePartnerType.REJECT_REQUEST:
                    {
                        Client pTarget;
                        if (ServerKernel.Players.TryGetValue(pMsg.Identity, out pTarget))
                        {
                            pMsg.Identity = pRole.Identity;
                            pMsg.Name = pRole.Name;
                            pMsg.Online = true;
                            pTarget.Character.Send(pMsg);
                        }
                        break;
                    }
                #endregion
                #region Break partnership
                case TradePartnerType.BREAK_PARTNERSHIP:
                    {
                        TradePartner pTarget;
                        if (pRole.TradePartners.TryRemove(pMsg.Identity, out pTarget))
                        {
                            pTarget.Delete();
                            if (pTarget.TargetOnline)
                            {
                                TradePartner trash;
                                pTarget.Owner.TradePartners.TryRemove(pRole.Identity, out trash);
                                trash.Delete();
                                pTarget.Owner.Send(string.Format(ServerString.STR_TRADE_BUDDY_BROKE_PARTNERSHIP0, pRole.Name));
                            }
                            else
                            {
                                new BusinessRepository().DeleteBusiness(pRole.Identity, pTarget.OwnerIdentity);
                            }
                            pRole.Send(string.Format(ServerString.STR_TRADE_BUDDY_BROKE_PARTNERSHIP1, pTarget.Name));
                        }
                        break;
                    }
                #endregion
                default:
                    {
                        ServerKernel.Log.SaveLog("Missing packet handler for type 2046:" + pMsg.Type, true, LogType.WARNING);
                        GamePacketHandler.Report(pMsg);
                        break;
                    }
            }
        }
    }
}