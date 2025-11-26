using System.ComponentModel.DataAnnotations;

namespace FinanceApp.API.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // checking, savings, ewallet, cash, investment

        [Required]
        public decimal Balance { get; set; }

        [MaxLength(7)]
        public string Color { get; set; } = "#3b82f6"; // Hex color for UI

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
