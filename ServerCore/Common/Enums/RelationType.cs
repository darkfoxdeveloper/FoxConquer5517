// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Relation Type.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum AssociateAction : byte
    {
        REQUEST_FRIEND = 10,
        NEW_FRIEND = 11,
        SET_ONLINE_FRIEND = 12,
        SET_OFFLINE_FRIEND = 13,
        REMOVE_FRIEND = 14,
        ADD_FRIEND = 15,
        SET_ONLINE_ENEMY = 16,
        SET_OFFLINE_ENEMY = 17,
        REMOVE_ENEMY = 18,
        ADD_ENEMY = 19,
    }
}
