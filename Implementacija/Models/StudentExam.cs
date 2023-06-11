using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class StudentExam : IActivity, INotificationObservable
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("StudentCourse")]
        public int CourseID { get; set; }
        public StudentCourse? Course { get; set; }

        [ForeignKey("Exam")]
        public int ExamID { get; set; }
        public Exam? Exam { get; set; }  

        public double PointsScored { get; set; }
        public bool IsPassed { get; set; }

        public StudentExam() { }    

        [NotMapped]
        private NotificationManager? Notifier = null;

        public double GetPointsScored()
        {
            return PointsScored;
        }

        public double GetTotalPoints()
        {
            return Exam.TotalPoints;
        }

        public DateTime GetActivityDate()
        {
            return Exam.Time.Date;
        }
        public string GetActivityType()
        {
            return Exam.Type.ToString();
        }

        public void Attach(NotificationManager Notifier)
        {
            this.Notifier = Notifier;
        }

        public void Detach()
        {
            Notifier = null;
        }

        public void Notify()
        {
            Notifier.UpdateForExamResults(this);
        }
    }
}
