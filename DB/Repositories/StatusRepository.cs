using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class StatusRepository : HibernateDataRow<DbStatus>
    {
        public DbStatus FindStatus(uint owner, uint status)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                return sessionFactory
                    .CreateCriteria<DbStatus>()
                    .Add(Restrictions.And(Restrictions.Eq("OwnerId", owner), Restrictions.Eq("Status", status)))
                    .SetMaxResults(1)
                    .UniqueResult<DbStatus>();
            }
        }

        public IList<DbStatus> LoadStatus(uint owner)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
                return sessionFactory
                    .CreateCriteria<DbStatus>()
                    .Add(Restrictions.Eq("OwnerId", owner))
                    .List<DbStatus>();
        }

        public bool SaveOrUpdate(DbStatus obj)
        {
            return base.SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbStatus obj)
        {
            return base.TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}