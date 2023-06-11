using System.ComponentModel.DataAnnotations;

namespace ooadproject.Models
{
    public enum RequestStatus
    {
        [Display(Name = "U obradi")] Pending, 
        [Display(Name = "Prihvaćen")] Accepted, 
        [Display(Name = "Odbijen")] Rejected
    }
}
