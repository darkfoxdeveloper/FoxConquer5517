// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - General Data.cs
// Last Edit: 2016/11/24 10:26
// Created: 2016/11/23 10:47
namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        // Movement arrays for the eight possible directions:
        public static readonly sbyte[] WALK_X_COORDS = { 0, -1, -1, -1, 0, 1, 1, 1 };
        public static readonly sbyte[] WALK_Y_COORDS = { 1, 1, 0, -1, -1, -1, 0, 1 };
        public static readonly sbyte[] DELTA_WALK_X_COORDS = { 0, -2, -2, -2, 0, 2, 2, 2, 1, 0, -2, 0, 1, 0, 2, 0, 0, -2, 0, -1, 0, 2, 0, 1, 0 };
        public static readonly sbyte[] DELTA_WALK_Y_COORDS = { 2, 2, 0, -2, -2, -2, 0, 2, 2, 0, -1, 0, -2, 0, 1, 0, 0, 1, 0, -2, 0, -1, 0, 2, 0 };
    }
}