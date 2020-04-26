// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Flower Object.cs
// Last Edit: 2016/12/06 20:39
// Created: 2016/12/06 20:39

using DB.Entities;

namespace MsgServer.Structures
{
    public class FlowerObject
    {
        public FlowerObject(uint idUser, string szName)
        {
            m_dbFlower = new DbFlower
            {
                PlayerName = szName,
                PlayerIdentity = idUser
            };
        }

        public FlowerObject(DbFlower obj)
        {
            m_dbFlower = obj;
        }

        public uint PlayerIdentity { get { return m_dbFlower.PlayerIdentity; } }
        public string PlayerName { get { return m_dbFlower.PlayerName; } set { m_dbFlower.PlayerName = value; } }
        public uint RedRoses { get; set; }
        public uint RedRosesToday { get { return m_dbFlower.RedRoses; } set { m_dbFlower.RedRoses = value; } }
        public uint WhiteRoses { get; set; }
        public uint WhiteRosesToday { get { return m_dbFlower.WhiteRoses; } set { m_dbFlower.WhiteRoses = value; } }
        public uint Orchids { get; set; }
        public uint OrchidsToday { get { return m_dbFlower.Orchids; } set { m_dbFlower.Orchids = value; } }
        public uint Tulips { get; set; }
        public uint TulipsToday { get { return m_dbFlower.Tulips; } set { m_dbFlower.Tulips = value; } }

        private DbFlower m_dbFlower;
        public DbFlower Database { get { return m_dbFlower; } }
    }
}