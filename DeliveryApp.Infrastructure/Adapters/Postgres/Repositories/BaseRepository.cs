using CSharpFunctionalExtensions;
using Ddd;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : Aggregate<Guid>
{
    protected readonly DbSet<TEntity> Entity;
    
    public BaseRepository(ApplicationDbContext dbContext)
    {
        Entity = dbContext.Set<TEntity>();
    }

    protected virtual IQueryable<TEntity> IncludeEntities()
    {
        return Entity;
    }
    
    public async Task AddAsync(TEntity aggregate)
    {
        await Entity.AddAsync(aggregate);
    }

    public void Update(TEntity aggregate)
    {
        Entity.Update(aggregate);
    }

    public async Task<Maybe<TEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await IncludeEntities()
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await IncludeEntities().ToListAsync(cancellationToken);
    }
}
