namespace PrintFlow.Domain.Orders;

public enum OrderStatus
{
    Draft = 1,
    Confirmed = 2,
    Scheduled = 3,
    Printing = 4,
    Workshop = 5,
    ReadyForDelivery = 6,
    Completed = 7,
    Cancelled = 8
}