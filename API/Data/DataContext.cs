using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //-------- p' UserLike
        builder.Entity<UserLike>()
            .HasKey(k => new { k.SourceUserId, k.TargetUserId }); // establece la primary-key en la tabla

        // un SourceUser puede tener varios LikedUsers
        builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade); // si borro un user => q se borren los likes

        // un LikedUser puede tener varios LikedByUsers
        builder.Entity<UserLike>()
            .HasOne(s => s.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
        // si uso sqlServer => uno de los dos OnDelete debe ser distinto como OnDelete.NoAction
    }
}


/*

// p' configurar el 2way-binding de los likes
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ...

        //-------- p' Message
        builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
 
 */