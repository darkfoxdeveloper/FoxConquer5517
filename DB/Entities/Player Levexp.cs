// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Player Levexp.cs
// File Created: 2015/08/01 15:25
namespace DB.Entities
{
    public class DbLevexp
    {
        public virtual byte Level { get; set; }
        public virtual ulong Exp { get; set; }
        public virtual int UpLevTime { get; set; }
    }
}