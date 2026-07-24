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

    public void Confirm()
    {
        ChangeStatus(expectedCurrentStatus: OrderStatus.Draft, newStatus: OrderStatus.Confirmed);
    }

    public void Schedule()
    {
        ChangeStatus(expectedCurrentStatus: OrderStatus.Confirmed, newStatus: OrderStatus.Scheduled);
    }
    
    public void StartPrinting()
    {
        ChangeStatus(
            expectedCurrentStatus: OrderStatus.Scheduled,
            newStatus: OrderStatus.Printing);
    }

    public void MoveToWorkshop()
    {
        ChangeStatus(
            expectedCurrentStatus: OrderStatus.Printing,
            newStatus: OrderStatus.Workshop);
    }

    public void MarkReadyForDelivery()
    {
        ChangeStatus(
            expectedCurrentStatus: OrderStatus.Workshop,
            newStatus: OrderStatus.ReadyForDelivery);
    }

    public void Complete()
    {
        ChangeStatus(
            expectedCurrentStatus: OrderStatus.ReadyForDelivery,
            newStatus: OrderStatus.Completed);
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
        {
            throw new DomainException("A completed order cannot be cancelled.");
        }
        
        if (Status == OrderStatus.Cancelled)
        {
            throw new DomainException("The order is already cancelled.");
        }
        
        Status = OrderStatus.Cancelled;
    }

    private void ChangeStatus(OrderStatus expectedCurrentStatus, OrderStatus newStatus)
    {
        if (Status != expectedCurrentStatus)
        {
            throw new DomainException(
                $"Order must be in {expectedCurrentStatus} status before moving to {newStatus}.");
        }
        
        Status = newStatus;
    }
}