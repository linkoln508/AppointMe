using System.ComponentModel.DataAnnotations;

namespace AppointMe.Api.Models
{
    public class Service
    {
        public Guid Id { get; set; }

        // клиент не присылает, идет с адресаа
        public Guid BusinessId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Range(5, 480)]
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; } = true;
    }
}