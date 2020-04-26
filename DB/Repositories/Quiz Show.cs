using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class QuizShowRepository : HibernateDataRow<DbGameQuiz>
    {
        public IList<DbGameQuiz> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbGameQuiz>()
                    .List<DbGameQuiz>();
        }

        public bool SaveOrUpdate(DbGameQuiz obj)
        {
            return base.SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbGameQuiz obj)
        {
            return base.TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}