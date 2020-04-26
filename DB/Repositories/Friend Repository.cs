using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class FriendRepository : HibernateDataRow<DbFriend>
    {
        public IList<DbFriend> GetUserFriends(uint userId)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
                return sessionFactory
                    .CreateCriteria<DbFriend>()
                    .Add(Restrictions.Eq("UserIdentity", userId))
                    .SetMaxResults(100)
                    .List<DbFriend>();
        }

        public int DeleteFriends(uint identity0, uint identity1)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                return sessionFactory.CreateSQLQuery("CALL DeleteFriend (?,?);")
                    .AddEntity(typeof (DbFriend))
                    .SetParameter(0, identity0)
                    .SetParameter(1, identity1)
                    .ExecuteUpdate();
            }
        }

        public bool SaveOrUpdate(DbFriend obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbFriend obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}