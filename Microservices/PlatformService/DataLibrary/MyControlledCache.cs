namespace DataLibrary;

using System.Collections.Concurrent;

public class MyControlledCache
{
    // Leseeinträge werden nicht gelockt
    // nur Schreibvorgänge sind thread-sicher
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object>> _cache;

    public MyControlledCache()
    {
        _cache = new();
    }

    public void SetCache<T>(string key, string subKey, T value)
    {
        if (!_cache.ContainsKey(key))
        {
            _cache[key] = new();
        }
        _cache[key][subKey] = value;
    }

    public T? GetCache<T>(string key, string subKey)
    {
        if (
            _cache.TryGetValue(key, out var subCache)
            && subCache.TryGetValue(subKey, out var value)
            && value is T tValue
        )
        {
            return (T)tValue;
        }
        return default;
    }

    public void RemoveCache(string key, string subKey)
    {
        if (_cache.TryGetValue(key, out var subCache))
        {
            subCache.TryRemove(subKey, out _);
            if (subCache.IsEmpty)
            {
                _cache.TryRemove(key, out _);
            }
        }
    }

    public void RemoveCache(string key)
    {
        _cache.TryRemove(key, out _);
    }

    public void ClearCache()
    {
        _cache.Clear();
    }

    public void UpdateCache<T>(string key, string subKey, T value)
    {
        SetCache(key, subKey, value);
    }
}
