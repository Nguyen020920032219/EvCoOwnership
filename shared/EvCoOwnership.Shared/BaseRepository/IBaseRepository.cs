using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EvCoOwnership.Shared.BaseRepository;

public interface IBaseRepository<TEntity, TKey> where TEntity : class
{
    #region Delete

    Task<TEntity?> Delete(TEntity entity);

    #endregion

    #region Read

    DbSet<TEntity> DbSet();

    DbContext DbContext();

    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

    #endregion

    #region Create

    Task<TEntity?> Add(TEntity entity);

    Task<ICollection<TEntity>> AddRange(ICollection<TEntity> entities);

    #endregion

    #region Update

    Task<TEntity?> Update(TEntity entity);

    Task<ICollection<TEntity>> UpdateRange(ICollection<TEntity> entities);

    #endregion
}