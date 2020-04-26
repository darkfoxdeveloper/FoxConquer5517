using DB.Entities;

namespace DB.Repositories
{
    public class LoginRcdRepository : HibernateDataRow<DbLoginRcd>
    {
        public bool SaveOrUpdate(DbLoginRcd obj)
        {
            return base.SaveOrUpdate(obj, SessionFactory.LoginDatabase.OpenSession());
        }

        public bool Delete(DbLoginRcd obj)
        {
            return base.TryDelete(obj, SessionFactory.LoginDatabase.OpenSession());
        }
    }
}