namespace webapi.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PhoneNumber { get; set;}

    }
}

public record UserDto(int Id, string Name, string Email, string PhoneNumber);
public record CreateUserDto(string Name,  string Email, string Password, string PhoneNumber);
public record UpdateUserDto(string Email, string Password, string PhoneNumber);