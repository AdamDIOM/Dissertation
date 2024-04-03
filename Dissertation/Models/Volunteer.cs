using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models
{
    public class Volunteer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
        public string? ImgUrl { get; set; }
        [Required]
        public string? Type { get; set; }
        public string? Title { get; set; }
        public string? Email { get; set; }
        [Required]
        public int PagePosition { get; set; }
        [Required]
        public bool? AdminPermissions { get; set; }
    }
}
