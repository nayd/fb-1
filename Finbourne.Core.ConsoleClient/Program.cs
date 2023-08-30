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
        cache.Add("key1", "value1");
        var value = cache.Get("key1");

        Console.WriteLine(value);

        Console.WriteLine("Please press any key to quit the application...");
        Console.ReadKey();
    }
}