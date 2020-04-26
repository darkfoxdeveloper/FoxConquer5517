// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Map.cs
// File Created: 2015/08/01 15:27
namespace DB.Entities
{
    public class DbMap
    {
        public virtual uint Identity { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual uint MapDoc { get; set; }
        public virtual uint Type { get; set; }
        public virtual uint OwnerId { get; set; }
        public virtual int MapGroup { get; set; }
        public virtual int? IdXServer { get; set; }
        public virtual uint? PortalX { get; set; }
        public virtual uint? PortalY { get; set; }
        public virtual uint? RebornMap { get; set; }
        public virtual uint? RebornPortal { get; set; }
        public virtual short ResLevel { get; set; }
        public virtual string Path { get; set; }
    }
}