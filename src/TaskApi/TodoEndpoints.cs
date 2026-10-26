using Microsoft.EntityFrameworkCore;

namespace TaskApi;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todos");

        group.MapGet("/", async (TodoDb db) =>
            await db.Todos.OrderBy(t => t.Id).ToListAsync());

        group.MapGet("/{id:int}", async (int id, TodoDb db) =>
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return Results.NotFound();
            return Results.Ok(todo);
        });

        group.MapPost("/", async (TodoInput input, TodoDb db) =>
        {
            if (string.IsNullOrWhiteSpace(input.Title))
                return TitleRequired();

            var todo = new Todo { Title = input.Title.Trim(), IsDone = input.IsDone };
            db.Todos.Add(todo);
            await db.SaveChangesAsync();
            return Results.Created($"/api/todos/{todo.Id}", todo);
        });

        group.MapPut("/{id:int}", async (int id, TodoInput input, TodoDb db) =>
        {
            if (string.IsNullOrWhiteSpace(input.Title))
                return TitleRequired();

            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return Results.NotFound();

            todo.Title = input.Title.Trim();
            todo.IsDone = input.IsDone;
            await db.SaveChangesAsync();
            return Results.Ok(todo);
        });

        group.MapDelete("/{id:int}", async (int id, TodoDb db) =>
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return Results.NotFound();

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    private static IResult TitleRequired() =>
        Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["title"] = ["Title is required."]
        });
}
