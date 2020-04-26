// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1107 - MsgSyndicate.cs
// Last Edit: 2016/12/28 19:30
// Created: 2016/11/25 05:23

using System;
using System.Linq;
using System.Text;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Society;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleSyndicate(Character pUser, MsgSyndicate pMsg)
        {
            if (pUser == null)
                return;

            switch (pMsg.Action)
            {
                #region Info / Refresh

                case SyndicateRequest.SYN_INFO:
                case SyndicateRequest.SYN_REFRESH:
                    {
                        if (pUser.Syndicate == null
                            || pUser.SyndicateMember == null)
                            return;

                        pUser.SyndicateMember.SendSyndicate();

                        MsgDutyMinContri pNewMsg = new MsgDutyMinContri();
                        foreach (var pPos in m_pShowDuty)
                            pNewMsg.Append(pPos, pUser.Syndicate.MinimumDonation(pPos));
                        pUser.Send(pNewMsg);
                        return;
                    }

                #endregion
                #region Bulletin/Announcement

                case SyndicateRequest.SYN_BULLETIN:
                    {
                        if (pUser.Syndicate == null
                            || pUser.SyndicateMember == null)
                            return;

                        if (pUser.SyndicateMember.Position != SyndicateRank.GUILD_LEADER)
                            return;
                        string msg = Encoding.ASCII.GetString(pMsg, 26, pMsg[25]);
                        if (msg.Length > 127)
                            return;
                        pUser.Syndicate.SetAnnouncement(msg);
                        return;
                    }

                #endregion
                #region Donate Silvers

                case SyndicateRequest.SYN_DONATE_SILVERS:
                    {
                        if (pUser.Syndicate == null
                            || pUser.SyndicateMember == null)
                            return;

                        if (pMsg.Param < 10000) // should donate at least 10000 silvers
                            return;

                        if (!pUser.ReduceMoney(pMsg.Param)) // not enough money
                            return;

                        pUser.Syndicate.ChangeFunds((int) pMsg.Param);
                        pUser.SyndicateMember.IncreaseMoney(pMsg.Param);
                        pUser.SyndicateMember.SendSyndicate();
                        pUser.SyndicateMember.SendCharacterInformation();
                        pUser.Syndicate.Send(string.Format(ServerString.STR_SYN_DONATE,
                                pUser.SyndicateMember.GetRankName(), pUser.Name, pMsg.Param));

                        return;
                    }

                #endregion
                #region Donate CPs

                case SyndicateRequest.SYN_DONATE_CONQUER_POINTS:
                    {
                        if (pUser.Syndicate == null
                            || pUser.SyndicateMember == null)
                            return;

                        if (!pUser.ReduceEmoney(pMsg.Param))
                            return;

                        pUser.Syndicate.ChangeEmoneyFunds((int) pMsg.Param);
                        pUser.SyndicateMember.IncreaseEmoney(pMsg.Param);
                        pUser.SyndicateMember.SendSyndicate();
                        pUser.SyndicateMember.SendCharacterInformation();
                        pUser.Syndicate.Send(string.Format(ServerString.STR_SYN_DONATE_EMONEY,
                                pUser.SyndicateMember.GetRankName(), pUser.Name, pMsg.Param));

                        return;
                    }

                #endregion
                #region Promote

                case SyndicateRequest.SYN_PROMOTE:
                    {
                        if (pUser.Syndicate == null
                            || pUser.SyndicateMember == null)
                            return;

                        pUser.SyndicateMember.SendPromotionList();
                        return;
                    }

                #endregion
                #region Paid Promotion

                case SyndicateRequest.SYN_PAID_PROMOTE:
                    {
                        if (pUser.Syndicate == null
                            || pUser.SyndicateMember == null)
                            return;

                        pUser.Syndicate.ProcessPaidPromotion(pMsg, pUser);
                        return;
                    }

                #endregion
                #region Request Promotion

                case SyndicateRequest.SYN_SEND_REQUEST:

                    if (pUser.Syndicate == null
                        || pUser.SyndicateMember == null)
                        return;

                    if (!Enum.IsDefined(typeof(SyndicateRank), (ushort)pMsg.Param))
                        return;

                    var pos = (SyndicateRank)pMsg.Param;

                    SyndicateMember pMember = pUser.Syndicate.Members.Values.FirstOrDefault(x => x.Name == pMsg.Name);
                    if (pMember == null)
                        return;

                    pUser.Syndicate.PromoteMember(pUser, pMember, pos);
                    return;

                #endregion
                #region Join Request

                case SyndicateRequest.SYN_JOIN_REQUEST:
                    {
                        // no syn join guild param: target
                        if (pUser.Syndicate != null || pUser.SyndicateMember != null)
                            return; // already joined

                        Client pTarget;
                        if (!ServerKernel.Players.TryGetValue(pMsg.Param, out pTarget))
                            return;

                        if (pUser.FetchSynInvite(pMsg.Param))
                        {
                            // do join
                            pTarget.Character.Syndicate.AppendMember(pTarget.Character, pUser.Identity, true);
                            pUser.ClearSynInvite();
                        }
                        else
                        {
                            // add join
                            pUser.SetSynJoin(pMsg.Param);
                            pMsg.Param = pUser.Identity;
                            pMsg.RequiredLevel = pUser.Level;
                            pMsg.RequiredMetempsychosis = pUser.Metempsychosis;
                            pMsg.RequiredProfession = pUser.Profession;
                            pTarget.Send(pMsg);
                            pTarget.Character.SendRelation(pUser);
                        }

                        return;
                    }

                #endregion
                #region Invite Request

                case SyndicateRequest.SYN_INVITE_REQUEST:
                    {
                        // syn invite member param: target
                        if (pUser.Syndicate == null || pUser.SyndicateMember == null)
                            return; // no syn

                        Client pTarget;
                        if (!ServerKernel.Players.TryGetValue(pMsg.Param, out pTarget))
                            return; // target not found

                        Character pTargetUser;
                        if (pUser.Syndicate.WaitQueue.TryRemove(pTarget.Character.Identity, out pTargetUser))
                        {
                            // player waiting to join
                            return;
                        }

                        if (pTarget.Character.FetchSynJoin(pUser.Identity))
                        {
                            // player requested to join
                            if (pUser.Syndicate.Members.Count >= 800)
                                return;

                            pUser.Syndicate.AppendMember(pUser, pTarget.Character.Identity, false);
                            pTarget.Character.ClearSynJoin();
                            return;
                        }
                        // inviting player to join
                        pTarget.Character.SetSynInvite(pUser.Identity);
                        pMsg.Param = pUser.Identity;
                        pMsg.RequiredLevel = pUser.Level;
                        pMsg.RequiredMetempsychosis = pUser.Metempsychosis;
                        pMsg.RequiredProfession = pUser.Profession;
                        pTarget.Send(pMsg);
                        pTarget.Character.SendRelation(pUser);
                        return;
                    }

                #endregion
                #region Quit
                case SyndicateRequest.SYN_QUIT:
                {
                    if (pUser.Syndicate == null
                        || pUser.SyndicateMember == null)
                        return;

                    if (pUser.SyndicateRank == SyndicateRank.GUILD_LEADER)
                        return;

                    pUser.Syndicate.QuitSyndicate(pUser);
                    break;
                }
                #endregion
                #region Set Requirements
                case SyndicateRequest.SYN_SET_REQUIREMENTS:
                {
                    if (pUser.Syndicate == null
                        || pUser.SyndicateMember == null)
                        return;

                    if (pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return;

                    if (pMsg.RequiredProfession > 127)
                        return; // max ^ 6
                    if (pMsg.RequiredMetempsychosis > 2)
                        return;
                    if (pMsg.RequiredLevel > ServerKernel.MAX_UPLEVEL)
                        return;

                    pUser.Syndicate.SetRequirements((byte) pMsg.RequiredProfession,
                        (byte) pMsg.RequiredMetempsychosis,
                        (byte) pMsg.RequiredLevel);
                    break;
                }
                #endregion
                #region Discharge

                case SyndicateRequest.SYN_DISCHARGE:
                case SyndicateRequest.SYN_DISCHARGE2:
                case SyndicateRequest.SYN_DISCHARGE3: // discharge paid promotion
                {
                    if (pUser.Syndicate == null || pUser.SyndicateMember == null)
                        return;

                    Client pClient =
                        ServerKernel.Players.Values.FirstOrDefault(x => x.Character != null && x.Character.Name == pMsg.Name);

                    SyndicateMember pSynMember = null;
                    if (pClient == null)
                    {
                        DB.Entities.DbUser dbUser = Database.Characters.SearchByName(pMsg.Name);
                        if (dbUser == null)
                            return;

                        if (!pUser.Syndicate.Members.TryGetValue(dbUser.Identity, out pSynMember))
                            return;
                    }
                    else
                    {
                        pSynMember = pClient.Character.SyndicateMember;
                    }

                    pUser.Syndicate.DischargeMember(pUser, pSynMember);
                    break;
                }

                #endregion
                #region Ally
                case SyndicateRequest.SYN_ALLIED:
                {
                    if (pUser.Syndicate == null)
                        return;

                    if (pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return;

                    if (pUser.Syndicate.Allies.Count >= pUser.Syndicate.MaxAllies())
                    {
                        pUser.Send(ServerString.STR_ALLY_FULL);
                        return;
                    }

                    Syndicate pTarget = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Name == pMsg.Name);
                    if (pTarget == null || pTarget.Deleted)
                    {
                        pUser.Send(ServerString.STR_SYN_NO_EXIST);
                        return;
                    }

                    if (!pTarget.LeaderIsOnline)
                    {
                        pUser.Send(ServerString.STR_SYN_LEADER_NOT_ONLINE);
                        return;
                    }

                    Character pTargetClient = pTarget.LeaderRole;
                    
                    RequestBox pBox = new RequestBox
                    {
                        OwnerIdentity = pUser.SyndicateIdentity,
                        OwnerName = pUser.SyndicateName,
                        ObjectIdentity = pTarget.Identity,
                        ObjectName = pTarget.Name,
                        Type = RequestBoxType.SYNDICATE_ALLY,
                        Message = string.Format("{0} Guild Leader of {1} wants to be your ally. Do you accept?", pUser.Name, pUser.SyndicateName)
                    };
                    pTargetClient.RequestBox = pUser.RequestBox = pBox;
                    pBox.Send(pTargetClient);
                    break;
                }
                #endregion
                #region Remove Ally
                case SyndicateRequest.SYN_NEUTRAL1:
                {
                    if (pUser.Syndicate == null)
                        return;

                    if (pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return;

                    Syndicate pTarget = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Name == pMsg.Name);
                    if (pTarget == null || pTarget.Deleted)
                    {
                        pUser.Send(ServerString.STR_SYN_NO_EXIST);
                        return;
                    }

                    pUser.Syndicate.RemoveAlliance(pTarget.Identity);
                    break;
                }
                #endregion
                #region Enemy
                case SyndicateRequest.SYN_ENEMIED:
                {
                    if (pUser.Syndicate == null)
                        return;

                    if (pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return;

                    if (pUser.Syndicate.Allies.Count >= pUser.Syndicate.MaxEnemies())
                    {
                        pUser.Send(ServerString.STR_ENEMY_FULL);
                        return;
                    }

                    Syndicate pTarget = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Name == pMsg.Name);
                    if (pTarget == null || pTarget.Deleted)
                    {
                        pUser.Send(ServerString.STR_SYN_NO_EXIST);
                        return;
                    }

                    pUser.Syndicate.AntagonizeSyndicate(pTarget);
                    break;
                }
                #endregion
                #region Peace
                case SyndicateRequest.SYN_NEUTRAL2:
                {
                    if (pUser.Syndicate == null)
                        return;

                    if (pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
                        return;

                    Syndicate pTarget = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Name == pMsg.Name);
                    if (pTarget == null || pTarget.Deleted)
                    {
                        pUser.Send(ServerString.STR_SYN_NO_EXIST);
                        return;
                    }

                    pUser.Syndicate.RemoveEnemy(pTarget.Identity);
                    break;
                }
                #endregion
                default:
                    ServerKernel.Log.SaveLog("Missing handler for 1107:" + pMsg.Action, true);
                    return;
            }
        }
    }
}