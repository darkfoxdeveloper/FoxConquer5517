using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class MonsterMagicRepository : HibernateDataRow<DbMonsterMagic>
    {
        public IList<DbMonsterMagic> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbMonsterMagic>()
                    .List<DbMonsterMagic>();
        }
    }
}