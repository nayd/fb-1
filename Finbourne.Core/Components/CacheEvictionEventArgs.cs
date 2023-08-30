namespace Finbourne.Core.Components;

public class CacheEvictionEventArgs<TKey, TValue> : EventArgs
{
    public TKey EvictedKey { get; }
    public TValue EvictedValue { get; }

    public CacheEvictionEventArgs(TKey key, TValue value)
    {
        EvictedKey = key;
        EvictedValue = value;
    }
}
