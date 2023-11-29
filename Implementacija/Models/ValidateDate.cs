using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ooadproject.Models
{
    public class ValidateDate : ValidationAttribute
    {
        protected override ValidationResult IsValid
        (object date, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }
    }

}
