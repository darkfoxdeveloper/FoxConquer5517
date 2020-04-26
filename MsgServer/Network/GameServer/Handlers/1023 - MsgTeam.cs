// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1023 - MsgTeam.cs
// Last Edit: 2016/12/07 10:06
// Created: 2016/12/07 10:05

using Core.Common.Enums;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleTeamAction(Character pRole, MsgTeam pMsg)
        {
            switch (pMsg.Type)
            {
                #region Create
                case TeamActionType.CREATE:
                    {
                        if (pRole.Team != null)
                        {
                            pRole.Send(ServerString.STR_TEAMMATE_CANNOT_CREATE);
                            return; // already have a team
                        }

                        if (!pRole.AttachStatus(pRole, FlagInt.TEAM_LEADER, 0, int.MaxValue, int.MaxValue, 0, pRole.Identity))
                        {
                            pRole.Send(ServerString.STR_CREATE_TEAM_FAILED);
                            return;
                        }

                        pRole.Team = new Team(pRole);
                        pRole.Send(pMsg);
                        pMsg.Type = TeamActionType.LEADER;
                        pRole.Send(pMsg);

                        break;
                    }
                #endregion
                #region Dismiss
                case TeamActionType.DISMISS:
                    {
                        if (pRole.Team == null)
                        {
                            pRole.Send(ServerString.STR_NO_TEAM_TO_DISMISS);
                            return;
                        }
                        pRole.Team.Destroy(pRole, pMsg);
                        pRole.Team = null;
                        break;
                    }
                #endregion
                #region Leave Team
                case TeamActionType.LEAVE_TEAM:
                    {
                        if (pRole.Team == null)
                        {
                            pRole.Send(ServerString.STR_NO_TEAM_TO_LEAVE);
                            return;
                        }
                        pRole.Team.LeaveTeam(pRole, pMsg);

                        break;
                    }
                #endregion
                #region Kick Member
                case TeamActionType.KICK:
                    {
                        if (pRole.Team == null)
                        {
                            pRole.Send(ServerString.STR_NO_TEAM_TO_LEAVE);
                            return;
                        }

                        pRole.Team.KickMember(pRole, pMsg);
                        break;
                    }
                #endregion
                #region Request Join (Applicant want to join)
                case TeamActionType.REQUEST_JOIN:
                    {
                        if (pRole.Team != null)
                        {
                            pRole.Send(ServerString.STR_HAVE_JOIN_TEAM);
                            return; // already has a team
                        }

                        Character pUserTarget;
                        if (!pRole.Map.Players.TryGetValue(pMsg.Target, out pUserTarget))
                        {
                            pRole.Send(ServerString.STR_NO_CAPTAIN_CLOSE);
                            return;
                        }

                        if (pUserTarget.Team == null)
                        {
                            pRole.Send(ServerString.STR_NOT_CREATE_TEAM);
                            return;
                        }

                        pUserTarget.Team.RequestMember(pRole, pUserTarget, pMsg);
                        break;
                    }
                #endregion
                #region Request Invite (Applicant is inviting)
                case TeamActionType.REQUEST_INVITE:
                    {
                        if (pRole.Team == null)
                        {
                            pRole.Send(ServerString.STR_NO_TEAM_TO_INVITE);
                            return; // dont have a team, cant invite
                        }

                        Character pUserTarget;
                        if (!pRole.Map.Players.TryGetValue(pMsg.Target, out pUserTarget))
                        {
                            pRole.Send(ServerString.STR_APPLICANT_NOT_FOUND);
                            return;
                        }

                        if (pUserTarget.Team != null)
                        {
                            pRole.Send(ServerString.STR_HAS_IN_TEAM);
                            return;
                        }

                        pRole.Team.InviteMember(pUserTarget, pRole, pMsg);
                        break;
                    }
                #endregion
                #region Accept Team (Me accepting invitation)
                case TeamActionType.ACCEPT_INVITE:
                    {
                        if (pRole.Team != null)
                        {
                            pRole.Send(ServerString.STR_HAVE_JOIN_TEAM);
                            return; // already has a team
                        }

                        Character pLeader;
                        if (!pRole.Map.Players.TryGetValue(pMsg.Target, out pLeader))
                        {
                            pRole.Send(ServerString.STR_NO_CAPTAIN_CLOSE);
                            return;
                        }

                        pLeader.Team.MemberAccept(pLeader, pRole, pMsg);
                        break;
                    }
                #endregion
                #region Accept Join (Leader accepting join request)
                case TeamActionType.ACCEPT_JOIN:
                    {
                        if (pRole.Team == null)
                        {
                            pRole.Send(ServerString.STR_NO_TEAM_TO_INVITE);
                            return; // dont have a team, cant accept
                        }

                        Character pUserTarget;
                        if (!pRole.Map.Players.TryGetValue(pMsg.Target, out pUserTarget))
                        {
                            pRole.Send(ServerString.STR_APPLICANT_NOT_FOUND);
                            return;
                        }

                        if (pUserTarget.Team != null)
                        {
                            pRole.Send(ServerString.STR_HAS_IN_TEAM);
                            return;
                        }

                        pRole.Team.AcceptMember(pRole, pUserTarget, pMsg);
                        break;
                    }
                #endregion
                #region Forbid Join
                case TeamActionType.JOIN_DISABLE:
                    {
                        if (pRole.Team == null || pRole.Team.Leader != pRole)
                            return;
                        pRole.Team.SetForbid(true);
                        pRole.Send(pMsg);
                        break;
                    }
                #endregion
                #region Enable Join
                case TeamActionType.JOIN_ENABLE:
                    {
                        if (pRole.Team == null || pRole.Team.Leader != pRole)
                            return;
                        pRole.Team.SetForbid(false);
                        pRole.Send(pMsg);
                        break;
                    }
                #endregion
                #region Open Item
                case TeamActionType.ITEM_ENABLE:
                    {
                        if (pRole.Team == null || pRole.Team.Leader != pRole)
                            return;
                        pRole.Team.SetCloseItem(true);
                        pRole.Send(pMsg);
                        break;
                    }
                #endregion
                #region Close Item
                case TeamActionType.ITEM_DISABLE:
                    {
                        if (pRole.Team == null || pRole.Team.Leader != pRole)
                            return;
                        pRole.Team.SetCloseItem(false);
                        pRole.Send(pMsg);
                        break;
                    }
                #endregion
                #region Open Money
                case TeamActionType.MONEY_ENABLE:
                    {
                        if (pRole.Team == null || pRole.Team.Leader != pRole)
                            return;
                        pRole.Team.SetCloseMoney(true);
                        pRole.Send(pMsg);
                        break;
                    }
                #endregion
                #region Close Money
                case TeamActionType.MONEY_DISABLE:
                    {
                        if (pRole.Team == null || pRole.Team.Leader != pRole)
                            return;
                        pRole.Team.SetCloseMoney(false);
                        pRole.Send(pMsg);
                        break;
                    }
                #endregion
                #region Cancel Join or Invite Request
                case (TeamActionType)14:
                    {
                        Client pTarget;
                        if (ServerKernel.Players.TryGetValue(pMsg.Target, out pTarget))
                        {
                            if (pTarget.Character != null && pTarget.Character.Team != null)
                            {
                                pTarget.Character.SetTeamInvite(0);
                                pTarget.Character.SetTeamJoin(0);
                            }
                        }
                        break;
                    }
                #endregion
            }
        }
    }
}