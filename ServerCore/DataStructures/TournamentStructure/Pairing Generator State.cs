// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Pairing Generator State.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51
namespace ServerCore.TournamentStructure
{
    /// <summary>
    /// Describes the current state of a pairings generator.
    /// </summary>
    public enum PairingsGeneratorState
    {
        /// <summary>
        /// Indicates that the pairings generator is not initialized.
        /// </summary>
        NOT_INITIALIZED,

        /// <summary>
        /// Indicates that the pairings generator has been initialized to a valid state.
        /// </summary>
        INITIALIZED
    }
}