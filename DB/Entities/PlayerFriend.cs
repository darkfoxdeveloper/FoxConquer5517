// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Player Friend.cs
// File Created: 2015/08/01 12:59
namespace DB.Entities
{
    public class DbFriend
    {
        public virtual uint Identity { get; set; }
        public virtual uint UserIdentity { get; set; }
        public virtual uint Friend { get; set; }
        public virtual string FriendName { get; set; }
    }
}