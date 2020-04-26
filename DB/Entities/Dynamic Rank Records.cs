// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Dynamic Rank Records.cs
// Last Edit: 2016/12/15 10:27
// Created: 2016/12/15 10:25
namespace DB.Entities
{
    public class DbDynamicRankRecord
    {
        public virtual uint Identity { get; set; }
        public virtual uint RankType { get; set; }
        public virtual uint ObjectIdentity { get; set; }
        public virtual string ObjectName { get; set; }
        public virtual uint PlayerIdentity { get; set; }
        public virtual string PlayerName { get; set; }
        public virtual long Value1 { get; set; }
        public virtual long Value2 { get; set; }
        public virtual long Value3 { get; set; }
        public virtual long Value4 { get; set; }
        public virtual long Value5 { get; set; }
        public virtual long Value6 { get; set; }
        public virtual long Value7 { get; set; }
    }
}