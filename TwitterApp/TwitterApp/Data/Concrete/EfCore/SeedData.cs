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
                        new User { UserId = 1, Username = "giraycelenk", Name = "Giray Çelenk", Email="a.giraycelenk@hotmail.com",Bio="giraycelenk biography",Location="Kocaeli, Turkey",BirthDate = new DateTime(1997, 5, 23), RegistrationDate = new DateTime(2024, 10, 14), Password = "123456",Image = "twitteregg.jpg"},
                        
                        new User { UserId = 2,Username = "testkullanici", Name = "Test Kullanıcı",Email="testkullanici123@hotmail.com",Bio="testkullanici biography",Location="Istanbul, Turkey",BirthDate = new DateTime(2024, 10, 7), RegistrationDate = new DateTime(2024, 10, 15), Password = "123456",Image = "twitteregg.jpg"}
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