// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Query Executer.cs
// Last Edit: 2017/01/02 15:02
// Created: 2017/01/02 15:02

using System;

namespace DB.Repositories
{
    public sealed class QueryExecuter : HibernateDataRow<object>
    {
        public void Execute(string strQuery)
        {
            try
            {
                using (var pSession = SessionFactory.GameDatabase.OpenSession())
                    ExecuteQuery(strQuery, pSession);
            }
            catch
            {
                Console.WriteLine("Could not execute query on QueryExecuter::Execute(string)");
            }
        }
    }
}