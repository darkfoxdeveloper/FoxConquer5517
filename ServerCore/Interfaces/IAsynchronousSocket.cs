// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - IAsynchronousSocket.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:52

using System;

namespace ServerCore.Interfaces
{
    /// <summary>
    /// An asynchronous socket interface encapsulates an asynchronous socket type, either client or server. This
    /// interface allows the passport to be used for server and client socket systems.
    /// </summary>
    public interface IAsynchronousSocket
    {
        string Name { get; set; }            // The name of the server.
        int FooterLength { get; set; }       // The length of the footer for each packet.
        string Footer { get; set; }          // The text for the footer at the end of each packet.
        void Disconnect(IAsyncResult result);// Disconnection method for the client / server.
    }
}
