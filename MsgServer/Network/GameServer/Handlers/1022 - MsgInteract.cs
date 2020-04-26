// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1022 - MsgInteract.cs
// Last Edit: 2016/12/07 01:37
// Created: 2016/12/07 01:37

using System;
using System.Linq;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleInteract(Character pRole, MsgInteract pMsg)
        {
            if (pRole == null || pRole.BattleSystem == null) return;

            pRole.BattleSystem.DestroyAutoAttack();
            pRole.BattleSystem.ResetBattle();
            if (pRole.BattleSystem.QueryMagic() != null)
            {
                pRole.BattleSystem.QueryMagic().SetMagicState(0);
                pRole.BattleSystem.QueryMagic().BreakAutoAttack();
                pRole.BattleSystem.QueryMagic().AbortMagic(true);
            }

            if (!pRole.IsAlive)
                return;

            var obj = pRole.BattleSystem.FindRole(pMsg.TargetIdentity);
            if (obj == null
                && pMsg.Action != InteractionType.ACT_ITR_MAGIC_ATTACK
                && pMsg.Action != InteractionType.ACT_ITR_COUNTER_KILL_SWITCH
                && pMsg.Action != InteractionType.ACT_ITR_PRESENT_EMONEY)
                return;

            switch (pMsg.Action)
            {
                    #region 2/28 - Meele and Bow

                case InteractionType.ACT_ITR_SHOOT:
                case InteractionType.ACT_ITR_ATTACK:
                {
                    pRole.BattleSystem.CreateBattle(pMsg.TargetIdentity);
                    pRole.BattleSystem.SetAutoAttack();
                    pRole.SetAttackTarget(obj);
                    break;
                }

                    #endregion
                    #region 8 - Court

                case InteractionType.ACT_ITR_COURT:
                {
                    if (pRole.Identity == pMsg.TargetIdentity)
                        return;

                    Character pTarget;
                    if (pRole.Map.Players.TryGetValue(pMsg.TargetIdentity, out pTarget))
                    {
                        if (pTarget.Mate != ServerString.NOMATE_NAME)
                        {
                            pRole.Send(ServerString.TARGET_ALREADY_MARRIED);
                            return;
                        }
                        if (pRole.Mate != ServerString.NOMATE_NAME)
                        {
                            pRole.Send(ServerString.YOURE_ALREADY_MARRIED);
                            return;
                        }
                        if (pTarget.Gender == pRole.Gender)
                        {
                            pRole.Send(ServerString.NOT_ALLOWED_SAME_GENDER_MARRIAGE);
                            return;
                        }

                        pTarget.SetMarryRequest(pRole.Identity);
                        pTarget.Send(pMsg);
                    }
                    else
                    {
                        pRole.Send(ServerString.MARRIAGE_NOT_APPLY);
                        return;
                    }
                    break;
                }

                    #endregion
                    #region 9 - Marry

                case InteractionType.ACT_ITR_MARRY:
                {
                    if (pRole.Identity == pMsg.TargetIdentity)
                        return;

                    Character pTarget;
                    if (pRole.Map.Players.TryGetValue(pMsg.TargetIdentity, out pTarget))
                    {
                        if (pTarget.Mate != ServerString.NOMATE_NAME)
                        {
                            pRole.Send(ServerString.TARGET_ALREADY_MARRIED);
                            return;
                        }
                        if (pRole.Mate != ServerString.NOMATE_NAME)
                        {
                            pRole.Send(ServerString.YOURE_ALREADY_MARRIED);
                            return;
                        }
                        if (pTarget.Gender == pRole.Gender)
                        {
                            pRole.Send(ServerString.NOT_ALLOWED_SAME_GENDER_MARRIAGE);
                            return;
                        }
                        if (!pRole.FetchMarryRequest(pTarget.Identity))
                        {
                            pRole.Send(ServerString.MARRIAGE_NOT_APPLY);
                            return;
                        }

                        pRole.Mate = pTarget.Name;
                        pTarget.Mate = pRole.Name;
                        MsgName msg = new MsgName
                        {
                            Identity = pRole.Identity,
                            Action = StringAction.MATE
                        };
                        msg.Append(pRole.Mate);
                        pRole.Send(msg);
                        msg = new MsgName
                        {
                            Identity = pTarget.Identity,
                            Action = StringAction.MATE
                        };
                        msg.Append(pTarget.Mate);
                        pTarget.Send(msg);

                        if (pRole.Family != null && pTarget.Family == null)
                        {
                            pRole.Family.AppendMember(pRole, pTarget);
                        } 
                        else if (pRole.Family == null && pTarget.Family != null)
                        {
                            pTarget.Family.AppendMember(pTarget, pRole);
                        }

                        ServerKernel.SendMessageToAll(string.Format(ServerString.STR_MARRY, pRole.Name, pTarget.Name),
                            ChatTone.CENTER);
                    }
                    else
                    {
                        pRole.Send(ServerString.MARRIAGE_NOT_APPLY);
                        return;
                    }
                    break;
                }

                    #endregion
                    #region 24 - Magic Attack

                case InteractionType.ACT_ITR_MAGIC_ATTACK:
                {
                    #region TemporaryDecryption

                    ushort skillId = Convert.ToUInt16(((long) pMsg[24] & 0xFF) | (((long) pMsg[25] & 0xFF) << 8));
                    skillId ^= 0x915d;
                    skillId ^= (ushort) pRole.Identity;
                    skillId = (ushort) (skillId << 0x3 | skillId >> 0xd);
                    skillId -= 0xeb42;

                    uint Target = ((uint) pMsg[12] & 0xFF) | (((uint) pMsg[13] & 0xFF) << 8) |
                                  (((uint) pMsg[14] & 0xFF) << 16) | (((uint) pMsg[15] & 0xFF) << 24);
                    Target = ((((Target & 0xffffe000) >> 13) | ((Target & 0x1fff) << 19)) ^ 0x5F2D2463 ^ pRole.Identity) -
                             0x746F4AE6;

                    pMsg.TargetIdentity = Target;

                    ushort TargetX = 0;
                    ushort TargetY = 0;
                    long xx = (pMsg[16] & 0xFF) | ((pMsg[17] & 0xFF) << 8);
                    long yy = (pMsg[18] & 0xFF) | ((pMsg[19] & 0xFF) << 8);
                    xx = xx ^ (pRole.Identity & 0xffff) ^ 0x2ed6;
                    xx = ((xx << 1) | ((xx & 0x8000) >> 15)) & 0xffff;
                    xx |= 0xffff0000;
                    xx -= 0xffff22ee;
                    yy = yy ^ (pRole.Identity & 0xffff) ^ 0xb99b;
                    yy = ((yy << 5) | ((yy & 0xF800) >> 11)) & 0xffff;
                    yy |= 0xffff0000;
                    yy -= 0xffff8922;
                    TargetX = Convert.ToUInt16(xx);
                    TargetY = Convert.ToUInt16(yy);
                    pMsg.MagicType = skillId;
                    pMsg.CellX = TargetX;
                    pMsg.CellY = TargetY;

                    #endregion

                    if (pRole.IsAlive)
                        pRole.ProcessMagicAttack(pMsg.MagicType, pMsg.TargetIdentity, pMsg.CellX, pMsg.CellY);
                    break;
                }

                    #endregion
                    #region 39 - Claim CPs

                case InteractionType.ACT_ITR_PRESENT_EMONEY:
                    if (pRole.CoinMoney > 0)
                    {
                        pRole.AwardEmoney(pRole.CoinMoney, true);
                        pMsg.Amount = pRole.CoinMoney;
                        pRole.CoinMoney = 0;
                        pRole.Send(pMsg);
                    }
                    break;

                    #endregion
                    #region 44 - Counter Kill Switch

                case InteractionType.ACT_ITR_COUNTER_KILL_SWITCH:
                {
                    if (pRole.Profession/10 != 5 || !pRole.IsPureClass())
                    {
                        pRole.Send(ServerString.STR_SCAPEGOAT_ONLY_PURE_NINJA);
                        return;
                    }

                    if (!pRole.IsAlive || pRole.IsWing())
                        return;

                    if (pRole.Scapegoat) // disable
                    {
                        pMsg.TargetIdentity = pMsg.EntityIdentity = pRole.Identity;
                        pMsg.Damage = 0;
                        pMsg.CellX = pRole.MapX;
                        pMsg.CellY = pRole.MapY;
                        pRole.Send(pMsg);
                        pRole.Scapegoat = false;
                    }
                    else
                    {
                        pMsg.TargetIdentity = pMsg.EntityIdentity = pRole.Identity;
                        pMsg.Damage = 1;
                        pMsg.CellX = pRole.MapX;
                        pMsg.CellY = pRole.MapY;
                        pRole.Send(pMsg);
                        pRole.Scapegoat = true;
                    }
                    break;
                }

                #endregion
                    #region 36 - QuestJar
                    case InteractionType.ACT_ITR_INCREASE_JAR:
                            {
                                // TODO Only temporal for now. But allowed login/logout the player.
                                QuestJar quest = ServerKernel.PlayerQuests.Where(x => x.Player.Identity == pRole.Identity).FirstOrDefault();
                                if (quest != null)
                                {
                                    pMsg.Amount = quest.Kills;
                                }
                                pRole.Send(pMsg);
                                break;
                            }
                    #endregion

                default:
                    ServerKernel.Log.SaveLog("Missing Interaction Type: " + pMsg.Action, true, "itr_msg",
                        LogType.WARNING);
                    break;
            }
        }
    }
}