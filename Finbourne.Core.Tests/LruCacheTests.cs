using Finbourne.Core.Components;

namespace Finbourne.Core.Tests;

using System;
using FluentAssertions;
using NUnit.Framework;

[TestFixture]
public class LruCacheTests
{
    [Test]
    public void Add_Should_AddItemToCache()
    {
        // Arrange
        var cache = new LruCache<int, object>(3);

        // Act
        cache.Add(1, "One");

        // Assert
        cache.Get(1).Should().Be("One");
    }

    [Test]
    public void Add_When_CacheIsFull_Should_EvictLeastRecentlyUsedItem()
    {
        // Arrange
        var cache = new LruCache<int, string>(3);

        // Act
        cache.Add(1, "One");
        cache.Add(2, "Two");
        cache.Add(3, "Three");
        cache.Add(4, "Four"); // This should trigger eviction of "One"

        // Assert
        Action getEvictedItem = () => cache.Get(1);
        getEvictedItem.Should().Throw<KeyNotFoundException>();
    }

    [Test]
    public void Get_When_ItemExists_Should_ReturnItemAndUpdateUsageOrder()
    {
        // Arrange
        var cache = new LruCache<int, string>(3);
        cache.Add(1, "One");
        cache.Add(2, "Two");

        // Act
        var value = cache.Get(1);

        // Assert
        value.Should().Be("One");
        cache.Get(2).Should().Be("Two"); // "Two" should still be in the cache
    }

    [Test]
    public void Get_When_ItemDoesNotExist_Should_ThrowKeyNotFoundException()
    {
        // Arrange
        var cache = new LruCache<int, string>(3);

        // Act
        Action act = () => cache.Get(1);

        // Assert
        act.Should().Throw<KeyNotFoundException>().WithMessage("Key '1' not found in cache.");
    }
    
    [Test]
    public void Cache_Should_Evict_Items_According_To_LRU_Policy()
    {
        // Arrange
        var cache = new LruCache<int, string>(3);

        cache.Add(1, "One");
        cache.Add(2, "Two");
        cache.Add(3, "Three");

        cache.Get(1); // Accessing key1 to make it least recently used

        cache.Add(4, "Four"); // This should trigger eviction of key2

        // Act
        Action act = () => cache.Get(2);
        
        // Assert
        act.Should().Throw<KeyNotFoundException>().WithMessage("Key '2' not found in cache.");
        cache.Get(1).Should().Be("One");
        cache.Get(3).Should().Be("Three");
        cache.Get(4).Should().Be("Four");
    }

    [Test]
    public void Cache_Should_Invoke_ItemEvicted_Event_On_Eviction()
    {
        // Arrange
        var cache = new LruCache<int, string>(3);
        int evictedKey = 0;
        string? evictedValue = null;

        cache.ItemEvicted += (sender, eventArgs) =>
        {
            evictedKey = eventArgs.EvictedKey;
            evictedValue = eventArgs.EvictedValue;
        };

        cache.Add(1, "One");
        cache.Add(2, "Two");
        cache.Add(3, "Three");
        
        // Act
        // This should trigger eviction of key1
        cache.Add(4, "Four");

        // Assert
        evictedKey.Should().Be(1);
        evictedValue.Should().Be("One");
    }
    
    [Test]
    public void Cache_Should_Be_Thread_Safe()
    {
        // Arrange
        const int numThreads = 10;
        const int numOperationsPerThread = 1000;
        const int maxCacheItems = numThreads * numOperationsPerThread / 2;
        var cache = new LruCache<string, int>(maxCacheItems);

        var threads = new List<Thread>();

        // Act
        for (int i = 0; i < numThreads; i++)
        {
            var thread = new Thread(() =>
            {
                for (int j = 0; j < numOperationsPerThread; j++)
                {
                    var key = Thread.CurrentThread.ManagedThreadId + "-" + j;
                    cache.Add(key, j);
                    var value = cache.Get(key);
                    
                    // Assert
                    j.Should().Be(value);
                }
            });

            threads.Add(thread);
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        // Assert
        maxCacheItems.Should().Be(cache.Count);
    }
}
