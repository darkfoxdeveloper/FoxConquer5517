// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Refinery.cs
// Last Edit: 2016/11/24 03:24
// Created: 2016/11/24 03:24

using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public class CqRefineryRepository : HibernateDataRow<DbRefinery>
    {
        public IList<DbRefinery> LoadAllRefineries()
        {
            using (var sessionFactory = SessionFactory.GameDatabase.OpenSession())
                return sessionFactory
                    .CreateCriteria<DbRefinery>()
                    .List<DbRefinery>();
        }
    }
}