// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Chat Flags.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50

using System;

namespace ServerCore.Common.Enums
{
    /// <summary>
    /// This enumeration type defines the style a message in the client can appear with. Messages can be sent with
    /// effects such as flash, scroll, and blast. Sending zero sends a normal message. Styles can be overlapped.
    /// </summary>
    [Flags]
    public enum ChatStyle : ushort
    {
        NORMAL = 0,
        SCROLL = 1 << 0,
        FLASH = 1 << 1,
        BLAST = 1 << 2
    }
}
