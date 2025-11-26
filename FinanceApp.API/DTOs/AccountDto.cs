namespace FinanceApp.API.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Color { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAccountDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Color { get; set; } = "#3b82f6";
    }

    public class UpdateAccountDto
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public decimal? Balance { get; set; }
        public string? Color { get; set; }
    }

    public class TransactionDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int? ToAccountId { get; set; }
        public string? ToAccountName { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTransactionDto
    {
        public int AccountId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // income, expense, transfer
        public decimal Amount { get; set; }
        public int? ToAccountId { get; set; } // For transfers
        public DateTime TransactionDate { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateTransactionDto
    {
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? Notes { get; set; }
    }


    public class BudgetDto
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount => Amount - SpentAmount;
        public double PercentageUsed => Amount > 0 ? (double)(SpentAmount / Amount * 100) : 0;
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class CreateBudgetDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class UpdateBudgetDto
    {
        public decimal? Amount { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class RegisterDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }

    // Dashboard/Report DTOs
    public class DashboardSummaryDto
    {
        public decimal TotalBalance { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpense { get; set; }
        public decimal NetSavings => MonthlyIncome - MonthlyExpense;
        public List<AccountDto> Accounts { get; set; } = new();
        public List<TransactionDto> RecentTransactions { get; set; } = new();
        public List<CategoryExpenseDto> ExpenseByCategory { get; set; } = new();
    }

    public class CategoryExpenseDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public double Percentage { get; set; }
    }

    public class MonthlyReportDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetSavings => TotalIncome - TotalExpense;
        public List<CategoryExpenseDto> ExpensesByCategory { get; set; } = new();
        public List<BudgetDto> Budgets { get; set; } = new();
    }

}
