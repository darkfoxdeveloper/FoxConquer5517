// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1066 - MsgMeteSpecial.cs
// Last Edit: 2017/01/13 02:50
// Created: 2016/12/29 21:30

using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleMeteSpecial(Character pUser, MsgMeteSpecial pMsg)
        {
            if (pUser.Metempsychosis < 2)
            {
                pUser.Send(ServerString.STR_METE_SPECIAL_METEMPSYCHOSIS);
                return;
            }

            if (pUser.Level < 110)
            {
                pUser.Send(ServerString.STR_METE_SPECIAL_LEVEL);
                return;
            }

            if (pUser.Gender == 1) // male
            {
                if (pMsg.Body < 3)
                {
                    pUser.Send(ServerString.STR_METE_SPECIAL_WRONG_GENDER);
                    return;
                }
            } 
            else if (pUser.Gender == 2) // female
            {
                if (pMsg.Body > 2)
                {
                    pUser.Send(ServerString.STR_METE_SPECIAL_WRONG_GENDER);
                    return;
                }
            }

            if (!pUser.Inventory.Remove(SpecialItem.OBLIVION_DEW, 1))
            {
                pUser.Send(ServerString.STR_METE_SPECIAL_NO_OBLIVION);
                return;
            }

            pUser.Reincarnate((ushort) pMsg.Profession, (ushort) pMsg.Body);
        }
    }
}