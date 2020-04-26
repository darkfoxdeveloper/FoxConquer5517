// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - DB - Carry.cs
// Last Edit: 2017/02/06 08:25
// Created: 2017/02/06 08:04

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CarryMap : ClassMap<DbCarry>
    {
        public CarryMap()
        {
            Table(TableName.CARRY);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Increment().Not.Nullable().Column("id");
            Map(x => x.ItemIdentity).Default("0").Column("item_id").Not.Nullable();
            Map(x => x.Name).Default("").Column("name").Not.Nullable();
            Map(x => x.RecordMapId).Default("0").Column("recordmapid").Not.Nullable();
            Map(x => x.RecordMapX).Default("0").Column("record_x").Not.Nullable();
            Map(x => x.RecordMapY).Default("0").Column("record_y").Not.Nullable();
        }
    }
}