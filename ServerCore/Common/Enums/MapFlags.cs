// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Map Flags.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50

using System;

namespace ServerCore.Common.Enums
{
    [Flags]
    public enum MapTypeFlags
    {
        NORMAL = 0,
        PK_FIELD = 1 << 0,//0x1 1
        CHANGE_MAP_DISABLE = 1 << 1,//0x2 2
        RECORD_DISABLE = 1 << 2,//0x4 4 
        PK_DISABLE = 1 << 3,//0x8 8
        BOOTH_ENABLE = 1 << 4,//0x10 16
        TEAM_DISABLE = 1 << 5,//0x20 32
        TELEPORT_DISABLE = 1 << 6, // 0x40 64
        GUILD_MAP = 1 << 7, // 0x80 128
        PRISON_MAP = 1 << 8, // 0x100 256
        WING_DISABLE = 1 << 9, // 0x200 512
        FAMILY = 1 << 10, // 0x400 1024
        MINE_FIELD = 1 << 11, // 0x800 2048
        PK_GAME = 1 << 12, // 0x1000 4098
        NEVER_WOUND = 1 << 13, // 0x2000 8196
        DEAD_ISLAND = 1 << 14, // 0x4000 16392
        SKILL_MAP = 1 << 17, // 0x20000 65568
        LINE_SKILL_ONLY = 1 << 18
    }
}