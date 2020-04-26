namespace DB.Entities
{
    public class DbFlower
    {
        public virtual uint PlayerIdentity { get; set; }
        public virtual string PlayerName { get; set; }
        public virtual uint RedRoses { get; set; }
        public virtual uint WhiteRoses { get; set; }
        public virtual uint Orchids { get; set; }
        public virtual uint Tulips { get; set; }
    }
}