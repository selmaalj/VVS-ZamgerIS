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
    //    [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage =
     //   "Naziv predmeta smije imati između 3 i 50 karaktera!")]

        public Course? Course { get; set; }
        public DateTime Time { get; set; }
        public ExamType Type { get; set; }
        public double TotalPoints { get; set; }
        public double MinimumPoints { get; set; }

        [NotMapped]
        private NotificationManager? Notifier = null;
        public void Attach(NotificationManager notifier)
        {
            this.Notifier = notifier;
        }

        public void Detach()
        {
            Notifier = null;
        }

        public async Task Notify()
        {
            await Notifier.UpdateForExamCreation(this);
        }
    }
}
