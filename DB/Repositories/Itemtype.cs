// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: FelipeVieira
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Itemtype.cs
// File Created: 2015/09/13 20:47

using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public sealed class ItemtypeRepository : HibernateDataRow<DbItemtype>
    {
        public IList<DbItemtype> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbItemtype>()
                    .List<DbItemtype>();
        }

        public IList<DbItemtype> FetchAllByType(uint dwType)
        {
            if (dwType > 1000)
                return null;
            uint min = dwType*1000;
            uint max = min + 999;
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbItemtype>()
                    .Add(Restrictions.Between("Type", min, max))
                    .SetMaxResults(1000)
                    .List<DbItemtype>();
        }

        public bool SaveOrUpdate(DbItemtype obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbItemtype obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}