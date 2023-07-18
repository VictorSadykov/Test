using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Test.Models;

namespace Test
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Tag> Tags { get; set; }

       
    }
}
