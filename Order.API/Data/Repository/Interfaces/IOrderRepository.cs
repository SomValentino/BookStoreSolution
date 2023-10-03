using System.Linq.Expressions;
using MongoDB.Driver;
using Order.API.Models;

namespace Order.API.Data.Repository.Interfaces;

public interface IOrderRepository {

    Task<Order.API.Models.Order> GetOrderById (string orderId);
    Task<IEnumerable<Order.API.Models.Order>> GetOrdersByUsername (string username);
    Task<IEnumerable<Order.API.Models.Order>> GetOrdersByQuery (FilterDefinition<Models.Order> filter);
    Task<IEnumerable<Order.API.Models.Order>> GetOrdersByQuery (FilterDefinition<Models.Order> filter,
        Expression<Func<Models.Order, object>> ? sort = null,
        bool sortAscending = true,
        int page = 1, int pageSize = 10);
    Task CreateOrder (Order.API.Models.Order order);
    Task<bool> UpdateOrder (Order.API.Models.Order order);
    Task<bool> DeleteOrder (Order.API.Models.Order order);
}