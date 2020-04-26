// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Battle System Processing.cs
// Last Edit: 2016/12/07 00:34
// Created: 2016/12/07 00:34

using System;
using System.Linq;
using System.Threading;
using ServerCore.Common;

namespace MsgServer.Threads
{
    public static partial class ThreadHandler
    {
        public static void BattleSystemTasks()
        {
            while (true)
            {
                try
                {
                    if (ServerKernel.Players.Count > 0)
                    {
                        foreach (var plr in ServerKernel.Players.Values.ToList().Where(x => x.Character != null))
                        {
                            plr.Character.OnBattleTimer();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
                }
                finally
                {
                    Thread.Sleep(200);
                }
            }
            Console.WriteLine("Battle System Processing Thread exited");
        }
    }
}