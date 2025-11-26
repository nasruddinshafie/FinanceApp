using System.ComponentModel.DataAnnotations;

namespace FinanceApp.API.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AccountId { get; set; }
        public Account Account { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty; // Makanan, Transport, Bil Utilities, etc

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty; // income, expense, transfer

        [Required]
        public decimal Amount { get; set; }

        // For transfers
        public int? ToAccountId { get; set; }
        public Account? ToAccount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
