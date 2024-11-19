using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;
namespace EventManagament.Service
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com"; // Địa chỉ SMTP server
        private readonly int _smtpPort = 587; // Cổng SMTP (587 cho TLS)
        private readonly string _smtpUser = "chuhaist123@gmail.com"; // Địa chỉ email gửi
        private readonly string _smtpPass = "eoamhxpofpyvocmz"; // Mật khẩu email

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("TDMU Event", _smtpUser));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, false);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}