// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - 2065 - MsgGuide.cs
// Last Edit: 2017/02/04 17:32
// Created: 2017/02/04 14:43

using System.Linq;
using DB.Entities;
using DB.Repositories;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleGuideRequest(Character pRole, MsgGuide pMsg)
        {
            switch (pMsg.Type)
            {
                    #region Request Mentor

                case MentorRequest.REQUEST_MENTOR:
                {
                    if (pRole.Mentor == null)
                    {
                        Client pUser;
                        if (pRole.Mentor == null && ServerKernel.Players.TryGetValue(pMsg.Param, out pUser))
                        {
                            if (pUser.Character == null)
                                return;
                            Character pTargetRole = pUser.Character;

                            if (pTargetRole.Level < pRole.Level ||
                                (pTargetRole.Level - pRole.Level < 20 &&
                                 pTargetRole.Metempsychosis <= pRole.Metempsychosis))
                            {
                                pRole.Send(ServerString.STR_GUIDE_TUTOR_LOW_LEVEL1);
                                return;
                            }

                            if (pTargetRole.Metempsychosis < pRole.Metempsychosis)
                            {
                                pRole.Send(ServerString.STR_GUIDE_TUTOR_LOW_LEVEL1);
                                return;
                            }

                            DbMentorType maxStud =
                                ServerKernel.MentorTypes.FirstOrDefault(
                                    x => pTargetRole.Level >= x.UserMinLevel && pTargetRole.Level <= x.UserMaxLevel);

                            if (maxStud == null)
                                return;

                            if (pRole.IsApprentice(pTargetRole) || pTargetRole.IsApprentice(pRole))
                                return;

                            if (pTargetRole.Apprentices.Count >= maxStud.StudentNum)
                            {
                                pRole.Send(ServerString.STR_GUIDE_TOO_MANY_STUDENTS1);
                                return;
                            }

                            pRole.SetGuideRequest(pMsg.Param);
                            //pRole.SetGuideRequest(pMsg.Param);
                            //pTargetRole.SetStudentRequest(pRole.Identity);

                            pTargetRole.RequestBox = new RequestBox
                            {
                                OwnerIdentity = pRole.Identity,
                                OwnerName = pRole.Name,
                                ObjectIdentity = pTargetRole.Identity,
                                ObjectName = pTargetRole.Name,
                                Message = string.Format("{0} wants to be your apprentice. Do you accept?", pRole.Name),
                                Type = RequestBoxType.ADD_MENTOR
                            };
                            pTargetRole.RequestBox.Send(pTargetRole);
                        }
                    }
                    break;
                }

                    #endregion

                    #region Request Apprentice

                case MentorRequest.REQUEST_APPRENTICE:
                {
                    Client pClient;
                    if (ServerKernel.Players.TryGetValue(pMsg.Param, out pClient))
                    {
                        Character pTarget = pClient.Character;

                        if (pTarget.Level > pRole.Level || pRole.Level - pTarget.Level < 20)
                        {
                            pRole.Send(ServerString.STR_GUIDE_TUTOR_LOW_LEVEL1);
                            return;
                        }

                        if (pTarget.Metempsychosis > pRole.Metempsychosis)
                        {
                            pRole.Send(ServerString.STR_GUIDE_TUTOR_LOW_LEVEL1);
                            return;
                        }

                        DbMentorType maxStud =
                            ServerKernel.MentorTypes.FirstOrDefault(
                                x => pRole.Level >= x.UserMinLevel && pRole.Level <= x.UserMaxLevel);

                        if (maxStud == null)
                            return;

                        if (pRole.Apprentices.Count >= maxStud.StudentNum)
                        {
                            pRole.Send(ServerString.STR_GUIDE_TOO_MANY_STUDENTS1);
                            return;
                        }

                        if (pRole.IsApprentice(pTarget) || pTarget.IsApprentice(pRole))
                            return;

                        pRole.SetStudentRequest(pMsg.Param);
                        //pTarget.SetGuideRequest(pRole.Identity);

                        pTarget.RequestBox = new RequestBox
                        {
                            OwnerIdentity = pRole.Identity,
                            OwnerName = pRole.Name,
                            ObjectIdentity = pTarget.Identity,
                            ObjectName = pTarget.Name,
                            Message = string.Format("{0} wants to be your mentor.", pRole.Name),
                            Type = RequestBoxType.ADD_APPRENTICE
                        };
                        pTarget.RequestBox.Send(pTarget);

                        pRole.Send(string.Format(ServerString.STR_GUIDE_SENDTUTOR, pTarget.Name));
                    }
                    break;
                }

                    #endregion

                    #region Leave Mentor

                case MentorRequest.LEAVE_MENTOR:
                {
                    if (pRole.Mentor == null)
                        return;

                    var repo = new MentorRepository();
                    var dbobj = repo.FetchMentor(pRole.Identity);
                    if (dbobj == null)
                        return;

                    if (dbobj.BetrayalFlag > 0)
                    {
                        System.DateTime date = UnixTimestamp.ToDateTime((uint) dbobj.BetrayalFlag);
                        pRole.Send("You still need to wait until " + date.ToString("M") + " to leave your mentor.");
                        return;
                    }

                    if (!pRole.ReduceMoney(50000, true))
                        return;

                    dbobj.BetrayalFlag = UnixTimestamp.Timestamp() + 60*60*24*3;
                    repo.Delete(dbobj);
                    pRole.Mentor.SendExpell();
                    pRole.Mentor = null;
                    break;
                }

                    #endregion

                    #region Leave Apprentice

                case MentorRequest.EXPELL_APPRENTICE:
                {
                    if (pRole.Apprentices.Count <= 0)
                        return;

                    var repo = new MentorRepository();
                    var apprentices = repo.FetchApprentices(pRole.Identity);
                    DbMentor pAppr = null;
                    foreach (var appr in apprentices)
                    {
                        if (appr.StudentIdentity == pMsg.Identity)
                        {
                            pAppr = appr;
                            break;
                        }
                    }

                    if (pAppr == null)
                        return;

                    Student pUserAppr;
                    if (!pRole.Apprentices.TryRemove(pAppr.StudentIdentity, out pUserAppr))
                        return;

                    repo.Delete(pAppr);
                    pUserAppr.SendExpell();
                    break;
                }

                    #endregion
            }
        }
    }
}