using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Report.API.Application.DTOs;
using SpendSmart.Report.API.Application.Services;
using SpendSmart.Report.API.Domain.Entities;

namespace SpendSmart.Report.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IReportRecordService _svc;

    public ReportController(IReportRecordService svc) => _svc = svc;

    private string GetToken()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return string.Empty;
        return authHeader.Substring("Bearer ".Length);
    }

    [HttpGet("monthly/{userId}/{month}/{year}")]
    [ProducesResponseType(typeof(MonthlySummaryDto), 200)]
    public async Task<IActionResult> GetMonthlySummary(int userId, int month, int year)
    {
        if (month < 1 || month > 12)
            return BadRequest(new { error = "Month must be between 1 and 12" });
        
        if (year < 2000 || year > DateTime.UtcNow.Year + 1)
            return BadRequest(new { error = "Invalid year" });

        var token = GetToken();
        var summary = await _svc.GetMonthlySummaryAsync(userId, month, year, token);
        return Ok(summary);
    }

    [HttpGet("category-breakdown")]
    [ProducesResponseType(typeof(List<Dictionary<string, object>>), 200)]
    public async Task<IActionResult> GetCategoryBreakdown(
        [FromQuery] int userId,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        if (start > end)
            return BadRequest(new { error = "Start date must be before end date" });

        var token = GetToken();
        var breakdown = await _svc.GetCategoryBreakdownAsync(userId, start, end, token);
        return Ok(breakdown);
    }

    [HttpGet("trend/{userId}/{months}")]
    [ProducesResponseType(typeof(Dictionary<string, decimal>), 200)]
    public async Task<IActionResult> GetTrendAnalysis(int userId, int months)
    {
        if (months < 1 || months > 24)
            return BadRequest(new { error = "Months must be between 1 and 24" });

        var token = GetToken();
        var trend = await _svc.GetTrendAnalysisAsync(userId, months, token);
        return Ok(trend);
    }

    [HttpGet("savings-rate/{userId}/{month}/{year}")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetSavingsRate(int userId, int month, int year)
    {
        if (month < 1 || month > 12)
            return BadRequest(new { error = "Month must be between 1 and 12" });

        var token = GetToken();
        var rate = await _svc.GetSavingsRateAsync(userId, month, year, token);
        return Ok(new { userId, month, year, savingsRate = rate });
    }

    [HttpPost("pdf")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GeneratePdf([FromBody] GenerateReportDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var token = GetToken();
            var downloadUrl = await _svc.GeneratePdfReportAsync(dto, token);  // ← Returns SAS URL
            return Ok(new { downloadUrl, message = "Report generated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<ReportRecord>), 200)]
    public async Task<IActionResult> GetUserReports(int userId)
    {
        var reports = await _svc.GetUserReportsAsync(userId);
        return Ok(reports);
    }
}