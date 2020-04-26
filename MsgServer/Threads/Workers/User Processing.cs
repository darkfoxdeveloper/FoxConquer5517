// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - User Processing.cs
// Last Edit: 2017/01/26 19:37
// Created: 2016/12/29 21:33

using System;
using System.Linq;
using System.Threading;
using ServerCore.Common;

namespace MsgServer.Threads
{
    public static partial class ThreadHandler
    {
        public static void UserTasks()
        {
            while (true)
            {
                try
                {
                    foreach (var usr in ServerKernel.Players.Values.ToList())
                    {
                        try
                        {
                            if (usr.Character != null)
                                usr.Character.OnTimer();
                        }
                        catch
                        {
                            Console.WriteLine("ERROR ThreadHandler.UserTasks()");
                        }
                    }

                    DateTime now = DateTime.Now;
                    if (now.Hour == 0 && now.Minute == 0 && now.Second == 0)
                    {
                        //List<Character> updated = new List<Character>();
                        foreach (var plr in ServerKernel.Players.Values.ToList())
                        {
                            plr.Character.DailyReset();
                            //updated.Add(plr.Character);
                        }
                        //ServerKernel.SendMessageToAll("Your daily missions and tasks has been reseted.", ChatTone.TALK);
                    }
                }
                catch (Exception ex)
                {
                    ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
                }
                finally
                {
                    Thread.Sleep(550);
                }
            }
            Console.WriteLine("User Processing Thread exited");
        }
    }
}