using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class Course
    {
        [Key]
        public int ID { get; set; }
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage =
"Naziv predmeta smije imati između 3 i 50 karaktera!")]
        [RegularExpression(@"[0-9| |a-z|A-Z]*", ErrorMessage = 
            "Dozvoljeno je samo korištenje velikih i malih slova, brojeva i razmaka!")]
        public string Name { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherID { get; set; }
        public Teacher? Teacher {  get; set; }

        [Range(2000,2100, ErrorMessage = "Vjerovatno nije akademska godina vam opsega (2000,2100)!")]
        public string AcademicYear { get; set; }


        [Range(1.0, 30.0, ErrorMessage = "Broj ECTS bodova mora biti između 1 i 30!")]
        public int ECTS { get; set; }
        [Range(1,6, ErrorMessage = "Broj semestra ide od I do VI!")]
        public int Semester { get; set; }

        public Course() {

        }


       
    }
}
