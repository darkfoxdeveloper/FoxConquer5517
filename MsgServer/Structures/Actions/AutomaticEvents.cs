// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Automatic Events.cs
// Last Edit: 2016/12/06 14:12
// Created: 2016/12/06 14:12

using System;
using System.Collections.Concurrent;
using System.Timers;
using MsgServer.Structures.Interfaces;
using ServerCore.Common;

namespace MsgServer.Structures.Actions
{
    public class AutomaticEvents
    {
        private Timer _timer;

        private const int _ACTION_SYSTEM_EVENT = 2000000;
        private const int _ACTION_SYSTEM_EVENT_LIMIT = 100;
        private readonly ConcurrentDictionary<uint, ActionStruct> _dictionary;
        private GameAction _pGameAction = new GameAction();

        public AutomaticEvents()
        {
            _dictionary = new ConcurrentDictionary<uint, ActionStruct>(1, _ACTION_SYSTEM_EVENT_LIMIT);
            for (int a = 0; a < _ACTION_SYSTEM_EVENT_LIMIT; a++)
            {
                ActionStruct action;
                if (ServerKernel.GameActions.TryGetValue((uint)(_ACTION_SYSTEM_EVENT + a), out action))
                    _dictionary.TryAdd(action.Id, action);
            }
        }

        public void RefreshEvents()
        {
            _dictionary.Clear();
            for (int a = 0; a < _ACTION_SYSTEM_EVENT_LIMIT; a++)
            {
                ActionStruct action;
                if (ServerKernel.GameActions.TryGetValue((uint)(_ACTION_SYSTEM_EVENT + a), out action))
                    _dictionary.TryAdd(action.Id, action);
            }
        }

        public void StartCheck()
        {
            _timer = new Timer();
            _timer.Elapsed += Execute;
            _timer.Interval = CalculateInterval();
            _timer.Start();
        }

        private void Execute(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var action in _dictionary.Values)
                    if (_pGameAction == null)
                    {
                        _pGameAction = new GameAction();
                        _pGameAction.ProcessAction(action.Id, null, null, null, null);
                    }
                    else
                    {
                        _pGameAction.ProcessAction(action.Id, null, null, null, null);
                    }
                // new NpcRequest(action.Id);

                RefreshEvents();
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
            }
            finally
            {
                _timer.Interval = CalculateInterval(); //every 00 of every minute
            }
        }

        private int CalculateInterval()
        {
            DateTime now = DateTime.Now;

            DateTime future = now.AddSeconds(60 - (now.Second % 60)).AddMilliseconds(now.Millisecond * -1);
            TimeSpan interval0 = future - now;
            return (int)interval0.TotalMilliseconds;
        }
    }
}