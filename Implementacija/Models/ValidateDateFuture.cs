using System.ComponentModel.DataAnnotations;

namespace ooadproject.Models
{
    public class ValidateDateFuture : ValidationAttribute
    {
        protected override ValidationResult IsValid
        (object date, ValidationContext validationContext)
        {
            return ((DateTime)date >= DateTime.Now)
            ? ValidationResult.Success
            : new ValidationResult("Mora rok zadace biti u buducnosti!");
        }
    }
}
