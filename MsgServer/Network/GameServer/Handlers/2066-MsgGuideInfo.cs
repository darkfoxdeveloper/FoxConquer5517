// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2066 - MsgGuideInfo.cs
// Last Edit: 2016/12/07 10:23
// Created: 2016/12/07 10:23

using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleGuideInfo(Character pRole, MsgGuideInfo pMsg)
        {
            switch (pMsg.Type)
            {
                case 1: // request mentor
                    {
                        if (pRole.Mentor == null)
                            return;
                        pRole.Mentor.Send();
                        break;
                    }
                case 2: // request apprentices
                    {
                        if (pRole.Apprentices != null && pRole.Apprentices.Count > 0)
                        {
                            foreach (var appr in pRole.Apprentices.Values)
                                appr.Send();
                        }
                        break;
                    }
                case 4:
                    {
                        break;
                    }
                // not tested, probably asked after sending apprentices information to client
                // Mentor identity gets the value of the apprentices identity
                // which means that offset 8 isn't really the mentor identity, but a general identity :)
                // what would I call this? what about offset 12 that carries the student identity?
                default:
                    ServerKernel.Log.SaveLog("Unhandled packet type 2066:" + pMsg.Type, true, "guide", LogType.DEBUG);
                    GamePacketHandler.Report(pMsg);
                    return;
            }
        }
    }
}
