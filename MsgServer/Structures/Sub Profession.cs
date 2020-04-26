// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Sub Profession.cs
// Last Edit: 2016/11/24 11:34
// Created: 2016/11/24 11:33

using System.Collections.Concurrent;
using System.Linq;
using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public sealed class SubClass
    {
        private readonly int[] LEVEL_STUDY_MARTIAL_ARTIST = { 0, 300, 900, 1800, 2700, 3600, 5100, 6900, 8700, 0 };
        private readonly int[] LEVEL_STUDY_WARLOCK = { 0, 300, 900, 1800, 2700, 3600, 5100, 6900, 8700, 0 };
        private readonly int[] LEVEL_STUDY_CHI_MASTER = { 0, 600, 1800, 3600, 5400, 7200, 10200, 13800, 17400, 0 };
        private readonly int[] LEVEL_STUDY_SAGE = { 0, 400, 1200, 2400, 3600, 4800, 6800, 9200, 11600, 0 };
        private readonly int[] LEVEL_STUDY_APOTHECARY = { 0, 100, 200, 300, 400, 500, 1000, 4000, 9000, 0 };
        private readonly int[] LEVEL_STUDY_PERFORMER = { 0, 400, 1200, 2400, 3600, 4800, 6800, 9200, 11600, 0 };
        private readonly int[] LEVEL_STUDY_WRANGLER = { 0, 400, 1200, 2400, 3600, 4800, 6800, 9200, 11600, 0 };

        public readonly uint[] POWER_MARTIAL_ARTIST = { 0, 100, 200, 300, 400, 600, 800, 1000, 1200, 1500 };
        public readonly uint[] POWER_WARLOCK = { 0, 100, 200, 300, 400, 600, 800, 1000, 1200, 1500 };
        public readonly uint[] POWER_CHI_MASTER = { 0, 100, 200, 300, 400, 600, 800, 1000, 1200, 1500 };
        public readonly uint[] POWER_SAGE = { 0, 100, 200, 300, 400, 600, 800, 1000, 1200, 1500 };
        public readonly uint[] POWER_APOTHECARY = { 0, 8, 16, 24, 32, 40, 48, 56, 64, 72 };
        public readonly uint[] POWER_PERFORMER = { 0, 100, 200, 300, 400, 500, 600, 700, 800, 1000 };
        public readonly uint[] POWER_WRANGLER = { 0, 100, 200, 300, 400, 500, 600, 800, 1000, 1200 };

        private Character m_pOwner;
        public ConcurrentDictionary<SubClasses, ISubclass> Professions;

        public SubClass(Character pOwner)
        {
            m_pOwner = pOwner;
            Professions = new ConcurrentDictionary<SubClasses, ISubclass>();
        }

        public bool Create(SubClasses pType)
        {
            if (Professions.ContainsKey(pType))
                return false;

            var dbClass = new DbSubclass
            {
                Class = (byte)pType,
                Level = 1,
                Promotion = 1,
                Userid = m_pOwner.Identity
            };

            if (!Database.SubclassRepository.SaveOrUpdate(dbClass))
                return false;

            var pClass = new ISubclass
            {
                Database = dbClass,
                Class = pType,
                Level = 1,
                Promotion = 1
            };

            if (!Professions.TryAdd(pClass.Class, pClass))
                return false;

            var pMsg = new MsgSubPro
            {
                Action = SubClassActions.LEARN,
                Subclass = pType
            };
            m_pOwner.Send(pMsg);
            return true;
        }

        public bool Add(ISubclass isub)
        {
            return Professions.TryAdd(isub.Class, isub);
        }

        public bool Active(SubClasses sClass)
        {
            if (sClass == SubClasses.NONE)
            {
                var nsPacket = new MsgSubPro();
                nsPacket.Action = SubClassActions.ACTIVATE;
                nsPacket.Subclass = 0;
                m_pOwner.Send(nsPacket);
                return true;
            }

            if (!Professions.ContainsKey(sClass))
                return false;

            m_pOwner.ActiveSubclass = sClass;
            var sPacket = new MsgSubPro();
            sPacket.Action = SubClassActions.ACTIVATE;
            sPacket.Subclass = sClass;
            m_pOwner.Send(sPacket);
            return true;
        }

        public bool Uplev(SubClasses sClass)
        {
            ISubclass iclass;
            if (!Professions.TryGetValue(sClass, out iclass))
                return false;

            if (iclass.Level + 1 > 10) return false; // max level

            switch (sClass)
            {
                case SubClasses.APOTHECARY:
                    {
                        if (!SpendStudy(LEVEL_STUDY_APOTHECARY[iclass.Level]))
                            return false;
                        break;
                    }
                case SubClasses.CHI_MASTER:
                    {
                        if (!SpendStudy(LEVEL_STUDY_CHI_MASTER[iclass.Level]))
                            return false;
                        break;
                    }
                case SubClasses.MARTIAL_ARTIST:
                    {
                        if (!SpendStudy(LEVEL_STUDY_MARTIAL_ARTIST[iclass.Level]))
                            return false;
                        break;
                    }
                case SubClasses.PERFORMER:
                    {
                        if (!SpendStudy(LEVEL_STUDY_PERFORMER[iclass.Level]))
                            return false;
                        break;
                    }
                case SubClasses.SAGE:
                    {
                        if (!SpendStudy(LEVEL_STUDY_SAGE[iclass.Level]))
                            return false;
                        break;
                    }
                case SubClasses.WARLOCK:
                    {
                        if (!SpendStudy(LEVEL_STUDY_WARLOCK[iclass.Level]))
                            return false;
                        break;
                    }
                case SubClasses.WRANGLER:
                    {
                        if (!SpendStudy(LEVEL_STUDY_WRANGLER[iclass.Level]))
                            return false;
                        break;
                    }
                default:
                    return false;
            }

            iclass.Level += 1;
            if (!Save(iclass))
                return false;
            var sPacket = new MsgSubPro();
            sPacket.Action = SubClassActions.MARTIAL_UPLEV;
            sPacket.Subclass = sClass;
            m_pOwner.Send(sPacket);
            return true;
        }

        public bool Promote(SubClasses sClass)
        {
            ISubclass iClass;
            if (!Professions.TryGetValue(sClass, out iClass) || iClass.Promotion >= iClass.Level)
                return false;

            if (iClass.Promotion + 1 > 9) return false; // max level

            iClass.Promotion += 1;
            if (!Save(iClass))
                return false;

            var sPacket = new MsgSubPro();
            sPacket.Action = SubClassActions.MARTIAL_PROMOTED;
            sPacket.Subclass = sClass;
            sPacket.WriteByte(iClass.Promotion, 7);
            m_pOwner.Send(sPacket);
            m_pOwner.RecalculateAttributes();
            return true;
        }

        public bool AwardStudy(uint dwAmount)
        {
            if (dwAmount + m_pOwner.StudyPoints > uint.MaxValue)
                dwAmount = uint.MaxValue - m_pOwner.StudyPoints;

            if (dwAmount == 0) return false;

            m_pOwner.StudyPoints += dwAmount;

            SendStudy();
            return true;
        }

        public bool SpendStudy(int dwAmount)
        {
            if (dwAmount < 0) dwAmount *= -1;
            if (m_pOwner.StudyPoints - dwAmount < 0)
                return false;

            m_pOwner.StudyPoints -= (uint) dwAmount;

            SendStudy();
            return true;
        }

        public void SendStudy()
        {
            var pMsg = new MsgSubPro
            {
                Action = SubClassActions.UPDATE_STUDY,
                StudyPoints = m_pOwner.StudyPoints
            };
            m_pOwner.Send(pMsg);
        }

        public bool Save(ISubclass pObj)
        {
            return Database.SubclassRepository.SaveOrUpdate(pObj.Database);
        }

        public bool SaveAll()
        {
            foreach (var prof in Professions.Values)
                if (!Save(prof))
                    return false;
            return true;
        }

        public void SendAll()
        {
            var pMsg = new MsgSubPro
            {
                Action = SubClassActions.SHOW_GUI,
                StudyPoints = m_pOwner.StudyPoints
            };
            foreach (var prof in Professions.Values)
                pMsg.Append(prof.Class, prof.Promotion, prof.Level);
            m_pOwner.Send(pMsg);
        }

        public void LearnAll()
        {
            foreach (var prof in Professions.Values)
            {
                var pMsg = new MsgSubPro
                {
                    Action = SubClassActions.MARTIAL_PROMOTED,
                    Subclass = prof.Class
                };
                pMsg.WriteByte(prof.Promotion, 7);
                m_pOwner.Send(pMsg);
            }
        }

        public ISubclass this[byte sub]
        {
            get { return Professions.Values.FirstOrDefault(x => x.Class == (SubClasses)sub); }
        }

        public ISubclass this[SubClasses sub]
        {
            get { return Professions.Values.FirstOrDefault(x => x.Class == sub); }
        }
    }
}