using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public class DataContext : IdentityDbContext
    <AppUser, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }   
       public DbSet<UserLike> Likes { get; set; } 
        public DbSet<Message> Messages { get; set; }     
        public DbSet<Group> Groups { get; set; }     
        public DbSet<Connection> Connections { get; set; }     

       protected override void OnModelCreating(ModelBuilder builder){
           base.OnModelCreating(builder);

           builder.Entity<AppUser>()
            .HasMany(m=>m.UserRoles)
            .WithOne(o=>o.User)
            .HasForeignKey(f=>f.UserId)
            .IsRequired();

             builder.Entity<AppRole>()
            .HasMany(m=>m.UserRoles)
            .WithOne(o=>o.Role)
            .HasForeignKey(f=>f.RoleId)
            .IsRequired();

           builder.Entity<UserLike>()
           .HasKey(k=> new {k.SourceUserId, k.LikedUserId});

           builder.Entity<UserLike>()
           .HasOne(o=>o.SourceUser)
           .WithMany(m=>m.LikedUsers)
           .HasForeignKey(k=>k.SourceUserId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
           .HasOne(o=>o.LikedUser)
           .WithMany(m=>m.LikeByUsers)
           .HasForeignKey(k=>k.LikedUserId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
           .HasOne(o=>o.Sender)
           .WithMany(m=>m.MessagesSent)
           .HasForeignKey(k=>k.SenderId)
           .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
           .HasOne(o=>o.Recipient)
           .WithMany(m=>m.MessagesReceived)
           .HasForeignKey(k=>k.RecipientId)
           .OnDelete(DeleteBehavior.Restrict);
       }
    }
}