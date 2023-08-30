namespace Finbourne.Core.Components;

using System;
using System.Collections.Generic;

public class LruCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
{
    public event EventHandler<CacheEvictionEventArgs<TKey, TValue>> ItemEvicted;

    private readonly int _maxCacheItems;
    private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cacheMap;
    private readonly LinkedList<CacheItem> _cacheList;

    public LruCache(int maxCacheItems)
    {
        _maxCacheItems = maxCacheItems;
        _cacheMap = new Dictionary<TKey, LinkedListNode<CacheItem>>();
        _cacheList = new LinkedList<CacheItem>();
    }

    public void Add(TKey key, TValue value)
    {
        if (_cacheMap.Count >= _maxCacheItems)
        {
            Evict();
        }

        if (_cacheMap.ContainsKey(key))
        {
            _cacheList.Remove(_cacheMap[key]);
        }

        var cacheItem = new CacheItem(key, value);
        var node = new LinkedListNode<CacheItem>(cacheItem);
        _cacheList.AddFirst(node);
        _cacheMap[key] = node;
    }

    public TValue Get(TKey key)
    {
        if (!_cacheMap.TryGetValue(key, out var node))
        {
            throw new KeyNotFoundException($"Key '{key}' not found in cache.");
        }

        // Move the accessed item to the front of the cache list (most recently used)
        _cacheList.Remove(node);
        _cacheList.AddFirst(node);

        return node.Value.Value;
    }

    private void Evict()
    {
        if (_cacheList.Last != null)
        {
            var lastNode = _cacheList.Last;
            _cacheList.RemoveLast();

            if (lastNode != null)
            {
                var evictedKey = lastNode.Value.Key;
                _cacheMap.Remove(evictedKey);

                ItemEvicted?.Invoke(this, new CacheEvictionEventArgs<TKey, TValue>(evictedKey, lastNode.Value.Value));    
            }
        }
    }

    private class CacheItem
    {
        public TKey Key { get; }
        public TValue Value { get; }

        public CacheItem(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
