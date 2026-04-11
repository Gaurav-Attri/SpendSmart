using Microsoft.EntityFrameworkCore;
using SpendSmart.Budget.API.Infrastructure.Data;

namespace SpendSmart.Budget.API.Infrastructure.BackgroundServices;

public class BudgetResetService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BudgetResetService> _logger;

    public BudgetResetService(IServiceProvider services, ILogger<BudgetResetService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextReset = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);
            var delay = nextReset - now;

            _logger.LogInformation($"Budget reset scheduled in {delay.TotalHours:F1} hours");
            await Task.Delay(delay, stoppingToken);

            using var scope = _services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<BudgetDbContext>();

            var resetCount = await ctx.BudgetPlans
                .Where(b => b.Period == "MONTHLY" && b.IsActive)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.SpentAmount, 0m), stoppingToken);

            _logger.LogInformation($"Reset SpentAmount for {resetCount} monthly budgets.");
        }
    }
}