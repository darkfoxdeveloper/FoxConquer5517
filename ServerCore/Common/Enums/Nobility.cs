// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Nobility.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum NobilityAction : uint
    {
        NONE = 0,
        DONATE = 1,
        LIST = 2,
        INFO = 3,
        QUERY_REMAINING_SILVER = 4
    }

    public enum NobilityLevel : byte
    {
        SERF = 0,
        KNIGHT = 1,
        BARON = 3,
        EARL = 5,
        DUKE = 7,
        PRINCE = 9,
        KING = 12
    }
}