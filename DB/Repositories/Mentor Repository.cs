using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class MentorRepository : HibernateDataRow<DbMentor>
    {
        public DbMentor FetchMentor(uint idRole)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbMentor>()
                    .Add(Restrictions.Eq("StudentIdentity", idRole))
                    .UniqueResult<DbMentor>();
            
        }

        public IList<DbMentor> FetchApprentices(uint idMentor)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbMentor>()
                    .Add(Restrictions.Eq("GuideIdentity", idMentor))
                    .List<DbMentor>();
        }

        public bool SaveOrUpdate(DbMentor obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbMentor obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}