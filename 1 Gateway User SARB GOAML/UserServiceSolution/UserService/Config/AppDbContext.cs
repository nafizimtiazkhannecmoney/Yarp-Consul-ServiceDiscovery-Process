using Microsoft.EntityFrameworkCore;
using UserService.Model;

namespace UserService.Data
{
    /**
    * Description : AppdbContext class for EF Core, which represents the database context for the application.
    * @author     : Nafiz Imtiaz khan
    * @since      : 07/05/2025      
    */
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<TUser> TUsers => Set<TUser>();          // ← DbSet name
    }
}
