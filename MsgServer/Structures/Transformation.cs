// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Transformation.cs
// Last Edit: 2016/12/06 20:21
// Created: 2016/12/06 20:19

using Core.Common.Enums;
using DB.Entities;
using MsgServer.Structures.Interfaces;
using ServerCore.Common;
using ServerCore.Common.Enums;

namespace MsgServer.Structures
{
    public class Transformation
    {
        private IRole m_pOwner;
        private DbMonstertype m_dbMonster;

        public Transformation(IRole pOwner)
        {
            m_pOwner = pOwner;
        }

        public bool Create(DbMonstertype pTrans)
        {
            if (m_pOwner == null || pTrans == null || pTrans.Life <= 0)
                return false;

            m_dbMonster = pTrans;
            m_usLife = (ushort)Calculations.CutTrail(1, Calculations.MulDiv(m_pOwner.Life, MaxLife, m_pOwner.MaxLife));

            return true;
        }

        private ushort m_usLife;

        public ushort Life
        {
            get { return m_usLife; }
            set { m_usLife = value; }
        }

        public ushort MaxLife
        {
            get { return (ushort)(m_dbMonster.Life > ushort.MaxValue ? ushort.MaxValue : m_dbMonster.Life); }
        }

        public int MinAttack
        {
            get { return m_dbMonster.AttackMin; }
        }

        public int MaxAttack
        {
            get { return m_dbMonster.AttackMax; }
        }

        public int Attack
        {
            get { return (MinAttack + MaxAttack) / 2; }
        }

        public int Defense
        {
            get { return m_dbMonster.Defence; }
        }

        public uint Defense2
        {
            get { return m_dbMonster.Defence2; }
        }

        public uint Dexterity
        {
            get { return m_dbMonster.Dexterity; }
        }

        public uint Dodge
        {
            get { return m_dbMonster.Dodge; }
        }

        public int MagicDefense
        {
            get { return m_dbMonster.MagicDef; }
        }

        public int InterAtkRate
        {
            get
            {
                int nRate = IntervalAtkRate;
                IStatus pStatus = m_pOwner.QueryStatus(FlagInt.CYCLONE);
                if (pStatus != null)
                    nRate = Calculations.CutTrail(0, Calculations.AdjustDataEx(nRate, pStatus.Power, 0));
                return nRate;
            }
        }

        public int IntervalAtkRate
        {
            get { return m_dbMonster.AttackSpeed; }
        }

        public int GetAttackRange(int nTargetSizeAdd)
        {
            return (int)((m_dbMonster.AttackRange + GetSizeAdd() + nTargetSizeAdd + 1) / 2);
        }

        public uint GetSizeAdd()
        {
            return m_dbMonster.SizeAdd;
        }

        public uint AttackHitRate
        {
            get { return m_dbMonster.Dexterity; }
        }

        public int MagicHitRate
        {
            get { return (int)m_dbMonster.MagicHitrate; }
        }

        public int Lookface
        {
            get { return m_dbMonster.Lookface; }
        }

        public bool IsJumpEnable
        {
            get { return (m_dbMonster.AttackUser & MonsterAttackMode.ATKUSER_JUMP) != 0; }
        }

        public bool IsMoveEnable
        {
            get { return (m_dbMonster.AttackUser & MonsterAttackMode.ATKUSER_FIXED) == 0; }
        }
    }
}