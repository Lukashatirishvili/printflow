using PrintFlow.Application.Abstractions.Persistence;
using PrintFlow.Domain.Orders;

namespace PrintFlow.Application.Orders.CreateOrder;

public sealed class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateOrderResult> HandleAsync(CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = new Order(
            command.CustomerName,
            command.ProductName,
            command.Quantity,
            command.TotalPrice,
            command.Deadline);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(
            order.Id,
            order.CustomerName,
            order.ProductName,
            order.Quantity,
            order.TotalPrice,
            order.Deadline,
            order.Status);  
    }
}