using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace ooadproject.Models
{
    public enum RequestType
    {

        [Display(Name = "Potvrda o redovnom studiju")] StudyTestimony,
        [Display(Name = "Uvjerenje o položenim ispitima")] PassedExamsTestimony
    }
}
