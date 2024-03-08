using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace STech_Web.CustomValidations
{
    public class BirthDay : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime birthday = Convert.ToDateTime(value);
            if(!(birthday < DateTime.Now && birthday >= new DateTime(1900, 1, 1)))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(this.ErrorMessage);
            }
            
        }
    }
}