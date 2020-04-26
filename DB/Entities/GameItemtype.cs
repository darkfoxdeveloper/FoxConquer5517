// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Itemtype.cs
// File Created: 2015/08/01 15:24
namespace DB.Entities
{
    public class DbItemtype
    {
        /// <summary>
        /// Item identity.
        /// </summary>
        public virtual uint Type { get; set; }
        public virtual string Name { get; set; }
        public virtual byte ReqProfession { get; set; }
        public virtual byte ReqWeaponskill { get; set; }
        public virtual byte ReqLevel { get; set; }
        public virtual byte ReqSex { get; set; }
        public virtual ushort ReqForce { get; set; }
        public virtual ushort ReqSpeed { get; set; }
        public virtual ushort ReqHealth { get; set; }
        public virtual ushort ReqSoul { get; set; }
        public virtual byte Monopoly { get; set; }
        public virtual ushort Weight { get; set; }
        public virtual uint Price { get; set; }
        public virtual uint IdAction { get; set; }
        public virtual ushort AttackMax { get; set; }
        public virtual ushort AttackMin { get; set; }
        public virtual short Defense { get; set; }
        public virtual short Dexterity { get; set; }
        public virtual short Dodge { get; set; }
        public virtual short Life { get; set; }
        public virtual short Mana { get; set; }
        public virtual ushort Amount { get; set; }
        public virtual ushort AmountLimit { get; set; }
        public virtual uint RequireWeaponType { get; set; } // 2014-12-14
        public virtual byte Ident { get; set; }
        public virtual byte Gem1 { get; set; }
        public virtual byte Gem2 { get; set; }
        public virtual byte Magic1 { get; set; }
        public virtual byte Magic2 { get; set; }
        public virtual byte Magic3 { get; set; }
        public virtual ushort MagicAtk { get; set; }
        public virtual ushort MagicDef { get; set; }
        public virtual ushort AtkRange { get; set; }
        public virtual ushort AtkSpeed { get; set; }
        public virtual byte FrayMode { get; set; }
        public virtual byte RepairMode { get; set; }
        public virtual byte TypeMask { get; set; }
        public virtual uint EmoneyPrice { get; set; }
        public virtual uint BoundEmoneyPrice { get; set; }  // 2014-12-14
        public virtual uint CritStrike { get; set; }
        public virtual uint SkillCritStrike { get; set; }
        public virtual uint Immunity { get; set; }
        public virtual uint Penetration { get; set; }
        public virtual uint Block { get; set; }
        public virtual uint Breakthrough { get; set; }
        public virtual uint Counteraction { get; set; }
        public virtual uint Detoxication { get; set; }
        public virtual ushort StackLimit { get; set; }
        public virtual uint ResistMetal { get; set; }
        public virtual uint ResistWood { get; set; }
        public virtual uint ResistWater { get; set; }
        public virtual uint ResistFire { get; set; }
        public virtual uint ResistEarth { get; set; }
        public virtual byte Phase { get; set; }
        public virtual uint MeteorAmount { get; set; } // 2014-12-14
        public virtual uint HonorPrice { get; set; } // 2014-12-14
        public virtual uint LifeTime { get; set; } // 2014-12-25
    }
}