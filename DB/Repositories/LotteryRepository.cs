using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class LotteryRepository : HibernateDataRow<DbGameLottery>
    {
        public IList<DbGameLottery> FetchAllByColor(byte pColor)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbGameLottery>()
                    .Add(Restrictions.Eq("Color", pColor))
                    .List<DbGameLottery>();
        }

        public IList<DbGameLottery> FetchAllByRank(byte pRank)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbGameLottery>()
                    .Add(Restrictions.Eq("Rank", pRank))
                    .List<DbGameLottery>();
        }

        public IList<DbGameLottery> FetchAllRank5()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbGameLottery>()
                    .Add(Restrictions.Between("Rank", 5, 8))
                    .List<DbGameLottery>();
        }
    }
}