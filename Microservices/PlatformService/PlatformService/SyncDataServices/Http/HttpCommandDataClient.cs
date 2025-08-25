using System.Text;
using System.Text.Json;
using PlatformServices.Dtos;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task SendPlatformToCommand(PlatformReadDto plat)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(plat),
            Encoding.UTF8,
            "application/json"
        );

        //@CommandsService_HostAddress = http://localhost:5218
        //{{CommandsService_HostAddress}}/api/c/platforms

        var response = await _httpClient.PostAsync(
            $"{_config.GetConnectionString("CommandService")}/api/c/platforms",
            httpContent
        );

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("--> Sync POST to CommandService was successful");
        }
        else
        {
            Console.WriteLine($"--> Sync POST to CommandService failed: {response.StatusCode}");
            Console.WriteLine(_config.GetConnectionString("CommandService"));
            Console.WriteLine(_config.GetConnectionString("CommandService") + "/api/c/platform");
        }
    }
}

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto plat);
}
