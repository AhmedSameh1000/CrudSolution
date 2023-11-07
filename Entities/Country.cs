using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Country
    {
        public Guid Id { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }
    }
}