// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Trade Mode.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum TradeType : byte
    {
        REQUEST = 1,
        CLOSE = 2,
        SHOW_TABLE = 3,
        HIDE_TABLE = 5,
        ADD_ITEM = 6,
        SET_MONEY = 7,
        SHOW_MONEY = 8,
        ACCEPT = 10,
        REMOVE_ITEM = 11,
        SHOW_CONQUER_POINTS = 12,
        SET_CONQUER_POINTS = 13,
        TIME_OUT = 17,
    }
}