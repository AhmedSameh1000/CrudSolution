using Entities;
using ServiceContract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.DTOs
{
    public class PersonForCreateDTO
    {
        [Required(ErrorMessage = "Person Name Cant Be Blank")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email Cant Be Blank")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Select Country")]
        public Guid? CountryId { get; set; }

        [Required]
        public bool ReceiveEmails { get; set; }

        public Person ToPerson()
        {
            return new() { Name = Name, Email = Email, DateOfBirth = DateOfBirth, Gender = Gender.ToString(), CountryId = CountryId, ReceiveEmails = ReceiveEmails };
        }
    }
}