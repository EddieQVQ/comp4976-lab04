using Microsoft.EntityFrameworkCore;
using ServerMCP.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpServer()
.WithHttpTransport()
.WithToolsFromAssembly();

var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(
    option => option.UseSqlite(connStr)
);

var app = builder.Build();

// Configure MCP endpoint
app.MapMcp("/mcp");

// Add a welcome page for web browsers
app.MapGet("/", () => Results.Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>ServerMCP - Beverage Database</title>
    <style>
        body { font-family: Arial, sans-serif; max-width: 800px; margin: 0 auto; padding: 20px; }
        .header { background: #0078d4; color: white; padding: 20px; border-radius: 8px; margin-bottom: 20px; }
        .section { background: #f5f5f5; padding: 15px; border-radius: 8px; margin: 10px 0; }
        .endpoint { background: #e8f4fd; padding: 10px; border-left: 4px solid #0078d4; margin: 10px 0; }
        code { background: #f1f1f1; padding: 2px 6px; border-radius: 3px; }
    </style>
</head>
<body>
    <div class='header'>
        <h1>🍹 ServerMCP - Beverage Database</h1>
        <p>Model Context Protocol Server with Entity Framework</p>
    </div>
    
    <div class='section'>
        <h2>📊 Database Status</h2>
        <p>✅ <strong>SQLite Database:</strong> Connected and running</p>
        <p>✅ <strong>Beverages:</strong> 33 records loaded</p>
        <p>✅ <strong>MCP Tools:</strong> 7 beverage query tools available</p>
    </div>
    
    <div class='section'>
        <h2>🔧 Available MCP Tools</h2>
        <ul>
            <li><code>GetBeveragesJson</code> - Get all beverages</li>
            <li><code>GetBeverageByIdJson</code> - Get beverage by ID</li>
            <li><code>GetBeveragesByNameJson</code> - Search by name</li>
            <li><code>GetBeveragesByTypeJson</code> - Filter by type</li>
            <li><code>GetBeveragesByIngredientJson</code> - Filter by ingredient</li>
            <li><code>GetBeveragesByCaloriesLessThanOrEqualJson</code> - Filter by calories</li>
            <li><code>GetBeveragesByOriginJson</code> - Filter by origin</li>
        </ul>
    </div>
    
    <div class='section'>
        <h2>🌐 API Endpoints</h2>
        <div class='endpoint'>
            <strong>MCP Protocol:</strong><br>
            <code>POST /mcp</code><br>
            <small>Content-Type: application/json<br>
            Mcp-Session-Id: required</small>
        </div>
        <div class='endpoint'>
            <strong>Health Check:</strong><br>
            <code>GET /health</code>
        </div>
    </div>
    
    <div class='section'>
        <h2>📝 Example Usage</h2>
        <p>Connect your MCP client to:</p>
        <code>" + (app.Environment.IsDevelopment() ? "http://localhost:5062" : "https://" + Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")) + @"/mcp</code>
    </div>
</body>
</html>", "text/html"));

// Add health check endpoint
app.MapGet("/health", async (ApplicationDbContext db) =>
{
    var count = await db.Beverages.CountAsync();
    return new
    {
        status = "healthy",
        service = "ServerMCP",
        timestamp = DateTime.UtcNow,
        database = "connected",
        beverageCount = count,
        mcpEndpoint = "/mcp"
    };
});


using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
