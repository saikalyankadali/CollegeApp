namespace CollegeApp.Models
{
    public class LoginUserResult
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string RoleName { get; set; }
    }
}