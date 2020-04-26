// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Time.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50

using System.Runtime.InteropServices;

namespace ServerCore.Common
{
    /// <summary>
    /// This structure contains a long value, representing the system's total millisecond count from startup. The total
    /// millisecond count starts from zero on system startup. This structure uses the GetTickCount64 function from the 
    /// Windows API for desktop applications. The function is limited to the resolution of the system timer, which is
    /// typically in the range of 10 to 16 milliseconds. 
    /// </summary>
    public struct Time
    {
        // Local-Scope Function Invokes:
        [DllImport(NativeFunctionCalls.KERNEL32)]
        private static extern uint GetTickCount64();

        // Local-Scope Variable Declarations:
        private uint _value;    // The total milliseconds elapsed starting from system startup.

        // Constructors:
        /// <summary>
        /// This structure contains a long value, representing the system's total millisecond count from startup. The 
        /// total millisecond count starts from zero on system startup. This structure uses the GetTickCount64 function 
        /// from the Windows API for desktop applications. The function is limited to the resolution of the system 
        /// timer, which is typically in the range of 10 to 16 milliseconds. 
        /// </summary>
        /// <param name="ticks">The total milliseconds being held by the structure.</param>
        public Time(uint ticks)
        {
            _value = ticks;
        }

        // Methods:
        /// <summary>
        /// This method creates a new time structure by adding the amount of time specified in the parameters to the 
        /// current time held by this structure.
        /// </summary>
        /// <param name="amount">The amount of milliseconds being added to the current millisecond count held.</param>
        public Time AddMilliseconds(uint amount)
        {
            return new Time(_value + amount);
        }

        /// <summary>
        /// This method creates a new time structure by adding the amount of time specified in the parameters to the 
        /// current time held by this structure.
        /// </summary>
        /// <param name="amount">The amount of seconds being added to the current millisecond count held.</param>
        public Time AddSeconds(uint amount)
        {
            // Add 1000 ticks onto the amount to convert to milliseconds.
            return new Time(_value + amount * 1000);
        }

        /// <summary>
        /// This method creates a new time structure by adding the amount of time specified in the parameters to the 
        /// current time held by this structure.
        /// </summary>
        /// <param name="amount">The amount of minutes being added to the current millisecond count held.</param>
        public Time AddMinutes(uint amount)
        {
            // Add 60000 ticks onto the amount to convert to milliseconds.
            return new Time(_value + amount * 60000);
        }

        /// <summary>
        /// This method creates a new time structure by adding the amount of time specified in the parameters to the 
        /// current time held by this structure.
        /// </summary>
        /// <param name="amount">The amount of hours being added to the current millisecond count held.</param>
        public Time AddHours(uint amount)
        {
            // Add 3600000 ticks onto the amount to convert to milliseconds.
            return new Time(_value + amount * 3600000);
        }

        // Static Functions:
        /// <summary> This function returns the current millisecond count. </summary>
        public static Time Now
        {
            get { return new Time(GetTickCount64()); }
        }

        // Operations:
        #region Operations and Implicit Conversions
        public static Time operator +(Time a, Time b)
        {
            return new Time(a._value + b._value);
        }
        public static Time operator -(Time a, Time b)
        {
            return new Time(a._value - b._value);
        }
        public static Time operator *(Time a, Time b)
        {
            return new Time(a._value * b._value);
        }
        public static Time operator /(Time a, Time b)
        {
            return new Time(a._value / b._value);
        }

        public static bool operator ==(Time a, Time b)
        {
            return a._value == b._value;
        }
        public static bool operator !=(Time a, Time b)
        {
            return a._value != b._value;
        }
        public static bool operator <(Time a, Time b)
        {
            return a._value < b._value;
        }
        public static bool operator >(Time a, Time b)
        {
            return a._value > b._value;
        }
        public static bool operator <=(Time a, Time b)
        {
            return a._value <= b._value;
        }
        public static bool operator >=(Time a, Time b)
        {
            return a._value >= b._value;
        }

        public static implicit operator Time(uint num)
        {
            return new Time(num);
        }
        public static implicit operator uint(Time time)
        {
            return time._value;
        }
        public override string ToString()
        {
            return _value.ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj is Time)
                return ((Time)obj)._value == _value;
            return false;
        }
        public override int GetHashCode()
        {
            return (int)_value;
        }
        #endregion
    }
}