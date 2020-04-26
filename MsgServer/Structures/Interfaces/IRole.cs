// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - IRole.cs
// Last Edit: 2016/12/06 14:19
// Created: 2016/11/23 10:29

using System.Drawing;
using Core.Common.Enums;
using MsgServer.Structures.Actions;
using MsgServer.Structures.World;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Interfaces
{
    public interface IRole
    {
        uint Identity { get; }
        string Name { get; set; }
        uint MapIdentity { get; set; }
        ushort MapX { get; set; }
        ushort MapY { get; set; }
        Map Map { get; set; }
        Screen Screen { get; }
        FacingDirection Direction { get; set; }
        StatusSet Status { get; }
        IStatus QueryStatus(Effect0 pEffect0);
        IStatus QueryStatus(Effect1 pEffect0);
        IStatus QueryStatus(int nType);
        BattleSystem BattleSystem { get; }
        GameAction GameAction { get; }
        MagicData Magics { get; }

        bool AppendStatus(StatusInfoStruct pInfo);
        bool AttachStatus(IRole pRole, int nStatus, int nPower, int nSecs, int nTimes, byte pLevel, uint wCaster = 0);
        bool DetachStatus(int nType);
        bool DetachStatus(Effect0 nType);
        bool DetachStatus(ulong nType, bool b64);
        bool DetachWellStatus(IRole pRole);
        bool DetachBadlyStatus(IRole pRole);
        bool DetachAllStatus(IRole pRole);
        bool IsWellStatus0(ulong nStatus);
        bool IsBadlyStatus0(ulong nStatus);

        uint Lookface { get; set; }

        byte Level { get; set; }
        int BattlePower { get; }

        int MinAttack { get; }
        int MaxAttack { get; }
        int MagicAttack { get; }
        int Dodge { get; }
        int AttackHitRate { get; }
        int Dexterity { get; }
        int Defense { get; }
        int MagicDefense { get; }
        int MagicDefenseBonus { get; }
        int AddFinalAttack { get; }
        int AddFinalMagicAttack { get; }
        int AddFinalDefense { get; }
        int AddFinalMagicDefense { get; }
        uint Life { get; set; }
        uint MaxLife { get; set; }
        ushort Mana { get; set; }
        ushort MaxMana { get; set; }
        uint CriticalStrike { get; }
        uint SkillCriticalStrike { get; }
        uint Breakthrough { get; }
        uint Penetration { get; }
        uint Immunity { get; }
        uint Counteraction { get; }
        uint Block { get; }
        uint Detoxication { get; }
        uint FireResistance { get; }
        uint WaterResistance { get; }
        uint EarthResistance { get; }
        uint WoodResistance { get; }
        uint MetalResistance { get; }

        ulong Flag1 { get; set; }
        ulong Flag2 { get; set; }

        bool AdditionMagic(int nLifeLost, int nDamage);
        bool AddAttribute(ClientUpdateType type, long value, bool bSynchro);
        bool IsPlayer();
        bool IsMonster();
        bool IsNpc();
        bool IsCallPet();
        bool IsDynaNpc();
        bool IsDynaMonster();
        bool IsBeAttackable();
        bool IsAttackable(IRole pTarget);
        bool IsFarWeapon();
        bool AutoSkillAttack(IRole pTarget);
        bool IsWing();
        bool IsBowman();
        bool IsSimpleMagicAtk();
        bool IsGoal();
        bool IsEvil();
        bool BeAttack(int bMagic, IRole pRole, int nPower, bool bReflectEnable);
        bool SetAttackTarget(IRole pTarget);
        bool IsImmunity(IRole pRole);
        bool CheckCrime(IRole pRole);
        bool IsBlinking();
        bool IsAlive { get; }
        ushort Profession { get; set; }
        byte Stamina { get; set; }
        //MagicData Magics { get; }

        void BeKill(IRole pRole);
        void Kill(IRole pTarget, ulong dwDieWay);
        void SendDamageMsg(uint idTarget, uint nDamage, InteractionEffect special);
        void AwardBattleExp(int nExp, bool bGemEffect);
        bool IsInFan(Point pos, Point posSource, int nRange, int nWidth, Point posCenter);

        int AdjustDefense(int nRawDef);
        int AdjustWeaponDamage(int nDamage);
        int GetAttackRange(int nTargetSizeAdd);
        int GetDistance(IScreenObject pObj);
        int Attack(IRole pTarget, ref InteractionEffect especial);//, ref List<InteractionEffect> pEffects);
        int AdjustExperience(IRole pTarget, int nRawExp, bool bNewbieBonusMsg);
        int CalculateFightRate();
        int GetExpGemEffect();
        int GetAtkGemEffect();
        int GetMAtkGemEffect();
        int GetSkillGemEffect();
        int GetTortoiseGemEffect();
        void Send(byte[] pMsg);
        int GetSizeAdd();
        float GetReduceDamage();
        int AdjustMagicDamage(int nDamage);
        bool CheckWeaponSubType(uint useItem, uint useItemNum = 0);
        int GetDistance(ushort x, ushort y);
        bool SpendEquipItem(uint useItem, uint useItemNum, bool bSynchro);
        bool DecEquipmentDurability(bool bAttack, int hitByMagic, ushort useItemNum);
        bool ProcessMagicAttack(ushort usMagicType, uint idTarget, ushort x, ushort y, byte ucAutoActive = 0);
    }
}