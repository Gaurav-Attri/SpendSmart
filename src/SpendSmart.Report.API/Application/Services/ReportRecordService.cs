using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SpendSmart.Report.API.Application.DTOs;
using SpendSmart.Report.API.Domain.Entities;
using SpendSmart.Report.API.Infrastructure.HttpClients;
using SpendSmart.Report.API.Infrastructure.Repositories;

namespace SpendSmart.Report.API.Application.Services;

public class ReportRecordService : IReportRecordService
{
    private readonly IExpenseHttpClient _expenseClient;
    private readonly IIncomeHttpClient _incomeClient;
    private readonly IReportRecordRepository _repo;
    private readonly IConfiguration _config;
    private readonly ILogger<ReportRecordService> _logger;

    public ReportRecordService(
        IExpenseHttpClient expenseClient,
        IIncomeHttpClient incomeClient,
        IReportRecordRepository repo,
        IConfiguration config,
        ILogger<ReportRecordService> logger)
    {
        _expenseClient = expenseClient;
        _incomeClient = incomeClient;
        _repo = repo;
        _config = config;
        _logger = logger;
    }

    private string GenerateSasUrl(string blobUrl)
    {
        try
        {
            var blobServiceClient = new BlobServiceClient(_config["Azure:BlobConnectionString"]);
            var uri = new Uri(blobUrl);
            var containerName = uri.Segments[1].TrimEnd('/');
            var blobName = string.Join("", uri.Segments.Skip(2));
            
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(7)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            
            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate SAS URL for {BlobUrl}", blobUrl);
            return blobUrl;
        }
    }

    public async Task<MonthlySummaryDto> GetMonthlySummaryAsync(int userId, int month, int year, string token)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var expenses = await _expenseClient.GetByDateRangeAsync(userId, startDate, endDate, token);
        var incomes = await _incomeClient.GetByUserAsync(userId, token);

        var totalExpense = expenses.Sum(e => e.Amount);
        var totalIncome = incomes.Sum(i => i.Amount);
        var netBalance = totalIncome - totalExpense;
        var savingsRate = totalIncome > 0 ? ((totalIncome - totalExpense) / totalIncome) * 100 : 0;

        var categoryBreakdown = expenses
            .GroupBy(e => e.CategoryId)
            .ToDictionary(g => g.Key.ToString(), g => g.Sum(e => e.Amount));

        var topCategory = categoryBreakdown.Count > 0 
            ? categoryBreakdown.OrderByDescending(kv => kv.Value).First().Key 
            : "N/A";

