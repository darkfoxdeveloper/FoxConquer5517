// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Spawn Generator.cs
// File Created: 2015/08/01 15:10
namespace DB.Entities
{
    public class DbGenerator
    {
        public virtual uint Id { get; set; }
        public virtual uint Mapid { get; set; }
        public virtual ushort BoundX { get; set; }
        public virtual ushort BoundY { get; set; }
        public virtual ushort BoundCx { get; set; }
        public virtual ushort BoundCy { get; set; }
        public virtual int MaxNpc { get; set; }
        public virtual int RestSecs { get; set; }
        public virtual int MaxPerGen { get; set; }
        public virtual uint Npctype { get; set; }
        public virtual int TimerBegin { get; set; }
        public virtual int TimerEnd { get; set; }
        public virtual int BornX { get; set; }
        public virtual int BornY { get; set; }
    }
}