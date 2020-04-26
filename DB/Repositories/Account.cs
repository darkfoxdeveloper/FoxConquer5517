// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Account.cs
// File Created: 2015/08/03 12:59

using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class AccountRepository : HibernateDataRow<DbAccount>
    {
        /// <summary>
        /// This method gatters the user information by the name.
        /// </summary>
        /// <param name="szName">The account username.</param>
        /// <returns>Returns all information of the account. Null if not found.</returns>
        public DbAccount SearchByName(string szName)
        {
            using (var pSession = SessionFactory.LoginDatabase.OpenSession())
                return pSession.CreateCriteria<DbAccount>()
                        .Add(Restrictions.Eq("Username", szName))
                        .SetMaxResults(1)
                        .UniqueResult<DbAccount>();
        }

        /// <summary>
        /// Gatters the user information by his Key. (ID)
        /// </summary>
        /// <param name="nIdentity">The account ID.</param>
        /// <returns>Returns all information of the account. Null if not found.</returns>
        public DbAccount SearchByIdentity(uint nIdentity)
        {
            using (var pSession = SessionFactory.LoginDatabase.OpenSession())
                return pSession.CreateCriteria<DbAccount>()
                    .Add(Restrictions.Eq("Identity", nIdentity))
                    .SetMaxResults(1)
                    .UniqueResult<DbAccount>();
        }

        public bool SaveOrUpdate(DbAccount obj)
        {
            return base.SaveOrUpdate(obj, SessionFactory.LoginDatabase.OpenSession());
        }

        public bool Delete(DbAccount obj)
        {
            return base.TryDelete(obj, SessionFactory.LoginDatabase.OpenSession());
        }
    }
}
