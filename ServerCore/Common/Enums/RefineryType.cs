// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Refinery Type.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum RefineryType : uint
    {
        REF_MDEFENSE = 1,
        REF_CRITICAL_STRIKE = 2,
        REF_SCRITICAL_STRIKE = 3,
        REF_IMMUNITY = 4,
        REF_BREAKTHROUGH = 5,
        REF_COUNTERACTION = 6,
        REF_DETOXICATION = 7,
        REF_BLOCK = 8,
        REF_PENETRATION = 9, // REF_PENES = 9, HEUEHUEHUE BRBRBR
        REF_INTENSIFICATION = 10,
        REF_FIRE_RESIST = 11,
        REF_WATER_RESIST = 12,
        REF_WOOD_RESIST = 13,
        REF_METAL_RESIST = 14,
        REF_EARTH_RESIST = 15,
        REF_FINAL_MATTACK = 16, // Addicts +FinalMDamage
        REF_FINAL_MDAMAGE = 17, // Reduces -FinalMDefense
    }
}
