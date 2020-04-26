// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Unix Timestamp.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50

using System;

namespace ServerCore.Common
{
    public static class UnixTimestamp
    {
        public static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToUniversalTime();

        public const int TIME_SECONDS_MINUTE = 60;
        public const int TIME_SECONDS_HOUR = 60 * TIME_SECONDS_MINUTE;
        public const int TIME_SECONDS_DAY = 24 * TIME_SECONDS_HOUR;

        #region Date Time Related Functions
        public static DateTime ToDateTime(uint timestamp)
        {
            return UNIX_EPOCH.AddSeconds(timestamp);
        }

        public static int Timestamp()
        {
            return Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToUniversalTime()).TotalSeconds);
        }

        public static long LongTimestamp()
        {
            return Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToUniversalTime()).TotalMilliseconds);
        }

        public static int Timestamp(DateTime time)
        {
            return Convert.ToInt32((time - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToUniversalTime()).TotalSeconds);
        }

        public static long LongTimestamp(DateTime time)
        {
            return Convert.ToInt64((time - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToUniversalTime()).TotalMilliseconds);
        }

        public static int MonthDayStamp()
        {
            return Convert.ToInt32((DateTime.Now - new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, 0).ToUniversalTime()).TotalSeconds);
        }

        public static int MonthDayStamp(DateTime time)
        {
            return Convert.ToInt32((time - new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, 0).ToUniversalTime()).TotalSeconds);
        }

        public static int DayOfTheMonthStamp()
        {
            return Convert.ToInt32((DateTime.Now - new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, 0).ToUniversalTime()).TotalSeconds);
        }

        public static int DayOfTheMonthStamp(DateTime time)
        {
            return Convert.ToInt32((time - new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, 0).ToUniversalTime()).TotalSeconds);
        }
        #endregion
    }
}