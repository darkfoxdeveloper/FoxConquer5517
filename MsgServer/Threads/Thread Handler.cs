using System;
using System.Threading;
using System.Timers;
using MsgServer.Structures.Actions;
using ServerCore.Common;
using Timer = System.Timers.Timer;

namespace MsgServer.Threads
{
    public static partial class ThreadHandler
    {
        private static Timer m_pCommon = new Timer(1000);
        private static Thread _P_COMMON = new Thread(ServerTasks) { Priority = ThreadPriority.BelowNormal };
        private static Thread _P_EVENTS = new Thread(EventTasks) { Priority = ThreadPriority.BelowNormal };
        private static Thread _P_USER = new Thread(UserTasks) { Priority = ThreadPriority.AboveNormal };
        private static Thread _P_MONSTER = new Thread(MonsterAiTasks) { Priority = ThreadPriority.Normal };
        //private static Thread _P_GENERATOR = new Thread(GeneratorTasks) { Priority = ThreadPriority.BelowNormal };
        private static Thread _P_BATTLE = new Thread(BattleSystemTasks) { Priority = ThreadPriority.AboveNormal };
        //private static Thread _P_ITEMS = new Thread(ItemTasks) { Priority = ThreadPriority.BelowNormal };
        private static AutomaticEvents m_pEvents = new AutomaticEvents(); 

        public static void StartThreading()
        {
            
            try
            {
                if (_P_COMMON.ThreadState != ThreadState.Running)
                    _P_COMMON.Start();
                //if (_P_COMMON.ThreadState != ThreadState.Running)
                //    _P_COMMON.Start();
                if (_P_EVENTS.ThreadState != ThreadState.Running)
                    _P_EVENTS.Start();
                if (_P_USER.ThreadState != ThreadState.Running)
                    _P_USER.Start();
                if (_P_MONSTER.ThreadState != ThreadState.Running)
                    _P_MONSTER.Start();
                //if (_P_GENERATOR.ThreadState != ThreadState.Running)
                //    _P_GENERATOR.Start();
                if (_P_BATTLE.ThreadState != ThreadState.Running)
                    _P_BATTLE.Start();
                //if (_P_ITEMS.ThreadState != ThreadState.Running)
                //    _P_ITEMS.Start();

                m_pEvents.StartCheck();

                m_pCommon.Elapsed += CheckThreads;
                m_pCommon.AutoReset = true;
                m_pCommon.Enabled = true;
                m_pCommon.Start();
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
                ServerKernel.Log.SaveLog("Server will be closed...", true, LogType.ERROR);
                Console.ReadKey();
                Environment.Exit(-1);
            }
        }

        public static void CheckThreads(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!_P_COMMON.IsAlive)
                {
                    _P_COMMON = new Thread(ServerTasks) { Priority = ThreadPriority.BelowNormal };
                    _P_COMMON.Start();
                    ServerKernel.Log.SaveLog("_P_COMMON thread restarted.", true, "Threads");
                }
                if (!_P_EVENTS.IsAlive)
                {
                    _P_EVENTS = new Thread(EventTasks) { Priority = ThreadPriority.BelowNormal };
                    _P_EVENTS.Start();
                    ServerKernel.Log.SaveLog("_P_EVENTS thread restarted.", true, "Threads");
                }
                if (!_P_USER.IsAlive)
                {
                    _P_USER = new Thread(UserTasks) { Priority = ThreadPriority.AboveNormal };
                    _P_USER.Start();
                    ServerKernel.Log.SaveLog("_P_USER thread restarted.", true, "Threads");
                }
                if (!_P_MONSTER.IsAlive)
                {
                    _P_MONSTER = new Thread(MonsterAiTasks) { Priority = ThreadPriority.Normal };
                    _P_MONSTER.Start();
                    ServerKernel.Log.SaveLog("_P_MONSTER thread restarted.", true, "Threads");
                }
                //if (!_P_GENERATOR.IsAlive)
                //{
                //    _P_GENERATOR = new Thread(GeneratorTasks) { Priority = ThreadPriority.BelowNormal };
                //    _P_GENERATOR.Start();
                //    ServerKernel.Log.SaveLog("_P_GENERATOR thread restarted.", true, "Threads");
                //}
                if (!_P_BATTLE.IsAlive)
                {
                    _P_BATTLE = new Thread(BattleSystemTasks) { Priority = ThreadPriority.AboveNormal };
                    _P_BATTLE.Start();
                    ServerKernel.Log.SaveLog("_P_BATTLE thread restarted.", true, "Threads");
                }
                //if (!_P_ITEMS.IsAlive)
                //{
                //    _P_ITEMS = new Thread(BattleSystemTasks) { Priority = ThreadPriority.BelowNormal };
                //    _P_ITEMS.Start();
                //    ServerKernel.Log.SaveLog("_P_ITEMS thread restarted.", true, "Threads");
                //}
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
            }
        }
    }
}