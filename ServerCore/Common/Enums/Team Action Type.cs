// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Team Action Type.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum TeamActionType
    {
        CREATE = 0x00, // 0
        REQUEST_JOIN = 0x01, // 1
        LEAVE_TEAM = 0x02, // 2
        ACCEPT_INVITE = 0x03, // 3
        REQUEST_INVITE = 0x04, // 4
        ACCEPT_JOIN = 0x05, // 5
        DISMISS = 0x06, // 6
        KICK = 0x07, // 7
        JOIN_DISABLE = 0x08, // 8
        JOIN_ENABLE = 9,
        MONEY_ENABLE = 10,
        MONEY_DISABLE = 11,
        ITEM_ENABLE = 12,
        ITEM_DISABLE = 13,
        LEADER = 15, // 15
    }
}