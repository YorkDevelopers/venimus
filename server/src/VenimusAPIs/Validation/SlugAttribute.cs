using System.ComponentModel.DataAnnotations;

namespace VenimusAPIs.Validation
{
    public class SlugAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var slug = (string)value;
            
            if (slug.Contains(" "))
            {
                return new ValidationResult("Slugs cannot contain spaces");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
