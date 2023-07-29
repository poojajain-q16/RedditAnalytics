using AutoMapper;
using BusinessLogic.AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.Implementation;
using EventBus;
using HttpSideCar;
using Models;
using PostConsumerService;
using PostPublisherService;

namespace RedditAPIStatistics.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        {
            services.AddSingleton<RedditPostDataStore>();
            services.AddSingleton<IEventBus, InMemoryEventBus>();
            services.AddSingleton<RateLimitingService>();
            services.AddSingleton<AuthenticationService>();
            services.AddScoped<PostsPublisherService>();
            services.AddScoped<PostsConsumerService>();

            return services;
        }

        public static IServiceCollection AddBusinessLayerDependencies(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IRedditStatisticsService, RedditStatisticsService>();
            return services;
        }
    }
}
