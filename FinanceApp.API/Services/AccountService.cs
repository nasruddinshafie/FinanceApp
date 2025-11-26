using FinanceApp.API.Data;
using FinanceApp.API.DTOs;
using FinanceApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.API.Services
{

    public interface IAccountService
    {
        Task<List<DTOs.AccountDto>> GetAllAccountsAsync(int userId);
        Task<DTOs.AccountDto?> GetAccountByIdAsync(int id, int userId);
        Task<DTOs.AccountDto> CreateAccountAsync(CreateAccountDto dto, int userId);
        Task<DTOs.AccountDto?> UpdateAccountAsync(int id, UpdateAccountDto dto, int userId);
        Task<bool> DeleteAccountAsync(int id, int userId);
        Task<decimal> GetTotalBalanceAsync(int userId);
    }

    public class AccountService : IAccountService
    {
        private readonly FinanceDbContext _context;

        public AccountService(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<List<AccountDto>> GetAllAccountsAsync(int userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new AccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Type = a.Type,
                    Balance = a.Balance,
                    Color = a.Color,
                    CreatedAt = a.CreatedAt
                })
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<AccountDto?> GetAccountByIdAsync(int id, int userId)
        {
            var account = await _context.Accounts
                .Where(a => a.Id == id && a.UserId == userId)
                .Select(a => new AccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Type = a.Type,
                    Balance = a.Balance,
                    Color = a.Color,
                    CreatedAt = a.CreatedAt
                })
                .FirstOrDefaultAsync();

            return account;
        }

        public async Task<AccountDto> CreateAccountAsync(CreateAccountDto dto, int userId)
        {
            var account = new Account
            {
                Name = dto.Name,
                Type = dto.Type,
                Balance = dto.Balance,
                Color = dto.Color,
                UserId = userId
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return new AccountDto
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type,
                Balance = account.Balance,
                Color = account.Color,
                CreatedAt = account.CreatedAt
            };
        }

        public async Task<AccountDto?> UpdateAccountAsync(int id, UpdateAccountDto dto, int userId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (account == null)
                return null;

            if (!string.IsNullOrEmpty(dto.Name))
                account.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Type))
                account.Type = dto.Type;

            if (dto.Balance.HasValue)
                account.Balance = dto.Balance.Value;

            if (!string.IsNullOrEmpty(dto.Color))
                account.Color = dto.Color;

            account.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new DTOs.AccountDto
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type,
                Balance = account.Balance,
                Color = account.Color,
                CreatedAt = account.CreatedAt
            };
        }

        public async Task<bool> DeleteAccountAsync(int id, int userId)
        {
            var account = await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (account == null)
                return false;

            // Check if account has transactions
            if (account.Transactions.Any())
            {
                throw new InvalidOperationException("Cannot delete account with existing transactions");
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetTotalBalanceAsync(int userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .SumAsync(a => a.Balance);
        }
    }
}
