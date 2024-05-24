using System.ComponentModel.DataAnnotations;

namespace ProtectedWeb.Attributes.Validations.User
{
    public class UniqueEmail : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (SneakersContext)validationContext.GetService(typeof(SneakersContext));
            var isEmailUnique = !dbContext.Users.Any(u => u.Email == (string)value);

            if (!isEmailUnique)
            {
                return new ValidationResult("Email должен быть уникальным");
            }

            return ValidationResult.Success;
        }
    }
}
