using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.Mapping;

public static class CommandMapping
{
    public static CommandReadDto MapToCommandReadDto(this Command command)
    {
        return new CommandReadDto
        {
            Id = command.Id,
            HowTo = command.HowTo,
            CommandLine = command.CommandLine,
            PlatformId = command.PlatformId,
        };
    }

    public static Command MapToCommand(this CommandCreateDto commandCreateDto)
    {
        return new Command
        {
            HowTo = commandCreateDto.HowTo,
            CommandLine = commandCreateDto.CommandLine,
        };
    }
}
