namespace DB.Entities
{
    public class DbDynaRankRec
    {
        public virtual uint Identity { get; set; }
        public virtual uint RankType { get; set; }
        public virtual long Value { get; set; }
        public virtual uint ObjectId { get; set; }
        public virtual string ObjectName { get; set; }
        public virtual uint UserIdentity { get; set; }
        public virtual string Username { get; set; }
    }
}