// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Player Status.cs
// File Created: 2015/08/01 15:44
namespace DB.Entities
{
    public class DbStatus
    {
        public virtual uint Id { get; set; }
        public virtual uint OwnerId { get; set; }
        public virtual uint Status { get; set; }
        public virtual int Power { get; set; }
        public virtual uint Sort { get; set; }
        public virtual uint LeaveTimes { get; set; }
        public virtual uint RemainTime { get; set; }
        public virtual uint EndTime { get; set; }
        public virtual uint IntervalTime { get; set; }
    }
}