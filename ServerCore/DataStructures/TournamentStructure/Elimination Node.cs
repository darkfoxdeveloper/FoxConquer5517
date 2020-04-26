// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Elimination Node.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System;
using System.Collections.Generic;

namespace ServerCore.TournamentStructure
{
    public abstract class EliminationNode
    {
        private EliminationDecider primaryParent;
        protected List<EliminationDecider> secondaryParents = new List<EliminationDecider>();

        protected EliminationDecider decider;

        public EliminationNode(EliminationDecider decider)
        {
            if (decider == null)
            {
                throw new ArgumentNullException("decider");
            }

            this.decider = decider;
        }

        public bool IsDecided
        {
            get
            {
                return decider.IsDecided;
            }
        }

        public EliminationDecider PrimaryParent
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
                    return 0;
                }
                else
                {
                    return primaryParent.Level + 1;
                }
            }
        }

        public EliminationNode CommonAncestor
        {
            get
            {
                if (primaryParent == null)
                {
                    return this;
                }
                else
                {
                    return primaryParent.CommonAncestor;
                }
            }
        }

        public bool Locked
        {
            get
            {
                return decider.Locked || (primaryParent != null && primaryParent.Locked);
            }
        }

        public abstract Contestant Team
        {
            get;
        }

        public EliminationDecider Decider
        {
            get
            {
                return decider;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                decider = value;
            }
        }

        public long? Score
        {
            get;
            set;
        }

        public abstract bool ApplyPairing(TournamentPairing pairing);
        public abstract IEnumerable<TournamentPairing> FindUndecided();
        public abstract IEnumerable<EliminationNode> FindNodes(Func<EliminationNode, bool> filter);
        public abstract IEnumerable<EliminationDecider> FindDeciders(Func<EliminationDecider, bool> filter);
    }
}
