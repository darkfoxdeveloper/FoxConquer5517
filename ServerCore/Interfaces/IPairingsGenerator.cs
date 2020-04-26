// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - IPairingsGenerator.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:52

using System;
using System.Collections.Generic;
using ServerCore.TournamentStructure;

namespace ServerCore.Interfaces
{
    /// <summary>
    /// Describes a pairings generator that will run a tournament.
    /// </summary>
    public interface IPairingsGenerator
    {
        /// <summary>
        /// Gets the name of the pairings generator.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the state of the pairings generator.
        /// </summary>
        PairingsGeneratorState State { get; }

        /// <summary>
        /// Gets a value indicating whether or not the pairings generator supports adding teams once a round has been generated.
        /// </summary>
        bool SupportsLateEntry { get; }

        /// <summary>
        /// Resets the pairings generator's initial state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Loads the specified teams and rounds as a state for the pairings generator.
        /// </summary>
        /// <param name="teams">The teams in play.</param>
        /// <param name="rounds">The rounds that have either been played or are scheduled to be played.</param>
        /// <exception cref="ArgumentNullException">When either the teams or the rounds parameter is null.</exception>
        /// <exception cref="Exception">When the state teams and rounds passed are in a state considered to be invalid to the pairings generator.</exception>
        /// <remarks>The generator may be as lax or as strict in the enforcement of tournament state as the implementer desires.  However, it is recommended that implementations be lenient in what they accept.</remarks>
        void LoadState(IEnumerable<Contestant> teams, IList<TournamentRound> rounds);

        /// <summary>
        /// Creates the next round for the current internal state.
        /// </summary>
        /// <param name="places">The number of places (such as game servers or basketball courts) available for users to compete on.  Must be null or greater than zero.</param>
        /// <returns>The next round of pairings for the tournament.</returns>
        TournamentRound CreateNextRound(int? places);

        /// <summary>
        /// Creates the list of rankings for the current state of the tournament.
        /// </summary>
        /// <returns>The list of rankings.</returns>
        IEnumerable<TournamentRanking> GenerateRankings();
    }
}