// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Entity Direction.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    /// <summary> This enumeration type defines the directions an entity can face in. </summary>
    public enum FacingDirection : byte
    {
        SOUTH_WEST = 0,
        WEST = 1,
        NORTH_WEST = 2,
        NORTH = 3,
        NORTH_EAST = 4,
        EAST = 5,
        SOUTH_EAST = 6,
        SOUTH = 7
    }
}