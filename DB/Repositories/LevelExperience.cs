using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class LevelExperience : HibernateDataRow<DbLevexp>
    {
        public IList<DbLevexp> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbLevexp>()
                    .List<DbLevexp>();
        }
    }
}