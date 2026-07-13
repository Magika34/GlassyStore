using System.Diagnostics;

namespace GlassyStore.Services.Email
{
    public class EmailService : IEmailService
    {
        public Task SendEmail(string to, string subject, string body)
        {
            Debug.WriteLine("========== EMAIL ==========");
            Debug.WriteLine($"To: {to}");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine(body);
            Debug.WriteLine("===========================");

            return Task.CompletedTask;
        }
    }
}