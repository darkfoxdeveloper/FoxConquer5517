// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - ISubClass.cs
// Last Edit: 2016/11/24 11:35
// Created: 2016/11/24 11:35

using DB.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Interfaces
{
    public struct ISubclass
    {
        public SubClasses Class { get { return (SubClasses)Database.Class; } set { Database.Class = (byte)value; } }
        public byte Level { get { return Database.Level; } set { Database.Level = value; } }
        public byte Promotion { get { return Database.Promotion; } set { Database.Promotion = value; } }

        public DbSubclass Database;
    }
}
