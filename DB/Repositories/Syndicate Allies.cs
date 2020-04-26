using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class SyndicateAlliesRepository : HibernateDataRow<DbSynAlly>
    {
        public IList<DbSynAlly> FetchBySyndicate(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbSynAlly>()
                    .Add(Restrictions.Eq("Synid", dwId))
                    .List<DbSynAlly>();
        }

        public void ClearAlliesAndEnemies(ushort syndicate)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                sessionFactory.CreateSQLQuery("CALL SynClearAllyAndEnemy (?);")
                    .AddEntity(typeof(DbSynAlly))
                    .SetParameter(0, syndicate)
                    .ExecuteUpdate();
            }
        }

        public void DeleteRelationship(ushort syndicate0, ushort syndicate1)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                sessionFactory.CreateSQLQuery("CALL SynDeleteAlliance (?,?);")
                    .AddEntity(typeof(DbSynAlly))
                    .SetParameter(0, syndicate0)
                    .SetParameter(1, syndicate1)
                    .ExecuteUpdate();
            }
        }

        public bool SaveOrUpdate(DbSynAlly obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbSynAlly obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}
