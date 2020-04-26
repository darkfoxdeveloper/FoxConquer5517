using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class MonstersRepository : HibernateDataRow<DbMonstertype>
    {
        public IList<DbMonstertype> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbMonstertype>()
                    .List<DbMonstertype>();
        }
    }
}