using PrintFlow.Domain.Common;

namespace PrintFlow.Domain.Orders;

public sealed class Order
{
    public Guid Id { get; private set; }

    public string CustomerName { get; private set; } = string.Empty;

    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal TotalPrice { get; private set; }

    public DateOnly Deadline { get; private set; }

    public OrderStatus Status { get; private set; }

    private Order()
    {
        // Required later by Entity Framework Core.
    }

    public Order(
        string customerName,
        string productName,
        int quantity,
        decimal totalPrice,
        DateOnly deadline)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new DomainException("Customer name is required.");
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new DomainException("Product name is required.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }

        if (totalPrice < 0)
        {
            throw new DomainException("Total price cannot be negative.");
        }

        Id = Guid.NewGuid();
        CustomerName = customerName.Trim();
        ProductName = productName.Trim();
        Quantity = quantity;
        TotalPrice = totalPrice;
        Deadline = deadline;
        Status = OrderStatus.Draft;
    }
}