using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class TasksRespository : HibernateDataRow<DbTask>
    {
        public IList<DbTask> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbTask>()
                    .List<DbTask>();
        }
    }
}
