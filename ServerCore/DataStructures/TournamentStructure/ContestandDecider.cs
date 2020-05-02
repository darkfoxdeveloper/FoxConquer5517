// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Contestand Decider.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;
using System.Collections.Generic;

namespace ServerCore.TournamentStructure
{
    public class TeamDecider : EliminationDecider
    {
        private Contestant team = null;

        public TeamDecider(Contestant team)
        {
            if (team == null)
            {
                throw new ArgumentNullException("team");
            }

            this.team = team;

            Lock();
        }

        public override bool IsDecided
        {
            get
            {
                return true;
            }
        }

        public override Contestant GetWinner()
        {
            return team;
        }

        public override Contestant GetLoser()
        {
            throw new InvalidOperationException("Cannot determine a loser from an individual team entry.");
        }

        public override bool ApplyPairing(TournamentPairing pairing)
        {
            if (pairing == null)
            {
                throw new ArgumentNullException("pairing");
            }

            return false;
        }

        public override IEnumerable<TournamentPairing> FindUndecided()
        {
            yield break;
        }

        public override IEnumerable<EliminationNode> FindNodes(Func<EliminationNode, bool> filter)
        {
            yield break;
        }

        public override IEnumerable<EliminationDecider> FindDeciders(Func<EliminationDecider, bool> filter)
        {
            if (filter.Invoke(this))
            {
                yield return this;
            }
        }
    }
}