// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Login Server Response.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum LoginServerResponse : byte
    {
        /// <summary>
        /// Sent by the LoginServer after a successful connection.
        /// </summary>
        LOGIN_SUCCESSFUL = 10,
        /// <summary>
        /// Sent by the LoginServer after an denied attempt of the MsgServer to login with a wrong username or password.
        /// </summary>
        LOGIN_DENIED_LOGIN,
        /// <summary>
        /// Sent by the LoginServer after denying the MsgServer to login due to a invalid IP Address.
        /// </summary>
        LOGIN_DENIED_IPADDRESS,
        /// <summary>
        /// Sent by the LoginServer after denying the MsgServer to login with a unauthorized Server Name.
        /// </summary>
        LOGIN_DENIED_SERVER_NOT_EXIST,
        /// <summary>
        /// Semt by the LoginServer after a manual disconnection.
        /// </summary>
        LOGIN_DENIED_DISCONNECTED
    }
}
