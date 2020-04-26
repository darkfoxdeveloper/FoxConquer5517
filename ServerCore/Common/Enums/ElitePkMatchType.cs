// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - ServerCore - ElitePkMatchType.cs
// Last Edit: 2017/02/15 18:42
// Created: 2017/02/15 18:41
namespace ServerCore.Common.Enums
{
    public enum ElitePkMatchType : ushort
    {
        INITIAL_LIST = 0,
        STATIC_UPDATE = 1,
        GUI_EDIT = 2,
        UPDATE_LIST = 3,
        REQUEST_INFORMATION = 4,
        STOP_WAGERS = 5,
        EPK_STATE = 6
    }
}