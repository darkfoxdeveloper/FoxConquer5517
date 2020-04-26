// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1056 - MsgTrade.cs
// Last Edit: 2016/12/28 19:29
// Created: 2016/12/07 10:14

using System;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleTrade(Character pSender, MsgTrade pMsg)
        {
            switch (pMsg.Type)
            {
                #region Request
                case TradeType.REQUEST:
                    {
                        Client pUserTarget;
                        if (!ServerKernel.Players.TryGetValue(pMsg.Target, out pUserTarget))
                        {
                            return;
                        }

                        if (pUserTarget.Character == null)
                        {
                            pSender.Send(ServerString.STR_NO_TRADE_TARGET);
                            return;
                        }

                        Character pAcceptRole = pUserTarget.Character;

                        if (pAcceptRole.FetchTradeRequest(pSender.Identity))
                        {
                            pSender.Trade = pAcceptRole.Trade = new Trade(pSender, pAcceptRole);
                            pSender.Trade.ShowTable();
                            pAcceptRole.ClearTradeRequest();
                            return;
                        }

                        if (pUserTarget.Character == null) return;
                        Character pRoleTarget = pUserTarget.Character;

                        if (pRoleTarget.Trade == null)
                        {
                            pMsg.Target = pSender.Identity;
                            pRoleTarget.Send(pMsg);
                            pRoleTarget.SendRelation(pSender);
                            pSender.SetTradeRequest(pRoleTarget.Identity);
                            pSender.Send(ServerString.STR_TRADING_REQEST_SENT);
                            return;
                        }
                        else
                        {
                            pSender.Send(ServerString.STR_TARGET_TRADING);
                            return;
                        }
                        break;
                    }
                #endregion
                #region Time out
                case TradeType.TIME_OUT:
                {
                    GamePacketHandler.Report(pMsg);
                    //Console.WriteLine("Trade close due to timeout");
                    //pSender.Trade.CloseWindow(pMsg);
                    break;
                }
                #endregion
                #region Accept Trade
                case TradeType.ACCEPT:
                    {
                        pSender.Trade.AcceptTrade(pSender, pMsg);
                        break;
                    }
                #endregion
                #region Add Item
                case TradeType.ADD_ITEM:
                    {
                        pSender.Trade.AddItem(pMsg.Target, pSender);
                        break;
                    }
                #endregion
                #region Set Money
                case TradeType.SET_MONEY:
                    {
                        pSender.Trade.AddMoney(pMsg.Target, pSender, pMsg);
                        break;
                    }
                #endregion
                #region Set Conquer Points
                case TradeType.SET_CONQUER_POINTS:
                    {
                        pSender.Trade.AddEmoney(pMsg.Target, pSender, pMsg);
                        break;
                    }
                #endregion
                #region Close
                case TradeType.CLOSE:
                    {
                        if (pSender.Trade != null)
                            pSender.Trade.CloseWindow(pMsg);
                        break;
                    }
                #endregion
            }
        }
    }
}