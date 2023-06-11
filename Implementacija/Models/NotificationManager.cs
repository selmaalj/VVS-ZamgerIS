using ooadproject.Data;

namespace ooadproject.Models
{
    public class NotificationManager : INotificationObserver
    {
        private ApplicationDbContext Db;
        public NotificationManager(ApplicationDbContext Db) 
        {
            this.Db = Db;
        }
        public void UpdateForExamCreation(Exam exam)
        {
            //for every exam registration this metod will be called and
            //it needs to make notifications for every student who is enrolled in this course 
        }

        public void UpdateForExamResoults(StudentExam studentExam)
        {
            //for every exam result (in StudentExams) this metod will be called and
            //it needs to make notifications for this student
        }

        public void UpdateForFinalGrade(StudentCourse studentCourse)
        {
            //it will be called when someone defines the final grade in StduentCourse... notification bla bla...
        }
    }
}
