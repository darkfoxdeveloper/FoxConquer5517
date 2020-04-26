// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Request Box.cs
// Last Edit: 2016/12/29 18:42
// Created: 2016/12/27 15:20

using System;
using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Society;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public sealed class RequestBox
    {
        public uint OwnerIdentity { get; set; }

        public string OwnerName { get; set; }

        public uint ObjectIdentity { get; set; }

        public string ObjectName { get; set; }

        public RequestBoxType Type { get; set; }

        public string Message { get; set; }

        public void OnOk(Character pUsrAccept)
        {
            switch (Type)
            {
                case RequestBoxType.SYNDICATE_ALLY:
                {
                    Syndicate pAsk, pAccept;
                    if (ServerKernel.Syndicates.TryGetValue(OwnerIdentity, out pAsk)
                        && ServerKernel.Syndicates.TryGetValue(ObjectIdentity, out pAccept))
                    {
                        pAsk.AllySyndicate(pAccept);
                        pAsk.Send(string.Format(ServerString.STR_SYN_ALLY1, pAsk.LeaderName, pAccept.Name));
                        pAccept.Send(string.Format(ServerString.STR_SYN_ALLY0, pAccept.LeaderName, pAsk.Name));
                    }
                    return;
                }
                case RequestBoxType.FAMILY_ALLY:
                {
                    Family pAsk, pAccept;
                    if (ServerKernel.Families.TryGetValue(OwnerIdentity, out pAsk)
                        && ServerKernel.Families.TryGetValue(ObjectIdentity, out pAccept))
                    {
                        pAsk.AllyFamily(pAccept);
                    }
                    return;
                }
                case RequestBoxType.ADD_APPRENTICE:
                {
                    // Who accepts is the apprentice
                    // Owner == Mentor
                    // Object == Apprentice
                    Client pMentor, pApprentice;
                    if (ServerKernel.Players.TryGetValue(OwnerIdentity, out pMentor)
                        && ServerKernel.Players.TryGetValue(ObjectIdentity, out pApprentice))
                    {
                        if (!pMentor.Character.FetchStudentRequest(pMentor.Identity))
                            return;

                        if (pApprentice.Character.IsMentor(pMentor.Character) ||
                            pMentor.Character.IsMentor(pApprentice.Character))
                        {
                            pApprentice.Character.ClearStudentRequest();
                            return;
                        }

                        if (pMentor.Character.Metempsychosis < pApprentice.Character.Metempsychosis)
                        {
                            pApprentice.Character.ClearStudentRequest();
                            return;
                        }

                        if (pMentor.Character.Metempsychosis == pApprentice.Character.Metempsychosis
                            && pMentor.Character.Level - pApprentice.Character.Level < 20)
                        {
                            pApprentice.Character.ClearStudentRequest();
                            return;
                        }

                        Guide pGuide = new Guide(pApprentice.Character);
                        if (!pGuide.Create(pMentor.Identity, uint.Parse(DateTime.Now.ToString("yyyymmdd"))))
                        {
                            pApprentice.Character.ClearStudentRequest();
                            return;
                        }

                        Student pStudent = new Student(pMentor.Character);
                        if (!pStudent.Create(pApprentice.Identity, uint.Parse(DateTime.Now.ToString("yyyymmdd"))))
                        {
                            pApprentice.Character.ClearStudentRequest();
                            return;
                        }

                        pMentor.Character.Apprentices.TryAdd(pApprentice.Identity, pStudent);
                        pApprentice.Character.Mentor = pGuide;

                        DbMentor dbMentor = new DbMentor
                        {
                            Date = (uint)UnixTimestamp.Timestamp(),
                            GuideIdentity = pMentor.Character.Identity,
                            GuideName = pMentor.Character.Name,
                            StudentIdentity = pApprentice.Character.Identity,
                            StudentName = pApprentice.Character.Name
                        };
                        new MentorRepository().SaveOrUpdate(dbMentor);
                        pGuide.Send();
                        pStudent.Send();

                        pApprentice.Character.ClearStudentRequest();
                    }
                    break;
                }
                case RequestBoxType.ADD_MENTOR:
                {
                    // Who accepts is the mentor
                    // Owner == Apprentice
                    // Object == Mentor
                    Client pMentor, pApprentice;
                    if (ServerKernel.Players.TryGetValue(OwnerIdentity, out pApprentice)
                        && ServerKernel.Players.TryGetValue(ObjectIdentity, out pMentor))
                    {
                        if (!pApprentice.Character.FetchGuideRequest(pMentor.Identity))
                            return;

                        if (pApprentice.Character.IsMentor(pMentor.Character) ||
                            pMentor.Character.IsMentor(pApprentice.Character))
                        {
                            pApprentice.Character.ClearGuideRequest();
                            return;
                        }

                        if (pMentor.Character.Metempsychosis < pApprentice.Character.Metempsychosis)
                        {
                            pApprentice.Character.ClearGuideRequest();
                            return;
                        }

                        if (pMentor.Character.Metempsychosis == pApprentice.Character.Metempsychosis
                            && pMentor.Character.Level - pApprentice.Character.Level < 20)
                        {
                            pApprentice.Character.ClearGuideRequest();
                            return;
                        }

                        Guide pGuide = new Guide(pApprentice.Character);
                        if (!pGuide.Create(pMentor.Identity, uint.Parse(DateTime.Now.ToString("yyyymmdd"))))
                        {
                            pApprentice.Character.ClearGuideRequest();
                            return;
                        }

                        Student pStudent = new Student(pMentor.Character);
                        if (!pStudent.Create(pApprentice.Identity, uint.Parse(DateTime.Now.ToString("yyyymmdd"))))
                        {
                            pApprentice.Character.ClearGuideRequest();
                            return;
                        }

                        pMentor.Character.Apprentices.TryAdd(pApprentice.Identity, pStudent);
                        pApprentice.Character.Mentor = pGuide;

                        DbMentor dbMentor = new DbMentor
                        {
                            Date = (uint) UnixTimestamp.Timestamp(),
                            GuideIdentity = pMentor.Character.Identity,
                            GuideName = pMentor.Character.Name,
                            StudentIdentity = pApprentice.Character.Identity,
                            StudentName = pApprentice.Character.Name
                        };
                        new MentorRepository().SaveOrUpdate(dbMentor);
                        pGuide.Send();
                        pStudent.Send();

                        pApprentice.Character.ClearGuideRequest();
                    }
                    break;
                }
            }
        }

        public void OnCancel(Character pUsrAccept)
        {
            switch (Type)
            {
                case RequestBoxType.SYNDICATE_ALLY:
                {
                    Syndicate pCancel;
                    if (ServerKernel.Syndicates.TryGetValue(ObjectIdentity, out pCancel))
                    {
                        pUsrAccept.Send(string.Format(ServerString.STR_SYN_ALLY_DENY));
                    }
                    return;
                }
                case RequestBoxType.FAMILY_ALLY:
                {
                    Family pCancel;
                    if (ServerKernel.Families.TryGetValue(ObjectIdentity, out pCancel))
                    {
                        pUsrAccept.Send(string.Format(ServerString.STR_FAMILY_ALLY_DENY));
                    }
                    return;
                }
                case RequestBoxType.ADD_APPRENTICE:
                {
                    // Who accepts is the apprentice
                    // Owner == Mentor
                    // Object == Apprentice
                    Client pMentor;
                    if (ServerKernel.Players.TryGetValue(OwnerIdentity, out pMentor))
                    {
                        pMentor.Character.ClearStudentRequest();
                    }
                    break;
                }
                case RequestBoxType.ADD_MENTOR:
                {
                    // Who accepts is the mentor
                    // Owner == Apprentice
                    // Object == Mentor
                    Client pMentor;
                    if (ServerKernel.Players.TryGetValue(OwnerIdentity, out pMentor))
                    {
                        pMentor.Character.ClearStudentRequest();
                    }
                    break;
                }
            }
        }

        public void Send(Character pTarget)
        {
            pTarget.Send(new MsgTaskDialog(MsgTaskDialog.MESSAGE_BOX, Message));
        }
    }

    public enum RequestBoxType
    {
        SYNDICATE_ALLY,
        SYNDICATE_ENEMY,
        FAMILY_ALLY,
        FAMILY_ENEMY,
        ADD_MENTOR,
        ADD_APPRENTICE
    }
}