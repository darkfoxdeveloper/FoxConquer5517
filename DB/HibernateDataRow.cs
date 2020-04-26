// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Hibernate Data Row.cs
// File Created: 2015/07/31 13:46

using System;
using System.Security;
using NHibernate;

namespace DB
{
    /// <summary>
    /// This class encapsulates an object from the .NET Hibernate database management system. It includes the 
    /// base of all objects in the database that can be altered by the server. It contains methods for saving 
    /// and deleting the data row the object represents.
    /// </summary>
    public abstract class HibernateDataRow<T> where T : class
    {
        /// <summary>
        /// This method saves the object to the database. If the object exists already, then this method will
        /// update that object. If the object does not exist, then it will be inserted into the database.
        /// </summary>
        public virtual bool SaveOrUpdate(T obj, ISession pSession)
        {
            try
            {
                using (var transaction = pSession.BeginTransaction())
                {
                    pSession.SaveOrUpdate(obj);
                    transaction.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// This method attempts to save an object to the database. If the object exists already, then this method
        /// will throw an error and return false; else, the save was successful and the method will return true.
        /// </summary>
        public virtual bool TrySave(T obj, ISession pSession)
        {
            try
            {
                // Attempt to insert the value. Throws exception if the value exists.
                using (var transaction = pSession.BeginTransaction())
                {
                    pSession.Save(obj);
                    transaction.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// This method attempts to update an object in the database. If the object exists already, then this method
        /// will update that object and return true; else, the object does not exist and the update was unsuccessful,
        /// thus the method returns false.
        /// </summary>
        public virtual bool TryUpdate(T obj, ISession pSession)
        {
            try
            {
                // Attempt to insert the value. Throws exception if the value exists.
                using (var transaction = pSession.BeginTransaction())
                {
                    pSession.Update(obj);
                    transaction.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// This method attempts to delete the object from the database. If the object exists and can be deleted
        /// successfully, then this method returns true; else, it returns false.
        /// </summary>
        public virtual bool TryDelete(T obj, ISession pSession)
        {
            try
            {
                // Attempt to insert the value. Throws exception if the value exists.
                using (var transaction = pSession.BeginTransaction())
                {
                    pSession.Delete(obj);
                    transaction.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// This method accepts a SQL query as a formatted string from the method's arguments, and executes the
        /// query. Error handling should be handled by the parent function. 
        /// </summary>
        /// <param name="query">The formatted SQL query string.</param>
        /// <param name="pSession"></param>
        public virtual int ExecuteQuery(string query, ISession pSession)
        {
            return pSession.GetNamedQuery(query).ExecuteUpdate();
        }
    }
}