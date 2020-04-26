// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Thread Handler.cs
// Last Edit: 2016/11/23 10:15
// Created: 2016/11/23 10:15

using System;
using System.Threading;
using ServerCore.Common;

namespace LoginServer.Threading
{
    public static partial class ThreadHandling
    {
        private static readonly Thread _P_COMMON = new Thread(ServerTasks) { Priority = ThreadPriority.BelowNormal };

        public static void StartThreading()
        {
            try
            {
                if (_P_COMMON.ThreadState != ThreadState.Running)
                    _P_COMMON.Start();
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), true, "LoginServer", LogType.EXCEPTION);
                ServerKernel.Log.SaveLog("Server will be closed...", true, "LoginServer", LogType.ERROR);
                Console.ReadKey();
                Environment.Exit(-1);
            }
        }

        public static void StopThreading()
        {
            try
            {
                if (_P_COMMON.ThreadState != ThreadState.Aborted)
                    _P_COMMON.Abort();
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), true, "LoginServer", LogType.EXCEPTION);
                ServerKernel.Log.SaveLog("Server will be closed...", true, "LoginServer", LogType.ERROR);
                Console.ReadKey();
                Environment.Exit(-1);
            }
        }
    }
}