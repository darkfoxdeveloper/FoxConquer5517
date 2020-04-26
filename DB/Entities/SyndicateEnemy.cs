// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Syndicate Enemy.cs
// File Created: 2015/08/01 15:47
namespace DB.Entities
{
    public class DbCqSynEnemy
    {
        public virtual uint Id { get; set; }
        public virtual uint Synid { get; set; }
        public virtual string Synname { get; set; }
        public virtual uint Enemyid { get; set; }
        public virtual string Enemyname { get; set; }
    }
}