using Feeds.Models;

namespace Feeds.Services;

public interface IMailService
{
    Task SendEmailAsync(MailRequest mailRequest);
}