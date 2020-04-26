// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Action.cs
// File Created: 2015/07/31 20:24
namespace DB.Entities
{
    public class DbGameAction
    {
        public virtual uint Identity { get; set; }
        public virtual uint IdNext { get; set; }
        public virtual uint IdNextfail { get; set; }
        public virtual uint Type { get; set; }
        public virtual uint Data { get; set; }
        public virtual string Param { get; set; }
    }
}
