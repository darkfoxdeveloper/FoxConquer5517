// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Dynamic Map Rec Map.cs
// Last Edit: 2016/12/15 10:28
// Created: 2016/12/15 10:28

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class DynamicRankRecMap : ClassMap<DbDynamicRankRecord>
    {
        public DynamicRankRecMap()
        {
            Table(TableName.DYNAMICRANKREC);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.RankType).Column("rank_type").Default("0").Not.Nullable();
            Map(x => x.ObjectIdentity).Column("object_id").Default("0").Not.Nullable();
            Map(x => x.ObjectName).Column("object_name").Default("UNDEFINED").Not.Nullable();
            Map(x => x.PlayerIdentity).Column("player_id").Default("0").Not.Nullable();
            Map(x => x.PlayerName).Column("player_name").Default("UNDEFINED").Not.Nullable();
            Map(x => x.Value1).Column("value1").Default("0").Not.Nullable();
            Map(x => x.Value2).Column("value2").Default("0").Not.Nullable();
            Map(x => x.Value3).Column("value3").Default("0").Not.Nullable();
            Map(x => x.Value4).Column("value4").Default("0").Not.Nullable();
            Map(x => x.Value5).Column("value5").Default("0").Not.Nullable();
            Map(x => x.Value6).Column("value6").Default("0").Not.Nullable();
            Map(x => x.Value7).Column("value7").Default("0").Not.Nullable();
        }
    }
}