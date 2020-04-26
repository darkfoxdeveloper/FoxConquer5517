namespace DB.Entities
{
    public class MentorBattleLimit
    {
        /// <summary>
        /// Battle power of the apprentice.
        /// </summary>
        public virtual ushort Id { get; set; }
        /// <summary>
        /// Maximum addition battle power for that level.
        /// </summary>
        public virtual ushort BattleLevelLimit { get; set; }
    }
}