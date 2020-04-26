// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Calculations.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ServerCore.Common.Enums;

namespace ServerCore.Common
{
    /// <summary>
    /// This class contains generic and handler specific calculation functions used during packet and server 
    /// processing. It contains calculations for screen updating, attacking, exploit checking, etc.
    /// </summary>
    public static class Calculations
    {
        // Global Calculation Constants:
        public const int MAX_DIFFERENCE_IN_ELEVATION = 210;
        public const double RADIAN_TO_DEGREE = 57.29;
        public const int SCREEN_DISTANCE = 18;

        /// <summary> This function returns the direction for a jump or attack. </summary>
        /// <param name="x1">The x coordinate of the destination point.</param>
        /// <param name="y1">The y coordinate of the destination point.</param>
        /// <param name="x2">The x coordinate of the reference point.</param>
        /// <param name="y2">The y coordinate of the reference point.</param>
        public static byte GetDirectionSector(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            double angle = GetAngle(x1, y1, x2, y2);
            byte direction = (byte)(Math.Round(angle / 45.0) % 8);
            return (byte)(direction == 8 ? 0 : direction);
        }

        /// <summary> This function returns the angle for a jump or attack. </summary>
        /// <param name="x1">The x coordinate of the destination point.</param>
        /// <param name="y1">The y coordinate of the destination point.</param>
        /// <param name="x2">The x coordinate of the reference point.</param>
        /// <param name="y2">The y coordinate of the reference point.</param>
        public static double GetAngle(double x1, double y1, double x2, double y2)
        {
            // Declare and initialize local variables:
            double angle = (Math.Atan2(y2 - y1, x2 - x1) * RADIAN_TO_DEGREE) + 90;
            return angle < 0 ? 270 + (90 - Math.Abs(angle)) : angle;
        }

