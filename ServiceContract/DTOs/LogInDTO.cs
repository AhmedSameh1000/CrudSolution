using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.DTOs
{
    public class LogInDTO
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
        [Required(ErrorMessage = "Email cant't be blank")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password can't be blank")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}