// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Tournament Score.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;

namespace ServerCore.TournamentStructure
{
    public sealed class TournamentScore
    {
        private readonly Contestant contestant;

        public TournamentScore(Contestant cont, long? score)
        {
            if (cont == null)
            {
                throw new ArgumentNullException("Contestant may not be null.");
            }

            contestant = cont;
            Score = score;
        }

        public Contestant Contestant { get { return contestant; } }

        public long? Score { get; set; }
    }
}