        /// <summary> This function returns the distance between two objects. </summary>
        /// <param name="x1">The x coordinate of the first object.</param>
        /// <param name="y1">The y coordinate of the first object.</param>
        /// <param name="x2">The x coordinate of the second object.</param>
        /// <param name="y2">The y coordinate of the second object.</param>
        public static double GetDistance(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        /// <summary> This function returns true if an object is within the bounds of another object's screen. </summary>
        /// <param name="x1">The first object's x coordinate.</param>
        /// <param name="y1">The first object's y coordinate.</param>
        /// <param name="x2">The second object's x coordinate.</param>
        /// <param name="y2">The second object's y coordinate.</param>
        public static bool InScreen(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            return Math.Abs(x1 - x2) <= SCREEN_DISTANCE && Math.Abs(y1 - y2) <= SCREEN_DISTANCE;
            //GetDistance(x1, y1, x2, y2) <= SCREEN_DISTANCE;
            //Math.Abs(x1 - x2) <= SCREEN_DISTANCE && Math.Abs(y1 - y2) <= SCREEN_DISTANCE;
        }

        /// <summary> This function checks the elevation difference of two tiles. </summary>
        /// <param name="newElevation">The tile being checked.</param>
        /// <param name="initial">The entity's reference tile.</param>
        public static bool WithinElevation(short newElevation, short initial)
        {
            if (newElevation - initial >= MAX_DIFFERENCE_IN_ELEVATION)
                return false;
            return true;
        }

        public static bool IsInCircle(Point pos, Point posCenter, int nRange)
        {
            return Math.Sqrt((double)(posCenter.X - pos.X) * (posCenter.X - pos.X) + (double)(posCenter.Y - pos.Y) * (posCenter.Y - pos.Y)) < nRange + .9;
        }

        public static float GetRadian(float posSourX, float posSourY, float posTargetX, float posTargetY)
        {
            if (posSourX != posTargetX || posSourY != posTargetY)
                return 0f;

            const float pi = 3.1415926535f;
            float fDeltaX = posTargetX - posSourX;
            float fDeltaY = posTargetY - posSourY;
            double fDistance = Math.Sqrt(fDeltaX * fDeltaX + fDeltaY * fDeltaY);
            if (fDeltaX <= fDistance && fDistance > 0)
                return 0f;
            double fRadian = Math.Asin(fDeltaX / fDistance);

            return (float)(fDeltaY > 0 ? (pi / 2 - fRadian) : (pi + fRadian + pi / 2));
        }

        public static byte GetDirection(ushort sourceX, ushort sourceY, ushort destX, ushort destY)
        {
            return GetDirection(new Point(sourceX, sourceY), new Point(destX, destY));
        }

        public static byte GetDirection(Point @from, Point to)
        {
            int dir = 0;
            int[] tan = { -241, -41, 41, 241 };
            int deltaX = to.X - @from.X;
            int deltaY = to.Y - @from.Y;

            if (deltaX == 0)
                if (deltaY > 0)
                    dir = 0;
                else
                    dir = 4;
            else if (deltaY == 0)
                if (deltaX > 0)
                    dir = 6;
                else
                    dir = 2;
            else
            {
                int flag = Math.Abs(deltaX) / deltaX;
                int tempY = deltaY * 100 * flag;
                int i;
                for (i = 0; i < 4; i++)
                    tan[i] *= Math.Abs(deltaX);
                for (i = 0; i < 3; i++)
                    if (tempY >= tan[i] && tempY < tan[i + 1])
                        break;
                if (deltaX > 0)
                {
                    if (i == 0) dir = 5;
                    else if (i == 1) dir = 6;
                    else if (i == 2) dir = 7;
                    else if (i == 3)
                        if (deltaY > 0)
                            dir = 0;
                        else
                            dir = 4;
                }
                else
                {
                    if (i == 0) dir = 1;
                    else if (i == 1) dir = 2;
                    else if (i == 2) dir = 3;
                    else if (i == 3)
                        if (deltaY > 0)
                            dir = 0;
                        else
                            dir = 4;
                }

            }
            dir = (dir + 8) % 8;
            return (byte)dir;

        }

        /// <summary>
        /// Gets the item position by checking the itemtype identity.
        /// </summary>
        /// <param name="itemId">The ITEMTYPE of the item.</param>
        /// <returns>The position where the item can be equiped.</returns>
        public static ItemPosition GetItemPosition(uint itemId)
        {
            itemId /= 1000;
            if (itemId >= 400 && itemId <= 699)
                return ItemPosition.RIGHT_HAND;
            if (itemId == 1050)
                return ItemPosition.LEFT_HAND;
            if (itemId >= 130 && itemId <= 136)
                return ItemPosition.ARMOR;
            if ((itemId >= 110 && itemId <= 119) || (itemId >= 140 && itemId <= 149) || (itemId >= 123 && itemId <= 129))
                return ItemPosition.HEADWEAR;
            if (itemId == 160)
                return ItemPosition.BOOTS;
            if (itemId >= 120 && itemId <= 122)
                return ItemPosition.NECKLACE;
            if (itemId >= 150 && itemId <= 152)
                return ItemPosition.RING;
            if (itemId == 300)
                return ItemPosition.STEED;
            if (itemId >= 181 && itemId <= 199)
                return ItemPosition.GARMENT;
            if (itemId == 200)
                return ItemPosition.STEED_ARMOR;
            if (itemId == 201)
                return ItemPosition.ATTACK_TALISMAN;
            if (itemId == 202)
                return ItemPosition.DEFENCE_TALISMAN;
            if (itemId >= 350 && itemId <= 399)
                return ItemPosition.ACCESSORY_R;
            if (itemId == 2100)
                return ItemPosition.BOTTLE;
            if (itemId == 900)
                return ItemPosition.LEFT_HAND;
            if (itemId == 203)
                return ItemPosition.CROP;
            if (itemId == 50)
                return ItemPosition.LEFT_HAND;

            return ItemPosition.INVENTORY;
        }

        /// <summary>
        /// Checks if the item can be equiped on the left hand. Backswords, Two handed weapons,
        /// and other exceptions.
        /// </summary>
        /// <param name="itemtype">The itemtype of the item.</param>
        /// <param name="bIsPureWarrior">If the target is pure warrior in case of shield</param>
        /// <returns>If the item can be equiped on the left hand.</returns>
        public static bool CanWieldSecondHand(uint itemtype, bool bIsPureWarrior = false)
        {
            itemtype /= 1000;
            if (itemtype == 421)
                return false;
            if (itemtype >= 500 && itemtype <= 599)
                return false;
            if (itemtype == 900 && !bIsPureWarrior)
                return false;
            return true;
        }

        public static int CalculateGemPercentage(SocketGem gem)
        {
            switch (gem)
            {
                case SocketGem.NORMAL_TORTOISE_GEM:
                    return 2;
                case SocketGem.REFINED_TORTOISE_GEM:
                    return 4;
                case SocketGem.SUPER_TORTOISE_GEM:
                    return 6;
                case SocketGem.NORMAL_DRAGON_GEM:
                case SocketGem.NORMAL_PHOENIX_GEM:
                case SocketGem.NORMAL_FURY_GEM:
                    return 5;
                case SocketGem.REFINED_DRAGON_GEM:
                case SocketGem.REFINED_PHOENIX_GEM:
                case SocketGem.NORMAL_RAINBOW_GEM:
                case SocketGem.REFINED_FURY_GEM:
                    return 10;
                case SocketGem.SUPER_DRAGON_GEM:
                case SocketGem.SUPER_PHOENIX_GEM:
                case SocketGem.REFINED_RAINBOW_GEM:
                case SocketGem.SUPER_FURY_GEM:
                case SocketGem.NORMAL_MOON_GEM:
                    return 15;
                case SocketGem.SUPER_RAINBOW_GEM:
                    return 25;
                case SocketGem.NORMAL_VIOLET_GEM:
                case SocketGem.REFINED_MOON_GEM:
                    return 30;
                case SocketGem.REFINED_VIOLET_GEM:
                case SocketGem.SUPER_MOON_GEM:
                case SocketGem.NORMAL_KYLIN_GEM:
                    return 50;
                case SocketGem.REFINED_KYLIN_GEM:
                case SocketGem.SUPER_VIOLET_GEM:
                    return 100;
                case SocketGem.SUPER_KYLIN_GEM:
                    return 200;
                default:
                    return 0;
            }
        }

        public static Random Random = new Random();
        private static int m_nTimes = 0;

        public static bool ChanceCalc(float value)
        {
            try
            {
                if (value > 100)
                    value = 100;

                if (m_nTimes > 1000000)
                {
                    Random = new Random();
                    m_nTimes = 0;
                }

                Interlocked.Increment(ref m_nTimes);

                value *= 100;

                int res = Random.Next(1, 10000);

                return res <= value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Return all points on that line. (From TQ)
        /// </summary>
        public static void DDALine(int x0, int y0, int x1, int y1, int nRange, ref List<Point> vctPoint)
        {
            if (x0 == x1 && y0 == y1)
                return;

            float scale = (float)(1.0f * nRange / Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0)));
            x1 = (int)(0.5f + scale * (x1 - x0) + x0);
            y1 = (int)(0.5f + scale * (y1 - y0) + y0);
            DDALineEx(x0, y0, x1, y1, ref vctPoint);
        }

        /// <summary>
        /// Return all points on that line. (From TQ)
        /// </summary>
        public static void DDALineEx(int x0, int y0, int x1, int y1, ref List<Point> vctPoint)
        {
            if (x0 == x1 && y0 == y1)
                return;
            if (vctPoint == null)
                vctPoint = new List<Point>();
            int dx = x1 - x0;
            int dy = y1 - y0;
            int absDx = Math.Abs(dx);
            int absDy = Math.Abs(dy);
            Point point;
            if (absDx > absDy)
            {
                int _0_5 = absDx * (dy > 0 ? 1 : -1);
                int numerator = dy * 2;
                int denominator = absDx * 2;
                if (dx > 0)
                {
                    // x0 ++
                    for (int i = 1; i <= absDx; i++)
                    {
                        point = new Point { X = x0 + i, Y = y0 + ((numerator * i + _0_5) / denominator) };
                        vctPoint.Add(point);
                    }
                }
                else if (dx < 0)
                {
                    // x0 --
                    for (int i = 1; i <= absDx; i++)
                    {
                        point = new Point { X = x0 - i, Y = y0 + ((numerator * i + _0_5) / denominator) };
                        vctPoint.Add(point);
                    }
                }
            }
            else
            {
                int _0_5 = absDy * (dx > 0 ? 1 : -1);
                int numerator = dx * 2;
                int denominator = absDy * 2;
                if (dy > 0)
                {
                    // y0 ++
                    for (int i = 1; i <= absDy; i++)
                    {
                        point = new Point { Y = y0 + i, X = x0 + ((numerator * i + _0_5) / denominator) };
                        vctPoint.Add(point);
                    }
                }
                else if (dy < 0)
                {
                    // y0 -- 
                    for (int i = 1; i <= absDy; i++)
                    {
                        point = new Point { Y = y0 - i, X = x0 + ((numerator * i + _0_5) / denominator) };
                        vctPoint.Add(point);
                    }
                }
            }
        }

        public static int MulDiv(byte number, byte numerator, byte denominator) { return ((number * numerator) / denominator); }
        public static int MulDiv(short number, short numerator, short denominator) { return ((number * numerator) / denominator); }
        public static int MulDiv(ushort number, ushort numerator, ushort denominator) { return ((number * numerator) / denominator); }
        public static int MulDiv(int number, int numerator, int denominator) { return ((number * numerator) / denominator); }
        public static uint MulDiv(uint number, uint numerator, uint denominator) { return ((number * numerator) / denominator); }
        public static long MulDiv(long number, long numerator, long denominator) { return ((number * numerator) / denominator); }
        public static ulong MulDiv(ulong number, ulong numerator, ulong denominator) { return ((number * numerator) / denominator); }

        public static long CutTrail(long x, long y) { return (x >= y) ? x : y; }
        public static long CutOverflow(long x, long y) { return (x <= y) ? x : y; }
        public static long CutRange(long n, long min, long max) { return (n < min) ? min : ((n > max) ? max : n); }

        public static int CutTrail(int x, int y) { return (x >= y) ? x : y; }
        public static int CutOverflow(int x, int y) { return (x <= y) ? x : y; }
        public static int CutRange(int n, int min, int max) { return (n < min) ? min : ((n > max) ? max : n); }

        public static short CutTrail(short x, short y) { return (x >= y) ? x : y; }
        public static short CutOverflow(short x, short y) { return (x <= y) ? x : y; }
        public static short CutRange(short n, short min, short max) { return (n < min) ? min : ((n > max) ? max : n); }

        public static ulong CutTrail(ulong x, ulong y) { return (x >= y) ? x : y; }
        public static ulong CutOverflow(ulong x, ulong y) { return (x <= y) ? x : y; }
        public static ulong CutRange(ulong n, ulong min, ulong max) { return (n < min) ? min : ((n > max) ? max : n); }

        public static uint CutTrail(uint x, uint y) { return (x >= y) ? x : y; }
        public static uint CutOverflow(uint x, uint y) { return (x <= y) ? x : y; }
        public static uint CutRange(uint n, uint min, uint max) { return (n < min) ? min : ((n > max) ? max : n); }

        public static ushort CutTrail(ushort x, ushort y) { return (x >= y) ? x : y; }
        public static ushort CutOverflow(ushort x, ushort y) { return (x <= y) ? x : y; }
        public static ushort CutRange(ushort n, ushort min, ushort max) { return (n < min) ? min : ((n > max) ? max : n); }

        public static byte CutTrail(byte x, byte y) { return (x >= y) ? x : y; }
        public static byte CutOverflow(byte x, byte y) { return (x <= y) ? x : y; }
        public static byte CutRange(byte n, byte min, byte max) { return (n < min) ? min : ((n > max) ? max : n); }

        public const int ADJUST_PERCENT = 30000;						// ADJUSTÊ±£¬>=30000 ±íÊ¾°Ù·ÖÊý
        public const int ADJUST_SET = -30000;						// ADJUSTÊ±£¬<=-30000 ±íÊ¾µÈÓÚ(-1*num - 30000)
        public const int ADJUST_FULL = -32768;						// ADJUSTÊ±£¬== -32768 ±íÊ¾ÌîÂú
        public const int DEFAULT_DEFENCE2 = 10000;						// Êý¾Ý¿âÈ±Ê¡Öµ

        public static bool IsInFan(Point pos, Point posSource, int nRange, int nWidth, Point posCenter)
        {
            if (nWidth <= 0 || nWidth > 360)
                return false;

            if (posCenter.X == posSource.X && posCenter.Y == posSource.Y)
                return false;
            if (pos.X == posSource.X && pos.Y == posSource.Y)
                return false;

            if (GetDistance((ushort)posSource.X, (ushort)posSource.Y, (ushort)pos.X, (ushort)pos.Y) > nRange)
                return false;

            const float pi = 3.1415926535f;
            float fRadianDelta = (pi * nWidth / 180) / 2;
            float fCenterLine = GetRadian(posSource.X, posSource.Y, posCenter.X, posCenter.Y);
            float fTargetLine = GetRadian(posSource.X, posSource.Y, pos.X, pos.Y);
            float fDelta = Math.Abs(fCenterLine - fTargetLine);
            if (fDelta <= fRadianDelta || fDelta >= 2 * pi - fRadianDelta)
                return true;

            return false;
        }

        public static int AdjustData(int nData, int nAdjust, int nMaxData = 0)
        {
            return AdjustDataEx(nData, nAdjust, nMaxData);
        }

        public static int AdjustDataEx(int nData, int nAdjust, int nMaxData)
        {
            if (nAdjust >= ADJUST_PERCENT)
                return MulDiv(nData, nAdjust - ADJUST_PERCENT, 100);

            if (nAdjust <= ADJUST_SET)
                return -1 * nAdjust + ADJUST_SET;

            if (nAdjust == ADJUST_FULL)
                return nMaxData;

            return nData + nAdjust;
        }

        public static long AdjustData(long nData, long nAdjust, long nMaxData = 0)
        {
            return AdjustDataEx(nData, nAdjust, nMaxData);
        }

        public static long AdjustDataEx(long nData, long nAdjust, long nMaxData)
        {
            if (nAdjust >= ADJUST_PERCENT)
                return MulDiv(nData, nAdjust - ADJUST_PERCENT, 100);

            if (nAdjust <= ADJUST_SET)
                return -1 * nAdjust + ADJUST_SET;

            if (nAdjust == ADJUST_FULL)
                return nMaxData;

            return nData + nAdjust;
        }

        public static bool IsTimeRange(uint dwStart, uint dwEnd)
        {
            if (dwEnd > dwStart) return false;
            uint dwNow = uint.Parse(DateTime.Now.ToString("MMddHHmmss"));
            return dwNow >= dwStart && dwNow <= dwEnd;
        }

        public static uint DateTimeEx(DateTime dtTime)
        {
            return uint.Parse(dtTime.ToString("MMddHHmmss"));
        }

        public static uint GetTalismanGemAttr(SocketGem gem)
        {
            switch (gem)
            {
                case SocketGem.NORMAL_GLORY_GEM:
                case SocketGem.NORMAL_THUNDER_GEM:
                    return 100;
                case SocketGem.REFINED_GLORY_GEM:
                case SocketGem.REFINED_THUNDER_GEM:
                    return 300;
                case SocketGem.SUPER_GLORY_GEM:
                case SocketGem.SUPER_THUNDER_GEM:
                    return 500;
                default:
                    return 0;
            }
        }

        public static ushort BossLifePercent(int nLife, int nMaxLife)
        {
            return (ushort) ((nLife/nMaxLife)*10000);
        }
    }
}