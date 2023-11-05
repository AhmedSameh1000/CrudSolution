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
    public class PersonForUpdateDTO
    {
        [Required(ErrorMessage = "ID Cant Be Blank")]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "Person Name Cant Be Blank")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email Cant Be Blank")]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public bool ReceiveEmails { get; set; }

        public Person ToPerson()
        {
            return new() { Id = ID, Name = Name, Email = Email, DateOfBirth = DateOfBirth, Gender = Gender.ToString(), CountryId = CountryId, ReceiveEmails = ReceiveEmails };
        }
    }
}