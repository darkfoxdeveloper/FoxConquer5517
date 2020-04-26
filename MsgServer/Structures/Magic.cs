// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Magic.cs
// Last Edit: 2016/12/06 13:58
// Created: 2016/12/06 13:57

using System.Linq;
using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public class Magic
    {
        private IRole m_pOwner;
        private MsgMagicInfo m_pPacket;
        private DbMagic m_dbMagic;
        private DbMagictype m_dbMagictype;
        private TimeOutMS m_tDelay;

        private byte m_pMaxLevel = 0;

        public Magic(IRole pOwner)
        {
            m_pOwner = pOwner;
        }

        public bool Create(uint idMgc)
        {
            return Create(idMgc, 0);
        }

        public bool Create(uint idMgc, ushort level)
        {
            m_dbMagictype = ServerKernel.Magictype.Values.FirstOrDefault(x => x.Type == idMgc && x.Level == level);
            if (m_dbMagictype == null)
            {
                ServerKernel.Log.GmLog("magic_fail", string.Format("Skill not existent for creation (type:{0}, level:{1}, player: {2})", idMgc, 0, m_pOwner.Identity));
                return false;
            }

            if (m_pOwner.Magics.CheckType((ushort)idMgc))
                return false;

            m_dbMagic = new DbMagic
            {
                OwnerId = m_pOwner.Identity,
                Type = (ushort)idMgc,
                Level = level
            };

            m_pPacket = new MsgMagicInfo(0, level, (ushort)idMgc);
            GetSetMaxLevel();
            if (m_pOwner is Character)
            {
                Save();
                SendSkill();
            }
            SetDelay();
            return true;
        }

        public bool Create(DbMagic pMgc)
        {
            m_dbMagictype = ServerKernel.Magictype.Values.FirstOrDefault(x => x.Type == pMgc.Type && x.Level == pMgc.Level);
            if (m_dbMagictype == null)
            {
                ServerKernel.Log.GmLog("magic_fail", string.Format("Skill not existent (type:{0}, level:{1}, player: {2})", pMgc.Type, pMgc.Level, m_pOwner.Identity));
                return false;
            }
            m_dbMagic = pMgc;
            m_pPacket = new MsgMagicInfo(pMgc.Experience, pMgc.Level, pMgc.Type);
            GetSetMaxLevel();
            SetDelay();
            return true;
        }

        public string Name
        {
            get { return m_dbMagictype.Name; }
        }

        public ushort Type
        {
            get { return m_dbMagic.Type; }
        }

        public ushort Level
        {
            get { return m_dbMagic.Level; }
            set
            {
                if (value > m_pMaxLevel)
                    return;
                m_dbMagic.Level = value;
                if (m_pOwner is Character)
                {
                    Save();
                    m_pPacket.Level = value;
                    SendSkill();

                    if (NextMagic != 0)
                    {
                        foreach (var magic in ServerKernel.Magictype.Values)
                        {
                            if (magic.Type == NextMagic)
                            {
                                m_dbMagictype = magic;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var magic in ServerKernel.Magictype.Values)
                        {
                            if (magic.Type == Type && magic.LearnLevel == value)
                            {
                                m_dbMagictype = magic;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public uint Experience
        {
            get { return m_dbMagic.Experience; }
            set
            {
                if (m_pOwner is Character)
                {
                    Save();
                    m_dbMagic.Experience = m_pPacket.Experience = value;
                    SendSkill();
                }
            }
        }

        public ushort OldLevel
        {
            get { return m_dbMagic.OldLevel; }
            set
            {
                if (m_dbMagic.OldLevel <= m_dbMagic.Level)
                    return;
                m_dbMagic.OldLevel = value;
                if (m_pOwner is Character)
                {
                    Save();
                }
            }
        }

        public byte AutoActive
        {
            get { return m_dbMagictype.AutoActive; }
        }

        public uint Sort
        {
            get { return m_dbMagictype.Sort; }
        }

        public ElementType ElementType
        {
            get { return (ElementType) m_dbMagictype.ElementType; }
        }

        public uint Percent
        {
            get { return m_dbMagictype.Percent; }
        }

        public byte Ground
        {
            get { return m_dbMagictype.Ground; }
        }

        public byte Crime
        {
            get { return m_dbMagictype.Crime; }
        }

        public ushort UseMana
        {
            get { return (ushort)m_dbMagictype.UseMp; }
        }

        public ushort UseStamina
        {
            get { return (ushort)m_dbMagictype.UseEp; }
        }

        public ushort UseXp
        {
            get { return m_dbMagictype.UseXp; }
        }

        public uint UseItem
        {
            get { return m_dbMagictype.UseItem; }
        }

        public uint UseItemNum
        {
            get { return m_dbMagictype.UseItemNum; }
        }

        public uint WeaponSubtype
        {
            get { return m_dbMagictype.WeaponSubtype; }
        }

        public uint WeaponSubtypeNum
        {
            get { return m_dbMagictype.WeaponSubtypeNum; }
        }

        public long Status
        {
            get { return m_dbMagictype.Status; }
        }

        public uint Distance
        {
            get { return m_dbMagictype.Distance; }
        }

        public uint Intone
        {
            get { return m_dbMagictype.IntoneSpeed; }
        }

        public uint NextMagic
        {
            get { return m_dbMagictype.NextMagic; }
        }

        public int WeaponHit
        {
            get { return m_dbMagictype.WeaponHit; }
        }

        public uint NeedProf
        {
            get { return m_dbMagictype.NeedProf; }
        }

        public int NeedExp
        {
            get { return m_dbMagictype.NeedExp; }
        }

        public uint NeedLevel
        {
            get { return m_dbMagictype.NeedLevel; }
        }

        public uint ActiveTimes
        {
            get { return m_dbMagictype.ActiveTimes; }
        }

        public uint FloorAttr
        {
            get { return m_dbMagictype.FloorAttr; }
        }

        public int Power
        {
            get { return m_dbMagictype.Power; }
        }

        public uint ElementPower
        {
            get { return m_dbMagictype.ElementPower; }
        }

        public uint Range
        {
            get { return m_dbMagictype.Range; }
        }

        public int Multi
        {
            get { return m_dbMagictype.Multi; }
        }

        public uint Target
        {
            get { return m_dbMagictype.Target; }
        }

        public uint StepSecs
        {
            get { return m_dbMagictype.StepSecs; }
        }

        public uint Timeout
        {
            get { return m_dbMagictype.DelayMs > 0 ? m_dbMagictype.DelayMs : 400; }
        }

        public bool Delay()
        {
            if (m_tDelay == null)
            {
                m_tDelay = new TimeOutMS(0);
                SetDelay();
            }
            return m_tDelay.ToNextTime();
        }

        public void GetSetMaxLevel()
        {
            DbMagictype temp =
                ServerKernel.Magictype.Values.OrderByDescending(x => x.Level).FirstOrDefault(x => x.Type == m_dbMagic.Type);
            if (temp != null)
                m_pMaxLevel = (byte)temp.Level;
            else
                m_pMaxLevel = (byte)m_dbMagictype.Level;
        }

        #region Battle Related

        public bool IsWeaponMagic() { return Type >= 10000 && Type < 10256; }
        public bool IsWeaponMagic(int nType) { return nType >= 10000 && nType < 10256; }

        public int GetApplyMs() { return (int)m_dbMagictype.StepSecs; }

        public void SetDelay()
        {
            if (m_dbMagictype == null)
                return;
            if (m_tDelay == null)
                m_tDelay = new TimeOutMS((int)Timeout);

            m_tDelay.Startup((int)Timeout);
            m_tDelay.SetInterval((int)Timeout);
            m_tDelay.Update();
        }

        #endregion

        public bool Save()
        {
            return m_pOwner is Character && Database.Magics.SaveOrUpdate(m_dbMagic);
        }

        public bool Delete()
        {
            return m_pOwner is Character && Database.Magics.Delete(m_dbMagic);
        }

        public void SendSkill()
        {
            if (m_pOwner is Character)
                m_pOwner.Send(m_pPacket);
        }

        public bool IsReady()
        {
            return m_tDelay.IsTimeOut();
        }

        public int GetLockSecs()
        {
            return (GetApplyMs() / 1000 + 1);
        }

        public void StartDelay()
        {
            m_tDelay.Update();
        }

        public bool ToNextTime()
        {
            return m_tDelay.ToNextTime();
        }
    }
}
