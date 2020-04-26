// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Tutor.cs
// File Created: 2015/08/03 12:57

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqTutorMap : ClassMap<DbMentor>
    {
        public CqTutorMap()
        {
            Table("cq_tutor");
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.GuideIdentity).Column("tutor_id");
            Map(x => x.GuideName).Column("tutor_name");
            Map(x => x.StudentIdentity).Column("Student_id");
            Map(x => x.StudentName).Column("Student_name");
            Map(x => x.BetrayalFlag).Column("Betrayal_flag");
            Map(x => x.Date).Column("Date");
        }
    }
}