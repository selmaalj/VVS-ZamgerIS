using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class StudentCourse : INotificationObservable
    {
        [Key]
        public int ID {  get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }
        public Course? Course { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }
        public Student? Student { get; set; }

        public double Points { get; set; }
        private int Grade { get; set; }

        private NotificationManager notification = null;

        public StudentCourse() { }

        public void setGrade(int grade) 
        {
            this.Grade = grade;
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
            notification.UpdateForFinalGrade(this);
        }
    }
}
