// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Entity Status.cs
// Last Edit: 2016/11/23 10:32
// Created: 2016/11/23 10:31

using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Core.Common.Enums;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi, Size = 16)]
    public struct StatusInfoStruct
    {
        public int Status;
        public int Power;
        public int Seconds;
        public int Times;

        public StatusInfoStruct(int nStatus, int nPower, int nSecs, int nTimes)
            : this()
        {
            Status = nStatus;
            Power = nPower;
            Seconds = nSecs;
            Times = nTimes;
        }
    }

    public sealed class StatusOnce : IStatus, IOnTimer
    {
        private IRole m_pOwner;
        private TimeOutMS m_tKeep;
        private long m_dAutoFlash;
        private int m_nData;
        private int m_nStatus;
        private TimeOutMS m_tInterval;
        private uint m_dwCaster;
        private byte m_pLevel;

        public StatusOnce()
        {
            m_pOwner = null;
            m_nStatus = 0;
        }

        public StatusOnce(IRole pOwner)
        {
            m_pOwner = pOwner;
            m_nStatus = 0;
        }
        
        public bool Create(IRole pRole, int nStatus, int nPower, int nSecs, int nTimes, uint caster = 0, byte level = 0)
        {
            m_pOwner = pRole;
            m_dwCaster = caster;
            m_nStatus = nStatus;
            m_nData = nPower;
            m_tKeep = new TimeOutMS(nSecs * 1000);
            m_tKeep.Startup((int)Math.Min(((long)nSecs * 1000), int.MaxValue));
            m_tKeep.Update();
            m_tInterval = new TimeOutMS(1000);
            m_tInterval.Update();
            m_pLevel = level;
            return true;
        }

        public int Identity { get { return m_nStatus; } }

        public bool IsValid
        {
            get { return m_tKeep.IsActive() && !m_tKeep.IsTimeOut(); }
        }

        public int Power
        {
            get { return m_nData; }
            set { m_nData = value; }
        }

        public byte Level
        {
            get { return m_pLevel; }
        }

        public int Time
        {
            get { return m_tKeep.GetInterval(); }
        }

        public bool GetInfo(ref StatusInfoStruct pInfo)
        {
            pInfo.Power = m_nData;
            pInfo.Seconds = m_tKeep.GetRemain() / 1000;
            pInfo.Status = m_nStatus;
            pInfo.Times = 0;

            return IsValid;
        }

        public bool ChangeData(int nPower, int nSecs, int nTimes = 0, uint wCaster = 0)
        {
            try
            {
                m_nData = nPower;
                m_tKeep.SetInterval(nSecs * 1000);
                m_tKeep.Update();

                m_dwCaster = wCaster;
                if (m_pOwner is Character)
                {
                    Character pUser = m_pOwner as Character;

                    if (Identity == FlagInt.AZURE_SHIELD)
                    {
                        pUser.UpdateAzureShield(nSecs, nPower, Level);
                    }

                    pUser.RecalculateAttributes();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IncTime(int nMilliSecs, int nLimit)
        {
            int nInterval = Math.Min(nMilliSecs + m_tKeep.GetRemain(), nLimit);
            m_tKeep.SetInterval(nInterval);
            return m_tKeep.Update();
        }

        public bool ToFlash()
        {
            if (!IsValid)
                return false;

            if (m_dAutoFlash == 0 && m_tKeep.GetRemain() <= 5000)
            {
                m_dAutoFlash = 1;
                return true;
            }
            return false;
        }

        public uint CasterId
        {
            get { return m_dwCaster; }
        }

        public bool IsUserCast
        {
            get { return m_dwCaster == m_pOwner.Identity || m_dwCaster == 0; }
        }

        public void OnTimer()
        {

        }
    }

    public sealed class StatusMore : IStatus, IOnTimer
    {
        private IRole m_pOwner;
        private TimeOut m_tKeep;
        private long m_dAutoFlash;
        private int m_nData;
        private int m_nStatus;
        private int m_nTimes;
        private uint m_dwCaster;
        private byte m_pLevel;

        public StatusMore()
        {
            m_pOwner = null;
            m_nStatus = 0;
        }

        public StatusMore(IRole pOwner)
        {
            m_pOwner = pOwner;
            m_nStatus = 0;
        }

        ~StatusMore()
        {
            // todo destroy and detach status
        }

        public bool Create(IRole pRole, int nStatus, int nPower, int nSecs, int nTimes, uint wCaster = 0, byte level = 0)
        {
            m_pOwner = pRole;
            m_nStatus = nStatus;
            m_nData = nPower;
            m_tKeep = new TimeOut(nSecs);
            m_nTimes = nTimes;
            m_dwCaster = wCaster;
            m_pLevel = level;
            return true;
        }

        public int Identity
        {
            get { return m_nStatus; }
        }

        public bool IsValid
        {
            get { return m_nTimes > 0; }
        }

        public int Power
        {
            get { return m_nData; }
            set { m_nData = value; }
        }

        public byte Level
        {
            get { return m_pLevel; }
        }

        public int Time
        {
            get { return m_tKeep.GetInterval(); }
        }

        public bool GetInfo(ref StatusInfoStruct pInfo)
        {
            pInfo.Power = m_nData;
            pInfo.Seconds = m_tKeep.GetRemain();
            pInfo.Status = m_nStatus;
            pInfo.Times = m_nTimes;

            return IsValid;
        }

        public bool ChangeData(int nPower, int nSecs, int nTimes = 0, uint wCaster = 0)
        {
            try
            {
                m_nData = nPower;
                m_tKeep.SetInterval(nSecs);
                m_tKeep.Update();

                m_dwCaster = wCaster;
                if (m_pOwner is Character)
                {
                    // todo recalculate attributes
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IncTime(int nMilliSecs, int nLimit)
        {
            int nInterval = Math.Min(nMilliSecs + m_tKeep.GetRemain(), nLimit);
            m_tKeep.SetInterval(nInterval);
            return m_tKeep.Update();
        }

        public bool ToFlash()
        {
            if (!IsValid)
                return false;

            if (m_dAutoFlash == 0 && m_tKeep.GetRemain() <= 5000)
            {
                m_dAutoFlash = 1;
                return true;
            }
            return false;
        }

        public uint CasterId
        {
            get { return m_dwCaster; }
        }

        public bool IsUserCast
        {
            get { return m_dwCaster == m_pOwner.Identity || m_dwCaster == 0; }
        }

        public void OnTimer()
        {
            try
            {
                if (!IsValid || !m_tKeep.ToNextTime())
                    return;

                if (m_pOwner != null)
                {
                    int nLoseLife;

                    switch (m_nStatus)
                    {
                        case FlagInt.POISONED: // poison
                            if (!m_pOwner.IsAlive)
                                return;

                            nLoseLife = (int)Calculations.CutOverflow(m_nData, m_pOwner.Life - 1);
                            m_pOwner.AddAttribute(ClientUpdateType.HITPOINTS, -1 * nLoseLife, true);

                            var msg2 = new MsgMagicEffect
                            {
                                Identity = m_pOwner.Identity,
                                SkillIdentity = 10010
                            };
                            msg2.AppendTarget(m_pOwner.Identity, (uint)nLoseLife, true, 0, 0);
                            m_pOwner.Map.SendToRange(msg2, m_pOwner.MapX, m_pOwner.MapY);

                            if (!m_pOwner.IsAlive)
                                m_pOwner.BeKill(null);
                            break;
                        case FlagInt.VORTEX: // shuriken vortex
                            if (!m_pOwner.IsAlive)
                                return;

                            if (m_pOwner is Character)
                                (m_pOwner as Character).ProcessMagicAttack(6010, 0, m_pOwner.MapX, m_pOwner.MapY);
                            break;
                        case FlagInt.TOXIC_FOG: // toxic fog
                            if (!m_pOwner.IsAlive || m_pOwner.Life <= 1)
                                return;

                            var power = (m_nData > 30000 ? ((m_nData - 30000) / 100f) : m_nData);
                            nLoseLife = (int)Calculations.CutOverflow((int)(m_pOwner.Life * power), m_pOwner.Life - 1);

                            if (m_pOwner.Detoxication > 0)
                            {
                                uint detox = m_pOwner.Detoxication;
                                if (m_pOwner.Detoxication > 100)
                                    detox = 100;
                                nLoseLife = (int)
                                        Calculations.MulDiv(nLoseLife, Math.Min(100 - detox, 100), 100);
                            }

                            m_pOwner.BeAttack(1, m_pOwner, nLoseLife, false);

                            var msg = new MsgMagicEffect
                            {
                                Identity = m_pOwner.Identity,
                                SkillIdentity = 10010
                            };
                            msg.AppendTarget(m_pOwner.Identity, (uint)nLoseLife, true, 0, 0);
                            m_pOwner.Map.SendToRange(msg, m_pOwner.MapX, m_pOwner.MapY);
                            break;
                    }
                    m_nTimes--;
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("StatusOnce::OnTimer() error!", false, LogType.EXCEPTION);
            }
        }
    }

    public sealed class StatusSet
    {
        private IRole m_pOwner;
        public ConcurrentDictionary<int, IStatus> Status;

        private ulong Status0
        {
            get { return m_pOwner.Flag1; }
            set { m_pOwner.Flag1 = value; }
        }

        private ulong Status1
        {
            get { return m_pOwner.Flag2; }
            set { m_pOwner.Flag2 = value; }
        }

        public StatusSet(IRole pRole)
        {
            if (pRole == null)
                return;
            m_pOwner = pRole;
            Status = new ConcurrentDictionary<int, IStatus>(5, 128);
        }

        public int GetAmount()
        {
            return Status.Count;
        }

        public IStatus GetObjByIndex(int nKey)
        {
            IStatus ret;
            return Status.TryGetValue(nKey, out ret) ? ret : null;
        }

        public IStatus GetObj(ulong nKey, bool b64 = false)
        {
            IStatus ret;
            return Status.TryGetValue(InvertFlag(nKey, b64), out ret) ? ret : null;
        }

        public bool AddObj(IStatus pStatus)
        {
            var pInfo = new StatusInfoStruct();
            pStatus.GetInfo(ref pInfo);
            if (Status.ContainsKey(pInfo.Status))
                return false; // status already exists

            if (pInfo.Status < 64)
            {
                ulong flag = 1UL << (pInfo.Status - 1);
                Status.TryAdd(pInfo.Status, pStatus);
                Status0 |= flag;
            }
            else if (pInfo.Status < 128)
            {
                ulong flag = 1UL << (pInfo.Status - 1);
                Status.TryAdd(pInfo.Status, pStatus);
                Status1 |= flag;
            }

            switch (pStatus.Identity)
            {
                case FlagInt.TYRANT_AURA:
                case FlagInt.FEND_AURA:
                case FlagInt.WATER_AURA:
                case FlagInt.FIRE_AURA:
                case FlagInt.METAL_AURA:
                case FlagInt.WOOD_AURA:
                case FlagInt.EARTH_AURA:
                {
                    if (m_pOwner is Character && (m_pOwner as Character).Team != null &&
                        pStatus.CasterId == m_pOwner.Identity)
                    {
                        (m_pOwner as Character).Team.AddAura(pStatus);
                    }
                    break;
                }
            }

            if (pStatus.Identity == FlagInt.SHACKLED && m_pOwner is Character)
            {
                (m_pOwner as Character).UpdateSoulShackle(pInfo.Seconds);
            } 
            else if (pStatus.Identity == FlagInt.AZURE_SHIELD && m_pOwner is Character)
            {
                (m_pOwner as Character).UpdateAzureShield(pInfo.Seconds, pInfo.Power, pStatus.Level);
            }

            var pUser = m_pOwner as Character;
            if (pUser != null)
                pUser.RecalculateAttributes();
            return true;
        }

        public bool DelObj(int nFlag)
        {
            if (nFlag > FlagInt.UNKNOWN128)
                return false;

            IStatus trash;
            if (!Status.TryRemove(nFlag, out trash))
                return false;

            ulong uFlag = 1UL << (nFlag - 1);
            if (nFlag < 64)
                Status0 &= ~uFlag;
            else
                Status1 &= ~uFlag;
            
            switch (nFlag)
            {
                case FlagInt.AZURE_SHIELD:
                {
                    var pStts = new StatusOnce(m_pOwner);
                    pStts.Create(m_pOwner, FlagInt.AZURE_SHIELD_FADE, 0, 3, 0, m_pOwner.Identity, 0);
                    AddObj(pStts);
                    break;
                }
                case FlagInt.TYRANT_AURA:
                case FlagInt.FEND_AURA:
                case FlagInt.WATER_AURA:
                case FlagInt.FIRE_AURA:
                case FlagInt.METAL_AURA:
                case FlagInt.WOOD_AURA:
                case FlagInt.EARTH_AURA:
                {
                    if (m_pOwner is Character && (m_pOwner as Character).Team != null && trash.CasterId == m_pOwner.Identity)
                    {
                        (m_pOwner as Character).Team.RemoveAura(trash);
                    }
                    break;
                }
                case FlagInt.SHACKLED:
                {
                    (m_pOwner as Character).UpdateSoulShackle(0);
                    break;
                }
            }

            var pUser = m_pOwner as Character;
            if (pUser != null)
                pUser.RecalculateAttributes();
            return true;
        }

        public bool DelObj(Effect0 pFlag)
        {
            return DelObj(InvertFlag((ulong)pFlag));
        }

        public bool DelObj(Effect1 pFlag)
        {
            return DelObj(InvertFlag((ulong)pFlag, true));
        }

        public IStatus this[int nKey]
        {
            get
            {
                try
                {
                    IStatus ret;
                    return Status.TryGetValue(nKey, out ret) ? ret : null;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gotta check if there is a faster way to do this.
        /// </summary>
        /// <param name="flag">The flag that will be checked.</param>
        /// <param name="b64">If it's a effect 2 flag, you should set this true.</param>
        /// <returns></returns>
        public static int InvertFlag(ulong flag, bool b64 = false)
        {
            var inv = flag >> 0;
            int ret = -1;
            for (int i = 0; inv > 1; i++)
            {
                inv = flag >> i;
                ret++;
            }
            return !b64 ? (int)ret : (int)(ret + 64);
        }

        public void SendAllStatus()
        {
            Character pUsr = m_pOwner as Character;
            if (pUsr == null || !pUsr.LoginComplete) return;
            pUsr.UpdateClient(ClientUpdateType.STATUS_FLAG, Status0, true);
        }
    }
}