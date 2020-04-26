using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class PortalsRepository : HibernateDataRow<DbPortal>
    {
        public DbPortal GetByIndex(uint mapid, uint index)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                return sessionFactory
                    .CreateCriteria<DbPortal>()
                    .Add(Restrictions.And(Restrictions.Eq("MapId", mapid), Restrictions.Eq("PortalIndex", index)))
                    .SetMaxResults(1)
                    .UniqueResult<DbPortal>();
            }
        }

        public IList<DbPortal> GetAllPortals()
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                return sessionFactory
                    .CreateCriteria<DbPortal>()
                    .List<DbPortal>();
            }
        }
    }
}