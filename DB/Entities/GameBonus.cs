// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Bonus.cs
// File Created: 2015/07/31 20:39
namespace DB.Entities
{
    public class DbGameBonus
    {
        public virtual uint Identity { get; set; }
        public virtual uint Action { get; set; }
        public virtual uint AccountIdentity { get; set; }
        public virtual byte Flag { get; set; }
        public virtual ushort ReferenceCode { get; set; }
        public virtual int Time { get; set; }
    }
}
