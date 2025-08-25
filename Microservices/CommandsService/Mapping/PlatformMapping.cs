using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.Mapping;

public static class PlatformMapping
{
    public static PlatformReadDto MapToReadDto(this Platform platform)
    {
        return new PlatformReadDto { Id = platform.Id, Name = platform.Name };
    }

    public static Platform MapToModel(this PlatformPublishedDto platformPublishedDto)
    {
        return new Platform { Id = platformPublishedDto.Id, Name = platformPublishedDto.Name };
    }
}
