namespace DB.Entities
{
    public class DbLoginRcd
    {
        public virtual uint Identity { get; set; }
        public virtual uint UserIdentity { get; set; }
        public virtual int LoginTime { get; set; }
        public virtual int OnlineSecond { get; set; }
        public virtual string MacAddress { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual string ResourceSource { get; set; }
    }
}