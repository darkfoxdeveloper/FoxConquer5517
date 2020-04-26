using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public sealed class CqSyntotemRepository : HibernateDataRow<DbSyntotem>
    {
        public IList<DbSyntotem> GetBySyndicate(uint id)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                return sessionFactory
                    .CreateCriteria<DbSyntotem>()
                    .Add(Restrictions.Eq("Synid", id))
                    .List<DbSyntotem>();
            }
        }

        public bool SaveOrUpdate(DbSyntotem obj)
        {
            return base.SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbSyntotem obj)
        {
            return base.TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}