using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class WeaponSkillRepository : HibernateDataRow<DbWeaponSkill>
    {
        public IList<DbWeaponSkill> FetchAll(uint id)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbWeaponSkill>()
                    .Add(Restrictions.Eq("OwnerIdentity", id))
                    .List<DbWeaponSkill>();
        }

        public bool SaveOrUpdate(DbWeaponSkill obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbWeaponSkill obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}