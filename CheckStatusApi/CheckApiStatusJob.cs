using System.Net.Mail;
using System.Net;
using CheckStatusApi.EmailServices;
using RESTAPITemplate.Models;
using CheckStatusApi.EmailServices.Dto;

namespace CheckStatusApi
{
    public class CheckApiStatusJob
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMailService _mailService;

        public CheckApiStatusJob(IConfiguration configuration, IHttpClientFactory httpClientFactory, IMailService mailService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _mailService= mailService;
        }

        public async Task CheckApiStatus()
        {
            var config = _configuration.GetSection("MonitorConfig").Get<MonitorConfig>();
            var emailConfig = _configuration.GetSection("MailSettings").Get<MailSettings>();

            if (config == null || string.IsNullOrEmpty(config.ApiUrl) || string.IsNullOrEmpty(config.AdminEmail))
            {
                Console.WriteLine("Configuration is missing");
                return;
            }

            var mailRequest = new MailRequest
            {
                Subject = "URGENT CAUTION",
                ToEmail= config.AdminEmail,
                Body= "Please do needed steps, due the financial service system is down!!!"
            };

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(config.ApiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    await _mailService.SendEmailAsync(mailRequest);
                    Console.WriteLine("API is down. Notification sent to admin.");
                }
                else
                {
                    Console.WriteLine("API is up and running.");
                }
            }
            catch
            {
                await _mailService.SendEmailAsync (mailRequest);
                Console.WriteLine("API is down. Notification sent to admin.");
            }
        }

       
    }
}
