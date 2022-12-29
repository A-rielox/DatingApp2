using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;


// p'q entityF trabaje con Identity NuGet Microsoft.AspNetCore.Identity.EntityFrameworkCore
// pongo el IdentityDbContext. Los tipos dentro de <> DEBEN estar en este orden


public class DataContext : /*DbContext*/ IdentityDbContext<AppUser, AppRole, int,
                        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
                        IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    //public DbSet<AppUser> Users { get; set; }     X IDENTITY
    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        //-------- p' roles usuario
        builder.Entity<AppUser>()
            .HasMany(u => u.UserRoles)
            .WithOne(aur => aur.User)
            .HasForeignKey(aur => aur.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
            .HasMany(ar => ar.UserRoles)
            .WithOne(aur => aur.Role)
            .HasForeignKey(aur => aur.RoleId)
            .IsRequired();

        /*
        public class AppUser
            {
                public ICollection<AppUserRole> UserRoles { get; set; }
            }

        public class AppRole
            {
                public ICollection<AppUserRole> UserRoles { get; set; }
            }

        public class AppUserRole
            {
                public AppUser User { get; set; }
                public AppRole Role { get; set; }
            }
        */


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

        //-------- p' Message
        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        // en ambas queda especificada la foreign key por convencion ( RecipientId y SenderId )

        builder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany(u => u.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
