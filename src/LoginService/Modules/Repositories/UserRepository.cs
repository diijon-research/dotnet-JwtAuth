using System.Collections.Generic;
using System.Linq;
using JwtAuth.LoginService.Modules.Models;

namespace JwtAuth.LoginService.Modules.Repositories
{
    public class UserRepository
    {
        public List<User> TestUsers { get; private set; }
        public UserRepository()
        {
            TestUsers = new List<User>
            {
                new User
                {
                    Username = "Admin", Password = "Pass", UserRole = new[] {UserRole.ADMIN, UserRole.NORMAL}
                },
                new User
                {
                    Username = "User", Password = "Pass", UserRole = new[] {UserRole.NORMAL}
                }
            };
        }
        public User GetUser(string username)
        {
            try
            {
                return TestUsers.First(user => user.Username.Equals(username));
            } catch
            {
                return null;
            }
        }
    }
}
