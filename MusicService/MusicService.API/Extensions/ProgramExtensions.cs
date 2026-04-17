using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicService.API.Exceptions;
using MusicService.API.Options;
using MusicService.API.Sockets;
using MusicService.Application.Interfaces.Caching;
using MusicService.Application.Interfaces.Factories;
using MusicService.Application.Interfaces.Messages;
using MusicService.Application.Interfaces.Repositories;
using MusicService.Application.Interfaces.Services;
using MusicService.Application.Options;
using MusicService.Application.Services;
using MusicService.Domain.Events;
using MusicService.Infrastructure.Caching;
using MusicService.Infrastructure.Contexts;
using MusicService.Infrastructure.Factories;
using MusicService.Infrastructure.Kafka;
using MusicService.Infrastructure.Options;
using MusicService.Infrastructure.Repositories;
using OpenTelemetry.Metrics;

namespace MusicService.API.Extensions;

public static class ProgramExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddAppInfrastructure(IConfiguration configuration)
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("Postgres")));

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration.GetConnectionString("Redis");
                    options.InstanceName = configuration["Redis:InstanceName"];
                });

                return services;
            }

            public IServiceCollection AddAppTelemetry()
            {
                services.AddOpenTelemetry()
                    .WithMetrics(metrics => metrics
                        .AddAspNetCoreInstrumentation()
                        .AddPrometheusExporter());

                return services;
            }
            
            public IServiceCollection AddSecurityConfiguration(IConfigurationSection jwtConfig)
            {
                services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = jwtConfig["Issuer"],
                            ValidateAudience = true,
                            ValidAudience = jwtConfig["Audience"],
                            ValidateLifetime = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Secret"]!)),
                            ValidateIssuerSigningKey = true
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                context.Token = context.Request.Cookies["jwt"];
                                return Task.CompletedTask;
                            }
                        };
                    });

                services.AddAuthorization();
                return services;
            }

            public IServiceCollection AddBusinessServices(IConfiguration configuration)
            {
                services.Configure<MusicFileOptions>(configuration.GetSection("Music"));
                services.Configure<SocketOptions>(configuration.GetSection("Socket"));

                services.AddSingleton<IFactory<IMusicService>, MusicServiceFactory>();
                
                services.AddScoped<ICachingService, RedisService>();
                
                services.AddScoped<IMusicDataRepository, MusicDataRepository>();
                services.AddScoped<ILikeRepository, LikeRepository>();
                
                services.AddScoped<IMusicService, Application.Services.MusicService>();
                services.AddScoped<IMusicFileService, MusicFileService>();
                services.AddScoped<ILikeService, LikeService>();

                services.AddProducer<MusicCreateEvent>(configuration.GetSection("Kafka:MusicCreated"));
                services.AddProducer<MusicDeleteEvent>(configuration.GetSection("Kafka:MusicDeleted"));
                
                services.AddHostedService<MusicSocketBackgroundService>();

                return services;
            }

            public IServiceCollection AddWebPresentation(IConfiguration configuration)
            {
                services.AddControllers();
                services.AddProblemDetails();
                services.AddExceptionHandler<ExceptionHandler>();
            
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod()
                        .AllowAnyHeader());
                    options.AddPolicy(
                        "YarpPolice", 
                        policy => policy
                            .WithOrigins(configuration.GetConnectionString("Yarp") ?? string.Empty)
                            .AllowAnyMethod().AllowAnyHeader());
                });

                return services;
            }
            
            private void AddProducer<TMessage>(IConfigurationSection configuration)
            {
                services.Configure<KafkaProducerOptions>(typeof(TMessage).Name, configuration);
                services.AddSingleton<IMessageProducer<TMessage>, KafkaProducer<TMessage>>();
            }

            private void AddConsumer<TMessage, THandler>(IConfigurationSection configuration)
                where THandler : class, IMessageHandler<TMessage>
            {
                services.Configure<KafkaConsumerOptions>(typeof(TMessage).Name, configuration);
                services.AddHostedService<KafkaConsumer<TMessage>>();
                services.AddSingleton<IMessageHandler<TMessage>, THandler>();
            }
    }
}