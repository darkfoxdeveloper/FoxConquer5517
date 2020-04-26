using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class SyndicateEnemiesRepository : HibernateDataRow<DbCqSynEnemy>
    {
        public IList<DbCqSynEnemy> FetchBySyndicate(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbCqSynEnemy>()
                    .Add(Restrictions.Eq("Synid", dwId))
                    .List<DbCqSynEnemy>();
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

        public void DeleteAntagonize(ushort syndicate0, ushort syndicate1)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                sessionFactory.CreateSQLQuery("CALL SynDeleteAntagonize (?,?);")
                    .AddEntity(typeof(DbCqSynEnemy))
                    .SetParameter(0, syndicate0)
                    .SetParameter(1, syndicate1)
                    .ExecuteUpdate();
            }
        }

        public bool SaveOrUpdate(DbCqSynEnemy obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbCqSynEnemy obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}