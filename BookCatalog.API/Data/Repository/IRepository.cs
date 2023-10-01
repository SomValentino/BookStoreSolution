using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
namespace BookCatalog.API.Data.Repository;


public interface IRepository<T> : IRepositoryBase<T> where T : class {
    public BookDbContext DbContext { get; }
}