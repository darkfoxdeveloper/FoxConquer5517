// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - DbFamily.cs
// Last Edit: 2016/12/05 06:09
// Created: 2016/12/05 05:56
namespace DB.Entities
{
    public class DbFamily
    {
        public virtual uint Identity { get; set; }
        public virtual uint CreationDate { get; set; }
        public virtual string Name { get; set; }
        public virtual string LeaderName { get; set; }
        public virtual uint LeaderIdentity { get; set; }
        public virtual ushort Amount { get; set; }
        public virtual string Announce { get; set; }
        public virtual uint Money { get; set; }
        public virtual byte DelFlag { get; set; }
        public virtual uint Ally0 { get; set; }
        public virtual uint Ally1 { get; set; }
        public virtual uint Ally2 { get; set; }
        public virtual uint Ally3 { get; set; }
        public virtual uint Ally4 { get; set; }
        public virtual uint Enemy0 { get; set; }
        public virtual uint Enemy1 { get; set; }
        public virtual uint Enemy2 { get; set; }
        public virtual uint Enemy3 { get; set; }
        public virtual uint Enemy4 { get; set; }
        public virtual uint OccupyDays { get; set; }
        public virtual uint OccupyMap { get; set; }
        public virtual byte Level { get; set; }
        public virtual byte BattlePowerTower { get; set; }
    }
}