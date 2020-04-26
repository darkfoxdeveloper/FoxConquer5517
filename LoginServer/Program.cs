// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Program.cs
// Last Edit: 2016/11/23 10:20
// Created: 2016/11/23 09:54

using System;
using System.IO;
using System.Linq;
using System.Text;
using DB;
using LoginServer.Network.LoginServer;
using LoginServer.Network.MsgServer;
using LoginServer.Threading;
using ServerCore.Common;
using ServerCore.Security;

namespace LoginServer
{
    public static class Program
    {
        private const string _LOGIN_ANALYTIC_LOG = "Login Analytic";

        static void Main()
        {
            Console.Title = "Starting login server, please wait...";
            CheckConfigurationFile();

            try
            {
                ServerKernel.ConfigReader = new IniFileName(Environment.CurrentDirectory + @"\Login.cfg");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            // load constants or create configuration file.

            // file handling

            Console.Title = "[World Conquer Online] Login Server";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tProject WConquer: Conquer Online Private Server Emulator");
            Console.WriteLine("\t\tDeveloped by Felipe Vieira (FTW! Masters)");
            Console.WriteLine("\t\tMarch 20th, 2016 - All Rights Reserved\n");
            // Output the description of the server
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The account server is designed to accept login data from the client and to\n"
                + "verify that the username and password combination inputted is correct with the\n"
                + "database. If the combination is correct, the client will be transferred to the\n"
                + "message server of their choice. Please wait for the database to be initialized.\n");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            ServerKernel.Log.SaveLog(Environment.CurrentDirectory, true, "Login_Server", LogType.DEBUG);
            ServerKernel.Log.SaveLog("Computer Name: " + Environment.MachineName, true, "Login_Server", LogType.DEBUG);
            ServerKernel.Log.SaveLog("User Name: " + Environment.UserName, true, "Login_Server", LogType.DEBUG);
            ServerKernel.Log.SaveLog("System Directory: " + Environment.SystemDirectory, true, "Login_Server", LogType.DEBUG);
            ServerKernel.Log.SaveLog("Some environment variables:", true, "Login_Server", LogType.DEBUG);
            ServerKernel.Log.SaveLog("OS=" + Environment.OSVersion, true, "Login_Server", LogType.DEBUG);
            ServerKernel.Log.SaveLog("NUMBER_OF_PROCESSORS: " + Environment.ProcessorCount, true, "Login_Server", LogType.DEBUG);
            ServerKernel.Log.SaveLog("PROCESSOR_ARCHITETURE: " + (Environment.Is64BitProcess ? "x64" : "x86"), true, "Login_Server", LogType.DEBUG);

            ServerKernel.Log.SaveLog("Initializing login server...", true, "Login_Server");

            ServerKernel.Log.SaveLog("Starting threads...", true, "Login_Server", LogType.DEBUG);
            ThreadHandling.StartThreading();
            ServerStartup();

            ServerKernel.ServerStartTime = DateTime.Now;
            ConsoleHandle();
        }

        private static void ServerStartup()
        {
            BlowfishCipher.InitialKey = Encoding.ASCII.GetBytes(ServerKernel.ConfigReader.GetEntryValue("Blowfish", "Key").ToString());

            ServerKernel.Log.SaveLog("Checking database...", true, "Login_Server");
            try
            {
                ServerKernel.MySql = new SessionFactory("Login.cfg", true);
                ServerKernel.Log.SaveLog("Connected to MySQL Server...", true, "Login_Server", LogType.MESSAGE);
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog("Could not connect to mysql server.", true, "LoginServer", LogType.ERROR);
                ServerKernel.Log.SaveLog(ex.ToString(), false, "LoginServer", LogType.EXCEPTION);
                Console.Read();
                Environment.Exit(-1);
            }

            ServerKernel.Log.SaveLog("Reading configuration file...", true, "Login_Server");
            ServerKernel.LoginPort = int.Parse(ServerKernel.ConfigReader.GetEntryValue("ServerConfig", "LISTEN_PORT").ToString());

            ServerKernel.Log.SaveLog("Starting Server...", true, "Login_Server");
            try
            {
                ServerKernel.GameSocket = new MsgServerSocket();
                ServerKernel.GameSocket.Bind("0.0.0.0", 9865);
                ServerKernel.GameSocket.Listen(5);
                ServerKernel.Log.SaveLog("Waiting for game server connections...", true, "Login_Server", LogType.MESSAGE);
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog("Could not open game socket on port " + ServerKernel.LoginPort, true, "LoginServer", LogType.ERROR);
                ServerKernel.Log.SaveLog(ex.ToString(), false, "LoginServer", LogType.EXCEPTION);
                Console.Read();
                Environment.Exit(-1);
            }

            try
            {
                ServerKernel.LoginSocket = new LoginSocket();
                ServerKernel.LoginSocket.Bind("0.0.0.0", ServerKernel.LoginPort);
                ServerKernel.LoginSocket.Listen(50);
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog("Could not open login socket on port " + ServerKernel.LoginPort, true, "LoginServer", LogType.ERROR);
                ServerKernel.Log.SaveLog(ex.ToString(), false, "LoginServer", LogType.EXCEPTION);
                Console.Read();
                Environment.Exit(-1);
            }
            ServerKernel.Log.SaveLog("Server is ready for connections...", true, "Login_Server");
        }

