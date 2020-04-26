using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class MagictypeOpRepository : HibernateDataRow<DbMagictypeop>
    {
        public IList<DbMagictypeop> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbMagictypeop>()
                    .List<DbMagictypeop>();
        }
    }
}