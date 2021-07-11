using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
       public DbSet<AppUser> Users { get; set; }     
       public DbSet<UserLike> Likes { get; set; } 
        public DbSet<Message> Messages { get; set; }     

       protected override void OnModelCreating(ModelBuilder builder){
           base.OnModelCreating(builder);
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