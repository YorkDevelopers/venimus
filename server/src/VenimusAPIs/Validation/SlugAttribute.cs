using System.ComponentModel.DataAnnotations;

namespace VenimusAPIs.Validation
{
    public class SlugAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var slug = (string)value;
            
            if (string.IsNullOrWhiteSpace(slug))
            {
                return new ValidationResult("The slug must be provided");
            }
            else if (slug.Contains(" "))
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
