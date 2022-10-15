namespace Auth_API.Models.Dtos
{
    public class UserDto
    {
        public string UserEmail { get; set; }
        public byte[] PasswordHash { get; set; }
    }
}
