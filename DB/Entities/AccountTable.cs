// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - DB - Account Table.cs
// Last Edit: 2017/03/01 17:34
// Created: 2017/02/04 14:29
namespace DB.Entities
{
    public class DbAccount
    {
        /// <summary>
        /// The user account unique identification.
        /// </summary>
        public virtual uint Identity { get; set; }
        /// <summary>
        /// The name the player uses to log in.
        /// </summary>
        public virtual string Username { get; set; }
        /// <summary>
        /// The password set by the player on registration.
        /// </summary>
        public virtual string Password { get; set; }
        /// <summary>
        /// The user VIP level.
        /// </summary>
        public virtual byte Vip { get; set; }
        /// <summary>
        /// The user authority level. 0 = None 8 = Administrator
        /// </summary>
        public virtual byte Type { get; set; }
        /// <summary>
        /// The user account status or the code ID of the error. 0 means normal.
        /// </summary>
        public virtual byte Lock { get; set; }
        /// <summary>
        /// The date when the user logged in for the last time.
        /// </summary>
        public virtual int LastLogin { get; set; }
        public virtual string MacAddress { get; set; } // 2017-03-01
        public virtual uint LockExpire { get; set; } // 2017-03-01
    }
}