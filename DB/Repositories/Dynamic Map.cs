// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - DynamicMapRepository.cs
// File Created: 2015/09/21 14:54

using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public sealed class DynamicMapRepository : HibernateDataRow<DbDynamicMap>
    {
        public IList<DbDynamicMap> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbDynamicMap>()
                    .List<DbDynamicMap>();
        }

        public bool SaveOrUpdate(DbDynamicMap obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbDynamicMap obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}