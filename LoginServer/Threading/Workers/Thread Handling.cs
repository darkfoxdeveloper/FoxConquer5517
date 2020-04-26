// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Thread Handling.cs
// Last Edit: 2016/11/23 10:21
// Created: 2016/11/23 10:16

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LoginServer.Structures;
using ServerCore.Common;

namespace LoginServer.Threading
{
    public static partial class ThreadHandling
    {
        private static TimeOut _tTitle = new TimeOut(1);
        private static TimeOut _tAnalytic = new TimeOut(300);
        private static TimeOutMS _tCheckBan = new TimeOutMS(10000);

        /// <summary>
        /// Starts the server common threading, like checking if a client is online,
        /// updating the window title and such things.
        /// </summary>
        public static void ServerTasks()
        {
            _tTitle.Update();
            _tAnalytic.Update();
            _tCheckBan.Update();

            var remove = new List<BannedAddress>();
            while (true)
            {
                if (_tTitle.ToNextTime())
                    Program.UpdateWindowTitle();

                if (_tAnalytic.ToNextTime())
                    Program.WriteAnalytic();

                if (ServerKernel.BannedAddresses.Count > 0 && _tCheckBan.ToNextTime())
                    remove = ServerKernel.BannedAddresses.Values.Where(banned => !banned.Banned).ToList();

                if (remove.Count > 0)
                {
                    BannedAddress trash;
                    foreach (var get in remove)
                        ServerKernel.BannedAddresses.TryRemove(get.Address, out trash);
                }

                Thread.Sleep(500);
            }
        }
    }
}
