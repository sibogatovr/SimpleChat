using Microsoft.EntityFrameworkCore;
using SimpleChat.Models;

namespace SimpleChat.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);
    }
}