using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class MagicTypeRepository : HibernateDataRow<DbMagictype>
    {
        public IList<DbMagictype> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbMagictype>()
                    .List<DbMagictype>();
        }
    }
}