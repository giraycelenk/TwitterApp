using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterApp.Data.Abstract;
using TwitterApp.Entity;
using TwitterApp.Models;

namespace TwitterApp.Controllers;

public class HomeController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly ITweetRepository _tweetRepository;
    public HomeController(IUserRepository userRepository,ITweetRepository tweetRepository)
    {
        _userRepository = userRepository;
        _tweetRepository = tweetRepository;
    }

    public async Task<IActionResult> Index()
    {
        var claims = User.Claims;

        var tweets = _tweetRepository
                    .Tweets
                    .Where(t => t.IsDeleted == false)
                    .Include(t => t.User);


        return View(new TweetViewModel{Tweets = await tweets.OrderByDescending(t => t.TweetDate).ToListAsync()});
    }
    [HttpPost]
    public async Task<IActionResult> Index(TweetCreateModel tweet)
    {

        if(ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!string.IsNullOrEmpty(userId))
            {
                var user = await _userRepository.Users.FirstOrDefaultAsync(u => u.UserId == Convert.ToInt32(userId ?? ""));
                _tweetRepository.CreateTweet(new Tweet {
                    Content = tweet.Content,
                    TweetDate = DateTime.Now,
                    IsDeleted = false,
                    UserId = Convert.ToInt32(userId),
                    User = user ?? new User(),
                });
            }
            
        }
        return View();
    }

}
