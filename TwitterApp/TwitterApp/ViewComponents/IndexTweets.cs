using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TwitterApp.Data.Abstract;
using TwitterApp.Models;

namespace TwitterApp.ViewComponents
{
    public class IndexTweets:ViewComponent
    {
        private ITweetRepository _tweetRepository;
        public IndexTweets(ITweetRepository tweetRepository)
        {
            _tweetRepository = tweetRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tweets = _tweetRepository
                    .Tweets
                    .Where(t => t.IsDeleted == false)
                    .Include(t => t.User);
            return View(new TweetViewModel{Tweets = await tweets.OrderByDescending(t => t.TweetDate).ToListAsync()});
        }
    }
}