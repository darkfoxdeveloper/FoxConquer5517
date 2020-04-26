// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Rejection Type.cs
// Last Edit: 2016/11/23 08:36
// Created: 2016/11/23 08:36
namespace ServerCore.Common.Enums
{
    /// <summary>
    /// This enumeration type defines the identities for error messages that can be displayed in the client from
    /// the login screen. These messages can only be sent and displayed in the account verification phase 
    /// (account server only). Most of these types are not supported by the target client (5187). 
    /// </summary>
    public enum RejectionType
    {
        INVALID_PASSWORD = 1,
        POINT_CARD_EXPIRED = 6,
        MONTHLY_CARD_EXPIRED = 7,
        SERVER_MAINTENANCE = 10,
        PLEASE_TRY_AGAIN_LATER = 11,
        ACCOUNT_BANNED = 12,
        SERVER_BUSY = 21,
        ACCOUNT_LOCKED = 22,
        ACCOUNT_NOT_ACTIVATED = 30,
        FAILED_TO_ACTIVATE_ACCOUNT = 31,
        MAXIMUM_LOGIN_ATTEMPTS = 51,
        DATABASE_ERROR = 64,
        INVALID_ACCOUNT = 57,
        SERVERS_NOT_CONFIGURED = 59,
        INVALID_AUTHENTICATION_PROTOCOL = 73
    }
}