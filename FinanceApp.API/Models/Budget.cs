using System.ComponentModel.DataAnnotations;

namespace FinanceApp.API.Models
{
    public class Budget
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty; // Makanan, Transport, etc

        [Required]
        public decimal Amount { get; set; } // Budget limit untuk kategori ni

        [Required]
        public int Month { get; set; } // 1-12

        [Required]
        public int Year { get; set; }

        public decimal SpentAmount { get; set; } = 0; // Calculated field

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
