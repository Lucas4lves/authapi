namespace AuthApi.Persistence;

class UserDTO
{
    public string Name {get; set;}
    public string Email {get; set;}

    public UserDTO(string name, string email)
    {
        Name = name;
        Email = email;
    }

}