using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class Quiz : ClassMap<DbGameQuiz>
    {
        public Quiz()
        {
            Table(TableName.QUIZ);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.Question).Column("question").Not.Nullable();
            Map(x => x.Answer0).Column("answer0").Not.Nullable();
            Map(x => x.Answer1).Column("answer1").Not.Nullable();
            Map(x => x.Answer2).Column("answer2").Not.Nullable();
            Map(x => x.Answer3).Column("answer3").Not.Nullable();
            Map(x => x.Correct).Column("correct").Not.Nullable().Default("1");
        }
    }
}