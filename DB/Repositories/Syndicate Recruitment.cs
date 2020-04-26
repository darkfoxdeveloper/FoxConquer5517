// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Syndicate Recruitment.cs
// Last Edit: 2017/01/27 18:23
// Created: 2017/01/27 18:22

using System.Collections.Generic;
using DB.Entities;

namespace DB.Repositories
{
    public sealed class SyndicateRecruitmentRepository : HibernateDataRow<DbSyndicateAdvertising>
    {
        public IList<DbSyndicateAdvertising> FetchAll()
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession
                    .CreateCriteria<DbSyndicateAdvertising>()
                    .List<DbSyndicateAdvertising>();
        }

        public bool SaveOrUpdate(DbSyndicateAdvertising obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbSyndicateAdvertising obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}