// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Gem Type.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum SocketGem : byte
    {
        NORMAL_PHOENIX_GEM = 1,
        REFINED_PHOENIX_GEM = 2,
        SUPER_PHOENIX_GEM = 3,

        NORMAL_DRAGON_GEM = 11,
        REFINED_DRAGON_GEM = 12,
        SUPER_DRAGON_GEM = 13,

        NORMAL_FURY_GEM = 21,
        REFINED_FURY_GEM = 22,
        SUPER_FURY_GEM = 23,

        NORMAL_RAINBOW_GEM = 31,
        REFINED_RAINBOW_GEM = 32,
        SUPER_RAINBOW_GEM = 33,

        NORMAL_KYLIN_GEM = 41,
        REFINED_KYLIN_GEM = 42,
        SUPER_KYLIN_GEM = 43,

        NORMAL_VIOLET_GEM = 51,
        REFINED_VIOLET_GEM = 52,
        SUPER_VIOLET_GEM = 53,

        NORMAL_MOON_GEM = 61,
        REFINED_MOON_GEM = 62,
        SUPER_MOON_GEM = 63,

        NORMAL_TORTOISE_GEM = 71,
        REFINED_TORTOISE_GEM = 72,
        SUPER_TORTOISE_GEM = 73,

        NORMAL_THUNDER_GEM = 101,
        REFINED_THUNDER_GEM = 102,
        SUPER_THUNDER_GEM = 103,

        NORMAL_GLORY_GEM = 121,
        REFINED_GLORY_GEM = 122,
        SUPER_GLORY_GEM = 123,

        NO_SOCKET = 0,
        EMPTY_SOCKET = 255
    }
}