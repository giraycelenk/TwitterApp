using Microsoft.EntityFrameworkCore;
using TwitterApp.Entity;

namespace TwitterApp.Data.Concrete.EfCore
{
    public static class SeedData
    {
        public static void TestDatasFill(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<TwitterContext>();

            if(context != null)
            {
                if(context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
                if(!context.Users.Any())
                {
                    context.Users.AddRange(
                        new User { UserId = 1, Username = "giraycelenk"},
                        new User { UserId = 2, Username = "testkullanici"}
                    );
                }
                if(!context.Tweets.Any())
                {
                    context.Tweets.AddRange(
                        new Tweet {
                            Content = "Test tweet 1",
                            TweetDate = DateTime.Now.AddHours(-1),
                            IsDeleted = false,                            
                            UserId = 1,                            
                            
                        },
                        new Tweet {
                            Content = "Test tweet 2",
                            TweetDate = DateTime.Now,
                            IsDeleted = false,                            
                            UserId = 2,                            
                            
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}