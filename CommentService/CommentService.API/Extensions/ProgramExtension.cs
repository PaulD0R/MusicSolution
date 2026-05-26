using System.Text;
using CommentService.API.Exceptions;
using CommentService.Application.Interfaces.Caches;
using CommentService.Application.Interfaces.Factories;
using CommentService.Application.Interfaces.Messages;
using CommentService.Application.Interfaces.Repositories;
using CommentService.Application.Interfaces.Services;
using CommentService.Application.Options;
using CommentService.Application.Services;
using CommentService.Domain.Events;
using CommentService.Infrastructure.Context;
using CommentService.Infrastructure.EventHandlers;
using CommentService.Infrastructure.Factories;
using CommentService.Infrastructure.Kafka;
using CommentService.Infrastructure.Options;
using CommentService.Infrastructure.Redis;
using CommentService.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using Serilog;
using Xabe.FFmpeg;

namespace CommentService.API.Extensions;

public static class ProgramExtension
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

            FFmpeg.SetExecutablesPath(configuration["FFmpeg:Path"]);
            
            return services;
        }

        public IServiceCollection AddLoggers(IConfigurationSection configuration)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
                .WriteTo.File(configuration["Path"]!, rollingInterval: RollingInterval.Hour)
                .CreateLogger();
                
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
            services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            services.AddTransient<INotificationHandler<CommentDeleteEvent>, CommentDeleteEventHandler>();
            
            services.Configure<CommentFileOptions>(configuration.GetSection("Comment"));

            services.AddSingleton<IFactory<IPersonService>, PersonServiceFactory>();
            services.AddSingleton<IFactory<ICommentService>, CommentServiceFactory>();

            services.AddScoped<ICachingService, RedisService>();

            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();

            services.AddScoped<ICommentFileService, CommentFileService>();
            services.AddScoped<ICommentService, Application.Services.CommentService>();
            services.AddScoped<IPersonService, PersonService>();
            
            services.AddConsumer<PersonCreateEvent, PersonCreateEventHandler>(configuration.GetSection("Kafka:PersonCreated"));
            services.AddConsumer<PersonDeleteEvent, PersonDeleteEventHandler>(configuration.GetSection("Kafka:PersonDeleted"));
            
            services.AddConsumer<MusicDeleteEvent, MusicDeleteEventHandler>(configuration.GetSection("Kafka:MusicDeleted"));

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

        private void AddConsumer<TMessage, THandler>(IConfigurationSection configuration)
            where THandler : class, IMessageHandler<TMessage>
        {
            services.Configure<KafkaConsumerOptions>(typeof(TMessage).Name, configuration);
            services.AddHostedService<KafkaConsumer<TMessage>>();
            services.AddSingleton<IMessageHandler<TMessage>, THandler>();
        }
    }
}