using TwitterApp.Data.Abstract;
using TwitterApp.Entity;

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
    }
}