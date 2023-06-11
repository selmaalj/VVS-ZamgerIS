using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ooadproject.Models
{
    public class ValidateDate : ValidationAttribute
    {
        protected override ValidationResult IsValid
        (object date, ValidationContext validationContext)
        {
            return ((DateTime)date >= DateTime.Now.AddDays(-14) &&
           (DateTime)date <= DateTime.Now)
            ? ValidationResult.Success
            : new ValidationResult("Validan je upis između 14 dana u prošlosti i danas!");
        }
    }

}
