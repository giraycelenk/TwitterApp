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
            int currentUserId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _userRepository.GetUserById(currentUserId);
            int pageuserId = _userRepository.GetIdByUsername(username);
            var pageUser = _userRepository.GetUserById(pageuserId);

            TweetViewModel viewModel = await _tweetRepository.GetTweetsAndActivitiesByUserIdAsync(currentUserId,pageuserId,user,pageUser,IsProfilePage);
            
            return View(viewModel);
            
            
        }
    }
}