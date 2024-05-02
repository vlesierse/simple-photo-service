using XaasKit.Domain.Entities;

namespace XaasKit.Domain.Repositories;

public interface IReadOnlyRepository<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// Gets total count of all entities.
    /// </summary>
    Task<long> GetCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a list of all the entities.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Entity</returns>
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all the entities.
    /// </summary>
    /// <param name="sorting"></param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <param name="skip"></param>
    /// <param name="maxResults"></param>
    /// <returns>Entity</returns>
    Task<List<TEntity>> GetPagedListAsync(
        int skip,
        int maxResults,
        string sorting,
        CancellationToken cancellationToken = default);
}

public interface IReadOnlyRepository<TEntity, in TKey> : IReadOnlyRepository<TEntity>
    where TEntity : class, IEntity<TKey>
{
    /// <summary>
    /// Gets an entity with given primary key.
    /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with given id.
    /// </summary>
    /// <param name="id">Primary key of the entity to get</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Entity</returns>
    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity with given primary key or null if not found.
    /// </summary>
    /// <param name="id">Primary key of the entity to get</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Entity or null</returns>
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);
}