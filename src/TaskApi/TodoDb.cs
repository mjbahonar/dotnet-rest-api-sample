using Microsoft.EntityFrameworkCore;

namespace TaskApi;

public class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
}
