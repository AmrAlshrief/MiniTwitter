using System;

namespace MiniTwitter.Core.Application.DTOs;

public class UserWithRoleDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? ProfileImage { get; set; }
}
