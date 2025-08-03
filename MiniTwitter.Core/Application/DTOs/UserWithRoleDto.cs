using System;

namespace MiniTwitter.Core.Application.DTOs;

public class UserWithRoleDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    //public string Name => $"{FirstName} {LastName}".Trim();
    public string Name { get; set;}
    public string Email { get; set; } = null!;
    public string Bio { get; set; }
    public string Role { get; set; } = null!;
    public string? ProfileImage { get; set; }
}
