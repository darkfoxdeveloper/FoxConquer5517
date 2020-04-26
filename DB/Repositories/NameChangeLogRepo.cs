// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Name Change Log Repo.cs
// Last Edit: 2016/12/29 17:25
// Created: 2016/12/29 17:24

using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public sealed class NameChangeLogRepo : HibernateDataRow<DbNameChangeLog>
    {
        public IList<DbNameChangeLog> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbNameChangeLog>()
                    .List<DbNameChangeLog>();
        }

        public IList<DbNameChangeLog> FetchByUser(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbNameChangeLog>()
                    .Add(Restrictions.Eq("UserIdentity", dwId))
                    .List<DbNameChangeLog>();
        }

        public bool SaveOrUpdate(DbNameChangeLog obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbNameChangeLog obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}