// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Player Title.cs
// File Created: 2015/08/01 15:49
namespace DB.Entities
{
    public class DbTitle
    {
        public virtual uint Id { get; set; }
        public virtual uint Userid { get; set; }
        public virtual byte Title { get; set; }
        public virtual uint Timestamp { get; set; }
    }
}