// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Npc.cs
// File Created: 2015/08/01 15:29
namespace DB.Entities
{
    public class DbNpc
    {
        public virtual uint Id { get; set; }
        public virtual uint Ownerid { get; set; }
        public virtual uint Playerid { get; set; }
        public virtual string Name { get; set; }
        public virtual ushort Type { get; set; }
        public virtual ushort Lookface { get; set; }
        public virtual int Idxserver { get; set; }
        public virtual uint Mapid { get; set; }
        public virtual ushort Cellx { get; set; }
        public virtual ushort Celly { get; set; }
        public virtual uint Task0 { get; set; }
        public virtual uint Task1 { get; set; }
        public virtual uint Task2 { get; set; }
        public virtual uint Task3 { get; set; }
        public virtual uint Task4 { get; set; }
        public virtual uint Task5 { get; set; }
        public virtual uint Task6 { get; set; }
        public virtual uint Task7 { get; set; }
        public virtual int Data0 { get; set; }
        public virtual int Data1 { get; set; }
        public virtual int Data2 { get; set; }
        public virtual int Data3 { get; set; }
        public virtual string Datastr { get; set; }
        public virtual uint Linkid { get; set; }
        public virtual ushort Life { get; set; }
        public virtual ushort Maxlife { get; set; }
        public virtual uint Base { get; set; }
        public virtual ushort Sort { get; set; }
        public virtual uint Itemid { get; set; }
        public virtual ushort Defence { get; set; }
        public virtual ushort MagicDef { get; set; }
    }
}