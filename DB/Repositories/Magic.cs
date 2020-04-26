using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class MagicRepository : HibernateDataRow<DbMagic>
    {
        public IList<DbMagic> FetchByUser(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbMagic>()
                    .Add(Restrictions.Eq("OwnerId", dwId))
                    .List<DbMagic>();
        }

        public IList<DbMagic> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbMagic>()
                    .List<DbMagic>();
        }

        public bool SaveOrUpdate(DbMagic obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbMagic obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}