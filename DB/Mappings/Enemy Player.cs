// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Enemy Player.cs
// File Created: 2015/08/03 12:25

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqEnemyMap : ClassMap<DbEnemy>
    {
        public CqEnemyMap()
        {
            Table(TableName.ENEMY);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.UserIdentity).Column("userid").Not.Nullable();
            Map(x => x.EnemyIdentity).Column("enemy").Not.Nullable();
            Map(x => x.Enemyname).Column("enemyname").Not.Nullable();
            Map(x => x.Time).Column("time").Not.Nullable();
        }
    }
}