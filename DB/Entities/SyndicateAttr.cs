// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Syndicate Attr.cs
// File Created: 2015/08/01 15:46
namespace DB.Entities
{
    public class DbCqSynattr
    {
        public virtual uint Id { get; set; }
        public virtual uint SynId { get; set; }
        public virtual ushort Rank { get; set; }
        public virtual long Proffer { get; set; }
        public virtual ulong ProfferTotal { get; set; }
        public virtual uint Emoney { get; set; }
        public virtual uint EmoneyTotal { get; set; }
        public virtual int Pk { get; set; }
        public virtual uint PkTotal { get; set; }
        public virtual uint Guide { get; set; }
        public virtual uint GuideTotal { get; set; }
        public virtual uint Exploit { get; set; }
        public virtual uint Arsenal { get; set; }
        public virtual uint Expiration { get; set; }
        public virtual uint JoinDate { get; set; }
    }
}