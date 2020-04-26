namespace DB.Entities
{
    public class DbMentorAccess
    {
        public virtual uint Identity { get; set; }
        public virtual uint GuideIdentity { get; set; }
        public virtual ulong Experience { get; set; }
        public virtual ushort Blessing { get; set; }
        public virtual ushort Composition { get; set; }
    }
}