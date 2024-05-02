using XaasKit.Domain.Entities;

namespace XaasKit.Domain.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public abstract class Repository<TEntity> :
    IRepository<TEntity>
    where TEntity : class, IEntity
{
    public abstract Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await InsertAsync(entity, cancellationToken: cancellationToken);
        }
    }

    public abstract Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await UpdateAsync(entity, cancellationToken: cancellationToken);
        }
    }

    public abstract Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    public virtual async Task DeleteManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await DeleteAsync(entity, cancellationToken: cancellationToken);
        }
    }
    
    public abstract Task<long> GetCountAsync(CancellationToken cancellationToken = default);

    public abstract Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);
    
    public abstract Task<List<TEntity>> GetPagedListAsync(int skip, int maxResults, string sorting, CancellationToken cancellationToken = default);
}

public abstract class Repository<TEntity, TKey> : Repository<TEntity>, IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken);
        return entity ?? throw new EntityNotFoundException(typeof(TEntity), id);
    }

    public abstract Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);

    public virtual async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            return false;
        }
        return await DeleteAsync(entity, cancellationToken);
    }

    public async Task DeleteManyAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        foreach (var id in ids)
        {
            await DeleteAsync(id, cancellationToken: cancellationToken);
        }
    }
}
