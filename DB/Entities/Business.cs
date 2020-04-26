// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Business.cs
// File Created: 2015/07/31 20:42
namespace DB.Entities
{
    public class DbBusiness
    {
        public virtual uint Id { get; set; }
        public virtual uint Userid { get; set; }
        public virtual uint Business { get; set; }
        public virtual string Name { get; set; }
        public virtual uint Date { get; set; }
    }
}
