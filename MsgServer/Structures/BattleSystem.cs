// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Battle System.cs
// Last Edit: 2016/12/06 13:54
// Created: 2016/12/06 13:53

using System;
using System.Linq;
using Core.Common.Enums;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    enum MagicType
    {
        MAGICTYPE_NONE = 0,
        MAGICTYPE_NORMAL = 1,
        MAGICTYPE_XPSKILL = 2
    }

    public class BattleSystem
    {
        private bool m_bAutoAttack = false;
        private uint m_dwTargetId = 0;
        private int m_nDelay = 0;
        private bool m_bTargetLocked = false;
        private int m_nData;
        private int m_nRawDelay;

        private IRole m_pOwner;
        private TimeOutMS m_tAttack;
        private TimeOutMS m_tDelay;
        private TimeOutMS m_tFight;

        public BattleSystem(IRole pRole)
        {
            m_pOwner = pRole;

            m_nDelay = m_nRawDelay = 800;
            m_tDelay = new TimeOutMS(m_nRawDelay);
            m_tAttack = new TimeOutMS(0);
            m_tFight = new TimeOutMS(0);
        }

        public IRole FindRole(uint idRole)
        {
            if (idRole >= 1000000)
            {
                return m_pOwner.Map.Players.Values.FirstOrDefault(x => x.Identity == idRole);
            }
            return m_pOwner.Map.GameObjects.Values.FirstOrDefault(x => x.Identity == idRole) as IRole;
        }

        public bool CreateBattle(uint idTarget)
        {
            if (idTarget == 0)
                return false;
            m_dwTargetId = idTarget;
            return true;
        }

        public void ResetBattle()
        {
            m_dwTargetId = 0;
            m_bAutoAttack = false;
        }

        public bool IsBattleMaintain()
        {
            if (m_dwTargetId == 0)
                return false;

            var pTarget = FindRole(m_dwTargetId);

            if (pTarget == null || !pTarget.IsAlive || !pTarget.IsAttackable(m_pOwner))
            {
                ResetBattle();
                return false;
            }

            if (pTarget.Map.IsLineSkillMap())
                return false;

            // cant attack flying
            if (pTarget.IsWing()
                && !m_pOwner.IsWing()
                && !(m_pOwner.IsBowman() || m_pOwner.IsSimpleMagicAtk()))
            {
                ResetBattle();
                return false;
            }

            int nDist = m_pOwner.GetDistance(pTarget as IScreenObject);

            if (nDist > m_pOwner.GetAttackRange(pTarget.GetSizeAdd()))
            {
                ResetBattle();
                return false;
            }

            if (m_pOwner is Character && !(m_pOwner as Character).LoginComplete)
                return false;

            if (m_pOwner.QueryStatus(FlagInt.DAZED) != null
                || m_pOwner.QueryStatus(FlagInt.HUGE_DAZED) != null
                || m_pOwner.QueryStatus(FlagInt.ICE_BLOCK) != null
                || m_pOwner.QueryStatus(FlagInt.CONFUSED) != null)
                return false;

            if (m_pOwner.Map.QueryRegion(RegionType.REGION_PK_PROTECTED, m_pOwner.MapX, m_pOwner.MapY)
                || pTarget.Map.QueryRegion(RegionType.REGION_PK_PROTECTED, pTarget.MapX, pTarget.MapY))
                return false;

            return true;
        }

        public bool IsActived()
        {
            return m_dwTargetId != 0;
        }

        public bool ProcAttack_Hand2Hand()
        {
            try
            {
                if (m_pOwner == null || m_dwTargetId == 0 || !IsBattleMaintain())
                {
                    ResetBattle();
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog("ProcAttack_Hand2Hand.IsBattleMaint", false, LogType.WARNING);
                ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
                return false;
            }

            var pTarget = FindRole(m_dwTargetId);

            if (pTarget == null)
            {
                ResetBattle();
                return false;
            }

            if (m_pOwner.IsImmunity(pTarget))
            {
                ResetBattle();
                return false;
            }

            Character pOwner = null;
            if (m_pOwner is Character)
            {
                pOwner = m_pOwner as Character;
            }

            if (IsTargetDodged(m_pOwner, pTarget))
            {
                m_pOwner.SendDamageMsg(pTarget.Identity, 0, InteractionEffect.NONE);

                return true;
            }

            // if archer
            //if (m_pOwner.IsBowman())
            //{
            //    if (pOwner != null)
            //    {
            //        if (!pOwner.Equipment.Items.ContainsKey(5) || !pOwner.Equipment.Items[5].IsArrowSort()
            //            || pOwner.Equipment.Items[5].Durability <= 0)
            //        {
            //            if (!pOwner.ReloadArrows())
            //            {
            //                ResetBattle();
            //                return false; // no arrows
            //            }
            //        }
            //        else
            //        {
            //            pOwner.Equipment.Items[5].Durability -= 1;
            //        }
            //        pOwner.Send(pOwner.Equipment.Items[5].InformationPacket(true));
            //    }
            //}

            if (QueryMagic() != null)
                QueryMagic().AbortMagic(true);

            if (pTarget is Character)
            {
                Character pTargetUser = pTarget as Character;
                if (pTargetUser.CheckScapegoat(m_pOwner))
                    return true;
            }

            if (m_pOwner is Character && m_pOwner.AutoSkillAttack(pTarget))
            {
                return true;
            }

            InteractionEffect special = InteractionEffect.NONE;
            int nDamage = m_pOwner.Attack(pTarget, ref special);
            int nTargetLifeLost = Math.Max(1, nDamage);
            int nExp = (int)Math.Min(pTarget.MaxLife, nTargetLifeLost);

            if (m_pOwner.QueryStatus(FlagInt.FATAL_STRIKE) != null
                && pTarget is Monster)
            {
                Monster pMob = pTarget as Monster;
                if (!pMob.IsGuard()
                    && !pMob.IsPlayer()
                    && !pMob.IsDynaNpc()
                    && !pMob.IsDynaMonster())
                {
                    m_pOwner.MapX = pTarget.MapX;
                    m_pOwner.MapY = pTarget.MapY;
                    var msg = new MsgAction(m_pOwner.Identity, pTarget.Identity, m_pOwner.MapX, m_pOwner.MapY,
                        GeneralActionType.NINJA_STEP);
                    m_pOwner.Send(msg);
                    m_pOwner.Screen.SendMovement(msg);
                }
            }

            m_pOwner.SendDamageMsg(pTarget.Identity, (uint)nTargetLifeLost, special);
            if (nDamage == 0)
                return false;

            // pTarget.AddAttribute(ClientUpdateType.HITPOINTS, -1*nTargetLifeLost, true);
            pTarget.BeAttack(0, m_pOwner, nDamage, true);

            // Syn rank
            DynamicNpc pNpc = null;
            Character pOwnerUser = null;
            if (pTarget is DynamicNpc)
                pNpc = pTarget as DynamicNpc;

            if (m_pOwner is Character)
                pOwnerUser = m_pOwner as Character;

            if (pNpc != null && pOwnerUser != null && pNpc.IsAwardScore())
            {
                if (pNpc.IsCtfFlag())
                    pOwnerUser.AwardCtfScore(pNpc, nTargetLifeLost);
                else
                    pOwnerUser.AwardSynWarScore(pNpc, nTargetLifeLost);
            }

            if (m_pOwner is Character && pNpc != null && pNpc.IsGoal() || pTarget.IsMonster()) // check is monster
            {
                nExp = m_pOwner.AdjustExperience(pTarget, nExp, false);
                int nAdditionExp = 0;
                if (!pTarget.IsAlive)
                {
                    nAdditionExp = (int)(pTarget.MaxLife * 5 / 100);
                    nExp += nAdditionExp;

                    if (pOwnerUser != null)
                    {
                        if (pOwnerUser.Team != null)
                            pOwnerUser.Team.AwardMemberExp(pOwnerUser.Identity, pTarget, nAdditionExp);
                    }
                }

                m_pOwner.AwardBattleExp(nExp, true);

                if (!pTarget.IsAlive && nAdditionExp > 0)
                    if (!pTarget.IsAlive && !m_pOwner.Map.IsTrainingMap() && pOwnerUser != null)
                        pOwnerUser.Send(string.Format(ServerString.STR_KILLING_EXPERIENCE, nAdditionExp),
                            ChatTone.TOP_LEFT);

                if (pOwnerUser != null)
                {
                    Item item;
                    if (pOwnerUser.Equipment.Items.TryGetValue(ItemPosition.RIGHT_HAND, out item))
                    {
                        pOwnerUser.WeaponSkill.AwardExperience((ushort)item.GetItemSubtype(), nExp);
                    }
                    if (pOwnerUser.Equipment.Items.TryGetValue(ItemPosition.LEFT_HAND, out item))
                    {
                        pOwnerUser.WeaponSkill.AwardExperience((ushort)item.GetItemSubtype(), nExp);
                    }
                }
            }

            m_pOwner.AdditionMagic(nTargetLifeLost, nDamage);

            if (Calculations.ChanceCalc(5f) && m_pOwner is Character)
            {
                (m_pOwner as Character).SendWeaponMagic2(pTarget);
            }

            if (!pTarget.IsAlive)
            {
                int dwDieWay = m_pOwner.IsSimpleMagicAtk() ? 3 : 1;
                if (nDamage > pTarget.MaxLife / 3)
                    dwDieWay = 2;

                m_pOwner.Kill(pTarget, m_pOwner.IsBowman() ? 5 : (uint)dwDieWay);
            }

            if (pOwnerUser != null && pOwnerUser.QueryStatus(FlagInt.FATAL_STRIKE) != null)
            {
                DestroyAutoAttack();
            }

            return true;
        }

        public void OtherMemberAwardExp(IRole pTarget, int nRawExp)
        {
            if (m_pOwner.Map.IsTrainingMap())
                return;

            Character pOwner = m_pOwner as Character;
            if (pOwner != null && pOwner.Team != null)
                pOwner.Team.AwardMemberExp(pOwner.Identity, pTarget, nRawExp);
        }

        public int GetDieMode()
        {
            return (m_pOwner.IsBowman() ? 5 : 1);
        }

        public int AdjustDrop(int nDrop, int nAtkLev, int nDefLev)
        {
            if (nAtkLev > 120)
                nAtkLev = 120;

            if (nAtkLev - nDefLev > 0)
            {
                int nDeltaLev = nAtkLev - nDefLev;
                if (1 < nAtkLev && nAtkLev <= 19)
                {
                    if (nDeltaLev < 3)
                        ;
                    else if (3 <= nDeltaLev && nDeltaLev < 6)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (19 < nAtkLev && nAtkLev <= 49)
                {
                    if (nDeltaLev < 5)
                        ;
                    else if (5 <= nDeltaLev && nDeltaLev < 10)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (49 < nAtkLev && nAtkLev <= 85)
                {
                    if (nDeltaLev < 4)
                        ;
                    else if (4 <= nDeltaLev && nDeltaLev < 8)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (85 < nAtkLev && nAtkLev <= 112)
                {
                    if (nDeltaLev < 3)
                        ;
                    else if (3 <= nDeltaLev && nDeltaLev < 6)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (112 < nAtkLev && nAtkLev <= 120)
                {
                    if (nDeltaLev < 2)
                        ;
                    else if (2 <= nDeltaLev && nDeltaLev < 4)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (120 < nAtkLev && nAtkLev <= 130)
                {
                    if (nDeltaLev < 2)
                        ;
                    else if (2 <= nDeltaLev && nDeltaLev < 4)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (130 < nAtkLev && nAtkLev <= 140)
                {
                    if (nDeltaLev < 2)
                        ;
                    else if (2 <= nDeltaLev && nDeltaLev < 4)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
            }

            return Calculations.CutTrail(0, nDrop);
        }

        public void SetAutoAttack()
        {
            m_bAutoAttack = true;
        }

        public bool IsAutoAttack()
        {
            return m_bAutoAttack;
        }

        public void DestroyAutoAttack()
        {
            m_bAutoAttack = false;
        }

        public int CalcPower(int nMagic, IRole pAtker, IRole pTarget, ref InteractionEffect pSpecial
            , int nAdjustAtk = 0, bool bCanDodge = false)
        {
            int nPower = 0;

            if (nMagic == (int)MagicType.MAGICTYPE_NONE)
                nPower += CalcAttackPower(pAtker, pTarget, ref pSpecial);
            else
            {
                nPower = CalcMagicPower(pAtker, pTarget, nAdjustAtk, ref pSpecial);
                if (nMagic == (int)MagicType.MAGICTYPE_XPSKILL)
                    if (pTarget is Character)
                        nPower /= 50;
            }

            return nPower;
        }

        public int CalcAttackPower(IRole attacker, IRole attacked, ref InteractionEffect pSpecial)
        {
            if (attacked is Character)
            {
                Character pUser = attacked as Character;
                if (pUser.QueryTransformation != null && pUser.QueryTransformation.Lookface == 223)
                    return 1;
            }

            if (m_pOwner.Map.IsLineSkillMap())
                return 1;

            if (attacked.QueryStatus(FlagInt.VORTEX) != null)
            {
                return 1;
            }

            int nAttack = 0;

            if (Calculations.ChanceCalc(50))
                nAttack = attacker.MaxAttack - ThreadSafeRandom.RandGet(1, Math.Max(1, attacker.MaxAttack - attacker.MinAttack) / 2 + 1);
            else
                nAttack = attacker.MinAttack + ThreadSafeRandom.RandGet(1, Math.Max(1, attacker.MaxAttack - attacker.MinAttack) / 2 + 1);

            if (attacker is Character && attacked is Character && (attacker as Character).IsBowman())
                nAttack = (int)(nAttack / 1.5f);

            // handle physical status
            if (attacker.QueryStatus(FlagInt.STIG) != null)
            {
                float tPower = attacker.QueryStatus(FlagInt.STIG).Power;
                if (tPower > 30000)
                {
                    tPower = (tPower - 30000) / 100f;
                    nAttack = (int)(nAttack * tPower);
                }
                else
                    nAttack += (short)tPower;
            }

            int nRawDefense = attacked.Defense;
            int nDef = attacked.AdjustDefense(nRawDefense);

            if (attacker.QueryStatus(FlagInt.OBLIVION) != null 
                && !(attacked is Character) 
                && ((attacked is Monster) && !(attacked as Monster).IsBoss))
            {
                nAttack *= 2;
            }

            if (attacker.QueryStatus(FlagInt.FATAL_STRIKE) != null
                && ((!attacked.IsDynaNpc() && !(attacked is Character))))
            {
                float tPower = attacker.QueryStatus(FlagInt.FATAL_STRIKE).Power;
                if (tPower > 30000)
                {
                    tPower = (tPower - 30000) / 100f;
                    nAttack = (int)(nAttack * tPower);
                }
                else
                    nAttack += (short)tPower;

                if (attacked is Monster)
                {
                    Monster pMob = attacked as Monster;
                    if (pMob.IsGuard())
                        nAttack /= 10;
                }
            }

            if (attacker.QueryStatus(FlagInt.VORTEX) != null && !attacked.IsDynaNpc() && !(attacked is Character))
            {
                float tPower = attacker.QueryStatus(FlagInt.VORTEX).Power;
                if (tPower > 30000)
                {
                    tPower = (tPower - 30000) / 100f;
                    nAttack = (int)(nAttack * tPower);
                }
                else
                    nAttack += (short)tPower;
            }

            if (attacker.QueryStatus(FlagInt.SUPERMAN) != null 
                && (!attacked.IsDynaNpc() && !(attacked is Character)))
            {
                float tPower = attacker.QueryStatus(FlagInt.SUPERMAN).Power;
                if (tPower > 30000)
                {
                    tPower = (tPower - 30000) / 100f;
                    nAttack = (int)(nAttack * tPower);
                }
                else
                    nAttack += (short)tPower;
            }

            if (attacked.QueryStatus(FlagInt.SHIELD) != null)
            {
                float tPower = attacked.QueryStatus(FlagInt.SHIELD).Power;
                if (tPower > 30000)
                {
                    tPower = (tPower - 30000) / 100f;
                    nDef = (int)(nDef * tPower);
                }
                else
                    nDef += (short)tPower;
            }

            if (attacker.Magics.QueryMagic() != null)
            {
                float tPower = attacker.Magics.QueryMagic().QueryPower();
                if (tPower > 30000)
                {
                    tPower = (tPower - 30000) / 100f;
                    nAttack = (int)(nAttack * tPower);
                }
                else
                    nAttack += (short)tPower;
            }

            //float reduction = attacked.GetReduceDamage();
            int nDamage = (int)((nAttack - nDef) * (1f - (attacked.GetReduceDamage() / 100f)));
            float tort = (attacked.GetTortoiseGemEffect() / 100f);
            nDamage = (int)(nDamage * (1f - tort));

            if (nDamage <= 0) nDamage = 7;

            if (attacker is Character && attacked.IsMonster())
            {
                nDamage = CalcDamageUser2Monster(nAttack, nDef, attacker.Level, attacked.Level);
                nDamage = attacked.AdjustWeaponDamage(nDamage);
                nDamage = AdjustMinDamageUser2Monster(nDamage, attacker, attacked);
            }
            else if (attacker.IsMonster() && attacked is Character)
            {
                nDamage = CalcDamageMonster2User(nAttack, nDef, attacker.Level, attacked.Level);
                nDamage = attacked.AdjustWeaponDamage(nDamage);
                nDamage = AdjustMinDamageMonster2User(nDamage, attacker, attacked);
            }
            else
            {
                nDamage = attacked.AdjustWeaponDamage(nDamage);
            }

            //if (attacker is Character && attacked is Character && attacker.BattlePower < attacked.BattlePower)
            //{
            //    nDamage /= 2;
            //}

            #region Block, Critical, Break
            if (attacker.BattlePower < attacked.BattlePower)
            {
                if (attacked is Character)
                {
                    // Break (Pene is on the magic code)
                    // Break through the battle power cap...
                    // If the break fails, the damage is reduced by half.
                    if (attacked.Counteraction < attacker.Breakthrough)
                    {
                        if (!Calculations.ChanceCalc((float)(attacker.Breakthrough - attacked.Counteraction) / 10))
                            nDamage /= 2;
                        else
                            pSpecial |= InteractionEffect.BREAKTHROUGH;
                        //Owner.SendMessage(string.Format("Break: {0} Counter: {1} Difference: {2}%", GetBreakthrough(), pTarget.GetCounteraction(), (float)(GetBreakthrough() - pTarget.GetCounteraction()) / 10));
                    }
                    else
                    {
                        nDamage /= 2;
                    }
                }
            }

            // Critical is enabled on every monster. :)
            // Multiply the damage by 1.5
            if (attacker.CriticalStrike > attacked.Immunity)
            {
                if (Calculations.ChanceCalc((float)(attacker.CriticalStrike - attacked.Immunity) / 100))
                {
                    nDamage = (int)(nDamage * 1.5f);
                    pSpecial |= InteractionEffect.CRITICAL_STRIKE;
                }
            }

            if (attacked.Block > 0 && Calculations.ChanceCalc((float)attacked.Block / 100))
            {
                nDamage /= 10;
                pSpecial |= InteractionEffect.BLOCK;
            }

            if (QueryMagic() != null && QueryMagic().GetElement() > ElementType.NONE)
            {
                switch (QueryMagic().GetElement())
                {
                    case ElementType.WATER:
                        pSpecial |= InteractionEffect.WATER_RESIST;
                        break;
                    case ElementType.FIRE:
                        pSpecial |= InteractionEffect.FIRE_RESIST;
                        break;
                    case ElementType.WOOD:
                        pSpecial |= InteractionEffect.WOOD_RESIST;
                        break;
                    case ElementType.METAL:
                        pSpecial |= InteractionEffect.METAL_RESIST;
                        break;
                    case ElementType.EARTH:
                        pSpecial |= InteractionEffect.EARTH_RESIST;
                        break;
                }
            }
            #endregion

            if (attacker is Monster && QueryMagic() != null && QueryMagic().GetElement() > ElementType.NONE)
            {
                switch (QueryMagic().GetElement())
                {
                    case ElementType.WATER:
                    {
                        nDamage = (int) (nDamage*(1 - (attacked.WaterResistance/100f)));
                        break;
                    }
                    case ElementType.FIRE:
                    {
                        nDamage = (int) (nDamage*(1 - (attacked.FireResistance/100f)));
                        break;
                    }
                    case ElementType.EARTH:
                    {
                        nDamage = (int) (nDamage*(1 - (attacked.EarthResistance/100f)));
                        break;
                    }
                    case ElementType.WOOD:
                    {
                        nDamage = (int) (nDamage*(1 - (attacked.WoodResistance/100f)));
                        break;
                    }
                    case ElementType.METAL:
                    {
                        nDamage = (int) (nDamage*(1 - (attacked.MetalResistance/100f)));
                        break;
                    }
                }
            }

            if (attacker is Character)
            {
                nDamage += attacker.AddFinalAttack;
            }

            if (attacked is Character)
                nDamage -= attacked.AddFinalDefense;

            if (attacked is Monster)
            {
                nDamage = (int) Math.Min(nDamage, attacked.MaxLife*700);
            }

            if (nDamage <= 0)
                nDamage = 1;

            return nDamage;
        }

        public int CalcMagicPower(IRole pAtker, IRole pTarget, int pAdjustAtk, ref InteractionEffect pSpecial) // /*=0*/, ref InteractionEffect special)
        {
            if (pTarget is Character)
            {
                Character pUser = pTarget as Character;
                if (pUser.QueryTransformation != null && pUser.QueryTransformation.Lookface == 223)
                    return 1;
            }

            if (m_pOwner.Map.IsLineSkillMap())
                return 1;

            if (pTarget.QueryStatus(FlagInt.VORTEX) != null)
            {
                return 1;
            }

            int nAtk = pAtker.MagicAttack;

            if (pAtker.Magics.QueryMagic() != null)
            {
                float tPower = pAtker.Magics.QueryMagic().QueryPower();
                if (tPower > 30000)
                {
                    tPower = (tPower - 30000) / 100f;
                    nAtk = (int)(nAtk * tPower);
                }
                else
                    nAtk += (short)tPower;
            }

            int nDef = pTarget.MagicDefense; // * (1 + (pTarget.Magic / 100));

            if (pTarget is Character)
            {
                int nCounter = (int) (pTarget.Counteraction / 10f);
                int nPene = (int) (pAtker.Penetration / 100f);
                if (nCounter < nPene)
                {
                    if (!Calculations.ChanceCalc((nPene - nCounter)))
                    {
                        Character pUser = pTarget as Character;
                        nDef = (int) (nDef*(1 + (pUser.MagicDefenseBonus/100f)));
                    }
                    else
                    {
                        nAtk = (int) (nAtk*1.25f);
                        pSpecial |= InteractionEffect.BREAKTHROUGH;
                    }
                }
                else
                {
                    Character pUser = pTarget as Character;
                    nDef = (int)(nDef * (1 + (pUser.MagicDefenseBonus / 100f)));
                }
            }

            int nDamage = (int)((nAtk - nDef) * (1f - (pTarget.GetReduceDamage() / 100f)));
            nDamage = (int)(nDamage * (1f - (pTarget.GetTortoiseGemEffect() / 100f)));

            if (pAtker is Character && pTarget.IsMonster())
            {
                nDamage = CalcDamageUser2Monster(nDamage, nDef, pAtker.Level, pTarget.Level);
                nDamage = pTarget.AdjustMagicDamage(nDamage);
                nDamage = AdjustMinDamageUser2Monster(nDamage, pAtker, pTarget);
            }
            else if (pAtker.IsMonster() && pTarget is Character)
            {
                nDamage = CalcDamageMonster2User(nDamage, nDef, pAtker.Level, pTarget.Level);
                nDamage = pTarget.AdjustMagicDamage(nDamage);
                nDamage = AdjustMinDamageMonster2User(nDamage, pAtker, pTarget);
            }
            else
            {
                nDamage = pAtker.AdjustMagicDamage(nDamage);
            }

            if (pAtker.BattlePower < pTarget.BattlePower)
            {
                if (pTarget is Character)
                {
                    int levelDiff = pTarget.BattlePower - pAtker.BattlePower;
                    float disccount = 0;
                    if (levelDiff > 50)
                        disccount = 50;
                    else
                        disccount = 100 - levelDiff;

                    nDamage = (int)(nDamage * (disccount / 100));
                }
            }

            if (pAtker.SkillCriticalStrike > pTarget.Immunity)
            {
                if (Calculations.ChanceCalc((float)(pAtker.SkillCriticalStrike - pTarget.Immunity) / 100))
                {
                    nDamage = (int)(nDamage * 2f);
                    pSpecial |= InteractionEffect.CRITICAL_STRIKE;
                }
            }

            if (QueryMagic() != null && QueryMagic().GetElement() > ElementType.NONE)
            {
                switch (QueryMagic().GetElement())
                {
                    case ElementType.WATER:
                        pSpecial |= InteractionEffect.WATER_RESIST;
                        break;
                    case ElementType.FIRE:
                        pSpecial |= InteractionEffect.FIRE_RESIST;
                        break;
                    case ElementType.WOOD:
                        pSpecial |= InteractionEffect.WOOD_RESIST;
                        break;
                    case ElementType.METAL:
                        pSpecial |= InteractionEffect.METAL_RESIST;
                        break;
                    case ElementType.EARTH:
                        pSpecial |= InteractionEffect.EARTH_RESIST;
                        break;
                }
            }

            if (pAtker is Monster && QueryMagic() != null && QueryMagic().GetElement() > ElementType.NONE)
            {
                nDamage += pAtker.Magics.GetElementPower(pTarget);
                //switch (QueryMagic().GetElement())
                //{
                //    case ElementType.WATER:
                //        {
                //            nDamage = (int)(nDamage * (1 - (pTarget.WaterResistance / 100f)));
                //            break;
                //        }
                //    case ElementType.FIRE:
                //        {
                //            nDamage = (int)(nDamage * (1 - (pTarget.FireResistance / 100f)));
                //            break;
                //        }
                //    case ElementType.EARTH:
                //        {
                //            nDamage = (int)(nDamage * (1 - (pTarget.EarthResistance / 100f)));
                //            break;
                //        }
                //    case ElementType.WOOD:
                //        {
                //            nDamage = (int)(nDamage * (1 - (pTarget.WoodResistance / 100f)));
                //            break;
                //        }
                //    case ElementType.METAL:
                //        {
                //            nDamage = (int)(nDamage * (1 - (pTarget.MetalResistance / 100f)));
                //            break;
                //        }
                //}
            }

            if (pAtker is Character)
                nDamage += pAtker.AddFinalMagicAttack;
            if (pTarget is Character)
                nDamage -= pTarget.AddFinalMagicDefense;

            // Adjust synflag damage
            if (pTarget is DynamicNpc)
            {
                //var npc = pTarget as DynamicNpc;
                //if (npc.IsSynFlag()
                //    && npc.IsSynMoneyEmpty())
                nDamage = nDamage * Character.SYNWAR_NOMONEY_DAMAGETIMES;
            }

            return Calculations.CutTrail(1, nDamage);
        }

        public int AdjustAttack(int nAtk, IScreenObject attacker)
        {
            int nAddAtk = 0;

            // TODO

            return nAtk + nAddAtk;
        }

        public bool IsTargetDodged(IRole attacker, IRole attacked)
        {
            if (attacker == null || attacked == null) return true;

            //if (attacked.BattleSystem.QueryMagic() != null && attacked.BattleSystem.QueryMagic().IsInLaunch()) return true;

            int nDodge = 0;
            if (attacked is Character || attacked is DynamicNpc || (attacked is Monster && (attacked as Monster).IsBoss))
                nDodge = 50;

            int atkHit = attacker.AttackHitRate;
            if (attacker.QueryStatus(FlagInt.STAR_OF_ACCURACY) != null)
                atkHit = Calculations.AdjustData(atkHit, attacker.QueryStatus(FlagInt.STAR_OF_ACCURACY).Power);

            int atkdDodge = attacked.Dodge;
            if (attacked.QueryStatus(FlagInt.DODGE) != null)
                atkHit = Calculations.AdjustData(atkHit, attacker.QueryStatus(FlagInt.DODGE).Power);

            int hitRate = Math.Min(100, Math.Max(40, 100 + atkHit - nDodge - atkdDodge));

            if (hitRate < 40)
                hitRate = 40;

            if (attacker is Character)
                if ((attacker as Character).IsPm)
                    (attacker as Character).Send("HitRate: " + hitRate);

            if (attacked is Character)
                if ((attacked as Character).IsPm)
                    (attacked as Character).Send("Attacker HitRate: " + hitRate);

            //if (attacker.Profession / 10 != 4 && hitRate > 90)
            //    hitRate = 90;

            return !Calculations.ChanceCalc(hitRate);
        }

        public int GetNameType(int nAtkLev, int nDefLev)
        {
            int nDeltaLev = nAtkLev - nDefLev;

            if (nDeltaLev >= 3)
                return NAME_GREEN;
            if (nDeltaLev >= 0)
                return NAME_WHITE;
            if (nDeltaLev >= -5)
                return NAME_RED;
            return NAME_BLACK;
        }

        public int CalcDamageUser2Monster(int nAtk, int nDef, int nAtkLev, int nDefLev)
        {
            if (nAtkLev > 120)
                nAtkLev = 120;

            int nDamage = nAtk - nDef;

            if (GetNameType(nAtkLev, nDefLev) != NAME_GREEN)
                return Calculations.CutTrail(0, nDamage);

            int nDeltaLev = nAtkLev - nDefLev;
            if (nDeltaLev >= 3
                && nDeltaLev <= 5)
                nAtk = (int)(nAtk * 1.5);
            else if (nDeltaLev > 5
                     && nDeltaLev <= 10)
                nAtk *= 2;
            else if (nDeltaLev > 10
                     && nDeltaLev <= 20)
                nAtk = (int)(nAtk * 2.5);
            else if (nDeltaLev > 20)
                nAtk *= 3;

            return Calculations.CutTrail(0, nAtk - nDef);
        }

        public int CalcDamageMonster2User(int nAtk, int nDef, int nAtkLev, int nDefLev)
        {
            if (nAtkLev > 120)
                nAtkLev = 120;

            int nDamage = nAtk - nDef;

            int nNameType = GetNameType(nAtkLev, nDefLev);

            if (nNameType == NAME_RED)
                nDamage = (int)(nAtk * 1.5f - nDef);
            else if (nNameType == NAME_BLACK)
            {
                int nDeltaLev = nDefLev - nAtkLev;
                if (nDeltaLev >= -10 && nDeltaLev <= -5)
                    nAtk *= 2;
                else if (nDeltaLev >= -20 && nDeltaLev < -10)
                    nAtk = (int)(nAtk * 3.5f);
                else if (nDeltaLev < -20)
                    nAtk *= 5;
                nDamage = nAtk - nDef;
            }

            return Calculations.CutTrail(0, nDamage);
        }

        public int AdjustMinDamageUser2Monster(int nDamage, IRole pAtker, IRole pTarget)
        {
            int nMinDamage = 1;
            nMinDamage += pAtker.Level / 10;

            if (!(pAtker is Character))
                return Calculations.CutTrail(nMinDamage, nDamage);

            Character pUser = pAtker as Character;
            Item pItem;
            if (pUser != null && pUser.Equipment.Items.TryGetValue(ItemPosition.RIGHT_HAND, out pItem))
            {
                nMinDamage += pItem.GetQuality();
            }

            return Calculations.CutTrail(nMinDamage, nDamage);
        }

        public int AdjustMinDamageMonster2User(int nDamage, IRole pAtker, IRole pTarget)
        {
            int nMinDamage = 7;

            if (nDamage >= nMinDamage
                || pTarget.Level <= 15)
                return nDamage;

            if (!(pTarget is Character))
                return Calculations.CutTrail(nMinDamage, nDamage);

            Character pUser = pTarget as Character;

            if (pUser != null)
            {
                foreach (var item in pUser.Equipment.Items.Values)
                {
                    switch (item.Position)
                    {
                        case ItemPosition.NECKLACE:
                        case ItemPosition.HEADWEAR:
                        case ItemPosition.ARMOR:
                            nMinDamage -= item.GetQuality() / 5;
                            break;
                    }
                }
            }

            nMinDamage = Calculations.CutTrail(1, nMinDamage);

            return Calculations.CutTrail(nMinDamage, nDamage);
        }

        public int AdjustExp(int nDamage, int nAtkLev, int nDefLev)
        {
            if (nAtkLev > 120)
                nAtkLev = 120;

            int nExp = nDamage;

            int nNameType = NAME_WHITE;
            int nDeltaLev = nAtkLev - nDefLev;
            if (nNameType == NAME_GREEN)
            {
                if (nDeltaLev >= 3 && nDeltaLev <= 5)
                    nExp = nExp * 70 / 100;
                else if (nDeltaLev > 5
                         && nDeltaLev <= 10)
                    nExp = nExp * 20 / 100;
                else if (nDeltaLev > 10
                         && nDeltaLev <= 20)
                    nExp = nExp * 10 / 100;
                else if (nDeltaLev > 20)
                    nExp = nExp * 5 / 100;
            }
            else if (nNameType == NAME_RED)
            {
                nExp = (int)(nExp * 1.3f);
            }
            else if (nNameType == NAME_BLACK)
            {
                if (nDeltaLev >= -10
                    && nDeltaLev < -5)
                    nExp = (int)(nExp * 1.5f);
                else if (nDeltaLev >= -20
                         && nDeltaLev < -10)
                    nExp = (int)(nExp * 1.8f);
                else if (nDeltaLev < -20)
                    nExp = (int)(nExp * 2.3f);
            }

            return Calculations.CutTrail(0, nExp);
        }

        public const int NAME_GREEN = 0,
            NAME_WHITE = 1,
            NAME_RED = 2,
            NAME_BLACK = 3;

        public void OnTimer()
        {

        }

        public MagicData QueryMagic()
        {
            if (m_pOwner.Magics == null)
                return null;
            return m_pOwner.Magics.QueryMagic();
        }

        public bool NextAttack(int nFightPause)
        {
            return m_tFight.ToNextTime(nFightPause);
        }
    }
}