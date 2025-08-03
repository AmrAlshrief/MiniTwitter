using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string ProfilePictureUrl { get; set; } = null!;
    }
}
