using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorMCP.Services;

namespace RazorMCP.Pages;

public class BeveragesModel : PageModel
{
    private readonly McpService _mcpService;
    private readonly ILogger<BeveragesModel> _logger;

    public BeveragesModel(McpService mcpService, ILogger<BeveragesModel> logger)
    {
        _mcpService = mcpService;
        _logger = logger;
    }

    public List<BeverageViewModel>? Beverages { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? SearchName { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? FilterType { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SearchName))
            {
                _logger.LogInformation("Searching beverages by name: {SearchName}", SearchName);
                Beverages = await _mcpService.SearchBeveragesByNameAsync(SearchName);
            }
            else if (!string.IsNullOrWhiteSpace(FilterType))
            {
                _logger.LogInformation("Filtering beverages by type: {FilterType}", FilterType);
                Beverages = await _mcpService.GetBeveragesByTypeAsync(FilterType);
            }
            else
            {
                _logger.LogInformation("Loading all beverages");
                Beverages = await _mcpService.GetAllBeveragesAsync();
            }

            _logger.LogInformation("Loaded {Count} beverages", Beverages?.Count ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load beverages");
            Beverages = new List<BeverageViewModel>();
        }
    }
}
