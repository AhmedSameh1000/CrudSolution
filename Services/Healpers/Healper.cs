using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Healpers
{
    public class Healper
    {
        internal static void ValidateModel(object model)
        {
            ValidationContext validationContext = new ValidationContext(model);
            var ValidationResults = new List<ValidationResult>();

            bool isvalid = Validator.TryValidateObject(model, validationContext, ValidationResults, true);

            if (!isvalid)
            {
                throw new ArgumentException(ValidationResults.FirstOrDefault()?.ErrorMessage);
            }
        }
    }
}