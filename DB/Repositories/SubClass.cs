// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - SubClass.cs
// Last Edit: 2016/11/24 11:32
// Created: 2016/11/24 11:32

using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class CqSubclassRepository : HibernateDataRow<DbSubclass>
    {
        public IList<DbSubclass> GetAllSubclasses(uint ownerId)
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
                return sessionFactory
                    .CreateCriteria<DbSubclass>()
                    .Add(Restrictions.Eq("Userid", ownerId))
                    .List<DbSubclass>();
        }

        public bool SaveOrUpdate(DbSubclass obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbSubclass obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}