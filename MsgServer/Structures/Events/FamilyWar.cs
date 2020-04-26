// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Family War.cs
// Last Edit: 2017/01/09 13:13
// Created: 2017/01/09 12:57
namespace MsgServer.Structures.Events
{
    public sealed class FamilyWar
    {
        private uint[] m_dwMapIds =
        {
            1863, // TwinCityArena
            1864, // WindPlainArena
            1868, // PhoenixCastleArena
            1869, // MapleForestArena
            1873, // ApeCityArena
            1874, // LoveCanyonArena
            1878, // BirdIslandArena
            1879, // BirdIslandArena
            1883, // DesertCityArena
            1884  // DesertArena
        };

        public FamilyWar()
        {
            
        }

        public void OnTimer()
        {
            
        }
    }
}