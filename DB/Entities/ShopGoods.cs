// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Shop Goods.cs
// File Created: 2015/08/01 15:22
namespace DB.Entities
{
    public class DbGoods
    {
        public virtual uint Identity { get; set; }
        public virtual uint OwnerIdentity { get; set; }
        public virtual uint Itemtype { get; set; }
        public virtual uint Moneytype { get; set; }
        public virtual uint Monopoly { get; set; }
    }
}