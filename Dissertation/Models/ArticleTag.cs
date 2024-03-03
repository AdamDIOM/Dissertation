using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models
{
    public class ArticleTag
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Tag { get; set; }
        [Required]
        public bool? NavDisplay { get; set; }
    }
}
