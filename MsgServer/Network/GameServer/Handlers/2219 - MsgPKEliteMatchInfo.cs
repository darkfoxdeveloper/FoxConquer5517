// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - 2219 - MsgPKEliteMatchInfo.cs
// Last Edit: 2017/02/15 18:56
// Created: 2017/02/15 18:56

using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandlePkEliteMatchInfo(Character pUser, MsgPKEliteMatchInfo pMsg)
        {
            switch (pMsg.Type)
            {
                case ElitePkMatchType.REQUEST_INFORMATION:
                {
                    //pMsg.OnGoing = true;
                    //pMsg.TimeLeft = 20;
                    //pMsg.InterfaceType = ElitePkGuiType.GUI_KNOCKOUT;
                    pUser.Send(pMsg);
                    break;
                }
                case ElitePkMatchType.INITIAL_LIST:
                {
                    //pUser.Send(pMsg);
                    //pMsg.Type = ElitePkMatchType.EPK_STATE;
                    //pMsg.OnGoing = true;
                    //pMsg.TimeLeft = 60;
                    //pMsg.InterfaceType = ElitePkGuiType.GUI_KNOCKOUT;
                    //for (int i = 0; i < 3; i++)
                    //{
                    //    UserMatchStatus[] match = new UserMatchStatus[2];
                    //    match[0] = new UserMatchStatus
                    //    {
                    //        Identity = (uint) (1000001+i),
                    //        Advance = false,
                    //        Cheer = 0,
                    //        Flag = ElitePkRoleStatusFlag.EPKTFLAG_FIGHTING,
                    //        Lookface = pUser.Lookface,
                    //        Name = string.Format("Test{0:0000}[PM]", i),
                    //        Points = 46354,
                    //        Wage = 10
                    //    };
                    //    match[1] = new UserMatchStatus
                    //    {
                    //        Identity = (uint) (1010001 + i),
                    //        Advance = false,
                    //        Cheer = 0,
                    //        Flag = ElitePkRoleStatusFlag.EPKTFLAG_FIGHTING,
                    //        Lookface = pUser.Lookface,
                    //        Name = string.Format("Test{0:1000}[PM]", i),
                    //        Points = 46354,
                    //        Wage = 15
                    //    };
                    //    pMsg.AppendMatch(10, 100, ElitePkRoleStatusFlag.EPKTFLAG_FIGHTING, match);
                    //}
                    //pMsg.MatchCount = 30;
                    //pUser.Send(pMsg);
                    //MsgElitePKGameRankInfo pRanking = new MsgElitePKGameRankInfo
                    //{
                    //    Type = 0,
                    //    Group = 0,
                    //    GroupStatus = 0,
                    //    Identity = pUser.Identity
                    //};
                    //pRanking.Append(0, "Felipe[PM]", 1321003, 1000001);
                    //pRanking.Append(1, "*~Maybe~*", 3022001, 1000003);
                    //pRanking.Append(2, "Shakaos", 251003, 1000004);
                    //pRanking.Append(3, "Nero", 951003, 1000005);
                    //pRanking.Append(4, "Lawliet", 991003, 1000007);
                    //pRanking.Append(5, "Krool[GM]", 2892002, 1000006);
                    //pRanking.Append(6, "*Mime*", 271003, 1000008);
                    //pRanking.Append(7, "~bigg", 81003, 1000010);
                    //pUser.Send(pMsg);
                    break;
                }
            }
        }
    }
}