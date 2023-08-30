namespace Finbourne.Core.Components;

using System;
using System.Collections.Generic;

public class LruCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
{
    // LRU implementation is based on the following Java example
    // https://www.baeldung.com/java-lru-cache
    
    /*
        How can we design a data structure that could do operations like reading, sorting (temporal sorting), and deleting elements in constant time?

        It seems that to find the answer to this question, we need to think deeply about what has been said about LRU cache and its features:
         - In practice, LRU cache is a kind of Queue — if an element is reaccessed, it goes to the end of the eviction order
         - This queue will have a specific capacity as the cache has a limited size. Whenever a new element is brought in, it is added at the head of the queue. When eviction happens, it happens from the tail of the queue.
         - Hitting data in the cache must be done in constant time, which isn't possible in Queue! But, it is possible with Java's HashMap data structure
         - Removal of the least recently used element must be done in constant time, which means for the implementation of Queue, we'll use DoublyLinkedList instead of SingleLinkedList or an array
        
        So, the LRU cache is nothing but a combination of the DoublyLinkedList and the Dictionary
     */
    
    public event EventHandler<CacheEvictionEventArgs<TKey, TValue>> ItemEvicted;

    private readonly int _maxCacheItems;
    private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cacheMap;
    private readonly LinkedList<CacheItem> _cacheList;
    private readonly object _lock = new object();
    
    public LruCache(int maxCacheItems)
    {
        _maxCacheItems = maxCacheItems;
        _cacheMap = new Dictionary<TKey, LinkedListNode<CacheItem>>();
        _cacheList = new LinkedList<CacheItem>();
    }

    public void Add(TKey key, TValue value)
    {
        lock (_lock)
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
    }

    public TValue Get(TKey key)
    {
        lock (_lock)
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
    }

    public int Count => _cacheMap.Count;

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
