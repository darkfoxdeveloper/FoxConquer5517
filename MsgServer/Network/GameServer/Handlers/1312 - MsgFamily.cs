// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1312 - MsgFamily.cs
// Last Edit: 2016/12/27 20:10
// Created: 2016/12/05 07:35

using System.Linq;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Society;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleFamily(Character pUser, MsgFamily pMsg)
        {
            // 22 - kick
            // Add Enemy and Add Ally send the family name
            switch (pMsg.Type)
            {
                #region Information
                case FamilyType.INFO:
                {
                    if (pUser.Family == null)
                        return;

                    pUser.Family.SendFamily(pUser);
                    break;
                }
                #endregion
                #region Members List
                case FamilyType.MEMBERS:
                {
                    if (pUser.Family == null)
                        return;
                    pUser.Family.SendMembers(pUser);
                    break;
                }
                #endregion
                #region Announcement
                case FamilyType.ANNOUNCE:
                {
                    if (pUser.Family == null)
                        return;
                    pMsg.Identity = pUser.FamilyIdentity;
                    pMsg.AddString(pUser.Family.Announcement);
                    pUser.Send(pMsg);
                    break;
                }
                #endregion
                #region Set Announcement
                case FamilyType.SET_ANNOUNCEMENT:
                {
                    if (pUser.Family == null)
                        return;

                    if (pUser.FamilyPosition != FamilyRank.CLAN_LEADER)
                        return;

                    string szAnnounce = pMsg.Announcement;
                    if (szAnnounce.Length > 127)
                        szAnnounce = szAnnounce.Substring(0, 127);

                    pUser.Family.Announcement = szAnnounce;
                    pUser.Send(pMsg);
                    break;
                }
                #endregion
                #region My Clan
                case FamilyType.MY_CLAN:
                {
                    if (pUser.Family == null) return;

                    pUser.Family.SendFamily(pUser);
                    pUser.Family.SendOccupation(pUser);
                    break;
                }
                #endregion
                #region Dedicate
                case FamilyType.DEDICATE:
                {
                    if (pUser.Family == null)
                        return;

                    if (!pUser.ReduceMoney(pMsg.Identity, true))
                        return;

                    pUser.Family.MoneyFunds += pMsg.Identity;
                    pUser.FamilyMember.Donation += pMsg.Identity;
                    pUser.Family.SendFamily(pUser);
                    break;
                }
                #endregion
                #region Invite
                case FamilyType.RECRUIT:
                {
                    if (pUser.Family == null)
                        return;

                    if (pUser.Family.IsFull)
                        return;

                    Client pTarget;
                    if (!ServerKernel.Players.TryGetValue(pMsg.Identity, out pTarget))
                        return;

                    if (pTarget.Character.Family != null)
                        return;

                    pMsg.Identity = pUser.Family.Identity;
                    pMsg.AddString(pUser.FamilyName);
                    pMsg.AddString(pUser.Name);
                    pTarget.Send(pMsg);

                    pUser.SetFamilyRecruitRequest(pTarget.Character.Identity);
                    break;
                }
                #endregion
                #region Accept Invite
                case FamilyType.ACCEPT_RECRUIT:
                {
                    if (pUser.Family != null) return;

                    Family pFamily;
                    if (!ServerKernel.Families.TryGetValue(pMsg.Identity, out pFamily))
                        return;

                    if (pFamily.IsFull)
                        return;

                    Client pLeaderS;
                    if (!ServerKernel.Players.TryGetValue(pFamily.LeaderIdentity, out pLeaderS))
                        return;

                    Character pLeader = pLeaderS.Character;
                    if (pLeader.FamilyPosition != FamilyRank.CLAN_LEADER)
                        return;

                    if (!pLeader.FetchFamilyRecruitRequest(pUser.Identity))
                        return;

                    pFamily.AppendMember(pLeader, pUser);
                    pLeader.ClearFamilyRecruitRequest();
                    break;
                }
                #endregion
                #region Join
                case FamilyType.JOIN:
                {
                    if (pUser.Family != null)
                        return;

                    Client pTarget;
                    if (!ServerKernel.Players.TryGetValue(pMsg.Identity, out pTarget))
                        return;

                    if (pTarget.Character.Family == null || pTarget.Character.Family.IsFull)
                        return;

                    pMsg.Identity = pUser.Identity;
                    pMsg.AddString(pUser.Name);
                    pTarget.Send(pMsg);

                    pUser.SetFamilyJoinRequest(pTarget.Character.Identity);
                    break;
                }
                #endregion
                #region Accept Join
                case FamilyType.ACCEPT_JOIN_REQUEST:
                {
                    if (pUser.Family == null || pUser.FamilyPosition != FamilyRank.CLAN_LEADER)
                        return;

                    Client pClienTarget;
                    if (!ServerKernel.Players.TryGetValue(pMsg.Identity, out pClienTarget))
                        return;

                    Character pInvited = pClienTarget.Character;
                    if (pInvited.Family != null)
                        return;

                    if (!pInvited.FetchFamilyJoinRequest(pUser.Identity))
                        return;

                    pUser.Family.AppendMember(pUser, pInvited);
                    pInvited.ClearFamilyJoinRequest();
                    break;
                }
                #endregion
                #region Quit
                case FamilyType.QUIT:
                {
                    if (pUser.Family == null)
                        return;
                    if (pUser.FamilyPosition == FamilyRank.CLAN_LEADER)
                        return;
                    if (pUser.FamilyPosition == FamilyRank.SPOUSE)
                        return;
                    pUser.Family.KickoutMember(pUser.FamilyMember);
                    break;
                }
                #endregion
                #region Abdicate
                case FamilyType.TRANSFER_LEADER:
                {
                    break;
                }
                #endregion
                #region Kickout
                case (FamilyType) 22:
                {
                    if (pUser.Family == null || pUser.FamilyPosition != FamilyRank.CLAN_LEADER)
                        return;
                    FamilyMember pTarget = null;
                    foreach (var client in pUser.Family.Members.Values)
                    {
                        if (client.Name == pMsg.Name)
                        {
                            pTarget = client;
                            break;
                        }
                    }
                    if (pTarget == null || pTarget.Position == FamilyRank.CLAN_LEADER || pTarget.Position == FamilyRank.SPOUSE)
                        return;
                    pUser.Family.KickoutMember(pTarget);
                    pUser.Family.SendFamily(pUser);
                    pUser.Family.SendMembers(pUser);
                    break;
                }
                #endregion
                #region Ally
                case FamilyType.ADD_ALLY:
                {
                    if (pUser.Family == null)
                        return;
                    if (pUser.FamilyPosition != FamilyRank.CLAN_LEADER)
                        return;
                    if (pUser.Family.Allies.Count >= 5)
                        return;
                    
                    Client pTargetLeader;
                    if (!ServerKernel.Players.TryGetValue(pMsg.Identity, out pTargetLeader))
                        return;

                    if (pTargetLeader.Character.FamilyPosition != FamilyRank.CLAN_LEADER)
                        return;

                    Family pTarget = pTargetLeader.Character.Family;
                    if (pTarget == null)
                        return;
                    
                    RequestBox pAlly = new RequestBox
                    {
                        OwnerIdentity = pUser.FamilyIdentity,
                        OwnerName = pUser.FamilyName,
                        ObjectIdentity = pTarget.Identity,
                        ObjectName = pTarget.Name,
                        Type = RequestBoxType.FAMILY_ALLY,
                        Message = string.Format("{0} Leader of the Clan {1} wants to be your ally. Do you accept?", pUser.Name, pUser.FamilyName)
                    };
                    pUser.RequestBox = pTargetLeader.Character.RequestBox = pAlly;
                    pAlly.Send(pTargetLeader.Character);
                    break;
                }
                #endregion
                #region Delete Ally
                case FamilyType.DELETE_ALLY:
                {
                    break;
                }
                #endregion
                #region Enemy
                case FamilyType.ADD_ENEMY:
                {
                    if (pUser.Family == null)
                        return;
                    if (pUser.FamilyPosition != FamilyRank.CLAN_LEADER)
                        return;
                    if (pUser.Family.Enemies.Count >= 5)
                        return;
                    Family pTarget = ServerKernel.Families.Values.FirstOrDefault(x => x.Name == pMsg.Name);
                    if (pTarget == null)
                        return;
                    pUser.Family.EnemyFamily(pTarget);
                    break;
                }
                #endregion
                #region Delete Enemy
                case FamilyType.DELETE_ENEMY:
                {
                    break;
                }
                #endregion
                default:
                {
                    ServerKernel.Log.SaveLog(string.Format("MsgFamily:Type{0} not handled", pMsg.Type), true, LogType.WARNING);
                    GamePacketHandler.Report(pMsg);
                    break;
                }
            }
        }
    }
}