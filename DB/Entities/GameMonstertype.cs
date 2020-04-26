// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Monstertype.cs
// File Created: 2015/08/01 15:28
namespace DB.Entities
{
    public class DbMonstertype
    {
        public virtual uint Id { get; set; }
        public virtual string Name { get; set; }
        public virtual uint Type { get; set; }
        public virtual ushort Lookface { get; set; }
        public virtual int Life { get; set; }
        public virtual uint Mana { get; set; }
        public virtual int AttackMax { get; set; }
        public virtual int AttackMin { get; set; }
        public virtual int Defence { get; set; }
        public virtual uint Dexterity { get; set; }
        public virtual uint Dodge { get; set; }
        public virtual uint HelmetType { get; set; }
        public virtual uint ArmorType { get; set; }
        public virtual uint WeaponrType { get; set; }
        public virtual uint WeaponlType { get; set; }
        public virtual int AttackRange { get; set; }
        public virtual int ViewRange { get; set; }
        public virtual int EscapeLife { get; set; }
        public virtual int AttackSpeed { get; set; }
        public virtual int MoveSpeed { get; set; }
        public virtual ushort Level { get; set; }
        public virtual uint AttackUser { get; set; }
        public virtual uint DropMoney { get; set; }
        public virtual uint DropItemtype { get; set; }
        public virtual uint SizeAdd { get; set; }
        public virtual uint Action { get; set; }
        public virtual uint RunSpeed { get; set; }
        public virtual byte DropArmet { get; set; }
        public virtual byte DropNecklace { get; set; }
        public virtual byte DropArmor { get; set; }
        public virtual byte DropRing { get; set; }
        public virtual byte DropWeapon { get; set; }
        public virtual byte DropShield { get; set; }
        public virtual byte DropShoes { get; set; }
        public virtual uint DropHp { get; set; }
        public virtual uint DropMp { get; set; }
        public virtual uint MagicType { get; set; }
        public virtual int MagicDef { get; set; }
        public virtual uint MagicHitrate { get; set; }
        public virtual uint AiType { get; set; }
        public virtual uint Defence2 { get; set; }
        public virtual ushort StcType { get; set; }
        public virtual byte AntiMonster { get; set; }
        public virtual byte ExtraBattlelev { get; set; }
        public virtual short ExtraExp { get; set; }
        public virtual short ExtraDamage { get; set; }
        public virtual int WaterAtk { get; set; }
        public virtual int FireAtk { get; set; }
        public virtual int EarthAtk { get; set; }
        public virtual int WoodAtk { get; set; }
        public virtual int MetalAtk { get; set; }
        public virtual byte WaterDef { get; set; }
        public virtual byte FireDef { get; set; }
        public virtual byte EarthDef { get; set; }
        public virtual byte WoodDef { get; set; }
        public virtual byte MetalDef { get; set; }
        public virtual byte Boss { get; set; }
    }
}