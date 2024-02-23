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
        public string? LinkedInEnding { get; set; }
        public string? ImgUrl { get; set; }
        [Required]
        public string? Level { get; set; }
        public string? Title { get; set; }
    }
}
