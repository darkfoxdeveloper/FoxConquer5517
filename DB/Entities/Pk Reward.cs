namespace DB.Entities
{
    public class DbPkReward
    {
        public virtual uint Identity { get; set; }
        public virtual uint TargetIdentity { get; set; }
        public virtual string TargetName { get; set; }
        public virtual uint HunterIdentity { get; set; }
        public virtual string HunterName { get; set; }
        public virtual byte BonusType { get; set; }
        public virtual uint Bonus { get; set; }
    }
}