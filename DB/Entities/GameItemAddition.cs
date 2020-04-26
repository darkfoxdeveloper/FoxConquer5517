// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Item Addition.cs
// File Created: 2015/08/01 15:23
namespace DB.Entities
{
    public class DbItemAddition
    {
        /// <summary>
        /// The unique identity of the item addition.
        /// </summary>
        public virtual uint Id { get; set; }
        /// <summary>
        /// The gear type. Example: 130000 gets items from 130000-130009
        /// </summary>
        public virtual uint TypeId { get; set; }
        /// <summary>
        /// The Plus level. 1 = (+1)
        /// </summary>
        public virtual byte Level { get; set; }
        public virtual ushort Life { get; set; }
        public virtual ushort AttackMax { get; set; }
        public virtual ushort AttackMin { get; set; }
        public virtual ushort Defense { get; set; }
        public virtual ushort MagicAtk { get; set; }
        public virtual ushort MagicDef { get; set; }
        public virtual ushort Dexterity { get; set; }
        public virtual ushort Dodge { get; set; }
    }
}