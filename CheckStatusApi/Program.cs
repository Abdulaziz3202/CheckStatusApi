using CheckStatusApi;
using CheckStatusApi.EmailServices;
using CheckStatusApi.EmailServices.Dto;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configuration for SMTP and API URL
builder.Services.Configure<MonitorConfig>(builder.Configuration.GetSection("MonitorConfig"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddHttpClient();
builder.Services.AddTransient<CheckApiStatusJob>();
builder.Services.AddSingleton<IMailService, MailService>();

// Configure Hangfire with in-memory storage
builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();

var app = builder.Build();

// Set up Hangfire Dashboard (optional, useful for debugging)
app.UseHangfireDashboard();

// Schedule the job
RecurringJob.AddOrUpdate<CheckApiStatusJob>(
    "CheckApiStatusJob",
    job => job.CheckApiStatus(),
    Cron.Minutely);

app.Run();

public class MonitorConfig
{
    public string ApiUrl { get; set; }
    public string AdminEmail { get; set; }
    public string SmtpEmail { get; set; }
    public string SmtpPassword { get; set; }
}
