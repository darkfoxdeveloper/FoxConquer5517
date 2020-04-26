using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class FlowerRepository : HibernateDataRow<DbFlower>
    {
        public IList<DbFlower> FetchByUser(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbFlower>()
                    .Add(Restrictions.Eq("PlayerIdentity", dwId))
                    .SetMaxResults(10)
                    .List<DbFlower>();
        }

        public IList<DbFlower> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbFlower>()
                    .List<DbFlower>();
        }

        public bool SaveOrUpdate(DbFlower obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbFlower obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}