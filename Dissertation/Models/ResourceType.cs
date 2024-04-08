using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models
{
    public class ResourceType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Type { get; set; }
    }
}
