// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - Tournament Matches.cs
// Last Edit: 2017/03/01 14:13
// Created: 2017/02/21 16:22

using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;

namespace MsgServer.Structures.Tournament
{
    public class SingleTournamentMatch
    {
        public uint Identity0 => Player0?.Identity ?? 0;
        public uint Identity1 => Player1?.Identity ?? 0;

        public string Name0 => Player0?.Name ?? "";
        public string Name1 => Player1?.Name ?? "";

        public Character Player0;
        public Character Player1;

        public Character Winner;

        public UserTournamentStatus Status0 = UserTournamentStatus.NONE;
        public UserTournamentStatus Status1 = UserTournamentStatus.NONE;

        public uint Score0;
        public uint Score1;

        public uint Wage0;
        public uint Wage1;

        public void SetWinner(uint idRole, UserTournamentStatus reason)
        {
            if (Identity0 == idRole)
            {
                Winner = Player0;
                Status0 = UserTournamentStatus.WON_MATCH;
                Status1 = reason;
            }
            else if (Identity1 == idRole)
            {
                Winner = Player1;
                Status0 = reason;
                Status1 = UserTournamentStatus.WON_MATCH;
            }
        }
    }

    public class DoubleTournamentMatch
    {
        public const int FIRST_MATCH = 0,
            SECOND_MATCH = 1,
            FINISHED = 2;

        public uint Identity0 => Player0?.Identity ?? 0;
        public uint Identity1 => Player1?.Identity ?? 0;
        public uint Identity2 => Player2?.Identity ?? 0;

        public string Name0 => Player0?.Name ?? "";
        public string Name1 => Player1?.Name ?? "";
        public string Name2 => Player2?.Name ?? "";

        public Character Player0;
        public Character Player1;
        public Character Player2;

        public UserTournamentStatus Status0 = UserTournamentStatus.NONE;
        public UserTournamentStatus Status1 = UserTournamentStatus.NONE;
        public UserTournamentStatus Status2 = UserTournamentStatus.NONE;

        public uint Score0;
        public uint Score1;

        public uint Wage0;
        public uint Wage1;

        public Character Winner;

        private int m_pStage = FIRST_MATCH;

        public void SetWinner(uint idRole, UserTournamentStatus reason)
        {
            // todo re-check the logic, there might be a better way to do this lulz
            if (m_pStage == FIRST_MATCH)
            {
                if (Score0 > Score1)
                {
                    Winner = Player0;
                    Status0 = UserTournamentStatus.WON_MATCH;
                    Status1 = reason;
                }
                else
                {
                    Winner = Player1;
                    Status0 = reason;
                    Status1 = UserTournamentStatus.WON_MATCH;
                }
                m_pStage = SECOND_MATCH;
            }
            else if (m_pStage == SECOND_MATCH)
            {
                if (Winner == Player0)
                {
                    if (Score0 > Score1)
                    {
                        Winner = Player0;
                        Status0 = UserTournamentStatus.WON_MATCH;
                        Status2 = reason;
                    }
                    else
                    {
                        Winner = Player2;
                        Status0 = reason;
                        Status2 = UserTournamentStatus.WON_MATCH;
                    }
                }
                else if (Winner == Player1)
                {
                    if (Score0 > Score1)
                    {
                        Winner = Player1;
                        Status1 = UserTournamentStatus.WON_MATCH;
                        Status2 = reason;
                    }
                    else
                    {
                        Winner = Player2;
                        Status1 = reason;
                        Status2 = UserTournamentStatus.WON_MATCH;
                    }
                }
                m_pStage = FINISHED;
            }
        }
    }
}