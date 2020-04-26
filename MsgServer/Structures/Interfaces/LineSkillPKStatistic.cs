// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - Line Skill PK Statistic.cs
// Last Edit: 2017/02/20 08:30
// Created: 2017/02/20 08:30

using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Events;

namespace MsgServer.Structures.Interfaces
{
    public sealed class LineSkillPkStatistic
    {
        private DbDynamicRankRecord m_pDynaRank;

        public LineSkillPkStatistic(Character pUser)
        {
            m_pDynaRank = new DbDynamicRankRecord
            {
                PlayerName = pUser.Name,
                PlayerIdentity = pUser.Identity,
                RankType = LineSkillPkTournament.MAP_ID_U,
                ObjectIdentity = 0,
                ObjectName = "NONE"
            };

            Save();
        }

        public LineSkillPkStatistic(DbDynamicRankRecord pRank)
        {
            m_pDynaRank = pRank;
        }

        public uint Identity => m_pDynaRank.PlayerIdentity;
        public string Name => m_pDynaRank.PlayerName;

        public int HitsDealt
        {
            get { return (int) m_pDynaRank.Value3; }
            set
            {
                m_pDynaRank.Value3 = value;
                Save();
            }
        }

        public int HitsTaken
        {
            get { return (int)m_pDynaRank.Value4; }
            set
            {
                m_pDynaRank.Value4 = value;
                Save();
            }
        }

        public int HitsDealtNow
        {
            get { return (int)m_pDynaRank.Value1; }
            set
            {
                m_pDynaRank.Value1 = value;
                Save();
            }
        }

        public int HitsTakenNow
        {
            get { return (int)m_pDynaRank.Value2; }
            set
            {
                m_pDynaRank.Value2 = value;
                Save();
            }
        }

        public float LifetimeKDA
        {
            get
            {
                if (HitsTaken == 0)
                    return HitsDealt;
                if (HitsTaken < 0)
                    return 0;
                return HitsDealt/(float) HitsTaken;
            }
        }

        public float KDA
        {
            get
            {
                //if (HitsTakenNow == 0)
                //    return HitsDealtNow;
                //if (HitsTakenNow < 0)
                //    return 0;
                //return HitsDealtNow / (float)HitsTakenNow;
                return HitsDealtNow;
            }
        }

        public bool Save()
        {
            return ServerKernel.LineSkillPk.Repository.SaveOrUpdate(m_pDynaRank);
        }
    }
}