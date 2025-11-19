using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EvCoOwnership.Shared.BaseRepository;

public class BaseRepository<TDbContext, TEntity, TKey> : IBaseRepository<TEntity, TKey>
    where TDbContext : DbContext
    where TEntity : class
{
    protected readonly TDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(TDbContext dbContext)
    {
        _context = dbContext;
        _dbSet = _context.Set<TEntity>();
    }

    public DbSet<TEntity> DbSet()
    {
        return _context.Set<TEntity>();
    }

    public DbContext DbContext()
    {
        return _context;
    }

    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet().Where(predicate);
    }

    /// <summary>
    ///     Asynchronously adds a new entity to the data store.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains the added entity if the operation is successful, otherwise null.
    /// </returns>
    public async Task<TEntity?> Add(TEntity entity)
    {
        DbSet().Add(entity);
        var result = await _context.SaveChangesAsync();
        if (result > 0) return entity;
        return null;
    }

    /// <summary>
    ///     Asynchronously adds a collection of entities to the data store.
    /// </summary>
    /// <param name="entities">The collection of entities to be added.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains the collection of entities before.
    /// </returns>
    public async Task<ICollection<TEntity>> AddRange(ICollection<TEntity> entities)
    {
        foreach (var entity in entities) DbSet().Add(entity);

        await _context.SaveChangesAsync();
        return entities;
    }

    /// <summary>
    ///     Asynchronously updates an existing entity in the data store.
    /// </summary>
    /// <param name="entity">The entity with updated values to be saved.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains the updated entity if the operation is successful, otherwise null.
    /// </returns>
    public async Task<TEntity?> Update(TEntity entity)
    {
        DbSet().Entry(entity).CurrentValues.SetValues(entity);
        var result = await _context.SaveChangesAsync();
        if (result > 0) return entity;
        return null;
    }

    /// <summary>
    ///     Asynchronously updates a collection of entities in the data store.
    /// </summary>
    /// <param name="entities">The collection of entities to be updated.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains the collection of entities before.
    /// </returns>
    public async Task<ICollection<TEntity>> UpdateRange(ICollection<TEntity> entities)
    {
        foreach (var entity in entities) DbSet().Entry(entity).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return entities;
    }

    public async Task<TEntity?> Delete(TEntity entity)
    {
        DbSet().Remove(entity);
        var result = await _context.SaveChangesAsync();
        if (result > 0) return entity;
        return null;
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }
}