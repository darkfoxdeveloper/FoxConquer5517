// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Passway.cs
// File Created: 2015/08/01 15:30
namespace DB.Entities
{
    public class DbPassway
    {
        public virtual uint Identity { get; set; }
        public virtual uint MapId { get; set; }
        public virtual uint MapIndex { get; set; }
        public virtual uint TargetMapId { get; set; }
        public virtual uint TargetPortal { get; set; }
    }
}