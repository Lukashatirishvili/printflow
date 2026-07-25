using PrintFlow.Application.Abstractions.Persistence;
using PrintFlow.Application.Orders.CreateOrder;
using PrintFlow.Domain.Orders;

namespace PrintFlow.Application.UnitTests.Orders.CreateOrder;

public sealed class CreateOrderHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithValidCommand_CreateAndSavesOrder()
    {
        // Arrange
        var repository = new FakeOrderRepository();
        var unitOfWork = new FakeUnitOfWork();
        
        var handler = new CreateOrderHandler(repository, unitOfWork);

        var command = new CreateOrderCommand(
            CustomerName: "Meridian Corp",
            ProductName: "Business Cards",
            Quantity: 1_000,
            TotalPrice: 3_200m,
            Deadline: DateOnly.FromDateTime(
                DateTime.UtcNow.AddDays(7)));
        
        // Act
        var result = await handler.HandleAsync(command);
        
        // Assert
        Assert.NotNull(repository.AddedOrder);
        Assert.Equal(result.Id, repository.AddedOrder.Id);
        Assert.Equal(OrderStatus.Draft, result.Status);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }
    
    private sealed class FakeOrderRepository : IOrderRepository
    {
        public Order? AddedOrder { get; private set; }
        
        public Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            AddedOrder = order; 
            
            return Task.CompletedTask;
        }
    }
    
    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;

            return Task.FromResult(1);
        }
    }
}   