namespace Finbourne.Core.Components;

public interface ICache<TKey, TValue>
{
    event EventHandler<CacheEvictionEventArgs<TKey, TValue>> ItemEvicted;
    
    void Add(TKey key, TValue value);
    TValue Get(TKey key);
    int Count { get; }
}
