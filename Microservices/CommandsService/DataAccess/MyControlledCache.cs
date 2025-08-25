namespace DataLibrary;

using System.Collections.Concurrent;
using Microsoft.VisualBasic;

public enum CacheType
{
    Platform,
    Command,
}

public class MyControlledCache
{
    // Leseeinträge werden nicht gelockt
    // nur Schreibvorgänge sind thread-sicher
    private readonly ConcurrentDictionary<CacheType, ConcurrentDictionary<int, object>> _cache;

    public MyControlledCache()
    {
        _cache = new();
    }

    public int GetNewSubKey(CacheType key)
    {
        List<int> subKeys = _cache.ContainsKey(key) ? _cache[key].Keys.ToList() : new List<int>();
        var max = subKeys.Count > 0 ? subKeys.Max() : 0;
        return max + 1;
    }

    public void SetCache<T>(CacheType key, int subKey, T value)
    {
        if (!_cache.ContainsKey(key))
        {
            _cache[key] = new();
        }
        _cache[key][subKey] = value;
    }

    public T? GetCache<T>(CacheType key, int subKey)
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

    public List<T> GetCacheByType<T>(CacheType key)
    {
        if (_cache.TryGetValue(key, out var subCache))
        {
            return subCache.Values.OfType<T>().ToList();
        }
        return new List<T>();
    }

    public void RemoveCache(CacheType key, int subKey)
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

    public void RemoveCache(CacheType key)
    {
        _cache.TryRemove(key, out _);
    }

    public void ClearCache()
    {
        _cache.Clear();
    }

    public void UpdateCache<T>(CacheType key, int subKey, T value)
    {
        SetCache(key, subKey, value);
    }
}
