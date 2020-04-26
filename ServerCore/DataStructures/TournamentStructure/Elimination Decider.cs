// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Elimination Decider.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;
using System.Collections.Generic;

namespace ServerCore.TournamentStructure
{
    public abstract class EliminationDecider
    {
        private EliminationNode primaryParent = null;
        protected List<EliminationNode> secondaryParents = new List<EliminationNode>();

        public void AddSecondaryParent(EliminationNode secondaryParent)
        {
            if (secondaryParent != null && !secondaryParents.Contains(secondaryParent))
            {
                secondaryParents.Add(secondaryParent);
            }
        }

        public EliminationNode PrimaryParent
        {
            get
            {
                return primaryParent;
            }

            set
            {
                primaryParent = value;
            }
        }

        public int Level
        {
            get
            {
                if (primaryParent == null)
                {
                    throw new InvalidOperationException("An elimination decider must have a parent EliminationNode.");
                }
                else
                {
                    return primaryParent.Level;
                }
            }
        }

        public EliminationNode CommonAncestor
        {
            get
            {
                if (primaryParent == null)
                {
                    throw new InvalidOperationException("An elimination decider must have a parent EliminationNode.");
                }
                else
                {
                    return primaryParent.CommonAncestor;
                }
            }
        }

        private bool locked = false;

        public bool Locked
        {
            get
            {
                return locked;
            }
        }

        public void Lock()
        {
            locked = true;
        }

        public abstract bool IsDecided
        {
            get;
        }
        public abstract Contestant GetWinner();
        public abstract Contestant GetLoser();
        public abstract bool ApplyPairing(TournamentPairing pairing);
        public abstract IEnumerable<TournamentPairing> FindUndecided();
        public abstract IEnumerable<EliminationNode> FindNodes(Func<EliminationNode, bool> filter);
        public abstract IEnumerable<EliminationDecider> FindDeciders(Func<EliminationDecider, bool> filter);
    }
}