namespace DB.Entities
{
    public class DbDetainedItem
    {
        public virtual uint Identity { get; set; }
        public virtual uint ItemIdentity { get; set; }
        public virtual uint TargetIdentity { get; set; }
        public virtual string TargetName { get; set; }
        public virtual uint HunterIdentity { get; set; }
        public virtual string HunterName { get; set; }
        public virtual int HuntTime { get; set; }
        public virtual uint RedeemPrice { get; set; }
    }
}