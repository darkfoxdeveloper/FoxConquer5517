// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Portal.cs
// File Created: 2015/08/01 15:39
namespace DB.Entities
{
    public class DbPortal
    {
        public virtual uint Identity { get; set; }
        public virtual uint MapId { get; set; }
        public virtual uint PortalIndex { get; set; }
        public virtual uint PortalX { get; set; }
        public virtual uint PortalY { get; set; }
    }
}