using MongoDB.Driver;

namespace Order.API.Data.Interfaces;

public interface IOrderContext {
    IMongoCollection<Order.API.Models.Order> Orders { get;}
}