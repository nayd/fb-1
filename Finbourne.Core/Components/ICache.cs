namespace Finbourne.Core.Components;

public interface ICache<TKey, TValue>
{
    void Add(TKey key, TValue value);
    TValue Get(TKey key);
}
