using RESTAPITemplate.Models;

namespace CheckStatusApi.EmailServices
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
