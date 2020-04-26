// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2067 - MsgGuideContribute.cs
// Last Edit: 2016/12/07 10:25
// Created: 2016/12/07 10:25

using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleGuideContribution(Character pRole, MsgContribute pMsg)
        {
            switch (pMsg.Type)
            {
                // check
                case GuideContributionType.CHECK_CONTRIBUTION:
                    {
                        pRole.FetchMentorAndApprentice();
                        pMsg.Composing = pRole.MentorComposition;
                        pMsg.HeavenBlessing = pRole.MentorBlessing;
                        pMsg.PrizeExperience = pRole.MentorExperience;
                        pRole.Send(pMsg);
                        break;
                    }
                case GuideContributionType.CLAIM_AMOUNT_HEAVEN_BLESSING: // this shit is the blessing
                    {
                        pRole.ClaimStudentBlessing();
                        break;
                    }
                case GuideContributionType.CLAIM_EXPERIENCE:
                    {
                        pRole.ClaimStudentExperience();
                        break;
                    }
                case GuideContributionType.CLAIM_COMPOSING:
                    {
                        pRole.ClaimStudentComposing();
                        break;
                    }
                default:
                    ServerKernel.Log.SaveLog("Unhandled packet type 2067:" + pMsg.Type, false, "guide_contribute", LogType.DEBUG);
                    return;
            }
        }
    }
}