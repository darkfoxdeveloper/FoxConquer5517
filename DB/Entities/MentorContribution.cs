namespace DB.Entities
{
    public class MentorContribution
    {
        public virtual uint Identity { get; set; }
        public virtual uint TutorIdentity { get; set; }
        public virtual uint StudentIdentity { get; set; }
        public virtual string StudentName { get; set; }
        public virtual ushort GodTime { get; set; }
        public virtual uint Experience { get; set; }
        public virtual uint PlusStone { get; set; }
    }
}