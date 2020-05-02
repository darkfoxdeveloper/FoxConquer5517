// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Login Player Amount.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum LoginPlayerAmountRequest : ushort
    {
        REQUEST_ONLINE_AMOUNT = 0,
        REQUEST_ONLINE_MAXAMOUNT,
        REPLY_ONLINE_AMOUNT = 10,
        REPLY_ONLINE_MAXAMOUNT
    }
}
