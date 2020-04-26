// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Refinery.cs
// File Created: 2015/08/01 15:40
namespace DB.Entities
{
    public class DbRefinery
    {
        public virtual uint Id { get; set; }
        public virtual uint Type { get; set; }
        public virtual uint Itemtype { get; set; }
        public virtual uint Level { get; set; }
        public virtual uint Power { get; set; }
    }
}