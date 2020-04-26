// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Arena Repository.cs
// Last Edit: 2016/12/07 20:27
// Created: 2016/12/07 20:13

using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public sealed class ArenaRepository : HibernateDataRow<DbArena>
    {
        public IList<DbArena> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbArena>()
                    .List<DbArena>();
        }

        public DbArena GetById(uint id)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbArena>()
                    .Add(Restrictions.Eq("PlayerIdentity", id))
                    .SetMaxResults(1)
                    .UniqueResult<DbArena>();
        }

        public bool SaveOrUpdate(DbArena obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbArena obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}