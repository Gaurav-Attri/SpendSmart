namespace SpendSmart.Contracts.Events;

public record ExpenseCreatedEvent(
    int UserId,
    int CategoryId,
    decimal Amount,
    string Currency
);