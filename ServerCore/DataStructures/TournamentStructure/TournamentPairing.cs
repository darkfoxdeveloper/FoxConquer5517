// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Tournament Pairing.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;
using System.Collections.Generic;

namespace ServerCore.TournamentStructure
{
    public sealed class TournamentPairing
    {
        private readonly List<TournamentScore> contestantScores;

        public TournamentPairing(IEnumerable<TournamentScore> scores)
        {
            if (scores == null)
            {
                throw new ArgumentNullException("scores");
            }
            contestantScores = new List<TournamentScore>(scores);
        }

        public TournamentPairing(params TournamentScore[] scores)
        {
            if (scores == null)
            {
                throw new ArgumentNullException("scores");
            }
            contestantScores = new List<TournamentScore>(scores);
        }

        public IList<TournamentScore> ContestantScores
        {
            get { return contestantScores.AsReadOnly(); }
        }

        public bool HasStarted { get; set; }

        public bool HasFinished { get; set; }
    }
}