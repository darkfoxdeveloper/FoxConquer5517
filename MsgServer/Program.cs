// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Program.cs
// Last Edit: 2016/12/07 20:58
// Created: 2016/11/23 10:12

using DB.Entities;
using DB.Repositories;
using IniParser;
using IniParser.Model;
using MsgServer.Network.GameServer;
using MsgServer.Structures.Actions;
using MsgServer.Structures.Items;
using MsgServer.Structures.Society;
using MsgServer.Threads;
using ServerCore;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Security;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace MsgServer
{
    class Program
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private static EventHandler m_pHandler;

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static void Main()
        {
            if (Utils.IsWindows)
            {
                // set close handler
                m_pHandler += Handler;
                SetConsoleCtrlHandler(m_pHandler, true);
            } else
            {
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            }

            ServerKernel.Log = new LogWriter(Environment.CurrentDirectory);

            Console.Title = "Starting server...";

            // Server header
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tFoxConquer Project: Conquer Online Private Server Emulator. 5517 Client Version.");
            Console.WriteLine("\t\tProject created by Felipe Vieira (FTW! Masters) and maintained by DaRkFoxDeveloper");
            Console.WriteLine("\t\tMay 01th, 2020 - All Rights Reserved");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            ServerKernel.Log.SaveLog(Environment.CurrentDirectory, true, LogType.DEBUG);
            ServerKernel.Log.SaveLog("Computer Name: " + Environment.MachineName, true, LogType.DEBUG);
            ServerKernel.Log.SaveLog("User Name: " + Environment.UserName, true, LogType.DEBUG);
            ServerKernel.Log.SaveLog("System Directory: " + Environment.SystemDirectory, true, LogType.DEBUG);
            ServerKernel.Log.SaveLog("Current Base Directory: " + Environment.CurrentDirectory, true, LogType.DEBUG);
            ServerKernel.Log.SaveLog("Some environment variables:", true, LogType.DEBUG);
            ServerKernel.Log.SaveLog("OS=" + Environment.OSVersion, true, LogType.DEBUG);
            ServerKernel.Log.SaveLog("NUMBER_OF_PROCESSORS: " + Environment.ProcessorCount, true, LogType.DEBUG);
            ServerKernel.Log.SaveLog("PROCESSOR_ARCHITETURE: " + (Environment.Is64BitProcess ? "x64" : "x86"), true, LogType.DEBUG);
            Console.WriteLine();
            ServerKernel.Log.SaveLog("Initializing game server...", true);

            if (Utils.IsWindows)
            {
                // read the configuration file
                ServerKernel.ConfigReader = new IniFileName(Environment.CurrentDirectory + @"\Shell.ini");
                ServerKernel.LoginServerAddress = ServerKernel.ConfigReader.GetEntryValue("AccountServer", "ACCOUNT_IP").ToString();
                ServerKernel.ServerName = ServerKernel.ConfigReader.GetEntryValue("AccountServer", "SERVERNAME").ToString();
                ServerKernel.LoginServerPort = int.Parse(ServerKernel.ConfigReader.GetEntryValue("AccountServer", "ACCOUNT_PORT").ToString());
                ServerKernel.MaxOnlinePlayer = ushort.Parse(ServerKernel.ConfigReader.GetEntryValue("AccountServer", "MAXLOGINTABLESIZE").ToString());
                ServerKernel.Username = ServerKernel.ConfigReader.GetEntryValue("AccountServer", "LOGINNAME").ToString();
                ServerKernel.Password = ServerKernel.ConfigReader.GetEntryValue("AccountServer", "PASSWORD").ToString();
                ServerKernel.TransferKey = ServerKernel.ConfigReader.GetEntryValue("TransferKey", "Key").ToString();
                ServerKernel.TransferSalt = ServerKernel.ConfigReader.GetEntryValue("TransferKey", "Salt").ToString();
                ServerKernel.Blowfish = ServerKernel.ConfigReader.GetEntryValue("Blowfish", "Key").ToString();
            } else
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(Path.Combine(Environment.CurrentDirectory, "Shell.ini"));
                ServerKernel.LoginServerAddress = data["AccountServer"]["ACCOUNT_IP"];
                ServerKernel.ServerName = data["AccountServer"]["SERVERNAME"];
                ServerKernel.LoginServerPort = int.Parse(data["AccountServer"]["ACCOUNT_PORT"]);
                ServerKernel.MaxOnlinePlayer = ushort.Parse(data["AccountServer"]["MAXLOGINTABLESIZE"]);
                ServerKernel.Username = data["AccountServer"]["LOGINNAME"];
                ServerKernel.Password = data["AccountServer"]["PASSWORD"];
                ServerKernel.TransferKey = data["TransferKey"]["Key"];
                ServerKernel.TransferSalt = data["TransferKey"]["Salt"];
                ServerKernel.Blowfish = data["Blowfish"]["Key"];
            }

            ServerKernel.Log.SaveLog("Initializing blowfish", true);
            // set the blowfish key before anything else
            BlowfishCipher.InitialKey = Encoding.ASCII.GetBytes(ServerKernel.Blowfish);

            ServerKernel.Log.SaveLog("Initializing Database", true);
            Database.Initialize();

            ServerKernel.Log.SaveLog("Starting sockets", true);

            ServerKernel.Log.SaveLog("Starting server socket...");
            try
            {
                ServerKernel.GameServer = new GameSocket();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    ServerKernel.GameServer.SetSecurity(IPProtectionLevel.Unrestricted, 6, 15);
                }
                ServerKernel.GameServer.Bind("0.0.0.0", ServerKernel.GamePort);
                ServerKernel.GameServer.Listen(10);
                ServerKernel.Log.SaveLog("Game Server waiting for client connections...", true, LogType.MESSAGE);
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog("Could not open game socket on port " + ServerKernel.GamePort, true, LogType.ERROR);
                ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
                Console.WriteLine(ex.ToString());//
                Console.ReadKey();
                Environment.Exit(-1);
            }

            ServerKernel.Log.SaveLog("Starting threads");
            new GameAction().ProcessAction(80000000u, null, null, null, null);
            ThreadHandler.StartThreading();

            HandleCommands();

            SaveAll();

            Environment.Exit(0);
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Server has been closed. Saving all.");
            SaveAll();
        }

        public static void SaveAll()
        {
            foreach (var plr in ServerKernel.Players.Values)
                try
                {
                    plr.Disconnect();
                }
                catch
                {
                    ServerKernel.Log.SaveLog("Could not save player: " + plr.Identity, true);
                }
        }

        private static void HandleCommands()
        {
            if (Utils.IsWindows)
            {
                DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
            }

            string command;
            do
            {
                command = Console.ReadLine();
                if (!string.IsNullOrEmpty(command))
                {
                    string[] pCom = command.Split(' ');
                    switch (pCom[0])
                    {
                        case "maintenance":
                            {
                                if (pCom.Length < 2)
                                    continue;

                                int nTime = 0;

                                if (!int.TryParse(pCom[1], out nTime))
                                    continue;

                                bool bRestart = true;

                                if (pCom.Length > 2)
                                {
                                    var nRestart = 0;
                                    if (!int.TryParse(pCom[2], out nRestart))
                                        continue;
                                    bRestart = nRestart > 0;
                                }

                                ThreadHandler.SetMaintenance(nTime, bRestart);
                                break;
                            }
                        case "cancelmaintenance":
                            {
                                ThreadHandler.CancelMaintenance();
                                break;
                            }
                        case "count_gen":
                            {
                                //int nCount = 0;
                                //string szMap = "undefined";
                                break;
                            }
                        case "cls":
                        case "clear":
                            Console.Clear();
                            break;
                        case "syninscribeall":
                            {
                                foreach (var syn in ServerKernel.Syndicates.Values)
                                {
                                    foreach (var synMember in syn.Members.Values)
                                    {
                                        var dbUser = Database.Characters.SearchByIdentity(synMember.Identity);
                                        if (dbUser == null)
                                            continue;
                                        var allItems = Database.Items.FetchByUser(synMember.Identity);
                                        if (allItems == null)
                                            continue;
                                        foreach (var item in allItems)
                                        {
                                            Item pItem = new Item(null, item);
                                            ItemPosition pos = Calculations.GetItemPosition(pItem.Type);
                                            switch (pos)
                                            {
                                                case ItemPosition.HEADWEAR:
                                                case ItemPosition.NECKLACE:
                                                case ItemPosition.RING:
                                                case ItemPosition.RIGHT_HAND:
                                                case ItemPosition.LEFT_HAND:
                                                case ItemPosition.ARMOR:
                                                case ItemPosition.BOOTS:
                                                case ItemPosition.ATTACK_TALISMAN:
                                                case ItemPosition.DEFENCE_TALISMAN:
                                                    {
                                                        if (!syn.Arsenal.Poles.ContainsKey(Arsenal.GetArsenalType(pos)))
                                                            continue;
                                                        Totem totem = new Totem(new DbSyntotem
                                                        {
                                                            Itemid = pItem.Identity,
                                                            Synid = syn.Identity,
                                                            Userid = synMember.Identity,
                                                            Username = dbUser.Name
                                                        }, pItem);
                                                        totem.Save();
                                                        if (syn.Arsenal.AddItem(pItem, totem))
                                                        {
                                                            Console.WriteLine("{0} has inscribed gear {1} on syndicate {2}.",
                                                                dbUser.Name, pItem.Itemtype.Name, syn.Name);
                                                        }
                                                        continue;
                                                    }
                                                default:
                                                    continue;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        case "fixpointallot":
                            {
                                var allUser = new CharacterRepository().FetchAll();
                                foreach (var user in allUser)
                                {
                                    if (user.Metempsychosis < 1 || user.Level < 15)
                                        continue;

                                    int prof = (int)(user.Profession >= 100 ? 10 : ((user.Profession - (user.Profession % 10)) / 10));
                                    if (prof > 10) prof = 10;
                                    var pData = ServerKernel.PointAllot.Values.FirstOrDefault(x => x.Profession == prof && x.Level == 1);
                                    if (pData == null)
                                    {
                                        Console.WriteLine("Could not fetch attribute points data. ResetAttrPoints for user {0}", user.Identity);
                                        continue;
                                    }

                                    user.Strength = pData.Strength;
                                    user.Agility = pData.Agility;
                                    user.Vitality = pData.Vitality;
                                    user.Spirit = pData.Spirit;

                                    ushort usAdd = 0;
                                    if (user.Metempsychosis == 1)
                                    {
                                        usAdd = (ushort)(30 + Math.Min((1 + (130 - 120)) * (130 - 120) / 2, 55));
                                        usAdd += (ushort)((user.Level - 15) * 3);
                                    }
                                    else if (user.Metempsychosis == 2)
                                    {
                                        //byte firstMetempsychosisLev = (byte)((user.MeteLevel - (user.MeteLevel % 10000)) / 10000);
                                        usAdd = (ushort)(30 + Math.Min((1 + (130 - 120)) * (130 - 120) / 2, 55));
                                        usAdd += (ushort)(52 + Math.Min((1 + (130 - 120)) * (130 - 120) / 2, 55));
                                        usAdd += (ushort)((user.Level - 15) * 3);
                                    }
                                    user.AdditionalPoints = (ushort)(usAdd - 10);
                                    Database.Characters.SaveOrUpdate(user);
                                    Console.WriteLine(
                                        "User(prof:{9}, id:{0}, name:{1}, str:{2}, agi:{3}, vit:{4}, spi:{5}, add:{8}, metem:{6}, level: {7}) updated",
                                        user.Identity, user.Name, user.Strength, user.Agility, user.Vitality, user.Spirit,
                                        user.Metempsychosis, user.Level, user.AdditionalPoints, prof);
                                }
                                Console.WriteLine("Finished...");
                                break;
                            }
                    }
                }
            } while (command != "close");
        }

        public static void UpdateTitle()
        {
            DateTime now = DateTime.Now;
            Console.Title = string.Format("[{0}] FoxConquer Project. GameServer - Server Time: {1:0000}/{2:00}/{3:00} {4:00}:{5:00} - Online/Max: {6}/{7}",
                ServerKernel.ServerName, now.Year, now.Month, now.Day, now.Hour, now.Minute, ServerKernel.Players.Count,
                ServerKernel.OnlineRecord);
        }

        /*private static void Close(CloseServerCode reason = CloseServerCode.SELF_CALL)
        {
            switch (reason)
            {
                case CloseServerCode.UNKNOWN:
                    ServerKernel.Log.SaveLog("Server is being closed an unknown call...", true, LogType.WARNING);
                    break;
                case CloseServerCode.EXCEPTION_THROWN:
                    ServerKernel.Log.SaveLog("Server is being closed due to an exception thrown...", true, LogType.WARNING);
                    break;
                case CloseServerCode.UNHANDLED_EXCEPTION:
                    ServerKernel.Log.SaveLog("Server is being closed due to an Unhandled exception thrown...", true, LogType.WARNING);
                    break;
                case CloseServerCode.SELF_CALL:
                    ServerKernel.Log.SaveLog(string.Format("Some internal reason is closing the server [called:{0}]...", new StackFrame(1).GetMethod().Name), true, LogType.WARNING);
                    break;
                default:
                    ServerKernel.Log.SaveLog("Server is being closed for an unknown reason...", true, LogType.WARNING);
                    break;
            }
        }*/

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                    ServerKernel.Log.SaveLog("Server has been closed due to a CTRL+C request... Saving data...", true,
                        LogType.WARNING);
                    break;
                case CtrlType.CTRL_LOGOFF_EVENT:
                    ServerKernel.Log.SaveLog("Server is being closed due to a logoff event...", true, LogType.WARNING);
                    break;
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                    ServerKernel.Log.SaveLog("Server is being closed due to a shutdown event...", true, LogType.WARNING);
                    break;
                case CtrlType.CTRL_CLOSE_EVENT:
                    // should not be used
                    ServerKernel.Log.SaveLog("Server is being closed due to a close button press...", true,
                        LogType.WARNING);
                    break;
                default:
                    Console.WriteLine("Server has been closed. Reason: {0}", sig);
                    break;
            }

            // todo save information
            SaveAll();

            return true;
        }
    }
}