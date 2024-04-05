using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Caption { get; set; }
        [Required]
        public string? Description { get; set; }
        public string? Link { get; set; }
        [Required]
        public bool? HomepageBannerDisplay { get; set; }
        [Required]
        public string? UploadedBy { get; set; }
    }
}
