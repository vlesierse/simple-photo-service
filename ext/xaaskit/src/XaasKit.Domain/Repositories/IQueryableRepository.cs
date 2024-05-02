using XaasKit.Domain.Entities;

namespace XaasKit.Domain.Repositories;

public interface IQueryableRepository<TEntity> where TEntity : class, IEntity { }

public interface IQueryableRepository<TEntity, TKey> : IQueryableRepository<TEntity>
    where TEntity : class, IEntity<TKey> { }