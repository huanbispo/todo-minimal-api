using Microsoft.EntityFrameworkCore;
using todo_minimal_api.Modals;

namespace todo_minimal_api
{
    public class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options) : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }
        public DbSet<Step> Steps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todo>()
                .HasMany(t => t.FileAttachments)
                .WithOne(fa => fa.ToDo)
                .HasForeignKey(fa => fa.TodoId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
