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
using System.IO;
using System.Runtime.InteropServices;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using IniParser;
using IniParser.Model;
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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
            } else
            {
                var iniFileParser = new FileIniDataParser();
                string iniFilePath = Path.Combine(Environment.CurrentDirectory, szHost);
                IniData iniFileData = iniFileParser.ReadFile(iniFilePath);
                _hostname = iniFileData["MySQL"]["Hostname"];
                _username = iniFileData["MySQL"]["Username"];
                _password = iniFileData["MySQL"]["Password"];
                _database = iniFileData["MySQL"]["Database"];
                if (bIsLogin)
                {
                    _port = int.Parse(iniFileData["MySQL"]["Port"]);
                    LoginDatabase = CreateLoginFactory();
                }
                else
                    GameDatabase = CreateSessionFactory();
            }
        }

        public SessionFactory(string szGame, string szLogin)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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
            } else
            {
                var loginIniFile = new FileIniDataParser();
                var gameIniFile = new FileIniDataParser();
                string szGamePath = Path.Combine(Environment.CurrentDirectory, szGame);
                string szLoginPath = Path.Combine(Environment.CurrentDirectory, szLogin);
                IniData LoginData = loginIniFile.ReadFile(szLoginPath);
                IniData GameData = gameIniFile.ReadFile(szGamePath);
                _hostname = GameData["MySQL"]["Hostname"];
                _username = GameData["MySQL"]["Username"];
                _password = GameData["MySQL"]["Password"];
                _database = GameData["MySQL"]["Database"];
                GameDatabase = CreateSessionFactory();
                _hostname = LoginData["MySQL"]["Hostname"];
                _username = LoginData["MySQL"]["Username"];
                _password = LoginData["MySQL"]["Password"];
                _database = LoginData["MySQL"]["Database"];
                _port = int.Parse(LoginData["MySQL"]["Port"]);
                LoginDatabase = CreateLoginFactory();
            }
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