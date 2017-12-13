namespace Meetup.WebApi.Commands
{
    public class RegisterUserCommand
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
