// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Syndicate Rank.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum SyndicateRank : ushort
    {
        GUILD_LEADER = 1000,

        DEPUTY_LEADER = 990,
        HONORARY_DEPUTY_LEADER = 980,
        LEADER_SPOUSE = 920,

        MANAGER = 890,
        HONORARY_MANAGER = 880,

        TULIP_SUPERVISOR = 859,
        ORCHID_SUPERVISOR = 858,
        CP_SUPERVISOR = 857,
        ARSENAL_SUPERVISOR = 856,
        SILVER_SUPERVISOR = 855,
        GUIDE_SUPERVISOR = 854,
        PK_SUPERVISOR = 853,
        ROSE_SUPERVISOR = 852,
        LILY_SUPERVISOR = 851,
        SUPERVISOR = 850,
        HONORARY_SUPERVISOR = 840,

        STEWARD = 690,
        HONORARY_STEWARD = 680,
        DEPUTY_STEWARD = 650,
        DEPUTY_LEADER_SPOUSE = 620,
        DEPUTY_LEADER_AIDE = 611,
        LEADER_SPOUSE_AIDE = 610,
        AIDE = 602,

        TULIP_AGENT = 599,
        ORCHID_AGENT = 598,
        CP_AGENT = 597,
        ARSENAL_AGENT = 596,
        SILVER_AGENT = 595,
        GUIDE_AGENT = 594,
        PK_AGENT = 593,
        ROSE_AGENT = 592,
        LILY_AGENT = 591,
        AGENT = 590,
        SUPERVISOR_SPOUSE = 521,
        MANAGER_SPOUSE = 520,
        SUPERVISOR_AIDE = 511,
        MANAGER_AIDE = 510,

        TULIP_FOLLOWER = 499,
        ORCHID_FOLLOWER = 498,
        CP_FOLLOWER = 497,
        ARSENAL_FOLLOWER = 496,
        SILVER_FOLLOWER = 495,
        GUIDE_FOLLOWER = 494,
        PK_FOLLOWER = 493,
        ROSE_FOLLOWER = 492,
        LILY_FOLLOWER = 491,
        FOLLOWER = 490,
        STEWARD_SPOUSE = 420,

        SENIOR_MEMBER = 210,
        MEMBER = 200,

        NONE = 0
    }
}