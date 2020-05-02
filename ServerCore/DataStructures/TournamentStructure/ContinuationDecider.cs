// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Continuation Decider.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;
using System.Collections.Generic;

namespace ServerCore.TournamentStructure
{
    public class ContinuationDecider : EliminationDecider
    {
        private EliminationNode nodeA = null;
        private EliminationNode nodeB = null;

        public ContinuationDecider(EliminationNode nodeA, EliminationNode nodeB)
        {
            if (nodeA == null)
            {
                throw new ArgumentNullException("nodeA");
            }

            if (nodeB == null)
            {
                throw new ArgumentNullException("nodeB");
            }

            this.nodeA = nodeA;
            this.nodeB = nodeB;
        }

        public EliminationNode ChildA
        {
            get
            {
                return nodeA;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                nodeA = value;
            }
        }

        public EliminationNode ChildB
        {
            get
            {
                return nodeB;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                nodeB = value;
            }
        }

        public override bool IsDecided
        {
            get
            {
                return nodeA.IsDecided && nodeB.IsDecided && ((nodeA.Team == null || nodeB.Team == null) || (nodeA.Score != null && nodeB.Score != null && nodeA.Score != nodeB.Score));
            }
        }

        public override Contestant GetWinner()
        {
            if (!nodeA.IsDecided)
            {
                throw new InvalidOperationException("Cannot determine a winner from a node that is undecided.");
            }

            if (!nodeB.IsDecided)
            {
                throw new InvalidOperationException("Cannot determine a winner from a node that is undecided.");
            }

            if (nodeB.Team == null)
            {
                return nodeA.Team;
            }
            else if (nodeA.Team == null)
            {
                return nodeB.Team;
            }

            if (nodeA.Score == null)
            {
                throw new InvalidOperationException("Cannot determine a winner from a node without a score.");
            }

            if (nodeB.Score == null)
            {
                throw new InvalidOperationException("Cannot determine a winner from a node without a score.");
            }

            if (nodeA.Score == nodeB.Score)
            {
                throw new InvalidOperationException("Cannot determine a winner from between two nodes with the same score.");
            }

            return nodeA.Score > nodeB.Score ? nodeA.Team : nodeB.Team;
        }

        public override Contestant GetLoser()
        {
            if (!nodeA.IsDecided)
            {
                throw new InvalidOperationException("Cannot determine a loser from a node that is undecided.");
            }

            if (!nodeB.IsDecided)
            {
                throw new InvalidOperationException("Cannot determine a loser from a node that is undecided.");
            }

            if (nodeA.Team == null)
            {
                return nodeA.Team;
            }
            else if (nodeB.Team == null)
            {
                return nodeB.Team;
            }

            if (nodeA.Score == null)
            {
                throw new InvalidOperationException("Cannot determine a loser from a node without a score.");
            }

            if (nodeB.Score == null)
            {
                throw new InvalidOperationException("Cannot determine a loser from a node without a score.");
            }

            if (nodeA.Score == nodeB.Score)
            {
                throw new InvalidOperationException("Cannot determine a loser from between two nodes with the same score.");
            }

            return nodeA.Score > nodeB.Score ? nodeB.Team : nodeA.Team;
        }

        public bool HasTeam(Contestant team)
        {
            return HasTeam(team.Identity);
        }

        public bool HasTeam(long teamId)
        {
            return (ChildA != null && ChildA.IsDecided && ChildA.Team.Identity == teamId) || (ChildB != null && ChildB.IsDecided && ChildB.Team.Identity == teamId);
        }

        public void SwapChildren()
        {
            var tempNode = nodeA;
            nodeA = nodeB;
            nodeB = tempNode;
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

            if (!nodeA.IsDecided || !nodeA.Locked || !nodeB.IsDecided || !nodeB.Locked)
            {
                return (!nodeA.IsDecided && nodeA.ApplyPairing(pairing)) || (!nodeB.IsDecided && nodeB.ApplyPairing(pairing));
            }
            else
            {
                var teamA = pairing.ContestantScores[0].Contestant;
                var scoreA = pairing.ContestantScores[0].Score;
                var teamB = pairing.ContestantScores[1].Contestant;
                var scoreB = pairing.ContestantScores[1].Score;

                if (teamA == null)
                {
                    teamA = teamB;
                    scoreA = scoreB;
                    teamB = null;
                    scoreB = null;
                }

                if (!TeamsMatch(teamA, nodeA.Team) || !TeamsMatch(teamB, nodeB.Team))
                {
                    var teamSwap = teamA;
                    var scoreSwap = scoreA;
                    teamA = teamB;
                    scoreA = scoreB;
                    teamB = teamSwap;
                    scoreB = scoreSwap;
                }

                if (TeamsMatch(teamA, nodeA.Team) && TeamsMatch(teamB, nodeB.Team))
                {
                    nodeA.Score = scoreA;
                    nodeB.Score = scoreB;
                    Lock();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool TeamsMatch(Contestant teamA, Contestant teamB)
        {
            if (object.ReferenceEquals(teamA, teamB))
            {
                return true;
            }
            else if (teamA == null || teamB == null)
            {
                return false;
            }
            else
            {
                return (teamA.Identity == teamB.Identity);
            }
        }

        public override IEnumerable<TournamentPairing> FindUndecided()
        {
            if (IsDecided)
            {
                yield break;
            }
            else if (nodeA.IsDecided && nodeB.IsDecided)
            {
                if (Locked)
                {
                    yield break;
                }
                else
                {
                    yield return new TournamentPairing(
                            new TournamentScore(nodeA.Team, null),
                            new TournamentScore(nodeB.Team, null));
                }
            }
            else
            {
                if (!nodeA.IsDecided)
                {
                    foreach (var undecided in nodeA.FindUndecided())
                    {
                        yield return undecided;
                    }
                }

                if (!nodeB.IsDecided)
                {
                    foreach (var undecided in nodeB.FindUndecided())
                    {
                        yield return undecided;
                    }
                }
            }
        }

        public override IEnumerable<EliminationNode> FindNodes(Func<EliminationNode, bool> filter)
        {
            if (nodeA != null)
            {
                foreach (var match in nodeA.FindNodes(filter))
                {
                    yield return match;
                }
            }

            if (nodeB != null)
            {
                foreach (var match in nodeB.FindNodes(filter))
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

            if (nodeA != null)
            {
                foreach (var match in nodeA.FindDeciders(filter))
                {
                    yield return match;
                }
            }

            if (nodeB != null)
            {
                foreach (var match in nodeB.FindDeciders(filter))
                {
                    yield return match;
                }
            }
        }
    }
}
