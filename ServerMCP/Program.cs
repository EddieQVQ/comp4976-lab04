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

// Configure MCP routes with a specific path prefix to avoid conflicts
app.MapMcp("/mcp");


using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
