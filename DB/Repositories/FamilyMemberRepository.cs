// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - FamilyMemberRepository.cs
// Last Edit: 2016/12/05 06:35
// Created: 2016/12/05 06:34

using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class FamilyMemberRepository : HibernateDataRow<DbFamilyMember>
    {
        public IList<DbFamilyMember> FetchByFamily(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbFamilyMember>()
                    .Add(Restrictions.Eq("FamilyIdentity", dwId))
                    .List<DbFamilyMember>();
        }

        public DbFamilyMember FetchByUser(uint dwId)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbFamilyMember>()
                    .Add(Restrictions.Eq("Identity", dwId))
                    .SetMaxResults(1)
                    .UniqueResult<DbFamilyMember>();
        }

        public bool SaveOrUpdate(DbFamilyMember obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbFamilyMember obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}