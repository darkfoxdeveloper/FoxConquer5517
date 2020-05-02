// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Chat Tone.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    /// <summary>
    /// This enumeration type defines the types of messages that can be sent. Each type defines how the message
    /// is displayed in the client (where the message is displayed and how the client reacts).
    /// </summary>
    public enum ChatTone : ushort
    {
        TALK = 2000,
        WHISPER = 2001,
        ACTION = 2002,
        TEAM = 2003,
        GUILD = 2004,
        FAMILY = 2006,
        SYSTEM = 2007,
        YELL = 2008,
        FRIEND = 2009,
        CENTER = 2011,
        TOP_LEFT = 2012,
        GHOST = 2013,
        SERVICE = 2014,
        TIP = 2015,
        WORLD = 2021,
        QUALIFIER = 2022,
        STUDY = 2024,
        CHARACTER_CREATION = 2100,
        LOGIN = 2101,
        SHOP = 2102,
        VENDOR_HAWK = 2104,
        WEBSITE = 2105,
        EVENT_RANKING = 2108,
        EVENT_RANKING_NEXT = 2109,
        OFFLINE_WHISPER = 2110,
        GUILD_ANNOUNCEMENT = 2111,
        AGATE = 2115,
        TRADE_BOARD = 2201,
        FRIEND_BOARD = 2202,
        TEAM_BOARD = 2203,
        GUILD_BOARD = 2204,
        OTHERS_BOARD = 2205,
        BROADCAST = 2500,
        MONSTER = 2600
    }
}