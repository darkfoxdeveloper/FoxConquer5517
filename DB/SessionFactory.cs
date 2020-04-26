// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Session Factory.cs
// Last Edit: 2016/12/29 21:52
// Created: 2016/12/29 21:33

using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using ServerCore.Common;

namespace DB
{
    public sealed class SessionFactory
    {
        /// <summary>
        /// The database hostname.
        /// </summary>
        private readonly string _hostname;
        /// <summary>
        /// The database username.
        /// </summary>
        private readonly string _username;
        /// <summary>
        /// The database password;
        /// </summary>
        private readonly string _password;
        /// <summary>
        /// The database name.
        /// </summary>
        private readonly string _database;

        private readonly int _port = 3306;

        public static ISessionFactory LoginDatabase;
        public static ISessionFactory GameDatabase;
        public static ISessionFactory EventDatabase;

        public SessionFactory(string szHost, bool bIsLogin)
        {
            var iniFile = new IniFileName(Environment.CurrentDirectory + @"\" + szHost);
            _hostname = iniFile.GetEntryValue("MySQL", "Hostname").ToString();
            _username = iniFile.GetEntryValue("MySQL", "Username").ToString();
            _password = iniFile.GetEntryValue("MySQL", "Password").ToString();
            _database = iniFile.GetEntryValue("MySQL", "Database").ToString();
            if (bIsLogin)
            {
                _port = int.Parse(iniFile.GetEntryValue("MySQL", "Port").ToString());
                LoginDatabase = CreateLoginFactory();
            }
            else
                GameDatabase = CreateSessionFactory();
        }

        public SessionFactory(string szGame, string szLogin, bool bConfirm)
        {
            var iniFile = new IniFileName(Environment.CurrentDirectory + @"\" + szGame);
            _hostname = iniFile.GetEntryValue("MySQL", "Hostname").ToString();
            _username = iniFile.GetEntryValue("MySQL", "Username").ToString();
            _password = iniFile.GetEntryValue("MySQL", "Password").ToString();
            _database = iniFile.GetEntryValue("MySQL", "Database").ToString();
            GameDatabase = CreateSessionFactory();

            iniFile = new IniFileName(Environment.CurrentDirectory + @"\" + szLogin);
            _hostname = iniFile.GetEntryValue("MySQL", "Hostname").ToString();
            _username = iniFile.GetEntryValue("MySQL", "Username").ToString();
            _password = iniFile.GetEntryValue("MySQL", "Password").ToString();
            _database = iniFile.GetEntryValue("MySQL", "Database").ToString();
            _port = int.Parse(iniFile.GetEntryValue("MySQL", "Port").ToString());
            LoginDatabase = CreateLoginFactory();
        }

        public SessionFactory(string szGame, string szEvent)
        {
            var iniFile = new IniFileName(Environment.CurrentDirectory + @"\" + szGame);
            _hostname = iniFile.GetEntryValue("MySQL", "Hostname").ToString();
            _username = iniFile.GetEntryValue("MySQL", "Username").ToString();
            _password = iniFile.GetEntryValue("MySQL", "Password").ToString();
            _database = iniFile.GetEntryValue("MySQL", "Database").ToString();
            GameDatabase = CreateSessionFactory();

            iniFile = new IniFileName(Environment.CurrentDirectory + @"\" + szEvent);
            _hostname = iniFile.GetEntryValue("MySQL", "Hostname").ToString();
            _username = iniFile.GetEntryValue("MySQL", "Username").ToString();
            _password = iniFile.GetEntryValue("MySQL", "Password").ToString();
            _database = iniFile.GetEntryValue("MySQL", "Database").ToString();
            EventDatabase = CreateSessionFactory();
        }

        /// <summary>
        /// Configure NHibernate. This method returns an ISessionFactory instance that is
        /// populated with mappings created by Fluent NHibernate.
        /// </summary>
        /// <returns>The SessionFactory so you can use it to Mappings and w/e.</returns>
        internal ISessionFactory CreateSessionFactory()
        {
            var session = Fluently
                .Configure()
                .Database(MySQLConfiguration.Standard.ConnectionString(x => x
                    .Server(_hostname)
                    .Username(_username)
                    .Password(_password)
                    .Database(_database)))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<SessionFactory>());
            //SchemaMetadataUpdater.QuoteTableAndColumns(session);
            session.ExposeConfiguration(x => x.SetProperty("hbm2ddl.keywords", "auto-quote"));
            return session.BuildSessionFactory();
        }

        internal ISessionFactory CreateLoginFactory()
        {
            string szString = string.Format("Server={0};Port={1};Database={2};Uid={3};Password={4};charset=utf8;"
                , _hostname, _port, _database, _username, _password);
            var session = Fluently
                .Configure()
                .Database(MySQLConfiguration.Standard.ConnectionString(szString))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<SessionFactory>());
            session.ExposeConfiguration(x => x.SetProperty("hbm2ddl.keywords", "auto-quote"));
            return session.BuildSessionFactory();
        }
    }
}