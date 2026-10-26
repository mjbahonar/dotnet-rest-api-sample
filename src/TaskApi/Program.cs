using Microsoft.EntityFrameworkCore;
using TaskApi;

var builder = WebApplication.CreateBuilder(args);

// A "Server=..." connection string selects SQL Server; anything else falls back to a local SQLite file.
builder.Services.AddDbContext<TodoDb>((sp, options) =>
{
    var connection = sp.GetRequiredService<IConfiguration>().GetConnectionString("Default") ?? "Data Source=todos.db";
    if (connection.Contains("Server=", StringComparison.OrdinalIgnoreCase))
        options.UseSqlServer(connection);
    else
        options.UseSqlite(connection);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<TodoDb>().Database.EnsureCreated();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapTodoEndpoints();

app.Run();

public partial class Program;
