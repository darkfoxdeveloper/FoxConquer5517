using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class GoodsRepository : HibernateDataRow<DbGoods>
    {
        public IList<DbGoods> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbGoods>()
                    .List<DbGoods>();
        }
    }
}