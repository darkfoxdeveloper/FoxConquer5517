// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Team.cs
// Last Edit: 2016/12/06 20:54
// Created: 2016/12/06 20:49

using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using Core.Common.Enums;
using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public class Team
    {
        private Character m_pLeader;
        public ConcurrentDictionary<uint, Character> Members;
        private ConcurrentDictionary<int, IStatus> m_lAuras;

        private bool m_bCloseMoney = false, m_bCloseItem = true, m_bCloseGem = true, m_bForbid = false;

        public Team(Character leader)
        {
            m_pLeader = leader;
            Members = new ConcurrentDictionary<uint, Character>();
            m_lAuras = new ConcurrentDictionary<int, IStatus>(5, 5);
            Members.TryAdd(leader.Identity, leader);
        }

        public bool IsCloseMoney
        {
            get { return m_bCloseMoney; }
        }

        public bool IsCloseItem
        {
            get { return m_bCloseItem; }
        }

        public bool IsCloseGem
        {
            get { return m_bCloseGem; }
        }

        public bool IsForbid
        {
            get { return m_bForbid; }
        }

        public Character Leader
        {
            get { return m_pLeader; }
        }

        public void SetForbid(bool bTrue)
        {
            m_bForbid = bTrue;
        }

        public void SetCloseMoney(bool bClose)
        {
            m_bCloseMoney = bClose;
        }

        public void SetCloseItem(bool bClose)
        {
            m_bCloseItem = bClose;
        }

        public void SetCloseGem(bool bClose)
        {
            m_bCloseGem = bClose;
        }

        public bool Destroy(Character pRole, MsgTeam pMsg)
        {
            if (pRole != m_pLeader)
            {
                pRole.Send(ServerString.STR_NOT_CAPTAIN_DISMISS);
                return false;
            }

            Send(pMsg);

            foreach (var plr in Members.Values)
                plr.Team = null;
            pRole.DetachStatus(FlagInt.TEAM_LEADER);
            m_pLeader = null;
            Members.Clear();
            return true;
        }

        public bool IsTeamMember(Character pRole)
        {
            return IsTeamMember(pRole.Identity);
        }

        public bool IsTeamMember(uint idRole)
        {
            return Members.ContainsKey(idRole);
        }

        public void Send(string szMsg)
        {
            foreach (var usr in Members.Values)
                usr.Send(new MsgTalk(szMsg, ChatTone.TEAM, Color.White));
        }

        public void Send(byte[] pMsg)
        {
            foreach (var usr in Members.Values)
                usr.Send(pMsg);
        }

        public void Send(byte[] pMsg, uint idSender)
        {
            foreach (var usr in Members.Values.Where(x => x.Identity != idSender))
                usr.Send(pMsg);
        }

        public void LeaveTeam(Character pRole, MsgTeam pMsg)
        {
            Character trash;
            if (!Members.TryRemove(pRole.Identity, out trash))
                return;
            pRole.Team = null;
            if (pMsg != null)
                Send(pMsg);
            else
            {
                Send(new MsgTeam
                {
                    Type = TeamActionType.KICK,
                    Target = pRole.Identity
                });
                return;
            }
            pRole.Send(pMsg);
        }

        public int MembersCount()
        {
            return Members.Count;
        }

        public void KickMember(Character pRole, MsgTeam pMsg)
        {
            if (pRole != m_pLeader)
            {
                pRole.Send(ServerString.STR_NOT_LEADER_KICK);
                return;
            }

            Character pTarget;
            if (!Members.TryRemove(pMsg.Target, out pTarget))
            {
                pRole.Send(ServerString.STR_APPLICANT_NOT_FOUND);
                return;
            }
            pTarget.Send(pMsg);
            Send(pMsg);
            pTarget.Team = null;

            var aura = m_lAuras.Values.FirstOrDefault(x => x.CasterId == pRole.Identity);
            if (aura != null)
            {
                RemoveAura(aura);
            }
        }

        /// <summary>
        /// This method will send the request to the leader to accept.
        /// </summary>
        /// <param name="pSender">The user who wants to join the team</param>
        /// <param name="pTarget">The leader of the team</param>
        /// <param name="pMsg">The packet that will be processed</param>
        public void RequestMember(Character pSender, Character pTarget, MsgTeam pMsg)
        {
            if (pTarget != m_pLeader)
            {
                pSender.Send(ServerString.STR_NO_CAPTAIN_CLOSE);
                return;
            }

            if (IsForbid)
            {
                pSender.Send(ServerString.STR_FORBIDDEN_JOIN); // message to player
                //pTarget.Send(ServerString.STR_TEAM_CLOSED); // message to leader
                return;
            }

            if (Members.Count >= 5)
            {
                pSender.Send(ServerString.STR_HIS_TEAM_FULL);
                return;
            }

            pSender.SetTeamJoin(pTarget.Identity);
            pMsg.Target = pSender.Identity;
            pTarget.Send(pMsg);
            pTarget.SendRelation(pSender);
        }

        /// <summary>
        /// This method will be called when the leader is inviting the member.
        /// </summary>
        /// <param name="pSender">The user who is being invited.</param>
        /// <param name="pTarget">The leader of the team</param>
        /// <param name="pMsg">The packet that will be processed</param>
        public void InviteMember(Character pSender, Character pTarget, MsgTeam pMsg)
        {
            if (pTarget != m_pLeader)
            {
                pSender.Send(ServerString.STR_NOT_CAPTAIN_ACCEPT);
                return;
            }

            if (IsForbid)
            {
                // pSender.Send(ServerString.STR_FORBIDDEN_JOIN); // message to player
                pTarget.Send(ServerString.STR_TEAM_CLOSED); // message to leader
                return;
            }

            if (Members.Count >= _MAX_MEMBER)
            {
                pSender.Send(ServerString.STR_TEAM_FULL);
                return;
            }

            pTarget.SetTeamInvite(pSender.Identity);
            pMsg.Target = pTarget.Identity;
            pSender.Send(pMsg);
            pSender.SendRelation(pTarget);
        }

        /// <summary>
        /// This method is called when the leader accept an user request.
        /// </summary>
        public void AcceptMember(Character pLeader, Character pMember, MsgTeam pMsg)
        {
            if (pLeader != m_pLeader)
            {
                pLeader.Send(ServerString.STR_NOT_CAPTAIN_ACCEPT);
                return;
            }

            if (IsForbid)
            {
                // pLeader.Send(ServerString.STR_FORBIDDEN_JOIN); // message to player
                pLeader.Send(ServerString.STR_TEAM_CLOSED); // message to leader
                return;
            }

            if (Members.Count >= _MAX_MEMBER)
            {
                pLeader.Send(ServerString.STR_TEAM_FULL);
                return;
            }

            if (!pMember.FetchTeamJoin(pLeader.Identity))
                return;

            if (Members.TryAdd(pMember.Identity, pMember))
            {
                pMember.Send(pMsg);
                pMember.Team = this;
                SendMember();
                pMember.Send(string.Format(ServerString.STR_TEAM_MONEY, !m_bCloseMoney ? "ON" : "OFF"), ChatTone.TEAM);
                pMember.Send(string.Format(ServerString.STR_TEAM_ITEM, !m_bCloseItem ? "ON" : "OFF"), ChatTone.TEAM);
                pMember.Send(string.Format(ServerString.STR_TEAM_GEM, !m_bCloseGem ? "ON" : "OFF"), ChatTone.TEAM);
                pMember.ClearTeamJoin();
            }
        }

        /// <summary>
        /// This method is called when the user accept a team invitation.
        /// </summary>
        public void MemberAccept(Character pLeader, Character pMember, MsgTeam pMsg)
        {
            if (pLeader != m_pLeader)
            {
                pMember.Send(ServerString.STR_NO_CAPTAIN_CLOSE);
                return;
            }

            if (IsForbid)
            {
                pLeader.Send(ServerString.STR_FORBIDDEN_JOIN); // message to player
                // pLeader.Send(ServerString.STR_TEAM_CLOSED); // message to leader
                return;
            }

            if (Members.Count >= _MAX_MEMBER)
            {
                pMember.Send(ServerString.STR_HIS_TEAM_FULL);
                return;
            }

            if (!pLeader.FetchTeamInvite(pMember.Identity))
                return;

            if (Members.TryAdd(pMember.Identity, pMember))
            {
                pMember.Send(pMsg);
                pMember.Team = this;
                SendMember();
                pMember.Send(string.Format(ServerString.STR_TEAM_MONEY, !m_bCloseMoney ? "ON" : "OFF"), ChatTone.TEAM);
                pMember.Send(string.Format(ServerString.STR_TEAM_ITEM, !m_bCloseItem ? "ON" : "OFF"), ChatTone.TEAM);
                pMember.Send(string.Format(ServerString.STR_TEAM_GEM, !m_bCloseGem ? "ON" : "OFF"), ChatTone.TEAM);
                pLeader.ClearTeamInvite();
            }
        }

        public void SendMember()
        {
            foreach (var usr0 in Members.Values)
            {
                foreach (var usr1 in Members.Values)
                {
                    usr0.Send(new MsgTeamMember
                    {
                        Entity = usr1.Identity,
                        Life = (ushort)usr1.Life,
                        MaxLife = (ushort)usr1.MaxLife,
                        Mesh = usr1.Lookface,
                        Name = usr1.Name
                    });
                }
            }
        }

        public void SendTeam(Character pTarget)
        {
            foreach (var usr in Members.Values)
            {
                pTarget.Send(new MsgTeamMember
                {
                    Entity = usr.Identity,
                    Life = (ushort)usr.Life,
                    MaxLife = (ushort)usr.MaxLife,
                    Mesh = usr.Lookface,
                    Name = usr.Name
                });
            }
        }

        public void SendLeaderPosition(Character pSender)
        {
            if (m_pLeader == null)
            {
                return;
            }
            var pMsg = new MsgAction(m_pLeader.Identity, m_pLeader.MapIdentity, m_pLeader.MapX, m_pLeader.MapY,
                GeneralActionType.TEAM_MEMBER_POS);
            pSender.Send(pMsg);
        }

        public void SendMemberPosition(Character pSender, MsgAction pMsg)
        {
            Character pTarget;
            if (!Members.TryGetValue(pMsg.Identity, out pTarget))
                return;
            pMsg.X = pTarget.MapX;
            pMsg.Y = pTarget.MapY;
            pMsg.Data = pTarget.MapIdentity;
            pSender.Send(pMsg);
        }

        public void AwardMemberExp(uint idKiller, IRole pTarget, int nExp)
        {
            if (pTarget == null || nExp < 0) return;

            Client pClient;
            if (!ServerKernel.Players.TryGetValue(idKiller, out pClient) || pClient.Character == null)
                return;
            Character pKiller = pClient.Character;

            int nMonsterLev = pTarget.Level;
            foreach (var pUser in Members.Values)
            {
                // map, no self
                if (pUser.Map.Identity != pKiller.Map.Identity || pUser.Identity == pKiller.Identity)
                    continue;
                // no self
                if (!(pUser.IsAlive && pUser.Identity != idKiller))
                    continue;
                // distance
                if (Calculations.GetDistance(pUser.MapX, pUser.MapY, pKiller.MapX, pKiller.MapY) > 32)
                    continue; // out of range

                DbLevexp exp = ServerKernel.Levelxp.Values.FirstOrDefault(x => x.Level == pUser.Level);
                if (exp == null)
                    continue;
                int nAddExp = pUser.AdjustExperience(pTarget, nExp, false);
                int nMaxStuExp = (int)exp.Exp;
                nAddExp = Math.Min(nAddExp, nMaxStuExp);

                //if (!pKiller.IsNewbie() && pUser.IsNewbie())
                //{
                //    int nTutorExp = nAddExp * 10 / nMaxStuExp;
                //    // todo handle tutor exp
                //}

                if (nAddExp > pUser.Level * 360)
                    nAddExp = pUser.Level * 360;
                if (nAddExp <= 0)
                    nAddExp = 1;

                if (pUser.IsMate(pKiller))
                    nAddExp *= 2;

                pUser.AwardBattleExp(nAddExp, false);
                pUser.Send(string.Format(ServerString.STR_TEAM_EXPERIENCE, nAddExp));
            }
        }

        public void AddAura(IStatus pAura)
        {
            IStatus aura;
            if (m_lAuras.TryGetValue(pAura.Identity, out aura))
            {
                if (aura.Power >= pAura.Power)
                    return;
                m_lAuras.TryRemove(pAura.Identity, out aura);
            }
            m_lAuras.TryAdd(pAura.Identity, pAura);
        }

        public void RemoveAura(IStatus pAura)
        {
            IStatus trash;
            m_lAuras.TryRemove(pAura.Identity, out trash);
            foreach (var member in Members.Values)
            {
                member.DetachStatus(pAura.Identity);
            }
        }

        public void CheckAuras()
        {
            foreach (var aura in m_lAuras.Values)
            {
                foreach (var member in Members.Values)
                {
                    if (aura.CasterId == member.Identity)
                        continue;

                    Character pCaster;
                    if (member.QueryStatus(aura.Identity) == null)
                    {
                        if (Members.TryGetValue(aura.CasterId, out pCaster))
                        {
                            if (pCaster.MapIdentity == member.MapIdentity
                                && !pCaster.IsWatcher
                                && pCaster.GetDistance(member) < 36)
                            {
                                member.AttachStatus(member, aura.Identity, aura.Power, aura.Time, 0, aura.Level,
                                    aura.CasterId);
                            }
                        }
                        else
                        {
                            RemoveAura(aura);
                        }
                    }
                    else
                    {
                        if (Members.TryGetValue(aura.CasterId, out pCaster))
                        {
                            if (pCaster.GetDistance(member) >= 36 || pCaster.MapIdentity != member.MapIdentity)
                            {
                                member.DetachStatus(aura.Identity);
                            }
                        }
                        else
                        {
                            member.DetachStatus(aura.Identity);
                        }
                    }
                }
            }
        }

        private const int _MAX_MEMBER = 5;
    }
}