// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - ArenaQualifier.cs
// Last Edit: 2016/12/29 13:09
// Created: 2016/12/07 19:45

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class ArenaQualifierMap : ClassMap<DbArena>
    {
        public ArenaQualifierMap()
        {
            Table(TableName.ARENA);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Not.Nullable().Default("0").Column("id");
            Map(x => x.PlayerIdentity).Not.Nullable().Default("0").Column("user_id");
            Map(x => x.Name).Not.Nullable().Default("FELIPZAO").Column("name");
            Map(x => x.Lookface).Not.Nullable().Default("0").Column("lookface");
            Map(x => x.Level).Not.Nullable().Default("0").Column("level");
            Map(x => x.Profession).Not.Nullable().Default("0").Column("profession");
            Map(x => x.WinsToday).Not.Nullable().Default("0").Column("win");
            Map(x => x.WinsTotal).Not.Nullable().Default("0").Column("total_win");
            Map(x => x.LossToday).Not.Nullable().Default("0").Column("loss");
            Map(x => x.LossTotal).Not.Nullable().Default("0").Column("total_loss");
            Map(x => x.Points).Not.Nullable().Default("0").Column("points");
            Map(x => x.TotalHonorPoints).Not.Nullable().Default("0").Column("total_honor_points");
            Map(x => x.HonorPoints).Not.Nullable().Default("0").Column("honor_points");
            Map(x => x.LastRank).Not.Nullable().Default("0").Column("last_rank");
            Map(x => x.LastWin).Not.Nullable().Default("0").Column("last_win");
            Map(x => x.LastLoss).Not.Nullable().Default("0").Column("last_loss");
            Map(x => x.LastPoints).Not.Nullable().Default("0").Column("last_points");
            Map(x => x.LockReleaseTime).Not.Nullable().Default("0").Column("lock_release");
        }
    }
}