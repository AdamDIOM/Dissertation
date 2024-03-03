using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models
{
    public class FAQ
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Question { get; set; }
        [Required]
        public string? Answer { get; set; }
    }
}
