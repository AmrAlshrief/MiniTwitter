using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MiniTwitter.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // TODO: Implement email sending logic
            return Task.CompletedTask;
        }
    }
}
