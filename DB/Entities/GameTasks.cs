// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Tasks.cs
// File Created: 2015/08/01 15:49
namespace DB.Entities
{
    public class DbTask
    {
        public virtual uint Id { get; set; }
        public virtual uint IdNext { get; set; }
        public virtual uint IdNextfail { get; set; }
        public virtual string Itemname1 { get; set; }
        public virtual string Itemname2 { get; set; }
        public virtual uint Money { get; set; }
        public virtual uint Profession { get; set; }
        public virtual uint Sex { get; set; }
        public virtual int MinPk { get; set; }
        public virtual int MaxPk { get; set; }
        public virtual uint Team { get; set; }
        public virtual uint Metempsychosis { get; set; }
        public virtual ushort Query { get; set; }
        public virtual short Marriage { get; set; }
        public virtual ushort ClientActive { get; set; }
    }
}