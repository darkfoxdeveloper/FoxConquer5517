// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Tournament Name Table.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51

using System.Collections.Generic;

namespace ServerCore.TournamentStructure
{
    /// <summary>
    /// Encapsulates a list of the names of tournament teams.
    /// </summary>
    public sealed class TournamentNameTable
    {
        /// <summary>
        /// Holds the mappings of team ids to team names.
        /// </summary>
        private readonly Dictionary<long, string> names;

        /// <summary>
        /// Initializes a new instance of the TournamentNameTable class, initialized with the supplied names.
        /// </summary>
        /// <param name="names">A pre-populated mapping of team ids to team names.</param>
        public TournamentNameTable(IDictionary<long, string> names)
        {
            this.names = new Dictionary<long, string>();

            foreach (var key in names.Keys)
            {
                this.names.Add(key, names[key]);
            }
        }

        /// <summary>
        /// Retrieves a team name associated with the supplied team id.
        /// </summary>
        /// <param name="teamId">The id of the team for which to retrieve the name.</param>
        /// <returns>The team name associated with the supplied team id.</returns>
        public string this[long teamId]
        {
            get
            {
                return names[teamId];
            }
        }
    }
}