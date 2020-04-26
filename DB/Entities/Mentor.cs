namespace DB.Entities
{
    public class DbMentor
    {
        public virtual uint Identity { get; set; }
        public virtual uint GuideIdentity { get; set; }
        public virtual uint StudentIdentity { get; set; }
        public virtual string GuideName { get; set; }
        public virtual string StudentName { get; set; }
        public virtual int BetrayalFlag { get; set; }
        public virtual uint Date { get; set; }
    }
}