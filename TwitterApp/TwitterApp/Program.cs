using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TwitterApp.Data.Abstract;
using TwitterApp.Data.Concrete.EfCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TwitterContext>(options => {
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString("sql_connection");
    options.UseSqlite(connectionString);
});

builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<ITweetRepository, EfTweetRepository>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    options => 
    {
        options.LoginPath = "/Users/Login";
});


var app = builder.Build();

SeedData.TestDatasFill(app);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "user_profile",
    pattern:"{username}",
    defaults:new{controller = "Users", action="Profile"}
);
app.MapControllerRoute(
    name: "user_likes",
    pattern:"{username}/likes",
    defaults:new{controller = "Users", action="Profile", tab = "likes" }
);
app.MapControllerRoute(
    name: "user_likes",
    pattern:"{username}/with_replies",
    defaults:new{controller = "Users", action="Profile", tab = "replies" }
);
app.MapControllerRoute(
    name: "user_followers",
    pattern:"{username}/followers",
    defaults:new{controller = "Users", action="Followers"}
);
app.MapControllerRoute(
    name: "user_followers",
    pattern:"{username}/following",
    defaults:new{controller = "Users", action="Following"}
);

app.MapControllerRoute(
    name: "tweet_details",
    pattern:"{username}/status/{tweetid}",
    defaults:new{controller = "Tweets", action="TweetDetails"}
);
app.MapControllerRoute(
    name: "edit_profile",
    pattern:"settings/profile",
    defaults:new{controller = "Users", action="EditProfile"}
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
