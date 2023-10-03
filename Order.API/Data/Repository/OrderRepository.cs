using MongoDB.Driver;
using Order.API.Data.Interfaces;
using Order.API.Data.Repository.Interfaces;
using Order.API.Models;

namespace Order.API.Data.Repository;

public class OrderRepository : IOrderRepository {
    private readonly IOrderContext _OrderContext;

    public OrderRepository (IOrderContext OrderContext) {
        _OrderContext = OrderContext;
    }

    public async Task CreateOrder (Order.API.Models.Order Order) {
        await _OrderContext.Orders.InsertOneAsync (Order);
    }

    public async Task<bool> DeleteOrder (Order.API.Models.Order order) {
        var updateResult = await _OrderContext
            .Orders
            .DeleteOneAsync(filter: g => g.OrderId == order.OrderId);

        return updateResult.IsAcknowledged &&
            updateResult.DeletedCount > 0;
    }

    public async Task<Order.API.Models.Order> GetOrderById (string orderId) {
        return await _OrderContext.Orders.Find (_ => _.OrderId == orderId).FirstOrDefaultAsync ();
    }

    public async Task<IEnumerable<Order.API.Models.Order>> GetOrdersByQuery(FilterDefinition<Order.API.Models.Order> filter)
    {
        return await _OrderContext.Orders.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Order.API.Models.Order>> GetOrdersByUsername (string username) {
        return await _OrderContext.Orders.Find (_ => _.UserName == username).ToListAsync ();
    }

    public async Task<bool> UpdateOrder (Order.API.Models.Order Order) {
        var updateResult = await _OrderContext
            .Orders
            .ReplaceOneAsync (filter: g => g.OrderId == Order.OrderId, replacement: Order);

        return updateResult.IsAcknowledged &&
            updateResult.ModifiedCount > 0;
    }
}