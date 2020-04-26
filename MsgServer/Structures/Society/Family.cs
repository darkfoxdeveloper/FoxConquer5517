// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Family.cs
// Last Edit: 2016/12/28 19:30
// Created: 2016/12/05 05:55

using System;
using System.Collections.Concurrent;
using System.Linq;
using DB.Entities;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Society
{
    public sealed class Family
    {
        private DbFamily m_dbObj;

        private byte[] m_pPercentShare =
        {
            0,
            40,
            50,
            60,
            70
        };

        public ConcurrentDictionary<uint, FamilyMember> Members;
        public ConcurrentDictionary<uint, Family> Allies;
        public ConcurrentDictionary<uint, Family> Enemies; 

        public Family()
        {
            Members = new ConcurrentDictionary<uint, FamilyMember>();
            Allies = new ConcurrentDictionary<uint, Family>();
            Enemies = new ConcurrentDictionary<uint, Family>();
        }

        public bool Create(DbFamily dbFamily)
        {
            m_dbObj = dbFamily;
            return true;
        }

        public bool Create(Character pOwner, string szName)
        {
            m_dbObj = new DbFamily
            {
                Announce = "This is a new family.",
                CreationDate = (uint)UnixTimestamp.Timestamp(),
                Name = szName,
                LeaderName = pOwner.Name,
                LeaderIdentity = pOwner.Identity,
                Level = 0
            };
            return Save();
        }

        public uint Identity
        {
            get { return m_dbObj.Identity; }
        }

        public string Name { get { return m_dbObj.Name; } }

        public uint LeaderIdentity
        {
            get { return m_dbObj.LeaderIdentity; }
            set
            {
                m_dbObj.LeaderIdentity = value;
                Save();
            }
        }

        public string LeaderName
        {
            get { return m_dbObj.LeaderName; }
            set
            {
                m_dbObj.LeaderName = value;
                Save();
            }
        }

        public string Announcement
        {
            get { return m_dbObj.Announce; }
            set
            {
                m_dbObj.Announce = value;
                Save();
            }
        }

        public byte Level
        {
            get { return (byte) (m_dbObj.Level+1); }
            set
            {
                m_dbObj.Level = (byte) (value-1);
                Save();
            }
        }

        public byte BpTower
        {
            get { return m_dbObj.BattlePowerTower; }
            set
            {
                m_dbObj.BattlePowerTower = value;
                Save();
            }
        }

        public byte SharedPercent
        {
            get
            {
                if (BpTower >= m_pPercentShare.Length)
                    return 0;
                return m_pPercentShare[BpTower];
            }
        }

        public int MembersCount
        {
            get { return Members.Values.Count(x => x.Position == FamilyRank.MEMBER || x.Position == FamilyRank.CLAN_LEADER); }
            set
            {
                m_dbObj.Amount = (ushort) value;
                Save();
            }
        }

        public uint MoneyFunds
        {
            get { return m_dbObj.Money; }
            set
            {
                m_dbObj.Money = value;
                Save();
            }
        }

        public bool Deleted
        {
            get { return m_dbObj.DelFlag != 0; }
        }

        public bool ChangeFunds(int nAmount)
        {
            if (nAmount < 0)
            {
                if (nAmount + MoneyFunds < 0)
                {
                    MoneyFunds = (uint) Math.Max(nAmount + MoneyFunds, 0);
                    return false;
                }
                nAmount *= -1;
                MoneyFunds -= (uint) nAmount;
                return Save();
            }

            if (nAmount > 0)
            {
                if ((ulong)nAmount + MoneyFunds > ulong.MaxValue)
                {
                    MoneyFunds = uint.MaxValue;
                    return Save();
                }
                MoneyFunds += (uint) nAmount;
                return Save();
            }
            return false;
        }

        #region Level Management

        #endregion

        #region Battle Power

        #endregion

        #region Member Management

        public bool IsFull
        {
            get { return MembersCount >= 6; }
        }

        public bool AppendMember(Character pLeader, Character pTarget)
        {
            if (pTarget.Family != null)
            {
                pLeader.Send(ServerString.STR_FAMILY_TARGET_ALREADY_HAVE_FAMILY);
                return false;
            }

            if (pTarget.Level < 50)
            {
                pLeader.Send(ServerString.STR_FAMILY_JOIN_LOW_LEVEL);
                return false;
            }

            if (MembersCount >= 6)
            {
                pLeader.Send(ServerString.STR_FAMILY_FULL);
                return false;
            }

            FamilyRank pPos = FamilyRank.MEMBER;
            if (pTarget.Mate != "None")
            {
                DbUser pUser = Database.Characters.SearchByName(pTarget.Mate);
                if (pUser != null)
                {
                    DbFamilyMember dbSpouseMember = Database.FamilyMemberRepository.FetchByUser(pUser.Identity);
                    if (dbSpouseMember != null // has a clan
                        && dbSpouseMember.FamilyIdentity != Identity // not this one
                        && dbSpouseMember.Position != 11) // is core member or leader
                    {
                        pLeader.Send(ServerString.STR_FAMILY_SPOUSE_CANTDO);
                        return false;
                    }
                    if (dbSpouseMember != null // has a clan
                        && dbSpouseMember.FamilyIdentity == Identity // is this clan
                        && dbSpouseMember.Position != 11) // is core member or leader
                    {
                        pPos = FamilyRank.SPOUSE;
                    } 
                    else if (dbSpouseMember == null)
                    {
                        FamilyMember pSpouse = new FamilyMember(this);

                        if (!pSpouse.Create(pUser, new DbFamilyMember
                        {
                            FamilyIdentity = Identity,
                            Identity = pUser.Identity,
                            JoinDate = (uint) UnixTimestamp.Timestamp(),
                            Money = 0,
                            Position = (byte) FamilyRank.SPOUSE
                        }) || !Members.TryAdd(pSpouse.Identity, pSpouse))
                        {
                            return false;
                        }

                        Client pClientSpouse;
                        if (pSpouse.IsOnline && ServerKernel.Players.TryGetValue(pSpouse.Identity, out pClientSpouse))
                        {
                            pClientSpouse.Character.Family = this;
                            pClientSpouse.Character.FamilyMember = pSpouse;
                            pClientSpouse.Character.FamilyIdentity = Identity;
                            pClientSpouse.Character.FamilyPosition = pSpouse.Position;
                            pClientSpouse.Character.FamilyName = Name;
                            pClientSpouse.Character.SetNames();
                            SendFamily(pClientSpouse.Character);
                            SendRelation(pClientSpouse.Character);
                        }
                    }
                    else // ???
                    {
                        return false;
                    }
                }
            }

            FamilyMember pMember = new FamilyMember(this);
            if (!pMember.Create(pTarget))
            {
                return false;
            }
            pMember.Position = pPos;
            Members.TryAdd(pMember.Identity, pMember);

            pTarget.Family = this;
            pTarget.FamilyMember = pMember;
            pTarget.FamilyIdentity = Identity;
            pTarget.FamilyName = Name;
            pTarget.FamilyPosition = pMember.Position;
            pTarget.SetNames();
            SendFamily(pTarget);
            SendRelation(pTarget);
            return true;
        }

        public bool KickoutMember(FamilyMember pTarget)
        {
            if (pTarget.Position == FamilyRank.CLAN_LEADER)
                return false;
            if (pTarget.Position == FamilyRank.SPOUSE)
                return false;
            FamilyMember pCore;
            if (!Members.TryRemove(pTarget.Identity, out pCore))
                return false;

            DbUser pUserTarget = Database.Characters.SearchByIdentity(pTarget.Identity);
            if (pUserTarget.Mate != "None")
            {
                FamilyMember pMember = Members.Values.FirstOrDefault(x => x.Name == pUserTarget.Mate);
                if (pMember != null)
                {
                    Members.TryRemove(pMember.Identity, out pMember);
                    pMember.Delete();
                    
                    Client pClientSpouse;
                    if (pMember.IsOnline && ServerKernel.Players.TryGetValue(pMember.Identity, out pClientSpouse))
                    {
                        pClientSpouse.Character.Family = null;
                        pClientSpouse.Character.FamilyMember = null;
                        pClientSpouse.Character.FamilyIdentity = 0;
                        pClientSpouse.Character.FamilyName = "";
                        pClientSpouse.Character.FamilyPosition = FamilyRank.NONE;
                        pClientSpouse.Character.SetNames();
                        pClientSpouse.Character.SendEmptyFamily();
                        pClientSpouse.Character.Screen.RefreshSpawnForObservers();
                    }
                }
            }

            pCore.Delete();
            if (pTarget.IsOnline)
            {
                Client pClient;
                Character pRole;
                if (!ServerKernel.Players.TryGetValue(pTarget.Identity, out pClient))
                    return false;
                pRole = pClient.Character;
                pRole.Family = null;
                pRole.FamilyMember = null;
                pRole.FamilyIdentity = 0;
                pRole.FamilyName = "";
                pRole.FamilyPosition = FamilyRank.NONE;
                pRole.SetNames();
                pRole.SendEmptyFamily();
                pRole.Screen.RefreshSpawnForObservers();
            }
            return true;
        }

        #endregion

        #region Relation Management

        public bool AllyFamily(Family pTarget)
        {
            if (IsAlly(pTarget.Identity) || IsEnemy(pTarget.Identity))
                return false;

            if (Allies.Count >= 5 || pTarget.Allies.Count >= 5)
                return false;

            MsgFamily pMsg = new MsgFamily
            {
                Identity = pTarget.Identity,
                Type = FamilyType.SEND_ALLY
            };
            pMsg.AddRelation(pTarget.Identity, pTarget.Name, pTarget.LeaderName);
            Send(pMsg);
            pMsg = new MsgFamily
            {
                Identity = Identity,
                Type = FamilyType.SEND_ALLY
            };
            pMsg.AddRelation(Identity, Name, LeaderName);
            pTarget.Send(pMsg);

            pTarget.SetAlly(Identity);
            SetAlly(pTarget.Identity);
            return Save() && Allies.TryAdd(pTarget.Identity, pTarget)
                && pTarget.Save() && pTarget.Allies.TryAdd(Identity, this);
        }

        public bool SetAlly(uint idAlly)
        {
            if (Allies.Count >= 5)
                return false;

            if (m_dbObj.Ally0 > 0
                && m_dbObj.Ally1 > 0
                && m_dbObj.Ally2 > 0
                && m_dbObj.Ally3 > 0
                && m_dbObj.Ally4 > 0)
                return false;

            if (m_dbObj.Ally0 == 0)
                m_dbObj.Ally0 = idAlly;
            else if (m_dbObj.Ally1 == 0)
                m_dbObj.Ally1 = idAlly;
            else if (m_dbObj.Ally2 == 0)
                m_dbObj.Ally2 = idAlly;
            else if (m_dbObj.Ally3 == 0)
                m_dbObj.Ally3 = idAlly;
            else if (m_dbObj.Ally4 == 0)
                m_dbObj.Ally4 = idAlly;
            else
                return false;
            return true;
        }

        public void ClearAlly(uint idAlly)
        {
            if (m_dbObj.Ally0 == idAlly)
                m_dbObj.Ally0 = 0;
            else if (m_dbObj.Ally1 == idAlly)
                m_dbObj.Ally1 = 0;
            else if (m_dbObj.Ally2 == idAlly)
                m_dbObj.Ally2 = 0;
            else if (m_dbObj.Ally3 == idAlly)
                m_dbObj.Ally3 = 0;
            else if (m_dbObj.Ally4 == idAlly)
                m_dbObj.Ally4 = 0;
        }

        public bool RemoveAlly(Family pTarget)
        {
            if (!IsAlly(pTarget.Identity))
                return false;

            MsgFamily pMsg = new MsgFamily
            {
                Identity = pTarget.Identity,
                Type = FamilyType.DELETE_ALLY
            };
            pMsg.AddRelation(pTarget.Identity, pTarget.Name, pTarget.LeaderName);
            Send(pMsg);

            ClearAlly(pTarget.Identity);
            Family trash;
            return Save() && Allies.TryRemove(pTarget.Identity, out trash);
        }

        public bool EnemyFamily(Family pTarget)
        {
            if (IsAlly(pTarget.Identity) || IsEnemy(pTarget.Identity))
                return false;

            if (Enemies.Count >= 5)
                return false;

            MsgFamily pMsg = new MsgFamily
            {
                Identity = Identity,
                Type = FamilyType.SEND_ENEMY
            };
            pMsg.AddRelation(pTarget.Identity, pTarget.Name, pTarget.LeaderName);
            pTarget.Send(pMsg);

            SetEnemy(pTarget.Identity);
            return Save() && Enemies.TryAdd(pTarget.Identity, pTarget);
        }

        public bool SetEnemy(uint idEnemy)
        {
            if (Enemies.Count >= 5)
                return false;

            if (m_dbObj.Enemy0 > 0
                && m_dbObj.Enemy1 > 0
                && m_dbObj.Enemy2 > 0
                && m_dbObj.Enemy3 > 0
                && m_dbObj.Enemy4 > 0)
                return false;

            if (m_dbObj.Enemy0 == 0)
                m_dbObj.Enemy0 = idEnemy;
            else if (m_dbObj.Enemy1 == 0)
                m_dbObj.Enemy1 = idEnemy;
            else if (m_dbObj.Enemy2 == 0)
                m_dbObj.Enemy2 = idEnemy;
            else if (m_dbObj.Enemy3 == 0)
                m_dbObj.Enemy3 = idEnemy;
            else if (m_dbObj.Enemy4 == 0)
                m_dbObj.Enemy4 = idEnemy;
            else
                return false;
            return true;
        }

        public void ClearEnemy(uint idEnemy)
        {
            if (m_dbObj.Enemy0 == idEnemy)
                m_dbObj.Enemy0 = 0;
            else if (m_dbObj.Enemy1 == idEnemy)
                m_dbObj.Enemy1 = 0;
            else if (m_dbObj.Enemy2 == idEnemy)
                m_dbObj.Enemy2 = 0;
            else if (m_dbObj.Enemy3 == idEnemy)
                m_dbObj.Enemy3 = 0;
            else if (m_dbObj.Enemy4 == idEnemy)
                m_dbObj.Enemy4 = 0;
        }

        public bool RemoveEnemy(Family pTarget)
        {
            if (!IsEnemy(pTarget.Identity))
                return false;

            MsgFamily pMsg = new MsgFamily
            {
                Identity = pTarget.Identity,
                Type = FamilyType.DELETE_ENEMY
            };
            pMsg.AddRelation(pTarget.Identity, pTarget.Name, pTarget.LeaderName);
            pTarget.Send(pMsg);

            ClearEnemy(pTarget.Identity);
            Family trash;
            return Save() && Enemies.TryRemove(pTarget.Identity, out trash);
        }

        public bool IsAlly(uint idFamily)
        {
            return Allies.ContainsKey(idFamily);
        }

        public bool IsEnemy(uint idFamily)
        {
            return Enemies.ContainsKey(idFamily);
        }

        #endregion

        #region Loading

        public void SendRelation(Character pTarget)
        {
            if (pTarget.FamilyIdentity != Identity)
                return;

            foreach (var allies in Allies.Values)
            {
                MsgFamily pMsg = new MsgFamily
                {
                    Identity = Identity,
                    Type = FamilyType.SEND_ALLY
                };
                pMsg.AddRelation(allies.Identity, allies.Name, allies.LeaderName);
                pTarget.Send(pMsg);
            }

            foreach (var enemies in Enemies.Values)
            {
                MsgFamily pMsg = new MsgFamily
                {
                    Identity = Identity,
                    Type = FamilyType.SEND_ENEMY
                };
                pMsg.AddRelation(enemies.Identity, enemies.Name, enemies.LeaderName);
                pTarget.Send(pMsg);
            }
        }

        public void LoadRelations()
        {
            Allies.Clear();
            Enemies.Clear();
            if (m_dbObj.Ally0 > 0)
            {
                Family pAlly = ServerKernel.Families.Values.FirstOrDefault(x => x.Identity == m_dbObj.Ally0);
                if (pAlly == null || pAlly.Deleted)
                {
                    m_dbObj.Ally0 = 0;
                    Save();
                }
                else
                {
                    Allies.TryAdd(pAlly.Identity, pAlly);
                }
            }

            if (m_dbObj.Ally1 > 0)
            {
                Family pAlly = ServerKernel.Families.Values.FirstOrDefault(x => x.Identity == m_dbObj.Ally1);
                if (pAlly == null || pAlly.Deleted)
                {
                    m_dbObj.Ally1 = 0;
                    Save();
                }
                else
                {
                    Allies.TryAdd(pAlly.Identity, pAlly);
                }
            }

            if (m_dbObj.Ally2 > 0)
            {
                Family pAlly = ServerKernel.Families.Values.FirstOrDefault(x => x.Identity == m_dbObj.Ally2);
                if (pAlly == null || pAlly.Deleted)
                {
                    m_dbObj.Ally2 = 0;
                    Save();
                }
                else
                {
                    Allies.TryAdd(pAlly.Identity, pAlly);
                }
            }

            if (m_dbObj.Ally3 > 0)
            {
                Family pAlly = ServerKernel.Families.Values.FirstOrDefault(x => x.Identity == m_dbObj.Ally3);
                if (pAlly == null || pAlly.Deleted)
                {
                    m_dbObj.Ally3 = 0;
                    Save();
                }
                else
                {
                    Allies.TryAdd(pAlly.Identity, pAlly);
                }
            }

            if (m_dbObj.Ally4 > 0)
            {
                Family pAlly = ServerKernel.Families.Values.FirstOrDefault(x => x.Identity == m_dbObj.Ally4);
                if (pAlly == null || pAlly.Deleted)
                {
                    m_dbObj.Ally4 = 0;
                    Save();
                }
                else
                {
                    Allies.TryAdd(pAlly.Identity, pAlly);
                }
            }

            if (m_dbObj.Enemy0 > 0)
            {
                Family pEnemy = ServerKernel.Families.Values.FirstOrDefault(x => x.Identity == m_dbObj.Enemy0);
                if (pEnemy == null || pEnemy.Deleted)
                {
                    m_dbObj.Enemy0 = 0;
                    Save();
                }
                else
                {
                    Enemies.TryAdd(pEnemy.Identity, pEnemy);
                }
            }

            if (m_dbObj.Enemy1 > 0)
            {
                Family pEnemy = ServerKernel.Families.Values.FirstOrDefault(x => x.Identity == m_dbObj.Enemy1);
                if (pEnemy == null || pEnemy.Deleted)
                {
                    m_dbObj.Enemy1 = 0;
                    Save();
                }
                else
                {
                    Enemies.TryAdd(pEnemy.Identity, pEnemy);
                }
            }

            if (m_dbObj.Enemy2 > 0)
            {
                Family pEnemy = ServerKernel.Families.Values.FirstOrDefault(x => x.Identity == m_dbObj.Enemy2);
                if (pEnemy == null || pEnemy.Deleted)
                {
                    m_dbObj.Enemy2 = 0;
                    Save();
                }
                else
                {
                    Enemies.TryAdd(pEnemy.Identity, pEnemy);
                }
            }

            if (m_dbObj.Enemy3 > 0)
            {
                Family pEnemy = ServerKernel.Families.Values.FirstOrDefault(x => x.Identity == m_dbObj.Enemy3);
                if (pEnemy == null || pEnemy.Deleted)
                {
                    m_dbObj.Enemy3 = 0;
                    Save();
                }
                else
                {
                    Enemies.TryAdd(pEnemy.Identity, pEnemy);
                }
            }

            if (m_dbObj.Enemy4 > 0)
            {
                Family pEnemy = ServerKernel.Families.Values.FirstOrDefault(x => x.Identity == m_dbObj.Enemy4);
                if (pEnemy == null || pEnemy.Deleted)
                {
                    m_dbObj.Enemy4 = 0;
                    Save();
                }
                else
                {
                    Enemies.TryAdd(pEnemy.Identity, pEnemy);
                }
            }
        }

        #endregion

        #region Socket

        /// <summary>
        /// Send a message to all online clan members.
        /// </summary>
        /// <param name="pMsg">The byte buffer that will be sent to the user.</param>
        /// <param name="idUser">Set the user identity if the message should not be sent to him.</param>
        public void Send(byte[] pMsg, uint idUser = 0)
        {
            foreach (var pMember in Members.Values.Where(x => x.IsOnline))
            {
                try
                {
                    if (idUser != 0 && pMember.Identity == idUser)
                        continue;
                    pMember.Owner.Send(pMsg);
                }
                catch
                {
                    Console.WriteLine("Could not send msg to family member");
                }
            }
        }

        /// <summary>
        /// Send a message to all online clan members.
        /// </summary>
        /// <param name="pMsg">The byte buffer that will be sent to the user.</param>
        /// <param name="idUser">Set the user identity if the message should not be sent to him.</param>
        public void Send(string pMsg, uint idUser = 0)
        {
            foreach (var pMember in Members.Values.Where(x => x.IsOnline))
            {
                try
                {
                    if (idUser != 0 && pMember.Identity == idUser)
                        continue;
                    pMember.Owner.Send(pMsg);
                }
                catch
                {
                    Console.WriteLine("Could not send msg to family member");
                }
            }
        }

        public void SendFamily(Character pTarget)
        {
            if (pTarget.FamilyIdentity != Identity)
                return;

            MsgFamily pMsg = new MsgFamily
            {
                Identity = Identity,
                Type = FamilyType.INFO
            };
            pMsg.AddString(string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
                Identity,
                Members.Count,
                Members.Count,
                MoneyFunds,
                m_dbObj.Level,
                (int) pTarget.FamilyPosition,
                0,
                BpTower,
                0,
                0,
                1,
                pTarget.FamilyMember.Donation));
            pMsg.AddString(Name);
            pMsg.AddString(pTarget.Name);
            pTarget.Send(pMsg);
        }

        public void SendOccupation(Character pTarget)
        {
            if (pTarget.FamilyIdentity != Identity)
                return;

            MsgFamily pMsg = new MsgFamily
            {
                Identity=Identity,
                Type = FamilyType.MY_CLAN
            };
            pMsg.AddString(string.Format("{0} {1} {2} {3} {4} {5} {6}",
                0, 0, 0, 0, 0, 0, 0));
            pTarget.Send(pMsg);
            // uid#reward#nextreward#occupydays#name#currentmap#dominationmap#
        }

        public void SendMembers(Character pTarget)
        {
            if (pTarget.FamilyIdentity != Identity)
                return;

            MsgFamily pMsg = new MsgFamily
            {
                Identity = Identity,
                Type = FamilyType.MEMBERS
            };

            foreach (var member in
                    Members.Values.OrderByDescending(x => x.Position).ThenByDescending(x => x.IsOnline ? 1 : 0))
            {
                ushort pProf = 0;
                if (member.IsOnline)
                    pProf = member.Owner.Profession;
                pMsg.AddMember(member.Name, member.Level, pProf, member.Position, member.IsOnline, member.Donation);
            }
            pTarget.Send(pMsg);
        }

        #endregion

        #region Database

        public bool Save()
        {
            return m_dbObj != null && Database.FamilyRepository.SaveOrUpdate(m_dbObj);
        }

        public bool Delete()
        {
            if (m_dbObj != null)
            {
                m_dbObj.DelFlag = 1;
                return Database.FamilyRepository.SaveOrUpdate(m_dbObj);
            }
            return false;
        }
        #endregion
    }
}