using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class Exam
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
        [Range(0,100,ErrorMessage = "Ukupan broj poena mora biti izmedju 0 i 100")]
        public double TotalPoints { get; set; }
        [Range(0, 100, ErrorMessage = "Ukupan broj poena mora biti izmedju 0 i 100")]
        public double MinimumPoints { get; set; }
    }
}
