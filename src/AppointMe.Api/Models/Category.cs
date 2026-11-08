using System.ComponentModel.DataAnnotations;

namespace AppointMe.Api.Models
{
    public class Category
    {
        public int Id { get; set; }

        // Тот же лимит, что varchar(100) в базе: на плохие данные будет 400, а не 500.
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;
    }
}
