// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - IKoCount.cs
// Last Edit: 2016/11/29 16:16
// Created: 2016/11/29 16:16

using DB.Entities;
using DB.Repositories;

namespace MsgServer.Structures.Interfaces
{
    public struct IKoCount
    {
        private DbSuperman m_dbSuper;

        public IKoCount(DbSuperman pSuper)
        {
            m_dbSuper = pSuper;
        }

        public uint Identity { get { return m_dbSuper.Identity; } }
        public string Name { get { return m_dbSuper.Name; } }

        public uint KoCount
        {
            get { return m_dbSuper.Amount; }
            set
            {
                m_dbSuper.Amount = value;
                Save();
            }
        }

        public bool Save()
        {
            return new KoBoardRepository().SaveOrUpdate(m_dbSuper);
        }

        public bool Delete()
        {
            return new KoBoardRepository().Delete(m_dbSuper);
        }
    }
}