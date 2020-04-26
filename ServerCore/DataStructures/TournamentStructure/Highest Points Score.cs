// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Highest Points Score.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;
using System.Globalization;

namespace ServerCore.TournamentStructure
{
    /// <summary>
    /// Descibes a scoring scenarion where the highest score in points should win.
    /// </summary>
    public sealed class HighestPointsScore : Score
    {
        /// <summary>
        /// Initializes a new instance of the HighestPointsScore class with the specified number of points.
        /// </summary>
        /// <param name="points">The number of points that the new instance will represent.</param>
        public HighestPointsScore(double points)
        {
            Points = points;
        }

        /// <summary>
        /// Gets the number of points that this score represents.
        /// </summary>
        public double Points
        {
            get;
            private set;
        }

        /// <summary>
        /// Converts the points value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this instance.</returns>
        public override string ToString()
        {
            return Points.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return (int)Points;
        }

        /// <summary>
        /// Compares the value of this instance to a specified Score value
        /// and returns an integer that indicates whether this instance is better than,
        /// the same as, or worse than the specified Score value.
        /// </summary>
        /// <param name="other">A Score object to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and the value
        /// parameter.  Value Description: Less than zero: This instance is worse than
        /// value. Zero: This instance is the same as value. Greater than zero: This instance
        /// is better than value.
        /// </returns>
        public override int CompareTo(Score other)
        {
            var o = other as HighestPointsScore;

            if (o == null)
            {
                throw new InvalidOperationException();
            }

            return Points.CompareTo(o.Points);
        }

        /// <summary>
        /// Adds this instance to the specified score.  Used in overloading the '+' operator.
        /// </summary>
        /// <param name="addend">The other score to add to this instance.</param>
        /// <returns>A new instance of Score representing the sum of this instance and the addend.</returns>
        public override Score Add(Score addend)
        {
            if (addend == null)
            {
                return new HighestPointsScore(Points);
            }

            var a = addend as HighestPointsScore;

            if (a == null)
            {
                throw new InvalidOperationException();
            }

            return new HighestPointsScore(Points + a.Points);
        }
    }
}