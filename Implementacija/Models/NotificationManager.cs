using Microsoft.EntityFrameworkCore;
using ooadproject.Data;
using System.Security.Cryptography.Xml;

namespace ooadproject.Models
{
    public class NotificationManager : INotificationObserver
    {
        private ApplicationDbContext Db;
        public NotificationManager(ApplicationDbContext Db) 
        {
            this.Db = Db;
        }
        public async Task UpdateForExamCreation(Exam exam)
        {
            //for every exam registration this metod will be called and
            //it needs to make notifications for every student who is enrolled in this course 

            var course = exam.Course;
            var studentCourses = await Db.StudentCourse.Include(sc => sc.Student).Where(sc => sc.CourseID == exam.CourseID).ToListAsync();
            var students = studentCourses.Select(sc => sc.Student).ToList();
            foreach (var student in students)
            {
                var notification = new NotificationBuilder()
                    .setTitle("Otvorene prijave na ispit")
                    .setMessage(exam.Course.Name + ". " + exam.Type + ", " + exam. Time.ToShortDateString() )
                    .setRecipient(student)
                    .build();
                await Db.AddAsync(notification);
                await Db.SaveChangesAsync();
            }

        }

        public async Task UpdateForExamResults(StudentExam studentExam)
        {
            //for every exam result (in StudentExams) this metod will be called and
            //it needs to make notifications for this student

            var notification = new NotificationBuilder()
                    .setTitle("Objavljeni rezultati ispita")
                    .setMessage(studentExam.Exam.Course.Name + ". " + studentExam.Exam.Type + ", " + studentExam.Exam.Time.ToShortDateString()
                        + ": " + studentExam.PointsScored + ".")
                    .setRecipient(studentExam.Course.Student)
                    .build();
            await Db.AddAsync(notification);
            await Db.SaveChangesAsync();
        }

        public void UpdateForFinalGrade(StudentCourse studentCourse)
        {
            //it will be called when someone defines the final grade in StduentCourse... notification bla bla...
        }
    }
}
