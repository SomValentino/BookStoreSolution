using MongoDB.Driver;
using Order.API.Data.Interfaces;

namespace Order.API.Data;

public class OrderContext : IOrderContext {
    public OrderContext (IConfiguration configuration) {
        var client = new MongoClient (configuration.GetValue<string> ("ConnectionString"));
        var database = client.GetDatabase (configuration.GetValue<string> ("DatabaseName"));

        Orders = database.GetCollection<Order.API.Models.Order> (configuration.GetValue<string> ("CollectionName"));
    }
    public IMongoCollection<Order.API.Models.Order> Orders { get; }
}