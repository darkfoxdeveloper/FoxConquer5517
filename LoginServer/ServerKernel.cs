// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - ServerKernel.cs
// Last Edit: 2016/11/23 10:20
// Created: 2016/11/23 09:58

using System;
using System.Collections.Concurrent;
using DB;
using LoginServer.Network;
using LoginServer.Network.LoginServer;
using LoginServer.Network.MsgServer;
using LoginServer.Structures;
using ServerCore.Common;

namespace LoginServer
{
    /// <summary>
    /// This static class holds all server basic information, such as statistics, connected
    /// game servers, connected players and others. This doesn't handle anything, just have
    /// informations to be read.
    /// </summary>
    public static class ServerKernel
    {
        public static LoginSocket LoginSocket = new LoginSocket();
        public static MsgServerSocket GameSocket = new MsgServerSocket();

        public static SessionFactory MySql;

        public static string Username = "test";
        public static string Password = "test";

        /// <summary>
        /// The server name shown in the Login Server Console Title.
        /// </summary>
        public static string ServerName = "Ftw! Masters";

        /// <summary>
        /// The message sent by this server to the MsgServer after a successfull connection
        /// to confirm if the server is compatible and enabled to login.
        /// </summary>
        public static string HelloSendString = "fzYPi0xsRGiBmj6X";
        /// <summary>
        /// The message that the MsgServer should send after a successfull connection to 
        /// confirm if the server is compatible and enabled to login.
        /// </summary>
        public static string HelloExpectedMsg = "rDOLjXHL3bFkyMVk";

        /// <summary>
        /// The Ini Parser used to read the configuration file from the LoginServer.
        /// </summary>
        public static IniFileName ConfigReader;

        /// <summary>
        /// The logger class used to write exceptions or errors thrown by the server.
        /// </summary>
        public static LogWriter Log = new LogWriter(Environment.CurrentDirectory + @"\");
        /// <summary>
        /// The time when the server has first started.
        /// </summary>
        public static DateTime ServerStartTime;

        public static int LoginPort = 9958;

        /// <summary>
        /// This dictionary holds information about banned IP Addresses.
        /// </summary>
        public static ConcurrentDictionary<string, BannedAddress> BannedAddresses = new ConcurrentDictionary<string, BannedAddress>();

        /// <summary>
        /// This dictionary holds information about the addresses that tried to login to this server.
        /// </summary>
        public static ConcurrentDictionary<string, LoginAttemptRecord> LoginAttemptRecords = new ConcurrentDictionary<string, LoginAttemptRecord>();

        // libraries
        /// <summary>
        /// This dictionary holds informations about the game servers enabled to login to 
        /// this MsgServer.
        /// </summary>
        public static ConcurrentDictionary<string, EnabledGameServer> EnabledServer = new ConcurrentDictionary<string, EnabledGameServer>();

        /// <summary>
        /// This dictionary holds players information.
        /// </summary>
        public static ConcurrentDictionary<uint, Client> Players = new ConcurrentDictionary<uint, Client>();

        /// <summary>
        /// This dictionary holds the Server Sockets that are already connected to this
        /// Login Server;
        /// </summary>
        public static ConcurrentDictionary<string, GameServer> OnlineServers = new ConcurrentDictionary<string, GameServer>();

        public static string LoginTransferKey = "EypKhLvYJ3zdLCTyz9Ak8RAgM78tY5F32b7CUXDuLDJDFBH8H67BWy9QThmaN5Vb";
        public static string LoginTransferSalt = "MyqVgBf3ytALHWLXbJxSUX4uFEu3Xmz2UAY9sTTm8AScB7Kk2uwqDSnuNJske4By";

        public static string LastLoginName;
        public static DateTime LastLogin;
        public static uint LoginCount,
            SuccessfulLoginCount, FailedLoginCount;
        public static uint FiveMinutesLoginCount,
            FiveMinutesSuccessLoginCount,
            FiveMinutesFailedLoginCount;
    }
}