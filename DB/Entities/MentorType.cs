namespace DB.Entities
{
    public class DbMentorType
    {
        public virtual uint Id { get; set; }
        /// <summary>
        /// Minimum user level for this rule. Below the lowest one can't be a mentor.
        /// </summary>
        public virtual byte UserMinLevel { get; set; }
        /// <summary>
        /// Maximum user level for this rule. After the highest one, get the higher.
        /// </summary>
        public virtual byte UserMaxLevel { get; set; }
        /// <summary>
        /// Maximum number of students a user can have.
        /// </summary>
        public virtual byte StudentNum { get; set; }
        /// <summary>
        /// Percentage of battle power to share.
        /// </summary>
        public virtual byte BattleLevelShare { get; set; }
    }
}