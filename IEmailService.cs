namespace GlassyStore.Services.Email
{
    public interface IEmailService
    {
        Task SendEmail(string to, string subject, string body);
    }
}