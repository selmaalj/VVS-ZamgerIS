using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class Exam : INotificationObservable
    {

        [Key]
        public int ID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }
        public Course? Course { get; set; }

        public DateTime Time { get; set; }
        public ExamType Type { get; set; }

        public double TotalPoints { get; set; }

        public double MinimumPoints { get; set; }

        private NotificationManager notification;

        public Exam(Course course, DateTime time, ExamType type, double totalPoints, double minimumPoints, NotificationManager notif) 
        {
            this.Course = course;
            this.Time = time;
            this.Type = type;
            this.TotalPoints = totalPoints;
            this.MinimumPoints = minimumPoints;
            this.notification = notif;
            this.Notify();
        }

        public void Attach(NotificationManager notifications)
        {
            this.notification = notification;
        }

        public void Detach()
        {
            notification = null;
        }

        public void Notify()
        {
            notification.UpdateForExamCreation(this);
        }
    }
}
