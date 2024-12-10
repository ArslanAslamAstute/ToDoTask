using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Models
{
    public class ToDoDbContext : DbContext
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
        {
            
        }
         public DbSet<ToDoItem> ToDoItems { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
