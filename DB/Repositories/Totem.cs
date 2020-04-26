// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Totem.cs
// Last Edit: 2016/11/25 02:12
// Created: 2016/11/25 02:11

using System.Collections.Generic;
using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public sealed class TotemPoleRepository : HibernateDataRow<DbSyntotem>
    {
        public IList<DbSyntotem> ListAllTotems(ushort syndicate)
        {
            return SessionFactory.GameDatabase.OpenSession()
                .CreateCriteria<DbSyntotem>()
                .Add(Restrictions.Eq("Synid", syndicate))
                .List<DbSyntotem>();
        }

        public void ClearUserTotem(uint userId)
        {
            SessionFactory.GameDatabase.OpenSession()
                .CreateSQLQuery("CALL SynTotemDeleteFromUser (?);")
                .AddEntity(typeof(DbSyntotem))
                .SetParameter(0, userId)
                .ExecuteUpdate();
        }

        public bool SaveOrUpdate(DbSyntotem obj)
        {
            return SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(DbSyntotem obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}