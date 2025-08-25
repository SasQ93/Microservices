using CommandsService.Models;
using DataLibrary;

namespace CommandsService.Data;

public class CommandRepository : ICommandRepository
{
    private readonly MyControlledCache _cache;

    public CommandRepository(MyControlledCache cache)
    {
        _cache = cache;
    }

    public void CreateCommand(int platformId, Command command)
    {
        command.PlatformId = platformId;
        _cache.SetCache(CacheType.Command, command.Id, command);
    }

    public void CreatePlatform(Platform platform)
    {
        _cache.SetCache(CacheType.Platform, platform.Id, platform);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _cache.GetCacheByType<Platform>(CacheType.Platform);
    }

    public Command GetCommand(int platformId, int commandId)
    {
        List<Command> commands = _cache.GetCacheByType<Command>(CacheType.Command);
        return commands.FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _cache
            .GetCacheByType<Command>(CacheType.Command)
            .Where(c => c.PlatformId == platformId);
    }

    public bool PlatformExists(int id)
    {
        List<Platform> platforms = _cache.GetCacheByType<Platform>(CacheType.Platform);
        return platforms.Any(p => p.Id == id);
    }

    public bool ExternalPlatformExists(int id)
    {
        List<Platform> platforms = _cache.GetCacheByType<Platform>(CacheType.Platform);
        return platforms.Any(p => p.ExternalId == id);
    }
}

public interface ICommandRepository
{
    // Platform-specific methods
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatformExists(int id);
    bool ExternalPlatformExists(int id);

    // Command-specific methods
    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
}
