using Microsoft.EntityFrameworkCore;
using SpendSmart.Report.API.Domain.Entities;
using SpendSmart.Report.API.Infrastructure.Data;

namespace SpendSmart.Report.API.Infrastructure.Repositories;

public class ReportRecordRepository : IReportRecordRepository
{
    private readonly ReportDbContext _ctx;

    public ReportRecordRepository(ReportDbContext ctx)
    {
        _ctx = ctx;
    }

    public Task<ReportRecord?> GetByIdAsync(int id) =>
        _ctx.ReportRecords.FindAsync(id).AsTask();

    public Task<List<ReportRecord>> GetByUserIdAsync(int userId) =>
        _ctx.ReportRecords
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();

    public async Task AddAsync(ReportRecord reportRecord) =>
        await _ctx.ReportRecords.AddAsync(reportRecord);

    public Task SaveChangesAsync() =>
        _ctx.SaveChangesAsync();
}