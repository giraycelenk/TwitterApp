using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TwitterApp.Data.Abstract;
using TwitterApp.Entity;
using TwitterApp.Models;
using TwitterApp.ViewComponents;

namespace TwitterApp.Data.Concrete.EfCore
{
    public class EfUserRepository : IUserRepository
    {
        private TwitterContext _context;
        public EfUserRepository(TwitterContext context)
        {
            _context = context;
        }

        public IQueryable<User> Users => _context.Users;

        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUserById(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            return user ?? new();
        }
        public int GetIdByUsername(string userName)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == userName);
            if(user != null)
            {
                return user.UserId;
            }
            return 0;
        }
        public List<Tweet> GetTweetsByUserId(int userId)
        {
            var tweets = _context.Tweets 
                    .Include(t => t.Likes)
                    .Include(t => t.Retweets)
                    .Where(t => t.UserId == userId) 
                    .OrderByDescending(t => t.TweetDate) 
                    .ToList();
            
            return tweets ?? new List<Tweet>(); 
        }
        

        public List<User> GetFollowers(int userId)
        {
            var userFollows = _context
                        .UserFollows
                        .Where(uf => uf.FollowingUserId == userId)
                        .ToList();

            return userFollows.Select(uf => uf.FollowerUser).ToList();
        }
        public async Task<FollowViewModel> GetFollowersForProfileAsync(string username,int userId)
        {
            var currentUser = await _context.Users
                                    .Include(u => u.Followers)
                                    .Include(u => u.Following)
                                    .FirstOrDefaultAsync(x => x.UserId == userId);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var userFollows = _context
                        .UserFollows
                        .Where(uf => uf.FollowingUserId == user.UserId)
                        .ToList();
            List<User> followers = new List<User>();
            Dictionary<int,bool> isFollower = new Dictionary<int,bool>();
            Dictionary<int,bool> isFollowing = new Dictionary<int,bool>();
            foreach(var userFollow in userFollows)
            {
                followers.Add
                (
                    await _context.Users.FirstOrDefaultAsync(u => u.UserId == userFollow.FollowerUserId)
                );
                isFollower[userFollow.FollowerUserId] = currentUser.Followers.Any(f => f.FollowerUserId == userFollow.FollowerUserId);
                isFollowing[userFollow.FollowerUserId] = currentUser.Following.Any(f => f.FollowingUserId == userFollow.FollowerUserId);

            }
        
            return new FollowViewModel {
                User = user,
                CurrentUser = currentUser,
                Followers = followers,
                IsFollowing = isFollowing,
                IsFollower = isFollower
            };
        }
        public async Task<FollowViewModel> GetFollowingForProfileAsync(string username,int userId)
        {
            var currentUser = await _context.Users
                                    .Include(u => u.Followers)
                                    .Include(u => u.Following)
                                    .FirstOrDefaultAsync(x => x.UserId == userId);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var userFollows = _context
                        .UserFollows
                        .Where(uf => uf.FollowerUserId == user.UserId)
                        .ToList();
            List<User> following = new List<User>();
            Dictionary<int,bool> isFollower = new Dictionary<int,bool>();
            Dictionary<int,bool> isFollowing = new Dictionary<int,bool>();
            foreach(var userFollow in userFollows)
            {
                following.Add
                (
                    await _context.Users.FirstOrDefaultAsync(u => u.UserId == userFollow.FollowingUserId)
                );
                isFollower[userFollow.FollowingUserId] = currentUser.Followers.Any(f => f.FollowerUserId == userFollow.FollowingUserId);
                isFollowing[userFollow.FollowingUserId] = currentUser.Following.Any(f => f.FollowingUserId == userFollow.FollowingUserId);

            }
        
            return new FollowViewModel {
                User = user,
                CurrentUser = currentUser,
                Following = following,
                IsFollowing = isFollowing,
                IsFollower = isFollower
            };
        }
        public List<User> GetFollowings(int userId)
        {
            var userFollows = _context
                                .UserFollows
                                .Where(uf => uf.FollowerUserId == userId)
                                .ToList();

            return userFollows.Select(uf => uf.FollowingUser).ToList();
        }
        public async Task<bool> IsFollowing(int followerUserId, int followingUserId)
        {
            return await _context
                        .UserFollows
                        .AnyAsync(uf => uf.FollowerUserId == followerUserId && uf.FollowingUserId == followingUserId);
        }

        public async Task<bool> FollowUserAsync(int currentUserId, int userIdToFollow)
        {
            
            var existingFollow = await _context
                                        .UserFollows
                                        .FirstOrDefaultAsync(f => f.FollowerUserId == currentUserId && f.FollowingUserId == userIdToFollow);
            
            if (existingFollow != null)
            {
                return false; 
            }

            var newFollow = new UserFollow
            {
                FollowerUserId = currentUserId,
                FollowingUserId = userIdToFollow
            };

            await _context.UserFollows.AddAsync(newFollow); 
            await _context.SaveChangesAsync();

            return true; 
        }

        public async Task<bool> UnfollowUserAsync(int currentUserId, int userIdToUnfollow)
        {
            var followRelation = await _context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerUserId == currentUserId && f.FollowingUserId == userIdToUnfollow);
            
            if (followRelation != null)
            {
                _context.UserFollows.Remove(followRelation);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<UserFollow> GetFollowAsync(int followerUserId, int followingUserId)
        {
            return await _context.UserFollows
                                .FirstOrDefaultAsync(f => f.FollowerUserId == followerUserId && f.FollowingUserId == followingUserId);
        }

        public async Task<ProfileViewModel> GetProfileByUserNameAsync(int userId,string username,string tab)
        {
            var currentUser = await _context.Users
                                    .Include(u => u.Tweets)
                                    .Include(u => u.Likes)
                                    .Include(u => u.Retweets)
                                    .Include(u => u.Mentions)
                                    .Include(u => u.Followers)
                                    .Include(u => u.Following)
                                    .FirstOrDefaultAsync(x => x.UserId == userId);
            var user = await _context.Users
                                    .Include(u => u.Tweets)
                                    .Include(u => u.Likes)
                                    .Include(u => u.Retweets)
                                    .Include(u => u.Mentions)
                                    .Include(u => u.Followers)
                                    .Include(u => u.Following)
                                    .FirstOrDefaultAsync(x => x.Username == username);
            var isFollowing = currentUser.Following.Any(f => f.FollowingUserId == user.UserId); 
            var isFollower = currentUser.Followers.Any(f => f.FollowerUserId == user.UserId); 
            return new ProfileViewModel
            {
                User = user,
                CurrentUser = currentUser,
                IsFollowing = isFollowing,
                IsFollower = isFollower,
                Tab = tab
            };
        }
        public async Task UpdateUser(EditProfileViewModel Model, User User)
        {
            if(User != null)
            {
                User.Username = Model.Username;
                User.Name = Model.Name;
                User.Email = Model.Email;
                User.Bio = Model.Bio;
                User.BirthDate = Model.BirthDate;
                User.Location = Model.Location;
                if(Model.ProfileImage != null)
                {
                    var userFolder = Path.Combine("wwwroot/img/profileimg", User.Username);
                    if (!Directory.Exists(userFolder))
                    {
                        Directory.CreateDirectory(userFolder);
                    }

                    var fileExtension = Path.GetExtension(Model.ProfileImage.FileName);
                    var fileName = string.Format($"{Guid.NewGuid()}{fileExtension}");
                    var filePath = Path.Combine(userFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Model.ProfileImage.CopyToAsync(stream);
                    }
    	            var ImageUrl = "profileimg/"+User.Username+"/"+fileName;
                    User.ImageUrl = ImageUrl;
                }
                if(Model.ProfileHeaderImage != null)
                {
                    var userFolder = Path.Combine("wwwroot/img/profileheaderimg", User.Username);
                    if (!Directory.Exists(userFolder))
                    {
                        Directory.CreateDirectory(userFolder);
                    }

                    var fileExtension = Path.GetExtension(Model.ProfileHeaderImage.FileName);
                    var fileName = string.Format($"{Guid.NewGuid()}{fileExtension}");
                    var filePath = Path.Combine(userFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Model.ProfileHeaderImage.CopyToAsync(stream);
                    }
    	            var HeaderImageUrl = "profileheaderimg/"+User.Username+"/"+fileName;
                    User.HeaderImageUrl = HeaderImageUrl;
                }
                _context.Entry(User).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

    }
}