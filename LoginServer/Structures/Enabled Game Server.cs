// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Enabled Game Server.cs
// Last Edit: 2016/11/23 09:59
// Created: 2016/11/23 09:59

using System.Runtime.InteropServices;

namespace LoginServer.Structures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4, Size = 52)]
    public struct EnabledGameServer
    {
        public EnabledGameServer(string szName, int nPort, string szUser, string szPass)
        {
            Name = szName;
            Port = nPort;
            Username = szUser;
            Password = szPass;
        }

        public readonly int Port;
        public readonly string Name;
        public readonly string Username;
        public readonly string Password;
    }
}