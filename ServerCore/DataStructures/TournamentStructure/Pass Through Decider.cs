// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Pass Through Decider.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;
using System.Collections.Generic;

namespace ServerCore.TournamentStructure
{
    public class PassThroughDecider : EliminationDecider
    {
        private EliminationNode node = null;

        public PassThroughDecider(EliminationNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            this.node = node;
        }

        public override bool IsDecided
        {
            get
            {
                return node.IsDecided;
            }
        }

        public override Contestant GetWinner()
        {
            if (!node.IsDecided)
            {
                throw new InvalidOperationException("Cannot determine a winner from a node that is undecided.");
            }

            return node.Team;
        }

        public override Contestant GetLoser()
        {
            throw new InvalidOperationException("Cannot determine a loser from a pass through.");
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
            if (node != null)
            {
                foreach (var match in node.FindNodes(filter))
                {
                    yield return match;
                }
            }
        }

        public override IEnumerable<EliminationDecider> FindDeciders(Func<EliminationDecider, bool> filter)
        {
            if (filter.Invoke(this))
            {
                yield return this;
            }

            if (node != null)
            {
                foreach (var match in node.FindDeciders(filter))
                {
                    yield return match;
                }
            }
        }
    }
}
