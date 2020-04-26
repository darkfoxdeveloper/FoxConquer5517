using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class MentorBattleLimitRepository : HibernateDataRow<MentorBattleLimit>
    {
        public IList<MentorBattleLimit> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<MentorBattleLimit>()
                    .List<MentorBattleLimit>();
        }
    }
}