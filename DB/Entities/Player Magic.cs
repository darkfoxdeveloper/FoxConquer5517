// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Player Magic.cs
// File Created: 2015/08/01 15:25
namespace DB.Entities
{
    public class DbMagic
    {
        public virtual uint Id { get; set; }
        public virtual uint OwnerId { get; set; }
        public virtual ushort Type { get; set; }
        public virtual ushort Level { get; set; }
        public virtual uint Experience { get; set; }
        public virtual byte Unlearn { get; set; }
        public virtual ushort OldLevel { get; set; }
    }
}