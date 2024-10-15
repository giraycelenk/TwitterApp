using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterApp.Data.Abstract;
using TwitterApp.Entity;
using TwitterApp.Models;

namespace TwitterApp.Controllers
{
    public class UsersController:Controller
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var isUser = _userRepository.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);

                if(isUser != null)
                {
                    var userClaims = new List<Claim>();

                    userClaims.Add(new Claim(ClaimTypes.NameIdentifier, isUser.UserId.ToString()));
                    userClaims.Add(new Claim(ClaimTypes.Name, isUser.Username));
                    userClaims.Add(new Claim(ClaimTypes.GivenName, isUser.Name ?? ""));
                    userClaims.Add(new Claim(ClaimTypes.UserData,isUser.Image ?? ""));

                    var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties 
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError("","Email or password is incorrect");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Profile(string username)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Users");
            }
            
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Users");
            }
            
            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }
            
            var user = await _userRepository.Users
                                    .Include(u => u.Followers)
                                    .Include(u => u.Following)
                                    .FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(userIdClaim);
            var currentUser = await _userRepository.Users
                                    .Include(u => u.Followers)
                                    .Include(u => u.Following)
                                    .FirstOrDefaultAsync(x => x.UserId == currentUserId);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var isFollowing = currentUser.Following.Any(f => f.FollowingUserId == user.UserId); 

            var tweets = _userRepository.GetTweetsByUserId(user.UserId);
            var viewModel = new ProfileViewModel
            {
                User = user,
                Tweets = tweets,
                IsFollowing = isFollowing
            };
            return View(viewModel);      
        }
        public async Task<IActionResult> Follow(int userIdToFollow)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _userRepository.GetUserById(userIdToFollow); 

            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

           
            bool isFollowing = await _userRepository.FollowUserAsync(currentUserId, userIdToFollow);
            
            return Json(new { success = true, isFollowing = true, username = user.Username });
        }

        [HttpPost]
        public async Task<IActionResult> Unfollow(int userIdToUnfollow)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                var existingFollow = await _userRepository.GetFollowAsync(currentUserId, userIdToUnfollow);

                if (existingFollow == null)
                {
                    return Json(new { success = false, message = "You are not following this user." });
                }

                var unfollowed = await _userRepository.UnfollowUserAsync(currentUserId, userIdToUnfollow);
                var user = _userRepository.GetUserById(userIdToUnfollow); 
                
                return Json(new { success = true, isFollowing = false, username = user?.Username });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}