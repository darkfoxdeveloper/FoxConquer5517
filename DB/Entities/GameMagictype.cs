// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Magictype.cs
// File Created: 2015/08/01 15:26
namespace DB.Entities
{
    public class DbMagictype
    {
        public virtual uint Id { get; set; }
        public virtual uint Type { get; set; }
        public virtual uint Sort { get; set; }
        public virtual string Name { get; set; }
        public virtual byte Crime { get; set; }
        public virtual byte Ground { get; set; }
        public virtual byte Multi { get; set; }
        public virtual uint Target { get; set; }
        public virtual uint Level { get; set; }
        public virtual uint UseMp { get; set; }
        public virtual int Power { get; set; }
        public virtual uint IntoneSpeed { get; set; }
        public virtual uint Percent { get; set; }
        public virtual uint StepSecs { get; set; }
        public virtual uint Range { get; set; }
        public virtual uint Distance { get; set; }
        public virtual long Status { get; set; }
        public virtual uint NeedProf { get; set; }
        public virtual int NeedExp { get; set; }
        public virtual uint NeedLevel { get; set; }
        public virtual byte UseXp { get; set; }
        public virtual uint WeaponSubtype { get; set; }
        public virtual uint ActiveTimes { get; set; }
        public virtual byte AutoActive { get; set; }
        public virtual uint FloorAttr { get; set; }
        public virtual byte AutoLearn { get; set; }
        public virtual uint LearnLevel { get; set; }
        public virtual byte DropWeapon { get; set; }
        public virtual uint UseEp { get; set; }
        public virtual byte WeaponHit { get; set; }
        public virtual uint UseItem { get; set; }
        public virtual uint NextMagic { get; set; }
        public virtual uint DelayMs { get; set; }
        public virtual uint UseItemNum { get; set; }
        public virtual byte WeaponSubtypeNum { get; set; } // 2015-02-12
        public virtual byte ElementType { get; set; } // 2016-12-11
        public virtual uint ElementPower { get; set; } // 2016-12-12
    }
}