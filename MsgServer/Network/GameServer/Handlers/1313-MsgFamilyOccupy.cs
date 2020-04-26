// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1313 - MsgFamilyOccupy.cs
// Last Edit: 2016/12/05 07:36
// Created: 2016/12/05 07:36

using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleFamilyOccupy(Character pUser, MsgFamilyOccupy pMsg)
        {
            switch (pMsg.Type)
            {
                case FamilyPromptType.REQUEST_NPC:
                    {
                        switch (pMsg.RequestNpc)
                        {
                            case 1005:
                                {
                                    pMsg.DailyPrize = 722458;
                                    pMsg.WeeklyPrize = 722454;
                                    break;
                                }
                            case 1006:
                                {
                                    pMsg.DailyPrize = 722458;
                                    pMsg.WeeklyPrize = 722454;
                                    break;
                                }
                            case 1007:
                                {
                                    pMsg.DailyPrize = 722478;
                                    pMsg.WeeklyPrize = 722474;
                                    break;
                                }
                            case 1008:
                                {
                                    pMsg.DailyPrize = 722478;
                                    pMsg.WeeklyPrize = 722474;
                                    break;
                                }
                            case 1009:
                                {
                                    pMsg.DailyPrize = 722473;
                                    pMsg.WeeklyPrize = 722469;
                                    break;
                                }
                            case 1010:
                                {
                                    pMsg.DailyPrize = 722473;
                                    pMsg.WeeklyPrize = 722469;
                                    break;
                                }
                            case 1011:
                                {
                                    pMsg.DailyPrize = 722463;
                                    pMsg.WeeklyPrize = 722459;
                                    break;
                                }
                            case 1012:
                                {
                                    pMsg.DailyPrize = 722463;
                                    pMsg.WeeklyPrize = 722459;
                                    break;
                                }
                            case 1013:
                                {
                                    pMsg.DailyPrize = 722468;
                                    pMsg.WeeklyPrize = 722464;
                                    break;
                                }
                            case 1014:
                                {
                                    pMsg.DailyPrize = 722468;
                                    pMsg.WeeklyPrize = 722464;
                                    break;
                                }
                        }
                        pUser.Send(pMsg);
                        break;
                    }
                default:
                    ServerKernel.Log.SaveLog("Family Prompt Packet " + pMsg.Type);
                    break;
            }
        }
    }
}