using PrintFlow.Domain.Orders;

namespace PrintFlow.Application.Orders.CreateOrder;

public sealed record CreateOrderResult(
    Guid Id,
    string CustomerName,
    string ProductName,
    int Quantity,
    decimal TotalPrice,
    DateOnly Deadline,
    OrderStatus Status);