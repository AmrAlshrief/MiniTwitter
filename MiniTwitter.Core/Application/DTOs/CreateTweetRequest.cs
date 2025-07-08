using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Application.DTOs
{
    public class CreateTweetRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}
