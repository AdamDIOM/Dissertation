using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models
{
    public class Sponsor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Information { get; set; }
    }
}
