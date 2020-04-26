// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Constants.cs
// Last Edit: 2016/11/23 10:20
// Created: 2016/11/23 10:04

using System;
using ServerCore.Common;

namespace LoginServer
{
    public static class Constants
    {
        // ini reader - config
        public static IniFileName ConfigParser;
        // ini reader - ban
        public static IniFileName BanParser;

        // log
        public static LogWriter LogSystem = new LogWriter(Environment.CurrentDirectory + @"\");
        public const string LOGIN_SERVER_LOG_FILE = "Account Server";

        // server config
        public static int MaxLoginAttempt = 5;
        public static int MinLoginTime = 10;
        public static int BanTime = 10; // in minutes
        public static int WrongPasswordLock = 5;
        public static int MinOnlinePlayer = 10;

        // constant configuration
        public static string LoginTransferKey = "EypKhLvYJ3zdLCTyz9Ak8RAgM78tY5F32b7CUXDuLDJDFBH8H67BWy9QThmaN5Vb";
        public static string LoginTransferSalt = "MyqVgBf3ytALHWLXbJxSUX4uFEu3Xmz2UAY9sTTm8AScB7Kk2uwqDSnuNJske4By";
    }
}