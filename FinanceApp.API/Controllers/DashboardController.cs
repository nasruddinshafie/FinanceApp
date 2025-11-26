using FinanceApp.API.DTOs;
using FinanceApp.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        /// <summary>
        /// Get dashboard summary data
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary()
        {
            var userId = GetUserId();
            var summary = await _dashboardService.GetDashboardSummaryAsync(userId);
            return Ok(summary);
        }

        /// <summary>
        /// Get monthly report
        /// </summary>
        [HttpGet("monthly-report")]
        public async Task<ActionResult<MonthlyReportDto>> GetMonthlyReport(
            [FromQuery] int month,
            [FromQuery] int year)
        {
            var userId = GetUserId();
            var report = await _dashboardService.GetMonthlyReportAsync(userId, month, year);
            return Ok(report);
        }

    }

}
