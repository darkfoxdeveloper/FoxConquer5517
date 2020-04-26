// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Magic Data.cs
// Last Edit: 2017/01/03 17:49
// Created: 2016/12/29 21:30

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Enums;
using DB.Entities;
using DB.Repositories;
using MsgServer.Network.GameServer.Handlers;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Events;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public class MagicData
    {
        private const int _MAX_TARGET_NUM = 25;

        private readonly int[] _Prof2MaskTable =
        {
            10, 11, 12, 13, 14, 15,
            20, 21, 22, 23, 24, 25,
            40, 41, 42, 43, 44, 45,
            50, 51, 52, 53, 54, 55,
            100, 101,
            132, 133, 134, 135,
            142, 143, 144, 145
        };

        private readonly uint MAX_USER_PROFS = 36;

        public ConcurrentDictionary<uint, Magic> Magics;
        private IRole m_pOwner;

        public MagicData(IRole pOwner)
        {
            m_pOwner = pOwner;
            Magics = new ConcurrentDictionary<uint, Magic>();

            if (pOwner.Identity < 1000000 && pOwner is Monster)
            {
                var mgc = ServerKernel.MonsterMagics.Where(x => x.OwnerIdentity == (pOwner as Monster).Type);
                if (mgc != null)
                {
                    foreach (var magic in mgc)
                    {
                        var newMgc = new Magic(m_pOwner);
                        if (!newMgc.Create(new DbMagic
                        {
                            Level = magic.MagicLevel,
                            Type = magic.MagicIdentity,
                            OwnerId = magic.OwnerIdentity
                        }))
                        {
                            ServerKernel.Log.SaveLog(
                                string.Format("Could not load magic (id:{0}) for (targetId:{1})", magic.Identity,
                                    magic.OwnerIdentity), false, "monstermagic_error");
                            continue;
                        }
                        Magics.TryAdd(newMgc.Type, newMgc);
                    }
                }
            }
            else if (pOwner.Identity >= 1000000)
            {
                var allMagics = new MagicRepository().FetchByUser(m_pOwner.Identity);
                if (allMagics != null)
                {
                    foreach (var mgc in allMagics)
                    {
                        var newMgc = new Magic(m_pOwner);
                        if (!newMgc.Create(mgc))
                        {
                            ServerKernel.Log.SaveLog(
                                string.Format("Could not load magic (id:{0}) for (targetId:{1})", mgc.Id,
                                    m_pOwner.Identity), false, "magic_error");
                            continue;
                        }
                        newMgc.SendSkill();
                        Magics.TryAdd(newMgc.Type, newMgc);
                    }
                }
            }

            m_tDelay = new TimeOutMS(100);
            m_tDelay.Update();
            m_tApply = new TimeOutMS(0);
            m_tIntone = new TimeOutMS(0);
        }

        public bool Create(ushort type, byte level)
        {
            Magic pMagic = new Magic(m_pOwner);
            if (pMagic.Create(type, level))
            {
                return Magics.TryAdd(type, pMagic);
            }
            return false;
        }

        #region Battle System

        private int m_nMagicState;
        private bool m_bAutoAttack;
        private TimeOutMS m_tApply;
        private TimeOutMS m_tIntone;
        private TimeOutMS m_tDelay;
        private Magic m_pMagic;
        private bool m_bTargetLocked;
        private uint m_idTarget;
        private int m_nData;
        private Point m_pPos;
        private int m_nApplyTimes;
        private int m_nDelay;
        private int m_nRawDelay;
        private List<IRole> m_pSetTargetLocked = new List<IRole>();

        // click detection
        private int m_nDetectedClicks = 0;
        private TimeOutMS m_tClickTimeOut = new TimeOutMS(1000);

        public bool MagicAttack(ushort usMagicType, uint idTarget, ushort x, ushort y, byte ucAutoActive = 0)
        {
            if (m_pOwner is Monster)
                m_nMagicState = 0;
            else if (m_pOwner is Character)
            {
                if (!m_tClickTimeOut.IsActive())
                    m_tClickTimeOut.Startup(1000);

                if (m_tClickTimeOut.ToNextTime(1000))
                {
                    m_nDetectedClicks = 0;
                    ServerKernel.Log.GmLog("click_detection_a",
                            string.Format("{0} has been caught clicking {1} times in {2} ms.",
                            m_pOwner.Name, m_nDetectedClicks, 1000), true);
                }
                else
                {
                    m_nDetectedClicks++;
                    if (m_nDetectedClicks > 5)
                    {
                        ServerKernel.Log.GmLog("click_detection",
                            string.Format("{0} has been caught clicking {1} times in {2} ms.",
                            m_pOwner.Name, m_nDetectedClicks, 1000-m_tClickTimeOut.GetRemain()), true);
                        //m_pOwner.Send(
                        //    new MsgTalk("Be careful! System has detected so many clicks in a small portion of time.",
                        //        ChatTone.SYSTEM));
                    }
                    if (m_nDetectedClicks > 30)
                    {
                        ServerKernel.Log.GmLog("click_detection",
                            string.Format("{0} has been disconnected by clicking 30 times in less than 1000 ms.",
                            m_pOwner.Name), true);
                        ServerKernel.SendMessageToAll(string.Format("{0} has been disconnected, suspect of using autoclick macro.", m_pOwner.Name), ChatTone.TALK);
                        m_pOwner.Send(new MsgTalk("You have been disconnected by being an autoclick suspect.",
                            ChatTone.TALK, Color.BlueViolet));
                        (m_pOwner as Character).Disconnect("AUTOCLICKER");
                    }
                }
            }

             switch (m_nMagicState)
            {
                case MagicState.MAGICSTATE_INTONE:
                    {
                        AbortMagic(true);
                        break;
                    }
                case MagicState.MAGICSTATE_DELAY:
                    {
                        return false;
                    }
                case MagicState.MAGICSTATE_LAUNCH:
                    {
                        return false;
                    }
            }

            m_bTargetLocked = false;

            if (!(Magics.TryGetValue(usMagicType, out m_pMagic) &&
                  (ucAutoActive == 0 || (m_pMagic.AutoActive & ucAutoActive) != 0)))
            {
                ServerKernel.Log.GmLog("cheat",
                    string.Format("invalid magic type: {0}, user[{1}][{2}]", usMagicType, m_pOwner.Name,
                        m_pOwner.Identity));
                AbortMagic(true);
                return false;
            }

            if (!CheckCondition(m_pMagic, idTarget, ref x, ref y))
            {
                if (m_pMagic.Sort == MagicSort.MAGICSORT_COLLIDE)
                {
                    ProcessCollideFail(x, y, (int)idTarget);		// idTarget: dir
                }
                else if (m_pMagic.Sort == MagicSort.MAGICSORT_JUMPATTACK)
                {
                    // handle jump attack
                    // no jump atk in cq
                }
                if (!m_pOwner.Map.IsTrainingMap())
                    AbortMagic(true);
                return false;
            }

            if (!m_pMagic.Delay())
                return false;

            m_idTarget = idTarget;
            if (m_pMagic.Ground > 0 && m_pMagic.Sort != MagicSort.MAGICSORT_ATKSTATUS)
                m_idTarget = 0;

            m_bAutoAttack = true;
            if (MagicSort.MAGICSORT_COLLIDE == m_pMagic.Sort)
            {
                m_nData = (int)idTarget;
            }

            m_pPos = new Point(x, y);
            //m_nMagicState = MagicState.MAGICSTATE_INTONE;
            //m_tIntone.Startup((int)m_pMagic.Intone);

            // IRole pRole = m_pOwner.BattleSystem.FindRole(m_idTarget);
            if ((!m_pOwner.Map.IsTrainingMap() && m_pOwner.MapIdentity != 1005) && m_pOwner is Character)
            {
                if (m_pMagic.UseMana > 0)
                    m_pOwner.AddAttribute(ClientUpdateType.MANA, -1 * m_pMagic.UseMana, true);
                if (m_pMagic.UseStamina > 0)
                    m_pOwner.AddAttribute(ClientUpdateType.STAMINA, -1 * m_pMagic.UseStamina, true);
                if (m_pMagic.UseItem > 0 && m_pMagic.UseItemNum > 0)
                    m_pOwner.SpendEquipItem(m_pMagic.UseItem, m_pMagic.UseItemNum, true);
            }

            if (m_pMagic.UseXp == 1 && m_pOwner is Character)
            {
                IStatus pStatus = m_pOwner.QueryStatus(FlagInt.START_XP);
                if (pStatus == null && (m_pOwner.QueryStatus(FlagInt.VORTEX) == null))
                    return false;
                m_pOwner.DetachStatus(FlagInt.START_XP);
                (m_pOwner as Character).ClsXpVal();
            }

            if (!IsWeaponMagic(m_pMagic.Type))
            {
                MsgInteract pMsg = new MsgInteract
                {
                    EntityIdentity = m_pOwner.Identity,
                    TargetIdentity = idTarget,
                    CellX = x,
                    CellY = y,
                    MagicType = m_pMagic.Type,
                    MagicLevel = m_pMagic.Level
                };
                m_pOwner.Map.SendToRange(pMsg, m_pOwner.MapX, m_pOwner.MapY);
            }

            if (m_pMagic.UseMana != 0)
            {
                if (!m_pOwner.Map.IsTrainingMap())
                    m_pOwner.DecEquipmentDurability(false, HitByMagic(), (ushort)m_pMagic.UseItemNum);

                if (Calculations.ChanceCalc(7))
                    m_pOwner.GetMAtkGemEffect();
            }

            if (m_pOwner.IsPlayer())
            {
                (m_pOwner as Character).ProcessOnAttack();
            }

            if (m_pMagic.Intone <= 0) // launch immediatly
            {
                m_tIntone.Clear();
                //m_nMagicState = MagicState.MAGICSTATE_LAUNCH;
                m_nApplyTimes = (int)m_pMagic.ActiveTimes;

                try
                {
                    if (!Launch())
                    {
                        LockTarget(false);
                        m_tApply.Clear();
                        ResetDelay();
                    }
                    else
                    {
                        if (m_pOwner.Map.IsTrainingMap())
                        {
                            m_tDelay = new TimeOutMS((int)m_pMagic.Timeout);
                            m_tDelay.Update();
                            m_nMagicState = MagicState.MAGICSTATE_DELAY;
                            return true;
                        }

                        if (m_tApply == null)
                            m_tApply = new TimeOutMS(0);
                        if (m_pMagic == null)
                            return false;
                        m_tApply.Startup(m_pMagic.GetApplyMs());
                        m_nMagicState = MagicState.MAGICSTATE_LAUNCH;
                    }
                }
                catch (Exception ex)
                {
                    ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
                }
                return true;
            }
            else
            {
                m_nMagicState = MagicState.MAGICSTATE_INTONE;
                m_tIntone.Startup((int)m_pMagic.Intone);
            }
            return true;
        }

        private bool LockTarget(bool bLock)
        {
            if (m_bTargetLocked == bLock)
                return true;

            int nLockSecs = m_pMagic.GetLockSecs();

            if (bLock)
            {
                if (m_pOwner.IsAlive)
                {
                    // m_pOwner.AttachStatus(m_pOwner, 251, 0, nLockSecs, 0);
                }
            }
            else
            {
                // m_pOwner.DetachStatus(251);
                // todo find lock status
            }
            m_bTargetLocked = bLock;
            return true;
        }

        private void ResetDelay()
        {
            if (m_pMagic == null) return;
            m_nDelay = m_nRawDelay;
            m_nMagicState = MagicState.MAGICSTATE_DELAY;
            m_tDelay.Update();

            m_pMagic.StartDelay();
        }

        private bool Launch()
        {
            bool ret = false;
            try
            {
                if (m_pMagic == null)
                    return false;

                if (!m_pOwner.IsAlive)
                {
                    ShowMiss();
                    return false;
                }

                switch (m_pMagic.Sort)
                {
                    case MagicSort.MAGICSORT_ATTACK:
                        ret = ProcessAttack();
                        break;
                    case MagicSort.MAGICSORT_RECRUIT:
                        ret = ProcessRecruit();
                        break;
                    case MagicSort.MAGICSORT_FAN:
                        ret = ProcessFan();
                        break;
                    case MagicSort.MAGICSORT_BOMB:
                        ret = ProcessBomb();
                        break;
                    case MagicSort.MAGICSORT_ATTACHSTATUS:
                        ret = ProcessAttach();
                        break;
                    case MagicSort.MAGICSORT_DETACHSTATUS:
                        ret = ProcessDetach();
                        break;
                    case MagicSort.MAGICSORT_DISPATCHXP:
                        ret = ProcessDispatchXp();
                        break;
                    case MagicSort.MAGICSORT_COLLIDE:
                        ret = ProcessCollide();
                        break;
                    case MagicSort.MAGICSORT_LINE:
                        ret = ProcessLine();
                        break;
                    case MagicSort.MAGICSORT_ATKSTATUS:
                        ret = ProcessAtkStatus();
                        break;
                    case MagicSort.MAGICSORT_ADDMANA:
                        ret = ProcessAddMana();
                        break;
                    case MagicSort.MAGICSORT_CALLPET:
                        ret = ProcessCallPet();
                        break;
                    case MagicSort.MAGICSORT_DECLIFE:
                        ret = ProcessDecLife();
                        break;
                    case MagicSort.MAGICSORT_GROUNDSTING:
                        ret = ProcessGroundSting();
                        break;
                    case MagicSort.MAGICSORT_VORTEX:
                        ret = ProcessVortex();
                        break;
                    case MagicSort.MAGICSORT_ACTIVATESWITCH:
                        ret = ProcessActivateSwitch();
                        break;
                    case MagicSort.MAGICSORT_TRANSFORM:
                        ret = ProcessTransform();
                        break;
                    case MagicSort.MAGICSORT_RIDING:
                        ret = ProcessMount();
                        break;
                    case MagicSort.MAGICSORT_CLOSE_LINE:
                        ret = ProcessCloseLine();
                        break;
                    case MagicSort.MAGICSORT_TEAMFLAG:
                        ret = ProcessTeamFlag();
                        break;
                    case MagicSort.MAGICSORT_TRIPLEATTACK:
                        ret = ProcessTripleAttack();
                        break;
                    case MagicSort.MAGICSORT_STUNBOMB:
                        ret = ProcessStunBomb();
                        break;
                    case MagicSort.MAGICSORT_ATTACHSTATUS_AREA:
                        ret = ProcessAttachAreaStatus();
                        break;
                    case MagicSort.MAGICSORT_OBLIVION:
                        ret = ProcessOblivion();
                        break;
                    case 36:
                    case MagicSort.MAGICSORT_REMOTEBOMB:
                        ret = ProcessRemoteBomb();
                        break;
                    case MagicSort.MAGICSORT_KNOCKBACK:
                        ret = ProcessKnockback();
                        break;
                    case MagicSort.MAGICSORT_INCREASEBLOCK:
                        ret = ProcessIncreaseBlock();
                        break;
                    case MagicSort.MAGICSORT_DASHWHIRL:
                        ret = ProcessDashWhirl();
                        break;
                    case MagicSort.MAGICSORT_SPOOK:
                        ret = ProcessSpook();
                        break;
                    case MagicSort.MAGICSORT_WARCRY:
                        ret = ProcessWarCry();
                        break;
                    case MagicSort.MAGICSORT_DETACHBADSTATUS:
                        ret = ProcessDetachBadStatus();
                        break;
                    case MagicSort.MAGICSORT_COMPASSION:
                        ret = ProcessDetachTeam();
                        break;
                    case MagicSort.MAGICSORT_SELFDETACH:
                        ret = ProcessSelfDetach();
                        break;
                }
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), true, LogType.EXCEPTION);
            }
            return ret;
        }

        #region Magic Processing

        #region Sort 1 - MagicSort Attack

        private bool ProcessAttack()
        {
            if (m_pMagic == null || m_pOwner == null || m_idTarget == 0)
                return false;

            m_pSetTargetLocked.Clear();

            var pTarget = m_pOwner.BattleSystem.FindRole(m_idTarget);
            if (pTarget == null
                || !Calculations.InScreen(m_pOwner.MapX, m_pOwner.MapY, pTarget.MapX, pTarget.MapY)
                || (!pTarget.IsAlive && !pTarget.IsAttackable(m_pOwner)))
            {
                return false;
            }

            if (m_pOwner.IsImmunity(pTarget) || !pTarget.IsAttackable(m_pOwner))
                return false;

            if (m_pMagic.FloorAttr > 0)
            {
                int nAttr = pTarget.Map[pTarget.MapX, pTarget.MapY].Elevation;
                if (nAttr != m_pMagic.FloorAttr)
                    return false;
            }

            if ((m_pOwner.IsWing() || pTarget.IsWing()) && m_pMagic.Type == 6000)
                return false;

            int nTotalExp = 0;
            InteractionEffect pSpecial = InteractionEffect.NONE;
            int nPower = m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, pTarget, ref pSpecial);
            
            if (pTarget is Character)
            {
                if ((pTarget as Character).CheckScapegoat(m_pOwner))
                    return true;
            }

            var pMsg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level,
                CellX = m_pOwner.MapX,
                CellY = m_pOwner.MapY
            };
            pMsg.AppendTarget(pTarget.Identity, (uint)nPower, true, (uint)pSpecial, (uint)GetElementPower(pTarget));
            m_pOwner.Map.SendToRange(pMsg, pTarget.MapX, pTarget.MapY);

            CheckCrime(pTarget);

            if (nPower > 0)
            {
                int nLifeLost = (int)Math.Min(pTarget.Life, nPower);
                //pTarget.AddAttribute(ClientUpdateType.HITPOINTS, -1 * nLifeLost, true);
                pTarget.BeAttack(HitByMagic(), m_pOwner, nPower, true);

                var pNpc = pTarget as DynamicNpc;

                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                {
                    if (pNpc.IsCtfFlag())
                        (m_pOwner as Character).AwardCtfScore(pNpc, nLifeLost);
                    else
                        (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);
                }

                nTotalExp += nLifeLost;
            }

            if (m_pOwner is Character)
            {
                (m_pOwner as Character).SendWeaponMagic2(pTarget);
            }

            if (nTotalExp > 0)
            {
                AwardExpOfLife(pTarget, nTotalExp);
            }

            if (ServerKernel.ScorePkEvent.IsParticipating(m_pOwner.Identity))
            {
                ServerKernel.ScorePkEvent.AlterPoints(m_pOwner.Identity, m_pMagic.Type == 6000 ? 5 : 1);
            }

            //if (pTarget is Character && m_pOwner is Monster && !pTarget.IsAlive)
            //    m_pOwner.Kill(pTarget, GetDieMode());

            if (!pTarget.IsAlive)
                m_pOwner.Kill(pTarget, GetDieMode());

            return true;
        }

        #endregion

        #region Sort 2 - Recruit Magic

        private bool ProcessRecruit()
        {
            if (m_pMagic == null) return false;

            m_pSetTargetLocked.Clear();
            var setRole = new Dictionary<uint, IRole>();
            var setPower = new Dictionary<uint, int>();

            // TODO check team
            // TODO handle recruit team
            var pRoleOwner = m_pOwner as Character;
            if (pRoleOwner != null && pRoleOwner.Team != null && m_pMagic.Multi > 0)
            {
                // Add targets to dictionary
                foreach (var target in pRoleOwner.Team.Members.Values)
                    if (pRoleOwner.Map.IsInScreen(pRoleOwner, target))
                        setRole.Add(target.Identity, target);
            }
            else
            {
                var pRole = m_pOwner.BattleSystem.FindRole(m_idTarget) as IRole;
                if (pRole == null || !pRole.IsAlive)
                    return false;

                if (!m_pOwner.Map.IsInScreen(pRole as IScreenObject, m_pOwner as IScreenObject))
                    return false;

                setRole.Add(pRole.Identity, pRole);
                m_pSetTargetLocked.Add(pRole);
            }

            int nExp = 0;

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };

            foreach (var obj in setRole.Values)
            {
                if (!obj.IsAlive) continue;

                var nPower = GetPower();
                if (nPower == -32768)
                    nPower = (int)(obj.MaxLife - obj.Life);

                setPower.Add(obj.Identity, nPower);
                msg.AppendTarget(obj.Identity, (uint)nPower, false, 0, 0);
            }
            m_pOwner.Map.SendToRange(msg, m_pOwner.MapX, m_pOwner.MapY);

            foreach (var obj in setRole.Values)
            {
                if (!obj.IsAlive)
                    continue;

                var nAddLife = Calculations.CutOverflow(setPower[obj.Identity], Calculations.CutTrail(0, obj.MaxLife - obj.Life));
                if (nAddLife > 0)
                {
                    obj.AddAttribute(ClientUpdateType.HITPOINTS, nAddLife, true);
                    // todo broadcast team life
                }

                var pNpc = obj as DynamicNpc;
                if (pNpc != null && pNpc.IsGoal())
                    nAddLife = nAddLife * (10 / 100);

                nExp += (int)nAddLife;
            }

            AwardExp(0, 0, (m_pMagic.Power % 1000));
            return true;
        }

        #endregion

        #region Sort 4 - Fan

        private bool ProcessFan()
        {
            if (m_pMagic == null)
                return false;

            Magic pMagic = m_pMagic;
            var setRole = new Dictionary<uint, IRole>();
            var setPower = new Dictionary<uint, int>();
            var pos = new Point();

            int nRange = (int) m_pMagic.Distance + 2;
            int nWidth = MagicSort.DEFAULT_MAGIC_FAN;

            int nSize = nRange*2 + 1;
            int nBufSize = nSize ^ 2;

            int nExp = 0;
            int nPowerSum = 0;

            m_pSetTargetLocked.Clear();

            if (m_pMagic.Ground != 0)
            {
                pos.X = m_pOwner.MapX;
                pos.Y = m_pOwner.MapY;
            }
            else
            {
                IRole pTarget = (m_pOwner as IScreenObject).FindAroundRole(m_idTarget) as IRole;
                if (pTarget == null || !pTarget.IsAlive) return false;

                pos.X = pTarget.MapX;
                pos.Y = pTarget.MapY;
                setRole.Add(pTarget.Identity, pTarget);
            }

            var setTarget = m_pOwner.Map.CollectMapThing(nRange, ref pos);

            foreach (var pScrnObj in setTarget)
            {
                var posThis = new Point(pScrnObj.MapX, pScrnObj.MapY);
                if (pScrnObj.Identity == m_idTarget || !m_pOwner.IsInFan(posThis, pos, nRange, nWidth, m_pPos))
                    continue;
                var pRole = pScrnObj as IRole;
                if (pRole.IsAttackable(m_pOwner)
                    && !IsImmunity(pRole)
                    &&
                    Calculations.IsInFan(m_pPos, new Point(pScrnObj.MapX, pScrnObj.MapY), (int) m_pMagic.Distance,
                        nWidth, m_pPos))
                    setRole.Add(pRole.Identity, pRole);
            }

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level,
                CellX = (ushort) pos.X,
                CellY = (ushort) pos.Y
            };

            var setRemove = new List<uint>();
            foreach (var pRole in setRole.Values)
            {
                if (pRole.Identity == m_pOwner.Identity || !pRole.IsAttackable(m_pOwner))
                {
                    setRemove.Add(pRole.Identity);
                    continue;
                }

                if (pRole is Character)
                {
                    if ((pRole as Character).CheckScapegoat(m_pOwner))
                    {
                        setRemove.Add(pRole.Identity);
                        continue;
                    }
                }

                var special = InteractionEffect.NONE;
                int nPower = m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, pRole, ref special);

                setPower.Add(pRole.Identity, nPower);

                if (msg.TargetCount > 15)
                {
                    m_pOwner.Map.SendToRange(msg, m_pOwner.MapX, m_pOwner.MapY);
                    msg = new MsgMagicEffect
                    {
                        Identity = m_pOwner.Identity,
                        SkillIdentity = m_pMagic.Type,
                        SkillLevel = m_pMagic.Level,
                        CellX = (ushort) pos.X,
                        CellY = (ushort) pos.Y
                    };
                }

                msg.AppendTarget(pRole.Identity, (uint) nPower, true, (uint) special, (uint) GetElementPower(pRole));
            }
            m_pOwner.Map.SendToRange(msg, m_pOwner.MapX, m_pOwner.MapY);

            foreach (var i in setRemove)
                setRole.Remove(i);

            CheckCrime(setRole);

            bool bMgc2Dealt = false;
            foreach (var pRole in setRole.Values)
            {
                if (!pRole.IsAttackable(m_pOwner))
                    continue;

                nPowerSum += setPower[pRole.Identity];

                var pNpc = pRole as DynamicNpc;

                int nLifeLost = (int) Math.Min(pRole.Life, setPower[pRole.Identity]);
                //pRole.AddAttribute(ClientUpdateType.HITPOINTS, -1 * nLifeLost, true);
                pRole.BeAttack(HitByMagic(), m_pOwner, nLifeLost, true);

                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                {
                    if (pNpc.IsCtfFlag())
                        (m_pOwner as Character).AwardCtfScore(pNpc, nLifeLost);
                    else
                        (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);
                }

                if (pRole.IsMonster() || (pNpc != null && pNpc.IsGoal() && m_pOwner.Level >= pNpc.Level))
                {
                    nExp += m_pOwner.AdjustExperience(pRole, nLifeLost, false);
                    if (!pRole.IsAlive)
                    {
                        int nBonusExp = (int) (pRole.MaxLife*20/100);
                        m_pOwner.BattleSystem.OtherMemberAwardExp(pRole, nBonusExp);
                        nExp += m_pOwner.AdjustExperience(pRole, nBonusExp, false);
                    }
                }

                if (!pRole.IsAlive)
                    m_pOwner.Kill(pRole, GetDieMode());

                if (!bMgc2Dealt && Calculations.ChanceCalc(10f) && m_pOwner is Character)
                {
                    (m_pOwner as Character).SendWeaponMagic2(pRole);
                    bMgc2Dealt = true;
                }
            }

            AwardExp(nExp, nExp, false, pMagic);
            return true;
        }

        #endregion

        #region Sort 5 - Bomb Magic
        private bool ProcessBomb()
        {
            if (m_pMagic == null) return false;
            var setRole = new Dictionary<uint, IRole>();
            var setPower = new Dictionary<uint, int>();
            var setRemove = new List<uint>();

            var pos = m_pPos;

            int nExp = 0;
            int nPowerSum = 0;

            CollectTargetSet_Bomb(ref pos, 0, (int)m_pMagic.Range + 1);

            var msg = new MsgMagicEffect
            {
                CellX = (ushort)pos.X,
                CellY = (ushort)pos.Y,
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };

            int targetNum = 0;
            foreach (var obj in m_pSetTargetLocked)
            {
                if (obj.Identity != m_pOwner.Identity && obj.IsAttackable(m_pOwner) && !IsImmunity(obj))
                {
                    var special = InteractionEffect.NONE;
                    int nPower = m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, obj, ref special);

                    if (obj is Character)
                    {
                        if ((obj as Character).CheckScapegoat(m_pOwner))
                        {
                            nPower = 0;
                        }
                    }

                    setPower.Add(obj.Identity, nPower);

                    if (targetNum++ < _MAX_TARGET_NUM)
                    {
                        msg.AppendTarget(obj.Identity, (uint)nPower, true, (uint)special, (uint)GetElementPower(obj));
                    }
                    else
                    {
                        m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);
                        msg = new MsgMagicEffect
                        {
                            CellX = (ushort)pos.X,
                            CellY = (ushort)pos.Y,
                            Identity = m_pOwner.Identity,
                            SkillIdentity = m_pMagic.Type,
                            SkillLevel = m_pMagic.Level
                        };
                        msg.AppendTarget(obj.Identity, (uint)nPower, true, (uint)special, (uint)GetElementPower(obj));
                        targetNum = 0;
                    }

                    setRole.Add(obj.Identity, obj);

                    CheckCrime(obj);
                }
                else
                    setRemove.Add(obj.Identity);
            }

            foreach (var rem in setRemove)
                setRole.Remove(rem);

            m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);

            foreach (var obj in setRole.Values)
            {
                if (!obj.IsAttackable(m_pOwner))
                    continue;

                nPowerSum += setPower[obj.Identity];

                var pNpc = obj as DynamicNpc;
                int nLifeLost = (int)Math.Min(obj.Life, setPower[obj.Identity]);

                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                {
                    if (pNpc.IsCtfFlag())
                        (m_pOwner as Character).AwardCtfScore(pNpc, nLifeLost);
                    else
                        (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);
                }

                obj.BeAttack(HitByMagic(), m_pOwner, setPower[obj.Identity], true);

                if (obj.IsMonster()
                    || (pNpc != null && pNpc.IsGoal() && pNpc.Level < m_pOwner.Level))
                {
                    nExp += m_pOwner.AdjustExperience(obj, nLifeLost, false);

                    if (!obj.IsAlive)
                    {
                        int nBonusExp = (int)(obj.MaxLife * (5 / 100));

                        m_pOwner.BattleSystem.OtherMemberAwardExp(obj, nExp);

                        nExp += m_pOwner.AdjustExperience(obj, nBonusExp, true);
                    }
                }

                if (!obj.IsAlive)
                    m_pOwner.Kill(obj, GetDieMode());
            }

            if (ServerKernel.ScorePkEvent.IsParticipating(m_pOwner.Identity))
            {
                ServerKernel.ScorePkEvent.AlterPoints(m_pOwner.Identity, 1);
            }

            AwardExp(0, nExp, nExp);

            return true;
        }
        #endregion

        #region Sort 6 - Attach Status

        private bool ProcessAttach()
        {
            if (m_pMagic == null) return false;
            m_pSetTargetLocked.Clear();

            var pRoleUser = m_pOwner.BattleSystem.FindRole(m_idTarget);

            if (pRoleUser == null && m_idTarget == m_pOwner.Identity)
                pRoleUser = m_pOwner;
            if (pRoleUser == null)
                return false;

            if (!pRoleUser.IsAlive && m_pMagic.Target != 64)
                return false;

            if (m_pMagic.Target == 64)
            {
                if (pRoleUser.IsAlive || !(pRoleUser is Character))
                    return false;

                if (m_pMagic.Status == FlagInt.SHACKLED && pRoleUser.QueryStatus(FlagInt.SHACKLED) != null && m_pOwner is Character)
                {
                    (m_pOwner as Character).Send(string.Format(ServerString.STR_TARGET_ALREADY_SHACKED));
                    return false;
                }
            }

            int nPower = GetPower();
            int nSecs = (int)m_pMagic.StepSecs;
            int nTimes = (int)m_pMagic.ActiveTimes;
            int nStatus = (int)m_pMagic.Status;
            byte pLevel = (byte) m_pMagic.Level;

            if (nPower < 0)
            {
                ServerKernel.Log.SaveLog(string.Format("ERROR: magic type [{0}] status [{1}] invalid power", m_pMagic.Type, nStatus), false);
                return false;
            }

            uint nDmg = 1;
            switch (nStatus)
            {
                case FlagInt.FLY:
                    {
                        if (pRoleUser.Identity != m_pOwner.Identity)
                            return false;
                        if (!pRoleUser.IsBowman() || !pRoleUser.IsAlive)
                            return false; // cant fly
                        if (pRoleUser.Map.IsWingDisable())
                            return false;
                        if (pRoleUser.QueryStatus(FlagInt.RIDING) != null)
                            pRoleUser.DetachStatus(FlagInt.RIDING);
                        break;
                    }
                case FlagInt.SHACKLED:
                case FlagInt.POISON_STAR:
                    {
                        if (m_pOwner.IsImmunity(pRoleUser))
                            return false;
                        float nChance = 100f;
                        if (m_pOwner.BattlePower < pRoleUser.BattlePower)
                        {
                            int nDeltaLev = pRoleUser.BattlePower - m_pOwner.BattlePower;
                            if (nDeltaLev < 20)
                                nChance = (float)(100 - (nDeltaLev * 5));
                            else
                                nChance = 0;
                        }
                        if (nChance < 0 || !Calculations.ChanceCalc(nChance))
                            nDmg = 0;
                        else if (m_pOwner is Character)
                        {
                            Character pUser = m_pOwner as Character;
                            if (ServerKernel.CaptureTheFlag.IsInside(pUser.Identity))
                                ServerKernel.CaptureTheFlag.AddPoints(pUser, 1);
                        }
                        break;
                    }
                case FlagInt.LUCKY_ABSORB:
                    {
                        nStatus = FlagInt.LUCKY_DIFFUSE;

                        if (m_pOwner is Character)
                        {
                            Character pUser = m_pOwner as Character;
                            if (pUser.Booth != null && pUser.Booth.Vending)
                                nDmg = 0;
                        }
                        break;
                    }
                case FlagInt.INVISIBLE:
                    {
                        return false;
                    }
            }

            if (nDmg == 0 && nStatus == FlagInt.LUCKY_DIFFUSE)
                return false;

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            msg.AppendTarget(pRoleUser.Identity, nDmg, nDmg != 0, 0, 0);
            m_pOwner.Map.SendToRange(msg, m_pOwner.MapX, m_pOwner.MapY);

            CheckCrime(pRoleUser);
            
            if (nDmg == 0)
                return false;

            pRoleUser.AttachStatus(pRoleUser, nStatus, nPower, nSecs, nTimes, pLevel);

            int nExp = 3;
            if (m_pOwner.Map.IsTrainingMap())
                nExp = 1;

            if (pRoleUser is Character)
            {
                Character pRoleChar = pRoleUser as Character;
                if (nPower >= 30000)
                {
                    var nPowerTimes = (nPower - 30000) - 100;
                    nExp = m_pMagic.NeedExp % 100;
                    switch (nStatus)
                    {
                        case FlagInt.STIG: // stigma
                            pRoleChar.Send(string.Format(ServerString.STR_STIGMA_ACTIVE_P,
                                    nSecs, nPowerTimes), ChatTone.TOP_LEFT);
                            break;
                        case FlagInt.STAR_OF_ACCURACY: // accuracy
                            pRoleChar.Send(string.Format(ServerString.STR_ACCURACY_ACTIVE_P,
                                    nSecs, nPowerTimes), ChatTone.TOP_LEFT);
                            break;
                        case FlagInt.SHIELD: // shield
                            pRoleChar.Send(string.Format(ServerString.STR_SHIELD_ACTIVE_P,
                                    nSecs, nPowerTimes), ChatTone.TOP_LEFT);
                            break;
                        case FlagInt.AZURE_SHIELD: // azure shield
                            break;
                        case FlagInt.DODGE: // dodge
                            pRoleChar.Send(string.Format(ServerString.STR_DODGE_ACTIVE_P,
                                    nSecs, nPowerTimes), ChatTone.TOP_LEFT);
                            break;
                    }
                }
                else
                {
                    int nPowerTimes = nPower;
                    switch (nStatus)
                    {
                        case FlagInt.STIG: // stigma
                            pRoleChar.Send(string.Format(ServerString.STR_STIGMA_ACTIVE_T,
                                    nSecs, nPowerTimes), ChatTone.TOP_LEFT);
                            break;
                        case FlagInt.STAR_OF_ACCURACY: // accuracy
                            pRoleChar.Send(string.Format(ServerString.STR_ACCURACY_ACTIVE_T,
                                    nSecs, nPowerTimes), ChatTone.TOP_LEFT);
                            break;
                        case FlagInt.SHIELD: // shield
                            pRoleChar.Send(string.Format(ServerString.STR_SHIELD_ACTIVE_T,
                                    nSecs, nPowerTimes), ChatTone.TOP_LEFT);
                            break;
                        case FlagInt.AZURE_SHIELD: // azure shield
                            break;
                        case FlagInt.DODGE: // dodge
                            pRoleChar.Send(string.Format(ServerString.STR_DODGE_ACTIVE_T,
                                    nSecs, nPowerTimes), ChatTone.TOP_LEFT);
                            break;
                    }
                }

                AwardExp(0, nExp, false);
            }

            return true;
        }
        #endregion

        #region Sort 7 - Detach Status

        private bool ProcessDetach()
        {
            if (m_pMagic == null)
                return false;

            m_pSetTargetLocked.Clear();

            var pRole = m_pOwner.BattleSystem.FindRole(m_idTarget) as IRole;
            if (pRole == null) return false;

            int nPower = 0;
            int nSecs = (int)m_pMagic.StepSecs;
            int nTimes = (int)m_pMagic.ActiveTimes;
            int nStatus = (int)m_pMagic.Status;

            if (!pRole.IsAlive)
            {
                if (nStatus != 0)
                    return false;

                if (!(pRole is Character))
                    return false;
            }

            if (nStatus == 0)
                if (pRole.Map.IsPkField())
                    return false;

            if (nStatus == 0 && pRole is Character)
            {
                // reborn
                (pRole as Character).Reborn(false, true);
                pRole.Send(new MsgMapInfo(pRole.Map.Identity,
                    pRole.Map.BaseIdentity, pRole.Map.WarFlag));

                if (m_pOwner is Character)
                {
                    Character pUser = m_pOwner as Character;
                    if (ServerKernel.CaptureTheFlag.IsInside(pUser.Identity))
                        ServerKernel.CaptureTheFlag.AddPoints(pUser, 5);
                }
            }
            else
            {
                switch (m_pMagic.Status)
                {
                    case FlagInt.FLY:
                        if (pRole is Character) // if is character
                        {
                            var pUser = pRole as Character;
                            if (!pUser.IsWing()) // the target is flying?
                                break;//return false;

                            if (pUser.BattlePower <= m_pOwner.BattlePower) // caster does have more bp than victim
                            {
                                pUser.DetachStatus(FlagInt.FLY);//pUser.Status.RemoveStatus0(Effect0.FLY));
                                var special = InteractionEffect.NONE;
                                nPower = (int)(pUser.MaxLife * ((m_pMagic.Power % 10000) / 100f));
                            }
                            else
                            {
                                var diff = m_pOwner.BattlePower - pUser.BattlePower; // get the diff
                                float pct = 100;
                                if (diff > 19) // if diff >= 20
                                    pct = 5; // 5 pct chance only
                                else
                                    pct -= diff * 5; // oooor
                                if (Calculations.ChanceCalc(pct)) // if success, remove fly of target
                                {
                                    pUser.DetachStatus(FlagInt.FLY); //pUser.Status.RemoveStatus0(Effect0.FLY));
                                    var special = InteractionEffect.NONE;
                                    nPower = (int)(pUser.MaxLife * ((m_pMagic.Power % 10000) / 100f));
                                }
                            }
                        }
                        else // isnt character, return
                            return false;
                        break;
                }
            }

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            msg.AppendTarget(pRole.Identity, (uint)nPower, true, 0, 0);
            pRole.Map.SendToRange(msg, pRole.MapX, pRole.MapY);

            if (nPower > 0)
            {
                int nLifeLost = (int)Math.Min(pRole.Life, nPower);
                pRole.AddAttribute(ClientUpdateType.HITPOINTS, -1 * nLifeLost, true);
                pRole.BeAttack(HitByMagic(), m_pOwner, nLifeLost, true);
            }

            return true;
        }

        #endregion

        #region Sort 11 - Dispatch XP

        private bool ProcessDispatchXp()
        {
            if (m_pMagic == null) return false;

            m_pSetTargetLocked.Clear();

            Dictionary<uint, Character> setUser = new Dictionary<uint, Character>();
            Dictionary<uint, int> setPower = new Dictionary<uint, int>();

            Team pTeam = null;
            Character pOwner = null;

            if (m_pOwner is Character)
                pOwner = m_pOwner as Character;

            if (pOwner.Team != null)
            {
                pTeam = pOwner.Team;
                string szMessage = string.Format(ServerString.STR_DISPATCHXP, pOwner.Name);

                foreach (var member in pTeam.Members.Values.Where(x => x.Identity != pOwner.Identity && x.IsAlive))
                {
                    if (member.GetDistance(pOwner) > Calculations.SCREEN_DISTANCE)
                        continue;

                    member.AddXp(20);
                    setUser.Add(member.Identity, member);
                    setPower.Add(member.Identity, 20);
                    member.Send(szMessage);
                }
            }

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level,
                CellX = (ushort)m_pPos.X,
                CellY = (ushort)m_pPos.Y
            };

            foreach (var tgt in setUser.Values)
                msg.AppendTarget(tgt.Identity, (uint)setPower[tgt.Identity], true, 0, 0);

            pOwner.Screen.Send(msg, true);

            AwardExp(0, 0, 0, m_pMagic);

            return true;
        }

        #endregion

        #region Sort 12 - Collide

        private bool ProcessCollide()
        {
            if (m_pMagic == null || m_pOwner == null)
                return false;

            m_pSetTargetLocked.Clear();

            if (m_pOwner is Character && !(m_pOwner as Character).SynchroPosition(m_pPos.X, m_pPos.Y))
            {
                (m_pOwner as Character).Disconnect("ProcessCollide SynPosition");
                return false;
            }

            int nDir = m_nData % 8;
            ushort nTargetX = (ushort)(m_pPos.X + Handlers.WALK_X_COORDS[nDir]);
            ushort nTargetY = (ushort)(m_pPos.Y + Handlers.WALK_Y_COORDS[nDir]);

            if (!m_pOwner.Map.IsStandEnable(nTargetX, nTargetY))
            {
                m_pOwner.Send(new MsgTalk(ServerString.STR_INVALID_MSG, ChatTone.TOP_LEFT));
                if (m_pOwner is Character)
                    (m_pOwner as Character).Disconnect("Collide coord!");
                return false;
            }

            // search the target
            bool bSuc = true;

            IRole pTarget = m_pOwner.Map.QueryRole(nTargetX, nTargetY) as IRole;
            if (pTarget == null || !pTarget.IsAlive)
                bSuc = false;

            if (pTarget == null || IsImmunity(pTarget))
                bSuc = false;

            int nPower = 0;
            bool bBackEnable = false;
            uint idTarget = 0;

            DynamicNpc pNpc = null;
            InteractionEffect pSpecial = InteractionEffect.NONE;
            if (pTarget != null)
            {
                pNpc = pTarget as DynamicNpc;

                if (bSuc)
                {
                    bBackEnable = m_pOwner.Map.IsStandEnable((ushort)(nTargetX + Handlers.WALK_X_COORDS[nDir]),
                        (ushort)(nTargetY + Handlers.WALK_Y_COORDS[nDir]));

                    if (pNpc != null)
                        bBackEnable = false;

                    if (!bBackEnable)
                        nDir = 0;

                    if (HitByWeapon())
                    {
                        nPower += m_pOwner.BattleSystem.CalcAttackPower(m_pOwner, pTarget, ref pSpecial);

                        if (bBackEnable || pNpc != null)
                            nPower = Calculations.MulDiv(nPower, 80, 100);
                        else
                            nPower = Calculations.AdjustDataEx(nPower, GetPower(), 0);
                    }

                    if (!m_pOwner.Map.IsTrainingMap() && m_pOwner is Character)
                        (m_pOwner as Character).AddEquipmentDurability(ItemPosition.LEFT_HAND, -1 * 3);
                }

                idTarget = pTarget.Identity;
            }

            uint dwData = (uint)(nDir * 0x01000000 + nPower);
            if (!bBackEnable)
                dwData += 1 * 0x10000000;

            MsgInteract pMsg = new MsgInteract
            {
                EntityIdentity = m_pOwner.Identity,
                TargetIdentity = idTarget,
                CellX = nTargetX,
                CellY = nTargetY,
                Data = dwData,
                ActivationType = pSpecial
            };
            m_pOwner.Screen.Send(pMsg, true);

            if (m_pOwner is Character && (m_pOwner as Character).IsPm)
                m_pOwner.Send(new MsgTalk(string.Format("bump move:({0},{1})->({2},{3})",
                    m_pOwner.MapX, m_pOwner.MapY, nTargetX, nTargetY), ChatTone.TALK));

            if (bBackEnable && (m_pOwner is Character))
            {
                (m_pOwner as Character).ProcessOnMove();
                (m_pOwner as Character).MoveToward((FacingDirection)nDir, true);
            }

            if (pTarget != null && bBackEnable && bSuc && pTarget is Character)
            {
                (pTarget as Character).ProcessOnMove();
                (pTarget as Character).MoveToward((FacingDirection)nDir, true);
                // not synchro yet
            }

            if (pTarget != null)
                CheckCrime(pTarget);

            if (nPower > 0 && pTarget != null)
            {
                int nLifeLost = (int)Math.Min(nPower, pTarget.Life);

                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                    (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);

                pTarget.BeAttack(HitByMagic(), m_pOwner, nLifeLost, true);

                if (pTarget.IsMonster() || pNpc != null && pNpc.IsGoal() && m_pOwner.Level >= pNpc.Level)
                {
                    int nExp = m_pOwner.AdjustExperience(pTarget, nLifeLost, false);

                    if (!pTarget.IsAlive)
                    {
                        int nBonusExp = (int)(pTarget.MaxLife * 0.05f);
                        m_pOwner.BattleSystem.OtherMemberAwardExp(pTarget, nBonusExp);

                        nExp += m_pOwner.AdjustExperience(pTarget, nBonusExp, true);
                    }

                    AwardExp(0, nExp, nExp, m_pMagic);
                }

                if (!pTarget.IsAlive)
                    m_pOwner.Kill(pTarget, GetDieMode());
            }

            return true;
        }

        #endregion

        #region Sort 14 - Line Attack

        private bool ProcessLine()
        {
            if (m_pMagic == null)
                return false;

            var setTarget = new Dictionary<uint, IRole>();
            var setPower = new Dictionary<uint, int>();
            var setPoint = new List<Point>();

            int nExp = 0;
            int nPowerSum = 0;

            m_pSetTargetLocked.Clear();

            var pos = new Point(m_pOwner.MapX, m_pOwner.MapY);

            Calculations.DDALine(pos.X, pos.Y, m_pPos.X, m_pPos.Y, (int)m_pMagic.Range, ref setPoint);

            foreach (var point in setPoint)
            {
                try
                {
                    if (m_pOwner.Map[point.X, point.Y].Access < TileType.NPC)
                        continue;

                    if (m_pOwner.Map[point.X, point.Y].Elevation - m_pOwner.Map[m_pOwner.MapX, m_pOwner.MapY].Elevation > 26)
                        continue;
                }
                catch { continue; }

                var pTarget = m_pOwner.Map.QueryRole((ushort)point.X, (ushort)point.Y) as IRole;
                if (pTarget == null
                    || IsImmunity(pTarget)
                    || !pTarget.IsAttackable(m_pOwner))
                    continue;

                if (pTarget.Identity != m_pOwner.Identity)
                    setTarget.Add(pTarget.Identity, pTarget);
            }

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level,
                CellX = (ushort)m_pPos.X,
                CellY = (ushort)m_pPos.Y
            };

            var setRemove = new List<IRole>();
            int nTargetNum = 0, nCount = 0;
            foreach (var pTarget in setTarget.Values)
            {
                if (pTarget.IsAttackable(m_pOwner) && !IsImmunity(pTarget))
                {
                    var special = InteractionEffect.NONE;
                    int nPower = m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, pTarget, ref special);

                    if (pTarget is Character)
                    {
                        if ((pTarget as Character).CheckScapegoat(m_pOwner))
                        {
                            nPower = 0;
                        }
                    }

                    if (++nTargetNum < 50)
                    {
                        msg.AppendTarget(pTarget.Identity, (uint)nPower, true, (byte)special, (uint) GetElementPower(pTarget));
                    }
                    else
                    {
                        m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);
                        msg = new MsgMagicEffect
                        {
                            CellX = (ushort)pos.X,
                            CellY = (ushort)pos.Y,
                            Identity = m_pOwner.Identity,
                            SkillIdentity = m_pMagic.Type,
                            SkillLevel = m_pMagic.Level
                        };
                        msg.AppendTarget(pTarget.Identity, 0, true, (byte) special, (uint) GetElementPower(pTarget));
                        nTargetNum = 0;
                        nCount++;
                    }
                    setPower.Add(pTarget.Identity, nPower);
                    CheckCrime(pTarget);
                        
                }
                else
                    setRemove.Add(pTarget);
            }

            foreach (var remove in setRemove)
                setTarget.Remove(remove.Identity);

            m_pOwner.Map.SendToRange(msg, m_pOwner.MapX, m_pOwner.MapY);

            foreach (var pTarget in setTarget.Values)
            {
                if (!pTarget.IsAttackable(m_pOwner))
                    continue;

                nPowerSum += setPower[pTarget.Identity];

                DynamicNpc pNpc = null;
                if (pTarget is DynamicNpc)
                    pNpc = pTarget as DynamicNpc;

                int nLifeLost = (int)Math.Min(pTarget.Life, setPower[pTarget.Identity]);
                //pTarget.AddAttribute(ClientUpdateType.HITPOINTS, -1 * setPower[pTarget.Identity], true);                //pTarget.AddAttribute(ClientUpdateType.HITPOINTS, -1 * setPower[pTarget.Identity], true);

                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                {
                    if (pNpc.IsCtfFlag())
                        (m_pOwner as Character).AwardCtfScore(pNpc, nLifeLost);
                    else
                        (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);
                }

                pTarget.BeAttack(HitByMagic(), m_pOwner, nLifeLost, true);

                if (pTarget.IsMonster() || pNpc != null && pNpc.IsGoal() && pTarget.Level < m_pOwner.Level)
                {
                    nExp += m_pOwner.AdjustExperience(pTarget, nLifeLost, false);
                    if (!pTarget.IsAlive)
                    {
                        int nBonusExp = (int)(pTarget.MaxLife * (5 / 100));

                        m_pOwner.BattleSystem.OtherMemberAwardExp(pTarget, nBonusExp);

                        nExp += m_pOwner.AdjustExperience(m_pOwner, nBonusExp, true);
                    }
                }

                Character owner = m_pOwner as Character;
                if (owner != null && pTarget is Character)
                {
                    if (ServerKernel.ScorePkEvent.IsParticipating(m_pOwner.Identity))
                    {
                        ServerKernel.ScorePkEvent.AlterPoints(m_pOwner.Identity, 5);
                    }

                    if (m_pOwner.MapIdentity == LineSkillPkTournament.MAP_ID_U)
                    {
                        ServerKernel.LineSkillPk.Hit(owner, pTarget as Character);
                    }
                }

                if (!pTarget.IsAlive)
                    m_pOwner.Kill(pTarget, (ulong)GetDieMode());
            }


            if (HitByWeapon() && m_pOwner.Map.IsTrainingMap())
            {
                // TODO decrease durability
            }

            AwardExp(0, 0, nExp);
            return true;
        }

        #endregion

        #region Sort 16 - Attack Status
        private bool ProcessAtkStatus()
        {
            if (m_pMagic == null) return false;

            m_pSetTargetLocked.Clear();
            var pTarget = m_pOwner.BattleSystem.FindRole(m_idTarget) as IRole;
            if (pTarget == null)
                return false;

            if (!pTarget.IsAttackable(m_pOwner) && !pTarget.IsAlive)
                return false;

            if (IsImmunity(pTarget))
                return false;

            int nPower = 0;
            bool bAttachStatus = false;
            int nStatusPower = 0;
            int nSecs = 0;
            int nTimes = 0;
            ulong nStatus = 0;

            int effectType = 1;

            var special = InteractionEffect.NONE;

            if (HitByWeapon())
            {
                int nAdjust = 0;

                switch (m_pMagic.Status)
                {
                    case 0:
                        break;
                    default:
                        {
                            if (Calculations.ChanceCalc(m_pMagic.Percent))
                            {
                                bAttachStatus = true;
                                nStatusPower = GetPower();
                                nSecs = (int)m_pMagic.StepSecs;
                                nTimes = (int)m_pMagic.ActiveTimes;
                                if (m_pMagic.Status > 64)
                                {
                                    nStatus = 1UL << (int)(m_pMagic.Status - 64);
                                    effectType = 2;
                                }
                                else
                                    nStatus = 1UL << (int)m_pMagic.Status;
                            }

                            nPower = m_pOwner.BattleSystem.CalcAttackPower(m_pOwner, pTarget, ref special);

                            break;
                        }
                }
            }

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            msg.AppendTarget(pTarget.Identity, (uint)nPower, false, (byte)special, (uint) GetElementPower(pTarget));
            m_pOwner.Map.SendToRange(msg, pTarget.MapX, pTarget.MapY);

            CheckCrime(pTarget);

            var pNpc = pTarget as DynamicNpc;
            int nExp = 0;
            if (nPower > 0)
            {
                int nLifeLost = 0;

                nLifeLost = (int)Math.Min(pTarget.Life, nPower);
                //pTarget.AddAttribute(ClientUpdateType.HITPOINTS, -1 * nLifeLost, true);
                // todo broadcast team life

                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                {
                    if (pNpc.IsCtfFlag())
                        (m_pOwner as Character).AwardCtfScore(pNpc, nLifeLost);
                    else
                        (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);
                }

                pTarget.BeAttack(HitByMagic(), m_pOwner, nLifeLost, true);

                if (pTarget.IsMonster()
                    || (pNpc != null && pNpc.IsGoal() && pNpc.Level < m_pOwner.Level))
                {
                    nExp += m_pOwner.AdjustExperience(pTarget, nLifeLost, false);
                    if (!pTarget.IsAlive)
                    {
                        int nBonusExp = (int)(pTarget.MaxLife * (5 / 100));
                        m_pOwner.BattleSystem.OtherMemberAwardExp(pTarget, nBonusExp);
                        nExp += m_pOwner.AdjustExperience(pTarget, nBonusExp, true);
                    }
                }
            }

            AwardExp(0, nExp, GetPower() % 100);

            if (bAttachStatus)
            {
                if (pTarget.IsAlive)
                {
                    pTarget.AttachStatus(pTarget, (int)m_pMagic.Status, 0, 0, 0, 0);
                }
            }

            if (!pTarget.IsAlive)
                pTarget.BeKill(m_pOwner);

            return true;
        }
        #endregion

        #region Sort 19 - Transform

        private bool ProcessTransform()
        {
            if (m_pMagic == null || !(m_pOwner is Character))
                return false;

            m_pSetTargetLocked.Clear();

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level,
                CellX = (ushort)m_pPos.X,
                CellY = (ushort)m_pPos.Y
            };
            m_pOwner.Map.SendToRange(msg, m_pOwner.MapX, m_pOwner.MapY);

            (m_pOwner as Character).Transform((uint)m_pMagic.Power, (int)m_pMagic.StepSecs, true);

            return true;
        }

        #endregion

        #region Sort 20 - Add Mana

        private bool ProcessAddMana()
        {
            if (m_pMagic == null) return false;

            m_pSetTargetLocked.Clear();
            int nAddMana = GetPower();
            nAddMana = Calculations.CutOverflow(nAddMana, m_pOwner.MaxMana - m_pOwner.Mana);

            MsgMagicEffect pMsg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                CellX = m_pOwner.MapX,
                CellY = m_pOwner.MapY,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            pMsg.AppendTarget(m_pOwner.Identity, (uint)nAddMana, true, 0, 0);
            m_pOwner.Map.SendToRange(pMsg, m_pOwner.MapX, m_pOwner.MapY);

            m_pOwner.AddAttribute(ClientUpdateType.MANA, nAddMana, true);

            int nExp = m_pMagic.Power;
            if (m_pOwner.Map.IsTrainingMap())
                nExp /= 10;

            AwardExp(0, 0, nExp);
            return true;
        }

        #endregion

        #region Sort 23 - Call Pet

        private bool ProcessCallPet()
        {
            if (m_pMagic == null) return false;

            Character pUser;
            if (!(m_pOwner is Character))
                return false;

            pUser = m_pOwner as Character;

            m_pSetTargetLocked.Clear();

            MsgInteract pMsg = new MsgInteract
            {
                EntityIdentity = pUser.Identity,
                CellX = pUser.MapX,
                CellY = pUser.MapY,
                MagicType = m_pMagic.Type,
                MagicLevel = m_pMagic.Level
            };
            pUser.Screen.Send(pMsg, true);

            //pUser.CallPet((uint)GetPower(), (ushort)m_pPos.X, (ushort)m_pPos.Y, (int)m_pMagic.StepSecs);

            AwardExp(0, 0, 0, m_pMagic);
            return true;
        }

        #endregion

        #region Sort 26 - Decrease Life

        private bool ProcessDecLife()
        {
            if (m_pMagic == null)
                return false;

            m_pSetTargetLocked.Clear();

            IRole pTarget = m_pOwner.BattleSystem.FindRole(m_idTarget);
            if (pTarget == null || !pTarget.IsAlive)
                return false;

            if (IsImmunity(pTarget))
                return false;

            int nPower = Calculations.CutTrail(0, Calculations.AdjustDataEx((int)pTarget.Life, GetPower(), 0));

            MsgMagicEffect msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            msg.AppendTarget(m_idTarget, (uint)nPower, true, 0, 0);
            m_pOwner.Map.SendToRange(msg, pTarget.MapX, pTarget.MapY);

            CheckCrime(pTarget);

            if (nPower > 0)
            {
                int nLifeLost = (int)Math.Min(pTarget.Life, nPower);
                pTarget.AddAttribute(ClientUpdateType.HITPOINTS, nLifeLost, true);
                pTarget.BeAttack(HitByMagic(), m_pOwner, nPower, true);

                AwardExpOfLife(m_pOwner, nPower);

                if (!pTarget.IsAlive)
                    m_pOwner.Kill(pTarget, GetDieMode());
            }

            return true;
        }

        #endregion

        #region Sort 27 - Ground Sting
        private bool ProcessGroundSting()
        {
            if (m_pMagic == null) return false;
            if (m_pMagic.Status <= 0) return false;
            if (m_pOwner.MapIdentity == 1005)
            {
                m_pOwner.Send(new MsgTalk("You cannot use this skill in this map.", ChatTone.TOP_LEFT));
                return false;
            }

            var setRole = new Dictionary<uint, IRole>();
            var pos = new Point(m_pPos.X, m_pPos.Y);

            CollectTargetSet_Bomb(ref pos, 0, (int)m_pMagic.Range);

            var msg = new MsgMagicEffect
            {
                CellX = (ushort)pos.X,
                CellY = (ushort)pos.Y,
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };

            int targetNum = 0;
            int nCount = 0;
            var remove = new List<IRole>();
            foreach (var obj in m_pSetTargetLocked)
            {
                Monster mob = obj as Monster;
                if (obj.IsAttackable(m_pOwner)
                    && obj != m_pOwner)
                {
                    if (obj.GetDistance(m_pOwner as IScreenObject) > m_pMagic.Distance || !(obj is Character) && mob == null)
                    {
                        remove.Add(obj);
                        continue;
                    }

                    if (mob != null && (mob.IsGuard() || mob.IsDynaMonster()))
                    {
                        remove.Add(obj);
                        continue;
                    }

                    if (mob != null && mob.IsBoss)
                    {
                        msg.AppendTarget(obj.Identity, 0, true, 0, 0);
                        remove.Add(obj);
                        continue;
                    }

                    if (IsImmunity(obj))
                    {
                        remove.Add(obj);
                        continue;
                    }

                    int nDeltaBp = 0;
                    float nChance = 0;
                    if (m_pOwner.BattlePower >= obj.BattlePower || !(m_pOwner is Character))
                        nChance = 100f;
                    else
                    {
                        nDeltaBp = obj.BattlePower - m_pOwner.BattlePower;
                        nChance = nDeltaBp >= 20 ? 0 : 100 - (nDeltaBp * 5);
                    }

                    uint nDmg = 1;
                    if (!Calculations.ChanceCalc(nChance))
                        nDmg = 0;

                    if (nDmg <= 0)
                        remove.Add(obj);

                    if (++targetNum < 50)
                    {
                        msg.AppendTarget(obj.Identity, nDmg, true, 0, 0);
                    }
                    else
                    {
                        m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);
                        msg = new MsgMagicEffect
                        {
                            CellX = (ushort)pos.X,
                            CellY = (ushort)pos.Y,
                            Identity = m_pOwner.Identity,
                            SkillIdentity = m_pMagic.Type,
                            SkillLevel = m_pMagic.Level
                        };
                        msg.AppendTarget(obj.Identity, nDmg, true, 0, 0);
                        targetNum = 0;
                        nCount++;
                    }

                    if (nDmg > 0)
                        CheckCrime(obj);
                }
                else
                {
                    remove.Add(obj);
                }
            }
            m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);

            foreach (var rem in remove)
                m_pSetTargetLocked.Remove(rem);

            foreach (var obj in m_pSetTargetLocked)
            {
                obj.AttachStatus(obj, (int)m_pMagic.Status, m_pMagic.Power,
                    (int)m_pMagic.StepSecs, (int)m_pMagic.ActiveTimes, 0);
            }

            int nExp = m_pMagic.Power % 100;
            if (m_pOwner.Map.IsTrainingMap())
                nExp /= 5;

            AwardExp(0, nExp, false);

            return true;
        }
        #endregion

        #region Sort 28 - Vortex

        private bool ProcessVortex()
        {
            if (m_pMagic == null)
                return false;

            if (m_pOwner.QueryStatus(FlagInt.VORTEX) != null && m_pMagic.IsReady()) // vortex active
            {
                m_pMagic.StartDelay();

                var pos = new Point();

                int nExp = 0;
                int nPowerSum = 0;

                CollectTargetSet_Bomb(ref pos, 0, (int)m_pMagic.Range);

                var msg = new MsgMagicEffect
                {
                    CellX = (ushort)pos.X,
                    CellY = (ushort)pos.Y,
                    Identity = m_pOwner.Identity,
                    SkillIdentity = m_pMagic.Type,
                    SkillLevel = m_pMagic.Level
                };

                int targetNum = 0, nCount = 0;
                foreach (var obj in m_pSetTargetLocked)
                {
                    if (obj.Identity != m_pOwner.Identity
                        && obj.IsAttackable(m_pOwner))
                    {
                        if (!obj.IsAttackable(m_pOwner) || IsImmunity(obj))
                            continue;

                        var special = InteractionEffect.NONE;
                        int nPower = m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, obj, ref special);//, ref special);

                        if (obj is Character)
                        {
                            if ((obj as Character).CheckScapegoat(m_pOwner))
                            {
                                nPower = 0;
                            }
                        }

                        if (++targetNum < _MAX_TARGET_NUM)
                        {
                            msg.AppendTarget(obj.Identity, (uint)nPower, true, (byte)special, (uint) GetElementPower(obj));
                        }
                        else
                        {
                            m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);
                            msg = new MsgMagicEffect
                            {
                                CellX = (ushort)pos.X,
                                CellY = (ushort)pos.Y,
                                Identity = m_pOwner.Identity,
                                SkillIdentity = m_pMagic.Type,
                                SkillLevel = m_pMagic.Level
                            };
                            msg.AppendTarget(obj.Identity, (uint)nPower, true, (byte) special, (uint) GetElementPower(obj));
                            targetNum = 0;
                            nCount++;
                        }

                        CheckCrime(obj);

                        nPowerSum += nPower;

                        var pNpc = obj as DynamicNpc;
                        int nLifeLost = (int)Math.Min(nPower, obj.Life);

                        //obj.AddAttribute(ClientUpdateType.HITPOINTS, -1 * nLifeLost, true);
                        //obj.BeAttack(HitByMagic(), m_pOwner, nLifeLost, true);

                        if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                        {
                            if (pNpc.IsCtfFlag())
                                (m_pOwner as Character).AwardCtfScore(pNpc, nLifeLost);
                            else
                                (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);
                        }

                        obj.BeAttack(HitByMagic(), m_pOwner, nPower, true);

                        if (obj.IsMonster()
                            || (pNpc != null && pNpc.IsGoal() && pNpc.Level < m_pOwner.Level))
                        {
                            nExp += m_pOwner.AdjustExperience(obj, nLifeLost, false);

                            if (!obj.IsAlive)
                            {
                                int nBonusExp = (int)(obj.MaxLife * (5 / 100));

                                m_pOwner.BattleSystem.OtherMemberAwardExp(obj, nExp);

                                nExp += m_pOwner.AdjustExperience(obj, nBonusExp, true);
                            }
                        }

                        if (!obj.IsAlive)
                            m_pOwner.Kill(obj, GetDieMode());
                    }
                }
                m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);

                AwardExp(0, nExp, 0);
                m_nMagicState = MagicState.MAGICSTATE_NONE;
            }
            else
            {
                int status = (int)m_pMagic.Status;
                int power = m_pMagic.Power;
                int step = (int)m_pMagic.StepSecs;
                int times = (int)m_pMagic.ActiveTimes;

                AbortMagic(true);
                m_pSetTargetLocked.Clear();

                if (!m_pOwner.IsAlive)
                    return false;

                if (m_pOwner.IsWing())
                    return false;

                m_pOwner.AttachStatus(m_pOwner, status, power, step, times, 0);
            }

            return true;
        }

        #endregion

        #region Sort 29 - Activate Switch

        private bool CheckActivateSwitch()
        {
            if (m_idTarget == 0) return false;

            IRole pRoleTarget = m_pOwner.BattleSystem.FindRole(m_idTarget);
            if (!(pRoleTarget is Character))
                return false;

            Character pTarget = pRoleTarget as Character;

            return pTarget.CheckScapegoat(m_pOwner);
        }

        private bool ProcessActivateSwitch()
        {
            if (m_idTarget == 0) return false;

            IRole pRoleTarget = m_pOwner.BattleSystem.FindRole(m_idTarget);
            if (!pRoleTarget.IsAlive)
                return false;

            InteractionEffect pSpecial = InteractionEffect.NONE;
            int nDamage = m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, pRoleTarget, ref pSpecial, 0, false);
            int nTotalExp = 0;

            var pMsg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            pMsg.AppendTarget(pRoleTarget.Identity, (uint)nDamage, true, (byte) pSpecial, 0);
            m_pOwner.Map.SendToRange(pMsg, pRoleTarget.MapX, pRoleTarget.MapY);

            if (nDamage > 0)
            {
                int nLifeLost = (int)Math.Min(pRoleTarget.Life, nDamage);
                //pRoleTarget.AddAttribute(ClientUpdateType.HITPOINTS, -1 * nLifeLost, true);
                pRoleTarget.BeAttack(HitByMagic(), m_pOwner, nDamage, true);

                nTotalExp += nLifeLost;
            }

            if (nTotalExp > 0)
            {
                AwardExpOfLife(pRoleTarget, nTotalExp);
            }

            if (!pRoleTarget.IsAlive)
                m_pOwner.Kill(pRoleTarget, GetDieMode());

            return true;
        }

        #endregion

        #region Sort 30 - Spook

        private bool ProcessSpook()
        {
            if (m_pOwner == null)
                return false;

            uint dwSteedPlus = 12;
            uint dwSteedProgress = uint.MaxValue;

            Character pRoleUser = null;
            if (m_pOwner is Character)
            {
                pRoleUser = m_pOwner as Character;
                Item steed = null;
                if (pRoleUser.Equipment.Items.TryGetValue(ItemPosition.STEED, out steed))
                {
                    dwSteedPlus = steed.Plus;
                    dwSteedProgress = steed.CompositionProgress;
                }
                else
                {
                    dwSteedPlus = 0;
                    dwSteedProgress = 0;
                }
            }

            MsgMagicEffect pMsg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                CellX = m_pOwner.MapX,
                CellY = m_pOwner.MapY,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };

            IRole pTarget = m_pOwner.BattleSystem.FindRole(m_idTarget);
            if (pTarget != null)
            {
                if (pTarget.IsAlive && (pTarget is Character))
                {
                    Character pTargetUser = pTarget as Character;
                    Item steed = null;

                    if (pTargetUser.QueryStatus(FlagInt.RIDING) != null)
                    {
                        if (pTargetUser.Equipment.Items.TryGetValue(ItemPosition.STEED, out steed))
                        {
                            if (steed.Plus <= dwSteedPlus)
                            {
                                if (steed.Plus < dwSteedPlus || steed.CompositionProgress <= dwSteedProgress)
                                {
                                    pMsg.AppendTarget(pTargetUser.Identity, 1, true, 0, 0);
                                }
                            } 
                        }
                    }
                }
            }
            if (pTarget != null)
                pTarget.DetachStatus(Effect0.RIDING);
            m_pOwner.Map.SendToRange(pMsg, m_pOwner.MapX, m_pOwner.MapY);
            return true;
        }

        #endregion

        #region Sort 31 - WarCry

        private bool ProcessWarCry()
        {
            if (m_pOwner == null)
                return false;

            uint dwSteedPlus = 12;
            uint dwSteedProgress = uint.MaxValue;

            Character pRoleUser = null;
            if (m_pOwner is Character)
            {
                pRoleUser = m_pOwner as Character;
                Item steed = null;
                if (pRoleUser.Equipment.Items.TryGetValue(ItemPosition.STEED, out steed))
                {
                    dwSteedPlus = steed.Plus;
                    dwSteedProgress = steed.CompositionProgress;
                }
                else
                {
                    dwSteedPlus = 0;
                    dwSteedProgress = 0;
                }
            }

            CollectTargetSet_Bomb(ref m_pPos, 0, (int) m_pMagic.Range);

            MsgMagicEffect pMsg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                CellX = m_pOwner.MapX,
                CellY = m_pOwner.MapY,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };

            List<IRole> pRemove = new List<IRole>();
            foreach (var pTarget in m_pSetTargetLocked)
            {
                if (!pTarget.IsAlive || !(pTarget is Character))
                {
                    pRemove.Add(pTarget);
                    continue;
                }

                Character pTargetUser = pTarget as Character;
                Item steed = null;

                if (pTargetUser.QueryStatus(FlagInt.RIDING) == null)
                {
                    pRemove.Add(pTarget);
                    continue;
                }

                if (pTargetUser.Equipment.Items.TryGetValue(ItemPosition.STEED, out steed))
                {
                    if (steed.Plus > dwSteedPlus)
                    {
                        pRemove.Add(pTarget);
                        continue;
                    }
                    if (steed.Plus == dwSteedPlus && steed.CompositionProgress > dwSteedProgress)
                    {
                        pRemove.Add(pTarget);
                        continue;
                    }
                }
                else
                {
                    pRemove.Add(pTarget);
                }

                if (pMsg.TargetCount >= _MAX_TARGET_NUM)
                {
                    m_pOwner.Map.SendToRange(pMsg, m_pOwner.MapX, m_pOwner.MapY);
                    pMsg = new MsgMagicEffect
                    {
                        Identity = m_pOwner.Identity,
                        CellX = m_pOwner.MapX,
                        CellY = m_pOwner.MapY,
                        SkillIdentity = m_pMagic.Type,
                        SkillLevel = m_pMagic.Level
                    };
                }
                pMsg.AppendTarget(pTargetUser.Identity, 1, true, 0, 0);
                pTargetUser.DetachStatus(Effect0.RIDING);
            }
            m_pOwner.Map.SendToRange(pMsg, m_pOwner.MapX, m_pOwner.MapY);
            return true;
        }

        #endregion

        #region Sort 32 - Mount
        private bool ProcessMount()
        {
            if (m_pOwner == null)
                return false;

            if (!(m_pOwner is Character))
            {
                ServerKernel.Log.SaveLog(string.Format("ERROR: [{0}] {1} tried to use ProcessMount().", m_pOwner.Identity, m_pOwner.Name), false);
                return false;
            }

            Character pUser = m_pOwner as Character;

            if (pUser.Map.IsPrisionMap() || ServerKernel.ArenaQualifier.IsInsideMatch(pUser.Identity))
                return false;

            Item mount = null;
            if (!pUser.Equipment.Items.TryGetValue(ItemPosition.STEED, out mount))
                return false;

            if (pUser.QueryStatus(FlagInt.FLY) != null)
            {
                return false;
            }

            if (pUser.QueryStatus(FlagInt.RIDING) != null)
            {
                pUser.DetachStatus(FlagInt.RIDING);
                return true;
            }

            if (pUser.Map.Identity == 1039 || (m_pOwner.Map.Identity == 1036 && mount.Plus < 6))
                return false;

            pUser.Vigor = pUser.MaxVigor;
            pUser.AttachStatus(m_pOwner, FlagInt.RIDING, 0, (int)m_pMagic.StepSecs, 0, 0);
            return true;
        }
        #endregion

        #region Sort 34 - Attach Area Status

        private bool ProcessAttachAreaStatus()
        {
            if (m_pMagic == null) return false;
            var setRole = new Dictionary<uint, IRole>();
            var setPower = new Dictionary<uint, int>();

            var pos = m_pPos;

            int nExp = 0;
            int nPowerSum = 0;

            CollectTargetSet_Bomb(ref pos, 0, (int)m_pMagic.Range + 1);

            var msg = new MsgMagicEffect
            {
                CellX = (ushort)pos.X,
                CellY = (ushort)pos.Y,
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };

            int nPower = GetPower();
            int nSecs = (int)m_pMagic.StepSecs;
            int nTimes = (int)m_pMagic.ActiveTimes;
            int nStatus = (int)m_pMagic.Status;

            if (nPower < 0)
            {
                ServerKernel.Log.SaveLog(string.Format("ERROR: magic type [{0}] status [{1}] invalid power", m_pMagic.Type, nStatus), false);
                return false;
            }

            int targetNum = 0;
            foreach (var obj in m_pSetTargetLocked)
            {
                if (obj.Identity == m_pOwner.Identity)
                    continue;

                if (!obj.IsAlive && m_pMagic.Target != 64)
                    continue;

                if (m_pMagic.Target == 64)
                {
                    if (obj.IsAlive || !(obj is Character))
                        continue;

                    if (m_pMagic.Status == FlagInt.SHACKLED && obj.QueryStatus(FlagInt.SHACKLED) != null && m_pOwner is Character)
                    {
                        (m_pOwner as Character).Send(string.Format(ServerString.STR_TARGET_ALREADY_SHACKED));
                        continue;
                    }
                }

                msg.AppendTarget(obj.Identity, (uint) nPower, nPower != 0, 0, 0);

                CheckCrime(obj);

                obj.AttachStatus(obj, nStatus, nPower, nSecs, nTimes, 0);
            }

            m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);
            
            AwardExp(0, nExp, nExp);
            return true;
        }

        #endregion

        #region Sort 35 - Remote Bomb

        private bool ProcessRemoteBomb()
        {
            if (m_pMagic == null) return false;
            var setRole = new Dictionary<uint, IRole>();
            var setPower = new Dictionary<uint, int>();
            var setRemove = new List<uint>();

            var pos = m_pPos;

            int nExp = 0;
            int nPowerSum = 0;

            CollectTargetSet_Bomb(ref pos, 0, (int)m_pMagic.Range + 1);

            var msg = new MsgMagicEffect
            {
                CellX = (ushort)pos.X,
                CellY = (ushort)pos.Y,
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };

            int targetNum = 0;
            foreach (var obj in m_pSetTargetLocked)
            {
                if (obj.Identity != m_pOwner.Identity && obj.IsAttackable(m_pOwner) && !IsImmunity(obj))
                {
                    var special = InteractionEffect.NONE;
                    int nPower = m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, obj, ref special);

                    if (obj is Character)
                    {
                        if ((obj as Character).CheckScapegoat(m_pOwner))
                        {
                            nPower = 0;
                        }
                    }

                    setPower.Add(obj.Identity, nPower);

                    if (targetNum < _MAX_TARGET_NUM)
                    {
                        msg.AppendTarget(obj.Identity, (uint)nPower, true, (uint)special, (uint)GetElementPower(obj));
                    }
                    else
                    {
                        m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);
                        msg = new MsgMagicEffect
                        {
                            CellX = (ushort)pos.X,
                            CellY = (ushort)pos.Y,
                            Identity = m_pOwner.Identity,
                            SkillIdentity = m_pMagic.Type,
                            SkillLevel = m_pMagic.Level
                        };
                        msg.AppendTarget(obj.Identity, (uint)nPower, true, (uint)special, (uint)GetElementPower(obj));
                        targetNum = 0;
                    }

                    setRole.Add(obj.Identity, obj);

                    CheckCrime(obj);
                }
                else
                    setRemove.Add(obj.Identity);
            }

            foreach (var rem in setRemove)
                setRole.Remove(rem);

            m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);

            foreach (var obj in setRole.Values)
            {
                if (!obj.IsAttackable(m_pOwner))
                    continue;

                nPowerSum += setPower[obj.Identity];

                var pNpc = obj as DynamicNpc;
                int nLifeLost = (int)Math.Min(obj.Life, setPower[obj.Identity]);

                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                    (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);

                obj.BeAttack(HitByMagic(), m_pOwner, setPower[obj.Identity], true);

                if (obj.IsMonster()
                    || (pNpc != null && pNpc.IsGoal() && pNpc.Level < m_pOwner.Level))
                {
                    nExp += m_pOwner.AdjustExperience(obj, nLifeLost, false);

                    if (!obj.IsAlive)
                    {
                        int nBonusExp = (int)(obj.MaxLife * (5 / 100));

                        m_pOwner.BattleSystem.OtherMemberAwardExp(obj, nExp);

                        nExp += m_pOwner.AdjustExperience(obj, nBonusExp, true);
                    }
                }

                if (!obj.IsAlive)
                    m_pOwner.Kill(obj, GetDieMode());
            }

            AwardExp(0, nExp, nExp);
            return true;
        }

        #endregion

        #region Sort 36 - Unknown type

        private bool Process36()
        {
            return true;
        }

        #endregion

        #region Sort 38 - Knockback

        private bool ProcessKnockback()
        {
            if (m_pMagic == null || m_pOwner == null)
                return false;

            var setPoint = new List<Point>();
            m_pSetTargetLocked.Clear();

            // search the target
            bool bSuc = true;

            var pTarget = m_pOwner.BattleSystem.FindRole(m_idTarget);
            if (pTarget == null
                || !Calculations.InScreen(m_pOwner.MapX, m_pOwner.MapY, pTarget.MapX, pTarget.MapY)
                || (!pTarget.IsAlive && !pTarget.IsAttackable(m_pOwner)))
            {
                return false;
            }

            if (IsImmunity(pTarget))
                return false;

            int nPower = 0;
            uint idTarget = 0;

            DynamicNpc pNpc = null;
            InteractionEffect pSpecial = InteractionEffect.NONE;
            pNpc = pTarget as DynamicNpc;

            FacingDirection nDir =
                (FacingDirection)
                    (Calculations.GetDirection(m_pOwner.MapX, m_pOwner.MapY, pTarget.MapX, pTarget.MapY)%8);

            Calculations.DDALine(m_pOwner.MapX, m_pOwner.MapY, m_pPos.X, m_pPos.Y, (int)m_pMagic.Range, ref setPoint);

            ushort nTargetX = pTarget.MapX;
            ushort nTargetY = pTarget.MapY;

            foreach (var point in setPoint)
            {
                try
                {
                    Tile pPos = m_pOwner.Map[point.X, point.Y];
                    if (pPos.Access < TileType.NPC
                        || pPos.Elevation - m_pOwner.Map[m_pOwner.MapX, m_pOwner.MapY].Elevation > 26)
                    {
                        break;
                    }
                    nTargetX = (ushort) point.X;
                    nTargetY = (ushort) point.Y;
                }
                catch
                {
                    continue;
                }
            }

            if (HitByWeapon())
            {
                nPower += m_pOwner.BattleSystem.CalcAttackPower(m_pOwner, pTarget, ref pSpecial);

                if (pNpc != null)
                    nPower = Calculations.MulDiv(nPower, 80, 100);
                else
                    nPower = Calculations.AdjustDataEx(nPower, GetPower(), 0);
            }

            if (!m_pOwner.Map.IsTrainingMap() && m_pOwner is Character)
                (m_pOwner as Character).AddEquipmentDurability(ItemPosition.LEFT_HAND, -1*3);

            MsgMagicEffect pMsg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level,
                CellX = nTargetX,
                CellY = nTargetY
            };
            pMsg.AppendTarget(pTarget.Identity, (uint) nPower, true, (uint) pSpecial, 0);
            m_pOwner.Screen.Send(pMsg, true);

            if (m_pOwner is Character && (m_pOwner as Character).IsPm)
                m_pOwner.Send(new MsgTalk(string.Format("bump move:({0},{1})->({2},{3})",
                    m_pOwner.MapX, m_pOwner.MapY, nTargetX, nTargetY), ChatTone.TALK));

            if (pTarget is Character)
            {
                (pTarget as Character).ProcessOnMove();
            }

            CheckCrime(pTarget);

            if (nPower > 0)
            {
                int nLifeLost = (int) Math.Min(nPower, pTarget.Life);

                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                {
                    if (pNpc.IsCtfFlag())
                        (m_pOwner as Character).AwardCtfScore(pNpc, nLifeLost);
                    else
                        (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);
                }

                pTarget.BeAttack(HitByMagic(), m_pOwner, nLifeLost, true);

                if (pTarget.IsMonster() || pNpc != null && pNpc.IsGoal() && m_pOwner.Level >= pNpc.Level)
                {
                    int nExp = m_pOwner.AdjustExperience(pTarget, nLifeLost, false);

                    if (!pTarget.IsAlive)
                    {
                        int nBonusExp = (int) (pTarget.MaxLife*0.05f);
                        m_pOwner.BattleSystem.OtherMemberAwardExp(pTarget, nBonusExp);

                        nExp += m_pOwner.AdjustExperience(pTarget, nBonusExp, true);
                    }

                    AwardExp(0, nExp, nExp, m_pMagic);
                }

                if (!pTarget.IsAlive)
                    m_pOwner.Kill(pTarget, GetDieMode());
            }

            return true;
        }

        #endregion

        #region Sort 40 - Dash Whirl

        private bool ProcessDashWhirl()
        {
            if (m_pMagic == null || m_pOwner == null)
                return false;

            var setPoint = new List<Point>();
            var setTarget = new Dictionary<uint, IRole>();
            var setPower = new Dictionary<uint, uint>();
            var setRemove = new List<IRole>();
            m_pSetTargetLocked.Clear();

            Calculations.DDALine(m_pOwner.MapX, m_pOwner.MapY, m_pPos.X, m_pPos.Y,
                (int)Calculations.GetDistance(m_pOwner.MapX, m_pOwner.MapY, (ushort)m_pPos.X, (ushort)m_pPos.Y), ref setPoint);

            ushort nTargetX = m_pOwner.MapX;
            ushort nTargetY = m_pOwner.MapY;

            var roles = m_pOwner.Screen.GetAroundRoles;
            int nTiles = 0;
            foreach (var point in setPoint)
            {
                try
                {
                    Tile pPos = m_pOwner.Map[point.X, point.Y];
                    if (pPos.Access < TileType.NPC
                        || pPos.Elevation - m_pOwner.Map[m_pOwner.MapX, m_pOwner.MapY].Elevation > 26)
                    {
                        break;
                    }

                    nTargetX = (ushort)point.X;
                    nTargetY = (ushort)point.Y;
                    ushort testX = nTargetX;
                    ushort testY = nTargetY;

                    var pList = roles.Where(x => Calculations.GetDistance(testX, testY, x.MapX, x.MapY) < m_pMagic.Range);
                    foreach (var role in pList)
                        if (!setTarget.ContainsKey(role.Identity))
                            setTarget.Add(role.Identity, role);
                    nTiles++;
                }
                catch
                {
                    continue;
                }
            }

            var pMsg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level,
                CellX = nTargetX,
                CellY = nTargetY
            };

            int nTargetNum = 0, nCount = 0;
            foreach (var pTarget in setTarget.Values)
            {
                if (pTarget.IsAttackable(m_pOwner) && !IsImmunity(pTarget))
                {
                    var special = InteractionEffect.NONE;
                    int nPower = m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, pTarget, ref special);

                    if (pTarget is Character)
                    {
                        if ((pTarget as Character).CheckScapegoat(m_pOwner))
                        {
                            nPower = 0;
                        }
                    }

                    if (++nTargetNum >= 50)
                    {
                        m_pOwner.Map.SendToRange(pMsg, (ushort)m_pOwner.MapX, (ushort)m_pOwner.MapY);
                        pMsg = new MsgMagicEffect
                        {
                            CellX = nTargetX,
                            CellY = nTargetY,
                            Identity = m_pOwner.Identity,
                            SkillIdentity = m_pMagic.Type,
                            SkillLevel = m_pMagic.Level
                        };
                        nTargetNum = 0;
                    }
                    pMsg.AppendTarget(pTarget.Identity, (uint)nPower, true, (byte)special, (uint)GetElementPower(pTarget));
                    setPower.Add(pTarget.Identity, (uint)nPower);
                    CheckCrime(pTarget);
                    nCount++;
                }
                else
                    setRemove.Add(pTarget);
            }

            foreach (var role in setRemove)
                if (setTarget.ContainsKey(role.Identity))
                    setTarget.Remove(role.Identity);

            MsgInteract pInteract = new MsgInteract
            {
                Timestamp = Time.Now,
                EntityIdentity = m_pOwner.Identity,
                TargetIdentity = m_pOwner.Identity,
                CellX = nTargetX,
                CellY = nTargetY,
                Action = (InteractionType) 53, 
                MagicType = m_pMagic.Type,
                MagicLevel = m_pMagic.Level
            };
            m_pOwner.Map.SendToRange(pInteract, m_pOwner.MapX, m_pOwner.MapY);
            m_pOwner.Map.SendToRange(pMsg, m_pOwner.MapX, m_pOwner.MapY);

            if (m_pOwner is Character && (m_pOwner as Character).IsPm)
                m_pOwner.Send(new MsgTalk(string.Format("bump move:({0},{1})->({2},{3}) distance({4})",
                    m_pOwner.MapX, m_pOwner.MapY, nTargetX, nTargetY, Calculations.GetDistance(m_pOwner.MapX, m_pOwner.MapY, nTargetX, nTargetY)), ChatTone.TALK));

            m_pOwner.MapX = nTargetX;
            m_pOwner.MapY = nTargetY;

            m_pOwner.Screen.SendMovement(pMsg);

            uint nPowerSum = 0u;
            int nExp = 0;
            foreach (var pTarget in setTarget.Values)
            {
                if (!pTarget.IsAttackable(m_pOwner))
                    continue;

                nPowerSum += setPower[pTarget.Identity];

                DynamicNpc pNpc = null;
                if (pTarget is DynamicNpc)
                    pNpc = pTarget as DynamicNpc;

                int nLifeLost = (int)Math.Min(pTarget.Life, setPower[pTarget.Identity]);

                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                {
                    if (pNpc.IsCtfFlag())
                        (m_pOwner as Character).AwardCtfScore(pNpc, nLifeLost);
                    else
                        (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);
                }

                pTarget.BeAttack(HitByMagic(), m_pOwner, nLifeLost, true);

                if (pTarget.IsMonster() || pNpc != null && pNpc.IsGoal() && pTarget.Level < m_pOwner.Level)
                {
                    nExp += m_pOwner.AdjustExperience(pTarget, nLifeLost, false);
                    if (!pTarget.IsAlive)
                    {
                        int nBonusExp = (int)(pTarget.MaxLife * .05f);

                        m_pOwner.BattleSystem.OtherMemberAwardExp(pTarget, nBonusExp);

                        nExp += m_pOwner.AdjustExperience(m_pOwner, nBonusExp, true);
                    }
                }

                if (ServerKernel.ScorePkEvent.IsParticipating(m_pOwner.Identity))
                {
                    ServerKernel.ScorePkEvent.AlterPoints(m_pOwner.Identity, 5);
                }

                if (!pTarget.IsAlive)
                    m_pOwner.Kill(pTarget, (ulong)GetDieMode());
            }
            AwardExp(nExp, nExp, true);
            return true;
        }

        #endregion

        #region Sort 46 - Self Detach

        private bool ProcessSelfDetach()
        {
            if (m_pMagic == null)
                return false;

            m_pSetTargetLocked.Clear();

            var pRole = m_pOwner as Character;
            if (pRole == null) return false;

            if (!pRole.IsAlive)
                return false;

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            msg.AppendTarget(pRole.Identity, 0, false, 0, 0);
            pRole.Map.SendToRange(msg, pRole.MapX, pRole.MapY);

            for (int i = 0; i < m_pTranquilityDetach.Length; i++)
                pRole.DetachStatus(m_pTranquilityDetach[i]);

            return true;
        }

        #endregion

        #region Sort 47 - Detach Bad Status

        private int[] m_pTranquilityDetach =
        {
            FlagInt.DAZED,
            FlagInt.HUGE_DAZED,
            FlagInt.POISONED,
            FlagInt.POISON_STAR,
            FlagInt.NO_POTION,
            FlagInt.CONFUSED,
            FlagInt.TOXIC_FOG,
            FlagInt.SHACKLED
        };

        private bool ProcessDetachBadStatus()
        {
            if (m_pMagic == null)
                return false;

            m_pSetTargetLocked.Clear();

            var pRole = m_pOwner.BattleSystem.FindRole(m_idTarget) as IRole;
            if (pRole == null) return false;
            
            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            msg.AppendTarget(pRole.Identity, 0, false, 0, 0);
            pRole.Map.SendToRange(msg, pRole.MapX, pRole.MapY);

            for (int i = 0; i < m_pTranquilityDetach.Length; i++)
                pRole.DetachStatus(m_pTranquilityDetach[i]);

            return true;
        }

        #endregion

        #region Sort 48 - Close Line

        private bool ProcessCloseLine()
        {
            if (m_pMagic == null)
                return false;

            var setTarget = new Dictionary<uint, IRole>();
            var setPower = new Dictionary<uint, int>();
            var setPoint = new List<Point>();

            int nExp = 0;
            int nPowerSum = 0;

            m_pSetTargetLocked.Clear();
            setTarget.Clear();
            setPower.Clear();
            setPoint.Clear();

            if (m_pPos.X == m_pOwner.MapX && m_pPos.Y == m_pOwner.MapY)
                return false;

            var pos = new Point(m_pOwner.MapX, m_pOwner.MapY);

            Calculations.DDALine(pos.X, pos.Y, m_pPos.X, m_pPos.Y, (int)m_pMagic.Distance, ref setPoint);

            foreach (var point in setPoint)
            {
                var pTarget = m_pOwner.Map.QueryRole((ushort) point.X, (ushort) point.Y) as IRole;
                if (pTarget == null
                    || !pTarget.IsAttackable(m_pOwner)
                    || IsImmunity(pTarget))
                    continue;

                if (pTarget is Character || pTarget.IsMonster())
                {
                    try
                    {
                        if (m_pOwner.Map[point.X, point.Y].Elevation > 26)
                            continue;
                    }
                    catch { continue; }
                }

                if (pTarget.Identity == m_pOwner.Identity || !pTarget.IsAttackable(m_pOwner)) continue;

                if (!m_pOwner.IsWing() && !pTarget.IsWing())
                    setTarget.Add(pTarget.Identity, pTarget);
            }

            var clickTarget = m_pOwner.BattleSystem.FindRole(m_idTarget);
            if (clickTarget != null && !setTarget.ContainsKey(clickTarget.Identity) && !IsImmunity(clickTarget))
                setTarget.Add(clickTarget.Identity, clickTarget);

            if (setTarget.Count <= 0) {
                return false;
            }

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level,
                CellX = (ushort)m_pPos.X,
                CellY = (ushort)m_pPos.Y
            };

            foreach (var pTarget in setTarget.Values)
            {
                if (pTarget.IsAttackable(m_pOwner))
                {
                    var special = InteractionEffect.NONE;
                    int nPower = m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, pTarget, ref special);
                    msg.AppendTarget(pTarget.Identity, (uint)nPower, true, (byte)special, 0);
                    setPower.Add(pTarget.Identity, nPower);
                    CheckCrime(pTarget);
                }
                else
                    setPower.Add(pTarget.Identity, 0);
            }

            m_pOwner.Map.SendToRange(msg, m_pOwner.MapX, m_pOwner.MapY);

            foreach (var pTarget in setTarget.Values)
            {
                if (!pTarget.IsAttackable(m_pOwner))
                    continue;

                nPowerSum += setPower[pTarget.Identity];

                DynamicNpc pNpc = null;
                if (pTarget is DynamicNpc)
                    pNpc = pTarget as DynamicNpc;

                int nLifeLost = (int) Math.Min(pTarget.Life, setPower[pTarget.Identity]);

                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                {
                    if (pNpc.IsCtfFlag())
                        (m_pOwner as Character).AwardCtfScore(pNpc, nLifeLost);
                    else
                        (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);
                }

                pTarget.BeAttack(HitByMagic(), m_pOwner, nLifeLost, true);

                if (pTarget.IsMonster() || pNpc != null && pNpc.IsGoal() && pTarget.Level < m_pOwner.Level)
                {
                    nExp += m_pOwner.AdjustExperience(pTarget, nLifeLost, false);
                    if (!pTarget.IsAlive)
                    {
                        int nBonusExp = (int) (pTarget.MaxLife * (5 / 100));

                        m_pOwner.BattleSystem.OtherMemberAwardExp(pTarget, nExp);

                        nExp += m_pOwner.AdjustExperience(m_pOwner, nBonusExp, true);
                    }
                }

                if (!pTarget.IsAlive)
                    m_pOwner.Kill(pTarget, GetDieMode());
            }

            if (ServerKernel.ScorePkEvent.IsParticipating(m_pOwner.Identity))
            {
                ServerKernel.ScorePkEvent.AlterPoints(m_pOwner.Identity, 5);
            }


            if (HitByWeapon() && m_pOwner.Map.IsTrainingMap())
            {
                // TODO decrease durability
            }

            AwardExp(0, 0, nExp);
            return true;
        }

        #endregion

        #region Sort 50 - Compassion

        private int[] m_pCompassionDetach =
        {
            FlagInt.DAZED,
            FlagInt.HUGE_DAZED,
            FlagInt.POISONED,
            FlagInt.POISON_STAR,
            FlagInt.NO_POTION,
            FlagInt.CONFUSED,
            FlagInt.TOXIC_FOG
        };

        private bool ProcessDetachTeam()
        {
            if (!(m_pOwner is Character))
                return false;

            Character pCaster = m_pOwner as Character;

            List<Character> pTargetList = new List<Character>();
            if (pCaster.Team == null)
            {
                pTargetList.Add(pCaster);
            }
            else
            {
                pTargetList = new List<Character>(pCaster.Team.Members.Values.ToList());
            }

            MsgMagicEffect pMsg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                CellX = m_pOwner.MapX,
                CellY = m_pOwner.MapY,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            foreach (var member in pTargetList)
            {
                if (!member.IsAlive)
                    continue;
                for (int i = 0; i < m_pCompassionDetach.Length; i++)
                    member.DetachStatus(m_pCompassionDetach[i]);
                pMsg.AppendTarget(member.Identity, 0, false, 0, 0);
            }
            m_pOwner.Map.SendToRange(pMsg, m_pOwner.MapX, m_pOwner.MapY);
            return true;
        }

        #endregion

        #region Sort 51 - Team Flag

        private bool ProcessTeamFlag()
        {
            if (m_pOwner == null || m_pMagic == null) return false;

            m_pSetTargetLocked.Clear();

            int nPower = GetPower();
            int nSecs = (int)m_pMagic.StepSecs;
            int nTimes = (int)m_pMagic.ActiveTimes;
            int nStatus = (int)m_pMagic.Status;
            byte pLevel = (byte) m_pMagic.Level;

            if (nPower < 0)
            {
                ServerKernel.Log.SaveLog( string.Format("ERROR: magic type [{0}] status [{1}] invalid power", m_pMagic.Type, nStatus), false);
                return false;
            }

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            msg.AppendTarget(m_pOwner.Identity, 0, true, 0, 0);
            m_pOwner.Map.SendToRange(msg, m_pOwner.MapX, m_pOwner.MapY);

            uint dwAura = 0;

            switch (m_pMagic.Status)
            {
                case FlagInt.TYRANT_AURA: dwAura = (uint)AuraType.TYRANT_AURA; break;
                case FlagInt.FEND_AURA: dwAura = (uint)AuraType.FEND_AURA; break;
                case FlagInt.WATER_AURA: dwAura = (uint)AuraType.WATER_AURA; break;
                case FlagInt.FIRE_AURA: dwAura = (uint)AuraType.FIRE_AURA; break;
                case FlagInt.METAL_AURA: dwAura = (uint)AuraType.METAL_AURA; break;
                case FlagInt.WOOD_AURA: dwAura = (uint)AuraType.WOOD_AURA; break;
                case FlagInt.EARTH_AURA: dwAura = (uint)AuraType.EARTH_AURA; break;
                case FlagInt.SUPER_SHIELD_HALO: dwAura = (uint)AuraType.MAGIC_DEFENDER; break;
            }

            var pMsg = new MsgAura
            {
                Action = IconAction.ADD,
                EntityIdentity = m_pOwner.Identity,
                Level = m_pMagic.Level,
                Power1 = (uint)m_pMagic.Power,
                Type = dwAura,
                Time = Time.Now
            };
            m_pOwner.Send(pMsg);

            Character pSender = m_pOwner as Character;
            if (pSender != null)
            {
                for (int i = 98; i <= 111; i++)
                {
                    var sts = pSender.QueryStatus(i);
                    if (sts != null && sts.IsUserCast)
                        pSender.DetachStatus(i);
                }

                pSender.AttachStatus(pSender, nStatus, nPower, nSecs, nTimes, pLevel, pSender.Identity);

                if (pSender.Team != null)
                {
                    pSender.Team.Send(string.Format("{0} has used {1} at {2}.", m_pOwner.Name,
                        m_pMagic.Name, m_pOwner.Map.Name));
                }
            }

            int nExp = 3;
            if (m_pOwner.Map.IsTrainingMap())
                nExp = 1;
            AwardExp(0, 0, nExp);
            return true;
        }

        #endregion

        #region Sort 52 - Increase Block

        private bool ProcessIncreaseBlock()
        {
            if (m_pMagic == null) return false;
            m_pSetTargetLocked.Clear();

            var pRoleUser = m_pOwner.BattleSystem.FindRole(m_idTarget);

            if (pRoleUser == null && m_idTarget == m_pOwner.Identity)
                pRoleUser = m_pOwner;
            if (pRoleUser == null)
                return false;

            int nPower = GetPower();
            int nSecs = (int)m_pMagic.StepSecs;
            int nTimes = (int)m_pMagic.ActiveTimes;
            int nStatus = (int)m_pMagic.Status;

            if (nPower < 0)
            {
                ServerKernel.Log.SaveLog(string.Format("ERROR: magic type [{0}] status [{1}] invalid power", m_pMagic.Type, nStatus), false);
                return false;
            }

            uint nDmg = 1;

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            msg.AppendTarget(pRoleUser.Identity, nDmg, nDmg != 0, 0, 0);
            m_pOwner.Map.SendToRange(msg, m_pOwner.MapX, m_pOwner.MapY);

            CheckCrime(pRoleUser);

            pRoleUser.AttachStatus(pRoleUser, nStatus, nPower, nSecs, nTimes, (byte) GetLevel());
            return true;
        }

        #endregion

        #region Sort 53 - Oblivion

        private bool ProcessOblivion()
        {
            if (m_pMagic == null || !(m_pOwner is Character)) return false;
            m_pSetTargetLocked.Clear();

            var pRoleUser = m_pOwner as Character;

            if (!pRoleUser.IsAlive)
                return false;

            int nPower = GetPower();
            int nSecs = (int)m_pMagic.StepSecs;
            int nTimes = (int)m_pMagic.ActiveTimes;
            int nStatus = (int)m_pMagic.Status;

            if (nPower < 0)
            {
                ServerKernel.Log.SaveLog(string.Format("ERROR: magic type [{0}] status [{1}] invalid power", m_pMagic.Type, nStatus), false);
                return false;
            }

            pRoleUser.ResetOblivion();

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            msg.AppendTarget(pRoleUser.Identity, 1, true, 0, 0);
            m_pOwner.Map.SendToRange(msg, m_pOwner.MapX, m_pOwner.MapY);

            CheckCrime(pRoleUser);

            pRoleUser.AttachStatus(pRoleUser, nStatus, nPower, nSecs, nTimes, 0);
            return true;
        }

        #endregion

        #region Sort 54 - Stun Bomb Magic
        private bool ProcessStunBomb()
        {
            if (m_pMagic == null) return false;
            var setRemove = new List<IRole>();
            var setPower = new Dictionary<uint, int>();

            var pos = new Point(m_pOwner.MapX, m_pOwner.MapY);

            int nExp = 0;
            int nPowerSum = 0;

            CollectTargetSet_Bomb(ref pos, 0, (int)m_pMagic.Range);

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                CellX = (ushort)m_pMagic.ActiveTimes,
                CellY = 0,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };

            int targetNum = 0;
            foreach (var obj in m_pSetTargetLocked)
            {
                if (obj.Identity != m_pOwner.Identity 
                    && obj.IsAttackable(m_pOwner)
                    && !IsImmunity(obj))
                {
                    if (msg.TargetCount >= _MAX_TARGET_NUM)
                    {
                        m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);
                        msg = new MsgMagicEffect
                        {
                            Identity = m_pOwner.Identity,
                            CellX = (ushort)m_pMagic.ActiveTimes,
                            CellY = 0,
                            SkillIdentity = m_pMagic.Type,
                            SkillLevel = m_pMagic.Level
                        };
                    }
                    var special = InteractionEffect.NONE;
                    int nPower = m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, obj, ref special);
                    setPower.Add(obj.Identity, nPower);
                    msg.AppendTarget(obj.Identity, (uint)nPower, true, (byte)special, 0);
                    CheckCrime(obj);

                    if (obj.IsWing())
                        obj.DetachStatus(FlagInt.FLY);
                }
                else
                {
                    setRemove.Add(obj);
                }
            }

            foreach (var remove in setRemove)
                m_pSetTargetLocked.Remove(remove);

            m_pOwner.Map.SendToRange(msg, (ushort)pos.X, (ushort)pos.Y);

            foreach (var obj in m_pSetTargetLocked)
            {
                if (!obj.IsAttackable(m_pOwner))
                    continue;

                nPowerSum += setPower[obj.Identity];

                var pNpc = obj as DynamicNpc;
                int nLifeLost = (int) Math.Min(setPower[obj.Identity], obj.Life);

                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                {
                    if (pNpc.IsCtfFlag())
                        (m_pOwner as Character).AwardCtfScore(pNpc, nLifeLost);
                    else
                        (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);
                }

                obj.BeAttack(HitByMagic(), m_pOwner, setPower[obj.Identity], true);

                if (obj.IsMonster()
                    || (pNpc != null && pNpc.IsGoal() && pNpc.Level < m_pOwner.Level))
                {
                    nExp += m_pOwner.AdjustExperience(obj, nLifeLost, false);

                    if (!obj.IsAlive)
                    {
                        int nBonusExp = (int) (obj.MaxLife * (5 / 100));

                        m_pOwner.BattleSystem.OtherMemberAwardExp(obj, nExp);
                        
                        nExp += m_pOwner.AdjustExperience(obj, nBonusExp, true);
                    }
                }

                if (!obj.IsAlive)
                    m_pOwner.Kill(obj, GetDieMode());
                //else if (obj is Character)
                //    (obj as Character).Freeze(250);

            }

            AwardExp(0, nExp, nExp);

            return true;
        }
        #endregion

        #region Sort 55 - Triple Attack
        private bool ProcessTripleAttack()
        {
            if (m_pMagic == null || m_pOwner == null)
                return false;

            m_pSetTargetLocked.Clear();

            var pTarget = m_pOwner.BattleSystem.FindRole(m_idTarget);
            if (pTarget == null
                || !Calculations.InScreen(m_pOwner.MapX, m_pOwner.MapY, pTarget.MapX, pTarget.MapY)
                || !pTarget.IsAttackable(m_pOwner))
                return false;

            if (m_pOwner.IsImmunity(pTarget))
                return false;

            int nTotalExp = 0;
            int nTotalDamage = 0;

            var msg = new MsgMagicEffect
            {
                Identity = m_pOwner.Identity,
                SkillIdentity = m_pMagic.Type,
                SkillLevel = m_pMagic.Level
            };
            var special = InteractionEffect.NONE;
            int nPower = 0;
            for (int i = 0; i < 3; i++)
            {
                if (!m_pOwner.BattleSystem.IsTargetDodged(m_pOwner, pTarget))
                    nPower = m_pOwner.BattleSystem.CalcPower(0, m_pOwner, pTarget, ref special, GetPower());

                nTotalDamage += nPower;
                msg.AppendTarget(pTarget.Identity, (uint)nPower, true, (byte)special, 0);
                special = InteractionEffect.NONE;
                nPower = 0;
            }
            m_pOwner.Map.SendToRange(msg, pTarget.MapX, pTarget.MapY);

            CheckCrime(pTarget);

            if (nTotalDamage > 0)
            {
                int nLifeLost = (int) Math.Min(pTarget.Life, nTotalDamage);

                //pTarget.AddAttribute(ClientUpdateType.HITPOINTS, -1 * nLifeLost, true);
                pTarget.BeAttack(HitByMagic(), m_pOwner, nLifeLost, true);

                var pNpc = pTarget as DynamicNpc;
                if (pNpc != null && pNpc.IsAwardScore() && m_pOwner is Character)
                {
                    if (pNpc.IsCtfFlag())
                        (m_pOwner as Character).AwardCtfScore(pNpc, nLifeLost);
                    else
                        (m_pOwner as Character).AwardSynWarScore(pNpc, nLifeLost);
                }

                if (!pTarget.IsAlive)
                {
                    int nBonusExp = (int)(pTarget.MaxLife * (5 / 100));

                    m_pOwner.BattleSystem.OtherMemberAwardExp(pTarget, nLifeLost);

                    nTotalExp += m_pOwner.AdjustExperience(pTarget, nBonusExp, true);
                }

                nTotalExp += nLifeLost;
            }
            
            if (!pTarget.IsAlive)
                m_pOwner.Kill(pTarget, (ulong)GetDieMode());

            int nSkillExp = m_pOwner.Map.IsTrainingMap() ? 1 : 3;
            AwardExp(0, nTotalExp, nSkillExp, m_pMagic);
            return true;
        }
        #endregion

        #endregion

        #region Collect Target Set

        private bool CollectTargetSet_Bomb(ref Point pos, int nLockType, int nRange)
        {
            if (m_pMagic == null) return false;

            int nSize = nRange * 2 + 1;
            int nBufSize = nSize * nSize;

            m_pSetTargetLocked.Clear();

            //int nDir = Owner.GetDir();
            if (m_pMagic.Ground == 1)
            {
                pos.X = m_pOwner.MapX;
                pos.Y = m_pOwner.MapY;
            }
            else if (pos.X > 0 && pos.Y > 0)
            {
                // bypass
            }
            else if (m_pMagic.Status == FlagInt.VORTEX)
            {
                pos.X = m_pOwner.MapX;
                pos.Y = m_pOwner.MapY;
            }
            else
            {
                IRole pTarget = (m_pOwner as IScreenObject).FindAroundRole(m_idTarget) as IRole;
                if (pTarget == null
                    || !pTarget.IsAttackable(m_pOwner))
                    return false;
                pos.X = pTarget.MapX;
                pos.Y = pTarget.MapY;
            }

            m_pSetTargetLocked = m_pOwner.Map.CollectMapThing(nRange, ref pos);

            if (m_pSetTargetLocked == null || m_pSetTargetLocked.Count <= 0)
                return false;

            foreach (var pRole in m_pSetTargetLocked)
            {
                if (!Calculations.IsInCircle(new Point(pRole.MapX, pRole.MapY), pos, nRange))
                    continue;

                if (!IsImmunity(pRole))
                {
                    if (pRole is DynamicNpc)
                        if (!pRole.IsAttackable(m_pOwner))
                            continue;

                    if (pRole.Identity == m_pOwner.Identity)
                        continue;

                    if ((nLockType == 1 && pRole.Identity == m_idTarget)
                        || nLockType == 2)
                    {
                        m_pSetTargetLocked.Add(pRole);
                    }
                }
            }
            return true;
        }

        #endregion

        public byte GetLevel()
        {
            return (byte) (m_pMagic == null ? 0 : m_pMagic.Level);
        }

        private int GetPower()
        {
            return m_pMagic == null ? 0 : m_pMagic.Power;
        }

        public int GetElementPower(IRole pTarget)
        {
            if (m_pMagic == null)
                return 0;

            int nDmg = 0;
            switch (m_pMagic.ElementType)
            {
                case ElementType.WATER:
                    {
                        nDmg = (int)(m_pMagic.Power * (1 - (pTarget.WaterResistance / 100f)));
                        break;
                    }
                case ElementType.FIRE:
                    {
                        nDmg = (int)(m_pMagic.Power * (1 - (pTarget.FireResistance / 100f)));
                        break;
                    }
                case ElementType.WOOD:
                    {
                        nDmg = (int)(m_pMagic.Power * (1 - (pTarget.WoodResistance / 100f)));
                        break;
                    }
                case ElementType.EARTH:
                    {
                        nDmg = (int)(m_pMagic.Power * (1 - (pTarget.EarthResistance / 100f)));
                        break;
                    }
                case ElementType.METAL:
                    {
                        nDmg = (int)(m_pMagic.Power * (1 - (pTarget.MetalResistance / 100f)));
                        break;
                    }
            }
            return nDmg;
        }

        public void ShowMiss()
        {
            if (m_pMagic == null)
                return;

            if (m_pMagic.Ground != 0)
                m_pOwner.Screen.Send(new MsgMagicEffect
                {
                    Identity = m_pOwner.Identity,
                    SkillIdentity = m_pMagic.Type,
                    SkillLevel = m_pMagic.Level,
                    CellX = m_pOwner.MapX,
                    CellY = m_pOwner.MapY
                }, true);
            else
                m_pOwner.Screen.Send(new MsgMagicEffect
                {
                    Identity = m_pOwner.Identity,
                    SkillIdentity = m_pMagic.Type,
                    SkillLevel = m_pMagic.Level,
                    TargetCount = m_idTarget
                }, true);
        }

        private int HitByMagic()
        {
            // 0 none, 1 normal, 2 xp
            if (m_pMagic == null) return 0;

            if (m_pMagic.WeaponHit == 0)
            {
                return m_pMagic.UseXp == 2 ? 2 : 1;
            }

            Item item = null;
            if (m_pOwner is Character)
            {
                var pRole = m_pOwner as Character;
                if (pRole.Equipment.Items.ContainsKey(ItemPosition.RIGHT_HAND) && m_pMagic.WeaponHit == 2 &&
                    pRole.Equipment.Items[ItemPosition.RIGHT_HAND].Itemtype.MagicAtk > 0)
                {
                    return m_pMagic.UseXp == 2 ? 2 : 1;
                }
            }

            return 0;
        }

        public bool IsWeaponMagic(ushort type)
        {
            return type >= 10000 && type < 10256;
        }

        public bool CheckCondition(Magic pData, uint idTarget, ref ushort x, ref ushort y)
        {
            if (!m_tDelay.IsTimeOut() && MagicSort.MAGICSORT_COLLIDE != pData.Sort) // check if user can already user another skill, 800ms is the min time
            {
                return false;
            }

            if (!pData.IsReady())
            {
                return false;
            }

            if (m_pOwner.Map.IsLineSkillMap()
                && pData.Sort != MagicSort.MAGICSORT_LINE)
                return false;
            
            if (!((pData.AutoActive & 1) == 1
                  || (pData.AutoActive & 4) == 4) && pData.Type != 6001)
            {
                if (!Calculations.ChanceCalc(pData.Percent))
                    return false;
            }

            if (m_pOwner.Map.QueryRegion(RegionType.REGION_PK_PROTECTED, m_pOwner.MapX, m_pOwner.MapY) && m_pOwner is Character)
            {
                if (pData.Ground > 0)
                {
                    if (pData.Crime > 0)
                    {
                        return false;
                    }
                }
                else
                {
                    IRole pTarget = m_pOwner.BattleSystem.FindRole(idTarget);
                    if (pTarget != null && pTarget is Character && pData.Crime > 0)
                        return false;
                }
            }

            if (!m_pOwner.Map.IsTrainingMap() && m_pOwner is Character)
            {
                if (m_pOwner.Mana < pData.UseMana)
                    return false;
                if (m_pOwner.Stamina < pData.UseStamina)
                    return false;

                if (pData.UseItem > 0)
                {
                    if (!m_pOwner.CheckWeaponSubType(pData.UseItem, pData.UseItemNum))
                        return false;
                }
            }

            if (pData.UseXp == 1)
            {
                IStatus pStatus = m_pOwner.QueryStatus(FlagInt.START_XP);
                if (pStatus == null && (pData.Status == FlagInt.VORTEX && m_pOwner.QueryStatus(FlagInt.VORTEX) == null))
                    return false;
            }

            if (pData.WeaponSubtype > 0 && m_pOwner is Character)
            {
                if (!m_pOwner.CheckWeaponSubType(pData.WeaponSubtype))
                    return false;
            }

            uint nSort = pData.Sort;
            if ((nSort == MagicSort.MAGICSORT_CALLTEAMMEMBER || nSort == MagicSort.MAGICSORT_RECORDTRANSSPELL)
                && m_pOwner.Map.IsChgMapDisable())
            {
                return false;
            }

            if (nSort == MagicSort.MAGICSORT_RECORDTRANSSPELL &&
                m_pOwner.Map.QueryRegion(RegionType.REGION_CITY, m_pOwner.MapX, m_pOwner.MapY))
            {
                return false;
            }

            if (m_pOwner is Character
                && (m_pOwner as Character).QueryTransformation != null
                && (m_pOwner as Character).QueryTransformation.Lookface > 0)
            {
                return false;
            }

            if (m_pOwner.IsWing() && nSort == MagicSort.MAGICSORT_TRANSFORM)
            {
                return false;
            }

            if (m_pOwner.Map.IsWingDisable() && nSort == MagicSort.MAGICSORT_ATTACHSTATUS && pData.Status == FlagInt.FLY)
            {
                return false;
            }

            IRole pRole = null;
            if (pData.Ground <= 0 
                && nSort != MagicSort.MAGICSORT_GROUNDSTING 
                && nSort != MagicSort.MAGICSORT_VORTEX
                && nSort != MagicSort.MAGICSORT_DASHWHIRL)
            {
                pRole = m_pOwner.BattleSystem.FindRole(idTarget);
                if (pRole == null)
                    return false;

                if (pRole is Character && m_pOwner is Character)
                {
                    if ((m_pOwner as Character).IsWatcher
                        || (pRole as Character).IsWatcher)
                        return false;
                }

                if (!pRole.IsAlive 
                    && nSort != MagicSort.MAGICSORT_DETACHSTATUS 
                    && nSort != MagicSort.MAGICSORT_ATTACHSTATUS
                    && nSort != MagicSort.MAGICSORT_DETACHBADSTATUS)
                    return false;

                if (nSort == MagicSort.MAGIC_ESCAPE_LIFE_PERCENT)
                {
                    if (pRole.Life * 100 / pRole.MaxLife >= 15)
                        return false;
                }

                x = pRole.MapX;
                y = pRole.MapY;
            }

            if (m_pOwner.GetDistance(x, y) > pData.Distance)
            {
                return false;
            }

            DynamicNpc pNpc = null;
            if (pRole != null && pRole is DynamicNpc)
            {
                pNpc = pRole as DynamicNpc;
            }

            if (pNpc != null)
            {
                if (!pNpc.IsBeAttackable())
                    return false;
                if (pNpc.IsGoal() && m_pOwner.Level < pNpc.Level)
                {
                    return false;
                }
            }

            return true;
        }

        public bool ProcessCollideFail(ushort x, ushort y, int nDir)
        {
            m_pSetTargetLocked.Clear();

            ushort nTargetX = (ushort)(m_pPos.X + Handlers.WALK_X_COORDS[nDir]);
            ushort nTargetY = (ushort)(m_pPos.Y + Handlers.WALK_Y_COORDS[nDir]);
            int nPower = 0;

            if (!m_pOwner.Map.IsStandEnable(nTargetX, nTargetY))
            {
                m_pOwner.Send(new MsgTalk(ServerString.STR_INVALID_MSG, ChatTone.TOP_LEFT));
                if (m_pOwner is Character)
                    (m_pOwner as Character).Disconnect("COLLIDE coord!");
                return false;
            }

            MsgInteract pMsg = new MsgInteract
            {
                EntityIdentity = m_pOwner.Identity,
                TargetIdentity = 0,
                CellX = nTargetX,
                CellY = nTargetY,
                Action = InteractionType.ACT_ITR_DASH,
                Damage = (ushort)(nDir * 0x01000000 + nPower)
            };
            m_pOwner.Screen.Send(pMsg, true);

            if (m_pOwner is Character)
            {
                (m_pOwner as Character).ProcessOnMove();
                (m_pOwner as Character).MoveToward((FacingDirection)nDir, false);
            }

            return true;
        }

        public bool AbortMagic(bool bSynchro)
        {
            //lock (m_pPreventDuplicity)
            {
                if (m_nMagicState == MagicState.MAGICSTATE_LAUNCH)
                {
                    //m_tApply.Clear();
                    return false;
                }
                BreakAutoAttack();

                if (m_nMagicState == MagicState.MAGICSTATE_DELAY)
                {
                    //m_tDelay.Clear();
                    return false;
                }

                m_pMagic = null;

                if (m_nMagicState == MagicState.MAGICSTATE_INTONE)
                {
                    m_tIntone.Clear();
                }

                m_nMagicState = MagicState.MAGICSTATE_NONE;

                if (bSynchro && m_pOwner is Character)
                {
                    m_pOwner.Send(new MsgAction(m_pOwner.Identity, 0, 0, GeneralActionType.ABORT_MAGIC));
                }
                return true;
            }
        }

        public void BreakAutoAttack()
        {
            m_bAutoAttack = false;
        }

        public int QueryPower()
        {
            return m_pMagic == null ? 0 : m_pMagic.Power;
        }

        public MagicData QueryMagic()
        {
            return m_pMagic == null ? null : this;
        }

        public void SetMagicState(int state) { m_nMagicState = state; }
        public bool IsIntone() { return m_nMagicState == 1; }
        public bool IsInLaunch() { return m_nMagicState == 2; }
        public bool IsActive() { return m_nMagicState == 0; }

        public bool AwardExpOfLife(IRole pTarget, int nLifeLost, bool bMagicRecruit = false)
        {
            Character pOwner = m_pOwner as Character;
            if (pTarget is DynamicNpc)
            {
                if (pOwner != null)
                    pOwner.AwardSynWarScore(pTarget as DynamicNpc, nLifeLost);
            }
            if (pTarget.IsMonster() || pTarget is DynamicNpc && pTarget.IsGoal()) // check if monster
            {
                int nExp = m_pOwner.AdjustExperience(pTarget, nLifeLost, false);

                if (!pTarget.IsAlive && !bMagicRecruit)
                {
                    int nBonusExp = (int)(pTarget.MaxLife * (5 / 100));
                    nExp += nBonusExp;

                    if (pOwner != null && !pOwner.Map.IsTrainingMap() && nBonusExp > 0)
                        pOwner.Send(string.Format(ServerString.STR_KILLING_EXPERIENCE, nBonusExp));
                }
                AwardExp(0, nExp, nExp);
            }
            return true;
        }

        public bool AwardExp(int nType, int nBattleExp, int nExp, Magic pMagic = null)
        {
            if (pMagic == null)
                return AwardExp(nBattleExp, nExp, true, m_pMagic);
            return AwardExp(nBattleExp, nExp, true, pMagic);
        }

        public bool AwardExp(int nBattleExp, int nExp, bool bIgnoreFlag, Magic pMagic = null)
        {
            if (nBattleExp <= 0 && nExp == 0) return false;

            if (pMagic == null)
                pMagic = m_pMagic;

            if (m_pOwner.Map.IsTrainingMap())
            {
                if (nBattleExp > 0)
                {
                    if (m_pOwner.IsBowman())
                        nBattleExp /= 2;
                    nBattleExp = Calculations.CutTrail(1, Calculations.MulDiv(nBattleExp, 10, 100));
                }
            }

            if (nBattleExp > 0)
                m_pOwner.AwardBattleExp(nBattleExp, true);

            if (pMagic == null)
                return false;

            if (!CheckAwardExpEnable(m_pOwner.Profession))
                return false;

            if (pMagic.NeedExp > 0
                && ((pMagic.AutoActive) & 16) == 0
                || bIgnoreFlag)
            {
                if (m_pOwner is Character)
                    nExp = (int)(nExp * ((1 + ((m_pOwner as Character).GetSkillGemEffect() / 100f))));

                pMagic.Experience += (uint)nExp;

                if (((pMagic.AutoActive) & 8) == 0)
                    pMagic.SendSkill();
                UpLevelMagic(true, pMagic);
                return true;
            }

            if (pMagic.NeedExp == 0
                && pMagic.Target == 4)
            {
                if (m_pOwner is Character)
                    nExp = (int)(nExp * ((1 + ((m_pOwner as Character).GetSkillGemEffect() / 100f))));

                pMagic.Experience += (uint)nExp;

                if (((pMagic.AutoActive) & 8) == 0)
                    pMagic.SendSkill();
                UpLevelMagic(true, pMagic);
            }
            return false;
        }

        public bool UpLevelMagic(bool synchro, Magic pMagic)
        {
            if (pMagic == null)
                return false;

            //if (!m_pMagic.IsWeaponMagic())
            //    return false;

            int nNeedExp = pMagic.NeedExp;

            if (!(nNeedExp > 0
                  && (pMagic.Experience >= nNeedExp
                      || (pMagic.OldLevel > 0
                      && pMagic.Level >= pMagic.OldLevel / 2
                      && pMagic.Level < pMagic.OldLevel))))
                return false;

            ushort nNewLevel = (ushort)(pMagic.Level + 1);
            pMagic.Experience = 0;
            pMagic.Level = nNewLevel;
            pMagic.SendSkill();

            return true;
        }

        public bool CheckAwardExpEnable(ushort dwProf)
        {
            if (m_pMagic == null)
                return false;
            return m_pOwner.Level >= m_pMagic.NeedLevel
                   && m_pMagic.NeedExp > 0
                   && m_pOwner.MapIdentity != 1005
                /*&& CheckProfession(dwProf, m_pMagic.NeedProf)*/;
        }

        public bool CheckProfession(uint dwProf, uint dwNeedProf)
        {
            // works but... unsure, sometimes fail with new skills
            ulong dwProfMask = Prof2Mask(dwProf);

            if (dwProfMask != 0)
            {
                const uint PROF_MASK_USER = 0x0000003F;
                if ((dwNeedProf & PROF_MASK_USER) == 0 || (dwProfMask & dwNeedProf) != 0)
                    return true;
            }
            else
            {
                const uint PROF_MASK_EVOLVE0 = 0x00003FC0;
                const uint PROF_MASK_EVOLVE1 = 0x0000C000;
                const uint PROF_MASK_EVOLVE2 = 0x00FF0000;

                dwProfMask = 0;
                uint dwProf1 = dwProf / 100;
                if (dwProf1 > 0)
                    dwProfMask = (ulong)(1 << (int)(dwProf1 - 1 + MAX_USER_PROFS));
                if ((dwNeedProf & PROF_MASK_EVOLVE0) != 0 && (dwNeedProf & dwProfMask) == 0)
                    return false;

                dwProfMask = 0;
                uint dwProf2 = dwProf / 10 % 10;
                if (dwProf2 > 0)
                    dwProfMask = (ulong)(1 << (int)(dwProf2 - 1 + MAX_USER_PROFS + 8));
                if ((dwNeedProf & PROF_MASK_EVOLVE1) != 0 && (dwNeedProf & dwProfMask) == 0)
                    return false;

                dwProfMask = 0;
                uint dwProf3 = dwProf % 10;
                if (dwProf3 > 0)
                    dwProfMask = (ulong)(1 << (int)(dwProf3 - 1 + MAX_USER_PROFS + 8 + 2));
                if ((dwNeedProf & PROF_MASK_EVOLVE2) != 0 && (dwNeedProf & dwProfMask) == 0)
                    return false;

                return true;
            }

            return false;
        }

        public ulong Prof2Mask(uint dwProf)
        {
            for (int i = 0; i < _Prof2MaskTable.Length; i++)
                if (dwProf == _Prof2MaskTable[i])
                    return (ulong)(1 << i);
            return 0;
        }

        public bool CheckCrime(IRole pRole)
        {
            if (pRole == null || m_pMagic == null) return false;

            if (m_pMagic.Crime <= 0)
                return false;

            return m_pOwner.CheckCrime(pRole);
        }

        public bool CheckCrime(Dictionary<uint, IRole> pRoleSet)
        {
            if (pRoleSet == null || m_pMagic == null) return false;

            if (m_pMagic.Crime <= 0)
                return false;

            foreach (var pRole in pRoleSet.Values)
                if (m_pOwner.Identity != pRole.Identity && m_pOwner.CheckCrime(pRole))
                    return true;
            return false;
        }

        public bool CheckCrime(List<IRole> pRoleSet)
        {
            if (pRoleSet == null || m_pMagic == null) return false;

            if (m_pMagic.Crime <= 0)
                return false;

            foreach (var pRole in pRoleSet)
                if (m_pOwner.Identity != pRole.Identity && m_pOwner.CheckCrime(pRole))
                    return true;
            return false;
        }

        public bool HitByWeapon()
        {
            if (m_pMagic == null)
                return true;

            if (m_pMagic.WeaponHit == 1)
                return true;

            Item pItem;
            if (m_pOwner is Character
                && (m_pOwner as Character).Equipment.Items.TryGetValue(ItemPosition.RIGHT_HAND, out pItem)
                && pItem.Itemtype.MagicAtk <= 0)
                return true;

            return false;
        }

        public bool IsImmunity(IRole pRole)
        {
            if (m_pMagic == null) return true;
            if (pRole.IsWing()
                && !m_pOwner.IsWing()
                && m_pMagic.WeaponHit > 0
                && m_pMagic.WeaponSubtype != 500
                && m_pMagic.WeaponSubtype != 610)
                return true;

            return m_pOwner.IsImmunity(pRole);
        }

        public void OnTimer()
        {
            if (m_pMagic == null)
            {
                m_nMagicState = MagicState.MAGICSTATE_NONE;
                return;
            }

            switch (m_nMagicState)
            {
                case MagicState.MAGICSTATE_INTONE: // intone
                    {
                        if (m_tIntone != null && !m_tIntone.IsTimeOut())
                            return;

                        if (m_tIntone != null && m_tIntone.IsTimeOut() && !Launch())
                        {
                            m_tApply.Clear();
                            ResetDelay();
                        }

                        //if (m_pOwner.Map.IsTrainingMap())
                        //{
                        //    m_tApply = new TimeOutMS((int) m_pMagic.Timeout*1000);
                        //    m_tApply.Update();
                        //    break;
                        //}

                        m_nMagicState = 2;
                        m_nApplyTimes = (int)Math.Max(1, m_pMagic.ActiveTimes);

                        if (m_tApply == null)
                            m_tApply = new TimeOutMS(m_pMagic.GetApplyMs());

                        m_tApply.Startup(m_pMagic.GetApplyMs());
                        break;
                    }
                case MagicState.MAGICSTATE_LAUNCH: // launch{
                    {
                        if (!m_tApply.IsActive() || m_tApply.TimeOver())
                        {
                            if (m_pMagic.Sort == MagicSort.MAGICSORT_ATTACK && m_idTarget != 0)
                            {
                                var pTarget = m_pOwner.BattleSystem.FindRole(m_idTarget);
                                if (pTarget != null && !pTarget.IsAlive && pTarget.IsAttackable(m_pOwner))
                                    m_pOwner.Kill(pTarget, GetDieMode());
                            }
                            ResetDelay();
                            m_nMagicState = MagicState.MAGICSTATE_NONE;
                            AbortMagic(false);
                        }
                        break;
                    }
                case MagicState.MAGICSTATE_DELAY: // delay
                    {
                        if (m_pOwner.Map.IsTrainingMap()
                            && m_tDelay.IsActive()
                            && m_pMagic.Sort != MagicSort.MAGICSORT_ATKSTATUS)
                        {
                            if (m_tDelay.IsTimeOut())
                            {
                                m_nMagicState = MagicState.MAGICSTATE_NONE;
                                if (!m_pOwner.ProcessMagicAttack(m_pMagic.Type, m_idTarget, (ushort)m_pPos.X,
                                        (ushort)m_pPos.Y,
                                        0))
                                    m_nMagicState = MagicState.MAGICSTATE_DELAY;
                            }
                            return;
                        }

                        if (!m_tDelay.IsActive())
                        {
                            m_nMagicState = MagicState.MAGICSTATE_NONE;
                            AbortMagic(true);
                            return;
                        }

                        if (m_bAutoAttack && m_tDelay.ToNextTime())
                        {
                            if ((m_tDelay.IsActive() && !m_tDelay.TimeOver()))
                                return;

                            m_nMagicState = MagicState.MAGICSTATE_NONE;
                            m_pOwner.ProcessMagicAttack(m_pMagic.Type, m_idTarget, (ushort)m_pPos.X, (ushort)m_pPos.Y,
                                0);

                            if (!m_pOwner.Map.IsTrainingMap())
                                AbortMagic(false);
                        }

                        if (m_tDelay.IsActive() && m_tDelay.TimeOver())
                        {
                            m_nMagicState = MagicState.MAGICSTATE_NONE;
                            AbortMagic(false);
                        }
                        break;
                    }
            }
        }

        private ulong GetDieMode()
        {
            return (ulong)(HitByMagic() > 0 ? 3 : (m_pOwner.IsBowman() ? 5 : 1));
        }

        public int GetSort()
        {
            if (m_pMagic != null) return (int)m_pMagic.Sort;
            return 0;
        }

        public ElementType GetElement()
        {
            if (m_pMagic != null) return m_pMagic.ElementType;
            return 0;
        }

        #endregion

        public bool CheckLevel(ushort type, ushort level)
        {
            return Magics.Values.FirstOrDefault(x => x.Type == type && x.Level == level) != null;
        }

        public bool CheckType(ushort type)
        {
            return Magics.ContainsKey(type);
        }

        public bool UpLevelByTask(ushort type)
        {
            Magic pMagic;
            if (!Magics.TryGetValue(type, out pMagic))
                return false;
            if (!IsWeaponMagic(pMagic.Type))
                return false;

            byte nNewLevel = (byte)(pMagic.Level + 1);
            if (!FindMagicType(type, nNewLevel))
                return false;

            pMagic.Experience = 0;
            pMagic.Level = nNewLevel;

            return true;
        }

        public bool FindMagicType(ushort type, byte pLevel)
        {
            return ServerKernel.Magictype.Values.FirstOrDefault(x => x.Type == type && x.Level == pLevel) != null;
        }

        public bool Delete(ushort nType)
        {
            Magic trash;
            if (Magics.TryRemove(nType, out trash))
            {
                m_pOwner.Send(new MsgAction(m_pOwner.Identity, nType, 0, 0, GeneralActionType.DROP_MAGIC));
                return trash.Delete();
            }
            return false;
        }

        public Magic this[ushort nType]
        {
            get
            {
                Magic ret;
                return Magics.TryGetValue(nType, out ret) ? ret : null;
            }
        }
    }
}
