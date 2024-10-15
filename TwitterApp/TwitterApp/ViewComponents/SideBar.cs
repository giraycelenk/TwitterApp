using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TwitterApp.Data.Abstract;
using TwitterApp.Models;

namespace TwitterApp.ViewComponents
{
    public class SideBar:ViewComponent
    {
        private readonly IUserRepository _userRepository;
        public SideBar(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IViewComponentResult Invoke()
        {
            var claimsPrincipal = User as ClaimsPrincipal;
            var userIdClaim = Convert.ToInt32(claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var user = _userRepository.GetUserById(userIdClaim);

            return View(user);
        }
    }
}