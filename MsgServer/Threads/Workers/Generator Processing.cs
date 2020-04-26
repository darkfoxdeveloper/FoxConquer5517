// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Generator Processing.cs
// Last Edit: 2016/12/07 00:36
// Created: 2016/12/07 00:36

using System;
using System.Threading;
using ServerCore.Common;

namespace MsgServer.Threads
{
    public static partial class ThreadHandler
    {
        public static void GeneratorTasks()
        {
            while (true)
            {
                try
                {
                    foreach (var gen in ServerKernel.Generators)
                        gen.OnTimer();
                }
                catch (Exception ex)
                {
                    ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
                }
                finally
                {
                    Thread.Sleep(500);
                }
            }
            Console.WriteLine("Generator Processing Thread exited");
        }
    }
}