        public static void WriteAnalytic()
        {
            ServerKernel.Log.SaveLog(false, new string('=', Console.WindowWidth), _LOGIN_ANALYTIC_LOG);
            ServerKernel.Log.SaveLog(false,
                string.Format("=== Conquer Online Login Server - {0} - {1}", "World Conquer Online",
                    DateTime.Now.ToString("MMM yy dd hh:mm:ss")), _LOGIN_ANALYTIC_LOG);
            ServerKernel.Log.SaveLog(false,
                string.Format("=== Start Server time is {0}",
                    ServerKernel.ServerStartTime.ToString("yyyy-MM-dd hh:mm:ss")), _LOGIN_ANALYTIC_LOG);
            ServerKernel.Log.SaveLog(false, new string('=', Console.WindowWidth), _LOGIN_ANALYTIC_LOG);
            ServerKernel.Log.SaveLog(false,
                string.Format("=== Current time is {0}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")),
                _LOGIN_ANALYTIC_LOG);
            var interval = DateTime.Now - ServerKernel.ServerStartTime;
            ServerKernel.Log.SaveLog(false,
                string.Format("=== Server takes {0} days {1:00}:{2:00}:{3:00}", interval.Days, interval.Hours,
                    interval.Minutes, interval.Seconds), _LOGIN_ANALYTIC_LOG);
            ServerKernel.Log.SaveLog(false, new string('=', Console.WindowWidth), _LOGIN_ANALYTIC_LOG);
            ServerKernel.Log.SaveLog(false, string.Format("LastLoginUsername [\"{0}\"], LastLoginTime[{1}]", ServerKernel.LastLoginName, ServerKernel.LastLogin),
                _LOGIN_ANALYTIC_LOG);
            ServerKernel.Log.SaveLog(false, string.Format(
                    "LoginCount[{0},{1}(5min)], SuccessLoginCount[{2},{3}(5min)], FailedLoginCount[{4},{5}(5min)]",
                    ServerKernel.LoginCount, ServerKernel.FiveMinutesLoginCount,
                    ServerKernel.SuccessfulLoginCount, ServerKernel.FiveMinutesSuccessLoginCount,
                    ServerKernel.FailedLoginCount, ServerKernel.FiveMinutesFailedLoginCount), _LOGIN_ANALYTIC_LOG);
            // ServerKernel.Log.SaveLog(false, string.Format(""), _LOGIN_ANALYTIC_LOG);
            ServerKernel.Log.SaveLog(false, "", _LOGIN_ANALYTIC_LOG);
        }

