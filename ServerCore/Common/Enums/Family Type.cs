// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Family Type.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum FamilyType : byte
    {
        INFO = 1,
        MEMBERS = 4,
        RECRUIT = 9,
        ACCEPT_RECRUIT = 10,
        JOIN = 11,
        ACCEPT_JOIN_REQUEST = 12,
        SEND_ENEMY = 13,
        ADD_ENEMY = 14,
        DELETE_ENEMY = 15,
        SEND_ALLY = 16,
        ADD_ALLY = 17,
        ACCEPT_ALLIANCE = 18,
        DELETE_ALLY = 20,
        TRANSFER_LEADER = 21,
        QUIT = 23,
        ANNOUNCE = 24,
        SET_ANNOUNCEMENT = 25,
        DEDICATE = 26,
        MY_CLAN = 29
    }

    public enum FamilyRank : ushort
    {
        CLAN_LEADER = 100,
        SPOUSE = 11,
        MEMBER = 10,
        NONE = 0
    }
}