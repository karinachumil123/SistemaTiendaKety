using TiendaKeytlin.Server.Models;

namespace TiendaKeytlin.Server.Models
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

public class EmailConfiguration
{
    public SmtpSettings SmtpSettings { get; set; }
}