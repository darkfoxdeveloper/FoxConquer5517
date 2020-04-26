using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class KoBoardRepository : HibernateDataRow<DbSuperman>
    {
        public IList<DbSuperman> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbSuperman>()
                    .List<DbSuperman>();
        }

        public bool SaveOrUpdate(DbSuperman obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbSuperman obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}