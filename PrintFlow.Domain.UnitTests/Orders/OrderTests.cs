using PrintFlow.Domain.Common;
using PrintFlow.Domain.Orders;

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
        var action = () => new Order(
            customerName: " ",
            productName: "Business Cards",
            quantity: 1_000,
            totalPrice: 3_200m,
            deadline: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));

        var exception = Assert.Throws<DomainException>(action);

        Assert.Equal("Customer name is required.", exception.Message);
    }

    [Fact]
    public void Constructor_WithMissingProductName_ThrowsDomainException()
    {
        var action = () => new Order(
            customerName: "Meridian Corp",
            productName: "",
            quantity: 1_000,
            totalPrice: 3_200m,
            deadline: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));

        var exception = Assert.Throws<DomainException>(action);

        Assert.Equal("Product name is required.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithNonPositiveQuantity_ThrowsDomainException(
        int invalidQuantity)
    {
        var action = () => new Order(
            customerName: "Meridian Corp",
            productName: "Business Cards",
            quantity: invalidQuantity,
            totalPrice: 3_200m,
            deadline: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));

        var exception = Assert.Throws<DomainException>(action);

        Assert.Equal("Quantity must be greater than zero.", exception.Message);
    }

    [Fact]
    public void Constructor_WithNegativeTotalPrice_ThrowsDomainException()
    {
        var action = () => new Order(
            customerName: "Meridian Corp",
            productName: "Business Cards",
            quantity: 1_000,
            totalPrice: -1m,
            deadline: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));

        var exception = Assert.Throws<DomainException>(action);

        Assert.Equal("Total price cannot be negative.", exception.Message);
    }
}