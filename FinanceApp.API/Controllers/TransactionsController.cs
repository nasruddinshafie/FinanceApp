using FinanceApp.API.DTOs;
using FinanceApp.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        /// <summary>
        /// Get all transactions with optional date filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TransactionDto>>> GetAllTransactions(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var userId = GetUserId();
            var transactions = await _transactionService.GetAllTransactionsAsync(userId, startDate, endDate);
            return Ok(transactions);
        }

        /// <summary>
        /// Get transaction by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            var userId = GetUserId();
            var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);

            if (transaction == null)
                return NotFound(new { message = "Transaction not found" });

            return Ok(transaction);
        }

        /// <summary>
        /// Create a new transaction (income, expense, or transfer)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            try
            {
                var userId = GetUserId();
                var transaction = await _transactionService.CreateTransactionAsync(dto, userId);
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing transaction
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionDto>> UpdateTransaction(int id, [FromBody] UpdateTransactionDto dto)
        {
            var userId = GetUserId();
            var transaction = await _transactionService.UpdateTransactionAsync(id, dto, userId);

            if (transaction == null)
                return NotFound(new { message = "Transaction not found" });

            return Ok(transaction);
        }

        /// <summary>
        /// Delete a transaction
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _transactionService.DeleteTransactionAsync(id, userId);

                if (!result)
                    return NotFound(new { message = "Transaction not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all transactions for a specific account
        /// </summary>
        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<List<TransactionDto>>> GetTransactionsByAccount(int accountId)
        {
            var userId = GetUserId();
            var transactions = await _transactionService.GetTransactionsByAccountAsync(accountId, userId);
            return Ok(transactions);
        }

        /// <summary>
        /// Get expenses grouped by category for a specific month
        /// </summary>
        [HttpGet("expenses-by-category")]
        public async Task<ActionResult<List<CategoryExpenseDto>>> GetExpensesByCategory(
            [FromQuery] int month,
            [FromQuery] int year)
        {
            var userId = GetUserId();
            var expenses = await _transactionService.GetExpensesByCategoryAsync(userId, month, year);
            return Ok(expenses);
        }
    }
}
