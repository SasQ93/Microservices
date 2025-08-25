namespace DataLibrary;

using Microsoft.Extensions.Caching.Memory;

// hier zu überlegen
// mehrere Cache-Klassen ? => mehr Trennung und damit bessere Wartbarkeit // aber mehr Overhead
// oder gut gewählte "key"wählen wie "worker:wokerid" und "product:productid" ?
public class MyMemoryCache
{
    private readonly IMemoryCache _cache;

    public MyMemoryCache()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    public void SetCache<T>(string key, T value) // Update is auch
    {
        _cache.Set(
            key,
            value,
            new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2),
            }
        );
    }

    public T GetCache<T>(string key)
    {
        if (_cache.TryGetValue(key, out T value))
        {
            return value;
        }
        return default;
    }

    public void RemoveCache(string key)
    {
        _cache.Remove(key);
    }
}
