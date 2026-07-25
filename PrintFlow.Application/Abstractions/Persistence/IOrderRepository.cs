using PrintFlow.Domain.Orders;

namespace PrintFlow.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
}