namespace MyCommLib.Server.Data;

using MyCommLib.Shared.Models;
using Microsoft.EntityFrameworkCore;

public class ConfigData : DbContext
{
    public ConfigData(DbContextOptions<ConfigData> options) : base(options) { }

    public DbSet<ConfigKVP> ConfigKVPs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //modelBuilder.Entity<ConfigKVP>();
    }
}
