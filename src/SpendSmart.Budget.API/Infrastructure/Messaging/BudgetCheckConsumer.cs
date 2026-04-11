using MassTransit;
using SpendSmart.Budget.API.Application.Services;
using SpendSmart.Contracts.Events;

namespace SpendSmart.Budget.API.Infrastructure.Messaging;

public class BudgetCheckConsumer : IConsumer<ExpenseCreatedEvent>
{
    private readonly IBudgetPlanService _budgetSvc;

    public BudgetCheckConsumer(IBudgetPlanService budgetSvc)
    {
        _budgetSvc = budgetSvc;
    }

    public async Task Consume(ConsumeContext<ExpenseCreatedEvent> context)
    {
        var msg = context.Message;
        await _budgetSvc.CheckBudgetOnExpenseAsync(msg.UserId, msg.CategoryId, msg.Amount);
    }
}