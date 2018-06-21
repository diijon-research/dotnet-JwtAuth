namespace LoginService.Modules.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole UserRole { get; set; }        
    }
}
