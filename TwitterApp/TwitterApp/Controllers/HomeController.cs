using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View();
    }
    
    

}
