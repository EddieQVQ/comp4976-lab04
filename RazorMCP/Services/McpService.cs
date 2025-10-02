using ModelContextProtocol.Client;
using System.Text.Json;

namespace RazorMCP.Services;

public class McpService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<McpService> _logger;

    public McpService(IConfiguration configuration, ILogger<McpService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<BeverageViewModel>> GetAllBeveragesAsync()
    {
        try
        {
            var mcpEndpoint = _configuration["MCP:ServerEndpoint"] ?? throw new InvalidOperationException("MCP ServerEndpoint not configured");
            
            // Create MCP client with SSE transport
            var transport = new HttpClientTransport(new HttpClientTransportOptions 
            { 
                Endpoint = new Uri(mcpEndpoint) 
            });
            
            await using var mcpClient = await McpClient.CreateAsync(transport);
            
            // Call the MCP tool to get beverages
            var result = await mcpClient.CallToolAsync("get_beverages_json", null);
            
            if (result?.Content?.FirstOrDefault() is { } content && content.ToString() != null)
            {
                var beveragesJson = content.ToString();
                var beverages = JsonSerializer.Deserialize<List<BeverageViewModel>>(beveragesJson, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return beverages ?? new List<BeverageViewModel>();
            }
            
            return new List<BeverageViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get beverages from MCP server");
            return new List<BeverageViewModel>();
        }
    }

    public async Task<List<BeverageViewModel>> SearchBeveragesByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return await GetAllBeveragesAsync();

        try
        {
            var mcpEndpoint = _configuration["MCP:ServerEndpoint"] ?? throw new InvalidOperationException("MCP ServerEndpoint not configured");
            
            var transport = new HttpClientTransport(new HttpClientTransportOptions 
            { 
                Endpoint = new Uri(mcpEndpoint) 
            });
            
            await using var mcpClient = await McpClient.CreateAsync(transport);
            
            var result = await mcpClient.CallToolAsync("get_beverages_by_name_json", new Dictionary<string, object?> { { "name", name } });
            
            if (result?.Content?.FirstOrDefault() is { } content && content.ToString() != null)
            {
                var beveragesJson = content.ToString();
                var beverages = JsonSerializer.Deserialize<List<BeverageViewModel>>(beveragesJson, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return beverages ?? new List<BeverageViewModel>();
            }
            
            return new List<BeverageViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search beverages by name from MCP server");
            return new List<BeverageViewModel>();
        }
    }

    public async Task<List<BeverageViewModel>> GetBeveragesByTypeAsync(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return await GetAllBeveragesAsync();

        try
        {
            var mcpEndpoint = _configuration["MCP:ServerEndpoint"] ?? throw new InvalidOperationException("MCP ServerEndpoint not configured");
            
            var transport = new HttpClientTransport(new HttpClientTransportOptions 
            { 
                Endpoint = new Uri(mcpEndpoint) 
            });
            
            await using var mcpClient = await McpClient.CreateAsync(transport);
            
            var result = await mcpClient.CallToolAsync("get_beverages_by_type_json", new Dictionary<string, object?> { { "type", type } });
            
            if (result?.Content?.FirstOrDefault() is { } content && content.ToString() != null)
            {
                var beveragesJson = content.ToString();
                var beverages = JsonSerializer.Deserialize<List<BeverageViewModel>>(beveragesJson, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return beverages ?? new List<BeverageViewModel>();
            }
            
            return new List<BeverageViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get beverages by type from MCP server");
            return new List<BeverageViewModel>();
        }
    }
}

public class BeverageViewModel
{
    public int BeverageId { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? MainIngredient { get; set; }
    public string? Origin { get; set; }
    public int? CaloriesPerServing { get; set; }
}
