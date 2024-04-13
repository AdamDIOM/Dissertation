using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        public string? Slug { get; set; }
        [Required]
        [StringLength(150)]
        public string? Title { get; set; }
        [Required]
        [StringLength(450)]
        public string? AuthorId { get; set; }
        [Required]
        public string? Content { get; set; }
        public DateTime? PublishDate { get; set; }
        [Required]
        public bool? HomepageDisplay { get; set; }

    }
}
