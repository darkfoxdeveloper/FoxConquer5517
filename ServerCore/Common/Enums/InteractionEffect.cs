// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Interaction Effect.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50

using System;

namespace ServerCore.Common.Enums
{
    [Flags]
    public enum InteractionEffect : ushort
    {
        NONE = 0x0,
        BLOCK = 0x1, // 1
        PENETRATION = 0x2, // 2
        CRITICAL_STRIKE = 0x4, // 4
        BREAKTHROUGH = 0x2, // 8
        METAL_RESIST = 0x10, // 16
        WOOD_RESIST = 0x20, // 32
        WATER_RESIST = 0x40, // 64
        FIRE_RESIST = 0x80, // 128
        EARTH_RESIST = 0x100,
    }
}