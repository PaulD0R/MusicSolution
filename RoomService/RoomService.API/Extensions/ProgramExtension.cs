using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using RoomService.API.Exceptions;
using RoomService.Application.Interfaces.Factories;
using RoomService.Application.Interfaces.Messages;
using RoomService.Application.Interfaces.Repositories;
using RoomService.Application.Interfaces.Services;
using RoomService.Application.Services;
using RoomService.Domain.Events;
using RoomService.Infrastructure.Context;
using RoomService.Infrastructure.Factories;
using RoomService.Infrastructure.Kafka;
using RoomService.Infrastructure.Kafka.Handlers;
using RoomService.Infrastructure.Options;
using RoomService.Infrastructure.Repositories;
using Serilog;

namespace RoomService.API.Extensions;

public static class ProgramExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAppInfrastructure(IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres")));

            return services;
        }

        public IServiceCollection AddBusinessServices(IConfiguration configuration)
        {
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();

            services.AddScoped<IRoomService, Application.Services.RoomService>();
            services.AddScoped<IPersonService, PersonService>();

            services.AddSingleton<IFactory<IPersonService>, PersonServiceFactory>();
            
            services.AddConsumer<PersonCreateEvent, PersonCreateEventHandler>
                (configuration.GetRequiredSection("Kafka:PersonCreated"));
            services.AddConsumer<PersonDeleteEvent, PersonDeleteEventHandler>
                (configuration.GetRequiredSection("Kafka:PersonDeleted"));
            services.AddConsumer<PersonUpdateEvent, PersonUpdateEventHandler>
                (configuration.GetRequiredSection("Kafka:PersonUpdated"));
            
            return services;
        }

        public IServiceCollection AddLoggers(IConfigurationSection configuration)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
                .WriteTo.File(configuration["Path"]!, rollingInterval: RollingInterval.Hour)
                .CreateLogger();
                
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
                    var rsa = RSA.Create();
                    rsa.ImportFromPem(jwtConfig["PublicKey"]);
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtConfig["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwtConfig["Audience"],
                        ValidateLifetime = true,
                        IssuerSigningKey = new RsaSecurityKey(rsa),
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
        
        public IServiceCollection AddAppTelemetry()
        {
            services.AddOpenTelemetry()
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddPrometheusExporter());

            return services;
        }

        public IServiceCollection AddWebPresentation(IConfiguration configuration)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddSignalR();      

            services.AddProblemDetails();
            services.AddExceptionHandler<ExceptionHandler>();

            services.AddCors(options =>
            {
                options.AddPolicy(
                    "YarpPolice", 
                    policy => policy
                        .WithOrigins(configuration.GetSection("Yarp")["Path"] ?? string.Empty)
                        .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
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