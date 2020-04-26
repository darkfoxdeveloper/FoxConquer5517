// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Item Addition.cs
// File Created: 2015/10/02 23:23
// Last Update: 2015/10/02 23:23

using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public sealed class ItemAdditionRepository : HibernateDataRow<DbItemAddition>
    {
        public IList<DbItemAddition> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbItemAddition>()
                    .List<DbItemAddition>();
        }

        public bool SaveOrUpdate(DbItemAddition obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbItemAddition obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}