// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Syndicate Recruitment.cs
// Last Edit: 2017/01/27 16:50
// Created: 2017/01/27 16:50

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DB.Entities;
using DB.Repositories;
using MsgServer.Structures.Entities;
using Org.BouncyCastle.Bcpg.OpenPgp;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Society
{
    public sealed class SyndicateRecruitment
    {
        private const int _MAX_PER_PAGE_I = 4;
        private const int _MIN_MONEY_AMOUNT = 500000;
        private SyndicateRecruitmentRepository m_pRepo;

        private ConcurrentDictionary<uint, DbSyndicateAdvertising> m_pSyndicateAdvertisings = new ConcurrentDictionary<uint, DbSyndicateAdvertising>();

        public bool Create()
        {
            m_pRepo = new SyndicateRecruitmentRepository();
            var allSyn = m_pRepo.FetchAll();
            if (allSyn != null)
            {
                foreach (var syn in allSyn)
                {
                    m_pSyndicateAdvertisings.TryAdd(syn.SyndicateIdentity, syn);
                }
            }
            return true;
        }

        public void AddSyndicate(Character pSender, MsgSynRecuitAdvertising pMsg)
        {
            if (pSender == null || pSender.Syndicate == null)
                return;

            string szMessage = pMsg.Description;
            if (pSender.SyndicateRank != SyndicateRank.GUILD_LEADER)
            {
                return;
            }

            if (szMessage.Length > 255)
                szMessage = szMessage.Substring(0, 255);

            if (m_pSyndicateAdvertisings.Values.FirstOrDefault(x => x.SyndicateIdentity == pSender.SyndicateIdentity) != null)
            {
                EditSyndicate(pSender, pMsg);
                return;
            }

            if (pMsg.LevelRequirement <= 0)
                pMsg.LevelRequirement = 1;

            if (pSender.Syndicate.SilverDonation < pMsg.Amount)
            {
                pSender.Send(ServerString.STR_SYNRECRUIT_NOT_ENOUGH_MONEY);
                return;
            }

            if (pMsg.Amount < _MIN_MONEY_AMOUNT)
            {
                pSender.Send(ServerString.STR_SYNRECRUIT_NOT_ENOUGH_DONATION);
                return;
            }

            pSender.Syndicate.ChangeFunds((int)pMsg.Amount * -1);

            DbSyndicateAdvertising dbSyn = new DbSyndicateAdvertising
            {
                AutoRecruit = (byte) (pMsg.IsAutoRecruit ? 1 : 0),
                Donation = (uint) pMsg.Amount,
                Message = szMessage,
                RequiredLevel = pMsg.LevelRequirement,
                RequiredMetempsychosis = pMsg.RebornRequirement,
                RequiredProfession = pMsg.ProfessionForbid,
                SyndicateIdentity = pSender.SyndicateIdentity,
                SyndicateName = pSender.SyndicateName,
                Timestamp = (uint) UnixTimestamp.Timestamp()
            };
            if (m_pRepo.SaveOrUpdate(dbSyn))
            {
                m_pSyndicateAdvertisings.TryAdd(dbSyn.SyndicateIdentity, dbSyn);
            }
            else
            {
                pSender.Send("Oops! Something went wrong.");
            }
        }

        public void EditSyndicate(Character pSender, MsgSynRecuitAdvertising pMsg)
        {
            if (pSender == null || pSender.Syndicate == null)
                return;

            string szMessage = pMsg.Description;
            if (pSender.SyndicateRank != SyndicateRank.GUILD_LEADER)
            {
                return;
            }

            if (szMessage.Length > 255)
                szMessage = szMessage.Substring(0, 255);

            DbSyndicateAdvertising dbSyn;
            if (!m_pSyndicateAdvertisings.TryGetValue(pSender.SyndicateIdentity, out dbSyn))
            {
                return;
            }

            if (pMsg.LevelRequirement <= 0)
                pMsg.LevelRequirement = 1;

            if (pSender.Syndicate.SilverDonation < pMsg.Amount)
            {
                pSender.Send(ServerString.STR_SYNRECRUIT_NOT_ENOUGH_MONEY);
                return;
            }

            if (pMsg.Amount < _MIN_MONEY_AMOUNT)
            {
                pSender.Send(ServerString.STR_SYNRECRUIT_NOT_ENOUGH_DONATION);
                return;
            }

            pSender.Syndicate.ChangeFunds((int)pMsg.Amount*-1);

            {
                dbSyn.AutoRecruit = (byte) (pMsg.IsAutoRecruit ? 1 : 0);
                dbSyn.Donation = (uint)pMsg.Amount;
                dbSyn.Message = szMessage;
                dbSyn.RequiredLevel = pMsg.LevelRequirement;
                dbSyn.RequiredMetempsychosis = pMsg.RebornRequirement;
                dbSyn.RequiredProfession = pMsg.ProfessionForbid;
                dbSyn.SyndicateIdentity = pSender.SyndicateIdentity;
                dbSyn.SyndicateName = pSender.SyndicateName;
                dbSyn.Timestamp = (uint)UnixTimestamp.Timestamp();
            }
            if (m_pRepo.SaveOrUpdate(dbSyn))
            {
                // m_pSyndicateAdvertisings.TryAdd(dbSyn.SyndicateIdentity, dbSyn);
            }
            else
            {
                pSender.Send("Oops! Something went wrong.");
            }
        }

        public void SendEditScreen(Character pSender)
        {
            if (pSender == null || pSender.Syndicate == null || pSender.SyndicateRank != SyndicateRank.GUILD_LEADER)
                return;

            DbSyndicateAdvertising dbSyn;
            if (!m_pSyndicateAdvertisings.TryGetValue(pSender.SyndicateIdentity, out dbSyn))
            {
                return;
            }
            MsgSynRecuitAdvertising pMsg = new MsgSynRecuitAdvertising
            {
                IsAutoRecruit = dbSyn.AutoRecruit > 0,
                Amount = dbSyn.Donation,
                Description = dbSyn.Message,
                GenderForbid = 0,
                Identity = dbSyn.SyndicateIdentity,
                LevelRequirement = (ushort) dbSyn.RequiredLevel,
                ProfessionForbid = (ushort) dbSyn.RequiredProfession,
                RebornRequirement = (ushort) dbSyn.RequiredMetempsychosis
            };
            pSender.Send(pMsg);
        }

        public void JoinSyndicate(Character pUser, uint idSyn)
        {
            Syndicate pSyn;
            if (!ServerKernel.Syndicates.TryGetValue(idSyn, out pSyn))
            {
                pUser.Send(ServerString.STR_SYNRECRUIT_UNEXISTENT_SYN);
                return;
            }
            DbSyndicateAdvertising dbSyn;
            if (!m_pSyndicateAdvertisings.TryGetValue(idSyn, out dbSyn))
            {
                return;
            }

            if (!IsValid(dbSyn))
                return;

            if (pSyn.Deleted)
            {
                pUser.Send(ServerString.STR_SYNRECRUIT_UNEXISTENT_SYN);
                return;
            }

            if (!pSyn.AppendMember(pUser))
            {
                pUser.Send(ServerString.STR_SYNRECRUIT_NOT_MEET_REQUIREMENTS);
                return;
            }
        }

        private bool IsValid(DbSyndicateAdvertising pSyn)
        {
            return pSyn.Timestamp + 60*60*24*7 > UnixTimestamp.Timestamp();
        }

        public void CheckSyndicates()
        {
            List<DbSyndicateAdvertising> pRemove = new List<DbSyndicateAdvertising>();
            foreach (var syn in m_pSyndicateAdvertisings.Values)
            {
                if (!IsValid(syn))
                {
                    pRemove.Add(syn);
                }
            }
            foreach (var syn in pRemove)
            {

                DbSyndicateAdvertising pSyn;
                m_pSyndicateAdvertisings.TryRemove(syn.SyndicateIdentity, out pSyn);
                m_pRepo.Delete(syn);
            }
        }

        public bool IsAdvertising(uint idSyn)
        {
            return m_pSyndicateAdvertisings.ContainsKey(idSyn);
        }

        public void SendSyndicates(uint nPage, MsgSynRecruitAdvertisingList pMsg, Character pUser)
        {
            uint beginAt = pMsg.StartIndex;
            pMsg.FirstMatch = 1;
            pMsg.MaxCount = (uint) m_pSyndicateAdvertisings.Count;

            int nAmount = 0;
            foreach (var syn in m_pSyndicateAdvertisings.Values.OrderByDescending(x => x.Donation).ThenBy(x => x.Identity))
            {
                if (nAmount < beginAt)
                {
                    nAmount++;
                    continue;
                }
                if (nAmount >= beginAt + _MAX_PER_PAGE_I)
                    break;
                Syndicate pSyn;
                if (!ServerKernel.Syndicates.TryGetValue(syn.SyndicateIdentity, out pSyn) || pSyn.Deleted)
                    continue;
                if (pMsg.Count >= 2)
                {
                    pUser.Send(pMsg);
                    pMsg = new MsgSynRecruitAdvertisingList
                    {
                        StartIndex = nPage,
                        MaxCount = (uint) m_pSyndicateAdvertisings.Count
                    };
                }
                pMsg.Append(syn.SyndicateIdentity, syn.Message, syn.SyndicateName, pSyn.LeaderName, pSyn.Level, pSyn.MemberCount,
                    pSyn.SilverDonation);
                nAmount++;
            }

            pUser.Send(pMsg);
        }
    }
}