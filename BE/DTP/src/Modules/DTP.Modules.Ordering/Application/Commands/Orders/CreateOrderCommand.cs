using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Ordering.Application.Commands.Orders
{
    public class CreateOrderCommand : IRequest<Result<Guid>>
    {
        public Guid CustomerId { get; set; }

        public string? CustomerEmail { get; set; }

        public string? CustomerPhone { get; set; }

        public string? CustomerName { get; set; }

        public string Currency { get; set; } = "VND";

        public string? Note { get; set; }

        public List<CreateOrderItemCommandItem> Items { get; set; } = new();
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        private readonly IOrderService _orderService;

        public CreateOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<Result<Guid>> Handle(
            CreateOrderCommand request,
            CancellationToken cancellationToken)
        {
            var items = request.Items.Select(x => new CreateOrderItemRequest
            {
                ItemType = x.ItemType,
                ProductId = x.ProductId,
                ProductVariantId = x.ProductVariantId,
                EsimPackageId = x.EsimPackageId,
                ProductName = x.ProductName,
                VariantName = x.VariantName,
                Sku = x.Sku,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList();

            return _orderService.CreateAsync(
                request.CustomerId,
                request.CustomerEmail,
                request.CustomerPhone,
                request.CustomerName,
                request.Currency,
                request.Note,
                items,
                cancellationToken);
        }
    }
}
