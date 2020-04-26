// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Monster AI.cs
// Last Edit: 2016/12/07 00:37
// Created: 2016/12/07 00:36

using System;
using System.Linq;
using System.Threading;
using MsgServer.Structures.Entities;

namespace MsgServer.Threads
{
    public static partial class ThreadHandler
    {
        public static void MonsterAiTasks()
        {
            int threadSleep = 500;
            while (true)
            {
                try
                {
                    if (ServerKernel.Players.Count > 0)
                    {
                        threadSleep = 500;
                        foreach (var map in ServerKernel.Maps.Values.ToList().Where(x => x.Players.Count > 0))
                        {
                            foreach (var mob in map.GameObjects.Values.ToList().Where(x => x is Monster).Cast<Monster>())
                            {
                                if (mob.IsAlive || mob.Life > 0)
                                {
                                    if (mob.IsActive)
                                    {
                                        mob.OnTimer();
                                    }
                                    if (mob.Status.Status.Count > 0)
                                        mob.StatusTimer();
                                }
                                else
                                {
                                    if (mob.IsDead() && mob.CanDisappear)
                                    {
                                        map.RemoveMonster(mob);
                                        Monster trash;
                                        mob.Generator.Collection.TryRemove(mob.Identity, out trash);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        threadSleep = 3000;
                    }

                    foreach (var gen in ServerKernel.Generators)
                        gen.OnTimer();
                }
                catch
                {

                }

                Thread.Sleep(threadSleep);
            }
            Console.WriteLine("Monster AI Processing Thread exited");
        }
    }
}