        static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        static void ConsoleHandle()
        {
            Console.WriteLine();

            string szCommand = string.Empty;
            do
            {
                szCommand = Console.ReadLine();
                if (string.IsNullOrEmpty(szCommand)) continue;

                string[] szParsed = szCommand.Split(' ');
                switch (szParsed[0].ToLower())
                {
                    case "addgameserver":
                        {
                            try
                            {
                                if (szParsed.Length < 6)
                                    throw new Exception();

                                // todo
                            }
                            catch (Exception ex)
                            {
                                ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
                                ServerKernel.Log.SaveLog("addgameserver [address] [port] [user] [pass] [name]", true,
                                    LogType.WARNING);
                                ServerKernel.Log.SaveLog("address is the server IP or hostname.", true, LogType.WARNING);
                                ServerKernel.Log.SaveLog("port is the port the login server will try connect to.", true,
                                    LogType.WARNING);
                                ServerKernel.Log.SaveLog("user is the username required to login.", true, LogType.WARNING);
                                ServerKernel.Log.SaveLog("pass is the password required to login.", true, LogType.WARNING);
                                ServerKernel.Log.SaveLog("name is the server name.", true, LogType.WARNING);
                            }
                            break;
                        }
                    case "cls":
                    case "clear":
                    case "clearconsole":
                        Console.Clear();
                        break;
                    case "exit":
                        return;
                }
            } while (true);

            //while (true)
            //{
            //    Console.Write(szDefault);
            //    string szCommand = Console.ReadLine();

            //    if (szCommand == null)
            //    {
            //        // ?? forgot what i was gonna do here
            //        continue;
            //    }

            //    string[] szParsed = szCommand.Split(' ');
            //    switch (szParsed[0].ToLower())
            //    {
            //        case "addgameserver":
            //        {
            //            try
            //            {
            //                if (szParsed.Length < 6)
            //                    throw new Exception();

            //                // todo
            //            }
            //            catch (Exception ex)
            //            {
            //                ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
            //                ServerKernel.Log.SaveLog("addgameserver [address] [port] [user] [pass] [name]", true, LogType.WARNING);
            //                ServerKernel.Log.SaveLog("address is the server IP or hostname.", true, LogType.WARNING);
            //                ServerKernel.Log.SaveLog("port is the port the login server will try connect to.", true, LogType.WARNING);
            //                ServerKernel.Log.SaveLog("user is the username required to login.", true, LogType.WARNING);
            //                ServerKernel.Log.SaveLog("pass is the password required to login.", true, LogType.WARNING);
            //                ServerKernel.Log.SaveLog("name is the server name.", true, LogType.WARNING);
            //            }
            //            break;
            //        }
            //        case "cls":
            //        case "clear":
            //        case "clearconsole":
            //            Console.Clear();
            //            break;
            //        case "exit":
            //            Console.WriteLine("Are you sure you want to close the server? (Y/N)");
            //            var pKey = Console.ReadKey();
            //            if (pKey.Key == ConsoleKey.Y)
            //                return;
            //            ClearLine();
            //            continue;
            //    }
            //}
        }

