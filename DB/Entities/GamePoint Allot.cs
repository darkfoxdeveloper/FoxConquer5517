// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Point Allot.cs
// File Created: 2015/08/01 15:31
namespace DB.Entities
{
    public class DbPointAllot
    {
        public virtual uint Identity { get; set; }
        public virtual byte Profession { get; set; }
        public virtual byte Level { get; set; }
        public virtual ushort Strength { get; set; }
        public virtual ushort Agility { get; set; }
        public virtual ushort Vitality { get; set; }
        public virtual ushort Spirit { get; set; }
    }
}