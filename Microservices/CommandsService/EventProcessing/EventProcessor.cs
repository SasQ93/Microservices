using System.Text.Json;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Mapping;

namespace CommandsService.EventProcessing;

enum EventType
{
    PlatformPublished,
    Undetermined,
}

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EventProcessor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            case EventType.Undetermined:
                Console.WriteLine($"---> Could not determine event type for message: {message}");
                break;
        }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(
                platformPublishedMessage
            );
            try
            {
                var platform = platformPublishedDto.MapToModel();
                // etwas verwirrend, die id von PlatformApp ist hier die ExternalID
                // die id von der PlatformList hier ist die interne ID
                if (repository.ExternalPlatformExists(platform.Id))
                {
                    Console.WriteLine($"---> Platform {platform.Name} already exists.");
                }
                else
                {
                    repository.CreatePlatform(platform);
                    Console.WriteLine($"---> Platform {platform.Name} added to DB.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"---> Could not add platform to DB: {ex.Message}");
            }
        }
    }

    private EventType DetermineEvent(string notificationString)
    {
        Console.WriteLine($"---> Determining event for {notificationString}");
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationString)?.Event;

        return eventType switch
        {
            "Platform_Published" => EventType.PlatformPublished,
            _ => EventType.Undetermined,
        };
    }
}

public interface IEventProcessor
{
    void ProcessEvent(string message);
}
