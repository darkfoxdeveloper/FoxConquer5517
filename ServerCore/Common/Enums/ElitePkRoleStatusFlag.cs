// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - ServerCore - ElitePkRoleStatusFlag.cs
// Last Edit: 2017/02/15 18:52
// Created: 2017/02/15 18:52
namespace ServerCore.Common.Enums
{
    public enum ElitePkRoleStatusFlag : ushort
    {
        EPKTFLAG_NONE = 0,
        EPKTFLAG_FIGHTING = 1,
        EPKTFLAG_LOST = 2,
        EPKTFLAG_QUALIFIED = 3,
        EPKTFLAG_WAITING = 4,
        EPKTFLAG_BYE = 5,
        EPKTFLAG_INACTIVE = 7,
        EPKTFLAG_WON_MATCH = 8
    }
}