using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class SyndicateMembersRepository : HibernateDataRow<DbCqSynattr>
    {
        public IList<DbCqSynattr> FetchBySyndicate(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbCqSynattr>()
                    .Add(Restrictions.Eq("SynId", dwId))
                    .List<DbCqSynattr>();
        }

        public DbCqSynattr FetchByUser(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbCqSynattr>()
                    .Add(Restrictions.Eq("Id", dwId))
                    .SetMaxResults(1)
                    .UniqueResult<DbCqSynattr>();
        }

        public bool SaveOrUpdate(DbCqSynattr obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbCqSynattr obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}