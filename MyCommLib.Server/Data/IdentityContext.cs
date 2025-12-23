namespace MyCommLib.Server.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
public class IdentityContext : IdentityDbContext<IdentityUser>
{
    public IdentityContext(DbContextOptions<IdentityContext> opts) : base(opts) { }
}
