// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Score.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;

namespace ServerCore.TournamentStructure
{
    public abstract class Score : IComparable<Score>, IEquatable<Score>
    {
        public static Score operator +(Score score1, Score score2)
        {
            if (score1 != null)
            {
                return score1.Add(score2);
            }

            if (score2 != null)
            {
                return score2.Add(score1);
            }

            return null;
        }

        public static bool operator ==(Score score1, Score score2)
        {
            if (object.ReferenceEquals(score1, score2))
            {
                return true;
            }

            if ((object)score1 == null || (object)score2 == null)
            {
                return false;
            }

            return score1.CompareTo(score2) == 0;
        }

        public static bool operator !=(Score score1, Score score2)
        {
            if (object.ReferenceEquals(score1, score2))
            {
                return false;
            }

            if ((object)score1 == null || (object)score2 == null)
            {
                return true;
            }

            return score1.CompareTo(score2) != 0;
        }

        public static bool operator >(Score score1, Score score2)
        {
            if (object.ReferenceEquals(score1, score2))
            {
                return false;
            }

            if ((object)score1 != null && (object)score2 == null)
            {
                return true;
            }

            if ((object)score1 == null && (object)score2 != null)
            {
                return false;
            }

            return score1.CompareTo(score2) > 0;
        }

        public static bool operator <(Score score1, Score score2)
        {
            if (object.ReferenceEquals(score1, score2))
            {
                return false;
            }

            if ((object)score1 != null && (object)score2 == null)
            {
                return false;
            }

            if ((object)score1 == null && (object)score2 != null)
            {
                return true;
            }

            return score1.CompareTo(score2) < 0;
        }

        public static bool operator >=(Score score1, Score score2)
        {
            if (object.ReferenceEquals(score1, score2))
            {
                return true;
            }

            if ((object)score1 != null && (object)score2 == null)
            {
                return true;
            }

            if ((object)score1 == null && (object)score2 != null)
            {
                return false;
            }

            return score1.CompareTo(score2) >= 0;
        }

        public static bool operator <=(Score score1, Score score2)
        {
            if (object.ReferenceEquals(score1, score2))
            {
                return true;
            }

            if ((object)score1 != null && (object)score2 == null)
            {
                return false;
            }

            if ((object)score1 == null && (object)score2 != null)
            {
                return true;
            }

            return score1.CompareTo(score2) <= 0;
        }

        public bool Equals(Score other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            var o = obj as Score;

            if (o == null)
            {
                return false;
            }

            return Equals(o);
        }

        public abstract override int GetHashCode();

        public abstract int CompareTo(Score other);

        public abstract Score Add(Score addend);
    }
}