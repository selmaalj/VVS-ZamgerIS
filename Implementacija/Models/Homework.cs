using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class Homework
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }
        [Required]
        public Course? Course { get; set; }
        [ValidateDateFuture]
        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }
        [Range(0,100,ErrorMessage = "Ne moze zadaca imati poene van opsega od 0 do 100!")]
        public double TotalPoints { get; set; }
        public string Description { get; set; }

        public Homework() { }
    }
}
