// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - IQuenchDrop.cs
// Last Edit: 2016/12/13 08:43
// Created: 2016/12/13 07:19

using System.Collections.Generic;

namespace MsgServer.Structures.Interfaces
{
    public class SpecialDrop
    {
        public uint MonsterIdentity { get; set; }
        public string MonsterName { get; set; }
        public byte DropNum { get; set; }
        public byte Level { get; set; }
        public byte LevelTolerance { get; set; }
        public uint DefaultAction { get; set; }
        public List<KeyValuePair<uint, ushort>> Actions { get; set; }
    }
}