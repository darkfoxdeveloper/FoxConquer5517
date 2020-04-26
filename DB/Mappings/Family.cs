// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Family.cs
// Last Edit: 2016/12/05 06:26
// Created: 2016/12/05 06:10

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class FamilyMap : ClassMap<DbFamily>
    {
        public FamilyMap()
        {
            Table(TableName.FAMILY);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Not.Nullable().Default("0").Column("id");
            Map(x => x.Name).Not.Nullable().Column("name").Default("NONE");
            Map(x => x.LeaderIdentity).Not.Nullable().Column("leader_id").Default("0");
            Map(x => x.LeaderName).Not.Nullable().Column("leader_name").Default("");
            Map(x => x.Announce).Not.Nullable().Column("announce").Default("");
            Map(x => x.Amount).Not.Nullable().Column("amount").Default("0");
            Map(x => x.Money).Not.Nullable().Column("money").Default("0");
            Map(x => x.DelFlag).Not.Nullable().Column("del_flag").Default("0");
            Map(x => x.Enemy0).Not.Nullable().Column("enemy0").Default("0");
            Map(x => x.Enemy1).Not.Nullable().Column("enemy1").Default("0");
            Map(x => x.Enemy2).Not.Nullable().Column("enemy2").Default("0");
            Map(x => x.Enemy3).Not.Nullable().Column("enemy3").Default("0");
            Map(x => x.Enemy4).Not.Nullable().Column("enemy4").Default("0");
            Map(x => x.Ally0).Not.Nullable().Column("ally0").Default("0");
            Map(x => x.Ally1).Not.Nullable().Column("ally1").Default("0");
            Map(x => x.Ally2).Not.Nullable().Column("ally2").Default("0");
            Map(x => x.Ally3).Not.Nullable().Column("ally3").Default("0");
            Map(x => x.Ally4).Not.Nullable().Column("ally4").Default("0");
            Map(x => x.OccupyDays).Not.Nullable().Column("occupy_days").Default("0");
            Map(x => x.OccupyMap).Not.Nullable().Column("occupy").Default("0");
            Map(x => x.Level).Not.Nullable().Column("level").Default("0");
            Map(x => x.BattlePowerTower).Not.Nullable().Column("bp_tower").Default("0");
        }
    }
}