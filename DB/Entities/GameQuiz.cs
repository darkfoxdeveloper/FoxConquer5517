namespace DB.Entities
{
    public class DbGameQuiz
    {
        public virtual uint Identity { get; set; }
        public virtual string Question { get; set; }
        public virtual string Answer0 { get; set; }
        public virtual string Answer1 { get; set; }
        public virtual string Answer2 { get; set; }
        public virtual string Answer3 { get; set; }
        public virtual byte Correct { get; set; }
    }
}