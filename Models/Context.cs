using Microsoft.EntityFrameworkCore;
using userDashboard.Models;
namespace userDashboard.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) {}
        public DbSet<User> Users {get;set;}
        public DbSet<Message> Messages {get;set;}
        public DbSet<Comment> Comments {get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<Message>()
            //     .HasOne(z => z.Creator)
            //     .WithMany(y => y.CreatedMessages)
            //     .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.MessageCommentBelongsTo)
                .WithMany(t => t.ListOfComments)
                .HasForeignKey(m => m.MessageId);

            // modelBuilder.Entity<Comment>()
            //     .HasOne(c1 => c1.UserCreator)
            //     .WithMany(m2 => m2.CreatedComments)
            //     .HasForeignKey(j => j.UserId);
        }
    }
}