using TwitterApp.Entity;

namespace TwitterApp.Data.Abstract
{
    public interface IUserRepository
    {
        IQueryable<User> Users {get;}
        void CreateUser(User User);
    }
}