using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models
{
    public class Policy
    {
        [Key]
        public string? Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Link { get; set; }
        [Required]
        public DateTime? DateUpdated { get; set; }
        [Required]
        public string? UploadedBy { get; set; }
    }
}
