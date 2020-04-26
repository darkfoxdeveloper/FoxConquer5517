// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Name Change Log.cs
// Last Edit: 2016/12/29 17:21
// Created: 2016/12/29 17:20
namespace DB.Entities
{
    public class DbNameChangeLog
    {
        public virtual uint Identity { get; set; }
        public virtual uint UserIdentity { get; set; }
        public virtual string OldName { get; set; }
        public virtual string NewName { get; set; }
        public virtual uint Timestamp { get; set; }
        public virtual byte Changed { get; set; }
    }
}