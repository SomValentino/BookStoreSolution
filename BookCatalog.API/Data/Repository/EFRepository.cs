using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.API.Data.Repository;

public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class
{
    private readonly BookDbContext _context;

    public EfRepository(BookDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public BookDbContext DbContext => _context;
}