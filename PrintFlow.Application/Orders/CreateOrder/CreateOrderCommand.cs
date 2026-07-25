namespace PrintFlow.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    string CustomerName,
    string ProductName,
    int Quantity,
    decimal TotalPrice,
    DateOnly Deadline);