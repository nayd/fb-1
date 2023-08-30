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
}
