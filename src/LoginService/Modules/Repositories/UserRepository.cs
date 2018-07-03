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
            TestUsers = new List<User>();
            TestUsers.Add(new User() { Username = "Test1", Password  = "Pass1"});
            TestUsers.Add(new User() { Username = "Test2", Password = "Pass2"});
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
