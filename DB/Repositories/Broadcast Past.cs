using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class BroadcastPastRepository : HibernateDataRow<DbBroadcastLog>
    {
        public IList<DbBroadcastLog> FetchByUser(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbBroadcastLog>()
                    .Add(Restrictions.Eq("UserIdentity", dwId))
                    .SetMaxResults(10)
                    .List<DbBroadcastLog>();
        }

        public IList<DbBroadcastLog> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbBroadcastLog>()
                    .SetMaxResults(50)
                    .List<DbBroadcastLog>();
        }

        public bool SaveOrUpdate(DbBroadcastLog obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbBroadcastLog obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}