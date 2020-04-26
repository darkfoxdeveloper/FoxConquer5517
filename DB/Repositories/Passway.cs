using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class PasswayRepository : HibernateDataRow<DbPassway>
    {
        public IList<DbPassway> GetAllPassways()
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                return sessionFactory
                    .CreateCriteria<DbPassway>()
                    .List<DbPassway>();
            }
        }
    }
}