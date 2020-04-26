// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Player Statistic.cs
// File Created: 2015/08/01 15:43
namespace DB.Entities
{
    public class DbStatistic
    {
        public virtual uint Identity { get; set; }
        public virtual uint PlayerIdentity { get; set; }
        public virtual string PlayerName { get; set; }
        public virtual uint EventType { get; set; }
        public virtual uint DataType { get; set; }
        public virtual uint Data { get; set; }
        public virtual uint Timestamp { get; set; }
    }
}