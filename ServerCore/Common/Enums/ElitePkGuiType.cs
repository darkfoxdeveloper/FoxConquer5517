// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - ServerCore - ElitePkGuiType.cs
// Last Edit: 2017/02/15 18:50
// Created: 2017/02/15 18:50

namespace ServerCore.Common.Enums
{
    public enum ElitePkGuiType : ushort
    {
        GUI_TOP8_RANKING = 0,
        GUI_KNOCKOUT = 3,
        GUI_TOP8_QUALIFIER = 4,
        GUI_TOP4_QUALIFIER = 5,
        GUI_TOP2_QUALIFIER = 6,
        GUI_TOP3 = 7,
        GUI_TOP1 = 8,
        GUI_RECONSTRUCT_TOP = 9
    }
}