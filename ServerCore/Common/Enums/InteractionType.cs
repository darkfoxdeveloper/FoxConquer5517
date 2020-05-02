// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Interaction Type.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum InteractionType
    {
        ACT_ITR_NONE = 0,
        ACT_ITR_STEAL = 1,
        ACT_ITR_ATTACK = 2,
        ACT_ITR_HEAL = 3,
        ACT_ITR_POISON = 4,
        ACT_ITR_ASSASSINATE = 5,
        ACT_ITR_FREEZE = 6,
        ACT_ITR_UNFREEZE = 7,
        ACT_ITR_COURT = 8,
        ACT_ITR_MARRY = 9,
        ACT_ITR_DIVORCE = 10,
        ACT_ITR_PRESENT_MONEY = 11,
        ACT_ITR_PRESENT_ITEM = 12,
        ACT_ITR_SEND_FLOWERS = 13,
        ACT_ITR_KILL = 14,
        ACT_ITR_JOIN_GUILD = 15,
        ACT_ITR_ACCEPT_GUILD_MEMBER = 16,
        ACT_ITR_KICKOUT_GUILD_MEMBER = 17,
        ACT_ITR_PRESENT_POWER = 18,
        ACT_ITR_QUERY_INFO = 19,
        ACT_ITR_RUSH_ATTACK = 20,
        ACT_ITR_UNKNOWN21 = 21,
        ACT_ITR_ABORT_MAGIC = 22,
        ACT_ITR_REFLECT_WEAPON = 23,
        ACT_ITR_MAGIC_ATTACK = 24,
        ACT_ITR_UNKNOWN = 25,
        ACT_ITR_REFLECT_MAGIC = 26,
        ACT_ITR_DASH = 27,
        ACT_ITR_SHOOT = 28,
        ACT_ITR_QUARRY = 29,
        ACT_ITR_CHOP = 30,
        ACT_ITR_HUSTLE = 31,
        ACT_ITR_SOUL = 32,
        ACT_ITR_ACCEPT_MERCHANT = 33,
        ACT_ITR_INCREASE_JAR = 36,
        ACT_ITR_PRESENT_EMONEY = 39,
        ACT_ITR_COUNTER_KILL = 43,
        ACT_ITR_COUNTER_KILL_SWITCH = 44,
        ACT_ITR_FATAL_STRIKE = 45,
        ACT_ITR_INTERACT_REQUEST = 46,
        ACT_ITR_INTERACT_CONFIRM,
        ACT_ITR_INTERACT,
        ACT_ITR_INTERACT_UNKNOWN,
        ACT_ITR_INTERACT_STOP,
        ACT_ITR_AZURE_DMG = 55
    }
}