// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - ServerCore - Elite PK Match State.cs
// Last Edit: 2017/03/01 14:12
// Created: 2017/03/01 14:12

namespace ServerCore.Common.Enums
{
    public enum UserTournamentStatus
    {
        NONE = 0,
        FIGHTING = 1,
        LOST = 2,
        QUALIFIED = 3,
        WAITING = 4,
        BYE = 5,
        INACTIVE = 7,
        WON_MATCH = 8
    }
}