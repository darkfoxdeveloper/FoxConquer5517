// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Region Type.cs
// Last Edit: 2016/12/15 10:54
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public static class RegionType
    {
        public const int REGION_NONE = 0,
            REGION_CITY = 1,
            REGION_WEATHER = 2,
            REGION_STATUARY = 3,
            REGION_DESC = 4,
            REGION_GOBALDESC = 5,
            REGION_DANCE = 6, // data0: idLeaderRegion, data1: idMusic, 
            REGION_PK_PROTECTED = 7,
            REGION_FLAG_BASE = 8;
    }
}