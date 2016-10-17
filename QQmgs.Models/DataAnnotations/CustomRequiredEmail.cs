using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomRequiredEmail : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return null;

            ValidationResult validationResult = null;

            // Check if Model is WorkphoneViewModel, if so, activate validation
            if (validationContext.ObjectInstance is string
             && string.IsNullOrWhiteSpace((string)value))
            {
                this.ErrorMessage = "Phone is required";
                validationResult = new ValidationResult(this.ErrorMessage);
            }
            else
            {
                validationResult = ValidationResult.Success;
            }

            return validationResult;
        }
    }
}
