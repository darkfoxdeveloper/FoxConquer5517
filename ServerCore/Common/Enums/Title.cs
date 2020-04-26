// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Title.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum TitleAction : byte
    {
        HIDE_TITLE = 0,
        ADD_TITLE = 1,
        REMOVE_TITLE = 2,
        SELECT_TITLE = 3,
        QUERY_TITLE = 4
    }

    /// <summary>
    /// This enum is irrelevant, the user may create new ones on the client and make
    /// an action to add them. By the way, we will set this here, just in case we want
    /// to make a string or something with the name.
    /// </summary>
    public enum UserTitle : byte
    {
        NONE = 0,
        GOLDEN_RACER = 11,
        ELITE_PK_FIRST_LOW = 12,
        ELITE_PK_SECOND_LOW = 13,
        ELITE_PK_THIRD_LOW = 14,
        ELITE_PK_EIGHT_LOW = 15,
        ELITE_PK_FIRST_HIGH = 16,
        ELITE_PK_SECOND_HIGH = 17,
        ELITE_PK_THIRD_HIGH = 18,
        ELITE_PK_EIGHT_HIGH = 19,
        TOP_GUILD_LEADER = 34,
        TOP_DEPUTY_LEADER = 35
    }
}