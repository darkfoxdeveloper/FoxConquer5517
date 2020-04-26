// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Player Item.cs
// File Created: 2015/08/01 15:23
namespace DB.Entities
{
    public class DbItem
    {
        /// <summary>
        /// The unique identification of the item.
        /// </summary>
        public virtual uint Id { get; set; }
        /// <summary>
        /// The itemtype of the item.
        /// </summary>
        public virtual uint Type { get; set; }
        /// <summary>
        /// The owner (shop or player) where the player got the item from.
        /// </summary>
        public virtual uint OwnerId { get; set; }
        /// <summary>
        /// The player who actually owns the item.
        /// </summary>
        public virtual uint PlayerId { get; set; }
        /// <summary>
        /// The actual durability of the item.
        /// </summary>
        public virtual ushort Amount { get; set; }
        /// <summary>
        /// The max amount of the durability of the item.
        /// </summary>
        public virtual ushort AmountLimit { get; set; }
        /// <summary>
        /// Not sure yet.
        /// </summary>
        public virtual byte Ident { get; set; }
        /// <summary>
        /// The actual position of the item. +200 for warehouses.
        /// </summary>
        public virtual byte Position { get; set; }
        /// <summary>
        /// The gem on socket 1. 255 for open hole.
        /// </summary>
        public virtual byte Gem1 { get; set; }
        /// <summary>
        /// The gem on socket 2. 255 for open hole.
        /// </summary>
        public virtual byte Gem2 { get; set; }
        /// <summary>
        /// The item effect.
        /// </summary>
        public virtual byte Magic1 { get; set; }
        /// <summary>
        /// Not sure yet.
        /// </summary>
        public virtual byte Magic2 { get; set; }
        /// <summary>
        /// The item plus.
        /// </summary>
        public virtual byte Magic3 { get; set; }
        /// <summary>
        /// Not sure yet. Guess i will use this for socket progress.
        /// </summary>
        public virtual uint Data { get; set; }
        /// <summary>
        /// The item blessing.
        /// </summary>
        public virtual byte ReduceDmg { get; set; }
        /// <summary>
        /// Item enchantment.
        /// </summary>
        public virtual byte AddLife { get; set; }
        /// <summary>
        /// The green attribute. Not used tho.
        /// </summary>
        public virtual byte AntiMonster { get; set; }
        /// <summary>
        /// Not sure yet.
        /// </summary>
        public virtual uint ChkSum { get; set; }
        /// <summary>
        /// Item locking timestamp. If 0, item is not locked, if timestamp is set 1 item is locked. If it has a timestamp, it's the unlock time.
        /// </summary>
        public virtual uint Plunder { get; set; }
        /// <summary>
        /// The remaining time for an item to disappear.
        /// </summary>
        public virtual uint RemainingTime { get; set; }
        /// <summary>
        /// Forbbiden or not?
        /// </summary>
        public virtual uint Specialflag { get; set; }
        /// <summary>
        /// The color of the item.
        /// </summary>
        public virtual byte Color { get; set; }
        /// <summary>
        /// The progress of the plus.
        /// </summary>
        public virtual uint AddlevelExp { get; set; }
        /// <summary>
        /// The kind of item. (Bound, Quest, etc)
        /// </summary>
        public virtual byte Monopoly { get; set; }
        /// <summary>
        /// If the item is inscribed or not.
        /// </summary>
        public virtual byte Inscribed { get; set; }
        /// <summary>
        /// The kind of Artifact (or DragonSoul).
        /// </summary>
        public virtual uint ArtifactType { get; set; }
        /// <summary>
        /// The unix timestamp of when the artifact has been activated.
        /// </summary>
        public virtual uint ArtifactStart { get; set; }
        /// <summary>
        /// The unix timestamp of when the artifact will expire.
        /// </summary>
        public virtual uint ArtifactExpire { get; set; }
        /// <summary>
        /// The amount of Stabilization Points of the artifact.
        /// </summary>
        public virtual uint ArtifactStabilization { get; set; }
        /// <summary>
        /// The kind of refinery. (Critical-Strike)
        /// </summary>
        public virtual uint RefineryType { get; set; }
        /// <summary>
        /// The level of the refinery.
        /// </summary>
        public virtual byte RefineryLevel { get; set; }
        /// <summary>
        /// The unix timestamp of when the refinery has been activated.
        /// </summary>
        public virtual uint RefineryStart { get; set; }
        /// <summary>
        /// When the refinery will expire.
        /// </summary>
        public virtual uint RefineryExpire { get; set; }
        /// <summary>
        /// The amount of Stabilization Points of the refinery.
        /// </summary>
        public virtual uint RefineryStabilization { get; set; }
        /// <summary>
        /// The amount of items stacked.
        /// </summary>
        public virtual ushort StackAmount { get; set; }
    }
}