// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Winner Node.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ServerCore.TournamentStructure
{
    [DebuggerDisplay("[WinnerNode: Level = {this.Level}, Decider = {this.Decider}]")]
    public class WinnerNode : EliminationNode
    {
        public WinnerNode(EliminationDecider decider)
            : base(decider)
        {
        }

        public override Contestant Team
        {
            get
            {
                return decider.GetWinner();
            }
        }

        public override bool ApplyPairing(TournamentPairing pairing)
        {
            if (pairing == null)
            {
                throw new ArgumentNullException("pairing");
            }

            if (IsDecided)
            {
                return false;
            }
            else
            {
                return decider.ApplyPairing(pairing);
            }
        }

        public override IEnumerable<TournamentPairing> FindUndecided()
        {
            if (IsDecided)
            {
                yield break;
            }
            else
            {
                foreach (var undecided in Decider.FindUndecided())
                {
                    yield return undecided;
                }
            }
        }

        public override IEnumerable<EliminationNode> FindNodes(Func<EliminationNode, bool> filter)
        {
            if (filter.Invoke(this))
            {
                yield return this;
            }

            foreach (var match in Decider.FindNodes(filter))
            {
                yield return match;
            }
        }

        public override IEnumerable<EliminationDecider> FindDeciders(Func<EliminationDecider, bool> filter)
        {
            foreach (var match in Decider.FindDeciders(filter))
            {
                yield return match;
            }
        }
    }
}