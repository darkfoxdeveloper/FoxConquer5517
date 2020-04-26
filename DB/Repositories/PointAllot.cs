using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class PointAllotRepository : HibernateDataRow<DbPointAllot>
    {
        public IList<DbPointAllot> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbPointAllot>()
                    .List<DbPointAllot>();
        }

        public bool SaveOrUpdate(DbPointAllot obj)
        {
            return base.SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbPointAllot obj)
        {
            return base.TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}