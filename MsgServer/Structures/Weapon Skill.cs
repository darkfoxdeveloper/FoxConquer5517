// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Weapon Skill.cs
// Last Edit: 2016/11/24 09:26
// Created: 2016/11/24 09:24

using System.Collections.Concurrent;
using System.Collections.Generic;
using DB.Entities;
using DB.Repositories;
using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public class WeaponSkill
    {
        private Character m_pOwner;
        public ConcurrentDictionary<ushort, DbWeaponSkill> Skills;

        public WeaponSkill(Character pOwner)
        {
            m_pOwner = pOwner;
            Skills = new ConcurrentDictionary<ushort, DbWeaponSkill>();

            IList<DbWeaponSkill> ws = new WeaponSkillRepository().FetchAll(m_pOwner.Identity);
            if (ws != null)
            {
                foreach (var wes in ws)
                {
                    Skills.TryAdd((ushort)wes.Type, wes);
                }
            }
        }

        public bool Create(ushort nType, byte nLevel)
        {
            if (Skills.ContainsKey(nType))
                return false;

            DbWeaponSkill dbSkill = new DbWeaponSkill
            {
                Experience = 0,
                Level = nLevel,
                Type = nType,
                OwnerIdentity = m_pOwner.Identity
            };
            Database.WeaponSkill.SaveOrUpdate(dbSkill);

            m_pOwner.Send(new MsgWeaponSkill(nType, nLevel, 0));
            return Skills.TryAdd(nType, dbSkill);
        }

        public bool AwardExperience(ushort nType, int nExp)
        {
            DbWeaponSkill dbSkill;
            if (!Skills.TryGetValue(nType, out dbSkill))
            {
                dbSkill = new DbWeaponSkill
                {
                    Experience = 0,
                    Level = 0,
                    Type = nType,
                    OwnerIdentity = m_pOwner.Identity
                };
                Skills.TryAdd(nType, dbSkill);
                Database.WeaponSkill.SaveOrUpdate(dbSkill);
            }

            if (dbSkill.Level >= 20)
                return true;

            dbSkill.Experience += (uint)nExp;
            if (dbSkill.Level < 20 && dbSkill.Experience > MsgWeaponSkill.EXP_PER_LEVEL[dbSkill.Level])
            {
                dbSkill.Experience -= MsgWeaponSkill.EXP_PER_LEVEL[dbSkill.Level];
                dbSkill.Level += 1;
            }
            Database.WeaponSkill.SaveOrUpdate(dbSkill);

            m_pOwner.Send(new MsgWeaponSkill(nType, dbSkill.Level, dbSkill.Experience));
            return true;
        }

        public void SendAll()
        {
            foreach (var skill in Skills.Values)
            {
                var pMsg = new MsgWeaponSkill(skill.Type, skill.Level, skill.Experience);
                m_pOwner.Send(pMsg);
            }
        }
    }
}