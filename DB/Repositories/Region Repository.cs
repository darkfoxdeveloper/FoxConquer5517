using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class RegionRepository : HibernateDataRow<DbRegion>
    {
        public IList<DbRegion> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbRegion>()
                    .List<DbRegion>();
        }
    }
}