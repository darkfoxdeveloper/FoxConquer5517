// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Region.cs
// File Created: 2015/08/01 15:41
namespace DB.Entities
{
    public class DbRegion
    {
        public virtual uint Identity { get; set; }
        public virtual uint MapIdentity { get; set; }
        public virtual uint Type { get; set; }
        public virtual uint BoundX { get; set; }
        public virtual uint BoundY { get; set; }
        public virtual uint BoundCX { get; set; }
        public virtual uint BoundCY { get; set; }
        public virtual string DataString { get; set; }
        public virtual uint Data0 { get; set; }
        public virtual uint Data1 { get; set; }
        public virtual uint Data2 { get; set; }
        public virtual uint Data3 { get; set; }
        public virtual uint ActionId { get; set; } // 2015-01-13
    }
}