        return new MonthlySummaryDto
        {
            TotalExpense = totalExpense,
            TotalIncome = totalIncome,
            NetBalance = netBalance,
            SavingsRate = savingsRate,
            TopCategory = topCategory,
            CategoryBreakdown = categoryBreakdown
        };
    }

    public async Task<List<Dictionary<string, object>>> GetCategoryBreakdownAsync(int userId, DateTime start, DateTime end, string token)
    {
        var expenses = await _expenseClient.GetByDateRangeAsync(userId, start, end, token);
        
        return expenses
            .GroupBy(e => e.CategoryId)
            .Select(g => new Dictionary<string, object>
            {
                ["CategoryId"] = g.Key,
                ["TotalAmount"] = g.Sum(e => e.Amount)
            })
            .OrderByDescending(d => Convert.ToDecimal(d["TotalAmount"]))
            .ToList();
    }

    public async Task<Dictionary<string, decimal>> GetTrendAnalysisAsync(int userId, int months, string token)
    {
        var trend = new Dictionary<string, decimal>();
        var endDate = DateTime.UtcNow;
        
        for (int i = months - 1; i >= 0; i--)
        {
            var monthDate = endDate.AddMonths(-i);
            var monthName = monthDate.ToString("MMM yyyy");
            var startDate = new DateTime(monthDate.Year, monthDate.Month, 1);
            var monthEndDate = startDate.AddMonths(1).AddDays(-1);
            
            var expenses = await _expenseClient.GetByDateRangeAsync(userId, startDate, monthEndDate, token);
            var totalExpense = expenses.Sum(e => e.Amount);
            trend[monthName] = totalExpense;
        }
        
        return trend;
    }

    public async Task<decimal> GetSavingsRateAsync(int userId, int month, int year, string token)
    {
        var summary = await GetMonthlySummaryAsync(userId, month, year, token);
        return summary.SavingsRate;
    }

    public async Task<string> GeneratePdfReportAsync(GenerateReportDto dto, string token)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        MonthlySummaryDto summary;
        
        if (dto.StartDate.HasValue && dto.EndDate.HasValue)
        {
            var expenses = await _expenseClient.GetByDateRangeAsync(dto.UserId, dto.StartDate.Value, dto.EndDate.Value, token);
            var incomes = await _incomeClient.GetByUserAsync(dto.UserId, token);
            
            var totalExpense = expenses.Sum(e => e.Amount);
            var totalIncome = incomes.Sum(i => i.Amount);
            
            summary = new MonthlySummaryDto
            {
                TotalExpense = totalExpense,
                TotalIncome = totalIncome,
                NetBalance = totalIncome - totalExpense,
                SavingsRate = totalIncome > 0 ? ((totalIncome - totalExpense) / totalIncome) * 100 : 0,
                TopCategory = "N/A",
                CategoryBreakdown = new Dictionary<string, decimal>()
            };
        }
        else
        {
            var month = dto.Month ?? DateTime.UtcNow.Month;
            var year = dto.Year ?? DateTime.UtcNow.Year;
            summary = await GetMonthlySummaryAsync(dto.UserId, month, year, token);
        }

        byte[] pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.Header()
                    .Text($"SpendSmart Report - {DateTime.UtcNow:MMMM yyyy}")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Green.Darken3);
                
                page.Content()
                    .Column(col =>
                    {
                        col.Item().PaddingBottom(10).Text($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                        col.Item().PaddingBottom(5).Text($"Total Expense: {summary.TotalExpense:C}").FontSize(12);
                        col.Item().PaddingBottom(5).Text($"Total Income: {summary.TotalIncome:C}").FontSize(12);
                        col.Item().PaddingBottom(5).Text($"Net Balance: {summary.NetBalance:C}").FontSize(12);
                        col.Item().PaddingBottom(5).Text($"Savings Rate: {summary.SavingsRate:F1}%").FontSize(12);
                        col.Item().PaddingBottom(5).Text($"Top Category: {summary.TopCategory}").FontSize(12);
                    });
                
                page.Footer()
                    .AlignRight()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
            });
        }).GeneratePdf();

        var blobName = $"reports/user-{dto.UserId}-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var blobServiceClient = new BlobServiceClient(_config["Azure:BlobConnectionString"]);
        var containerClient = blobServiceClient.GetBlobContainerClient(_config["Azure:ReportsContainer"] ?? "reports");
        await containerClient.CreateIfNotExistsAsync();
        
        var blobClient = containerClient.GetBlobClient(blobName);
        using var stream = new MemoryStream(pdfBytes);
        await blobClient.UploadAsync(stream, overwrite: true);
        
        var blobUrl = blobClient.Uri.ToString();
        var sasUrl = GenerateSasUrl(blobUrl);

        var reportRecord = new ReportRecord
        {
            UserId = dto.UserId,
            ReportType = dto.ReportType,
            Title = $"Monthly Report {DateTime.UtcNow:MMMM yyyy}",
            FilePath = blobUrl,
            Status = "GENERATED",
            Parameters = System.Text.Json.JsonSerializer.Serialize(dto)
        };
        
        await _repo.AddAsync(reportRecord);
        await _repo.SaveChangesAsync();

        _logger.LogInformation("PDF report generated for user {UserId}. SAS URL: {SasUrl}", dto.UserId, sasUrl);

        return sasUrl;
    }

    public Task<List<ReportRecord>> GetUserReportsAsync(int userId) =>
        _repo.GetByUserIdAsync(userId);
}