// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Server Processing.cs
// Last Edit: 2017/01/23 19:50
// Created: 2016/12/29 21:33

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using MsgServer.Network;
using MsgServer.Network.LoginServer;
using MsgServer.Structures;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Threads
{
    public static partial class ThreadHandler
    {
        private static readonly TimeOutMS m_pLoginMs = new TimeOutMS(800);
        private static readonly TimeOut m_pLogin = new TimeOut(5);
        private static readonly TimeOut m_t_1Second = new TimeOut(1);
        private static readonly TimeOut m_tPlayer = new TimeOut(1);
        private static readonly TimeOut m_tItem = new TimeOut(5);
        private static readonly TimeOut m_tRestart = new TimeOut(30);

        public static void ServerTasks()
        {
            m_pLoginMs.Update();
            m_pLogin.Update();
            m_t_1Second.Startup(1);
            m_tPlayer.Startup(1);
            m_tItem.Startup(5);

            while (true)
            {
                try
                {
                    // handle socket connection
                    if (ServerKernel.LoginServer == null && m_pLoginMs.ToNextTime())
                    {
                        ServerKernel.Log.SaveLog("Connect to the account server...", true, LogType.MESSAGE);

                        try
                        {
                            var pSocket = new LoginSocket();
                            pSocket.ConnectTo(ServerKernel.LoginServerAddress, ServerKernel.LoginServerPort);
                            ServerKernel.LoginServer = new LoginClient(pSocket, pSocket, null);
                            try
                            {
                                ServerKernel.Log.SaveLog("Connected to the account server!", true, LogType.MESSAGE);
                                ServerKernel.Log.SaveLog("Server will attempt a connection after 1 second...", true,
                                    LogType.MESSAGE);
                                Thread.Sleep(1000);
                                var pMsg = new MsgLoginSvAuthRequest(ServerKernel.HelloSendString);
                                ServerKernel.LoginServer.Send(pMsg);
                            }
                            catch (SocketException ex)
                            {
                                ServerKernel.LoginServer = null;
                                pSocket.Dispose();
                                pSocket = null;
                            }
                        }
                        catch (SocketException ex)
                        {
                            if (ex.ErrorCode != 10061 && ex.ErrorCode != 10057)
                            {
                                ServerKernel.Log.SaveLog("Exception thrown while trying to connect to login server",
                                    true,
                                    LogType.ERROR);
                                ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
                            }
                        }
                    }

                    // clean the login request list
                    if (ServerKernel.LoginServer != null)
                    {
                        if (m_pLogin.ToNextTime() && ServerKernel.LoginQueue.Count > 0)
                        {
                            List<LoginRequest> temp = ServerKernel.LoginQueue.Values.Where(x => x.IsExpired()).ToList();
                            LoginRequest trash;
                            foreach (var pReq in temp)
                                ServerKernel.LoginQueue.TryRemove(pReq.AccountIdentity, out trash);
                        }
                    }

                    if (m_t_1Second.ToNextTime())
                    {
                        Program.UpdateTitle();
                    }

                    if (!m_tItem.ToNextTime())
                        continue;

                    foreach (Client user in ServerKernel.Players.Values.ToList())
                    {
                        if (user.Character == null)
                            continue;
                        if (!user.Character.LoginComplete || user.Character.Inventory == null ||
                            user.Character.Equipment == null)
                            continue;

                        foreach (Item item in user.Character.Inventory.Items.Values)
                        {
                            if (!item.ArtifactIsActive() || !item.RefineryIsActive())
                                item.CheckForPurificationExpired();
                            if (item.ItemExpired())
                            {
                                user.Character.Inventory.Remove(item.Identity);
                                ServerKernel.Log.GmLog("item_expire",
                                    string.Format("Item[{0}] type[{1}] owner[{2}] expired-at[{3}]", item.Identity, item.Type,
                                        item.PlayerIdentity, UnixTimestamp.Timestamp()));
                            }
                            item.TryUnlockItem();
                        }
                        foreach (Item item in user.Character.Equipment.Items.Values)
                        {
                            if (!item.ArtifactIsActive() || !item.RefineryIsActive())
                                item.CheckForPurificationExpired();
                            if (item.ItemExpired())
                            {
                                user.Character.Equipment.Remove(item.Position, Equipment.ItemRemoveMethod.DELETE);
                                ServerKernel.Log.GmLog("item_expire",
                                    string.Format("Item[{0}] type[{1}] owner[{2}] expired-at[{3}]", item.Identity, item.Type,
                                        item.PlayerIdentity, UnixTimestamp.Timestamp()));
                            }
                            item.TryUnlockItem();
                        }
                        try
                        {
                            foreach (Warehouse wh in user.Character.Warehouses.Values.ToList())
                            {
                                foreach (Item item in wh.Items.Values.ToList())
                                {
                                    if (!item.ArtifactIsActive() || !item.RefineryIsActive())
                                        item.CheckForPurificationExpired();
                                    if (item.ItemExpired())
                                    {
                                        wh.Delete(item.Identity);
                                        ServerKernel.Log.GmLog("item_expire",
                                            string.Format("Item[{0}] type[{1}] owner[{2}] expired-at[{3}]", item.Identity,
                                                item.Type, item.PlayerIdentity, UnixTimestamp.Timestamp()));
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }

                    foreach (var detained in ServerKernel.DetainedObjects.Values.Where(x => x.IsItem()))
                        detained.OnTimer();

                    foreach (Map map in ServerKernel.Maps.Values)
                        foreach (IScreenObject _object in map.GameObjects.Values)
                            if (_object is MapItem)
                            {
                                var item = _object as MapItem;
                                if (item.IsDisappear())
                                {
                                    map.RemoveItem(item);
                                    item.SelfDelete();
                                }
                            }

                    if (m_tRestart.IsActive() && m_tRestart.ToNextTime(30))
                    {
                        m_nRestartAlert++;
                        if (MaintenanceRemainingSeconds() <= 0)
                        {
                            ServerKernel.SendMessageToAll("Server is being brought down to maintenance.", ChatTone.TALK);
                            Program.SaveAll();
                            Environment.Exit(0);
                            break;
                        }
                        //string msg = string.Format(ServerString.STR_SHUTDOWN_NOTIFY,
                        //    MaintenanceRemainingSeconds());
                        int remainSecs = MaintenanceRemainingSeconds();
                        string msg = "";
                        if (remainSecs == 60)
                        {
                            msg = string.Format(ServerString.STR_SHUTDOWN_NOTIFY_MINUTE, remainSecs/60);
                        }
                        else if (remainSecs > 60 && remainSecs%60 == 0)
                        {
                            msg = string.Format(ServerString.STR_SHUTDOWN_NOTIFY_MINUTES, remainSecs/60);
                        }
                        else if (remainSecs%60 > 0)
                        {
                            msg = string.Format(ServerString.STR_SHUTDOWN_NOTIFY_MIN_AND_SECS, remainSecs/60,
                                remainSecs%60);
                        }
                        else
                        {
                            msg = string.Format(ServerString.STR_SHUTDOWN_NOTIFY_SECONDS, remainSecs);
                        }

                        ServerKernel.SendMessageToAll(msg, ChatTone.CENTER);
                        ServerKernel.Log.SaveLog(msg, true, LogType.DEBUG);
                    }
                }
                catch (Exception ex)
                {
                    ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private static int m_nRestartTime = 300;
        private static bool m_bRestart = false;
        private static int m_nRestartAlert = 0;

        public static int MaintenanceRemainingSeconds()
        {
            return (m_nRestartTime - (m_nRestartAlert * 30));
        }

        public static void SetMaintenance(int nTime = 5, bool bRestart = false)
        {
            m_nRestartAlert = 0;
            m_nRestartTime = nTime * 60;
            m_bRestart = bRestart;
            m_tRestart.Startup(30);
        }

        public static void CancelMaintenance()
        {
            m_tRestart.Clear();
            ServerKernel.SendMessageToAll("Maintenance canceled! You're enabled to play normally.", ChatTone.CENTER);
            ServerKernel.Log.SaveLog("Canceled.", true);
        }
    }
}