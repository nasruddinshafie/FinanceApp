using FinanceApp.API.DTOs;
using FinanceApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        /// <summary>
        /// Get all accounts for the current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<AccountDto>>> GetAllAccounts()
        {
            var userId = GetUserId();
            var accounts = await _accountService.GetAllAccountsAsync(userId);
            return Ok(accounts);
        }

        /// <summary>
        /// Get account by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetAccount(int id)
        {
            var userId = GetUserId();
            var account = await _accountService.GetAccountByIdAsync(id, userId);

            if (account == null)
                return NotFound(new { message = "Account not found" });

            return Ok(account);
        }

        /// <summary>
        /// Create a new account
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AccountDto>> CreateAccount([FromBody] CreateAccountDto dto)
        {
            var userId = GetUserId();
            var account = await _accountService.CreateAccountAsync(dto, userId);
            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
        }

        /// <summary>
        /// Update an existing account
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<AccountDto>> UpdateAccount(int id, [FromBody] UpdateAccountDto dto)
        {
            var userId = GetUserId();
            var account = await _accountService.UpdateAccountAsync(id, dto, userId);

            if (account == null)
                return NotFound(new { message = "Account not found" });

            return Ok(account);
        }

        /// <summary>
        /// Delete an account
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAccount(int id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _accountService.DeleteAccountAsync(id, userId);

                if (!result)
                    return NotFound(new { message = "Account not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get total balance across all accounts
        /// </summary>
        [HttpGet("total-balance")]
        public async Task<ActionResult<decimal>> GetTotalBalance()
        {
            var userId = GetUserId();
            var totalBalance = await _accountService.GetTotalBalanceAsync(userId);
            return Ok(new { totalBalance });
        }

    }
}
