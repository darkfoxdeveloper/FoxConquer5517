// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Player Subclass.cs
// File Created: 2015/08/03 12:52
namespace DB.Entities
{
    public class DbSubclass
    {
        public virtual uint Id { get; set; }
        public virtual uint Userid { get; set; }
        public virtual byte Class { get; set; }
        public virtual byte Promotion { get; set; }
        public virtual byte Level { get; set; }
    }
}