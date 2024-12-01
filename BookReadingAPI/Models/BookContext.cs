
using Microsoft.EntityFrameworkCore;



namespace ReadingBookAPI.Models
{
    public class BookContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookItem>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();

        }
        public BookContext(DbContextOptions<BookContext> options) : base(options)
        {
            
        }
        
        public DbSet<BookItem> BookItems { get; set; } = null!;
    }
}
