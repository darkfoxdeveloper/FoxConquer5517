// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2101 - MsgFactionRankInfo.cs
// Last Edit: 2016/12/02 03:24
// Created: 2016/12/02 03:06

using System;
using System.Collections.Generic;
using System.Linq;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Society;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        private static SyndicateRank[] m_pShowDuty =
        {
            SyndicateRank.MANAGER,
            SyndicateRank.SUPERVISOR,
            SyndicateRank.SILVER_SUPERVISOR,
            SyndicateRank.CP_SUPERVISOR,
            SyndicateRank.PK_SUPERVISOR,
            SyndicateRank.GUIDE_SUPERVISOR,
            SyndicateRank.ARSENAL_SUPERVISOR,
            SyndicateRank.ROSE_SUPERVISOR,
            SyndicateRank.LILY_SUPERVISOR,
            SyndicateRank.ORCHID_SUPERVISOR,
            SyndicateRank.TULIP_SUPERVISOR,
            SyndicateRank.STEWARD,
            SyndicateRank.DEPUTY_STEWARD,
            SyndicateRank.TULIP_AGENT,
            SyndicateRank.ORCHID_AGENT,
            SyndicateRank.CP_AGENT,
            SyndicateRank.ARSENAL_AGENT,
            SyndicateRank.SILVER_AGENT,
            SyndicateRank.GUIDE_AGENT,
            SyndicateRank.PK_AGENT,
            SyndicateRank.ROSE_AGENT,
            SyndicateRank.LILY_AGENT,
            SyndicateRank.AGENT,
            SyndicateRank.TULIP_FOLLOWER,
            SyndicateRank.ORCHID_FOLLOWER,
            SyndicateRank.CP_FOLLOWER,
            SyndicateRank.ARSENAL_FOLLOWER,
            SyndicateRank.SILVER_FOLLOWER,
            SyndicateRank.GUIDE_FOLLOWER,
            SyndicateRank.PK_FOLLOWER,
            SyndicateRank.ROSE_FOLLOWER,
            SyndicateRank.LILY_FOLLOWER,
            SyndicateRank.FOLLOWER,
            SyndicateRank.SENIOR_MEMBER
        };

        public static void HandleFactionRankInfo(Character pUser, MsgFactionRankInfo pMsg)
        {
            if (pUser.Syndicate == null || pUser.SyndicateMember == null)
                return;

            Syndicate pSyn = pUser.Syndicate;

            //PropertyInfo pInfo = null;

            List<SyndicateMember> memberList = new List<SyndicateMember>(10);
            switch (pMsg.Subtype)
            {
                case 0: // silver
                {
                    //pInfo = typeof (SyndicateMember).GetProperty("");
                    memberList = pSyn.Members.Values.OrderByDescending(x => x.SilverDonation).ToList();
                        break;
                    }
                case 1: // Emoney
                    {
                        memberList = pSyn.Members.Values.OrderByDescending(x => x.EmoneyDonation).ToList();
                        break;
                    }
                case 2: // guide
                    {
                        memberList = pSyn.Members.Values.OrderByDescending(x => x.GuideDonation).ToList();
                        break;
                    }
                case 3: // Pk
                    {
                        memberList = pSyn.Members.Values.OrderByDescending(x => x.PkDonation).ToList();
                        break;
                    }
                case 4: // Arsenal
                    {
                        memberList = pSyn.Members.Values.OrderByDescending(x => x.ArsenalDonation).ToList();
                        break;
                    }
                case 5: // Rose
                    {
                        memberList = pSyn.Members.Values.OrderByDescending(x => x.RedRoseDonation).ToList();
                        break;
                    }
                case 6: // orchid
                    {
                        memberList = pSyn.Members.Values.OrderByDescending(x => x.OrchidDonation).ToList();
                        break;
                    }
                case 7: // lily
                    {
                        memberList = pSyn.Members.Values.OrderByDescending(x => x.WhiteRoseDonation).ToList();
                        break;
                    }
                case 8: // tulip
                    {
                        memberList = pSyn.Members.Values.OrderByDescending(x => x.TulipDonation).ToList();
                        break;
                    }
                case 9: // total
                    {
                        memberList = pSyn.Members.Values.OrderByDescending(x => x.TotalDonation).ToList();
                        break;
                    }
                //case 10: // total donation
                //    {
                        
                //        break;
                //    }
                default:
                    Console.WriteLine("MsgFactionRankInfo::Subtype {0} not handled", pMsg.Subtype);
                    break;
            }

            //if (pInfo != null)
            //{
                
            //}

            uint count = 0;
            foreach (var member in memberList)
            //for (int i = 0; i < memberList.Count; i++)
            {
                //SyndicateMember member = memberList[i];
                if (count++ > 10)
                    break;
                pMsg.AddMember(member.Identity, member.Position, count, (int)member.SilverDonation, member.EmoneyDonation, member.PkDonation,
                    member.GuideDonation, member.ArsenalDonation, member.RedRoseDonation, member.WhiteRoseDonation, member.OrchidDonation,
                    member.TulipDonation, member.TotalDonation, member.Name);
            }
            //pMsg.Subtype = 4;
            pMsg.Count = (ushort) count;
            pMsg.MaxCount = (uint) Math.Min(pSyn.Members.Count, 30);
            pUser.Send(pMsg);
        }
    }
}