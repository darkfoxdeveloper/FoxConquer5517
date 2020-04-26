using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class RebirthRepository : HibernateDataRow<DbCqRebirth>
    {
        public IList<DbCqRebirth> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbCqRebirth>()
                    .List<DbCqRebirth>();
        }
    }
}
