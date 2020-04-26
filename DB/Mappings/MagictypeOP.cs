using FluentNHibernate.Mapping;
using DB.Entities;

namespace DB.Mappings
{
    public class CqMagictypeopMap : ClassMap<DbMagictypeop>
    {
        public CqMagictypeopMap()
        {
            Table("cq_magictypeop");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.RebirthTime).Column("rebirth_time").Not.Nullable();
            Map(x => x.ProfessionAgo).Column("profession_ago").Not.Nullable();
            Map(x => x.ProfessionNow).Column("profession_now").Not.Nullable();
            Map(x => x.MagictypeOp).Column("magictype_op").Not.Nullable();
            Map(x => x.Skill1).Column("skill_1").Not.Nullable();
            Map(x => x.Skill2).Column("skill_2").Not.Nullable();
            Map(x => x.Skill3).Column("skill_3").Not.Nullable();
            Map(x => x.Skill4).Column("skill_4").Not.Nullable();
            Map(x => x.Skill5).Column("skill_5").Not.Nullable();
            Map(x => x.Skill6).Column("skill_6").Not.Nullable();
            Map(x => x.Skill7).Column("skill_7").Not.Nullable();
            Map(x => x.Skill8).Column("skill_8").Not.Nullable();
            Map(x => x.Skill9).Column("skill_9").Not.Nullable();
            Map(x => x.Skill10).Column("skill_10").Not.Nullable();
            Map(x => x.Skill11).Column("skill_11").Not.Nullable();
            Map(x => x.Skill12).Column("skill_12").Not.Nullable();
            Map(x => x.Skill13).Column("skill_13").Not.Nullable();
            Map(x => x.Skill14).Column("skill_14").Not.Nullable();
            Map(x => x.Skill15).Column("skill_15").Not.Nullable();
            Map(x => x.Skill16).Column("skill_16").Not.Nullable();
            Map(x => x.Skill17).Column("skill_17").Not.Nullable();
            Map(x => x.Skill18).Column("skill_18").Not.Nullable();
            Map(x => x.Skill19).Column("skill_19").Not.Nullable();
            Map(x => x.Skill20).Column("skill_20").Not.Nullable();
            Map(x => x.Skill21).Column("skill_21").Not.Nullable();
            Map(x => x.Skill22).Column("skill_22").Not.Nullable();
            Map(x => x.Skill23).Column("skill_23").Not.Nullable();
            Map(x => x.Skill24).Column("skill_24").Not.Nullable();
            Map(x => x.Skill25).Column("skill_25").Not.Nullable();
            Map(x => x.Skill26).Column("skill_26").Not.Nullable();
            Map(x => x.Skill27).Column("skill_27").Not.Nullable();
            Map(x => x.Skill28).Column("skill_28").Not.Nullable();
            Map(x => x.Skill29).Column("skill_29").Not.Nullable();
            Map(x => x.Skill30).Column("skill_30").Not.Nullable();
            Map(x => x.Skill31).Column("skill_31").Not.Nullable();
            Map(x => x.Skill32).Column("skill_32").Not.Nullable();
            Map(x => x.Skill33).Column("skill_33").Not.Nullable();
            Map(x => x.Skill34).Column("skill_34").Not.Nullable();
            Map(x => x.Skill35).Column("skill_35").Not.Nullable();
            Map(x => x.Skill36).Column("skill_36").Not.Nullable();
            Map(x => x.Skill37).Column("skill_37").Not.Nullable();
            Map(x => x.Skill38).Column("skill_38").Not.Nullable();
            Map(x => x.Skill39).Column("skill_39").Not.Nullable();
            Map(x => x.Skill40).Column("skill_40").Not.Nullable();
            Map(x => x.Skill41).Column("skill_41").Not.Nullable();
            Map(x => x.Skill42).Column("skill_42").Not.Nullable();
            Map(x => x.Skill43).Column("skill_43").Not.Nullable();
            Map(x => x.Skill44).Column("skill_44").Not.Nullable();
            Map(x => x.Skill45).Column("skill_45").Not.Nullable();
            Map(x => x.Skill46).Column("skill_46").Not.Nullable();
            Map(x => x.Skill47).Column("skill_47").Not.Nullable();
            Map(x => x.Skill48).Column("skill_48").Not.Nullable();
            Map(x => x.Skill49).Column("skill_49").Not.Nullable();
            Map(x => x.Skill50).Column("skill_50").Not.Nullable();
            Map(x => x.Skill51).Column("skill_51").Not.Nullable();
            Map(x => x.Skill52).Column("skill_52").Not.Nullable();
            Map(x => x.Skill53).Column("skill_53").Not.Nullable();
            Map(x => x.Skill54).Column("skill_54").Not.Nullable();
            Map(x => x.Skill55).Column("skill_55").Not.Nullable();
            Map(x => x.Skill56).Column("skill_56").Not.Nullable();
            Map(x => x.Skill57).Column("skill_57").Not.Nullable();
            Map(x => x.Skill58).Column("skill_58").Not.Nullable();
            Map(x => x.Skill59).Column("skill_59").Not.Nullable();
            Map(x => x.Skill60).Column("skill_60").Not.Nullable();
        }
    }
}