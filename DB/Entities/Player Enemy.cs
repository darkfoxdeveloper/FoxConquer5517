// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Player Enemy.cs
// File Created: 2015/08/01 12:59
namespace DB.Entities
{
    public class DbEnemy
    {
        public virtual uint Identity { get; set; }
        public virtual uint UserIdentity { get; set; }
        public virtual uint EnemyIdentity { get; set; }
        public virtual string Enemyname { get; set; }
        public virtual uint Time { get; set; }
    }
}