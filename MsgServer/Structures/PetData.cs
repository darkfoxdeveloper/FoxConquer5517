// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Pet Data.cs
// Last Edit: 2016/12/06 14:16
// Created: 2016/12/06 14:16

namespace MsgServer.Structures
{
    public class PetData
    {
        public uint OwnerIdentity;
        public uint OwnerType;
        public uint Generator;
        public uint Type;
        public string Name;
        public uint Life;
        public uint Mana;
        public uint MapIdentity;
        public ushort MapX;
        public ushort MapY;
        public object Data;
    }
}