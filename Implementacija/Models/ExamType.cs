using System.ComponentModel.DataAnnotations;

namespace ooadproject.Models
{
    public enum ExamType

    {
        [Display(Name = "Parcijalni")] Midterm,
        [Display(Name = "Završni")] Final, 
        [Display(Name = "Integralni")] Integrated, 
        [Display(Name = "Test")] Test,
        [Display(Name = "Usmeni")] Oral
    }
}
