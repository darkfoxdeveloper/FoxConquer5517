using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class MentorAccessRepository : HibernateDataRow<DbMentorAccess>
    {
        public DbMentorAccess FetchByUser(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbMentorAccess>()
                    .Add(Restrictions.Eq("GuideIdentity", dwId))
                    .UniqueResult<DbMentorAccess>();
        }

        public bool SaveOrUpdate(DbMentorAccess obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbMentorAccess obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}