using SpendSmart.Report.API.Application.DTOs;
using SpendSmart.Report.API.Domain.Entities;

namespace SpendSmart.Report.API.Application.Services;

public interface IReportRecordService
{
    Task<MonthlySummaryDto> GetMonthlySummaryAsync(int userId, int month, int year, string token);
    Task<List<Dictionary<string, object>>> GetCategoryBreakdownAsync(int userId, DateTime start, DateTime end, string token);
    Task<Dictionary<string, decimal>> GetTrendAnalysisAsync(int userId, int months, string token);
    Task<decimal> GetSavingsRateAsync(int userId, int month, int year, string token);
    Task<string> GeneratePdfReportAsync(GenerateReportDto dto, string token);  // ← Returns SAS URL
    Task<List<ReportRecord>> GetUserReportsAsync(int userId);
}