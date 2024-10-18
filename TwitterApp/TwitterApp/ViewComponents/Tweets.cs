using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TwitterApp.Data.Abstract;
using TwitterApp.Models;

namespace TwitterApp.ViewComponents
{
    public class Tweets:ViewComponent
    {
        private ITweetRepository _tweetRepository;
        private IUserRepository _userRepository;
        
        public Tweets(ITweetRepository tweetRepository,IUserRepository userRepository)
        {
            _tweetRepository = tweetRepository;
            _userRepository = userRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(bool IsProfilePage = false,string username="")
        {
            var currentUserId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(IsProfilePage == true && !string.IsNullOrEmpty(username))
            {
                var user = await _userRepository.Users
                                    .Include(u => u.Followers)
                                    .Include(u => u.Following)
                                    .FirstOrDefaultAsync(x => x.Username == username);
                
                var currentUser = await _userRepository.Users
                                        .Include(u => u.Followers)
                                        .Include(u => u.Following)
                                        .FirstOrDefaultAsync(x => x.UserId == currentUserId);

                var tweets = _userRepository.GetTweetsByUserId(user.UserId).ToList();
                var likesInfo = tweets.ToDictionary(
                    t => t.TweetId,
                    t => t.Likes.Any(l => l.UserId == currentUserId)
                );
                var retweetsInfo = tweets.ToDictionary(
                    t => t.TweetId,
                    t => t.Retweets != null && t.Retweets.Any(l => l.UserId == currentUserId)
                );

                var viewModel = new TweetViewModel
                {
                    Tweets = tweets,
                    IsLikedByCurrentUser = likesInfo,
                    IsRetweetedByCurrentUser = retweetsInfo
                };
                return View(viewModel);
            }
            else
            {
                TweetViewModel tweetViewModel = await _tweetRepository.GetTweetActivityByUserIdAsync(currentUserId);
                var tweets = await _tweetRepository.GetAllTweetsAsync();
                return View(tweetViewModel);
            }
            
        }
    }
}