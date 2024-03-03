using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models
{
    public class ArticleTagLink
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ArticleId { get; set; }
        [Required]
        public int TagId { get; set; }
    }
}
