using Microsoft.EntityFrameworkCore;
using FinanceApp.API.Data;
using FinanceApp.API.DTOs;

namespace FinanceApp.API.Services
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(int userId);
        Task<MonthlyReportDto> GetMonthlyReportAsync(int userId, int month, int year);
    }

    public class DashboardService : IDashboardService
    {
        private readonly FinanceDbContext _context;
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;

        public DashboardService(FinanceDbContext context, IAccountService accountService, ITransactionService transactionService)
        {
            _context = context;
            _accountService = accountService;
            _transactionService = transactionService;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(int userId)
        {
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            var startDate = new DateTime(currentYear, currentMonth, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Get all accounts
            var accounts = await _accountService.GetAllAccountsAsync(userId);

            // Get total balance
            var totalBalance = await _accountService.GetTotalBalanceAsync(userId);

            // Get monthly income
            var monthlyIncome = await _context.Transactions
                .Where(t => t.UserId == userId
                    && t.Type == "income"
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate)
                .SumAsync(t => t.Amount);

            // Get monthly expense
            var monthlyExpense = await _context.Transactions
                .Where(t => t.UserId == userId
                    && t.Type == "expense"
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate)
                .SumAsync(t => t.Amount);

            // Get recent transactions (last 10)
            var recentTransactions = await _transactionService.GetAllTransactionsAsync(userId);
            recentTransactions = recentTransactions.Take(10).ToList();

            // Get expense by category for current month
            var expenseByCategory = await _transactionService.GetExpensesByCategoryAsync(userId, currentMonth, currentYear);

            return new DashboardSummaryDto
            {
                TotalBalance = totalBalance,
                MonthlyIncome = monthlyIncome,
                MonthlyExpense = monthlyExpense,
                Accounts = accounts,
                RecentTransactions = recentTransactions,
                ExpenseByCategory = expenseByCategory
            };
        }

        public async Task<MonthlyReportDto> GetMonthlyReportAsync(int userId, int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Get monthly income
            var totalIncome = await _context.Transactions
                .Where(t => t.UserId == userId
                    && t.Type == "income"
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate)
                .SumAsync(t => t.Amount);

            // Get monthly expense
            var totalExpense = await _context.Transactions
                .Where(t => t.UserId == userId
                    && t.Type == "expense"
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate)
                .SumAsync(t => t.Amount);

            // Get expenses by category
            var expensesByCategory = await _transactionService.GetExpensesByCategoryAsync(userId, month, year);

            // Get budgets for the month
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
                .Select(b => new BudgetDto
                {
                    Id = b.Id,
                    Category = b.Category,
                    Amount = b.Amount,
                    SpentAmount = b.SpentAmount,
                    Month = b.Month,
                    Year = b.Year
                })
                .ToListAsync();

            // Calculate spent amounts for budgets
            foreach (var budget in budgets)
            {
                var spent = await _context.Transactions
                    .Where(t => t.UserId == userId
                        && t.Type == "expense"
                        && t.Category == budget.Category
                        && t.TransactionDate >= startDate
                        && t.TransactionDate <= endDate)
                    .SumAsync(t => t.Amount);

                budget.SpentAmount = spent;
            }

            return new MonthlyReportDto
            {
                Month = month,
                Year = year,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                ExpensesByCategory = expensesByCategory,
                Budgets = budgets
            };
        }
    }
}
