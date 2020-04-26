// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Character.cs
// File Created: 2015/08/03 12:58

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqDelUserMap : ClassMap<DbUserDeleted>
    {
        public CqDelUserMap()
        {
            Table("cq_deluser");
            LazyLoad();
            Id(x => x.Identity).Column("id").Not.Nullable().GeneratedBy.Identity();
            Map(x => x.AccountId).Column("account_id").Not.Nullable();
            Map(x => x.Name).Column("name").Not.Nullable();
            Map(x => x.Mate).Column("mate").Default("None").Not.Nullable();
            Map(x => x.Lookface).Column("lookface").Not.Nullable();
            Map(x => x.Hair).Column("hair").Not.Nullable().Default("0");
            Map(x => x.Money).Column("money").Default("100").Not.Nullable();
            Map(x => x.MoneySaved).Column("money_saved").Default("0").Not.Nullable();
            Map(x => x.CoinMoney).Column("coin_money").Default("0").Not.Nullable();
            Map(x => x.Emoney).Column("emoney").Default("0").Not.Nullable();
            Map(x => x.BoundEmoney).Column("emoney2").Default("0").Not.Nullable();
            Map(x => x.Level).Column("level").Default("1").Not.Nullable();
            Map(x => x.Experience).Column("exp").Default("0").Not.Nullable();
            Map(x => x.Strength).Column("strength").Default("0").Not.Nullable();
            Map(x => x.Agility).Column("Speed").Default("0").Not.Nullable();
            Map(x => x.Vitality).Column("health").Default("0").Not.Nullable();
            Map(x => x.Spirit).Column("soul").Default("0").Not.Nullable();
            Map(x => x.AdditionalPoints).Column("additional_point").Default("0").Not.Nullable();
            Map(x => x.AutoAllot).Column("auto_allot").Default("1").Not.Nullable();
            Map(x => x.Life).Column("life").Default("1").Not.Nullable();
            Map(x => x.Mana).Column("mana").Default("0").Not.Nullable();
            Map(x => x.Profession).Column("profession").Not.Nullable();
            Map(x => x.FirstProfession).Column("first_prof").Default("0").Not.Nullable();
            Map(x => x.LastProfession).Column("old_prof").Default("0").Not.Nullable();
            Map(x => x.Metempsychosis).Column("metempsychosis").Default("0").Not.Nullable();
            Map(x => x.PkPoints).Column("pk").Default("0").Not.Nullable();
            Map(x => x.MapId).Column("recordmap_id").Default("1002").Not.Nullable();
            Map(x => x.MapX).Column("recordx").Default("430").Not.Nullable();
            Map(x => x.MapY).Column("recordy").Default("380").Not.Nullable();
            Map(x => x.LastLogin).Column("last_login").Default("0").Not.Nullable();
            Map(x => x.LuckyTime).Column("time_of_life").Default("0").Not.Nullable();
            Map(x => x.Virtue).Column("virtue").Default("0").Not.Nullable();
            Map(x => x.HomeId).Column("home_id").Default("0").Not.Nullable();
            Map(x => x.LockKey).Column("lock_key").Default("0").Not.Nullable();
            Map(x => x.AutoExercise).Column("auto_exercise").Default("0").Not.Nullable();
            Map(x => x.HeavenBlessing).Column("god_status").Default("0").Not.Nullable();
            Map(x => x.LastLogout).Column("last_logout").Default("0").Not.Nullable();
            Map(x => x.MeteLevel).Column("mete_lev").Default("0").Not.Nullable();
            Map(x => x.CurrentLayout).Column("current_layout_type").Default("0").Not.Nullable();
            Map(x => x.Flower).Column("flower").Default("0").Not.Nullable();
            Map(x => x.Donation).Column("donation").Default("0").Not.Nullable();
            Map(x => x.Business).Column("business").Default("255").Not.Nullable();
            Map(x => x.RedRoses).Column("flower_r").Default("0").Not.Nullable();
            Map(x => x.WhiteRoses).Column("flower_w").Default("0").Not.Nullable();
            Map(x => x.Orchids).Column("flower_lily").Default("0").Not.Nullable();
            Map(x => x.Tulips).Column("flower_tulip").Default("0").Not.Nullable();
            Map(x => x.StudentPoints).Column("professor_points").Default("0").Not.Nullable();
            Map(x => x.StudyPoints).Column("study_points").Default("0").Not.Nullable();
            Map(x => x.ActiveSubclass).Column("active_subclass").Default("0").Not.Nullable();
            Map(x => x.EnlightPoints).Column("enlight_points").Default("0").Not.Nullable();
            Map(x => x.SelectedTitle).Column("selected_title").Default("0").Not.Nullable();
            Map(x => x.ExperienceMultipler).Column("exp_multiply").Default("1").Not.Nullable();
            Map(x => x.ExperienceExpires).Column("exp_expires").Default("0").Not.Nullable();
            Map(x => x.ChkSum).Column("chk_sum").Default("0").Not.Nullable();
            Map(x => x.ExpBallUsage).Column("exp_ball_usage").Default("0").Not.Nullable();
        }
    }
}