// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - DynamicRankRecordRepo.cs
// Last Edit: 2016/12/15 10:40
// Created: 2016/12/15 10:40

using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public sealed class DynamicRankRecordRepository : HibernateDataRow<DbDynamicRankRecord>
    {
        public IList<DbDynamicRankRecord> FetchByType(uint id)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<DbDynamicRankRecord>()
                    .Add(Restrictions.Eq("RankType", id))
                    .List<DbDynamicRankRecord>();
        }

        public bool SaveOrUpdate(DbDynamicRankRecord obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbDynamicRankRecord obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}