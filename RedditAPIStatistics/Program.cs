using Models;
using PostConsumerService;
using PostPublisherService;
using RedditAPIStatistics.Infrastructure.Extensions;
using RedditAPIStatistics.Infrastructure.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.SetBasePath(Directory.GetCurrentDirectory());
    config.AddJsonFile("AppConfig/appsettings.json", optional: false, reloadOnChange: true);
});

// Add services to the container.
builder.Services.AddOptions();
builder.Services.Configure<RedditApiCredentials>(builder.Configuration.GetSection("RedditApiCredentials"));
builder.Services.AddControllers()
    .AddMvcOptions(options =>
    {
        options.Filters.Add(typeof(GlobalExceptionFilter));
    });
builder.Services.AddCoreDependencies();
builder.Services.AddBusinessLayerDependencies();

builder.Services.AddHostedService<PostsPublisherService>();
builder.Services.AddHostedService<PostsConsumerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
