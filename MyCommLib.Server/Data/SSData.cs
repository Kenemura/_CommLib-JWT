using Microsoft.EntityFrameworkCore;
using MyCommLib.Shared.Models;

namespace MyCommLib.Server.Data;
public class SSData : DbContext
{
    public SSData(DbContextOptions<SSData> opts) : base(opts) { }
    public DbSet<SessionStore> SessionStores { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
