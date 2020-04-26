// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Guide.cs
// Last Edit: 2016/12/06 22:37
// Created: 2016/12/06 22:37

using System;
using System.Linq;
using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public class Guide
    {
        private Character m_pOwner;
        private DbMentorAccess m_dbReward;

        private uint m_dwIdentity;
        private string m_szName;
        private ushort m_usOldBp;
        private uint m_dwEnrole = 20000101;

        public Guide(Character pOwner)
        {
            m_pOwner = pOwner;
        }

        public bool Create(uint dwTarget, uint dwEnrole)
        {
            Client pUser;
            if (!ServerKernel.Players.TryGetValue(dwTarget, out pUser))
            {
                DbUser user = new CharacterRepository().SearchByIdentity(dwTarget);
                if (user == null) return false;
                m_dwIdentity = user.Identity;
                m_szName = user.Name;
                return true;
            }
            if (pUser.Character == null)
                return false;

            m_dwIdentity = pUser.Character.Identity;
            m_szName = pUser.Character.Name;
            m_dwEnrole = dwEnrole;

            return true;
        }

        public uint Identity
        {
            get { return m_dwIdentity; }
        }

        public string Name
        {
            get { return m_szName; }
        }

        public uint EnroleDate
        {
            get { return m_dwEnrole; }
        }

        public Character Role
        {
            get
            {
                Client pUser;
                if (ServerKernel.Players.TryGetValue(m_dwIdentity, out pUser))
                    return pUser.Character ?? null;
                return null;
            }
        }

        public bool IsOnline
        {
            get { return ServerKernel.Players.ContainsKey(m_dwIdentity); }
        }

        public ushort SharedBattlePower
        {
            get
            {
                try
                {
                    if (Role == null)
                        return 0;

                    if (Role.PureBattlePower <= m_pOwner.PureBattlePower)
                        return 0;

                    ushort ret = 0;
                    DbMentorType share =
                        ServerKernel.MentorTypes.FirstOrDefault(
                            x => Role.Level >= x.UserMinLevel && Role.Level <= x.UserMaxLevel);
                    if (share == null) return 0;
                    ret = (ushort) (((Role.PureBattlePower) - m_pOwner.PureBattlePower)*(share.BattleLevelShare/100f));
                    MentorBattleLimit limit =
                        ServerKernel.MentorBattleLimits.FirstOrDefault(x => x.Id == m_pOwner.PureBattlePower);
                    if (limit == null) return 0;
                    ret = Math.Min(ret, limit.BattleLevelLimit);
                    if (ret != m_usOldBp)
                    {
                        m_usOldBp = ret;
                        m_pOwner.UpdateClient(ClientUpdateType.EXTRA_BATTLE_POWER, m_pOwner.SharedBattlePower);
                    }
                    return ret;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public void Send()
        {
            var msg = new MsgGuideInfo
            {
                Identity = m_pOwner.Identity,
                TargetIdentity = m_dwIdentity,
                Type = 1
            };
            msg.AddString(m_szName);
            msg.AddString(m_pOwner.Name);
            msg.RemainingTime = 999999;
            if (IsOnline)
            {
                msg.SharedBattlePower = SharedBattlePower;
                msg.TargetMesh = Role.Lookface;
                msg.TargetLevel = Role.Level;
                msg.TargetOnline = true;
                msg.TargetPkPoint = Role.PkPoints;
                msg.TargetProfession = (ProfessionType)Role.Profession;
                msg.EnroleDate = m_dwEnrole;
                msg.RemainingTime = 999999;
                if (Role.Syndicate != null)
                {
                    msg.SyndicateIdentity = (ushort)Role.Syndicate.Identity;
                    msg.SyndicatePosition = Role.SyndicateMember.Position;
                }
                msg.AddString(Role.Mate);
            }
            m_pOwner.Send(msg);
        }

        public void SendExpell()
        {
            var msg = new MsgGuideInfo
            {
                Identity = m_pOwner.Identity,
                Type = 1
            };
            msg.RemainingTime = 0;
            m_pOwner.Send(msg);
        }
    }
}