using Feeds.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Feeds.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    // Change the Identity db context to application user instead of identity user
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    
    public DbSet<PostTag> PostTags { get; set; }
    
    public DbSet<Tag>Tags { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public override int SaveChanges()
    {
        foreach (var entityEntry in ChangeTracker.Entries<BaseModel>())
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Entity.CreatedOn = DateTime.Now;
                entityEntry.Entity.UpdatedOn = DateTime.Now;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Entity.UpdatedOn = DateTime.Now;
            }
        }

        return base.SaveChanges();
    }
}