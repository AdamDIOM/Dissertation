using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace Dissertation.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string? Title { get; set; }
        [Required]
        [StringLength(450)]
        public string? AuthorId { get; set; }
        [Required]
        public string? Content { get; set; }
        public string? Tags { get; set; }
        public DateTime? PublishDate { get; set; }

    }
}
