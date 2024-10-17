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
        public Tweets(ITweetRepository tweetRepository)
        {
            _tweetRepository = tweetRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUserId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tweets = await _tweetRepository.GetAllTweetsAsync();

            var likesInfo = tweets.ToDictionary(
                t => t.TweetId,
                t => t.Likes != null && t.Likes.Any(l => l.UserId == currentUserId)
            );

            return View(new TweetViewModel
            {
                Tweets = tweets,
                IsLikedByCurrentUser = likesInfo
            });
        }
    }
}