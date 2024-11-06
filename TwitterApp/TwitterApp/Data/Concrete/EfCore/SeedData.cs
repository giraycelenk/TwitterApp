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
                        new User { UserId = 1, Username = "giraycelenk", Name = "Giray Çelenk", Email="a.giraycelenk@hotmail.com",Bio="giraycelenk biography",Location="Kocaeli, Turkey",BirthDate = new DateTime(1997, 5, 23), RegistrationDate = new DateTime(2024, 10, 14), Password = "123456",ImageUrl = "profileimg/giraycelenk/twitteregg.jpg"},
                        
                        new User { UserId = 2,Username = "testkullanici", Name = "Test Kullanıcı",Email="testkullanici123@hotmail.com",Bio="testkullanici biography",Location="Istanbul, Turkey",BirthDate = new DateTime(2024, 10, 7), RegistrationDate = new DateTime(2024, 10, 15), Password = "123456",ImageUrl = "profileimg/testkullanici/twitteregg.jpg"},

                        new User { UserId = 3,Username = "elonmusk", Name = "Elon Musk",Email="elon_musk_test@hotmail.com",Bio="elon_musk_test biography", RegistrationDate = new DateTime(2009, 6, 1), Password = "123456",ImageUrl = "profileimg/elonmusk/elonmusk.jpg"},

                        new User { UserId = 4,Username = "markzuckerberg", Name = "Mark Zuckerberg",Email="mark_zuckerberg_test@hotmail.com",Bio="mark_zuckerberg_test biography", RegistrationDate = new DateTime(2009, 6, 1), Password = "123456",ImageUrl = "profileimg/markzuckerberg/markzuckerberg.jpg"}
                    );
                }
                if(!context.Tweets.Any())
                {
                    context.Tweets.AddRange(
                        new Tweet {
                            Content = "Test tweet 1",
                            TweetDate = DateTime.Now.AddHours(-3),
                            IsDeleted = false,                            
                            UserId = 1,                            
                        },
                        new Tweet {
                            Content = "Test tweet 2",
                            TweetDate = DateTime.Now.AddHours(-2),
                            IsDeleted = false,                            
                            UserId = 2,                            
                        },
                        new Tweet {
                            Content = "Twitter is mine",
                            TweetDate = DateTime.Now.AddHours(-1),
                            IsDeleted = false,                            
                            UserId = 3,                              
                        },
                        new Tweet {
                            Content = "Twitter is not good",
                            TweetDate = DateTime.Now,
                            IsDeleted = false,                            
                            UserId = 4,                            
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}