        static void CheckConfigurationFile()
        {
            string szPath = Environment.CurrentDirectory + @"\";
            if (!File.Exists(szPath + "Login.cfg"))
            {
                Console.WriteLine("The file Login.cfg has not been found. The system will guide you to create a new valid one.");
                bool bFinish = false;

                while (!bFinish)
                {
                    // create config
                    Console.WriteLine("[MySQL configuration]");
                    Console.Write("MySQL Hostname or Address: ");
                    string szSqlHost = Console.ReadLine() ?? string.Empty;
                    Console.Write("MySQL Username: ");
                    string szSqluser = Console.ReadLine() ?? string.Empty;
                    Console.Write("MySQL Password: ");
                    string szSqlPass = Console.ReadLine() ?? string.Empty;
                    Console.Write("MySQL Database: ");
                    string szSqlData = Console.ReadLine() ?? string.Empty;
                    Console.WriteLine();
                    Console.WriteLine("[Server Key Configuration]");
                    Console.WriteLine("The system will use default values if you set it with a invalid key.");
                    Console.Write("Type your 32 bits encryption Key: ");
                    string szKey = Console.ReadLine();
                    if (szKey == null || szKey.Length < 32)
                        szKey = "EypKhLvYJ3zdLCTyz9Ak8RAgM78tY5F32b7CUXDuLDJDFBH8H67BWy9QThmaN5Vb";
                    Console.Write("Type your 32 bits encryption Salt: ");
                    string szSalt = Console.ReadLine();
                    if (szSalt == null || szSalt.Length < 32)
                        szSalt = "MyqVgBf3ytALHWLXbJxSUX4uFEu3Xmz2UAY9sTTm8AScB7Kk2uwqDSnuNJske4By";

                    Console.WriteLine();
                    Console.WriteLine("[Login Server Configuration]");
                    Console.Write("Type the port that this server will be listening for incoming connections: ");
                    string szListenPort = Console.ReadLine() ?? string.Empty;

                    Console.WriteLine();
                    Console.WriteLine("[Game Server Configuration]");
                    Console.Write("Type your Server Address: ");
                    string szServerAddr = Console.ReadLine() ?? string.Empty;
                    Console.Write("Type your Server Port: ");
                    string szServerPort = Console.ReadLine() ?? string.Empty;
                    Console.Write("Type your Server Username: ");
                    string szServerUser = Console.ReadLine() ?? string.Empty;
                    Console.Write("Type your Server Password: ");
                    string szServerPass = Console.ReadLine() ?? string.Empty;
                    Console.Write("Type your Server Name: ");
                    string szServerName = Console.ReadLine() ?? string.Empty;

                    Console.WriteLine();
                    if (szSqlHost == string.Empty || szSqluser == string.Empty || szSqlPass == string.Empty ||
                        szSqlData == string.Empty || szKey == string.Empty || szSalt == string.Empty || szListenPort == string.Empty ||
                        szServerAddr == string.Empty || szServerPort == string.Empty || szServerUser == string.Empty || szServerPass == string.Empty ||
                        szServerName == string.Empty)
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid configuration.");
                        continue;
                    }

                    Console.WriteLine("If you want to add new servers to your account server, type \"addgameserver [param]\".");
                    Console.WriteLine("System is building your configuration file.");
                    Console.WriteLine();

                    string szFileWrite = string.Format("[Database]\r\n" +
                                                       "MYSQL_HOST={0}\r\n" +
                                                       "MYSQL_USER={1}\r\n" +
                                                       "MYSQL_PASS={2}\r\n" +
                                                       "MYSQL_DATA={3}\r\n" +
                                                       "\r\n" +
                                                       "[Security]\r\n" +
                                                       "; maximum login attempt in less than n MIN_LOGIN_TIME seconds\r\n" +
                                                       "MAX_LOGIN_ATTEMPT=5\r\n" +
                                                       "; minimum amount of login tries in n seconds\r\n" +
                                                       "MIN_LOGIN_TIME=10\r\n" +
                                                       "; ban time for users that try to login MAX_LOGIN_ATTEMPT times in MIN_LOGIN_TIME seconds\r\n" +
                                                       "BAN_TIME=0\r\n" +
                                                       "; lock user after n failed login attempt\r\n" +
                                                       "WRONG_PASSWORD_LOCK=5\r\n" +
                                                       "\r\n" +
                                                       "[ServerConfig]\r\n" +
                                                       "MIN_ONLINE_PLAYER=10\r\n" +
                                                       "\r\n" +
                                                       "; Don't change this if you don't know what you're doing.\r\n" +
                                                       "[TransferKey]\r\n" +
                                                       "Key={4}\r\n" +
                                                       "Salt={5}\r\n" +
                                                       "\r\n" +
                                                       "[ListenPort]\r\n" +
                                                       "Port0={6}\r\n" +
                                                       "\r\n" +
                                                       "[GameServer]\r\n" +
                                                       "; Server0=GameServerIP GameServerPort Username Password ServerName\r\n" +
                                                       "Server0={7} {8} {9} {10} {11}", szSqlHost, szSqluser, szSqlPass,
                        szSqlData,
                        szKey, szSalt, szListenPort, szServerAddr, szServerPort, szServerUser, szServerPass,
                        szServerName);

                    Console.WriteLine(szFileWrite);
                    Console.WriteLine();
                    Console.Write("Your server will use this configuration file? (y/n)");

                    var pKey = Console.ReadKey();
                    if (pKey.Key == ConsoleKey.Y)
                    {
                        // todo write to file

                        bFinish = true;
                        Console.Clear();
                    }
                }
            }
        }

        public static void UpdateWindowTitle()
        {
            var now = DateTime.Now;
            Console.Title = string.Format("[{0}] Conquer Online Login Server - {4:00}/{5:00}/{6:00} {1:00}:{2:00}:{3:00} - Online: {7}",
                ServerKernel.ServerName, now.Hour, now.Minute, now.Second,
                now.Month, now.Day, now.Year, OnlineCount());
        }

        private static int OnlineCount()
        {
            return ServerKernel.OnlineServers.Values.Sum(sv => sv.OnlinePlayers);
        }
    }
}
