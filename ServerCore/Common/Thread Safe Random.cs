// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Thread Safe Random.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50

using System;

namespace ServerCore.Common
{
    public static class ThreadSafeRandom
    {
        private static readonly Random m_global = new Random();
        [ThreadStatic]
        private static Random _local;
        private static int count = 0;

        public static int RandGet()
        {
            Random inst = _local;
            if (inst == null || count++ >= 10000)
            {
                int seed;
                lock (m_global) seed = m_global.Next();
                _local = inst = new Random(seed);
                count = 0;
            }
            return inst.Next();
        }

        public static int RandGet(int nValue)
        {
            Random inst = _local;
            if (inst == null || count++ >= 10000)
            {
                int seed;
                lock (m_global) seed = m_global.Next(nValue);
                _local = inst = new Random(seed);
                count = 0;
            }
            return inst.Next(nValue);
        }

        public static int RandGet(int nMin, int nMax)
        {
            Random inst = _local;
            if (inst == null || count++ >= 10000)
            {
                int seed;
                lock (m_global) seed = m_global.Next(nMin, nMax);
                _local = inst = new Random(seed);
                count = 0;
            }
            return inst.Next(nMin, nMax);
        }
    }
}
