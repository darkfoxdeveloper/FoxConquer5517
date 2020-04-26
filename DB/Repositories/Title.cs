// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Title.cs
// Last Edit: 2016/11/24 09:55
// Created: 2016/11/24 09:55

using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class CqTitleRepository : HibernateDataRow<DbTitle>
    {
        public IList<DbTitle> GetUserTitle(uint identity)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
                return sessionFactory
                    .CreateCriteria<DbTitle>()
                    .Add(Restrictions.Eq("Userid", identity))
                    .SetMaxResults(10)
                    .List<DbTitle>();
        }

        public bool SaveOrUpdate(DbTitle obj)
        {
            return base.SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbTitle obj)
        {
            return base.TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}