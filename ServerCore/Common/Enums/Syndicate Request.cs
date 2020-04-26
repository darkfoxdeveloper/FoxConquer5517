// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Syndicate Request.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum SyndicateRequest : uint
    {
        SYN_JOIN_REQUEST = 1,
        SYN_INVITE_REQUEST = 2,
        SYN_QUIT = 3,
        SYN_INFO = 6,
        SYN_ACCEPT_REQUEST = 29,
        SYN_ALLIED = 7,
        SYN_NEUTRAL1 = 8,
        SYN_ENEMIED = 9,
        SYN_NEUTRAL2 = 10,
        SYN_DONATE_SILVERS = 11,
        SYN_REFRESH = 12,
        SYN_DISBAND = 19,
        SYN_DONATE_CONQUER_POINTS = 20,
        SYN_SET_REQUIREMENTS = 24,
        SYN_SEND_REQUEST = 28,
        SYN_BULLETIN = 27,
        /// <summary>
        /// What
        /// </summary>
        SYN_DISCHARGE = 30,
        SYN_RESIGN = 32,
        /// <summary>
        /// The
        /// </summary>
        SYN_DISCHARGE2 = 33,
        SYN_PAID_PROMOTE = 34,
        SYN_EXTEND_PROMOTE = 35,
        /// <summary>
        /// Fuck?
        /// </summary>
        SYN_DISCHARGE3 = 36,
        SYN_PROMOTE = 37
    }
}
