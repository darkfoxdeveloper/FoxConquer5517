// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Bonus.cs
// Last Edit: 2017/01/07 01:45
// Created: 2016/12/29 21:30

using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class BonusRepository : HibernateDataRow<DbGameBonus>
    {
        public DbGameBonus CatchBonus(uint id)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbGameBonus>()
                        .Add(Restrictions.And(Restrictions.Eq("AccountIdentity", id), Restrictions.Eq("Flag", (byte)0)))
                        .SetMaxResults(1)
                        .UniqueResult<DbGameBonus>();
        }

        public int BonusAmount(uint id)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbGameBonus>()
                        .Add(Restrictions.And(Restrictions.Eq("AccountIdentity", id), Restrictions.Eq("Flag", (byte)0)))
                        .List<DbGameBonus>()
                        .Count;
        }

        public bool SaveOrUpdate(DbGameBonus obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbGameBonus obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}