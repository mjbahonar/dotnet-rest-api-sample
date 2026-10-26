using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskApi;

namespace TaskApi.Tests;

public class TodoApiTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TodoApiTests()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseSetting("ConnectionStrings:Default", $"Data Source={_dbPath}"));
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
    }

    [Fact]
    public async Task Create_then_get_returns_the_todo()
    {
        var created = await _client.PostAsJsonAsync("/api/todos", new TodoInput("  Write tests  ", false));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var todo = await created.Content.ReadFromJsonAsync<Todo>();
        Assert.NotNull(todo);
        Assert.Equal("Write tests", todo.Title);

        var fetched = await _client.GetFromJsonAsync<Todo>($"/api/todos/{todo.Id}");
        Assert.Equal(todo.Id, fetched?.Id);
    }

    [Fact]
    public async Task Create_with_blank_title_returns_validation_error()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new TodoInput("   ", false));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_changes_title_and_status()
    {
        var created = await _client.PostAsJsonAsync("/api/todos", new TodoInput("Draft", false));
        var todo = await created.Content.ReadFromJsonAsync<Todo>();

        var updated = await _client.PutAsJsonAsync($"/api/todos/{todo!.Id}", new TodoInput("Final", true));
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);

        var fetched = await _client.GetFromJsonAsync<Todo>($"/api/todos/{todo.Id}");
        Assert.Equal("Final", fetched?.Title);
        Assert.True(fetched?.IsDone);
    }

    [Fact]
    public async Task Delete_removes_the_todo()
    {
        var created = await _client.PostAsJsonAsync("/api/todos", new TodoInput("Temporary", false));
        var todo = await created.Content.ReadFromJsonAsync<Todo>();

        var deleted = await _client.DeleteAsync($"/api/todos/{todo!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

        var after = await _client.GetAsync($"/api/todos/{todo.Id}");
        Assert.Equal(HttpStatusCode.NotFound, after.StatusCode);
    }

    [Fact]
    public async Task Unknown_id_returns_not_found()
    {
        var response = await _client.GetAsync("/api/todos/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
