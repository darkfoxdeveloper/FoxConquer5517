// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Timeout.cs
// Last Edit: 2017/01/10 00:47
// Created: 2017/01/10 00:45

using System;

namespace ServerCore.Common
{
    public sealed class TimeOut
    {
        private long _updateTime;
        private int _interval;

        public TimeOut(int nInterval)
        {
            _interval = nInterval;
            _updateTime = 0;
        }

        public long Clock() { return (Environment.TickCount / 1000); }
        public bool Update() { _updateTime = Clock(); return true; }
        public bool IsTimeOut() { return Clock() >= _updateTime + _interval; }
        public bool ToNextTime() { if (IsTimeOut()) return Update(); return false; }
        public void SetInterval(int nSecs) { _interval = nSecs; }
        public void Startup(int nSecs) { _interval = nSecs; Update(); }
        public bool TimeOver() { if (IsActive() && IsTimeOut()) return Clear(); return false; }
        public bool IsActive() { return _updateTime != 0; }
        public bool Clear() { _updateTime = _interval = 0; return true; }

        public void IncInterval(int nSecs, int nLimit) { _interval = Calculations.CutOverflow(_interval + nSecs, nLimit); }
        public void DecInterval(int nSecs) { _interval = Calculations.CutTrail(_interval - nSecs, 0); }

        public bool IsTimeOut(int nSecs) { return Clock() >= _updateTime + nSecs; }
        public bool ToNextTime(int nSecs) { if (IsTimeOut(nSecs)) return Update(); return false; }
        public bool TimeOver(int nSecs) { if (IsActive() && IsTimeOut(nSecs)) return Clear(); return false; }

        public bool ToNextTick(int nSecs)
        {
            if (IsTimeOut(nSecs))
            {
                if (Clock() >= _updateTime + nSecs * 2)
                    return Update();
                _updateTime += nSecs;
                return true;
            }
            return false;
        }

        public int GetRemain() { return _updateTime != 0 ? Calculations.CutRange(_interval - ((int)Clock() - (int)_updateTime), 0, _interval) : 0; }
        public int GetInterval() { return _interval; }
    }

    public sealed class TimeOutMS
    {
        private long _updateTime;
        private int _interval;

        public TimeOutMS(int nInterval)
        {
            if (nInterval < 0 || nInterval > int.MaxValue)
                nInterval = int.MaxValue;
            _interval = nInterval;
            _updateTime = 0;
        }
        public long Clock() { return Environment.TickCount; }
        public bool Update() { _updateTime = Clock(); return true; }
        public bool IsTimeOut() { return Clock() >= _updateTime + _interval; }
        public bool ToNextTime()
        {
            if (IsTimeOut())
                return Update();
            return false;
        }
        public void SetInterval(int nMilliSecs) { _interval = nMilliSecs; }
        public void Startup(int nMilliSecs) { _interval = Math.Min(nMilliSecs, int.MaxValue); Update(); }
        public bool TimeOver()
        {
            if (IsActive() && IsTimeOut()) return Clear();
            return false;
        }
        public bool IsActive() { return _updateTime != 0; }
        public bool Clear() { _updateTime = _interval = 0; return true; }

        public void IncInterval(int nMilliSecs, int nLimit) { _interval = Calculations.CutOverflow(_interval + nMilliSecs, nLimit); }
        public void DecInterval(int nMilliSecs) { _interval = Calculations.CutTrail(_interval - nMilliSecs, 0); }

        public bool IsTimeOut(int nMilliSecs) { return Clock() >= _updateTime + nMilliSecs; }
        public bool ToNextTime(int nMilliSecs) { if (IsTimeOut(nMilliSecs)) return Update(); return false; }
        public bool TimeOver(int nMilliSecs) { if (IsActive() && IsTimeOut(nMilliSecs)) return Clear(); return false; }

        public bool ToNextTick(int nMilliSecs)
        {
            if (IsTimeOut(nMilliSecs))
            {
                if (Clock() >= _updateTime + nMilliSecs * 2)
                    return Update();
                _updateTime += nMilliSecs;
                return true;
            }
            return false;
        }

        public int GetRemain() { return _updateTime != 0 ? Calculations.CutRange(_interval - ((int)Clock() - (int)_updateTime), 0, _interval) : 0; }
        public int GetInterval() { return _interval; }
    }
}