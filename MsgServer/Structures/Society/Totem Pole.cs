// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Totem Pole.cs
// Last Edit: 2016/11/25 02:15
// Created: 2016/11/25 00:09

using System.Collections.Concurrent;
using ServerCore.Common.Enums;

namespace MsgServer.Structures.Society
{
    public sealed class TotemPole
    {
        public TotemPoleType Type;
        public bool Locked = true;
        public ulong Donation;
        public byte BattlePower;
        public byte Enhance;
        public ConcurrentDictionary<uint, Totem> Items;

        public TotemPole(TotemPoleType type)
        {
            Type = type;
            Items = new ConcurrentDictionary<uint, Totem>();
        }
    }
}