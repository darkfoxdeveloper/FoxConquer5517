// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Tile.cs
// Last Edit: 2016/11/23 10:27
// Created: 2016/11/23 10:26

using System.Runtime.InteropServices;

namespace MsgServer.Structures.World
{
    /// <summary>
    /// This structure encapsulates a tile from the floor's coordinate grid. It contains the tile access information
    /// and the elevation of the tile. The map's coordinate grid is composed of these tiles. The tile structure
    /// is not optimized by C#, and thus takes up 48 bits of memory (or 6 bytes).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2, Size = 6)]
    public struct Tile
    {
        public TileType Access;     // The access type for processing the tile.
        public short Elevation;     // The elevation of the tile on the map.
        public int Index;

        /// <summary>
        /// This structure encapsulates a tile from the floor's coordinate grid. It contains the tile access information
        /// and the elevation of the tile. The map's coordinate grid is composed of these tiles. The tile structure
        /// is not optimized by C#, and thus takes up 24 bits of memory (or 3 bytes).
        /// </summary>
        /// <param name="type">The access type for processing the tile.</param>
        /// <param name="elevation">The elevation of the tile on the map.</param>
        /// <param name="index">The index of a portal.</param>
        public Tile(TileType type, short elevation, int index = -1)
        {
            Access = type;
            Elevation = elevation;
            if (type == TileType.PORTAL)
                Index = index;
            else
                Index = -1;
        }
    }

    /// <summary> This enumeration type defines the access types for tiles. </summary>
    public enum TileType : byte
    {
        TERRAIN, NPC, MONSTER, PORTAL, ITEM, MARKET_SPOT, AVAILABLE
    }

    /// <summary> This enumeration type defines the types of scenery files used by the client. </summary>
    public enum SceneryType
    {
        SCENERY_OBJECT = 1,
        DDS_COVER = 4,
        EFFECT = 10,
        SOUND = 15
    }
}