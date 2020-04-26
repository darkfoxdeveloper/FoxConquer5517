// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - IQualifierRank.cs
// Last Edit: 2016/12/25 20:59
// Created: 2016/12/07 18:03

using System.Linq;
using DB.Entities;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Interfaces
{
    public class QualifierRankObj
    {
        private DbArena m_dbRole;
        private const uint LOCK_TIME = 60*60*24*7;

        public QualifierRankObj(DbArena pArena)
        {
            m_dbRole = pArena;
            Status = ArenaWaitStatus.NOT_SIGNED_UP;
        }

        public uint Ranking
        {
            get
            {
                if (TodayWins <= 0 && TodayLoses <= 0)
                    return 0;

                uint rank = 0;
                foreach (var cont in ServerKernel.ArenaRecord.Values
                    .Where(x => x.TodayWins > 0 || x.TodayLoses > 0)
                    .OrderByDescending(x => x.Points)
                    .ThenByDescending(x => x.TodayWins)
                    .ThenBy(x => x.TodayLoses))
                {
                    rank++;
                    if (cont.PlayerIdentity == PlayerIdentity)
                    {
                        break;
                    }
                }
                return rank;
            }
        }

        public uint Identity { get { return m_dbRole.Identity; } }

        public ArenaWaitStatus Status { get; set; }

        public uint PlayerIdentity { get { return m_dbRole.PlayerIdentity; } set { m_dbRole.PlayerIdentity = value; } }

        public string PlayerName { get { return m_dbRole.Name; } set { m_dbRole.Name = value; } }

        public uint Lookface { get { return m_dbRole.Lookface; } set { m_dbRole.Lookface = value; } }

        public byte Level { get { return m_dbRole.Level; } set { m_dbRole.Level = value; } }

        public ushort Profession { get { return m_dbRole.Profession; } set { m_dbRole.Profession = value; } }

        public uint TodayWins { get { return m_dbRole.WinsToday; } set { m_dbRole.WinsToday = value; } }

        public uint TodayLoses { get { return m_dbRole.LossToday; } set { m_dbRole.LossToday = value; } }

        public uint TotalWins { get { return m_dbRole.WinsTotal; } set { m_dbRole.WinsTotal = value; } }

        public uint TotalLoses { get { return m_dbRole.LossTotal; } set { m_dbRole.LossTotal = value; } }

        public uint Points { get { return m_dbRole.Points; } set { m_dbRole.Points = value; } }

        public uint HonorPoints { get { return m_dbRole.HonorPoints; } set { m_dbRole.HonorPoints = value; } }

        public uint TotalHonorPoints { get { return m_dbRole.TotalHonorPoints; } set { m_dbRole.TotalHonorPoints = value; } }

        public uint LastRanking { get { return m_dbRole.LastRank; } set { m_dbRole.LastRank = value; } }

        public uint LastSeasonWins { get { return m_dbRole.LastWin; } set { m_dbRole.LastWin = value; } }

        public uint LastSeasonsLoses { get { return m_dbRole.LastLoss; } set { m_dbRole.LastLoss = value; } }

        public uint LastSeasonPoints { get { return m_dbRole.LastPoints; } set { m_dbRole.LastPoints = value; } }

        public void SetLock()
        {
            m_dbRole.LockReleaseTime = (uint) (UnixTimestamp.Timestamp()+ LOCK_TIME);
            Save();
        }

        public bool IsLocked
        {
            get { return UnixTimestamp.Timestamp() < m_dbRole.LockReleaseTime; }
        }

        public void ClearLock()
        {
            m_dbRole.LockReleaseTime = 0;
            Save();
        }

        public bool Save() { return Database.ArenaRepository.SaveOrUpdate(m_dbRole); }

        public bool Delete() { return Database.ArenaRepository.Delete(m_dbRole); }
    }
}