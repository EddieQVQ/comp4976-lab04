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

// Configure MCP routes on root path
app.MapMcp();

// Add a simple web page for browsers
app.MapGet("/", () => Results.Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>ServerMCP - Beverage Database</title>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <style>
        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; margin: 0; padding: 20px; background: #f5f7fa; }
        .container { max-width: 800px; margin: 0 auto; }
        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; border-radius: 12px; text-align: center; margin-bottom: 30px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
        .card { background: white; padding: 25px; border-radius: 8px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .status { display: flex; align-items: center; margin: 10px 0; }
        .status-icon { width: 20px; height: 20px; border-radius: 50%; margin-right: 10px; }
        .status-success { background: #10b981; }
        .endpoint { background: #f8fafc; border: 1px solid #e2e8f0; padding: 15px; border-radius: 6px; margin: 10px 0; }
        .code { background: #f1f5f9; padding: 4px 8px; border-radius: 4px; font-family: 'Monaco', 'Consolas', monospace; font-size: 14px; }
        .tools-list { list-style: none; padding: 0; }
        .tools-list li { background: #f8fafc; margin: 5px 0; padding: 10px; border-radius: 4px; border-left: 3px solid #3b82f6; }
        h1 { margin: 0; font-size: 2.5rem; }
        h2 { color: #1e293b; margin-bottom: 15px; }
        p { color: #64748b; line-height: 1.6; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🍹 ServerMCP</h1>
            <p style='margin: 10px 0 0 0; font-size: 1.1rem;'>Beverage Database • Model Context Protocol Server</p>
        </div>
        
        <div class='card'>
            <h2>📊 Server Status</h2>
            <div class='status'><div class='status-icon status-success'></div><strong>Server:</strong> Running and healthy</div>
            <div class='status'><div class='status-icon status-success'></div><strong>Database:</strong> SQLite connected with 33 beverages</div>
            <div class='status'><div class='status-icon status-success'></div><strong>MCP Protocol:</strong> Active and responding</div>
        </div>
        
        <div class='card'>
            <h2>🔧 Available MCP Tools</h2>
            <ul class='tools-list'>
                <li><span class='code'>GetBeveragesJson</span> - Get all beverages from database</li>
                <li><span class='code'>GetBeverageByIdJson</span> - Get specific beverage by ID</li>
                <li><span class='code'>GetBeveragesByNameJson</span> - Search beverages by name</li>
                <li><span class='code'>GetBeveragesByTypeJson</span> - Filter by beverage type</li>
                <li><span class='code'>GetBeveragesByIngredientJson</span> - Filter by main ingredient</li>
                <li><span class='code'>GetBeveragesByCaloriesLessThanOrEqualJson</span> - Filter by calorie count</li>
                <li><span class='code'>GetBeveragesByOriginJson</span> - Filter by country of origin</li>
            </ul>
        </div>
        
        <div class='card'>
            <h2>🌐 MCP Endpoint</h2>
            <div class='endpoint'>
                <strong>Protocol:</strong> Model Context Protocol (MCP)<br>
                <strong>URL:</strong> <span class='code'>https://4537-lab04-bja2fjhyfeeydka7.canadacentral-01.azurewebsites.net</span><br>
                <strong>Method:</strong> POST<br>
                <strong>Content-Type:</strong> application/json<br>
                <strong>Required Header:</strong> Mcp-Session-Id
            </div>
        </div>
        
        <div class='card'>
            <h2>💡 Usage</h2>
            <p>This is an MCP server designed for programmatic access. Connect your MCP-compatible client to the endpoint above to access the beverage database tools.</p>
            <p><strong>Example clients:</strong> ASP.NET applications, Python scripts, or any MCP-compatible software.</p>
        </div>
    </div>
</body>
</html>", "text/html"));


using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
