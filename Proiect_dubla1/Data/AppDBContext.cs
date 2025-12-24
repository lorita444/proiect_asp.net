using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proiect_dubla1.Models;

namespace Proiect_dubla1.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Opinion> Opinions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // FOLLOW
        modelBuilder.Entity<Follow>()
            .HasKey(f => new { f.FollowerId, f.FollowedId });

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Followed)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowedId)
            .OnDelete(DeleteBehavior.NoAction);

        // MESSAGE
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction);

        // OPINION
        modelBuilder.Entity<Opinion>()
            .HasOne(o => o.Author)
            .WithMany(u => u.WrittenOpinions)
            .HasForeignKey(o => o.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Opinion>()
            .HasOne(o => o.TargetUser)
            .WithMany(u => u.ReceivedOpinions)
            .HasForeignKey(o => o.TargetUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Comment>()
        .HasOne(c => c.User)
        .WithMany(u => u.Comments)
        .HasForeignKey(c => c.UserId)
        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Like>()
        .HasOne(l => l.User)
        .WithMany(u => u.Likes)
        .HasForeignKey(l => l.UserId)
        .OnDelete(DeleteBehavior.Restrict);
    }





}
