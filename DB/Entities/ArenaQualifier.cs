// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - ArenaQualifier.cs
// Last Edit: 2016/12/29 13:09
// Created: 2016/12/07 18:04
namespace DB.Entities
{
    public class DbArena
    {
        public virtual uint Identity { get; set; }
        public virtual uint PlayerIdentity { get; set; }
        public virtual string Name { get; set; }
        public virtual uint Lookface { get; set; }
        public virtual byte Level { get; set; }
        public virtual ushort Profession { get; set; }
        public virtual uint WinsToday { get; set; }
        public virtual uint WinsTotal { get; set; }
        public virtual uint LossToday { get; set; }
        public virtual uint LossTotal { get; set; }
        public virtual uint Points { get; set; }
        public virtual uint HonorPoints { get; set; }
        public virtual uint TotalHonorPoints { get; set; }
        public virtual uint LastRank { get; set; }
        public virtual uint LastWin { get; set; }
        public virtual uint LastLoss { get; set; }
        public virtual uint LastPoints { get; set; }
        public virtual uint LockReleaseTime { get; set; }
    }
}