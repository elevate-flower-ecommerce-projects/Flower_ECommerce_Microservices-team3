using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure.Settings
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
