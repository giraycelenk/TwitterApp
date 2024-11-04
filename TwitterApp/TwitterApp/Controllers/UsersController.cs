using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ITweetRepository _tweetRepository;
        public UsersController(IUserRepository userRepository,ITweetRepository tweetRepository)
        {
            _userRepository = userRepository;
            _tweetRepository = tweetRepository;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
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
        [Authorize]
        public async Task<IActionResult> Profile(string username,string tab="")
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

            ProfileViewModel profileModel = new ProfileViewModel();

            if(tab=="likes")
            {
                if(_userRepository.GetIdByUsername(username) != int.Parse(userIdClaim))
                {
                    profileModel = await _userRepository.GetProfileByUserNameAsync(int.Parse(userIdClaim),username,tab = "");
                    return Redirect("/"+username);  
                }
            }
            profileModel = await _userRepository.GetProfileByUserNameAsync(int.Parse(userIdClaim),username,tab);

            return View(profileModel);      
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Follow(int userIdToFollow)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _userRepository.FollowUserAsync(currentUserId, userIdToFollow);
                return Json(new { success = true, isFollowing=true});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }  
        }
        [Authorize]
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

                await _userRepository.UnfollowUserAsync(currentUserId, userIdToUnfollow);
                return Json(new { success = true, isFollowing = false});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public async Task<IActionResult> Followers(string username)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            FollowViewModel followerViewModel = await _userRepository.GetFollowersForProfileAsync(username,currentUserId);
            return View(followerViewModel);
        }
        public async Task<IActionResult> Following(string username)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            FollowViewModel followingViewModel = await _userRepository.GetFollowingForProfileAsync(username,currentUserId);
            return View(followingViewModel);
        }
        public async Task<IActionResult> EditProfile()
        {
            var currentUser = await _userRepository.Users.FirstOrDefaultAsync(u => u.UserId == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            var model = new EditProfileViewModel
            {
                Username = currentUser.Username,
                Name = currentUser.Name,
                Email = currentUser.Email,
                Bio = currentUser.Bio,
                Location = currentUser.Location,
                BirthDate = currentUser.BirthDate,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var currentUser = await _userRepository.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (currentUser != null)
                {
                    var updateUser = await _userRepository.Users.FirstOrDefaultAsync(x => x.Username == model.Username || x.Email == model.Email);
                    if(updateUser == null || (currentUser.Email == updateUser.Email && currentUser.Username == updateUser.Username))
                    {
                        _userRepository.UpdateUser(model,currentUser);
                    }
                    else
                    {
                        ModelState.AddModelError("","Username or Email already exists");
                        return View("EditProfile", model);
                    }
                    
                }
                return Redirect("/"+currentUser.Username); 
            }
            return View("EditProfile", model);
        }
        
    }
}