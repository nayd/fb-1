using Finbourne.Core.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Finbourne.Core.ConsoleClient;

static class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<ICache<string, object>>(_ =>
            {
                var maxCacheItems = 3;
                return new LruCache<string, object>(maxCacheItems);
            })
            .BuildServiceProvider();

        var cache = serviceProvider.GetRequiredService<ICache<string, object>>();
        
        cache.ItemEvicted += (sender, eventArgs) =>
        {
            Console.WriteLine($"Item with key '{eventArgs.EvictedKey}' and value '{eventArgs.EvictedValue}' was evicted.");
        };
        
        cache.Add("key1", "value1");
        cache.Add("key2", "value2");
        cache.Add("key3", "value3");
        
        // after adding key4 we should get the event notification
        cache.Add("key4", "value4");
        
        var value = cache.Get("key4");

        Console.WriteLine(value);

        Console.WriteLine("Please press any key to quit the application...");
        Console.ReadKey();
    }
}