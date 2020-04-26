using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class BusinessRepository : HibernateDataRow<DbBusiness>
    {
        public bool Exists(uint idSource, uint idTarget)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbBusiness>()
                    .Add(Restrictions.Or(Restrictions.And(Restrictions.Eq("Userid", idSource), Restrictions.Eq("Business", idTarget)),
                    Restrictions.And(Restrictions.Eq("Userid", idTarget), Restrictions.Eq("Business", idSource))))
                    .UniqueResult<DbBusiness>() != null;
        }

        public IList<DbBusiness> FetchByUser(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbBusiness>()
                    .Add(Restrictions.Eq("Userid", dwId))
                    .List<DbBusiness>();
        }

        /// <summary>
        /// This procedure will delete offline trade partnership.
        /// </summary>
        /// <param name="idTarget">The identity of the target (business)</param>
        /// <param name="idSource">The identity of the asker or offline user (Userid)</param>
        public void DeleteBusiness(uint idTarget, uint idSource)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                sessionFactory.CreateSQLQuery("CALL DeleteTradeBuddy (?,?);")
                    .AddEntity(typeof(DbSynAlly))
                    .SetParameter(0, idTarget)
                    .SetParameter(1, idSource)
                    .ExecuteUpdate();
            }
        }

        public bool SaveOrUpdate(DbBusiness obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbBusiness obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}
