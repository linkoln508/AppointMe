using System.ComponentModel.DataAnnotations;

namespace AppointMe.Api.Models
{
    public class Business
    {
        public Guid Id { get; set; }

        public int CategoryId { get; set; }

        // Заполняется только при чтении (через JOIN), клиент его не присылает.
        public string? CategoryName { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}