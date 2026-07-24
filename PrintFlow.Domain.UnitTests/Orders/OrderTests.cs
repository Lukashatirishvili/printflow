using PrintFlow.Domain.Common;
using PrintFlow.Domain.Orders;
using Xunit;

namespace PrintFlow.Domain.UnitTests.Orders;

public sealed class OrderTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesDraftOrder()
    {
        // Arrange
        const string customerName = "Meridian Corp";
        const string productName = "Business Cards";
        const int quantity = 1_000;
        const decimal totalPrice = 3_200m;
        var deadline = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));

        // Act
        var order = new Order(
            customerName,
            productName,
            quantity,
            totalPrice,
            deadline);

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customerName, order.CustomerName);
        Assert.Equal(productName, order.ProductName);
        Assert.Equal(quantity, order.Quantity);
        Assert.Equal(totalPrice, order.TotalPrice);
        Assert.Equal(deadline, order.Deadline);
        Assert.Equal(OrderStatus.Draft, order.Status);
    }

    [Fact]
    public void Constructor_WithMissingCustomerName_ThrowsDomainException()
    {
        // Act
        var action = () => new Order(
            customerName: " ",
            productName: "Business Cards",
            quantity: 1_000,
            totalPrice: 3_200m,
            deadline: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));

        // Assert
        var exception = Assert.Throws<DomainException>(action);

        Assert.Equal(
            "Customer name is required.",
            exception.Message);
    }

    [Fact]
    public void Constructor_WithMissingProductName_ThrowsDomainException()
    {
        // Act
        var action = () => new Order(
            customerName: "Meridian Corp",
            productName: "",
            quantity: 1_000,
            totalPrice: 3_200m,
            deadline: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));

        // Assert
        var exception = Assert.Throws<DomainException>(action);

        Assert.Equal(
            "Product name is required.",
            exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithNonPositiveQuantity_ThrowsDomainException(
        int invalidQuantity)
    {
        // Act
        var action = () => new Order(
            customerName: "Meridian Corp",
            productName: "Business Cards",
            quantity: invalidQuantity,
            totalPrice: 3_200m,
            deadline: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));

        // Assert
        var exception = Assert.Throws<DomainException>(action);

        Assert.Equal(
            "Quantity must be greater than zero.",
            exception.Message);
    }

    [Fact]
    public void Constructor_WithNegativeTotalPrice_ThrowsDomainException()
    {
        // Act
        var action = () => new Order(
            customerName: "Meridian Corp",
            productName: "Business Cards",
            quantity: 1_000,
            totalPrice: -1m,
            deadline: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));

        // Assert
        var exception = Assert.Throws<DomainException>(action);

        Assert.Equal(
            "Total price cannot be negative.",
            exception.Message);
    }

    [Fact]
    public void Confirm_WhenOrderIsDraft_ChangesStatusToConfirmed()
    {
        // Arrange
        var order = CreateValidOrder();

        // Act
        order.Confirm();

        // Assert
        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void Confirm_WhenOrderIsAlreadyConfirmed_ThrowsDomainException()
    {
        // Arrange
        var order = CreateValidOrder();
        order.Confirm();

        // Act
        var action = () => order.Confirm();

        // Assert
        var exception = Assert.Throws<DomainException>(action);

        Assert.Equal(
            "Order must be in Draft status before moving to Confirmed.",
            exception.Message);
    }

    [Fact]
    public void Order_CanProgressThroughCompleteWorkflow()
    {
        // Arrange
        var order = CreateValidOrder();

        // Act and Assert
        order.Confirm();
        Assert.Equal(OrderStatus.Confirmed, order.Status);

        order.Schedule();
        Assert.Equal(OrderStatus.Scheduled, order.Status);

        order.StartPrinting();
        Assert.Equal(OrderStatus.Printing, order.Status);

        order.MoveToWorkshop();
        Assert.Equal(OrderStatus.Workshop, order.Status);

        order.MarkReadyForDelivery();
        Assert.Equal(OrderStatus.ReadyForDelivery, order.Status);

        order.Complete();
        Assert.Equal(OrderStatus.Completed, order.Status);
    }

    [Fact]
    public void StartPrinting_WhenOrderIsDraft_ThrowsDomainException()
    {
        // Arrange
        var order = CreateValidOrder();

        // Act
        var action = () => order.StartPrinting();

        // Assert
        var exception = Assert.Throws<DomainException>(action);

        Assert.Equal(
            "Order must be in Scheduled status before moving to Printing.",
            exception.Message);
    }

    [Fact]
    public void Cancel_WhenOrderIsActive_ChangesStatusToCancelled()
    {
        // Arrange
        var order = CreateValidOrder();
        order.Confirm();

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_WhenOrderIsCompleted_ThrowsDomainException()
    {
        // Arrange
        var order = CreateValidOrder();

        order.Confirm();
        order.Schedule();
        order.StartPrinting();
        order.MoveToWorkshop();
        order.MarkReadyForDelivery();
        order.Complete();

        // Act
        var action = () => order.Cancel();

        // Assert
        var exception = Assert.Throws<DomainException>(action);

        Assert.Equal(
            "A completed order cannot be cancelled.",
            exception.Message);
    }

    private static Order CreateValidOrder()
    {
        return new Order(
            customerName: "Meridian Corp",
            productName: "Business Cards",
            quantity: 1_000,
            totalPrice: 3_200m,
            deadline: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));
    }
}