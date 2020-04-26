namespace DB.Entities
{
    public class DbGameLottery
    {
        public virtual uint Identity { get; set; }
        public virtual byte Type { get; set; }
        public virtual byte Rank { get; set; }
        public virtual byte Chance { get; set; }
        public virtual string Itemname { get; set; }
        public virtual uint ItemIdentity { get; set; }
        public virtual byte Color { get; set; }
        public virtual byte SocketNum { get; set; }
        public virtual byte Plus { get; set; }
    }
}