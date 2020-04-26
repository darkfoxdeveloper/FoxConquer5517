// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Dynamic Npc.cs
// Last Edit: 2016/11/24 11:32
// Created: 2016/11/23 08:02

using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class DynamicNpcRepository : HibernateDataRow<DbDynamicNPC>
    {
        public IList<DbDynamicNPC> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbDynamicNPC>()
                    .List<DbDynamicNPC>();
        }

        public bool SaveOrUpdate(DbDynamicNPC obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbDynamicNPC obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}