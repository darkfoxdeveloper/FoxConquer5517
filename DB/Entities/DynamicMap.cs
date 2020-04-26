// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Dynamic Map.cs
// File Created: 2015/07/31 22:36
namespace DB.Entities
{
    public class DbDynamicMap
    {
        public virtual uint Identity { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual uint MapDoc { get; set; }
        public virtual uint Type { get; set; }
        public virtual uint OwnerId { get; set; }
        public virtual uint Mapgroup { get; set; }
        public virtual uint Idxserver { get; set; }
        public virtual uint Weather { get; set; }
        public virtual uint Bgmusic { get; set; }
        public virtual uint BgmusicShow { get; set; }
        public virtual uint Portal0X { get; set; }
        public virtual uint Portal0Y { get; set; }
        public virtual uint RebornMapid { get; set; }
        public virtual uint RebornPortal { get; set; }
        public virtual byte ResLev { get; set; }
        public virtual byte OwnerType { get; set; }
        public virtual uint LinkMap { get; set; }
        public virtual ushort LinkX { get; set; }
        public virtual ushort LinkY { get; set; }
        public virtual byte DelFlag { get; set; }
        public virtual uint Color { get; set; }
        public virtual string FileName { get; set; }
    }
}