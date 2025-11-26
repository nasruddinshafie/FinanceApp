using FinanceApp.API.Data;
using FinanceApp.API.DTOs;
using FinanceApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.API.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionDto>> GetAllTransactionsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<TransactionDto?> GetTransactionByIdAsync(int id, int userId);
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, int userId);
        Task<TransactionDto?> UpdateTransactionAsync(int id, UpdateTransactionDto dto, int userId);
        Task<bool> DeleteTransactionAsync(int id, int userId);
        Task<List<TransactionDto>> GetTransactionsByAccountAsync(int accountId, int userId);
        Task<List<CategoryExpenseDto>> GetExpensesByCategoryAsync(int userId, int month, int year);
    }

    public class TransactionService : ITransactionService
    {
        private readonly FinanceDbContext _context;

        public TransactionService(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionDto>> GetAllTransactionsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.ToAccount)
                .Where(t => t.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);

            return await query
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    AccountId = t.AccountId,
                    AccountName = t.Account.Name,
                    Description = t.Description,
                    Category = t.Category,
                    Type = t.Type,
                    Amount = t.Amount,
                    ToAccountId = t.ToAccountId,
                    ToAccountName = t.ToAccount != null ? t.ToAccount.Name : null,
                    TransactionDate = t.TransactionDate,
                    Notes = t.Notes,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id, int userId)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.ToAccount)
                .Where(t => t.Id == id && t.UserId == userId)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    AccountId = t.AccountId,
                    AccountName = t.Account.Name,
                    Description = t.Description,
                    Category = t.Category,
                    Type = t.Type,
                    Amount = t.Amount,
                    ToAccountId = t.ToAccountId,
                    ToAccountName = t.ToAccount != null ? t.ToAccount.Name : null,
                    TransactionDate = t.TransactionDate,
                    Notes = t.Notes,
                    CreatedAt = t.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, int userId)
        {
            // Use execution strategy for retry logic
            var strategy = _context.Database.CreateExecutionStrategy();


            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Validate account exists and belongs to user
                    var account = await _context.Accounts
                        .FirstOrDefaultAsync(a => a.Id == dto.AccountId && a.UserId == userId);

                    if (account == null)
                        throw new InvalidOperationException("Account not found");

                    // Create transaction
                    var newTransaction = new Transaction
                    {
                        AccountId = dto.AccountId,
                        Description = dto.Description,
                        Category = dto.Category,
                        Type = dto.Type.ToLower(),
                        Amount = dto.Amount,
                        TransactionDate = dto.TransactionDate,
                        Notes = dto.Notes,
                        UserId = userId
                    };

                    // Update account balance based on transaction type
                    switch (dto.Type.ToLower())
                    {
                        case "income":
                            account.Balance += dto.Amount;
                            break;

                        case "expense":
                            if (account.Balance < dto.Amount)
                                throw new InvalidOperationException("Insufficient balance");
                            account.Balance -= dto.Amount;
                            break;

                        case "transfer":
                            if (!dto.ToAccountId.HasValue)
                                throw new InvalidOperationException("Transfer requires ToAccountId");

                            var toAccount = await _context.Accounts
                                .FirstOrDefaultAsync(a => a.Id == dto.ToAccountId.Value && a.UserId == userId);

                            if (toAccount == null)
                                throw new InvalidOperationException("Destination account not found");

                            if (account.Balance < dto.Amount)
                                throw new InvalidOperationException("Insufficient balance");

                            account.Balance -= dto.Amount;
                            toAccount.Balance += dto.Amount;
                            newTransaction.ToAccountId = dto.ToAccountId.Value;
                            toAccount.UpdatedAt = DateTime.UtcNow;
                            break;

                        default:
                            throw new InvalidOperationException("Invalid transaction type");
                    }

                    account.UpdatedAt = DateTime.UtcNow;

                    _context.Transactions.Add(newTransaction);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Reload transaction with navigation properties
                    var createdTransaction = await _context.Transactions
                        .Include(t => t.Account)
                        .Include(t => t.ToAccount)
                        .FirstAsync(t => t.Id == newTransaction.Id);

                    return new TransactionDto
                    {
                        Id = createdTransaction.Id,
                        AccountId = createdTransaction.AccountId,
                        AccountName = createdTransaction.Account.Name,
                        Description = createdTransaction.Description,
                        Category = createdTransaction.Category,
                        Type = createdTransaction.Type,
                        Amount = createdTransaction.Amount,
                        ToAccountId = createdTransaction.ToAccountId,
                        ToAccountName = createdTransaction.ToAccount?.Name,
                        TransactionDate = createdTransaction.TransactionDate,
                        Notes = createdTransaction.Notes,
                        CreatedAt = createdTransaction.CreatedAt
                    };
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<TransactionDto?> UpdateTransactionAsync(int id, UpdateTransactionDto dto, int userId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
                return null;

            // Note: Updating amount requires recalculating account balances
            // For simplicity, we'll only allow updating description, category, date, and notes
            if (!string.IsNullOrEmpty(dto.Description))
                transaction.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.Category))
                transaction.Category = dto.Category;

            if (dto.TransactionDate.HasValue)
                transaction.TransactionDate = dto.TransactionDate.Value;

            if (dto.Notes != null)
                transaction.Notes = dto.Notes;

            transaction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetTransactionByIdAsync(id, userId);
        }

        public async Task<bool> DeleteTransactionAsync(int id, int userId)
        {
            // Use execution strategy for retry logic
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var transactionToDelete = await _context.Transactions
                        .Include(t => t.Account)
                        .Include(t => t.ToAccount)
                        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

                    if (transactionToDelete == null)
                        return false;

                    // Reverse the account balance changes
                    switch (transactionToDelete.Type.ToLower())
                    {
                        case "income":
                            transactionToDelete.Account.Balance -= transactionToDelete.Amount;
                            break;

                        case "expense":
                            transactionToDelete.Account.Balance += transactionToDelete.Amount;
                            break;

                        case "transfer":
                            if (transactionToDelete.ToAccount != null)
                            {
                                transactionToDelete.Account.Balance += transactionToDelete.Amount;
                                transactionToDelete.ToAccount.Balance -= transactionToDelete.Amount;
                                transactionToDelete.ToAccount.UpdatedAt = DateTime.UtcNow;
                            }
                            break;
                    }

                    transactionToDelete.Account.UpdatedAt = DateTime.UtcNow;

                    _context.Transactions.Remove(transactionToDelete);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<List<TransactionDto>> GetTransactionsByAccountAsync(int accountId, int userId)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.ToAccount)
                .Where(t => (t.AccountId == accountId || t.ToAccountId == accountId) && t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    AccountId = t.AccountId,
                    AccountName = t.Account.Name,
                    Description = t.Description,
                    Category = t.Category,
                    Type = t.Type,
                    Amount = t.Amount,
                    ToAccountId = t.ToAccountId,
                    ToAccountName = t.ToAccount != null ? t.ToAccount.Name : null,
                    TransactionDate = t.TransactionDate,
                    Notes = t.Notes,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<CategoryExpenseDto>> GetExpensesByCategoryAsync(int userId, int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var expenses = await _context.Transactions
                .Where(t => t.UserId == userId
                    && t.Type == "expense"
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate)
                .GroupBy(t => t.Category)
                .Select(g => new CategoryExpenseDto
                {
                    Category = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .ToListAsync();

            var totalExpense = expenses.Sum(e => e.Amount);

            foreach (var expense in expenses)
            {
                expense.Percentage = totalExpense > 0 ? (double)(expense.Amount / totalExpense * 100) : 0;
            }

            return expenses.OrderByDescending(e => e.Amount).ToList();
        }
    }
}
