// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Character.cs
// File Created: 2015/08/03 14:33

using DB.Entities;

namespace DB.Repositories
{
    public class DeletedCharacterRepository : HibernateDataRow<DbUserDeleted>
    {
        public int DeleteUser(uint identity0)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
            {
                return sessionFactory.CreateSQLQuery("CALL DeleteUser (?);")
                    .AddEntity(typeof(DbUserDeleted))
                    .SetParameter(0, identity0)
                    .ExecuteUpdate();
            }
        }

        public bool SaveOrUpdate(DbUserDeleted obj)
        {
            return base.SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbUserDeleted obj)
        {
            return base.TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}