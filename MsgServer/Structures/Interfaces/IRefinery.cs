// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - IRefinery.cs
// Last Edit: 2016/11/23 23:16
// Created: 2016/11/23 23:16

using ServerCore.Common.Enums;

namespace MsgServer.Structures.Interfaces
{
    public struct IRefinery
    {
        public bool Avaiable;
        public uint ItemIdentity;
        public uint RefineryType;
        public uint RefineryLevel;
        public uint RefineryPercent;
        public uint RefineryExpireTime;
        public uint RefineryStartTime;
        public uint StabilizationPoints;
        public RefineryType Mode;
    }
}