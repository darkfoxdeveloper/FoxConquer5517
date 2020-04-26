// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Player Weapon Skill.cs
// File Created: 2015/08/01 15:52
namespace DB.Entities
{
    public class DbWeaponSkill
    {
        /// <summary>
        /// The unique identification of the weapon skill.
        /// </summary>
        public virtual uint Identity { get; set; }
        /// <summary>
        /// The actual level of the weapon skill.
        /// </summary>
        public virtual uint Level { get; set; }
        /// <summary>
        /// The amount of experience of the actual level.
        /// </summary>
        public virtual uint Experience { get; set; }
        /// <summary>
        /// The owner unique identity (character identity).
        /// </summary>
        public virtual uint OwnerIdentity { get; set; }
        /// <summary>
        /// The old level of the weapon skill before reborn (if higher)
        /// </summary>
        public virtual uint OldLevel { get; set; }
        /// <summary>
        /// If the weapon skill is active. 1 Means that it is waiting the level hit the
        /// old level to restore the old status.
        /// </summary>
        public virtual uint Unlearn { get; set; }
        /// <summary>
        /// The 3 digit type of weapon. (410 - Blade)
        /// </summary>
        public virtual uint Type { get; set; }
    }
}