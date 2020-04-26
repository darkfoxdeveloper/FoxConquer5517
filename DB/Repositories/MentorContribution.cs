using DB.Entities;
using NHibernate.Criterion;

namespace DB.Repositories
{
    public class MentorContributionRepository : HibernateDataRow<MentorContribution>
    {
        public MentorContribution FetchInformation(uint idMentor, uint idApprentice)
        {
            using (var pSession = SessionFactory.GameDatabase.OpenSession())
                return pSession.CreateCriteria<MentorContribution>()
                    .Add(Restrictions.And(Restrictions.Eq("TutorIdentity", idMentor),
                        Restrictions.Eq("StudentIdentity", idApprentice)))
                    .UniqueResult<MentorContribution>();
        }

        public bool SaveOrUpdate(MentorContribution obj)
        {
            return base.SaveOrUpdate(obj, SessionFactory.GameDatabase.OpenSession());
        }

        public bool Delete(MentorContribution obj)
        {
            return TryDelete(obj, SessionFactory.GameDatabase.OpenSession());
        }
    }
}