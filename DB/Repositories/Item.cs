// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Item.cs
// File Created: 2015/09/22 13:44

using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public sealed class ItemRepository : HibernateDataRow<DbItem>
    {
        public IList<DbItem> FetchByUser(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbItem>()
                    .Add(Restrictions.Eq("PlayerId", dwId))
                    .List<DbItem>();
        }

        public IList<DbItem> FetchInventory(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbItem>()
                    .Add(Restrictions.And(Restrictions.Eq("PlayerId", dwId), Restrictions.Eq("Position", 0)))
                    .SetMaxResults(40)
                    .List<DbItem>();
        }

        public IList<DbItem> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbItem>()
                    .List<DbItem>();
        }

        public IList<DbItem> FetchAll(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbItem>()
                    .Add(Restrictions.Eq("PlayerId", dwId))
                    .List<DbItem>();
        }

        public IList<DbItem> FetchAllByType(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbItem>()
                    .Add(Restrictions.Eq("Type", dwId))
                    .List<DbItem>();
        }

        public DbItem FetchByIdentity(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbItem>()
                    .Add(Restrictions.Eq("Id", dwId))
                    .SetMaxResults(1)
                    .UniqueResult<DbItem>();
        }

        public bool SaveOrUpdate(DbItem obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbItem obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}