using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterApp.Data.Abstract;
using TwitterApp.Entity;
using TwitterApp.Models;

namespace TwitterApp.Controllers
{
    public class TweetsController:Controller
    {
        private ITweetRepository _tweetRepository;
        private IUserRepository _userRepository;
        public TweetsController(ITweetRepository tweetRepository,IUserRepository userRepository)
        {
            _tweetRepository = tweetRepository;
            _userRepository = userRepository;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTweet(TweetCreateModel tweet)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _userRepository.Users.FirstOrDefaultAsync(u => u.UserId == Convert.ToInt32(userId));
                    
                    if (!string.IsNullOrEmpty(tweet.Content))
                    {
                        _tweetRepository.CreateTweet(new Tweet
                        {
                            Content = tweet.Content,
                            TweetDate = DateTime.Now,
                            IsDeleted = false,
                            IsMentionTweet = false,
                            UserId = Convert.ToInt32(userId),
                            User = user ?? new User(),
                        });
                    }
                }
            }

            return RedirectToAction("Index","Home");
        }
        [HttpPost]
        public IActionResult DeleteTweet(int tweetId)
        {
            _tweetRepository.DeleteTweet(tweetId);
            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult LikeTweet(int tweetId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tweet = _tweetRepository
                        .Tweets
                        .Include(t => t.Likes)
                        .FirstOrDefault(t => t.TweetId == tweetId);
            
            if (tweet != null)
            {
                var likeExists = tweet.Likes.Any(l => l.UserId == userId);
                if(!likeExists)
                {
                    _tweetRepository.AddLike(tweetId,userId);
                    var updatedLikeCount = tweet.Likes.Count();
                    return Json(new { success = true,likeCount = updatedLikeCount});
                }
            }

            return Json(new { success = false});
        }
        [HttpPost]
        public IActionResult UnlikeTweet(int tweetId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tweet = _tweetRepository
                        .Tweets
                        .Include(t => t.Likes)
                        .FirstOrDefault(t => t.TweetId == tweetId);

            if(tweet != null && tweet.Likes.Any(l => l.UserId == userId))
            {
                _tweetRepository.RemoveLike(tweetId,userId);
                var updatedLikeCount = tweet.Likes.Count();
                return Json(new { success = true , likeCount = updatedLikeCount});
            }
            
            return Json(new { success = false}); 
        }
        [HttpPost]
        public IActionResult Retweet(int tweetId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tweet = _tweetRepository
                        .Tweets
                        .Include(t => t.Retweets)
                        .FirstOrDefault(t => t.TweetId == tweetId);
            
            if (tweet != null && !tweet.Retweets.Any(r => r.UserId == userId))
            {
                _tweetRepository.AddRetweet(tweetId, userId);
                var updatedRetweetCount = tweet.Retweets.Count();
                return Json(new { success = true, retweetCount = updatedRetweetCount });
            }

            return Json(new { success = false});
        }
        [HttpPost]
        public IActionResult UnRetweet(int tweetId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tweet = _tweetRepository
                        .Tweets
                        .Include(t => t.Retweets)
                        .FirstOrDefault(t => t.TweetId == tweetId);
            
            if (tweet != null && tweet.Retweets.Any(r => r.UserId == userId))
            {
                _tweetRepository.RemoveRetweet(tweetId, userId); 
                var updatedRetweetCount = tweet.Retweets.Count();
                return Json(new { success = true, retweetCount = updatedRetweetCount });
            }

            return Json(new { success = false });
        }
        public async Task<IActionResult> TweetDetails(string username,int tweetId)
        {
            int currentUserId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            TweetDetailsViewModel viewModel = await _tweetRepository.GetTweetDetailsAsync(tweetId,currentUserId);
            return View(viewModel);
            
        }
        [HttpPost]
        public async Task<IActionResult> AddMention(int tweetId,TweetCreateModel tweet)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _tweetRepository.AddMention(tweetId, userId, tweet);

                var username = User?.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Username could not be found.");
                }

                return RedirectToRoute("tweet_details", new { username, tweetid = tweetId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }   
        }
    }
}