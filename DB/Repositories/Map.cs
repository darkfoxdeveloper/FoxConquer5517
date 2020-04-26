// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Map.cs
// File Created: 2015/09/21 14:51

using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public sealed class MapRepository : HibernateDataRow<DbMap>
    {
        public IList<DbMap> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbMap>()
                    .List<DbMap>();
        }

        public bool SaveOrUpdate(DbMap obj)
        {
            return base.SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbMap obj)
        {
            return base.TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}