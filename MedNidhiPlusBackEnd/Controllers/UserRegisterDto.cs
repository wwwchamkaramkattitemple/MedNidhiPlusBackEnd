namespace MedNidhiPlusBackEnd.API.Controllers;

public class UserRegisterDto
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string Role { get; set; } = "Staff";
}
