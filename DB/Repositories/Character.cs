// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Character.cs
// File Created: 2015/08/03 14:33

using System;
using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class CharacterRepository : HibernateDataRow<DbUser>
    {
        public DbUser SearchByName(string szName)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbUser>()
                    .Add(Restrictions.Eq("Name", szName))
                    .SetMaxResults(1)
                    .UniqueResult<DbUser>();
        }

        public DbUser SearchByIdentity(uint nId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbUser>()
                    .Add(Restrictions.Eq("Identity", nId))
                    .SetMaxResults(1)
                    .UniqueResult<DbUser>();
        }

        public DbUser SearchByAccount(uint nId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbUser>()
                    .Add(Restrictions.Eq("AccountId", nId))
                    .SetMaxResults(1)
                    .UniqueResult<DbUser>();
        }

        public bool AccountHasCharacter(uint nAccountId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbUser>()
                    .Add(Restrictions.Eq("AccountId", nAccountId))
                    .SetMaxResults(1)
                    .UniqueResult<DbUser>() != null;
        }

        public bool CharacterExists(string szName)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbUser>()
                    .Add(Restrictions.Eq("Name", szName))
                    .SetMaxResults(1)
                    .UniqueResult<DbUser>() != null;
        }

        public bool CharacterExists(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbUser>()
                    .Add(Restrictions.Eq("Identity", dwId))
                    .SetMaxResults(1)
                    .UniqueResult<DbUser>() != null;
        }

        public bool CreateNewCharacter(DbUser newUser)
        {
            try
            {
                using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
                {
                    bool check = sessionFactory.CreateCriteria<DbUser>()
                        .Add(Restrictions.Eq("Name", newUser.Name))
                        .UniqueResult<DbUser>() != null;

                    return !check && SaveOrUpdate(newUser, SessionFactory.GameDatabase.OpenSession());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public IList<DbUser> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbUser>()
                    .List<DbUser>();
        }

        public void ChangeName(uint idUser, string szNewName, string szOldName)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                sessionFactory.CreateSQLQuery("CALL ChangeName (?,?,?);")
                    .AddEntity(typeof(DbUser))
                    .SetParameter(0, idUser)
                    .SetParameter(1, szNewName)
                    .SetParameter(2, szOldName)
                    .ExecuteUpdate();
            }
        }

        public bool SaveOrUpdate(DbUser obj)
        {
            return base.SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbUser obj)
        {
            return base.TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}