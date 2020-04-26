using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class NpcRepository : HibernateDataRow<DbNpc>
    {
        public IList<DbNpc> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbNpc>()
                    .List<DbNpc>();
        }

        public bool SaveOrUpdate(DbNpc obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbNpc obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}