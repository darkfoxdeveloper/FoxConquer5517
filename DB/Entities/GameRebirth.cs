// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Game Rebirth.cs
// File Created: 2015/08/01 15:39
namespace DB.Entities
{
    public class DbCqRebirth
    {
        public virtual uint Identity { get; set; }
        public virtual ushort NeedProfession { get; set; }
        public virtual ushort NewProfession { get; set; }
        public virtual byte NeedLevel { get; set; }
        public virtual byte NewLevel { get; set; }
        public virtual byte Metempsychosis { get; set; }
    }
}