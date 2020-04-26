namespace DB.Entities
{
    public class DbUser
    {
        public virtual uint Identity { get; set; } // Unique ID
        public virtual uint AccountId { get; set; } // Account ID
        public virtual string Name { get; set; } // Character name
        public virtual string Mate { get; set; } // Character mate
        public virtual uint Lookface { get; set; } // Lookface * 10000 + Body
        public virtual ushort Hair { get; set; } // Hair ID
        public virtual uint Money { get; set; } // Money
        public virtual uint MoneySaved { get; set; } // Warehouse Money
        public virtual uint CoinMoney { get; set; } // Credits
        public virtual uint Emoney { get; set; } // CPs
        public virtual uint BoundEmoney { get; set; } // CPs(B)
        public virtual byte Level { get; set; }
        public virtual long Experience { get; set; }
        public virtual ushort Strength { get; set; }
        public virtual ushort Agility { get; set; }
        public virtual ushort Vitality { get; set; }
        public virtual ushort Spirit { get; set; }
        public virtual ushort Profession { get; set; } // Actual 
        public virtual ushort FirstProfession { get; set; } // 0rb
        public virtual ushort LastProfession { get; set; } // 1rb
        public virtual byte Metempsychosis { get; set; } // Reborns
        public virtual ushort AdditionalPoints { get; set; }
        public virtual byte AutoAllot { get; set; } // 1 = Auto 0 = Not
        public virtual ushort Life { get; set; }
        public virtual ushort Mana { get; set; }
        public virtual ushort PkPoints { get; set; }
        public virtual uint MapId { get; set; }
        public virtual ushort MapX { get; set; }
        public virtual ushort MapY { get; set; }
        public virtual uint LastLogin { get; set; } // int unix timestamp
        public virtual uint LuckyTime { get; set; }
        public virtual uint Virtue { get; set; }
        public virtual uint HomeId { get; set; }
        public virtual uint HeavenBlessing { get; set; }
        public virtual ulong LockKey { get; set; } // warehouse password
        public virtual byte AutoExercise { get; set; } // 1 = OffTG 0 = Common
        public virtual uint LastLogout { get; set; }
        public virtual uint MeteLevel { get; set; } // The Level * 1000 + Exp Percent
        public virtual byte CurrentLayout { get; set; }
        public virtual long Donation { get; set; }
        public virtual uint Business { get; set; }
        public virtual uint Flower { get; set; } // The last day when male sent a flower
        public virtual uint RedRoses { get; set; } // RedRoses flower_r
        public virtual uint WhiteRoses { get; set; } // Lillies flower_w
        public virtual uint Orchids { get; set; } // Orchids flower_lily
        public virtual uint Tulips { get; set; } // Tulips flower_tulip
        public virtual uint StudentPoints { get; set; } // Quiz Points
        public virtual uint StudyPoints { get; set; } // Study Points, subclass 2014-12-23
        public virtual byte ActiveSubclass { get; set; } // felipe 2014-12-24 christmas *u*
        public virtual ushort EnlightPoints { get; set; } // felipe 2014-12-26 after christmas :)
        public virtual byte SelectedTitle { get; set; } // felipe 2014-12-27
        public virtual float ExperienceMultipler { get; set; } // felipe 2016-05-08
        public virtual uint ExperienceExpires { get; set; } // felipe 2016-05-08
        public virtual byte ChkSum { get; set; }  // exp ball usage num felipe 2016-09-16
        public virtual uint ExpBallUsage { get; set; } // exp ball last use time felipe 2016-09-16
        public virtual uint LastUpdate { get; set; } // set last daily reset 2016-12-25
    }
}