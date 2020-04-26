// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Stay Decider.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;
using System.Collections.Generic;

namespace ServerCore.TournamentStructure
{
    public class StayDecider : EliminationDecider
    {
        private EliminationNode previousWinnerNode = null;
        private EliminationNode stayNode = null;

        public StayDecider(EliminationNode previousWinnerNode, EliminationNode stayNode)
        {
            if (previousWinnerNode == null)
            {
                throw new ArgumentNullException("previousWinnerNode");
            }

            if (stayNode == null)
            {
                throw new ArgumentNullException("stayNode");
            }

            this.previousWinnerNode = previousWinnerNode;
            this.stayNode = stayNode;
        }

        public override bool IsDecided
        {
            get
            {
                return previousWinnerNode.IsDecided && stayNode.IsDecided && ((previousWinnerNode.Team.Identity == stayNode.Team.Identity) || (previousWinnerNode.Score != null && stayNode.Score != null && previousWinnerNode.Score != stayNode.Score));
            }
        }

        public override Contestant GetWinner()
        {
            if (!previousWinnerNode.IsDecided || !stayNode.IsDecided)
            {
                throw new InvalidOperationException("Cannot determine a winner from a node that is undecided.");
            }

            if (previousWinnerNode.Team.Identity == stayNode.Team.Identity)
            {
                return previousWinnerNode.Team;
            }

            if (previousWinnerNode.Score == null || stayNode.Score == null)
            {
                throw new InvalidOperationException("Cannot determine a winner from a node without a score.");
            }

            if (previousWinnerNode.Score == stayNode.Score)
            {
                throw new InvalidOperationException("Cannot determine a winner from between two nodes with the same score.");
            }

            return previousWinnerNode.Score > stayNode.Score ? previousWinnerNode.Team : stayNode.Team;
        }

        public override Contestant GetLoser()
        {
            if (!previousWinnerNode.IsDecided || !stayNode.IsDecided)
            {
                throw new InvalidOperationException("Cannot determine a loser from a node that is undecided.");
            }

            if (previousWinnerNode.Team.Identity == stayNode.Team.Identity)
            {
                throw new InvalidOperationException("Cannot determine a loser from a competition between a team and itself.");
            }

            if (previousWinnerNode.Score == null || stayNode.Score == null)
            {
                throw new InvalidOperationException("Cannot determine a loser from a node without a score.");
            }

            if (previousWinnerNode.Score == stayNode.Score)
            {
                throw new InvalidOperationException("Cannot determine a loser from between two nodes with the same score.");
            }

            return previousWinnerNode.Score > stayNode.Score ? stayNode.Team : previousWinnerNode.Team;
        }

        public override bool ApplyPairing(TournamentPairing pairing)
        {
            if (pairing == null)
            {
                throw new ArgumentNullException("pairing");
            }

            if (Locked)
            {
                // If we (and, all of our decentants) are decided, return false, indicating that no node below us is in a state that needs a score.
                return false;
            }

            if (pairing.ContestantScores.Count != 2 || pairing.ContestantScores[0] == null || pairing.ContestantScores[1] == null || pairing.ContestantScores[0].Contestant == null || pairing.ContestantScores[1].Contestant == null)
            {
                // If the pairing did not contain exactly two teams, or if either of the teams passed was null.
                throw new ArgumentException("A bye was passed as a pairing.", "pairing");
            }

            if (previousWinnerNode.IsDecided && stayNode.IsDecided && !(previousWinnerNode.Score != null || stayNode.Score != null))
            {
                // If our component nodes have played out, but we haven't
                var teamA = pairing.ContestantScores[0].Contestant;
                var scoreA = pairing.ContestantScores[0].Score;
                var teamB = pairing.ContestantScores[1].Contestant;
                var scoreB = pairing.ContestantScores[1].Score;

                if (previousWinnerNode.Team.Identity == teamB.Identity && stayNode.Team.Identity == teamA.Identity)
                {
                    // If the order of the pairing is reversed, we will normalize the pairing to us.
                    var teamSwap = teamA;
                    teamB = teamA;
                    teamA = teamSwap;

                    var scoreSwap = scoreA;
                    scoreB = scoreA;
                    scoreA = scoreSwap;
                }

                if (previousWinnerNode.Team.Identity == teamA.Identity && stayNode.Team.Identity == teamB.Identity)
                {
                    // If we are a match, assign the scores.
                    previousWinnerNode.Score = scoreA;
                    stayNode.Score = scoreB;
                    Lock();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return (!previousWinnerNode.IsDecided && previousWinnerNode.ApplyPairing(pairing)) || (!stayNode.IsDecided && stayNode.ApplyPairing(pairing));
            }
        }

        public override IEnumerable<TournamentPairing> FindUndecided()
        {
            if (IsDecided)
            {
                yield break;
            }
            else if (previousWinnerNode.IsDecided && stayNode.IsDecided)
            {
                if (Locked)
                {
                    yield break;
                }
                else
                {
                    yield return new TournamentPairing(
                            new TournamentScore(previousWinnerNode.Team, null),
                            new TournamentScore(stayNode.Team, null));
                }
            }
            else
            {
                if (!previousWinnerNode.IsDecided)
                {
                    foreach (var undecided in previousWinnerNode.FindUndecided())
                    {
                        yield return undecided;
                    }
                }

                if (!stayNode.IsDecided)
                {
                    foreach (var undecided in stayNode.FindUndecided())
                    {
                        yield return undecided;
                    }
                }
            }
        }

        public override IEnumerable<EliminationNode> FindNodes(Func<EliminationNode, bool> filter)
        {
            if (previousWinnerNode != null)
            {
                foreach (var match in previousWinnerNode.FindNodes(filter))
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

            if (previousWinnerNode != null)
            {
                foreach (var match in previousWinnerNode.FindDeciders(filter))
                {
                    yield return match;
                }
            }
        }
    }
}
