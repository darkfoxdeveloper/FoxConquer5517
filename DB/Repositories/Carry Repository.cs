// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - DB - Carry Repository.cs
// Last Edit: 2017/02/06 08:47
// Created: 2017/02/06 08:25

using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public sealed class CarryRepository : HibernateDataRow<DbCarry>
    {
        public IList<DbCarry> FetchByItem(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbCarry>()
                    .Add(Restrictions.Eq("ItemIdentity", dwId))
                    .List<DbCarry>();
        }

        public bool SaveOrUpdate(DbCarry obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbCarry obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}