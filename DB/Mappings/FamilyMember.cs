// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - FamilyMember.cs
// Last Edit: 2016/12/05 06:32
// Created: 2016/12/05 06:28

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class FamilyMemberMap : ClassMap<DbFamilyMember>
    {
        public FamilyMemberMap()
        {
            Table(TableName.FAMILY_ATTR);
            LazyLoad();
            Id(x => x.Identity).Column("id").Not.Nullable().Default("0");
            Map(x => x.FamilyIdentity).Column("family_id").Not.Nullable().Default("0");
            Map(x => x.Position).Column("rank").Not.Nullable().Default("0");
            Map(x => x.Money).Column("money").Not.Nullable().Default("0");
            Map(x => x.JoinDate).Column("join_date").Not.Nullable().Default("0");
        }
    }
}