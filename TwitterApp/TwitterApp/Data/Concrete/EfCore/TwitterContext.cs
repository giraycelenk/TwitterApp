using Microsoft.EntityFrameworkCore;
using TwitterApp.Entity;

namespace TwitterApp.Data.Concrete.EfCore
{
    public class TwitterContext : DbContext
    {
        public TwitterContext(DbContextOptions<TwitterContext> options):base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<Mention> Mentions { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Retweet> Retweets { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            
            modelBuilder.Entity<Tweet>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tweets)
                .HasForeignKey(t => t.UserId);

            
            modelBuilder.Entity<Mention>()
                .HasOne(m => m.Tweet)
                .WithMany(t => t.Mentions)
                .HasForeignKey(m => m.TweetId);

            
            modelBuilder.Entity<Mention>()
                .HasOne(m => m.MentionedUser)
                .WithMany(u => u.Mentions)
                .HasForeignKey(m => m.MentionedUserId);

           
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Tweet)
                .WithMany(t => t.Likes)
                .HasForeignKey(l => l.TweetId);

            
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId);

            
            modelBuilder.Entity<Retweet>()
                .HasOne(r => r.Tweet)
                .WithMany(t => t.Retweets)
                .HasForeignKey(r => r.TweetId);

            
            modelBuilder.Entity<Retweet>()
                .HasOne(r => r.User)
                .WithMany(u => u.Retweets)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<UserFollow>()
            .HasKey(uf => new { uf.FollowerUserId, uf.FollowingUserId });

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.FollowerUser)
                .WithMany(u => u.Following)
                .HasForeignKey(uf => uf.FollowerUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.FollowingUser)
                .WithMany(u => u.Followers)
                .HasForeignKey(uf => uf.FollowingUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}