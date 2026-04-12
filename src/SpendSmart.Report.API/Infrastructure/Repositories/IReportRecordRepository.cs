using SpendSmart.Report.API.Domain.Entities;

namespace SpendSmart.Report.API.Infrastructure.Repositories;

public interface IReportRecordRepository
{
    Task<ReportRecord?> GetByIdAsync(int id);
    Task<List<ReportRecord>> GetByUserIdAsync(int userId);
    Task AddAsync(ReportRecord reportRecord);
    Task SaveChangesAsync();
}