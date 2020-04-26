// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Syndicate Totem.cs
// File Created: 2015/08/01 15:47
namespace DB.Entities
{
    public class DbSyntotem
    {
        public virtual uint Id { get; set; }
        public virtual uint Synid { get; set; }
        public virtual uint Userid { get; set; }
        public virtual uint Itemid { get; set; }
        public virtual string Username { get; set; }
    }
}