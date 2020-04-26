namespace DB.Entities
{
    public class DbMonsterMagic
    {
        public virtual uint Identity { get; set; }
        public virtual uint OwnerIdentity { get; set; }
        public virtual ushort MagicIdentity { get; set; }
        public virtual byte MagicLevel { get; set; }
        public virtual uint Chance { get; set; }
    }
}