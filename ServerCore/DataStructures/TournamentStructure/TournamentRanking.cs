// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Tournament Ranking.cs
// Last Edit: 2016/11/23 07:58
// Created: 2016/11/23 07:51
namespace ServerCore.TournamentStructure
{
    /// <summary>
    /// Describes the position of a team in a tournament's rankings.
    /// </summary>
    public sealed class TournamentRanking
    {
        /// <summary>
        /// Holds the team being ranked.
        /// </summary>
        private readonly Contestant team;

        /// <summary>
        /// Holds the rank number of the ranking.
        /// </summary>
        private readonly double rank;

        /// <summary>
        /// Holds the score description or justification of the ranking.
        /// </summary>
        private readonly string scoreDescription;

        /// <summary>
        /// Initializes a new instance of the TournamentRanking class.
        /// </summary>
        /// <param name="team">The team being ranked.</param>
        /// <param name="rank">The actual rank number of the ranking.</param>
        /// <param name="scoreDescription">The score description or justification of the ranking.</param>
        public TournamentRanking(Contestant team, double rank, string scoreDescription)
        {
            this.team = team;
            this.rank = rank;
            this.scoreDescription = scoreDescription;
        }

        /// <summary>
        /// Gets the team being ranked.
        /// </summary>
        public Contestant Team
        {
            get
            {
                return team;
            }
        }

        /// <summary>
        /// Gets the rank number of the ranking.
        /// </summary>
        public double Rank
        {
            get
            {
                return rank;
            }
        }

        /// <summary>
        /// Gets the score description or justification of the ranking.
        /// </summary>
        public string ScoreDescription
        {
            get
            {
                return scoreDescription;
            }
        }
    }
}