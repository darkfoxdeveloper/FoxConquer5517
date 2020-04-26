// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Close Server Code.cs
// Last Edit: 2016/11/23 11:29
// Created: 2016/11/23 11:29
namespace ServerCore.Common.Enums
{
    public enum CloseServerCode
    {
        /// <summary>
        /// Used when there is no known reason for closing the server
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// Used when the server has been closed due to some exception
        /// </summary>
        EXCEPTION_THROWN,
        /// <summary>
        /// Used when the server has been closed due to some unhandled or unknown exception
        /// </summary>
        UNHANDLED_EXCEPTION,
        /// <summary>
        /// Used when the server itself makes a call for any reason
        /// </summary>
        SELF_CALL
    }
}