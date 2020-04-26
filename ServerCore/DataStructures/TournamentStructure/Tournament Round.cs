// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Tournament Round.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;
using System.Collections.Generic;

namespace ServerCore.TournamentStructure
{
    public sealed class TournamentRound
    {
        private readonly List<TournamentPairing> pairings;

        public TournamentRound(IEnumerable<TournamentPairing> pair)
        {
            if (pair == null)
            {
                throw new ArgumentNullException("pair");
            }

            pairings = new List<TournamentPairing>(pair);
        }

        public TournamentRound(params TournamentPairing[] pairs)
        {
            if (pairs == null)
            {
                throw new ArgumentNullException("pairs");
            }

            pairings = new List<TournamentPairing>(pairs);
        }

        public IList<TournamentPairing> Pairings
        {
            get { return pairings.AsReadOnly(); }
        }

        public bool HasStarted { get; set; }

        public bool HasFinished { get; set; }
    }
}