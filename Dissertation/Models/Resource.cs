using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Link { get; set; }
        public int? Category { get; set; }
        [Required]
        public int? PagePosition { get; set; }
    }
}
