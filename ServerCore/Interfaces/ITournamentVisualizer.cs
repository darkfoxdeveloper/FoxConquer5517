// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - ITournamentVisualizer.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:52

using System;
using System.Collections.Generic;
using ServerCore.TournamentStructure;

namespace ServerCore.Interfaces
{
    /// <summary>
    /// Specifies the interface required for a tournament visualizer.
    /// </summary>
    public interface ITournamentVisualizer
    {
        /// <summary>
        /// Gets the name of the tournament visualizer.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Loads the specified teams and rounds as a state for the tournament visualizer.
        /// </summary>
        /// <param name="teams">The teams in play.</param>
        /// <param name="rounds">The rounds that have either been played or are scheduled to be played.</param>
        /// <exception cref="ArgumentNullException">When either the teams or the rounds parameter is null.</exception>
        /// <exception cref="InvalidTournamentStateException">When the state teams and rounds passed are in a state considered to be invalid to the tournament visualizer</exception>
        /// <remarks>The visualizer may be as lax or as strict in the enforcement of tournament state as the implementer desires.  However, it is recommended that implementations be lenient in what they accept.</remarks>
        void LoadState(IEnumerable<Contestant> teams, IList<TournamentRound> rounds);
    }
}
