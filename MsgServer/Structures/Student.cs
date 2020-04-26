// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Student.cs
// Last Edit: 2016/12/06 22:45
// Created: 2016/12/06 22:38

using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public class Student
    {
        private Character m_pOwner;

        private uint m_dwIdentity;
        private string m_szName;
        private uint m_dwExperience;
        private ushort m_usBlessing;
        private uint m_usComposing;
        private uint m_dwEnrole = 20000101;

        public Student(Character pOwner)
        {
            m_pOwner = pOwner;
        }

        public bool Create(uint idStudent, uint dwEnrole)
        {
            m_dwEnrole = dwEnrole;

            MentorContribution dbContri = Database.MentorContribution.FetchInformation(m_pOwner.Identity, idStudent);
            if (dbContri != null)
            {
                m_dwExperience = dbContri.Experience;
                m_usBlessing = dbContri.GodTime;
                m_usComposing = dbContri.PlusStone;
            }

            Client pUser;
            if (!ServerKernel.Players.TryGetValue(idStudent, out pUser))
            {
                DbUser dbUser = new CharacterRepository().SearchByIdentity(idStudent);
                if (dbUser == null) return false;
                m_dwIdentity = dbUser.Identity;
                m_szName = dbUser.Name;
                return true;
            }

            if (pUser.Character == null)
                return false;

            m_dwIdentity = pUser.Character.Identity;
            m_szName = pUser.Character.Name;

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

        public uint Experience
        {
            get { return m_dwExperience; }
            set { m_dwExperience = value; }
        }

        public uint Composing
        {
            get { return m_usComposing; }
            set { m_usComposing = value; }
        }

        public ushort Blessing
        {
            get { return m_usBlessing; }
            set { m_usBlessing = value; }
        }

        public bool IsOnline
        {
            get { return ServerKernel.Players.ContainsKey(m_dwIdentity); }
        }

        public Character Role
        {
            get
            {
                Client temp;
                if (!ServerKernel.Players.TryGetValue(m_dwIdentity, out temp) || temp.Character == null)
                    return null;
                return temp.Character;
            }
        }

        public void Send()
        {
            var msg = new MsgGuideInfo
            {
                Identity = m_pOwner.Identity,
                TargetIdentity = m_dwIdentity,
                ApprenticeExperience = m_dwExperience,
                ApprenticeBlessing = m_usBlessing,
                ApprenticeComposing = (ushort)(m_usComposing > ushort.MaxValue ? ushort.MaxValue : m_usComposing),
                Type = 2
            };
            msg.AddString(m_pOwner.Name);
            msg.AddString(m_szName);
            msg.RemainingTime = 999999;
            if (IsOnline)
            {
                Role.FetchMentorAndApprentice();
                msg.TargetMesh = Role.Lookface;
                msg.TargetLevel = Role.Level;
                msg.TargetOnline = true;
                msg.TargetPkPoint = Role.PkPoints;
                msg.TargetProfession = (ProfessionType)Role.Profession;
                msg.ApprenticeExperience = m_dwExperience = Role.StudentExperience;
                msg.ApprenticeBlessing = m_usBlessing = Role.StudentBlessing;
                m_usComposing = Role.StudentComposition;
                msg.ApprenticeComposing = (ushort)(m_usComposing > ushort.MaxValue ? ushort.MaxValue : m_usComposing);
                msg.EnroleDate = m_dwEnrole;
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
                Type = 2
            };
            msg.RemainingTime = 0;
            m_pOwner.Send(msg);
        }
    }
}